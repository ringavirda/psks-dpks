using PSCS.Core.Services;
using PSCS.Core.Services.Default;

namespace PSCS.Core
{
    public class CompilerConf
    {
        private CompilerConf _defaultConf;
        private CompilerConf _conf;
        
        public CompilerConf()
        {
            _conf = this;
            _conf.SetLogger<DefaultLogger>()
                .SetDictionary<DefaultDictionary>()
                .SetLexemAnalizer<DefaultLexemAnalyzer>()
                .SetSyntaxAnalizer<DefaultSyntaxAnalyzer>()
                .SetParallelizer<DefaultParallelizer>()
                .SetProcessor<VLIV>();
            _defaultConf = _conf;
        }

        public CompilerConf Default { get => _defaultConf; }

        public ILogger Logger { get; private set; }
        public IDictionary Dictionary { get; private set; }
        public ILexemAnalyzer LexemAnalizer { get; private set; }
        public ISyntaxAnalyzer SyntaxAnalizer { get; private set; }
        public IParallelizer Parallelizer { get; private set; }
        public ProcessorBase Processor { get; private set; }

        public CompilerConf SetLexemAnalizer<T>() where T : AnalyzerBase, ILexemAnalyzer, new()
        {
            LexemAnalizer = new T();
            (LexemAnalizer as AnalyzerBase).Logger = Logger;
            (LexemAnalizer as AnalyzerBase).Dictionary = Dictionary;
            return _conf;
        }

        public CompilerConf SetSyntaxAnalizer<T>() where T : AnalyzerBase, ISyntaxAnalyzer, new()
        {
            SyntaxAnalizer = new T();
            (SyntaxAnalizer as AnalyzerBase).Logger = Logger;
            (SyntaxAnalizer as AnalyzerBase).Dictionary = Dictionary;
            return _conf;
        }
        
        public CompilerConf SetParallelizer<T>() where T : AnalyzerBase, IParallelizer, new()
        {
            Parallelizer = new T();
            (Parallelizer as AnalyzerBase).Logger = Logger;
            (Parallelizer as AnalyzerBase).Dictionary = Dictionary;
            return _conf;
        }

        public CompilerConf SetProcessor<T>() where T : ProcessorBase, new()
        {
            Processor = new T();
            return _conf;
        }

        public CompilerConf SetDictionary<T>() where T : IDictionary, new()
        {
            Dictionary = new T();
            if (LexemAnalizer != null)
                (LexemAnalizer as AnalyzerBase).Dictionary = Dictionary;
            if (SyntaxAnalizer != null)
                (SyntaxAnalizer as AnalyzerBase).Dictionary = Dictionary;
            if (Parallelizer != null)
                (Parallelizer as AnalyzerBase).Dictionary = Dictionary;
            return _conf;
        }

        public CompilerConf SetLogger<T>() where T : ILogger, new()
        {
            Logger = new T();
            if (LexemAnalizer != null)
                (LexemAnalizer as AnalyzerBase).Logger = Logger;
            if (SyntaxAnalizer != null)
                (SyntaxAnalizer as AnalyzerBase).Logger = Logger;
            if (Parallelizer != null)
                (Parallelizer as AnalyzerBase).Logger = Logger;
            return _conf;
        }
    }
}