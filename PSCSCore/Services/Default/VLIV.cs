using PSCS.Core.Domain.Parallel;
using PSCS.Core.Domain.RequestModel;
using PSCS.Core.Domain.ResponseModel;
using System.Collections.Generic;
using System.Linq;

namespace PSCS.Core.Services.Default
{
    // a-j+b-c*(e+d/f-(x+z*v))*l
    // (A+B)+C/D+G+(K/L+M+N)
    // a/b/c/d/e/f/l
    // a-b-c-d-e-f-l
    public class VLIV : ProcessorBase
    {
        private StateTree _stateTree;
        private ProcessorLog _log;
        private int _cycleCounter;

        public override ProcessorResponseModel Execute(ProcessorRequestModel requestModel)
        {
            _stateTree = requestModel.StateTree;
            foreach(var layer in _stateTree.Layers)
            {
                foreach (var expr in layer.Expressions)
                    expr.Executed = false;
            }
            _log = new ProcessorLog();
            _cycleCounter = 0;

            var ready = GetReadyExressions();
            while (ready.Count > 0 || ProcessorUnits.Any(u => u.InUse))
            {
                FlushDatabanks();
                if (ready.Count > ProcessorUnitsCount)
                    ready = ready.Take(ProcessorUnitsCount).ToList();

                if (ProcessorUnits.All(p => !p.InUse))
                    AddToExecute(ready);

                RunCycle();

                ready = GetReadyExressions();
            }


            return new ProcessorResponseModel { SourceString = requestModel.SourceString, ProcessorLog = _log };
        }

        private void RunCycle()
        {
            _cycleCounter++;
            foreach (var unit in ProcessorUnits)
            {
                unit.RunCycle(_cycleCounter, _log, this);
            }
        }

        private void FlushDatabanks()
        {
            foreach (var databank in ProcessorDatabanks)
            {
                databank.InUse = false;
            }
        }

        private void AddToExecute(List<Expression> ready)
        {
            foreach (var expr in ready)
            {
                var free = ProcessorUnits.FirstOrDefault(p => !p.InUse);
                if (free == null) break;
                free.InUse = true;
                free.RunningOperation = expr;
                expr.Executed = true;
            }   
        }

        private List<Expression> GetReadyExressions()
            => _stateTree.Layers.SelectMany(l => l.Expressions.FindAll(e => !e.Executed && (e.Parents.Count == 0 || e.Parents.All(p => p.Executed)))).ToList();

        private List<Expression> GetSame(List<Expression> expressions)
        {
            var operators = expressions.Select(e => e.Operator).ToList();
            var unique = operators.ToHashSet();
            var counts = new Dictionary<string, int>();
            foreach (var item in unique)
                counts.Add(item, operators.FindAll(o => o == item).Count);

            if (counts.Values.Any(v => v > 1))
            {
                var oper = counts.FirstOrDefault(p => p.Value == counts.Values.Max()).Key;
                return expressions.FindAll(e => e.Operator == oper);
            }
            else
                return new List<Expression> { expressions[0] };
        }
    }
}
