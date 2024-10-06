using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Utils
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float MapRange(float x, float a_min, float a_max, float b_min, float b_max)
    {
        if (a_min == a_max)
        {
            return b_min;
        }
        return Mathf.Clamp01((x - a_min) / (a_max - a_min)) * (b_max - b_min) + b_min;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float MapRangeUnclamped(float x, float a_min, float a_max, float b_min, float b_max)
    {
        if (a_min == a_max)
        {
            return b_min;
        }
        return (x - a_min) / (a_max - a_min) * (b_max - b_min) + b_min;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float TemporalLerp(float a, float b, float t)
    {
        return Mathf.Lerp(a, b, 1.0f - Mathf.Pow(1.0f - t, Time.deltaTime * 60));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static decimal TemporalLerp(decimal a, decimal b, double t) {
        decimal mix = (decimal) (1.0 - Math.Pow(1.0 - t, Time.deltaTime * 60.0));
        return a * (1.0m - mix) + b * mix;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3 TemporalLerp(Vector3 a, Vector3 b, float t)
    {
        return Vector3.Lerp(a, b, 1.0f - Mathf.Pow(1.0f - t, Time.deltaTime * 60));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Quaternion TemporalLerp(Quaternion a, Quaternion b, float t)
    {
        return Quaternion.Lerp(a, b, 1.0f - Mathf.Pow(1.0f - t, Time.deltaTime * 60));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Quaternion TemporalSlerp(Quaternion a, Quaternion b, float t)
    {
        return Quaternion.Slerp(a, b, 1.0f - Mathf.Pow(1.0f - t, Time.deltaTime * 60));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float FixedTemporalLerp(float a, float b, float t)
    {
        return Mathf.Lerp(a, b, 1.0f - Mathf.Pow(1.0f - t, Time.fixedDeltaTime * 60));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3 FixedTemporalLerp(Vector3 a, Vector3 b, float t)
    {
        return Vector3.Lerp(a, b, 1.0f - Mathf.Pow(1.0f - t, Time.fixedDeltaTime * 60));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Quaternion FixedTemporalLerp(Quaternion a, Quaternion b, float t)
    {
        return Quaternion.Lerp(a, b, 1.0f - Mathf.Pow(1.0f - t, Time.fixedDeltaTime * 60));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Quaternion FixedTemporalSlerp(Quaternion a, Quaternion b, float t)
    {
        return Quaternion.Slerp(a, b, 1.0f - Mathf.Pow(1.0f - t, Time.fixedDeltaTime * 60));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float FloatDist(float a, float b)
    {
        return Mathf.Abs(a - b);
    }

    public static float DecibelsToLinear(float decibels)
    {
        if (float.IsInfinity(decibels))
        {
            return 0.0f;
        }

        return Mathf.Pow(10.0f, decibels / 20.0f);
    }

    public static float LinearToDecibels(float linear)
    {
        float decibels = 20.0f * Mathf.Log10(linear);

        if (float.IsInfinity(decibels))
        {
            return -80.0f;
        }

        return decibels;
    }
}
