using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using Colibri.Configuration.Models;
using Colibri.Runtime.Pipeline;
using Colibri.Runtime.Pipeline.Cluster.Authorization;
using Colibri.Runtime.Pipeline.Cluster.CircuitBreaker;
using Colibri.Runtime.Pipeline.Cluster.LoadBalancer;
using Colibri.Runtime.Pipeline.Cluster.Retrier;
using Colibri.Runtime.Pipeline.Cluster.Terminal;
using Colibri.Services;

namespace Colibri.Runtime.Snapshots.Cluster;

public sealed class ClusterSnapshotBuilder(TokenCache cache)
{
    public ClusterSnapshot Build(
        JwtSchemeCfg[] cfgJwtSchemes,
        ClusterCfg[] cfgClusters)
    {
        var snpClusters = new List<ClusterSnp>();
        
        foreach (var cfgCluster in cfgClusters)
        {
            if (!cfgCluster.Enabled)
            {
                continue;
            }
            
            List<IPipelineMiddleware> clusterMiddlewares = new();
            var hostsCount = cfgCluster.Hosts.Length;

            if (cfgCluster.Authorization.Any(a => a.Enabled))
            {
                AddAuthorization(clusterMiddlewares, cfgJwtSchemes, cfgCluster);
            }
           
            if (cfgCluster.Retry?.Enabled == true)
            {
                AddRetry(clusterMiddlewares, cfgCluster);
            }
            
            if (cfgCluster.LoadBalancer?.Enabled == true)
            {
                AddLoadBalancer(clusterMiddlewares, cfgCluster, hostsCount);
            }
            
            if (cfgCluster.CircuitBreaker?.Enabled == true)
            {
                AddCircuitBreaker(clusterMiddlewares, cfgCluster, hostsCount);
            }
            
            AddTerminal(clusterMiddlewares, cfgCluster);
            
            var snpCluster = new ClusterSnp
            {
                Name = cfgCluster.Name,
                Protocol = cfgCluster.Protocol,
                Prefix = cfgCluster.Prefix,
                HostsCount = cfgCluster.Hosts.Length,
                Pipeline = new PipelineSrv(clusterMiddlewares.ToArray())
            };
            
            snpClusters.Add(snpCluster);
        }

        return new ClusterSnapshot
        {
            Clusters = snpClusters.ToImmutableArray()
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void AddAuthorization(
        List<IPipelineMiddleware> clusterMiddlewares,
        JwtSchemeCfg[] cfgJwtSchemes,
        ClusterCfg cfgCluster)
    {
        List<Authorizer> authorizers = new();
        foreach (var auth in cfgCluster.Authorization)
        {
            if (auth.Enabled)
            {
                var jwtScheme = cfgJwtSchemes
                    .First(s => s.Name == auth.JwtScheme);
            
                var authorizer = new Authorizer(
                    auth.Claims,
                    jwtScheme.Algorithm,
                    jwtScheme.Key);
            
                authorizers.Add(authorizer);
            }
        }

        if (authorizers.Count > 0)
        {
            clusterMiddlewares.Add(new AuthorizationMiddleware(authorizers.ToArray(), cache));
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void AddRetry(
        List<IPipelineMiddleware> clusterMiddlewares,
        ClusterCfg cfgCluster)
    {
        clusterMiddlewares.Add(new RetryMiddleware(cfgCluster.Retry!.Attempts));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void AddLoadBalancer(
        List<IPipelineMiddleware> clusterMiddlewares,
        ClusterCfg cfgCluster,
        int hostsCount)
    {
        ILoadBalancer loadBalancer = cfgCluster.LoadBalancer!.Type switch
        {
            "rr" => new RoundRobinBalancer(hostsCount),
            "rnd" => new RandomBalancer(hostsCount),
            _ => new RoundRobinBalancer(hostsCount)
        };
        
        clusterMiddlewares.Add(new LoadBalancerMiddleware(loadBalancer));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void AddCircuitBreaker(
        List<IPipelineMiddleware> clusterMiddlewares,
        ClusterCfg cfgCluster,
        int hostsCount)
    {
        var breaker = new CircuitBreaker(
            hostsCount,
            cfgCluster.CircuitBreaker!.Failures,
            cfgCluster.CircuitBreaker.Timeout);
        
        clusterMiddlewares.Add(new CircuitBreakerMiddleware(breaker));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void AddTerminal(
        List<IPipelineMiddleware> clusterMiddlewares,
        ClusterCfg cfgCluster)
    {
        switch (cfgCluster.Protocol.ToLower())
        {
            case "http":
                clusterMiddlewares.Add(new HttpTerminalMiddleware(cfgCluster.Hosts));
                break;
            
            case "ws":
                clusterMiddlewares.Add(new WebsocketTerminalMiddleware(cfgCluster.Hosts));
                break;
        }
    }
}