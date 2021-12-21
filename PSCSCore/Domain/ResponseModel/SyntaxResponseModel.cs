using PSCS.Core.Domain.Lexems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSCS.Core.Domain.ResponseModel
{
    public class SyntaxResponseModel : ResponseModelBase
    {
        public List<LexemBase> ProcessedSequence { get; set; } = new List<LexemBase>();

        public override string ConstructLog()
        {
            throw new NotImplementedException();
        }
    }
}
