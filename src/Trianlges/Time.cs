using System;
using System.Diagnostics;

namespace Trianlges;

public class Time
{
    public static float DetalTime;

    private static readonly Stopwatch Stopwatch = Stopwatch.StartNew();
    private static TimeSpan _lastFrameTime = TimeSpan.Zero;
    
    public static void Update()
    {
        var now = Stopwatch.Elapsed;
        var delta = now - _lastFrameTime;
        _lastFrameTime = now;
        DetalTime = (float)delta.TotalSeconds;
    }
}