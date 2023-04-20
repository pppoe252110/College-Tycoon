using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "TimePreset", menuName = "ScriptableObjects/TimePreset", order = 3)]
public class TimePreset : ScriptableObject
{
    public Period[] periods;
}
[Serializable]
public class Period
{
    public string startTime = "8:30";
    [Space]
    public string endTime = "10:00";
    [Space]
    public PeriodAction action;
}
public enum PeriodAction
{
    Sleep, Study, FreeTime
}