namespace PSCS.Core.Domain.Lexems
{
    public class NonLexem : LexemBase
    {
        private static NonLexem _instance;

        private NonLexem()
            : base("", LexemType.None) { }

        public static LexemBase Instance
        {
            get {
                if (_instance == null)
                {
                    _instance = new NonLexem();
                }
                return _instance;
            }
        }
    }
}
