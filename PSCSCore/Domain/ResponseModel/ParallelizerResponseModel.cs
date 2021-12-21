using PSCS.Core.Domain.Parallel;
using System;
using System.Text;

namespace PSCS.Core.Domain.ResponseModel
{
    public class ParallelizerResponseModel : ResponseModelBase
    {
        public StateTree Tree { get; set; }

        public override string ConstructLog()
        {
            var builder = new StringBuilder();
            builder.AppendLine("Finished parallelising, generated tree structure:");
            for (int l = 0; l < Tree.Layers.Count; l++)
            {
                var layer = Tree.Layers[l];
                for (int i = 0; i < layer.Expressions.Count; i++)
                {
                    builder.Append($"{layer.Expressions[i].LeftOperand.Value} {layer.Expressions[i].Operator} {layer.Expressions[i].RightOperand.Value}");
                    if (i != layer.Expressions.Count - 1)
                    {
                        builder.Append("  |  ");
                    }
                }
                if (l != Tree.Layers.Count - 1)
                {
                    builder.AppendLine();
                }
            }
            return builder.ToString();
        }
    }
}
