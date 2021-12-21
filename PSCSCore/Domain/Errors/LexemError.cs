using System;
using System.Text;

namespace PSCS.Core.Domain.Errors
{
    public class LexemError : ErrorBase
    {
        private FormattableString _errorMessageTemplate = $"L Error at [{0}], {1}\n";

        public LexemError(int i, string lexem, string message, string source)
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine(source.Insert(i, "[").Insert(i + lexem.Length + 1, "]"));
            builder.Append(string.Format(_errorMessageTemplate.Format, i, message));

            Message = builder.ToString();
        }
    }
}
