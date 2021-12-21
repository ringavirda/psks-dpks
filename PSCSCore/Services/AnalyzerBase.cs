using PSCS.Core.Services.Default;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSCS.Core.Services
{
    public abstract class AnalyzerBase
    {
        public ILogger Logger { get; set; }
        public IDictionary Dictionary { get; set; }

        public AnalyzerBase()
            => (Logger, Dictionary) = (new DefaultLogger(), new DefaultDictionary());
    }
}
