using System;
using System.Collections.Generic;

namespace PSCS.Core.Domain.Parallel
{
    public class ProcessorLog
    {
        public Dictionary<int, List<string>> CycleLog { get; set; } = new Dictionary<int, List<string>>();

        public Dictionary<int, Dictionary<string, int>> UnitLog { get; set; } = new Dictionary<int, Dictionary<string, int>>();

        public void LogExecution(int cycleId, ProcessorUnit unit, int executionDone, int executionMax)
        {
            CheckLogIntegrity(cycleId, unit);

            CycleLog[cycleId].Add($"{cycleId:D3}: [PU-{unit.Id}] Executing ({unit.RunningOperation.LeftOperand.Value} {unit.RunningOperation.Operator}" +
                $" {unit.RunningOperation.RightOperand.Value}) progress {executionDone}/{executionMax}");
            UnitLog[unit.Id]["Execution Time"]++;
        }

        public void LogDataRead(int cycleId, ProcessorUnit unit, ProcessorDatabank databank)
        {
            CheckLogIntegrity(cycleId, unit);
         
            CycleLog[cycleId].Add($"{cycleId:D3}: [PU-{unit.Id}] Reading data uning DB-{databank.Id}");
            UnitLog[unit.Id]["Datatransfer Time"]++;
        }

        public void LogDataWrite(int cycleId, ProcessorUnit unit, ProcessorDatabank databank)
        {
            CheckLogIntegrity(cycleId, unit);

            CycleLog[cycleId].Add($"{cycleId:D3}: [PU-{unit.Id}] Writing data uning DB-{databank.Id}");
            UnitLog[unit.Id]["Datatransfer Time"]++;
        }

        public void LogIdle(int cycleId, ProcessorUnit unit)
        {
            CheckLogIntegrity(cycleId, unit);

            CycleLog[cycleId].Add($"{cycleId:D3}: [PU-{unit.Id}] Idle");
            UnitLog[unit.Id]["Idle Time"]++;
        }

        private void CheckLogIntegrity(int cycleId, ProcessorUnit unit)
        {
            if (!CycleLog.ContainsKey(cycleId))
                CycleLog.Add(cycleId, new List<string>());
            if (!UnitLog.ContainsKey(unit.Id))
            {
                UnitLog.Add(unit.Id, new Dictionary<string, int>());
                UnitLog[unit.Id].Add("Execution Time", 0);
                UnitLog[unit.Id].Add("Datatransfer Time", 0);
                UnitLog[unit.Id].Add("Idle Time", 0);
            }
        }
    }
}
