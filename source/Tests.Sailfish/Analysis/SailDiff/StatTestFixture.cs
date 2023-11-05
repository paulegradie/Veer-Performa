﻿using Sailfish.Analysis;
using Sailfish.Analysis.SailDiff;
using Sailfish.Statistics.Tests;
using Sailfish.Statistics.Tests.MWWilcoxonTestSailfish;
using Sailfish.Statistics.Tests.TTestSailfish;
using Sailfish.Statistics.Tests.TwoSampleWilcoxonSignedRankTestSailfish;
using Shouldly;
using Xunit;

namespace Test.Analysis.SailDiff;

public class StatisticalTestFixture
{
    [Fact]
    public void WhenStdDevIsZeroMannWhitneyWilcoxonTestSailfishDoesNotThrow()
    {
        var test = new MannWhitneyWilcoxonTestSailfish(new TestPreprocessor(new SailfishOutlierDetector()));

        var before = new[] { 0.0, 0, 0, 0, 0 };
        var after = new[] { 0.0, 0, 0, 0, 0 };

        Should.NotThrow(() => test.ExecuteTest(before, after, new SailDiffSettings(0.001, 2)));
    }

    [Fact]
    public void WhenStdDevIsZeroTwoSampleWilcoxonSignedRankTestSailfishDoesNotThrow()
    {
        var test = new TwoSampleWilcoxonSignedRankTestSailfish(new TestPreprocessor(new SailfishOutlierDetector()));

        var before = new[] { 0.0, 0, 0, 0, 0 };
        var after = new[] { 0.0, 0, 0, 0, 0 };

        Should.NotThrow(() => test.ExecuteTest(before, after, new SailDiffSettings(0.001, 2)));
    }

    [Fact]
    public void WhenStdDevIsZeroTestSailfishDoesNotThrow()
    {
        var test = new TTestSailfish(new TestPreprocessor(new SailfishOutlierDetector()));

        var before = new[] { 0.0, 0, 0, 0, 0 };
        var after = new[] { 0.0, 0, 0, 0, 0 };

        Should.NotThrow(() => test.ExecuteTest(before, after, new SailDiffSettings(0.001, 2)));
    }
}