namespace Colibri.Runtime.Snapshots.Routing;

public sealed class TrieBuilder
{
    public TrieNode Build(TempRoute[] tempRoutes)
    {
        var root = new TrieNode();
        
        foreach (var tempRoute in tempRoutes)
        {
            CreateTrieRecursively(root, tempRoute);
        }
        
        OrderTrieRecursively(root);
        
        return root;

        void CreateTrieRecursively(TrieNode root, TempRoute tempRoute)
        {
            var chunk = tempRoute.TotalUpstreamChunks[0];
            
            var child = root.Children
                .FirstOrDefault(n => n.Name == chunk.Name);

            if (child == null)
            {
                child = new TrieNode
                {
                    Name = chunk.Name,
                    IsParameter = chunk.IsParameter,
                    ParamIndex = chunk.ParamIndex
                };
                
                root.Children.Add(child);
            }
            
            var nextRoute = tempRoute with { TotalUpstreamChunks = tempRoute.TotalUpstreamChunks[1..] };
            if (nextRoute.TotalUpstreamChunks.Count > 0)
            {
                CreateTrieRecursively(child, nextRoute);
            }
            else
            {
                foreach (var method in tempRoute.Methods)
                {
                    var chunksHolder = new ChunksHolder
                    {
                        ClusterId = tempRoute.ClusterId,
                        Chunks = tempRoute.TotalDownstreamChunks
                    };
                    
                    child.Methods.Add(method, chunksHolder);
                }
            }
        }
        
        void OrderTrieRecursively(TrieNode root)
        {
            root.Children = root.Children
                .OrderBy(c => c.IsParameter)
                .ThenByDescending(c => c.Name!.Length)
                .ToList();

            foreach (var child in root.Children)
            {
                OrderTrieRecursively(child);
            }
        }
    }
}

public sealed class TrieNode
{
    public string? Name { get; set; }
    public bool IsParameter { get; set; }
    public int ParamIndex { get; set; }
    public List<TrieNode> Children { get; set; } = [];
    public Dictionary<string, ChunksHolder> Methods { get; } = [];
}

public sealed class ChunksHolder
{
    public ushort ClusterId { get; set; }
    public List<DownstreamChunk> Chunks { get; set; } = [];
}