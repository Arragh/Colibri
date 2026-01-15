// using Colibri.Configuration;
// using Colibri.Helpers;
// using Colibri.Snapshots.RoutingSnapshot.Models;
//
// namespace Colibri.Snapshots.RoutingSnapshot;
//
// public class GarbageModel
// {
//     public RoutingSnapshot Build(RoutingSettings settings)
//     {
//         string downstreamPathChunks = string.Empty;
//         var clustersData = PrepareDataForTrie(settings,  ref downstreamPathChunks);
//         var hosts = CreateHosts(clustersData);
//         var trie = CreateTrie(clustersData, hosts);
//         SortTrieRecursively(trie);
//
//         var tempSegments = new List<TempSegment>();
//         string segmentNames = string.Empty;
//         var tempDownstreams = new List<TempDownstream>();
//         var tempDownstreamChunks = new List<TempDownstreamChunk>();
//         FillDataArraysRecursively(tempSegments, tempDownstreams, tempDownstreamChunks, trie, ref segmentNames);
//         
//         var snapshot = CreateSnapshot(
//             tempSegments,
//             tempDownstreams,
//             tempDownstreamChunks,
//             segmentNames,
//             downstreamPathChunks,
//             trie.ChildrenSegments.Count,
//             hosts);
//
//         return snapshot;
//     }
//
//     private List<TempCluster> PrepareDataForTrie(RoutingSettings settings, ref string downstreamPathChunks)
//     {
//         var tempClusters = new List<TempCluster>();
//
//         foreach (var cluster in settings.Clusters)
//         {
//             var tempCluster = new TempCluster
//             {
//                 Protocol = cluster.Protocol,
//                 Hosts = cluster.Hosts
//             };
//
//             foreach (var route in cluster.Routes)
//             {
//                 var tempRoute = new TempRoute
//                 {
//                     Method = route.Method,
//                     Upstream = route.UpstreamPattern.Split('/', StringSplitOptions.RemoveEmptyEntries)
//                 };
//
//                 var downstreamChunks = new List<TempDownstreamChunk>();
//                 var upstreamSegments = route.UpstreamPattern
//                     .Split('/', StringSplitOptions.RemoveEmptyEntries)
//                     .Where(s => s.StartsWith('{') && s.EndsWith('}'))
//                     .ToArray();
//                 var downstreamSegments = route.DownstreamPattern.Split('/', StringSplitOptions.RemoveEmptyEntries);
//                 foreach (var segment in downstreamSegments)
//                 {
//                     var downstreamChunk = new TempDownstreamChunk
//                     {
//                         Name = '/' + segment,
//                         PathStartIndex = downstreamPathChunks.Length,
//                         PathLength = (byte)(segment.Length + 1)
//                     };
//                     
//                     downstreamPathChunks += downstreamChunk.Name;
//
//                     if (segment.StartsWith('{') && segment.EndsWith('}'))
//                     {
//                         downstreamChunk.IsParameter = true;
//                         downstreamChunk.ParamIndex = (short)upstreamSegments.IndexOf(segment);
//                     }
//                     
//                     downstreamChunks.Add(downstreamChunk);
//                 }
//
//                 tempRoute.Downstream = downstreamChunks.ToArray();
//                 tempCluster.Routes.Add(tempRoute);
//             }
//             
//             tempClusters.Add(tempCluster);
//         }
//         
//         return tempClusters;
//     }
//
//     private List<string> CreateHosts(List<TempCluster> clustersData)
//     {
//         var hosts = new List<string>();
//         
//         foreach (var cluster in clustersData)
//         {
//             hosts.AddRange(cluster.Hosts);
//         }
//         
//         return hosts;
//     }
//     
//     private TrieNode CreateTrie(List<TempCluster> clustersData, List<string> hosts)
//     {
//         var root = new TrieNode();
//         
//         foreach (var cluster in clustersData)
//         {
//             foreach (var route in cluster.Routes)
//             {
//                 fillTrieRecursively(route, root, cluster.Hosts);
//             }
//         }
//         
//         return root;
//         
//         void fillTrieRecursively(TempRoute route, TrieNode root, string[] clusterHosts)
//         {
//             var segment = root.ChildrenSegments
//                 .FirstOrDefault(t => t.SegmentName == route.Upstream[0]);
//         
//             if (segment == null)
//             {
//                 segment = new TrieNode
//                 {
//                     SegmentName = route.Upstream[0]
//                 };
//
//                 if (route.Upstream[0].StartsWith('{')
//                     && route.Upstream[0].EndsWith('}'))
//                 {
//                     segment.IsParameter = true;
//                 }
//             
//                 root.ChildrenSegments.Add(segment);
//             }
//         
//             route.Upstream = route.Upstream[1..];
//         
//             if (route.Upstream.Length == 0)
//             {
//                 var indexes = new List<int>();
//                 for (int i = 0; i < hosts.Count; i++)
//                 {
//                     if (clusterHosts.Contains(hosts[i]))
//                     {
//                         indexes.Add(i);
//                     }
//                 }
//
//                 if (indexes.Count > 0)
//                 {
//                     segment.HostStartIndex = indexes[0];
//                     segment.HostsCount = indexes.Count;
//                 }
//             
//                 segment.Methods.Add(route.Method, route.Downstream);
//             
//                 return;
//             }
//         
//             fillTrieRecursively(route, segment, clusterHosts);
//         }
//     }
//     
//     private void SortTrieRecursively(TrieNode segment)
//     {
//         segment.ChildrenSegments = segment.ChildrenSegments
//             .OrderBy(s => s.IsParameter)
//             .ThenByDescending(s => s.SegmentName!.Length)
//             .ToList();
//
//         foreach (var child in segment.ChildrenSegments)
//         {
//             SortTrieRecursively(child);
//         }
//     }
//
//     private void FillDataArraysRecursively(
//             List<TempSegment> tempSegments,
//             List<TempDownstream> tempDownstreams,
//             List<TempDownstreamChunk> tempDownstreamChunks,
//             TrieNode node,
//             ref string segmentNames)
//     {
//         foreach (var child in node.ChildrenSegments)
//         {
//             var tempSegment = new TempSegment();
//             
//             tempSegment.PathStartIndex = segmentNames.Length;
//             segmentNames += '/' + child.SegmentName;
//             tempSegment.PathLength = (short)(segmentNames.Length - tempSegment.PathStartIndex);
//
//             tempSegments.Add(tempSegment);
//             
//             foreach (var method in  child.Methods)
//             {
//                 var tempDownstream = new TempDownstream();
//                 
//                 tempDownstream.ChunkStartIndex = tempDownstreamChunks.Count;
//                 tempDownstream.ChunksCount = (short)method.Value.Length;
//                 tempDownstreamChunks.AddRange(method.Value);
//                 // downstreamPaths += method.Value;
//                 tempDownstream.MethodMask = HttpMethodMask.GetMask(method.Key);
//                 tempDownstream.HostStartIndex = (short)child.HostStartIndex;
//                 tempDownstream.HostsCount = (byte)child.HostsCount;
//                     
//                 tempDownstreams.Add(tempDownstream);
//
//                 tempSegment.DownstreamStartIndex = (short)(tempDownstreams.Count - 1);
//                 tempSegment.DownstreamCount++;
//                 tempSegment.MethodMask |= HttpMethodMask.GetMask(method.Key);
//             }
//         }
//
//         var createdTempSegments = tempSegments
//             .TakeLast(node.ChildrenSegments.Count)
//             .ToList();
//
//         for (int i = 0; i < node.ChildrenSegments.Count; i++)
//         {
//             createdTempSegments[i].ChildrenCount = (short)node.ChildrenSegments[i].ChildrenSegments.Count;
//
//             if (createdTempSegments[i].ChildrenCount > 0)
//             {
//                 createdTempSegments[i].FirstChildIndex = tempSegments.Count;
//             }
//             
//             FillDataArraysRecursively(
//                 tempSegments,
//                 tempDownstreams,
//                 tempDownstreamChunks,
//                 node.ChildrenSegments[i],
//                 ref segmentNames);
//         }
//     }
//
//     private RoutingSnapshot CreateSnapshot(
//         List<TempSegment> tempSegments,
//         List<TempDownstream> tempDownstreams,
//         List<TempDownstreamChunk> tempDownstreamChunks,
//         string segmentNames,
//         string downstreamPathChunks,
//         int rootSegmentsCount,
//         List<string> hosts)
//     {
//         var segments = new Segment[tempSegments.Count];
//         var downstreams = new Downstream[tempDownstreams.Count];
//         var downstreamChunks = new DownstreamChunk[tempDownstreamChunks.Count];
//
//         for (int i = 0; i < tempSegments.Count; i++)
//         {
//             segments[i] = new Segment(
//                 tempSegments[i].PathStartIndex,
//                 tempSegments[i].PathLength,
//                 tempSegments[i].FirstChildIndex,
//                 tempSegments[i].ChildrenCount,
//                 tempSegments[i].DownstreamStartIndex,
//                 tempSegments[i].DownstreamCount,
//                 tempSegments[i].MethodMask);
//         }
//
//         for (int i = 0; i < tempDownstreams.Count; i++)
//         {
//             downstreams[i] = new Downstream(
//                 tempDownstreams[i].ChunkStartIndex,
//                 tempDownstreams[i].ChunksCount,
//                 tempDownstreams[i].MethodMask,
//                 tempDownstreams[i].HostStartIndex,
//                 tempDownstreams[i].HostsCount);
//         }
//
//         for (int i = 0; i < tempDownstreamChunks.Count; i++)
//         {
//             downstreamChunks[i] = new DownstreamChunk(
//                 tempDownstreamChunks[i].IsParameter,
//                 tempDownstreamChunks[i].ParamIndex,
//                 tempDownstreamChunks[i].PathStartIndex,
//                 tempDownstreamChunks[i].PathLength);
//         }
//
//         var routingSnapshot = new RoutingSnapshot(
//             rootSegmentsCount,
//             segments,
//             segmentNames.ToArray(),
//             downstreams,
//             downstreamPathChunks.ToArray(),
//             downstreamChunks,
//             hosts.Select(h => new Uri(h)).ToArray());
//         
//         return routingSnapshot;
//     }
// }