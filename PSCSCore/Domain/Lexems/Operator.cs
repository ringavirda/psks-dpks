namespace PSCS.Core.Domain.Lexems
{
    public class Operator : LexemBase
    {
        public Operator(string value)
            : base(value, LexemType.Operator)
        {
        }

        public LexemBase LeftOperand { get; set; } = NonLexem.Instance;
        public LexemBase RightOperand { get; set; } = NonLexem.Instance;
    }
}
