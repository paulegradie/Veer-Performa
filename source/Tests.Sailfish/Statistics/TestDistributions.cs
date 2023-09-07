using System;
using Accord.Statistics.Distributions.Univariate;

namespace Test.Statistics;

public static class TestDistributions
{
    public static double[] GenerateRandomNormalDistribution(int sampleSize, double mean, double standardDeviation, Random random)
    {
        return new NormalDistribution(mean, standardDeviation).Generate(sampleSize, random);
    }
}