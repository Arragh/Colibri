using Colibri.Configuration.Models;

namespace Colibri.Runtime.Snapshots.Routing;

public class RoutesBuilder
{
    public TempRoute[] Build(ClusterCfg[] cfgClusters, RouteCfg[] cfgRoutes)
    {
        var tempRoutes = new List<TempRoute>();

        for (ushort i = 0; i < cfgClusters.Length; i++)
        {
            var clusterRoutes = cfgRoutes
                .Where(r => r.ClusterName == cfgClusters[i].Name)
                .ToArray();

            foreach (var route in clusterRoutes)
            {
                var tempRoute = new TempRoute
                {
                    ClusterId = i,
                    Methods = route.Methods,
                    UpstreamSegments = route.UpstreamPattern
                        .Split('/', StringSplitOptions.RemoveEmptyEntries),
                    DownstreamSegments = route.DownstreamPattern
                        .Split('/', StringSplitOptions.RemoveEmptyEntries),
                };

                if (cfgClusters[i].UsePrefix)
                {
                    var prefix = cfgClusters[i].Prefix;
                    
                    if (prefix[0] == '/')
                    {
                        prefix = prefix[1..];
                    }
                    
                    tempRoute.TotalUpstreamChunks.Add(new UpstreamChunk
                    {
                        Name = prefix
                    });
                }
                
                var paramIndex = 0;
                foreach (var segment in tempRoute.UpstreamSegments)
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
                    
                    tempRoute.TotalUpstreamChunks.Add(upstreamChunk);
                }
                
                foreach (var segment in tempRoute.DownstreamSegments)
                {
                    var downstreamChunk = new DownstreamChunk
                    {
                        Name = segment
                    };
                    
                    var upstreamChunk = tempRoute.TotalUpstreamChunks
                        .FirstOrDefault(c => c.Name == segment);
                    
                    if (upstreamChunk != null)
                    {
                        downstreamChunk.IsParameter = upstreamChunk.IsParameter;
                        downstreamChunk.ParamIndex = upstreamChunk.ParamIndex;
                    }
                    
                    tempRoute.TotalDownstreamChunks.Add(downstreamChunk);
                }
                
                tempRoutes.Add(tempRoute);
            }
        }
        
        return tempRoutes.ToArray();
    }
}

public sealed record TempRoute
{
    public ushort ClusterId { get; set; }
    public string[] Methods { get; set; }
    public string[] UpstreamSegments { get; set; }
    public string[] DownstreamSegments { get; set; }
    public List<UpstreamChunk> TotalUpstreamChunks { get; set; } = [];
    public List<DownstreamChunk> TotalDownstreamChunks { get; set; } = [];
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