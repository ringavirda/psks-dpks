using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSCS.Core.Domain.Errors
{
    public abstract class ErrorBase
    {
        public string Message { get; set; }
    }
}
