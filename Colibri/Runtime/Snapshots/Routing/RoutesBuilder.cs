using Colibri.Configuration.Models;

namespace Colibri.Runtime.Snapshots.Routing;

public class RoutesBuilder
{
    public TempRoute[] Build(ClusterCfg[] cfgClusters, RouteCfg[] cfgRoutes)
    {
        var tempRoutes = new List<TempRoute>();

        for (int i = 0; i < cfgClusters.Length; i++)
        {
            var clusterRoutes = cfgRoutes
                .Where(r => r.ClusterId == cfgClusters[i].ClusterId)
                .ToArray();

            foreach (var route in clusterRoutes)
            {
                var tempRoute = new TempRoute
                {
                    ClusterId = i,
                    Methods = route.Methods,
                    UpstreamPattern = route.UpstreamPattern
                        .Split('/', StringSplitOptions.RemoveEmptyEntries),
                    DownstreamPattern = route.DownstreamPattern
                        .Split('/', StringSplitOptions.RemoveEmptyEntries),
                };

                if (cfgClusters[i].UsePrefix)
                {
                    tempRoute.UpstreamChunks.Add(new UpstreamChunk
                    {
                        Name = cfgClusters[i].Prefix
                    });
                }
                
                var paramIndex = 0;
                foreach (var segment in tempRoute.UpstreamPattern)
                {
                    var upstreamChunk = new UpstreamChunk
                    {
                        Name = segment
                    };
                    
                    if (segment.StartsWith('{') && segment.EndsWith('}'))
                    {
                        upstreamChunk.IsParameter = true;
                        upstreamChunk.ParamIndex = paramIndex++;
                    }
                    
                    tempRoute.UpstreamChunks.Add(upstreamChunk);
                }
                
                foreach (var segment in tempRoute.DownstreamPattern)
                {
                    var downstreamChunk = new DownstreamChunk
                    {
                        Name = segment
                    };
                    
                    var upstreamChunk = tempRoute.UpstreamChunks
                        .FirstOrDefault(c => c.Name == segment);
                    
                    if (upstreamChunk != null)
                    {
                        downstreamChunk.IsParameter = upstreamChunk.IsParameter;
                        downstreamChunk.ParamIndex = upstreamChunk.ParamIndex;
                    }
                    
                    tempRoute.DownstreamChunks.Add(downstreamChunk);
                }
                
                tempRoutes.Add(tempRoute);
            }
        }
        
        return tempRoutes.ToArray();
    }
}

public sealed record TempRoute
{
    public int ClusterId { get; set; }
    public string[] Methods { get; set; }
    public string[] UpstreamPattern { get; set; }
    public string[] DownstreamPattern { get; set; }
    public List<UpstreamChunk> UpstreamChunks { get; set; } = [];
    public List<DownstreamChunk> DownstreamChunks { get; set; } = [];
}

public sealed class DownstreamChunk
{
    public string Name { get; set; }
    public bool IsParameter { get; set; }
    public int ParamIndex { get; set; }
}

public sealed class UpstreamChunk
{
    public string Name { get; set; }
    public bool IsParameter { get; set; }
    public int ParamIndex { get; set; }
}