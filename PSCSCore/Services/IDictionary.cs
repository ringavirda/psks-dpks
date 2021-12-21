namespace PSCS.Core.Services
{
    public interface IDictionary
    {
        bool IsAllowedSymbol(string sample);
        bool IsDigit(string sample);
        bool IsLetter(string sample);
        bool IsOperator(string sample);
        bool IsFunction(string sample);
        bool IsSpecialWord(string sample);
        bool IsContainerStart(string sample);
        bool IsContainerEnd(string sample);
        bool IsFloatDesignator(string sample);
    }
}