using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; 

public class Timer : MonoBehaviour
{
    public TextMeshProUGUI timerText; // UI text to display the timer
    private float startTime;
    private bool isRunning;

    void Start()
    {
        startTime = Time.time; 
        isRunning = true; 
    }

    void Update()
    {
        if (isRunning)
        {
            float t = Time.time - startTime; // Calculate elapsed time

            string minutes = ((int)t / 60).ToString();
            string seconds = ((int)t % 60).ToString();

            timerText.text = $"{minutes} : {seconds}"; 
        }
    }

    public void StopTimer()
    {
        isRunning = false;
    }
}
