using BenchmarkDotNet.Running;
using Colibri.Benchmarks.UpstreamMatcher;

Console.WriteLine("\nList of benchmarks:\n");

Console.WriteLine("1. UpstreamMatcher - RealCase");
Console.WriteLine("2. UpstreamMatcher - BestCase");

Console.Write("\nChose the benchmark: ");

string? chose = Console.ReadLine();

if (!int.TryParse(chose, out int parsed))
{
    Console.WriteLine("Invalid choice");
    return;
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
        Console.WriteLine("Invalid choice");
        return;
}