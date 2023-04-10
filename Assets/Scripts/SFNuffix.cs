using System;
using UnityEngine;

public static class SFNuffix
{
    private static NumbersSuffixHandler suffixHandler;
    static SFNuffix()
    {
        suffixHandler = Resources.Load<NumbersSuffixHandler>("Numbers");
    }

    public static string GetFullValue(double value)
    {
        for (int i = 0; i < suffixHandler.numberSuffixes.Length; i++)
        {
            double pow = Math.Pow(10, suffixHandler.numberSuffixes[i].PowerOfTen);
            if (value < pow)
            {
                if (i - 1 < 0)
                    break;
                double powResult = Math.Pow(10, suffixHandler.numberSuffixes[i-1].PowerOfTen);
                string val = (Math.Floor(value) / powResult).ToString().Split(",")[0];
                return val + suffixHandler.numberSuffixes[i - 1].SuffixAbbreviation;
            }
        }
        return Math.Floor(value).ToString();
    }
}
