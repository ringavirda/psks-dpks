using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPCS
{
    public class Topology1
    {
        private static int COUNT_OF_UNITS_IN_CLUSTER = 9;

        private Topology1() { }
        public Topology1(int coutOfClusters)
        {
            if (coutOfClusters <= 0)
            {
                throw new ArgumentException("Count of clusters can be only positive");
            }
            CountOfVertices = COUNT_OF_UNITS_IN_CLUSTER * coutOfClusters;
            
            RelationTable = Replicate(coutOfClusters);

            ShortestPathTable = UnleashDijkstra(coutOfClusters);
        }

        public int[,] RelationTable { get; private set; }
        public int[,] ShortestPathTable { get; private set; }
        public int CountOfVertices { get; private set; }

        private int[,] Replicate(int coutOfClusters)
        {
            var table = new int[CountOfVertices, CountOfVertices];
            for (int i = 0; i < coutOfClusters; i++)
            {
                table[i * COUNT_OF_UNITS_IN_CLUSTER + 0, i * COUNT_OF_UNITS_IN_CLUSTER + 1] = 1;
                table[i * COUNT_OF_UNITS_IN_CLUSTER + 0, i * COUNT_OF_UNITS_IN_CLUSTER + 2] = 1;
                table[i * COUNT_OF_UNITS_IN_CLUSTER + 0, i * COUNT_OF_UNITS_IN_CLUSTER + 3] = 1;

                table[i * COUNT_OF_UNITS_IN_CLUSTER + 1, i * COUNT_OF_UNITS_IN_CLUSTER + 0] = 1;
                table[i * COUNT_OF_UNITS_IN_CLUSTER + 1, i * COUNT_OF_UNITS_IN_CLUSTER + 2] = 1;
                table[i * COUNT_OF_UNITS_IN_CLUSTER + 1, i * COUNT_OF_UNITS_IN_CLUSTER + 4] = 1;

                table[i * COUNT_OF_UNITS_IN_CLUSTER + 2, i * COUNT_OF_UNITS_IN_CLUSTER + 0] = 1;
                table[i * COUNT_OF_UNITS_IN_CLUSTER + 2, i * COUNT_OF_UNITS_IN_CLUSTER + 1] = 1;
                table[i * COUNT_OF_UNITS_IN_CLUSTER + 2, i * COUNT_OF_UNITS_IN_CLUSTER + 3] = 1;
                table[i * COUNT_OF_UNITS_IN_CLUSTER + 2, i * COUNT_OF_UNITS_IN_CLUSTER + 4] = 1;

                table[i * COUNT_OF_UNITS_IN_CLUSTER + 3, i * COUNT_OF_UNITS_IN_CLUSTER + 0] = 1;
                table[i * COUNT_OF_UNITS_IN_CLUSTER + 3, i * COUNT_OF_UNITS_IN_CLUSTER + 2] = 1;
                table[i * COUNT_OF_UNITS_IN_CLUSTER + 3, i * COUNT_OF_UNITS_IN_CLUSTER + 4] = 1;

                table[i * COUNT_OF_UNITS_IN_CLUSTER + 4, i * COUNT_OF_UNITS_IN_CLUSTER + 1] = 1;
                table[i * COUNT_OF_UNITS_IN_CLUSTER + 4, i * COUNT_OF_UNITS_IN_CLUSTER + 2] = 1;
                table[i * COUNT_OF_UNITS_IN_CLUSTER + 4, i * COUNT_OF_UNITS_IN_CLUSTER + 3] = 1;
                table[i * COUNT_OF_UNITS_IN_CLUSTER + 4, i * COUNT_OF_UNITS_IN_CLUSTER + 5] = 1;
                table[i * COUNT_OF_UNITS_IN_CLUSTER + 4, i * COUNT_OF_UNITS_IN_CLUSTER + 6] = 1;
                table[i * COUNT_OF_UNITS_IN_CLUSTER + 4, i * COUNT_OF_UNITS_IN_CLUSTER + 7] = 1;
                
                table[i * COUNT_OF_UNITS_IN_CLUSTER + 5, i * COUNT_OF_UNITS_IN_CLUSTER + 4] = 1;
                table[i * COUNT_OF_UNITS_IN_CLUSTER + 5, i * COUNT_OF_UNITS_IN_CLUSTER + 6] = 1;
                table[i * COUNT_OF_UNITS_IN_CLUSTER + 5, i * COUNT_OF_UNITS_IN_CLUSTER + 8] = 1;

                table[i * COUNT_OF_UNITS_IN_CLUSTER + 6, i * COUNT_OF_UNITS_IN_CLUSTER + 4] = 1;
                table[i * COUNT_OF_UNITS_IN_CLUSTER + 6, i * COUNT_OF_UNITS_IN_CLUSTER + 5] = 1;
                table[i * COUNT_OF_UNITS_IN_CLUSTER + 6, i * COUNT_OF_UNITS_IN_CLUSTER + 7] = 1;
                table[i * COUNT_OF_UNITS_IN_CLUSTER + 6, i * COUNT_OF_UNITS_IN_CLUSTER + 8] = 1;

                table[i * COUNT_OF_UNITS_IN_CLUSTER + 7, i * COUNT_OF_UNITS_IN_CLUSTER + 4] = 1;
                table[i * COUNT_OF_UNITS_IN_CLUSTER + 7, i * COUNT_OF_UNITS_IN_CLUSTER + 6] = 1;
                table[i * COUNT_OF_UNITS_IN_CLUSTER + 7, i * COUNT_OF_UNITS_IN_CLUSTER + 8] = 1;

                table[i * COUNT_OF_UNITS_IN_CLUSTER + 8, i * COUNT_OF_UNITS_IN_CLUSTER + 5] = 1;
                table[i * COUNT_OF_UNITS_IN_CLUSTER + 8, i * COUNT_OF_UNITS_IN_CLUSTER + 6] = 1;
                table[i * COUNT_OF_UNITS_IN_CLUSTER + 8, i * COUNT_OF_UNITS_IN_CLUSTER + 7] = 1;
            }

            for (int i = 1; i <= coutOfClusters; i++)
            {
                if (i < coutOfClusters)
                {
                    table[i * COUNT_OF_UNITS_IN_CLUSTER - 1, i * COUNT_OF_UNITS_IN_CLUSTER] = 1;
                    table[i * COUNT_OF_UNITS_IN_CLUSTER, i * COUNT_OF_UNITS_IN_CLUSTER - 1] = 1;

                    table[i * COUNT_OF_UNITS_IN_CLUSTER - 8, i * COUNT_OF_UNITS_IN_CLUSTER + 1] = 1;
                    table[i * COUNT_OF_UNITS_IN_CLUSTER + 1, i * COUNT_OF_UNITS_IN_CLUSTER - 8] = 1;
                    
                    table[i * COUNT_OF_UNITS_IN_CLUSTER + 7, i * COUNT_OF_UNITS_IN_CLUSTER - 2] = 1;
                    table[i * COUNT_OF_UNITS_IN_CLUSTER - 2, i * COUNT_OF_UNITS_IN_CLUSTER + 7] = 1;
                }
                else
                {
                    table[0, i * COUNT_OF_UNITS_IN_CLUSTER - 1] = 1;
                    table[i * COUNT_OF_UNITS_IN_CLUSTER - 1, 0] = 1;

                    table[2, i * COUNT_OF_UNITS_IN_CLUSTER - 8] = 1;
                    table[i * COUNT_OF_UNITS_IN_CLUSTER - 8, 2] = 1;
                    
                    table[7, i * COUNT_OF_UNITS_IN_CLUSTER - 2] = 1;
                    table[i * COUNT_OF_UNITS_IN_CLUSTER - 2, 7] = 1;
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
