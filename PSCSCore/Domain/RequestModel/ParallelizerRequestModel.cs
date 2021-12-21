using PSCS.Core.Domain.Lexems;
using System.Collections.Generic;

namespace PSCS.Core.Domain.RequestModel
{
    public class ParallelizerRequestModel : RequestModelBase
    {
        public List<LexemBase> ProcessedSequence { get; set; }
    }
}
