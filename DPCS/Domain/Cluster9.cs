using System.Collections.Generic;

namespace DPCS.Domain
{
    public class Cluster9
    {
        public Unit Unit1 { get; set; }
        public Unit Unit2 { get; set; }
        public Unit Unit3 { get; set; }
        public Unit Unit4 { get; set; }
        public Unit Unit5 { get; set; }
        public Unit Unit6 { get; set; }
        public Unit Unit7 { get; set; }
        public Unit Unit8 { get; set; }
        public Unit Unit9 { get; set; }

        public Cluster9 LeftClild { get; private set; }
        public Cluster9 RightClild { get; private set; }

        public bool HasOnlyOneChild() => LeftClild != null ^ RightClild != null;
        public bool HasNoChildren() => LeftClild == null && RightClild == null;
        public List<Unit> AsList() => new List<Unit> { Unit1, Unit2, Unit3, Unit4, Unit5, Unit6, Unit7, Unit8, Unit9 };
    
        public void AddLeftChild(Cluster9 child)
        {
            Unit2.Relatives.Add(child.Unit2);
            Unit6.Relatives.Add(child.Unit4);
            Unit8.Relatives.Add(child.Unit8);

            child.Unit2.Relatives.Add(Unit2);
            child.Unit4.Relatives.Add(Unit6);
            child.Unit8.Relatives.Add(Unit8);
            
            LeftClild = child;
        }

        public void AddRightChild(Cluster9 child)
        {
            Unit6.Relatives.Add(child.Unit6);
            Unit8.Relatives.Add(child.Unit2);
            Unit4.Relatives.Add(child.Unit4);
         
            child.Unit6.Relatives.Add(Unit6);
            child.Unit2.Relatives.Add(Unit8);
            child.Unit4.Relatives.Add(Unit4);

            RightClild = child;
        }
    }
}
