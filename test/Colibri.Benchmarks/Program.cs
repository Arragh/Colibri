using BenchmarkDotNet.Running;
using Colibri.Benchmarks.UpstreamMatcher;

BenchmarkRunner.Run<UpstreamMatcherRealCase>();
BenchmarkRunner.Run<UpstreamMatcherBestCase>();