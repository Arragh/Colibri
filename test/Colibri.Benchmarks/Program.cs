using BenchmarkDotNet.Running;
using Colibri.Benchmarks.UpstreamMatcher;

/*
 * Use cmd: dotnet run -c Release
 */

Console.WriteLine("\nList of benchmarks:\n");

Console.WriteLine("1. UpstreamMatcher - RealCase");
Console.WriteLine("2. UpstreamMatcher - BestCase");

Console.Write("\nChose the benchmark: ");

string? chose = Console.ReadLine();

if (!int.TryParse(chose, out int parsed))
{
    throw new InvalidOperationException("Invalid choice");
}

switch (parsed)
{
    case 1:
        BenchmarkRunner.Run<UpstreamMatcherRealCase>();
        break;
    
    case 2:
        BenchmarkRunner.Run<UpstreamMatcherBestCase>();
        break;
    
    default:
        throw new InvalidOperationException("Invalid choice");
}