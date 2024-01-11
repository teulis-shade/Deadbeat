using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingMetronome : MonoBehaviour
{
    [SerializeField] float extremity;
    bool left;
    float beat;
    float currentBeat;
    bool wasHigh = false;
    public void StartMetronome(float beat)
    {
        transform.rotation = Quaternion.Euler(0, 0, extremity);
        this.beat = beat;
        currentBeat = 0;
        left = true;
    }

    private void Update()
    {
        if (currentBeat % beat < beat / 2 && wasHigh)
        {
            left = !left;
            wasHigh = false;
        }
        if (left)
        {
            transform.rotation = Quaternion.Slerp(Quaternion.Euler(0, 0, extremity), Quaternion.Euler(0, 0, -extremity), (currentBeat % beat) / beat);
        }
        else
        {
            transform.rotation = Quaternion.Slerp(Quaternion.Euler(0, 0, -extremity), Quaternion.Euler(0, 0, extremity), (currentBeat % beat) / beat);
        }
        if (currentBeat % beat > beat / 2)
        {
            wasHigh = true;
        }
        currentBeat += Time.deltaTime;
    }
}
