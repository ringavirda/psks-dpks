using System;
using System.Collections.Generic;
using System.Linq;
using DPCS.Domain;

namespace DPCS
{
    public class Topology3
    {
        private static int COUNT_OF_UNITS_IN_CLUSTER = 7;

        private Topology3() { }
        public Topology3(int coutOfClusters)
        {
            if (coutOfClusters <= 0)
            {
                throw new ArgumentException("Count of clusters can be only positive");
            }
            CountOfVertices = COUNT_OF_UNITS_IN_CLUSTER * coutOfClusters;

            var topology = GenerateTopology(coutOfClusters);
            RelationTable = Replicate(topology);

            ShortestPathTable = UnleashDijkstra(coutOfClusters);
        }


        private List<Cluster7> GenerateTopology(int countOfClusters)
        {
            int width = (int)Math.Sqrt(countOfClusters);

            Cluster7[,] topology = new Cluster7[width, width];

            int startingPoint = 0;
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    var cluster = new Cluster7();
                    
                    cluster.Unit1 = new Unit { ID = startingPoint + 1 };
                    cluster.Unit2 = new Unit { ID = startingPoint + 2 };
                    cluster.Unit3 = new Unit { ID = startingPoint + 3 };
                    cluster.Unit4 = new Unit { ID = startingPoint + 4 };
                    cluster.Unit5 = new Unit { ID = startingPoint + 5 };
                    cluster.Unit6 = new Unit { ID = startingPoint + 6 };
                    cluster.Unit7 = new Unit { ID = startingPoint + 7 };

                    startingPoint += COUNT_OF_UNITS_IN_CLUSTER;
                
                    cluster.Unit1.Relatives.AddRange(new List<Unit> { cluster.Unit2, cluster.Unit4 });
                    cluster.Unit2.Relatives.AddRange(new List<Unit> { cluster.Unit1, cluster.Unit3, cluster.Unit4 });
                    cluster.Unit3.Relatives.AddRange(new List<Unit> { cluster.Unit2, cluster.Unit4 });
                    cluster.Unit4.Relatives.AddRange(new List<Unit> { cluster.Unit1, cluster.Unit2, cluster.Unit3, cluster.Unit5, cluster.Unit6, cluster.Unit7 });
                    cluster.Unit5.Relatives.AddRange(new List<Unit> { cluster.Unit4 });
                    cluster.Unit6.Relatives.AddRange(new List<Unit> { cluster.Unit4 });
                    cluster.Unit7.Relatives.AddRange(new List<Unit> { cluster.Unit4 });
                    
                    topology[i, j] = cluster;
                }
            }

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (i + 1 >= width && width > 1)
                        topology[i, j].AddDown(topology[i - width + 1, j]);
                    else
                        if (i + 1 < width)
                            topology[i, j].AddDown(topology[i + 1, j]);

                    if (j + 1 >= width && width > 1)
                        topology[i, j].AddRight(topology[i, j - width + 1]);
                    else
                        if (j + 1 < width)
                            topology[i, j].AddRight(topology[i, j + 1]);
                }
            }

            return topology.Cast<Cluster7>().ToList();
        }

        public int[,] RelationTable { get; private set; }
        public int[,] ShortestPathTable { get; private set; }
        public int CountOfVertices { get; private set; }

        private int[,] Replicate(List<Cluster7> topology)
        {
            int[,] table = new int[CountOfVertices, CountOfVertices];
            foreach (var cluster in topology)
            {
                var unitList = cluster.AsList();
                foreach (var unit in unitList)
                {
                    foreach (var relative in unit.Relatives)
                    {
                        table[unit.ID - 1, relative.ID - 1] = 1;
                    }
                }
            }
            return table;
        }

        private int[,] UnleashDijkstra(int countOfClusters)
        {
            int verticesCount = countOfClusters * COUNT_OF_UNITS_IN_CLUSTER;

            var table = new int[verticesCount, verticesCount];

            int[] distance = new int[verticesCount];
            bool[] shortestPathTreeSet = new bool[verticesCount];

            for (int k = 0; k < verticesCount; k++)
            {
                for (int i = 0; i < verticesCount; i++)
                {
                    distance[i] = int.MaxValue;
                    shortestPathTreeSet[i] = false;
                }

                distance[k] = 0;

                for (int count = 0; count < verticesCount - 1; count++)
                {
                    int u = MinimumDistance(distance, shortestPathTreeSet, verticesCount);
                    shortestPathTreeSet[u] = true;

                    for (int v = 0; v < verticesCount; v++)
                        if (!shortestPathTreeSet[v] && Convert.ToBoolean(RelationTable[u, v]) && distance[u] != int.MaxValue && distance[u] + RelationTable[u, v] < distance[v])
                            distance[v] = distance[u] + RelationTable[u, v];
                }

                for (int i = 0; i < verticesCount; i++)
                {
                    table[k, i] = distance[i];
                }
            }

            return table;
        }
        private int MinimumDistance(int[] distance, bool[] shortestPathTreeSet, int verticesCount)
        {
            int min = int.MaxValue;
            int minIndex = 0;

            for (int v = 0; v < verticesCount; ++v)
            {
                if (shortestPathTreeSet[v] == false && distance[v] <= min)
                {
                    min = distance[v];
                    minIndex = v;
                }
            }

            return minIndex;
        }
    }
}
