using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Metronome : Beat
{
    AudioSource source;

    private void Awake()
    {
        source = GetComponent<AudioSource>();
        source.Stop();
    }

    public override void OnBeat()
    {
        //StartCoroutine(Beep());
    }

    IEnumerator Beep()
    {
        source.Play();
        yield return new WaitForSeconds(.2f);
        source.Stop();
    }

}
