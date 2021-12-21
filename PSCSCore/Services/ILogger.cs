using PSCS.Core.Domain.Errors;
using PSCS.Core.Domain.ResponseModel;

namespace PSCS.Core.Services
{
    public interface ILogger
    {
        void Log(ResponseModelBase responseModel);
        void Log(ErrorBase error);
        void Log(string message);
    }
}