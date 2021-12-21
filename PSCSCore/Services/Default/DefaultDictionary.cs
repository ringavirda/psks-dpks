using System.Collections.Generic;

namespace PSCS.Core.Services.Default
{
    public class DefaultDictionary : IDictionary
    {
        private List<char> _allovedSpecialSymbols = new List<char> { '+', '-', '*', '/', '(', ')' };
        private List<string> _knownFunctions = new List<string> { "sum", "sub", "mul", "div", "pow" };
        private List<string> _knownSpecialWords = new List<string> { };
        private List<string> _allowedFloatDesignators = new List<string> { ",", "." };

        private List<string> _containerStart  = new List<string> { "(" };
        private List<string> _containerEnd = new List<string> { ")" };

        public bool IsAllowedSymbol(string sample) 
        {
            if (sample.Length != 1) return false; 
            return char.IsLetterOrDigit(sample[0]) || _allovedSpecialSymbols.Contains(sample[0]);
        }
        public bool IsLetter(string sample) 
        {
            if (sample.Length != 1) return false;
            return char.IsLetter(sample[0]);
        }

        public bool IsDigit(string sample) 
        {
            if (sample.Length != 1) return false;
            return char.IsDigit(sample[0]); 
        }

        public bool IsOperator(string sample)
        {
            if (sample.Length != 1) return false;
            return _allovedSpecialSymbols.Contains(sample[0]);
        }

        public bool IsFunction(string sample)
        {
            if (sample.Length >= 1) return false;
            return _knownFunctions.Contains(sample);
        }

        public bool IsSpecialWord(string sample)
        {
            if (sample.Length != 1) return false;
            return _knownSpecialWords.Contains(sample);
        }

        public bool IsContainerStart(string sample)
        {
            if (sample.Length != 1) return false;
            return _containerStart.Contains(sample);
        }

        public bool IsContainerEnd(string sample)
        {
            if (sample.Length != 1) return false;
            return _containerEnd.Contains(sample);
        }

        public bool IsFloatDesignator(string sample)
        {
            if (sample.Length != 1) return false;
            return _allowedFloatDesignators.Contains(sample);
        }
    }
}
