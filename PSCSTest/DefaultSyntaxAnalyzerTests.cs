using Microsoft.VisualStudio.TestTools.UnitTesting;
using PSCS.Core.Domain.RequestModel;
using PSCS.Core.Services.Default;
using System.Collections.Generic;
using System.Linq;

namespace PSCSCoreTests
{
    [TestClass]
    public class DefaultSyntaxAnalyzerTests
    {
        [TestMethod]
        public void AnalysisSuccessTest()
        {
            var inputs = new List<string> { "1+22*(sp-b)^2", "ab*((0+as)-a*2)" };

            var sequences = inputs.Select((input) =>
                new DefaultLexemAnalyzer().Analize(new LexemRequestModel { SourceString = input })).ToList();

            var responses = sequences.Select((sequence) =>
                new DefaultSyntaxAnalyzer().Analize(new SyntaxRequestModel { SourceSequence = sequence.Sequence, SourceString = sequence.SourceString })).ToList();

            responses.ForEach((response) => Assert.IsTrue(response.Successful));
        }

        [TestMethod]
        public void AnalysisFailedTest()
        {
            var inputs = new List<string> {
                "/1+22*(sp-b)^2",
                "ab*((0+as)-a*2))",
                "12**1*(2)",
                "()*2(a+b)",
                "(+22-a)*20",
                "ss)*2",
                "t+2(22)"
            };

            var sequences = inputs.Select((input) =>
                new DefaultLexemAnalyzer().Analize(new LexemRequestModel { SourceString = input })).ToList();

            var responses = sequences.Select((sequence) =>
                new DefaultSyntaxAnalyzer().Analize(new SyntaxRequestModel { SourceSequence = sequence.Sequence, SourceString = sequence.SourceString })).ToList();

            responses.ForEach((response) => Assert.IsFalse(response.Successful));
            responses.ForEach((response) => Assert.AreEqual(response.FailsCount, 1));
        }
    }
}
