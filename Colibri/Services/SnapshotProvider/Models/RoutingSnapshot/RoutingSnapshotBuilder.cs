using System.Data;
using Colibri.Configuration;

namespace Colibri.Services.SnapshotProvider.Models.RoutingSnapshot;

public sealed class RoutingSnapshotBuilder
{
    public RoutingSnapshot Build(RoutingSettings settings)
    {
        var root = new Dictionary<string, SegmentNode>();
        
        int upstreamPathCharsCount = 0;
        int segmentsCount = 0;
        int downstreamPathCharsCount = 0;
        int downstreamsCount = 0;
        
        var allHosts = settings.Clusters
            .SelectMany(c => c.Hosts)
            .ToArray();
        
        FillTrie(
            allHosts,
            root,
            settings,
            ref upstreamPathCharsCount,
            ref segmentsCount,
            ref downstreamPathCharsCount,
            ref downstreamsCount);

        var upstreamPathChars = new char[upstreamPathCharsCount];
        var segments = new Segment[segmentsCount];
        var downstreamPathChars = new char[downstreamPathCharsCount];
        var downstreams = new Downstream[downstreamsCount];
        
        FillDataArrays(
            root,
            upstreamPathChars,
            segments,
            downstreamPathChars,
            downstreams);
        
        return new RoutingSnapshot(
            segments,
            upstreamPathChars,
            downstreams,
            downstreamPathChars,
            allHosts.Select(h => new Uri(h)).ToArray());
    }

    #region FillTrie
    private void FillTrie(
        string[] allHosts,
        Dictionary<string, SegmentNode> root,
        RoutingSettings settings,
        ref int upstreamPathCharsCount,
        ref int segmentsCount,
        ref int downstreamPathCharsCount,
        ref int downstreamsCount)
    {
        foreach (var cluster in settings.Clusters)
        {
            foreach (var route in cluster.Routes)
            {
                var upstreamPathSegments = route.UpstreamPattern.Split('/', StringSplitOptions.RemoveEmptyEntries);

                CreateTrieRecursively(
                    route.Method,
                    route.DownstreamPattern,
                    upstreamPathSegments,
                    allHosts,
                    cluster.Hosts,
                    root,
                    ref upstreamPathCharsCount,
                    ref segmentsCount,
                    ref downstreamPathCharsCount,
                    ref downstreamsCount);
            }
        }
    }

    private void CreateTrieRecursively(
        string method,
        string downStreamPattern,
        string[] upstreamPathSegments,
        string[] allHosts,
        string[] clusterHosts,
        Dictionary<string, SegmentNode> root,
        ref int upstreamPathCharsCount,
        ref int segmentsCount,
        ref int downstreamPathCharsCount,
        ref int downstreamsCount)
    {
        if (!root.ContainsKey(upstreamPathSegments[0]))
        {
            root[upstreamPathSegments[0]] = new SegmentNode
            {
                SegmentName = upstreamPathSegments[0]
            };
            
            upstreamPathCharsCount += upstreamPathSegments[0].Length;
            segmentsCount++;
        }
        
        if (upstreamPathSegments.Length > 1)
        {
            CreateTrieRecursively(
                method,
                downStreamPattern,
                upstreamPathSegments.Skip(1).ToArray(),
                allHosts,
                clusterHosts,
                root[upstreamPathSegments[0]].IncludedSegments,
                ref upstreamPathCharsCount,
                ref segmentsCount,
                ref downstreamPathCharsCount,
                ref downstreamsCount);
        }
        else
        {
            if (root[upstreamPathSegments[0]].Methods.TryGetValue(method, out _))
            {
                throw new DuplicateNameException($"Duplicate method type: {method}");
            }
            
            root[upstreamPathSegments[0]].Methods.Add(method, downStreamPattern);
            downstreamsCount++;
            downstreamPathCharsCount += downStreamPattern.Length;

            for (int i = 0; i < allHosts.Length; i++)
            {
                if (root[upstreamPathSegments[0]].SegmentName == "info")
                {
                    Console.WriteLine($"{root[upstreamPathSegments[0]].SegmentName}: {allHosts[i]}");
                }
                
                foreach (var ch in clusterHosts)
                {
                    if (allHosts[i] == ch)
                    {
                        root[upstreamPathSegments[0]].HostStartIndex ??= i;
                    }
                }
            }
            
            root[upstreamPathSegments[0]].HostsCount = clusterHosts.Length;
        }
    }
    #endregion

    #region FillDataArrays
    private void FillDataArrays(
        Dictionary<string, SegmentNode> root,
        char[] upstreamPathChars,
        Segment[] segments,
        char[] downstreamPathChars,
        Downstream[] downstreams)
    {
        var upstreamPathStartIndex = 0;
        var segmentIndex = 0;
        
        var downstreamPathStartIndex = 0;
        short downstreamIndex = 0;
        
        var tempSegments = new TempSegment[segments.Length];
        var tempDownstreams = new TempDownstream[downstreams.Length];

        CreateTempSegmentsRecursively(
            upstreamPathChars,
            downstreamPathChars,
            tempSegments,
            root.Values.ToArray(),
            tempDownstreams,
            ref upstreamPathStartIndex,
            ref segmentIndex,
            ref downstreamPathStartIndex,
            ref downstreamIndex);

        for (int i = 0; i < tempSegments.Length; i++)
        {
            segments[i] = new Segment(
                tempSegments[i].PathStartIndex,
                tempSegments[i].PathLength,
                tempSegments[i].FirstChildIndex,
                tempSegments[i].ChildrenCount,
                tempSegments[i].DownstreamStartIndex,
                tempSegments[i].DownstreamCount,
                tempSegments[i].MethodMask);
        }

        for (int i = 0; i < tempDownstreams.Length; i++)
        {
            downstreams[i] = new Downstream(
                tempDownstreams[i].PathStartIndex,
                tempDownstreams[i].PathLength,
                tempDownstreams[i].MethodMask,
                tempDownstreams[i].HostStartIndex,
                tempDownstreams[i].HostsCount);
        }
    }
    
