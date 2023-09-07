using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Demo.API;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Sailfish.Attributes;
using Sailfish.Registration;

namespace PerformanceTests.ExamplePerformanceTests;

[WriteToCsv]
[WriteToMarkdown]
[Sailfish(SampleSize = 3, NumWarmupIterations = 2, DisableOverheadEstimation = false, Disabled = false)]
public class SailfishFixtureExample : TestBase
{
    private readonly SailfishFixture sailfishFixture;

    [SailfishVariable(1, 10)] public int VariableA { get; set; }

    [SailfishRangeVariable(true, 1, 4, 1)] public int Multiplier { get; set; }


    [SailfishMethod]
    public async Task Control(CancellationToken cancellationToken)
    {
        await Task.Delay(VariableA * Multiplier, cancellationToken);
    }

    public ExampleDep ExampleDep = null!;

    [SailfishMethodSetup(nameof(TestB))]
    public void ResolveSetup()
    {
        ExampleDep = sailfishFixture.ResolveType<ExampleDep>();
    }

    [SailfishMethod]
    public async Task TestB(CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
        ExampleDep.WriteSomething("Hello", VariableA * Multiplier);
    }

    public SailfishFixtureExample(SailfishFixture sailfishFixture, WebApplicationFactory<DemoApp> factory) : base(factory)
    {
        this.sailfishFixture = sailfishFixture;
    }
}

public class ExampleDep
{
    public void WriteSomething(string something, int sleepPeriod)
    {
        Console.WriteLine(something);
        Thread.Sleep(sleepPeriod);
    }
}

public class TestBase : ISailfishFixture<SailfishFixture>
{
    public TestBase(WebApplicationFactory<DemoApp> factory)
    {
        WebHostFactory = factory.WithWebHostBuilder(
            builder => { builder.UseTestServer(); });
        Client = WebHostFactory.CreateClient();
    }

    public WebApplicationFactory<DemoApp> WebHostFactory { get; set; }
    public HttpClient Client { get; }

    public virtual async Task GlobalTeardown(CancellationToken cancellationToken)
    {
        await Client.GetAsync("api", cancellationToken);
    }
}

// Example of what can be implemented
public class SailfishFixture : IDisposable
{
    private IContainer Container { get; set; }

    // single parameterless ctor is all that is allowed
    public SailfishFixture()
    {
        var builder = new ContainerBuilder();
        RegisterThings(builder);
        Container = builder.Build();
    }

    private static void RegisterThings(ContainerBuilder builder)
    {
        builder.RegisterType<ExampleDep>().AsSelf();
        builder.RegisterType<WebApplicationFactory<DemoApp>>();
    }

    public void Dispose() => Container.Dispose();

    public object ResolveType(Type type)
    {
        return Container.Resolve(type);
    }

    public T ResolveType<T>() where T : notnull
    {
        return Container.Resolve<T>();
    }
}