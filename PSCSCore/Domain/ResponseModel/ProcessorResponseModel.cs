using PSCS.Core.Domain.Parallel;
using System.Linq;
using System.Text;

namespace PSCS.Core.Domain.ResponseModel
{
    public class ProcessorResponseModel : ResponseModelBase
    {
        public ProcessorLog ProcessorLog { get; set; }

        public override string ConstructLog()
        {
            StringBuilder builder = new StringBuilder();
            foreach (var cycle in ProcessorLog.CycleLog)
            {
                builder.AppendLine($"{cycle.Key:D3}: ====================================");
                foreach (var log in ProcessorLog.CycleLog[cycle.Key])
                {
                    builder.AppendLine(log);
                }
            }
            builder.AppendLine();
            builder.AppendLine("Logged Unit Characteristics ======================");
            int totalExec = 0, totalData = 0, totalIdle = 0;
            foreach (var unit in ProcessorLog.UnitLog)
            {
                builder.AppendLine($"[PU {unit.Key}] Times:");
                builder.AppendLine($"\tExecution = {unit.Value["Execution Time"]} ({(unit.Value["Execution Time"] /(float) ProcessorLog.CycleLog.Last().Key) * 100:F3}%)");
                totalExec += unit.Value["Execution Time"];
                if (unit.Value["Datatransfer Time"] != 0)
                {
                    builder.AppendLine($"\tDatatransfer = {unit.Value["Datatransfer Time"]} ({(unit.Value["Datatransfer Time"] /(float) ProcessorLog.CycleLog.Last().Key) * 100:F3}%)");
                    totalData += unit.Value["Datatransfer Time"];
                }
                builder.AppendLine($"\tIdle = {unit.Value["Idle Time"]} ({(unit.Value["Idle Time"] /(float) ProcessorLog.CycleLog.Last().Key) * 100:F3}%)");
                totalIdle += unit.Value["Idle Time"];
            }
            builder.AppendLine("Total Processor Characteristics ======================");
            var total = totalExec + totalData + totalIdle;
            builder.AppendLine($"\tExecution = {(totalExec /(float) total) * 100:F3}%");
            if (totalData != 0) 
                builder.AppendLine($"\tDataTransfer = {(totalData /(float) total) * 100:F3}%");
            builder.Append($"\tIdle = {(totalIdle /(float) total) * 100:F3}%");
            return builder.ToString();
        }
    }
}
