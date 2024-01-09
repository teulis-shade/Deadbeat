using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Routine : Beat
{
    public int numBeats;
    private int currentBeat;

    public void Awake()
    {
        currentBeat = 0;
    }

    public override void OnBeat()
    {
        if (++currentBeat == numBeats)
        {
            currentBeat = 0;
            PerformRoutine();
        }
    }
    public virtual void PerformRoutine()
    {

    }
}
