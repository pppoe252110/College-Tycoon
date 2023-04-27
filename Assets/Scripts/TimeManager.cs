using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TimeManager : MonoBehaviour
{
    public TimePreset preset;
    public Text timer;
    public Text actionText;
    public float timeBoost = 60;
    private float time = 43200f;
    public static TimeManager Instance;

    private void Awake()
    {
        Instance=this;
    }

    public PeriodAction GetCurrentAction()
    {
        var current = GetCurrentPeriod();
        if (current == null)
            return PeriodAction.FreeTime;
        return current.action;
    }

    public Period GetCurrentPeriod()
    {
        TimeSpan currentTime = TimeSpan.FromSeconds(time);
        var result = preset.periods.Where(a => TimeSpan.Parse(a.startTime) <= currentTime && currentTime < TimeSpan.Parse(a.endTime)).FirstOrDefault();
        return result;
    }

    void Update()
    {
        switch (GetCurrentAction())
        {
            case PeriodAction.Sleep:
                actionText.text = "Сон";
                break;
            case PeriodAction.Study:
                actionText.text = "Учеба";
                break;
            case PeriodAction.FreeTime:
                actionText.text = "Свободное время";
                break;
        }
        time = Mathf.Repeat(time + Time.deltaTime * timeBoost, 86400f);
        TimeSpan timeSpan = TimeSpan.FromSeconds(time);
        timer.text = timeSpan.ToString(@"hh\:mm");
    }
}
