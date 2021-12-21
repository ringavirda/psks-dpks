using PSCS.Core.Domain.Errors;
using PSCS.Core.Domain.Lexems;
using PSCS.Core.Domain.RequestModel;
using PSCS.Core.Domain.ResponseModel;
using System.Linq;

namespace PSCS.Core.Services.Default
{
    public class DefaultLexemAnalyzer : AnalyzerBase, ILexemAnalyzer
    {
        private LexemResponseModel _responseModel;
        private string _source;

        public LexemResponseModel Analize(LexemRequestModel lexemRequest)
        {
            Logger.Log("Starting lexem analysis");
            _responseModel = new LexemResponseModel();
            _responseModel.SourceString = lexemRequest.SourceString;
            _source = lexemRequest.SourceString;

            string sample;
            for (int i = 0; i < _source.Length; i++)
            {
                sample = _source.Substring(i, 1);

                if (!Dictionary.IsAllowedSymbol(sample))
                {
                    LogError(i, sample, "Unrecognized symbol");
                    continue;
                }

                if (Dictionary.IsOperator(sample))
                {
                    _responseModel.Sequence.Add(new Operator(sample));
                    continue;
                }

                string word = sample;
                int length = 1;
                int designatorsCount = 0;
                while (i + length < _source.Length && 
                    (char.IsLetterOrDigit(_source[i + length]) || Dictionary.IsFloatDesignator(_source[i + length].ToString())))
                {
                    if (Dictionary.IsFloatDesignator(_source[i + length].ToString()))
                    {
                        designatorsCount++;
                        if (designatorsCount > 1)
                        {
                            LogError(i, word, "Digit cannot contain more then 1 float designators");
                            i += length - 1;
                            continue;
                        }
                    }
                    word = _source.Substring(i, ++length);
                }

                if (word.Length > 1 && word.Any((c) => Dictionary.IsLetter(c.ToString()))
                    && word.Any((c) => Dictionary.IsFloatDesignator(c.ToString())))
                {
                    LogError(i, word, "Variable cannot contain float designators");
                    i += length - 1;
                    continue;
                }

                if (word.Length > 1 && char.IsDigit(word[0])
                    && word.Substring(1, word.Length - 1).Any(char.IsLetter))
                {
                    LogError(i, word, "Variable cannot start with digit");
                    i += length - 1;
                    continue;
                }

                i += length - 1;
                _responseModel.Sequence.Add(new Value(word));
            }

            if (_responseModel.FailsCount > 0)
            {
                Logger.Log($"Analysis failed, [{_responseModel.FailsCount}] errors found");
                return _responseModel;
            }
            else
            {
                Logger.Log($"Analysis successful, no errors found");
                return _responseModel;
            }
        }

        private void LogError(int i, string lexem, string message)
        {
            var error = new LexemError(i, lexem, message, _source);
            _responseModel.Errors.Add(error);
            Logger.Log(error);
        }
    }
}