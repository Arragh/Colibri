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
            var chunk = tempRoute.UpstreamChunks[0];
            
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
            
            var nextRoute = tempRoute with { UpstreamChunks = tempRoute.UpstreamChunks[1..] };
            if (nextRoute.UpstreamChunks.Count > 0)
            {
                CreateTrieRecursively(child, nextRoute);
            }
            else
            {
                foreach (var method in tempRoute.Methods)
                {
                    child.Methods.Add(method, tempRoute.DownstreamChunks);
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
    public Dictionary<string, List<DownstreamChunk>> Methods { get; } = [];
}