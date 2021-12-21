namespace PSCS.Core.Domain.Lexems
{
    public abstract class LexemBase
    {
        public LexemBase(string val, LexemType type)
        {
            Value = val;
            Type = type;
        }

        public string Value { get; set; }
        public LexemType Type { get; private set; }
    }
}
