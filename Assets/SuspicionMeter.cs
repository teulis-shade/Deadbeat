using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SuspicionMeter : MonoBehaviour
{
    [SerializeField] float suspicion;
    float maxSuspicion = 10f;
    bool decreasing = true;
    Coroutine waitToDecrease;
    [SerializeField] float decreasingCooldown;
    // Start is called before the first frame update
    void Start()
    {
        suspicion = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<Image>().fillAmount = suspicion / maxSuspicion;
        if (decreasing)
        {
            suspicion -= maxSuspicion / 1000f;
        }
        if (suspicion < 0f)
        {
            suspicion = 0f;
        }
        if (suspicion >= maxSuspicion)
        {
            //GAME OVER
            suspicion = maxSuspicion;
        }
    }

    public void IncreaseSuspicion(float suspicion)
    {
        Debug.Log(suspicion);
        if (waitToDecrease != null)
        {
            StopCoroutine(waitToDecrease);
        }
        waitToDecrease = StartCoroutine(WaitToDecrease());
        this.suspicion += suspicion;
    }

    IEnumerator WaitToDecrease()
    {
        decreasing = false;
        yield return new WaitForSeconds(decreasingCooldown);
        decreasing = true;
    }
}
