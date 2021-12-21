using PSCS.Core.Domain.RequestModel;
using PSCS.Core.Domain.ResponseModel;

namespace PSCS.Core.Services
{
    public interface ISyntaxAnalyzer
    {
        public SyntaxResponseModel Analize(SyntaxRequestModel syntaxRequest);
    }
}
