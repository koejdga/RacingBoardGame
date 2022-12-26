using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public bool isActive;
    public float CurrentTime;

    public void StartTimer(float time)
    {
        CurrentTime = time;
        isActive = true;
    }

    void Update()
    {
        if (isActive)
        {
            CurrentTime -= Time.deltaTime;
            //Debug.Log(CurrentTime);

            if (CurrentTime < 0)
            {
                CurrentTime = 0;
            }
        }
        
    }

    public void Stop()
    {
        isActive = false;
        CurrentTime = 0;
    }
}
