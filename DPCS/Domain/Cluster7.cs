using System.Collections.Generic;

namespace DPCS.Domain
{
    public class Cluster7
    {
        public Unit Unit1 { get; set; }
        public Unit Unit2 { get; set; }
        public Unit Unit3 { get; set; }
        public Unit Unit4 { get; set; }
        public Unit Unit5 { get; set; }
        public Unit Unit6 { get; set; }
        public Unit Unit7 { get; set; }

        public List<Unit> AsList() => new List<Unit> { Unit1, Unit2, Unit3, Unit4, Unit5, Unit6, Unit7 };

        public Cluster7 Right { get; set; }
        public Cluster7 Down { get; set; }

        public void AddRight(Cluster7 cluster)
        {
            Unit3.Relatives.Add(cluster.Unit1);
            Unit7.Relatives.Add(cluster.Unit5);

            cluster.Unit1.Relatives.Add(Unit3);
            cluster.Unit5.Relatives.Add(Unit7);
        }

        public void AddDown(Cluster7 cluster)
        {
            Unit5.Relatives.Add(cluster.Unit1);
            Unit6.Relatives.Add(cluster.Unit2);
            Unit7.Relatives.Add(cluster.Unit3);

            cluster.Unit1.Relatives.Add(Unit5);
            cluster.Unit2.Relatives.Add(Unit6);
            cluster.Unit3.Relatives.Add(Unit7);
        }
    }
}
