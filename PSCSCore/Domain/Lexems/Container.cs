using System.Collections.Generic;

namespace PSCS.Core.Domain.Lexems
{
    public class Container : LexemBase
    {
        public Container(string value)
            : base(value, LexemType.Container)
        {
        }

        public List<LexemBase> Sequence { get; set; } = new List<LexemBase>();
        public LexemBase LeftOperator { get; set; }
        public LexemBase RightOperator { get; set; }
    }
}
