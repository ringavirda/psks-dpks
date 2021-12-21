using PSCS.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSCS.Core.Domain.Parallel
{
    public class ProcessorUnit
    {
        private int _operationDone = 0;
        private bool loaded = false;
        private bool upload = false;

        public int Id { get; set; }
        public bool InUse { get; set; }
        public Expression RunningOperation;
        public List<Expression> DoneOperations = new List<Expression>();

        public void RunCycle(int cycleId, ProcessorLog log, ProcessorBase processor)
        {
            if(!InUse) 
            {
                log.LogIdle(cycleId, this);
                return;
            }
            else
            {
                if (processor.ProcessorUseDataTransfer)
                {
                    var free = processor.ProcessorDatabanks.FirstOrDefault(db => !db.InUse);
                    if (free == null) 
                    {
                        log.LogIdle(cycleId, this);
                        return;
                    }

                    if (_operationDone == 0 && !loaded) 
                    {
                        free.InUse = true;
                        log.LogDataRead(cycleId, this, free);
                        loaded = true;
                        return;
                    }

                    if (_operationDone == processor.OperationsCost[RunningOperation.Operator] && upload)
                    {
                        free.InUse = true;
                        log.LogDataWrite(cycleId, this, free);
                        FinishExecution();
                        return;
                    }
                }
                _operationDone++;
                log.LogExecution(cycleId, this, _operationDone, processor.OperationsCost[RunningOperation.Operator]);
                if (_operationDone == processor.OperationsCost[RunningOperation.Operator])
                {
                    if (processor.ProcessorUseDataTransfer && !upload)
                        upload = true;
                    else
                        FinishExecution();
                }
            }
        }

        private void FinishExecution()
        {
            loaded = false;
            upload = false;
            InUse = false;
            RunningOperation = null;
            _operationDone = 0;
        }
    }
}