    private int? CreateTempSegmentsRecursively(
        char[] upstreamPathChars,
        char[] downstreamPathChars,
        TempSegment[] tempSegments,
        SegmentNode[] segmentNodesArray,
        TempDownstream[] tempDownstreams,
        ref int upstreamPathStartIndex,
        ref int segmentIndex,
        ref int downstreamPathStartIndex,
        ref short downstreamIndex)
    {
        /*
         * Так как смысл кода неочевиден, добавил подробные комментарии,
         * чтобы потом было проще вспомнить, что тут происходит.
         * Может показаться, что где-то комментарий объясняет очевидную вещь,
         * но лучше пусть так, чем потом ничего не вспомнить и не понимать, что тут происходит.
         */
        
        int? firstChildIndex = null;

        for (int i = 0; i < segmentNodesArray.Length; i++)
        {
            tempSegments[segmentIndex] = new TempSegment // Создаем модель и заполняем поля, которые сразу на 100% известны
            {
                PathStartIndex = upstreamPathStartIndex, // С какого индекса в массиве "paths" начинается имя текущего сегмента
                PathLength = (short)segmentNodesArray[i].SegmentName.Length, // Длина имени текущего сегмента в массиве "paths"
                
                /*
                 * Количество наследников после сегмента.
                 * Например, если имеем 2 маршрута "/users/{id}" и "/users/info",
                 * то у сегмента "users" 2 наследника - "{id}" и "info"
                 */
                ChildrenCount = (short)segmentNodesArray[i].IncludedSegments.Count
            };
            
            byte methodMask = 0;
            foreach (var k in segmentNodesArray[i].Methods.Keys) // Устанавливаем побитовую маску доступных методов для данного маршрута
            {
                methodMask = (byte)(methodMask | GetMethodMask(k)); // То есть на каждой итерации как бы пополняем список доступных методов
            }
            tempSegments[segmentIndex].MethodMask = methodMask;
            
            tempSegments[segmentIndex].DownstreamStartIndex = downstreamIndex;
            foreach (var kv in segmentNodesArray[i].Methods) // Добавляем доступные маршруты для эндпоинта по типу метода
            {
                var tempDownstream = new TempDownstream();
                tempDownstream.MethodMask = GetMethodMask(kv.Key);

                if (segmentNodesArray[i].HostStartIndex.HasValue)
                {
                    tempDownstream.HostStartIndex = (short)segmentNodesArray[i].HostStartIndex!.Value;
                    tempDownstream.HostsCount = (byte)segmentNodesArray[i].HostsCount;
                }
                
                tempDownstream.PathStartIndex = downstreamPathStartIndex; // С какого индекса в общем массиве начинать читать маршрут
                
                foreach (var c in kv.Value) // Заполнение общего массива символами маршрута
                {
                    downstreamPathChars[downstreamPathStartIndex++] = c;
                }
                
                tempDownstream.PathLength = (short)kv.Value.Length; // Общее количество символов маршрута, которые нужно прочитать
                tempDownstreams[downstreamIndex++] = tempDownstream;
                tempSegments[segmentIndex].DownstreamCount++; // Количество downstream-маршрутов на upstream-маршрут
            }
            
            /*
             * Присваиваем индекс первого элемента в списке,
             * чтобы вернуть его родителю как индекс первого наследника, т.е. FirstChildIndex
             */
            if (firstChildIndex == null)
            {
                firstChildIndex = segmentIndex;
            }
            
            foreach (var c in segmentNodesArray[i].SegmentName)
            {
                upstreamPathChars[upstreamPathStartIndex++] = c; // Заполняем общий массив "paths" символами имени сегмента
            }
            
            segmentIndex++; // Инкрементим индекс для следующего сегмента в цикле
        }
        
        for (int i = 0; i < segmentNodesArray.Length; i++) // Рекурсивный проход по наследникам
        {
            var frstChldIndx = CreateTempSegmentsRecursively(
                upstreamPathChars,
                downstreamPathChars,
                tempSegments,
                segmentNodesArray[i].IncludedSegments.Values.ToArray(),
                tempDownstreams,
                ref upstreamPathStartIndex,
                ref segmentIndex,
                ref downstreamPathStartIndex,
                ref downstreamIndex);
            
            if (frstChldIndx != null) // Если есть наследник и вернуло индекс, то присваиваем его в поле родителя
            {
                /*
                 * Так как "segmentIndex" убежал вперед в предыдущем цикле,
                 * то будем использовать "firstChildIndex", которому ранее было присвоено значение segmentIndex.
                 * Так как нужно присвоить значения всем родителям, мы инкрементимся за счет итерации "i".
                 * То есть это равносильно тому, что мы делали в предыдущем цикле, инкрементируя "segmentIndex"
                 */
                tempSegments[firstChildIndex!.Value + i].FirstChildIndex = frstChldIndx.Value;
            }
        }

        return firstChildIndex;
    }
    
    private static byte GetMethodMask(string method)
    {
        switch (method.Length)
        {
            case 3: return method[0] == 'G'
                ? HttpMethodBits.Get
                : HttpMethodBits.Put;

            case 4: return method[0] == 'P'
                ? HttpMethodBits.Post
                : HttpMethodBits.Head;

            case 5: return HttpMethodBits.Patch;
            case 6: return HttpMethodBits.Delete;
            case 7: return HttpMethodBits.Options;
            
            default: return HttpMethodBits.None;
        }
    }
    #endregion
}