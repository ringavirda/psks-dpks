using DPCS.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DPCS
{
    public class Topology4
    {
        private static int COUNT_OF_UNITS_IN_CLUSTER = 16;

        private Topology4() { }
        public Topology4(int coutOfClusters)
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


        private List<Cluster16> GenerateTopology(int countOfClusters)
        {
            int width = (int)Math.Sqrt(countOfClusters);

            Cluster16[,] topology = new Cluster16[width, width];

            int startingPoint = 0;
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    var cluster = new Cluster16();

                    for (int l = 0; l < COUNT_OF_UNITS_IN_CLUSTER; l++)
                    {
                        cluster.Units[l] = new Unit { ID = startingPoint + l };
                    }

                    startingPoint += COUNT_OF_UNITS_IN_CLUSTER;

                    cluster.Units[0].Relatives.AddRange(new List<Unit> { cluster.Units[1], cluster.Units[5] });
                    cluster.Units[1].Relatives.AddRange(new List<Unit> { cluster.Units[0], cluster.Units[2], cluster.Units[8], cluster.Units[11] });
                    cluster.Units[2].Relatives.AddRange(new List<Unit> { cluster.Units[1], cluster.Units[3], cluster.Units[12], cluster.Units[10] });
                    cluster.Units[3].Relatives.AddRange(new List<Unit> { cluster.Units[2], cluster.Units[4], cluster.Units[6] });
                    cluster.Units[4].Relatives.AddRange(new List<Unit> { cluster.Units[3], cluster.Units[5], cluster.Units[14], cluster.Units[8] });
                    cluster.Units[5].Relatives.AddRange(new List<Unit> { cluster.Units[0], cluster.Units[4], cluster.Units[10], cluster.Units[7] });
                    cluster.Units[6].Relatives.AddRange(new List<Unit> { cluster.Units[3], cluster.Units[7], cluster.Units[11] });
                    cluster.Units[7].Relatives.AddRange(new List<Unit> { cluster.Units[6], cluster.Units[8], cluster.Units[13], cluster.Units[5] });
                    cluster.Units[8].Relatives.AddRange(new List<Unit> { cluster.Units[1], cluster.Units[7], cluster.Units[9], cluster.Units[4] });
                    cluster.Units[9].Relatives.AddRange(new List<Unit> { cluster.Units[8], cluster.Units[10] });
                    cluster.Units[10].Relatives.AddRange(new List<Unit> { cluster.Units[5], cluster.Units[9], cluster.Units[11], cluster.Units[2] });
                    cluster.Units[11].Relatives.AddRange(new List<Unit> { cluster.Units[6], cluster.Units[10], cluster.Units[15], cluster.Units[1] });
                    cluster.Units[12].Relatives.AddRange(new List<Unit> { cluster.Units[2], cluster.Units[13], cluster.Units[14] });
                    cluster.Units[13].Relatives.AddRange(new List<Unit> { cluster.Units[7], cluster.Units[12], cluster.Units[15] });
                    cluster.Units[14].Relatives.AddRange(new List<Unit> { cluster.Units[4], cluster.Units[15], cluster.Units[12] });
                    cluster.Units[15].Relatives.AddRange(new List<Unit> { cluster.Units[11], cluster.Units[14], cluster.Units[13] });

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

            return topology.Cast<Cluster16>().ToList();
        }

        public int[,] RelationTable { get; private set; }
        public int[,] ShortestPathTable { get; private set; }
        public int CountOfVertices { get; private set; }

        private int[,] Replicate(List<Cluster16> topology)
        {
            int[,] table = new int[CountOfVertices, CountOfVertices];
            foreach (var cluster in topology)
            {
                var unitList = cluster.AsList();
                foreach (var unit in unitList)
                {
                    foreach (var relative in unit.Relatives)
                    {
                        table[unit.ID, relative.ID] = 1;
                    }
                }
            }
            return table;
        }

        private int[,] UnleashDijkstra(int countOfClusters)
        {
            int verticesCount = countOfClusters * COUNT_OF_UNITS_IN_CLUSTER;

            var table = new int[verticesCount, verticesCount];

            Parallel.For(0, verticesCount, k =>
            {
                bool[] shortestPathTreeSet = new bool[verticesCount];
                int[] distance = new int[verticesCount];

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
            });

            return table;
        }
        private int MinimumDistance(int[] distance, bool[] shortestPathTreeSet, int verticesCount)
        {
            int min = int.MaxValue;
            int minIndex = 0;

            for (int v = 0; v < verticesCount; v++)
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
