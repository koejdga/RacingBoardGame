using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stopwatch : MonoBehaviour
{
    public bool TimerActive;
    public float CurrentTime;

    public void Startt()
    {
        CurrentTime = 0;
        TimerActive = true;
    }

    void Update()
    {
        if (TimerActive)
        {
            CurrentTime += Time.deltaTime;
            Debug.Log(CurrentTime);
        }

    }

    public void Stop()
    {
        TimerActive = false;
    }
}
