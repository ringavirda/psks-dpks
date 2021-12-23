using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPCS.Domain
{
    public class Cluster16
    {
        public Unit[] Units { get; set; } = new Unit[16];

        public Cluster16()
        {
        }

        public List<Unit> AsList() => Units.ToList();

        public Cluster16 Right { get; set; }
        public Cluster16 Down { get; set; }

        public void AddRight(Cluster16 cluster)
        {
            Units[12].Relatives.Add(cluster.Units[0]);
            Units[13].Relatives.Add(cluster.Units[5]);
            Units[7].Relatives.Add(cluster.Units[4]);
            Units[8].Relatives.Add(cluster.Units[14]);

            cluster.Units[0].Relatives.Add(Units[12]);
            cluster.Units[5].Relatives.Add(Units[13]);
            cluster.Units[4].Relatives.Add(Units[7]);
            cluster.Units[14].Relatives.Add(Units[8]);
        }

        public void AddDown(Cluster16 cluster)
        {
            Units[15].Relatives.Add(cluster.Units[0]);
            Units[11].Relatives.Add(cluster.Units[1]);
            Units[10].Relatives.Add(cluster.Units[2]);
            Units[9].Relatives.Add(cluster.Units[12]);

            cluster.Units[0].Relatives.Add(Units[15]);
            cluster.Units[1].Relatives.Add(Units[11]);
            cluster.Units[2].Relatives.Add(Units[10]);
            cluster.Units[12].Relatives.Add(Units[9]);
        }
    }
}
