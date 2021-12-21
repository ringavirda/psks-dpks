using PSCS.Core.Domain.RequestModel;
using PSCS.Core.Domain.ResponseModel;
using PSCS.Core.Services;
using System.Text;

namespace PSCS.Core
{
    public class CompilerCore
    {
        private CompilerConf _conf;
        private CLI _cli;

        private string _sequence;
        private LexemResponseModel _lexemResponse;
        private SyntaxResponseModel _syntaxRespose;
        private ParallelizerResponseModel _parallelizerResponse;
        private ProcessorResponseModel _processorResponce;

        private ILogger Logger { get => _conf.Logger; }

        public CompilerCore(CompilerConf conf)
        {
            _conf = conf;
            _cli = new CLI();
            ConfigureCLI(_cli);
        }

        public void Start()
        {
            System.Console.WriteLine("Custom Compiler CLI\n");

            while (true)
            {
                System.Console.Write("> ");

                var input = System.Console.ReadLine();

                if (string.IsNullOrWhiteSpace(input)) continue;

                _cli.ExecuteCommand(input);
            }
        }

        private void ConfigureCLI(CLI cli)
        {
            cli.AddCommand("seq", () => Logger.Log($"Remember sequence:\n{_sequence}"));
            cli.AddCommand("seq", (input) => { _sequence = input; Logger.Log("Got it"); });

            cli.AddCommand("lex", () =>
            {
                if (string.IsNullOrEmpty(_sequence))
                {
                    Logger.Log("No sequence recognized");
                }
                else
                {
                    Logger.Log($"Preparing for lexem analysis of sequence:\n{_sequence}");
                    _lexemResponse = _conf.LexemAnalizer.Analize(new LexemRequestModel { SourceString = _sequence });
                }
            });
            cli.AddCommand("lex", (input) =>
            {
                Logger.Log($"Preparing for lexem analysis of sequence:\n{input}");
                _conf.LexemAnalizer.Analize(new LexemRequestModel { SourceString = input });
            });
            cli.AddCommand("lexl", () =>
            {
                if (_lexemResponse != null)
                {
                    Logger.Log(_lexemResponse);
                }
                else
                {
                    Logger.Log("There was no lexem analysis performed yet");
                }
            });

            cli.AddCommand("syn", () =>
            {
                if (_lexemResponse != null)
                {
                    Logger.Log($"Preparing for syntax analysis of sequence:\n{_sequence}");
                    _syntaxRespose = _conf.SyntaxAnalizer.Analize(new SyntaxRequestModel { SourceSequence = _lexemResponse.Sequence, SourceString = _sequence });
                }
                else
                {
                    Logger.Log("There was no lexem analysis performed yet");
                }
            });

            cli.AddCommand("fa", () =>
            {
                if (string.IsNullOrEmpty(_sequence))
                {
                    Logger.Log("No sequence recognized");
                }
                else
                {
                    Logger.Log($"Preparing for full analysis of sequence:\n{_sequence}");
                    _lexemResponse = _conf.LexemAnalizer.Analize(new LexemRequestModel { SourceString = _sequence });
                    if (_lexemResponse.Successful)
                    {
                        _syntaxRespose = _conf.SyntaxAnalizer.Analize(new SyntaxRequestModel
                        {
                            SourceSequence = _lexemResponse.Sequence,
                            SourceString = _lexemResponse.SourceString
                        });
                    }
                }
            });

            cli.AddCommand("fa", (input) =>
            {
                Logger.Log($"Preparing for full analysis of sequence:\n{input}");
                var lexemResponseModel = _conf.LexemAnalizer.Analize(new LexemRequestModel { SourceString = input });
                if (lexemResponseModel.Successful)
                {
                    _conf.SyntaxAnalizer.Analize(new SyntaxRequestModel { SourceSequence = lexemResponseModel.Sequence, SourceString = lexemResponseModel.SourceString });
                }
            });

            cli.AddCommand("prl", () =>
            {
                if (string.IsNullOrEmpty(_sequence))
                {
                    Logger.Log("No sequence recognized");
                }
                else
                {
                    if (_syntaxRespose == null)
                    {
                        Logger.Log("Syntax Analysis hasn't been performed yet");
                    }
                    else
                    {
                        Logger.Log($"Preparing to parallelize sequence:\n{_sequence}");
                        _parallelizerResponse = _conf.Parallelizer.Parallelize(new ParallelizerRequestModel
                        {
                            SourceString = _sequence,
                            ProcessedSequence = _syntaxRespose.ProcessedSequence
                        });
                        _conf.Logger.Log(_parallelizerResponse.ConstructLog());
                    }
                }
            });

            cli.AddCommand("spuc", (input) =>
            {
                var newCount = ValidateNewCount(input);
                if (newCount == -1) return;

                Logger.Log($"Setting Processor Unit Count to [{newCount}]");
                _conf.Processor.ProcessorUnitsCount = newCount;
                _conf.Processor.UpdateProcessor();
                Logger.Log("Done");
            });

            cli.AddCommand("spdc", (input) =>
            {
                var newCount = ValidateNewCount(input);
                if (newCount == -1) return;

                Logger.Log($"Setting Processor Databanks Count to [{newCount}]");
                _conf.Processor.ProcessorDatabanksCount = newCount;
                _conf.Processor.UpdateProcessor();
                Logger.Log("Done");
            });

            cli.AddCommand("spdt", (input) =>
            {
                var newFlag = 0;
                if (!int.TryParse(input, out newFlag))
                {
                    Logger.Log("Argument can only be 0 or 1");
                    return;
                }
                if (newFlag < 0 || newFlag > 1)
                {
                    Logger.Log("Argument can only be 0 or 1");
                    return;
                }
                Logger.Log($"Setting Processor Data Transfer to [{newFlag}]");
                if (newFlag == 0)
                {
                    _conf.Processor.ProcessorUseDataTransfer = false;
                    Logger.Log("Processor Data Transfer - Disabled");
                }
                else
                {
                    _conf.Processor.ProcessorUseDataTransfer = true;
                    Logger.Log("Processor Data Transfer - Enabled");
                }
            });

            cli.AddCommand("exec", () =>
            {
                if (string.IsNullOrEmpty(_sequence))
                {
                    Logger.Log("No sequence recognized");
                }
                else
                {
                    if (_parallelizerResponse == null)
                    {
                        Logger.Log("Parallezation hasn't happen yet");
                    }
                    else
                    {
                        Logger.Log($"Preparing to execute sequence:\n{_sequence}");
                        _processorResponce = _conf.Processor.Execute(new ProcessorRequestModel
                        {
                            SourceString = _sequence,
                            StateTree = _parallelizerResponse.Tree
                        });
                        Logger.Log("Execution complete, processor log:");
                        Logger.Log(_processorResponce.ConstructLog());
                    }
                }
            });

            cli.AddCommand("gpuc", () =>
            {
                Logger.Log($"Processor Unit Count - {_conf.Processor.ProcessorUnits.Count}");
            });
            cli.AddCommand("gpdc", () =>
            {
                Logger.Log($"Processor Databank Count - {_conf.Processor.ProcessorDatabanks.Count}");
            });
            cli.AddCommand("gpdt", () =>
            {
                if (_conf.Processor.ProcessorUseDataTransfer)
                    Logger.Log("Processor Data Transfer - Enabled");
                else
                    Logger.Log("Processor Data Transfer - Disabled");
            });


            cli.AddCommand("lc", () =>
            {
                StringBuilder builder = new StringBuilder();
                builder.AppendLine("-----");
                foreach (var item in _conf.Processor.OperationsCost)
                {
                    builder.AppendLine($"{item.Key} = {item.Value}");
                }
                builder.Append("-----");
                Logger.Log(builder.ToString());
            });
            cli.AddCommand("spc", (input) =>
            {
                var newValue = ValidateNewCount(input);
                if (newValue == -1) return;

                Logger.Log($"Setting Plus Value to [{newValue}]");
                _conf.Processor.OperationsCost["+"] = newValue;
                Logger.Log("Done");
            });
            cli.AddCommand("ssc", (input) =>
            {
                var newValue = ValidateNewCount(input);
                if (newValue == -1) return;

                Logger.Log($"Setting Subtruct Value to [{newValue}]");
                _conf.Processor.OperationsCost["-"] = newValue;
                Logger.Log("Done");
            });
            cli.AddCommand("smc", (input) =>
            {
                var newValue = ValidateNewCount(input);
                if (newValue == -1) return;

                Logger.Log($"Setting Multiply Value to [{newValue}]");
                _conf.Processor.OperationsCost["*"] = newValue;
                Logger.Log("Done");
            });
            cli.AddCommand("sdc", (input) =>
            {
                var newValue = ValidateNewCount(input);
                if (newValue == -1) return;

                Logger.Log($"Setting Division Value to [{newValue}]");
                _conf.Processor.OperationsCost["/"] = newValue;
                Logger.Log("Done");
            });
        }

        private int ValidateNewCount(string input)
        {
            int newCount = 0;
            if (!int.TryParse(input, out newCount))
            {
                Logger.Log("Argument cannot be not number");
                return - 1;
            }

            if (newCount < 1)
            {
                Logger.Log("Argument cannot be less then 1");
                return - 1;
            }

            if (newCount > 10)
            {
                Logger.Log("Argument cannot be more then 10");
                return -1;
            }

            return newCount;
        }
    }
}