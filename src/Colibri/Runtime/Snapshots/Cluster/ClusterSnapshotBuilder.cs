using System.Collections.Immutable;
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
           
            if (cfgCluster.Retry?.Enabled == true)
            {
                clusterMiddlewares.Add(new RetryMiddleware(cfgCluster.Retry.Attempts));
            }
            
            if (cfgCluster.LoadBalancer?.Enabled == true)
            {
                ILoadBalancer loadBalancer = cfgCluster.LoadBalancer.Type switch
                {
                    "rr" => new RoundRobinBalancer(hostsCount),
                    "rnd" => new RandomBalancer(hostsCount),
                    _ => new RoundRobinBalancer(hostsCount)
                };
                
                clusterMiddlewares.Add(new LoadBalancerMiddleware(loadBalancer));
            }
            
            if (cfgCluster.CircuitBreaker?.Enabled == true)
            {
                var breaker = new CircuitBreaker(
                    hostsCount,
                    cfgCluster.CircuitBreaker.Failures,
                    cfgCluster.CircuitBreaker.Timeout);
                
                clusterMiddlewares.Add(new CircuitBreakerMiddleware(breaker));
            }
            
            switch (cfgCluster.Protocol.ToLower())
            {
                case "http":
                    clusterMiddlewares.Add(new HttpTerminalMiddleware(cfgCluster.Hosts));
                    break;
                
                case "ws":
                    clusterMiddlewares.Add(new WebsocketTerminalMiddleware(cfgCluster.Hosts));
                    break;
                
                default:
                    throw new ArgumentException($"Invalid protocol {cfgCluster.Protocol}");
            }
            
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
}