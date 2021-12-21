using PSCS.Core.Domain.Parallel;

namespace PSCS.Core.Domain.RequestModel
{
    public class ProcessorRequestModel : RequestModelBase
    {
        public StateTree StateTree { get; set; }
    }
}
