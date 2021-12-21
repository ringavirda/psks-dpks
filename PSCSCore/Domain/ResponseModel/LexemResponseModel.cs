using PSCS.Core.Domain.Lexems;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PSCS.Core.Domain.ResponseModel
{
    public class LexemResponseModel : ResponseModelBase
    {
        public List<LexemBase> Sequence { get; set; } = new List<LexemBase>();

        public override string ConstructLog()
        {
            var builder = new StringBuilder();
            builder.AppendLine("Lexem analysis log");
            builder.AppendLine($"Source string: {SourceString}");
            builder.AppendLine();
            if (FailsCount > 0)
            {
                builder.AppendLine($"Analysis failed, [{FailsCount}] errors found. Error log:");
                foreach (var error in Errors)
                {
                    builder.AppendLine(error.Message);
                }
            }
            else
            {
                builder.AppendLine("Analysis successful, no errors found");
                builder.AppendLine("Composed sequence:");
                foreach (var lexem in Sequence)
                {
                    builder.Append(lexem != Sequence.Last() ? $"[{lexem.Value}], " : $"[{lexem.Value}]"); ;
                }
            }

            builder.AppendLine();
            return builder.ToString();
        }
    }
}
