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

    private static int ThresholdWindowSize = 100; // Adjust window size as needed
    private static double SmoothingFactor = 0.1; // Adjust smoothing factor as needed

    // Define variables to track threshold and energy history
    private static double[] thresholds;
    private static List<double> energyHistory = new List<double>();

    public static void DetectBeats(string audioFilePath, int numBands, int fftLength, double thresholdAdjustmentFactor)
    {
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
            int samplesRead = audioFile.Read(buffer, 0, buffer.Length);

            // Apply FFT to the samples
            Complex[] fftBuffer = new Complex[fftLength];
            for (int i = 0; i < samplesRead / fftLength; i++)
            {
                // Fill FFT buffer with samples
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
                double dynamicThreshold = CalculateDynamicThreshold(bandEnergy, thresholdAdjustmentFactor);

                // Detect beats based on energy and threshold
                for (int j = 0; j < numBands; j++)
                {
                    if (bandEnergy[j] > dynamicThreshold)
                    {
                        //Find the time from the sampleRate and the amount of samples taken
                        double currentTime = (double)processedNum / (double)sampleRate / 2;

                        data.AddBeat(currentTime);
                    }
                }
                //Increment the processed sample number by the amount of processed numbers
                processedNum += fftLength;

                // Update energy history
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
            // Calculate energy in the band
            for (int k = 0; k < samplesPerBand; k++)
            {
                bandEnergy[j] += Math.Sqrt(Math.Pow(fftBuffer[j * samplesPerBand + k].X, 2) + Math.Pow(fftBuffer[j * samplesPerBand + k].Y, 2));
            }
            // Normalize energy (optional)
            bandEnergy[j] /= samplesPerBand;
        }
        return bandEnergy;
    }

    private static double CalculateDynamicThreshold(double[] bandEnergy, double thresholdAdjustment)
    {
        // Calculate average energy over the recent history
        double sum = 0;
        foreach (var energy in energyHistory)
        {
            sum += energy;
        }
        double averageEnergy = energyHistory.Count > 0 ? sum / energyHistory.Count : 0;

        // Calculate dynamic threshold (e.g., as a percentage of average energy)
        double dynamicThreshold = averageEnergy * thresholdAdjustment;

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
    int numBands = 16;
    int fftLength = 8192;
    double thresholdAdjustmentFactor = 1.5;
    public void OnGUI()
    {
        audioFilePath = EditorGUILayout.TextField("File Name", audioFilePath);

        numBands = EditorGUILayout.IntField("Number of Bands", numBands);

        fftLength = EditorGUILayout.IntField("Length of FFT", fftLength);

        thresholdAdjustmentFactor = EditorGUILayout.DoubleField("Threshold Adjustment Factor", thresholdAdjustmentFactor);

        if (GUILayout.Button("Detect Beats"))
        {
            DetectBeats(audioFilePath, numBands, fftLength, thresholdAdjustmentFactor);
        }
    }
}
