using PSCS.Core.Domain.Lexems;
using PSCS.Core.Domain.Parallel;
using PSCS.Core.Domain.RequestModel;
using PSCS.Core.Domain.ResponseModel;
using System.Collections.Generic;
using System.Linq;

namespace PSCS.Core.Services.Default
{
    public class DefaultParallelizer : AnalyzerBase, IParallelizer
    {
        private List<LexemBase> _processedSequence;
        private string _source;
        private List<Expression> _storage;

        
        public ParallelizerResponseModel Parallelize(ParallelizerRequestModel requestModel)
        {
            _processedSequence = requestModel.ProcessedSequence;
            _source = requestModel.SourceString;
            _storage = new List<Expression>();

            var stateTree = new StateTree();

            var whatIsThis = Containerise(_processedSequence);
            var deconstr = DeconstructSequence(whatIsThis);
            var almost = FindRelatives(deconstr);
            almost.AddRange(_storage);

            var numberOfLayers = almost.Max(expr => expr.Distance) + 1;
            for (int i = 0; i < numberOfLayers; i++)
                stateTree.Layers.Add(new Layer());

            foreach (var expr in almost)
            {
                stateTree.Layers[expr.Distance].Expressions.Add(expr);
            }

            return new ParallelizerResponseModel() { SourceString = _source, Tree = stateTree };
        }

        private List<LexemBase> Containerise(List<LexemBase> sequence)
        {
            var containerized = sequence;
            while (true)
            {
                var repetitions = ExtractRepetitions(containerized);
                if (repetitions == null) break;
                int start = containerized.IndexOf(repetitions.First());
                int len = repetitions.Count;
                var operators = repetitions.FindAll(rep => rep.Type == LexemType.Operator);

                if (operators.Count > 2)
                {
                    for (int i = 0; i < operators.Count; i += 2)
                    {
                        var container = new Container("()");
                        var oper = operators[i] as Operator;

                        if (oper.Value == "-" && i > 0)
                            oper.Value = "+";
                        if (oper.Value == "/" && i > 0)
                            oper.Value = "*";
                        container.Sequence = new List<LexemBase> { oper.LeftOperand, oper, oper.RightOperand };
                        
                        if (i - 1 > 0)
                            (operators[i - 1] as Operator).RightOperand = container;
                        if (i + 1 < operators.Count)
                            (operators[i + 1] as Operator).LeftOperand = container;

                        int index = repetitions.IndexOf(oper.LeftOperand);
                        repetitions.RemoveRange(index, 3);
                        repetitions.Insert(index, container);
                    }

                    containerized.RemoveRange(start, len);
                    containerized.InsertRange(start, repetitions);
                }
                else break;
            }
            return containerized;
        }

        private List<LexemBase> ExtractRepetitions(List<LexemBase> sequence)
        {
            for (int i = 0; i < sequence.Count - 1; i++)
            {
                if (sequence[i].Type == LexemType.Operator)
                {
                    var repetitions = new List<LexemBase>();
                    for (int j = i; j < sequence.Count; j++)
                    {
                        if (sequence[j].Type == LexemType.Operator)
                        {
                            if (sequence[j].Value == sequence[i].Value)
                            {
                                repetitions.Add(sequence[j - 1]);
                                repetitions.Add(sequence[j]);
                            }
                            else break;
                        }
                    }
                    if (repetitions.Count > 4)
                    {
                        repetitions.Add((repetitions.Last() as Operator).RightOperand);
                        return repetitions;
                    }
                }
            }
            return null;
        }

        private List<Expression> DeconstructSequence(List<LexemBase> sequence)
        {
            var exprs = new List<Expression>();

            for (int i = 0; i < sequence.Count; i++)
            {
                var current = sequence[i];
                if (current.Type == LexemType.Container)
                {
                    var containerised = Containerise((current as Container).Sequence);
                    var container = DeconstructSequence(containerised);
                    container = FindRelatives(container);
                    _storage.AddRange(container);
                    var maxDistance = container.Max(expr => expr.Distance);
                    var root = container.Find(expr => expr.Distance == maxDistance);
                    if (i != 0 && !new List<string> { "*", "/" }.Contains((current as Container).RightOperator?.Value))
                    {
                        var prev = exprs.Last();
                        prev.Parents.Add(root);
                        root.Child = prev;
                        prev.RightOperand = root.Result();
                        prev.Distance = (prev.Distance > root.Distance + 1) ? prev.Distance : root.Distance + 1;
                    }
                    else
                    {
                        var next = sequence[++i];
                        var expr = new Expression();
                        expr.LeftOperand = root.Result();
                        expr.Parents.Add(root);
                        root.Child = expr;
                        expr.Operator = next.Value;
                        expr.RightOperand = (next as Operator).RightOperand;
                        expr.Distance = (expr.Distance > root.Distance + 1) ? expr.Distance : root.Distance + 1;
                        exprs.Add(expr);
                    }
                    continue;
                }

                if (current.Type == LexemType.Operator)
                {
                    var expr = new Expression();
                    expr.LeftOperand = (current as Operator).LeftOperand;
                    expr.Operator = current.Value;
                    expr.RightOperand = (current as Operator).RightOperand;
                    exprs.Add(expr);
                }
            }
            return exprs;
        }

        private List<Expression> FindRelatives(List<Expression> sequence)
        {
            for (int i = 0; i < sequence.Count; i++)
            {
                var current = sequence[i];
                if (current.Operator == "*"
                    || current.Operator == "/")
                {
                    AddRelatives(sequence, i);
                }
            }
            for (int i = 0; i < sequence.Count; i++)
            {
                var current = sequence[i];
                if (current.Operator == "+" 
                    || current.Operator == "-")
                {
                    AddRelatives(sequence, i);
                }
            }
            return sequence;
        }

        private void AddRelatives(List<Expression> sequence, int i)
        {
            var current = sequence[i];
            if (i < sequence.Count - 1)
            {
                var next = sequence[i + 1];
                if (next.Child != null && next.Parents.Count > 0)
                    next = FindMostDistantChild(next);
                if (new List<string> { "*", "/" }.Contains(next.Operator))
                {
                    current.RightOperand = next.Result();
                    current.Parents.Add(next);
                    next.Child = current;
                    current.Distance = (current.Distance > next.Distance + 1) ? current.Distance : next.Distance + 1;
                    return;
                }
                else
                {
                    next.Parents.Add(current);
                    current.Child = next;
                    next.LeftOperand = current.Result();
                    next.Distance = (next.Distance > current.Distance + 1) ? next.Distance : current.Distance + 1;
                    return;
                }
            }
            if (i > 0)
            {
                var prev = sequence[i - 1];
                if (prev.Child != null)
                {
                    prev = FindMostDistantChild(prev);
                    if (prev == current) return;
                    prev.Child = current;
                    current.Parents.Add(prev);
                    current.LeftOperand = prev.Result();
                    current.Distance = (current.Distance > prev.Distance + 1) ? current.Distance : prev.Distance + 1;
                }
            }
        }

        private Expression FindMostDistantChild(Expression expr)
        {
            if (expr.Child != null)
            {
                return FindMostDistantChild(expr.Child);
            }
            return expr;
        }
    }
}
