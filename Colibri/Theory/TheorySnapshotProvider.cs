using System.Data;
using Colibri.Configuration;
using Colibri.Theory.Models;
using Colibri.Theory.Structs;
using Microsoft.Extensions.Options;

namespace Colibri.Theory;

public class TheorySnapshotProvider
{
    private readonly Dictionary<string, SegmentNode> _root = new();
    private readonly char[] _paths;
    private readonly Segment[] _segments;

    public TheorySnapshotProvider(IOptionsMonitor<RoutingSettings> monitor)
    {
        var trieDataCounts = CreateTrie(monitor.CurrentValue);

        _paths = new char[trieDataCounts.Item1];
        _segments = new Segment[trieDataCounts.Item2];
        
        FillDataArrays(_paths, _segments);

        Console.WriteLine();
    }

    private void FillDataArrays(char[] paths, Segment[] segments)
    {
        var pathStartIndex = 0;
        var segmentIndex = 0;
        var tempSegments = new TempSegment[segments.Length];

        CreateTempSegmentsRecursively(
            ref pathStartIndex,
            ref segmentIndex,
            paths,
            tempSegments,
            _root.Values.ToArray());

        for (int i = 0; i < tempSegments.Length; i++)
        {
            _segments[i] = new Segment(
                tempSegments[i].PathStartIndex,
                tempSegments[i].PathLength,
                tempSegments[i].FirstChildIndex,
                tempSegments[i].ChildrenCount);
        }
    }
    
    private int? CreateTempSegmentsRecursively(
        ref int pathStartIndex,
        ref int segmentIndex,
        char[] paths,
        TempSegment[] tempSegments,
        SegmentNode[] segmentsArray)
    {
        /*
         * Так как смысл кода неочевиден, добавил подробные комментарии,
         * чтобы потом было проще вспомнить, что тут происходит.
         */
        
        int? firstChildIndex = null;

        for (int i = 0; i < segmentsArray.Length; i++)
        {
            // Создаем модель и заполняем поля, которые сразу на 100% известны
            tempSegments[segmentIndex] = new TempSegment
            {
                PathStartIndex = pathStartIndex, // С какого индекса в массиве "paths" начинается имя текущего сегмента
                PathLength = segmentsArray[i].SegmentName.Length, // Длина имени текущего сегмента в массиве "paths"
                
                /*
                 * Количество наследников после сегмента.
                 * Например, если имеем 2 маршрута "/users/{id}" и "/users/info",
                 * то у сегмента "users" 2 наследника - "{id}" и "info"
                 */
                ChildrenCount = segmentsArray[i].IncludedSegments.Count
            };

            /*
             * Присваиваем индекс первого элемента в списке,
             * чтобы вернуть его родителю как индекс первого наследника, т.е. FirstChildIndex
             */
            if (firstChildIndex == null)
            {
                firstChildIndex = segmentIndex;
            }
            
            foreach (var c in segmentsArray[i].SegmentName)
            {
                paths[pathStartIndex++] = c; // Заполняем общий массив "paths" символами имени сегмента
            }
            
            segmentIndex++; // Инкрементим индекс для следующего сегмента в цикле
        }
        
        // Рекурсивный проход по наследникам
        for (int i = 0; i < segmentsArray.Length; i++)
        {
            var frstChldIndx = CreateTempSegmentsRecursively(
                ref pathStartIndex,
                ref segmentIndex,
                paths,
                tempSegments,
                segmentsArray[i].IncludedSegments.Values.ToArray());

            // Если есть наследник и вернуло индекс, то присваиваем его в поле родителя
            if (frstChldIndx != null)
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

    private (int, int) CreateTrie(RoutingSettings settings)
    {
        int charsCount = 0;
        int segmentsCount = 0;
        
        foreach (var cluster in settings.Clusters)
        {
            foreach (var route in cluster.Routes)
            {
                var upstreamSegmentsArray = route.UpstreamPattern.Split('/', StringSplitOptions.RemoveEmptyEntries);
                CreateTrieRecursively(
                    route.Method,
                    upstreamSegmentsArray,
                    route.DownstreamPattern,
                    _root,
                    ref charsCount,
                    ref segmentsCount);
            }
        }

        return (charsCount, segmentsCount);
    }

    private void CreateTrieRecursively(
        string method,
        string[] segments,
        string downStreamPattern,
        Dictionary<string, SegmentNode> root,
        ref int charsLenght,
        ref int segmentsCount)
    {
        if (!root.ContainsKey(segments[0]))
        {
            root[segments[0]] = new SegmentNode
            {
                SegmentName = segments[0]
            };
            
            charsLenght += segments[0].Length;
            segmentsCount++;
        }
        
        if (segments.Length > 1)
        {
            CreateTrieRecursively(
                method,
                segments.Skip(1).ToArray(),
                downStreamPattern,
                root[segments[0]].IncludedSegments,
                ref charsLenght,
                ref segmentsCount);
        }
        else
        {
            if (root[segments[0]].Methods.TryGetValue(method, out _))
            {
                throw new DuplicateNameException($"Duplicate method type: {method}");
            }
                
            root[segments[0]].Methods.Add(method, downStreamPattern);
        }
    }
}