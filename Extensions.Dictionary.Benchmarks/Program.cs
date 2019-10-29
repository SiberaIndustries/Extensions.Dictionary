using BenchmarkDotNet.Running;

namespace Extensions.Dictionary.Benchmarks
{
    static class Program
    {
        static void Main(string[] args)
        {
            BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
        }
    }
}
