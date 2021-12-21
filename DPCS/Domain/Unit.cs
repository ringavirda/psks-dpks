using System.Collections.Generic;

namespace DPCS.Domain
{
    public class Unit
    {
        public int ID { get; set; }
        public List<Unit> Relatives { get; set; } = new List<Unit>();
    }
}
