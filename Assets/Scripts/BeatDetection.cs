using System;
using System.Collections.Generic;
using System.Linq;
using NAudio.Dsp;
using NAudio.Wave;
using UnityEditor;
using UnityEngine;
using System.IO;

[Serializable]
public class BeatData
{
    public List<double> beats;

    public BeatData(List<double> beats)
    {
        this.beats = beats;
    }

    public BeatData()
    {
        this.beats = new List<double>();
    }

    public void AddBeat(double time)
    {
        if (!beats.Contains(time))
        {
            this.beats.Add(time);
        }
    }
}

public class BeatDetection : EditorWindow
{
    [MenuItem("Audio Processing/Beat Detection")]
    public static void OpenWindow()
    {
        BeatDetection window = (BeatDetection)EditorWindow.GetWindow(typeof(BeatDetection));
        window.Show();
    }


    // Define variables to track threshold and energy history
    private static double[] thresholds;
    private static List<double> energyHistory = new List<double>();

    public static void DetectBeats(string audioFilePath, int numBands, int fftLength, double thresholdAdjustmentFactor, double minInterval, int ThresholdWindowSize, double SmoothingFactor)
    {
        Debug.Log(Path.GetFileName(audioFilePath));
        double lastBeatTime = -1;
        BeatData data = new BeatData();
        // Read audio file
        using (var audioFile = new AudioFileReader(Application.streamingAssetsPath + "/" + Path.GetFileName(audioFilePath)))
        {
            //Get the sample rate to help us find the time later
            int sampleRate = audioFile.WaveFormat.SampleRate;

            //The number of samples that have been processed, also for time purposes
            int processedNum = 0;

            // Allocate memory for the samples
            float[] buffer = new float[audioFile.Length];
            Debug.Log(buffer.Length);
            int samplesRead = audioFile.Read(buffer, 0, buffer.Length);

            // Apply FFT to the samples
            Complex[] fftBuffer = new Complex[fftLength];
            for (int i = 0; i < samplesRead / fftLength; i++)
            {
                // Fill our FFT buffer with samples. For simplicity, we only have 1 real component, but FFT requires it to be complex
                for (int j = 0; j < fftLength; j++)
                {
                    // Yay, complex numbers
                    Complex newComplex = new Complex();
                    newComplex.X = buffer[i * fftLength + j];
                    newComplex.Y = 0;
                    fftBuffer[j] = newComplex;
                }

                // Perform FFT
                FastFourierTransform.FFT(true, (int)Math.Log(fftLength, 2), fftBuffer);

                // Calculate energy in frequency bands
                double[] bandEnergy = CalculateBandEnergy(fftBuffer, numBands, fftLength);

                // Add the average energy to the energy history for threshold smoothing
                double sum = 0;
                foreach (var num in bandEnergy)
                {
                    sum += num;
                }
                energyHistory.Add(sum /= bandEnergy.Length);

                // Calculate dynamic threshold
                double dynamicThreshold = CalculateDynamicThreshold(bandEnergy, SmoothingFactor);

                // Detect beats based on energy and threshold
                for (int j = 0; j < numBands; j++)
                {
                    if (bandEnergy[j] > dynamicThreshold * thresholdAdjustmentFactor)
                    {
                        //Find the time from the sampleRate and the amount of samples taken
                        double currentTime = (double)processedNum / (double)sampleRate / 2;
                        if (currentTime - lastBeatTime >= minInterval)
                        {
                            // Add the beat to the data
                            data.AddBeat(currentTime);

                            // Update the time of the last detected beat
                            lastBeatTime = currentTime;
                        }
                    }

                }
                //Increment the amount of samples we have processed
                processedNum += fftLength;

                // Update energy history. This assumes that the adjustment is done in the if, otherwise we end up with everything exponentially increasing
                energyHistory.Add(dynamicThreshold);
                if (energyHistory.Count > ThresholdWindowSize)
                {
                    // Remove the first number if it is past our value range
                    energyHistory.RemoveAt(0); 
                }
            }
        }
        File.WriteAllText(Application.streamingAssetsPath + "/" + Path.GetFileNameWithoutExtension(audioFilePath) + ".json", JsonUtility.ToJson(data));
    }

    private static double[] CalculateBandEnergy(Complex[] fftBuffer, int numBands, int fftLength)
    {
        double[] bandEnergy = new double[numBands];
        int samplesPerBand = fftLength / numBands;
        for (int j = 0; j < numBands; j++)
        {
            // Add all of the magnitudes together
            for (int k = 0; k < samplesPerBand; k++)
            {
                bandEnergy[j] += Math.Sqrt(Math.Pow(fftBuffer[j * samplesPerBand + k].X, 2) + Math.Pow(fftBuffer[j * samplesPerBand + k].Y, 2));
            }
            // Average it baby
            bandEnergy[j] /= samplesPerBand;
        }
        return bandEnergy;
    }

    private static double CalculateDynamicThreshold(double[] bandEnergy, double SmoothingFactor)
    {
        // Calculate average energy over the recent history
        double sum = 0;
        foreach (var energy in energyHistory)
        {
            sum += energy;
        }
        double averageEnergy = energyHistory.Count > 0 ? sum / energyHistory.Count : 0;

        // dynamic threshold is just the average energy
        double dynamicThreshold = averageEnergy;

        // Smooth the threshold using exponential smoothing
        double smoothedThreshold = (dynamicThreshold * SmoothingFactor) + (thresholds != null ? thresholds.Average() * (1 - SmoothingFactor) : 0);


        // Update thresholds array for smoothing
        if (thresholds == null || thresholds.Length != bandEnergy.Length)
        {
            thresholds = new double[bandEnergy.Length];
        }
        Array.Fill(thresholds, smoothedThreshold); // Fill array with smoothed threshold value

        return smoothedThreshold;
    }

    string audioFilePath = "Put Your Audio File Path Here";
    int numBands = 64;
    int fftLength = 1024;
    double thresholdAdjustmentFactor = 10;
    double minInterval = 0.1;

    int thresholdWindow = 100;
    double smoothing = 0.1;
    public void OnGUI()
    {
        audioFilePath = EditorGUILayout.TextField("File Name", audioFilePath);

        numBands = EditorGUILayout.IntField("Number of Bands", numBands);

        fftLength = EditorGUILayout.IntField("Length of FFT", fftLength);

        thresholdAdjustmentFactor = EditorGUILayout.DoubleField("Threshold Adjustment Factor", thresholdAdjustmentFactor);

        minInterval = EditorGUILayout.DoubleField("Minimum Interval Between Beats", minInterval);

        thresholdWindow = EditorGUILayout.IntField("Smoothing Window", thresholdWindow);

        smoothing = EditorGUILayout.DoubleField("Smoothing Factor", smoothing);

        if (GUILayout.Button("Detect Beats"))
        {
            DetectBeats(audioFilePath, numBands, fftLength, thresholdAdjustmentFactor, minInterval, thresholdWindow, smoothing);
        }
    }
}
