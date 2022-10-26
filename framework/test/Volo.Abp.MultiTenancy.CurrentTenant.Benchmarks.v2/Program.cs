// See https://aka.ms/new-console-template for more information

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using Volo.Abp.MultiTenancy;

[Config(typeof(Config))]
public class CurrentTenantBenchmark
{
    private class Config : ManualConfig
    {
        public Config()
        {
            AddExporter(MarkdownExporter.GitHub);
            AddDiagnoser(MemoryDiagnoser.Default);
            AddJob(Job.Default.WithInvocationCount(4096000));
        }
    }
    
    private readonly CurrentTenantWithClosureAlloc CurrentTenantWithClosureAlloc;
    private readonly CurrentTenant CurrentTenantWithoutClosureAlloc;
    private readonly CurrentTenantWithStruct CurrentTenantWithStruct;
    private readonly Guid tenantId1 = Guid.NewGuid();
    private readonly Guid tenantId2 = Guid.NewGuid(); 
    private readonly Guid tenantId3 = Guid.NewGuid(); 
    private readonly Guid tenantId4= Guid.NewGuid(); 
    private readonly Guid tenantId5 = Guid.NewGuid(); 
    private readonly Guid tenantId6 = Guid.NewGuid(); 
    private readonly Guid tenantId7 = Guid.NewGuid(); 
    private readonly Guid tenantId8 = Guid.NewGuid(); 
    private readonly Guid tenantId9 = Guid.NewGuid(); 
    private readonly Guid tenantId10= Guid.NewGuid(); 
    public CurrentTenantBenchmark()
    {
        CurrentTenantWithClosureAlloc = new CurrentTenantWithClosureAlloc(AsyncLocalCurrentTenantAccessor.Instance);
        CurrentTenantWithoutClosureAlloc = new CurrentTenant(AsyncLocalCurrentTenantAccessor.Instance);
        CurrentTenantWithStruct = new CurrentTenantWithStruct(AsyncLocalCurrentTenantAccessor.Instance);
    }

    [Benchmark(Baseline = true)]
    public void WithClosureAllocation()
    {
        using (CurrentTenantWithClosureAlloc.Change(tenantId1))
        {
            
        }
    }

    [Benchmark]
    public void WithoutClosureAllocation()
    {
        using (CurrentTenantWithoutClosureAlloc.Change(tenantId1))
        {
            
        }
    }

    [Benchmark]
    public void WithStructDisposeAction()
    {
        using (CurrentTenantWithStruct.Change(tenantId1))
        {
            
        }
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        var summary = BenchmarkRunner.Run(typeof(Program).Assembly);
    }
}