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
            AddJob(Job.Default.WithInvocationCount(25600000));
        }
    }
    
    private readonly CurrentTenantWithClosureAlloc CurrentTenantWithClosureAlloc;
    private readonly CurrentTenant CurrentTenantWithoutClosureAlloc;
    private readonly Guid tenantId1 = Guid.NewGuid();
    private readonly Guid tenantId2 = Guid.NewGuid(); 
    private readonly Guid tenantId3 = Guid.NewGuid(); 
    private readonly Guid tenantId4= Guid.NewGuid(); 
    public CurrentTenantBenchmark()
    {
        CurrentTenantWithClosureAlloc = new CurrentTenantWithClosureAlloc(AsyncLocalCurrentTenantAccessor.Instance);
        CurrentTenantWithoutClosureAlloc = new CurrentTenant(AsyncLocalCurrentTenantAccessor.Instance);
    }

    [Benchmark]
    public void WithClosureAllocation()
    {
        using (CurrentTenantWithClosureAlloc.Change(tenantId1))
        {
            using (CurrentTenantWithClosureAlloc.Change(tenantId2))
            {
                using (CurrentTenantWithClosureAlloc.Change(tenantId3))
                {
                    using (CurrentTenantWithClosureAlloc.Change(tenantId4))
                    {
                        
                    }
                }
            }
        }
    }

    [Benchmark]
    public void WithoutClosureAllocation()
    {
        using (CurrentTenantWithoutClosureAlloc.Change(tenantId1))
        {
            using (CurrentTenantWithoutClosureAlloc.Change(tenantId2))
            {
                using (CurrentTenantWithoutClosureAlloc.Change(tenantId3))
                {
                    using (CurrentTenantWithoutClosureAlloc.Change(tenantId4))
                    {
                        
                    }
                }
            }
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