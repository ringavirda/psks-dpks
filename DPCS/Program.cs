using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DPCS
{
    public class Program
    {

        public static void Main(string[] args)
        {
            var topologies = new List<Topology4>();

            var specs = new List<int> { 1, 4, 9, 16, 25, 36, 49, 64, 81, 100, 121, 144 };
            specs.ForEach(spec => topologies.Add(new Topology4(spec)));

            for (int i = 0; i < 1 * 16; i++)
            {
                for (int l = 0; l < 1 * 16; l++)
                {
                    Console.Write($"{topologies[0].ShortestPathTable[i, l]} ");
                }
                Console.WriteLine();
            }
            
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();

            foreach (var topology in topologies)
            {
                StringBuilder builder = new StringBuilder();
                builder.Append($"{topology.CountOfVertices}: {topology.ShortestPathTable.Cast<int>().Max()} ");
                builder.Append($"{Math.Round(topology.ShortestPathTable.Cast<int>().Sum() / (double)(topology.CountOfVertices * (topology.CountOfVertices - 1)), 3)} ");
                builder.Append($"{Enumerable.Range(0, topology.RelationTable.GetLength(0)).Select(i => Enumerable.Range(0, topology.RelationTable.GetLength(1)).Select(j => topology.RelationTable[i, j])).Select(row => row.Sum()).Max()} ");
                builder.Append($"{topology.ShortestPathTable.Cast<int>().Max() * topology.CountOfVertices * 6} ");
                builder.Append($"{Math.Round(topology.ShortestPathTable.Cast<int>().Sum() / (double)(topology.CountOfVertices * (topology.CountOfVertices - 1)) * 2 / 6, 3)}");
                Console.WriteLine(builder.ToString());
            }
        }
    }
}
