﻿using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Sailfish.Contracts.Public;
using Sailfish.Contracts.Public.Commands;
using Sailfish.Presentation;
using Sailfish.Presentation.Console;
using Serilog;

namespace Sailfish.Analysis.Saildiff;

public interface ISailDiff : IAnalyzeFromFile
{
}

public class SailDiff : ISailDiff
{
    private readonly IMediator mediator;
    private readonly ILogger logger;
    private readonly ITestComputer testComputer;
    private readonly ITestResultTableContentFormatter testResultTableContentFormatter;
    private readonly IConsoleWriter consoleWriter;

    public SailDiff(
        IMediator mediator,
        ILogger logger,
        ITestComputer testComputer,
        ITestResultTableContentFormatter testResultTableContentFormatter,
        IConsoleWriter consoleWriter)
    {
        this.mediator = mediator;
        this.logger = logger;
        this.testComputer = testComputer;
        this.testResultTableContentFormatter = testResultTableContentFormatter;
        this.consoleWriter = consoleWriter;
    }

    public async Task Analyze(
        DateTime timeStamp,
        IRunSettings runSettings,
        string trackingDir,
        CancellationToken cancellationToken
    )
    {
        var beforeAndAfterFileLocations = await mediator.Send(
                new BeforeAndAfterFileLocationCommand(
                    trackingDir,
                    runSettings.Tags,
                    runSettings.ProvidedBeforeTrackingFiles,
                    runSettings.Args),
                cancellationToken)
            .ConfigureAwait(false);

        if (!beforeAndAfterFileLocations.AfterFilePaths.Any() || !beforeAndAfterFileLocations.BeforeFilePaths.Any())
        {
            var message = new StringBuilder();
            if (!beforeAndAfterFileLocations.BeforeFilePaths.Any())
            {
                message.Append("No 'Before' file locations discovered. ");
            }

            if (!beforeAndAfterFileLocations.AfterFilePaths.Any())
            {
                message.Append("No 'After' file locations discovered. ");
            }

            message.Append(
                $"If file locations are not provided, data must be provided via the {nameof(ReadInBeforeAndAfterDataCommand)} handler.");
            var msg = message.ToString();
            logger.Warning("{Message}", msg);
        }

        var beforeAndAfterData = await mediator.Send(
                new ReadInBeforeAndAfterDataCommand(
                    beforeAndAfterFileLocations.BeforeFilePaths,
                    beforeAndAfterFileLocations.AfterFilePaths,
                    runSettings.Tags,
                    runSettings.Args),
                cancellationToken)
            .ConfigureAwait(false);

        if (beforeAndAfterData.BeforeData is null || beforeAndAfterData.AfterData is null)
        {
            logger.Warning("Failed to retrieve tracking data... aborting the test operation");
            return;
        }

        var testResults = testComputer.ComputeTest(
            beforeAndAfterData.BeforeData,
            beforeAndAfterData.AfterData,
            runSettings.Settings);

        if (!testResults.Any())
        {
            logger.Information("No prior test results found for the current set");
            return;
        }

        var testIds = new TestIds(beforeAndAfterData.BeforeData.TestIds, beforeAndAfterData.AfterData.TestIds);
        var testResultFormats = testResultTableContentFormatter.CreateTableFormats(testResults, testIds, cancellationToken);

        consoleWriter.WriteStatTestResultsToConsole(testResultFormats.MarkdownFormat, testIds, runSettings.Settings);

        await mediator.Publish(
                new WriteTestResultsAsMarkdownCommand(
                    testResultFormats.MarkdownFormat,
                    runSettings.LocalOutputDirectory ?? DefaultFileSettings.DefaultOutputDirectory,
                    runSettings.Settings,
                    timeStamp,
                    runSettings.Tags,
                    runSettings.Args),
                cancellationToken)
            .ConfigureAwait(false);

        await mediator.Publish(
                new WriteTestResultsAsCsvCommand(
                    testResultFormats.CsvFormat,
                    runSettings.LocalOutputDirectory ?? DefaultFileSettings.DefaultOutputDirectory,
                    runSettings.Settings,
                    timeStamp,
                    runSettings.Tags,
                    runSettings.Args),
                cancellationToken)
            .ConfigureAwait(false);

        if (runSettings.Notify)
        {
            await mediator.Publish(
                    new NotifyOnTestResultCommand(
                        testResultFormats,
                        runSettings.Settings,
                        timeStamp,
                        runSettings.Tags,
                        runSettings.Args),
                    cancellationToken)
                .ConfigureAwait(false);
        }
    }
}