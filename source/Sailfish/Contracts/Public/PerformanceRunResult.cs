﻿using System;
using System.Linq;
using MathNet.Numerics.Statistics;
using Sailfish.Analysis;
using Sailfish.Execution;

namespace Sailfish.Contracts.Public;

public class PerformanceRunResult
{
    public PerformanceRunResult(string displayName, DateTimeOffset globalStart, DateTimeOffset globalEnd, double globalDuration, double mean, double stdDev, double variance,
        double median, double[] rawExecutionResults, int sampleSize, int numWarmupIterations, double[] dataWithOutliersRemoved, double[] upperOutliers, double[] lowerOutliers,
        int totalNumOutliers)
    {
        DisplayName = displayName;
        GlobalStart = globalStart;
        GlobalEnd = globalEnd;
        GlobalDuration = globalDuration;
        Mean = mean;
        StdDev = stdDev;
        Variance = variance;
        Median = median;
        RawExecutionResults = rawExecutionResults;
        SampleSize = sampleSize;
        NumWarmupIterations = numWarmupIterations;
        DataWithOutliersRemoved = dataWithOutliersRemoved;
        UpperOutliers = upperOutliers;
        LowerOutliers = lowerOutliers;
        TotalNumOutliers = totalNumOutliers;
    }

    private const double Tolerance = 0.000000001;
    public string DisplayName { get; init; }
    public double Mean { get; init; }
    public double Median { get; init; }
    public double StdDev { get; init; }
    public double Variance { get; init; }

    public double GlobalDuration { get; init; }
    public DateTimeOffset GlobalStart { get; init; }
    public DateTimeOffset GlobalEnd { get; init; }

    public double[] RawExecutionResults { get; init; } // milliseconds

    public int SampleSize { get; set; }
    public int NumWarmupIterations { get; set; }

    public double[] DataWithOutliersRemoved { get; init; } // milliseconds
    public double[] LowerOutliers { get; init; }
    public double[] UpperOutliers { get; init; }
    public int TotalNumOutliers { get; init; }

    public static PerformanceRunResult ConvertFromPerfTimer(TestCaseId testCaseId, PerformanceTimer performanceTimer, IExecutionSettings executionSettings)
    {
        var executionIterations = performanceTimer.ExecutionIterationPerformances
            .Select(x => x.GetDurationFromTicks())
            .Select(x => x.MilliSeconds.Duration)
            .ToArray();
        return ConvertWithOutlierAnalysis(testCaseId, performanceTimer, executionSettings, executionIterations);
    }

    private static PerformanceRunResult ConvertWithOutlierAnalysis(
        TestCaseId testCaseId,
        PerformanceTimer performanceTimer,
        IExecutionSettings executionSettings,
        double[] executionIterations)
    {
        var detector = new SailfishOutlierDetector();

        var (cleanData, lowerOutliers, upperOutliers, totalNumOutliers) = detector.DetectOutliers(executionIterations);

        var mean = cleanData.Mean();
        var median = cleanData.Median();
        var stdDev = executionIterations.Length > 1 ? cleanData.StandardDeviation() : 0;
        var variance = executionIterations.Length > 1 ? cleanData.Variance() : 0;
        return new PerformanceRunResult(displayName: testCaseId.DisplayName, globalStart: performanceTimer.GlobalStart, globalEnd: performanceTimer.GlobalStop,
            globalDuration: performanceTimer.GlobalDuration.TotalSeconds, mean: mean, stdDev: stdDev, variance: variance, median: median, rawExecutionResults: executionIterations,
            sampleSize: executionSettings.SampleSize, numWarmupIterations: executionSettings.NumWarmupIterations, dataWithOutliersRemoved: cleanData,
            upperOutliers: upperOutliers.ToArray(), lowerOutliers: lowerOutliers.ToArray(), totalNumOutliers: totalNumOutliers);
    }

    public void SetNumIterations(int n)
    {
        SampleSize = n;
    }
}