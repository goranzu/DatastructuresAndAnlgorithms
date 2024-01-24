﻿public class Vertex
{
    public int Id { get; set; }
}

public class Edge
{
    public Vertex From { get; set; }
    public Vertex To { get; set; }
    public double Weight { get; set; }
}

public interface IGraph
{
    void AddVertex(Vertex vertex);
    void RemoveVertex(Vertex vertex);
    void AddEdge(Edge edge);
    void RemoveEdge(Edge edge);
    void BuildFromEdgeList(List<List<int>> edgeList);
    void BuildFromAdjacencyList(List<List<int>> adjacencyList);
    void BuildFromAdjacencyMatrix(List<List<int>> adjacencyMatrix);
    void BuildFromWeightedEdgeList(List<List<double>> weightedEdgeList);
    void BuildFromWeightedAdjacencyList(List<List<List<double>>> weightedAdjacencyList);
    void BuildFromWeightedAdjacencyMatrix(List<List<double>> weightedAdjacencyMatrix);
    // List<Vertex> DijkstraShortestPath(Vertex start, Vertex end);
    List<Edge> DijkstraShortestPath(Vertex start, Vertex end);
    
}

public class Graph : IGraph
{
    private readonly Dictionary<int, Vertex> _vertices = new Dictionary<int, Vertex>();
    private readonly List<Edge> _edges = new List<Edge>();

    public void AddVertex(Vertex vertex)
    {
        _vertices.TryAdd(vertex.Id, vertex);
    }

    public void RemoveVertex(Vertex vertex)
    {
        if (_vertices.ContainsKey(vertex.Id))
        {
            _vertices.Remove(vertex.Id);
            _edges.RemoveAll(e => e.From.Id == vertex.Id || e.To.Id == vertex.Id);
        }
    }

    public void AddEdge(Edge edge)
    {
        _edges.Add(edge);
        AddVertex(edge.From);
        AddVertex(edge.To);
    }

    public void RemoveEdge(Edge edge)
    {
        _edges.Remove(edge);
    }

    public void BuildFromEdgeList(List<List<int>> edgeList)
    {
        foreach (var edgeData in edgeList)
        {
            var fromVertex = new Vertex { Id = edgeData[0] };
            var toVertex = new Vertex { Id = edgeData[1] };

            var edge = new Edge
            {
                From = fromVertex,
                To = toVertex,
            };
            AddEdge(edge);
        }
    }

    public void BuildFromAdjacencyList(List<List<int>> adjacencyList)
    {
        for (int i = 0; i < adjacencyList.Count; i++)
        {
            foreach (var adjacent in adjacencyList[i])
            {
                var fromVertex = new Vertex { Id = i };
                var toVertex = new Vertex { Id = adjacent };
                var edge = new Edge
                {
                    From = fromVertex,
                    To = toVertex,
                };
                AddEdge(edge);
            }
        }
    }

    public void BuildFromAdjacencyMatrix(List<List<int>> adjacencyMatrix)
    {
        for (int i = 0; i < adjacencyMatrix.Count; i++)
        {
            for (int j = 0; j < adjacencyMatrix[i].Count; j++)
            {
                if (adjacencyMatrix[i][j] != 0)
                {
                    var fromVertex = new Vertex { Id = i };
                    var toVertex = new Vertex { Id = j };

                    var edge = new Edge { From = fromVertex, To = toVertex };
                    AddEdge(edge);
                }
            }
        }
    }

    public void BuildFromWeightedEdgeList(List<List<double>> weightedEdgeList)
    {
        foreach (var edgeData in weightedEdgeList)
        {
            var fromVertex = new Vertex { Id = (int)edgeData[0] };
            var toVertex = new Vertex { Id = (int)edgeData[1] };
            var weight = edgeData[2];

            var edge = new Edge { From = fromVertex, To = toVertex, Weight = weight };
            AddEdge(edge);
        }
    }

    public void BuildFromWeightedAdjacencyList(List<List<List<double>>> weightedAdjacencyList)
    {
        for (int i = 0; i < weightedAdjacencyList.Count; i++)
        {
            foreach (var adjacent in weightedAdjacencyList[i])
            {
                var fromVertex = new Vertex { Id = i };
                var toVertex = new Vertex { Id = (int)adjacent[0] };
                var weight = adjacent[1];

                var edge = new Edge { From = fromVertex, To = toVertex, Weight = weight };
                AddEdge(edge);
            }
        }
    }

