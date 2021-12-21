using System;
using System.Collections.Generic;
using System.Linq;
using DPCS.Domain;

namespace DPCS
{
    public class Topology2
    {
        private static int COUNT_OF_UNITS_IN_CLUSTER = 9;

        private Topology2() { }
        public Topology2(int coutOfClusters)
        {
            if (coutOfClusters <= 0)
            {
                throw new ArgumentException("Count of clusters can be only positive");
            }
            CountOfVertices = COUNT_OF_UNITS_IN_CLUSTER * coutOfClusters;

            RelationTable = Replicate(GenerateTopology(coutOfClusters));

            ShortestPathTable = UnleashDijkstra(coutOfClusters);
        }


        public int[,] RelationTable { get; private set; }
        public int[,] ShortestPathTable { get; private set; }
        public int CountOfVertices { get; private set; }

        private List<Cluster9> GenerateTopology(int countOfClusters)
        {
            List<Cluster9> topology = new List<Cluster9>();
            
            Cluster9 cluster;
            int startingPoint = 0;
            for (int i = 1; i < countOfClusters + 1; i++)
            {
                cluster = new Cluster9();
                cluster.Unit1 = new Unit { ID = startingPoint + 1 };
                cluster.Unit2 = new Unit { ID = startingPoint + 2 };
                cluster.Unit3 = new Unit { ID = startingPoint + 3 };
                cluster.Unit4 = new Unit { ID = startingPoint + 4 };
                cluster.Unit5 = new Unit { ID = startingPoint + 5 };
                cluster.Unit6 = new Unit { ID = startingPoint + 6 };
                cluster.Unit7 = new Unit { ID = startingPoint + 7 };
                cluster.Unit8 = new Unit { ID = startingPoint + 8 };
                cluster.Unit9 = new Unit { ID = startingPoint + 9 };

                startingPoint += COUNT_OF_UNITS_IN_CLUSTER;

                cluster.Unit1.Relatives.AddRange(new List<Unit> { cluster.Unit2, cluster.Unit6 });
                cluster.Unit2.Relatives.AddRange(new List<Unit> { cluster.Unit1, cluster.Unit3, cluster.Unit5 });
                cluster.Unit3.Relatives.AddRange(new List<Unit> { cluster.Unit2, cluster.Unit4 });
                cluster.Unit4.Relatives.AddRange(new List<Unit> { cluster.Unit3, cluster.Unit5, cluster.Unit9 });
                cluster.Unit5.Relatives.AddRange(new List<Unit> { cluster.Unit2, cluster.Unit4, cluster.Unit6, cluster.Unit8 });
                cluster.Unit6.Relatives.AddRange(new List<Unit> { cluster.Unit1, cluster.Unit7, cluster.Unit5 });
                cluster.Unit7.Relatives.AddRange(new List<Unit> { cluster.Unit6, cluster.Unit8 });
                cluster.Unit8.Relatives.AddRange(new List<Unit> { cluster.Unit7, cluster.Unit5, cluster.Unit9 });
                cluster.Unit9.Relatives.AddRange(new List<Unit> { cluster.Unit4, cluster.Unit8 });
                

                var parent = topology.FirstOrDefault(cl => cl.HasOnlyOneChild());
                if (parent == null)
                {
                    parent = topology.FirstOrDefault(cl => cl.HasNoChildren());
                    if (parent == null)
                    {
                        topology.Add(cluster);
                        continue;
                    }
                }
                if (parent.LeftClild == null) parent.AddLeftChild(cluster);
                else if (parent.RightClild == null) parent.AddRightChild(cluster);
                topology.Add(cluster);
            }
            return topology;
        }

        private int[,] Replicate(List<Cluster9> topology)
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
