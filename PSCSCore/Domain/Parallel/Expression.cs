using PSCS.Core.Domain.Lexems;
using System;
using System.Collections.Generic;

namespace PSCS.Core.Domain.Parallel
{
    public class Expression : ICloneable
    {
        private LexemBase _result;

        public LexemBase LeftOperand { get; set; }
        public string Operator { get; set; }
        public LexemBase RightOperand { get; set; }

        public List<Expression> Parents { get; set; } = new List<Expression>();
        public Expression Child { get; set; }
        public int Distance { get; set; } = 0;

        public bool Executed { get; set; } = false;

        public object Clone()
            => new Expression
            {
                Child = Child,
                Distance = Distance,
                Executed = false,
                LeftOperand = LeftOperand,
                Operator = Operator,
                Parents = Parents,
                RightOperand = RightOperand
            };

        public LexemBase Result()
        {
            if (_result == null)
                _result = new Value(LeftOperand.Value + RightOperand.Value);
            return _result;
        }
    }
}
