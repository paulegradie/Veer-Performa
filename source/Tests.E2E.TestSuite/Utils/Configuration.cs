﻿using Sailfish.Registration;

namespace Tests.E2E.TestSuite.Utils;

public class Configuration : ISailfishDependency
{
    public ScenarioData Get(string key)
    {
        return new ScenarioData(key);
    }
}