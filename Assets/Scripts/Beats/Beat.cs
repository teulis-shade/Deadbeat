using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Beat : MonoBehaviour
{
    //DO NOT OVERRIDE START
    public void Start()
    {
        FindObjectOfType<GameController>().Beat.AddListener(OnBeat);
    }

    public virtual void OnBeat()
    {
        
    }
}
