using PSCS.Core.Domain.RequestModel;
using PSCS.Core.Domain.ResponseModel;

namespace PSCS.Core.Services
{
    public interface IParallelizer
    {
        public ParallelizerResponseModel Parallelize(ParallelizerRequestModel requestModel);
    }
}
