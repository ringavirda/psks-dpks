using PSCS.Core.Domain.Lexems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PSCS.Core.Domain.Errors
{
    public class SyntaxError : ErrorBase
    {
        private FormattableString _errorMessageTemplate = $"S Error at [{0}], {1}\n";

        public SyntaxError(int i, List<LexemBase> lexems, string message, List<LexemBase> source)
        {
            StringBuilder builder = new StringBuilder();
            var sourceStrings = source.Select((lexem) => lexem.Value).ToList();

            sourceStrings.Insert(i, "[");
            sourceStrings.Insert(i + lexems.Count + 1, "]");
            builder.AppendLine(string.Join("", sourceStrings));
            builder.Append(string.Format(_errorMessageTemplate.Format, i, message));

            Message = builder.ToString();
        }
    }
}
