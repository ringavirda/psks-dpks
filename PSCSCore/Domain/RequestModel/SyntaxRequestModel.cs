using PSCS.Core.Domain.Lexems;
using System.Collections.Generic;

namespace PSCS.Core.Domain.RequestModel
{
    public class SyntaxRequestModel : RequestModelBase
    {
        public List<LexemBase> SourceSequence { get; set; }
    }
}
