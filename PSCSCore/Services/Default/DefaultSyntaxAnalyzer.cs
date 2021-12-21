using PSCS.Core.Domain.Errors;
using PSCS.Core.Domain.Lexems;
using PSCS.Core.Domain.RequestModel;
using PSCS.Core.Domain.ResponseModel;
using System.Collections.Generic;
using System.Linq;

namespace PSCS.Core.Services.Default
{
    public class DefaultSyntaxAnalyzer : AnalyzerBase, ISyntaxAnalyzer
    {
        private SyntaxResponseModel _response;
        private string _sourceString;
        private List<LexemBase> _sourceSequence;

        // "1+22*(sp-b)^2"
        public SyntaxResponseModel Analize(SyntaxRequestModel syntaxRequest)
        {
            Logger.Log("Starting syntax analysis");
            _response = new SyntaxResponseModel();
            _response.SourceString = syntaxRequest.SourceString;
            _sourceString = syntaxRequest.SourceString;
            _sourceSequence = syntaxRequest.SourceSequence;

            _response.ProcessedSequence = ProcessSequence(_sourceSequence);

            if (_response.Successful)
            {
                Logger.Log("Analysis successful, no errors found");
            }
            else
            {
                Logger.Log($"Analysis failed, [{_response.FailsCount}] errors found");
            }

            return _response;
        }

