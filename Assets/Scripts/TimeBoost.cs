using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeBoost : MonoBehaviour
{
    private bool isPaused = false;
    private float currentTimeScale = 1;
    public Text timeText;
    private void Start()
    {
        UpdateTime();
        Physics.simulationMode = SimulationMode.Script;
    }

    private void Update()
    {
        Physics.Simulate(Time.unscaledDeltaTime);
    }

    public void AddTimeScale(float amount = 1)
    {
        if (isPaused)
        {
            isPaused = false;
            
            return;
        }
        currentTimeScale += amount;
        if (currentTimeScale > 10) 
        {
            currentTimeScale = 1;
        }
        UpdateTime();
    }
    public void SetTimeScale(float t = 1)
    {
        if (isPaused)
        {
            isPaused = false;
        }

        currentTimeScale = t;
        UpdateTime();
    }
    public void UpdateTime()
    {
        if (isPaused)
        {
            Time.timeScale = 0;
            timeText.text = "Пауза";
        }
        else
        {
            Time.timeScale = currentTimeScale;
            timeText.text = $"Ускорение {currentTimeScale}X";
        }

    }
    public void Pause()
    {
        isPaused = !isPaused;
        UpdateTime();
    }
}
