using Microsoft.VisualStudio.TestTools.UnitTesting;
using PSCS.Core.Domain.RequestModel;
using PSCS.Core.Services.Default;

namespace PSCS.Core.Tests
{
    [TestClass]
    public class DefaultLexemAnalyzerTests
    {
        [TestMethod]
        public void InputTest()
        {
            string input = "1+22*(sp-b)^2";

            var subject = new DefaultLexemAnalyzer();

            var res = subject.Analize(new LexemRequestModel { SourceString = input });

            Assert.IsTrue(res.Successful);
        }

        [TestMethod]
        public void InputFailTest()
        {
            string input = "$1+22&(1sp-b@)^2";

            var subject = new DefaultLexemAnalyzer();

            var res = subject.Analize(new LexemRequestModel { SourceString = input });

            Assert.IsFalse(res.Successful);
        }
    }
}
