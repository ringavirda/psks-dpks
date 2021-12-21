using PSCS.Core.Domain.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSCS.Core.Domain.ResponseModel
{
    public abstract class ResponseModelBase
    {
        public ResponseModelBase()
        {
            Errors = new List<ErrorBase>();
        }

        public string SourceString { get; set; }
        public List<ErrorBase> Errors { get; private set; }
        public int FailsCount { get => Errors.Count; }
        public bool Successful { get => Errors.Count == 0;  }

        public abstract string ConstructLog();
    }
}
