using PSCS.Core.Domain.Parallel;
using PSCS.Core.Domain.RequestModel;
using PSCS.Core.Domain.ResponseModel;
using System.Collections.Generic;

namespace PSCS.Core.Services
{
    public abstract class ProcessorBase
    {
        public ProcessorBase()
        {
            OperationsCost = new Dictionary<string, int>();
            OperationsCost.Add("+", 1);
            OperationsCost.Add("-", 1);
            OperationsCost.Add("*", 2);
            OperationsCost.Add("/", 4);

            UpdateProcessor();
        }

        public List<ProcessorUnit> ProcessorUnits { get; set; } = new List<ProcessorUnit>();
        public List<ProcessorDatabank> ProcessorDatabanks { get; set; } = new List<ProcessorDatabank>();

        public int ProcessorUnitsCount { get; set; } = 2;
        public int ProcessorDatabanksCount { get; set; } = 2;
        public bool ProcessorUseDataTransfer { get; set; } = true;

        public Dictionary<string, int> OperationsCost { get; private set; }

        public abstract ProcessorResponseModel Execute(ProcessorRequestModel requestModel);

        public void UpdateProcessor()
        {
            if (ProcessorDatabanks.Count != ProcessorDatabanksCount)
            {
                ProcessorDatabanks.Clear();
                for (int i = 1; i <= ProcessorDatabanksCount; i++)
                    ProcessorDatabanks.Add(new ProcessorDatabank() { Id = i, InUse = false });
            }

            if (ProcessorUnits.Count != ProcessorUnitsCount)
            {
                ProcessorUnits.Clear();
                for (int i = 1; i <= ProcessorUnitsCount; i++)
                    ProcessorUnits.Add(new ProcessorUnit() { Id = i, InUse = false });
            }
        }
    }
}
