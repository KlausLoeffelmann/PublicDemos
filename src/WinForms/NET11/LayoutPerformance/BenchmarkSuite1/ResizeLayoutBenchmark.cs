using Microsoft.VSDiagnostics;

namespace LargeFormSmokeTest.Benchmarks;
using BenchmarkDotNet.Attributes;
using LargeFormSmokeTest;
using LargeFormSmokeTest.Forms;
using LargeFormSmokeTest.Models;

[CPUUsageDiagnoser]
public class ResizeLayoutBenchmark
{
    private DeclarationForm _form = null!;
    private int _toggle;
    [GlobalSetup]
    public void Setup()
    {
        AppServices.Initialize();
        Person person = AppServices.Repository.Persons[0];
        Declaration declaration = person.Declarations[0];
        _form = new DeclarationForm(person, declaration);
        _form.CreateControl();
    }

    [Benchmark]
    public void ResizeAndLayout()
    {
        _toggle ^= 1;
        _form.Width = 900 + (_toggle * 240);
        _form.PerformLayout();
    }

    [GlobalCleanup]
    public void Cleanup() => _form.Dispose();
}