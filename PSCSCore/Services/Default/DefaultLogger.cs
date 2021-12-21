using PSCS.Core.Domain.Errors;
using PSCS.Core.Domain.ResponseModel;

namespace PSCS.Core.Services.Default
{
    public class DefaultLogger : ILogger
    {
        public void Log(ResponseModelBase responseModel)
        {
            System.Console.WriteLine(responseModel.ConstructLog());
        }

        public void Log(ErrorBase error)
        {
            System.Console.WriteLine(error.Message);
        }

        public void Log(string message)
        {
            System.Console.WriteLine(message);
        }
    }
}