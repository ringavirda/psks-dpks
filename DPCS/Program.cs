using System;
using System.Collections.Generic;
using System.Linq;

namespace DPCS
{
    public class Program
    {

        public static void Main(string[] args)
        {
            var topologies = new List<Topology3>();

            var specs = new List<int> { 1, 4, 9, 16 };
            specs.ForEach(spec => topologies.Add(new Topology3(spec)));

            for (int i = 0; i < 4 * 7; i++)
            {
                for (int l = 0; l < 4 * 7; l++)
                {
                    Console.Write($"{topologies[1].RelationTable[i, l]} ");
                }
                Console.WriteLine();
            }
            
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();

            foreach (var topology in topologies)
            {
                Console.WriteLine($"{topology.CountOfVertices}: {topology.ShortestPathTable.Cast<int>().Max()} " +
                    $"{Math.Round(topology.ShortestPathTable.Cast<int>().Sum() / (double)(topology.CountOfVertices * (topology.CountOfVertices - 1)), 3)} " +
                    $"{Enumerable.Range(0, topology.RelationTable.GetLength(0)).Select(i => Enumerable.Range(0, topology.RelationTable.GetLength(1)).Select(j => topology.RelationTable[i, j])).Select(row => row.Sum()).Max()} " +
                    $"{topology.ShortestPathTable.Cast<int>().Max() * topology.CountOfVertices * 6} " +
                    $"{Math.Round(topology.ShortestPathTable.Cast<int>().Sum() / (double) (topology.CountOfVertices * (topology.CountOfVertices - 1)) * 2 / 6, 3)}");
            }
        }
    }
}
