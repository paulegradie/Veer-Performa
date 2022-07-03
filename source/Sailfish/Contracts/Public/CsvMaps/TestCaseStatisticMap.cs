﻿using CsvHelper.Configuration;
using Sailfish.Statistics;

namespace Sailfish.Contracts.Public.CsvMaps;

public sealed class TestCaseStatisticMap : ClassMap<DescriptiveStatistics>
{
    public TestCaseStatisticMap()
    {
        Map(m => m.DisplayName).Index(0);
        Map(m => m.Median).Index(1);
        Map(m => m.Mean).Index(2);
        Map(m => m.StdDev).Index(3);
        Map(m => m.Variance).Index(4);
        Map(m => m.RawExecutionResults).Index(5);
    }
}