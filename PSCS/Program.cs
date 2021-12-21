using PSCS.Core;

namespace PSCS
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var core = new CompilerCore(new CompilerConf().Default);
            core.Start();
        }
    }
}
