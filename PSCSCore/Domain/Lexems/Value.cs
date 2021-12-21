namespace PSCS.Core.Domain.Lexems
{
    public class Value : LexemBase
    {
        public Value(string value)
            : base(value, LexemType.Value)
        {
        }

        public LexemBase LeftOperator { get; set; } = NonLexem.Instance;
        public LexemBase RightOperator { get; set; } = NonLexem.Instance;
    }
}