        private List<LexemBase> ProcessSequence(List<LexemBase> sequence, int start = 0)
        {
            List<LexemBase> sourceSequence = sequence;
            List<LexemBase> processedSequence = new List<LexemBase>();
            LexemBase lexem;
            for (int i = 0; i < sourceSequence.Count; i++)
            {
                lexem = sourceSequence[i];
                if (lexem.Type == LexemType.Value)
                {
                    var value = lexem as Value;

                    if (i == sourceSequence.Count - 1)
                    {
                        processedSequence.Add(value);
                        continue;
                    }

                    var nextLexem = sourceSequence[i + 1];
                    if (nextLexem.Type == LexemType.Operator)
                    {
                        if (Dictionary.IsContainerStart(nextLexem.Value))
                        {
                            LogError(i + start, $"Value [{value.Value}] cannot be directly followed by container start, missing operator", value, nextLexem);
                            continue;
                        }
                        else if (Dictionary.IsContainerEnd(nextLexem.Value)) continue;
                        else
                        {
                            value.RightOperator = nextLexem;
                        }
                    }
                    else
                    {
                        LogError(i + start, $"Value [{value}] cannot be directly folloved by {nextLexem.Type}, missing operator");
                        continue;
                    }

                    if (i == 0)
                    {
                        processedSequence.Add(value);
                        continue;
                    }

                    var prevLexem = sourceSequence[i - 1];
                    if (prevLexem.Type == LexemType.Operator)
                    {
                        if (Dictionary.IsContainerEnd(prevLexem.Value))
                        {
                            LogError(i - 1 + start, $"Value [{value.Value}] cannot directly follow container end, missing operator]", prevLexem, value);
                            continue;
                        }
                        else if (Dictionary.IsContainerStart(prevLexem.Value)) continue;
                        else
                        {
                            value.LeftOperator = prevLexem;
                            processedSequence.Add(value);
                            continue;
                        }
                    }
                }

                if (lexem.Type == LexemType.Operator)
                {
                    var oper = lexem as Operator;

                    if (Dictionary.IsContainerStart(oper.Value))
                    {
                        var end = FindContainerEnd(i, sourceSequence, start);
                        if (end == -1) continue;

                        var subSequence = sourceSequence.Skip(i + 1).Take(end - i - 1).ToList();
                        var container = new Container("()");
                        if (subSequence.Count == 0)
                        {
                            LogError(i + start, "Container cannot be empty", oper);
                            i++;
                            continue;
                        }
                        container.Sequence = ProcessSequence(subSequence, i + 1 + start);

                        if (i != 0)
                        {
                            var prevLexem = sourceSequence[i - 1];
                            if (prevLexem.Type == LexemType.Operator && !Dictionary.IsContainerStart(prevLexem.Value)
                                && !Dictionary.IsContainerEnd(prevLexem.Value))
                            {
                                container.LeftOperator = prevLexem;
                                (prevLexem as Operator).RightOperand = container;
                            }
                        }
                        if (end != sourceSequence.Count - 1)
                        {
                            var containerEnd = sourceSequence[end];
                            var nextLexem = sourceSequence[end + 1];

                            if (nextLexem.Type == LexemType.Value)
                            {
                                LogError(end + start, "Container end cannot be directly followed by value, missing operator", containerEnd, nextLexem);
                            }
                            if (Dictionary.IsContainerStart(nextLexem.Value))
                            {
                                LogError(end + start, "Container cannot be directly followed by another container, missing operator", containerEnd, nextLexem);
                            }
                            if (nextLexem.Type == LexemType.Operator && !Dictionary.IsContainerEnd(nextLexem.Value))
                            {
                                container.RightOperator = nextLexem;
                                (nextLexem as Operator).LeftOperand = container;
                            }
                        }
                        processedSequence.Add(container);
                        i += end - i;
                        continue;
                    }
                    else if (Dictionary.IsContainerEnd(oper.Value))
                    {
                        if (i == 0)
                        {
                            LogError(i + start, "Sequence cannot start with container end", oper);
                            continue;
                        }
                        LogError(i + start, "Container start is missing", oper);
                        continue;
                    }
                    else
                    {
                        if (i != 0)
                        {
                            var prevLexem = sourceSequence[i - 1];
                            if (Dictionary.IsContainerStart(prevLexem.Value))
                            {
                                LogError(i + start, $"Operator [{oper.Value}] cannot directly follow container start", prevLexem, oper);
                            }
                            if (prevLexem.Type == LexemType.Value)
                            {
                                oper.LeftOperand = prevLexem;
                                (prevLexem as Value).RightOperator = oper;
                            }
                        }

                        if (i != sourceSequence.Count - 1)
                        {
                            var nextLexem = sourceSequence[i + 1];
                            if (nextLexem.Type == LexemType.Operator)
                            {
                                if (Dictionary.IsContainerEnd(nextLexem.Value))
                                {
                                    LogError(i + start, $"Operator [{oper.Value}] cannot be directly followed by container end", oper, nextLexem);
                                }
                                else if (!Dictionary.IsContainerStart(nextLexem.Value)) 
                                {
                                    LogError(i + start, $"Operator [{oper.Value}] cannot be directly followed by another operator [{nextLexem.Value}], missing value", oper, nextLexem);
                                }
                                processedSequence.Add(oper);
                                continue;
                            }
                            if (nextLexem.Type == LexemType.Value)
                            {
                                oper.RightOperand = nextLexem;
                                (nextLexem as Value).LeftOperator = oper;
                            }
                        }

                        if (i == 0)
                        {
                            LogError(i + start, "Sequence cannot start with operator", oper);
                            continue;
                        }
                        if (i == sourceSequence.Count - 1)
                        {
                            LogError(i + start, "Sequence cannot end with operator", oper);
                            continue;
                        }

                        processedSequence.Add(oper);
                    }

                }
            }

            return processedSequence;
        }

        private void LogError(int i, string message, params LexemBase[] lexems)
        {
            var error = new SyntaxError(i, lexems.ToList(), message, _sourceSequence);
            _response.Errors.Add(error);
            Logger.Log(error);
        }

        private int FindContainerEnd(int start, List<LexemBase> sourceSequence, int origin = 0)
        {
            int innerContainersCount = 0;
            LexemBase current;
            for (int i = start + 1; i < sourceSequence.Count; i++)
            {
                current = sourceSequence[i];
                if (Dictionary.IsContainerStart(current.Value))
                {
                    innerContainersCount++;
                }
                if (Dictionary.IsContainerEnd(current.Value))
                {
                    if (innerContainersCount > 0)
                    {
                        innerContainersCount--;
                        continue;
                    }
                    return i;
                }
            }

            LogError(start + origin + 1, "Container end is missing", sourceSequence[start]);
            return -1;
        }
    }
}