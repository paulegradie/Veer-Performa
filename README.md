<h1 align="center" style="flex-direction: column;"><img src="assets/Sailfish.png" alt="Sailfish" width="700" /></h1>

Sailfish is a .net library used to perform low resolution performance analysis of your component or API.

Available on [https://www.nuget.org/packages/Sailfish/](https://www.nuget.org/packages/Sailfish/)

## [Visit the Wiki](https://github.com/paulegradie/Sailfish/wiki)

Visit the [wiki](https://github.com/paulegradie/Sailfish/wiki) to view the [getting started guide](https://github.com/paulegradie/Sailfish/wiki/Using-Sailfish-as-a-C%23-console-app) and other helpful details.

## Intended Use
This test framework is intended to provide approximate millisecond resolution performance data of your component or API. It is NOT intended to produce high resolution (microsecond, nanosecond) results on performance.

You may use this project however you'd like, however the intended use case for this is to provide approximate millisecond
response time data for API calls that you're developing against. Please keep in mind the physical machines that you run this software will have a direct affect on the results that are produced. In otherwords, for more reliable results, execute tests on stable hardware that is, if possible, not really doing anything else. For example, running these tests on dynamic cloud infrastructure may introduce signficant outlier results.

Fortunately, tools to mitigate the affects of such volatility in the infrastructure are currently under development.

For this reason, this project does not go to the extent that other more rigorous benchmark analysis tools such as, say, BenchmarkDotNet do to buffer the effects of hardware and compute sharing.

Please visit our wiki for examples on how to use Sailfish effectively for your project or organization.

## Quick Start Guide

```csharp
class Program : SailfishProgramBase
{
    static async Task Main(string[] userRequestedTestNames)
    {
        // SailfishMain is in the base
        await SailfishMain<Program>(userRequestedTestNames);
    }

    // this override is provided by the cli tool
    public override async Task OnExecuteAsync()
    {
        var cliOptions = AssembleRunRequest();
        new SailfishExecution.Run(cliOptions)
     }
}

[Sailfish]
public class AMostBasicTest
{
    public AMostBasicTest(CancellationTokenAccess ctAccess)
    {
        CancellationToken = ctAccess.Token;
    }
    
    CancellationToken CancellationToken { get; set; }

    [SailfishMethod]
    public async Task TestMethod()
    {
        await Task.Delay(2000, CancellationToken);
    }
}
```

## License
Sailfish is [MIT licensed](./LICENSE).

## Acknowledgements

Sailfish is inspired by the [BenchmarkDotNet](https://benchmarkdotnet.org/) precision benchmarking library.
