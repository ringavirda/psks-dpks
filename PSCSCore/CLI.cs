using System;
using System.Collections.Generic;

namespace PSCS
{
    public class CLI
    {
        private Dictionary<string, Action> _knownNonParametrizedCommands;   
        private Dictionary<string, Action<string>> _knownOneParameterCommands;  

        public CLI()
        {
            _knownNonParametrizedCommands = new Dictionary<string, Action>();
            _knownOneParameterCommands = new Dictionary<string, Action<string>>();
        }

        public void AddCommand(string command, Action action)
            => _knownNonParametrizedCommands.Add(command, action);

        public void AddCommand(string command, Action<string> action)
            => _knownOneParameterCommands.Add(command, action);
       
        public void ExecuteCommand(string input)
        {
            var splitted =  input.Split(" ");
            
            if (splitted.Length == 1)
            {
                if (!_knownNonParametrizedCommands.ContainsKey(splitted[0]))
                {
                    System.Console.WriteLine($"[{splitted[0]}] command not recognized");
                    return;
                }
                _knownNonParametrizedCommands[splitted[0]].Invoke();
            }

            if (splitted.Length == 2)
            {
                if (!_knownOneParameterCommands.ContainsKey(splitted[0]))
                {
                    System.Console.WriteLine($"[{splitted[0]}] command not recognized");
                    return;
                }
                _knownOneParameterCommands[splitted[0]].Invoke(splitted[1]);
            }
        }
    }
}
