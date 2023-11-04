﻿using System;
using System.Collections.Generic;
using MediatR;
using Sailfish.Contracts.Serialization.V1;
using Sailfish.Presentation;

namespace Sailfish.Contracts.Public.Commands;

public class WriteCurrentTrackingFileNotification : INotification
{
    public WriteCurrentTrackingFileNotification(IEnumerable<ClassExecutionSummaryTrackingFormat> classExecutionSummaries, DateTime timeStamp)
    {
        ClassExecutionSummaries = classExecutionSummaries;
        DefaultFileName = DefaultFileSettings.DefaultTrackingFileName(timeStamp);
    }

    public IEnumerable<ClassExecutionSummaryTrackingFormat> ClassExecutionSummaries { get; }
    public string DefaultFileName { get; }
}