    public void BuildFromWeightedAdjacencyMatrix(List<List<double>> weightedAdjacencyMatrix)
    {
        for (int i = 0; i < weightedAdjacencyMatrix.Count; i++)
        {
            for (int j = 0; j < weightedAdjacencyMatrix[i].Count; j++)
            {
                var weight = weightedAdjacencyMatrix[i][j];
                if (weight != 0)
                {
                    var fromVertex = new Vertex { Id = i };
                    var toVertex = new Vertex { Id = j };
                    var edge = new Edge { From = fromVertex, To = toVertex, Weight = weight };
                    AddEdge(edge);
                }
            }
        }
    }

    public List<Edge> DijkstraShortestPath(Vertex start, Vertex end)
    {
        var distances = new Dictionary<int, double>();
        var previous = new Dictionary<int, int>();
        var nodes = new List<int>();

        foreach (var vertex in _vertices)
        {
            if (vertex.Key == start.Id)
            {
                distances[vertex.Key] = 0;
            }
            else
            {
                distances[vertex.Key] = double.PositiveInfinity;
            }

            nodes.Add(vertex.Key);
        }

        while (nodes.Count != 0)
        {
            nodes.Sort((x, y) => distances[x] < distances[y] ? -1 : 1);

            var smallest = nodes[0];
            nodes.Remove(smallest);

            if (smallest == end.Id)
            {
                var path = new List<Edge>();
                while (previous.ContainsKey(smallest))
                {
                    path.Add(_edges.Find(e => e.From.Id == previous[smallest] && e.To.Id == smallest)!);
                    smallest = previous[smallest];
                }

                path.Reverse();
                return path;
            }

            if (double.IsPositiveInfinity(distances[smallest]))
            {
                break;
            }

            foreach (var neighbor in GetNeighbors(smallest))
            {
                var alt = distances[smallest] + neighbor.Weight;
                if (alt < distances[neighbor.To.Id])
                {
                    distances[neighbor.To.Id] = alt;
                    previous[neighbor.To.Id] = smallest;
                }
            }
        }

        return new List<Edge>();
    }
    
    public bool HasEdge(Vertex from, Vertex to)
    {
        foreach (var edge in _edges)
        {
            if (edge.From.Id == from.Id && edge.To.Id == to.Id)
            {
                return true;
            }
        }
        return false;
    }
    
    private List<Edge> GetNeighbors(int id)
    {
        var neighbors = new List<Edge>();
        foreach (var edge in _edges)
        {
            if (edge.From.Id == id)
            {
                neighbors.Add(edge);
            }
        }

        return neighbors;
    }

    // public List<Vertex> DijkstraShortestPath(Vertex start, Vertex end)
    // {
    //     var distances = new Dictionary<int, double>();
    //     var previous = new Dictionary<int, int>();
    //     var nodes = new List<int>();
    //
    //     foreach (var vertex in _vertices)
    //     {
    //         if (vertex.Key == start.Id)
    //         {
    //             distances[vertex.Key] = 0;
    //         }
    //         else
    //         {
    //             distances[vertex.Key] = double.PositiveInfinity;
    //         }
    //
    //         nodes.Add(vertex.Key);
    //     }
    //
    //     while (nodes.Count != 0)
    //     {
    //         nodes.Sort((x, y) => distances[x] < distances[y] ? -1 : 1);
    //
    //         var smallest = nodes[0];
    //         nodes.Remove(smallest);
    //
    //         if (smallest == end.Id)
    //         {
    //             var path = new List<Vertex>();
    //             while (previous.ContainsKey(smallest))
    //             {
    //                 path.Add(_vertices[smallest]);
    //                 smallest = previous[smallest];
    //             }
    //
    //             return path;
    //         }
    //
    //         if (double.IsPositiveInfinity(distances[smallest]))
    //         {
    //             break;
    //         }
    //
    //         foreach (var neighbor in GetNeighbors(smallest))
    //         {
    //             var alt = distances[smallest] + neighbor.Weight;
    //             if (alt < distances[neighbor.To.Id])
    //             {
    //                 distances[neighbor.To.Id] = alt;
    //                 previous[neighbor.To.Id] = smallest;
    //             }
    //         }
    //     }
    //
    //     return new List<Vertex>();
    // }
    //
    // private List<Edge> GetNeighbors(int id)
    // {
    //     var neighbors = new List<Edge>();
    //     foreach (var edge in _edges)
    //     {
    //         if (edge.From.Id == id)
    //         {
    //             neighbors.Add(edge);
    //         }
    //     }
    //
    //     return neighbors;
    // }
}