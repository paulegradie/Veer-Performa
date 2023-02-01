﻿using System.Collections.Generic;
using Accord.Collections;
using Sailfish.Execution;

namespace Sailfish.Presentation.Console;

internal interface IConsoleWriter
{
    string Present(IEnumerable<IExecutionSummary> result, OrderedDictionary<string, string> tags);
}   