using System.Threading;
using Sailfish.Analysis.SailDiff;
using Sailfish.Contracts.Public;
using Sailfish.Execution;

namespace Sailfish.TestAdapter.Execution;

internal interface IAdapterSailDiff : ISailDiff
{
    string ComputeTestCaseDiff(
        TestExecutionResult testExecutionResult,
        IClassExecutionSummary classExecutionSummary,
        SailDiffSettings sailDiffSettings,
        PerformanceRunResult preloadedLastRun,
        CancellationToken cancellationToken);
}