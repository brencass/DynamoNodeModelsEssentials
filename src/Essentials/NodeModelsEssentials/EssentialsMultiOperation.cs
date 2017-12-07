﻿using System;
using System.Collections.Generic;
using Dynamo.Graph.Nodes;
using ProtoCore.AST.AssociativeAST;
using NodeModelsEssentials.Functions;
using System.Linq;
using Newtonsoft.Json;

namespace NodeModelsEssentials
{
    // <summary>
    // Sample Node Model called MultiplyMulti.
    // It returns the product of two numbers and a verbose string.
    // </summary>
    [NodeName("Essentials.MultiOperation")]
    [NodeDescription("Performs multiple operations with the inputs.")]
    [NodeCategory("NodeModelsEssentials")]
    [InPortNames("A", "B")]
    [InPortTypes("double", "double")]
    [InPortDescriptions("Number A", "Number B")]
    [OutPortNames("AxB", "A+B", "A-B", "A/B")]
    [OutPortTypes("double", "double")]
    [OutPortDescriptions(
        "Product of A x B",
        "Addition of A and B",
        "Subtraction of A and B",
        "Division of A by B")]
    [IsDesignScriptCompatible]
    public class MultiOperation : NodeModel
    {
        [JsonConstructor]
        private MultiOperation(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts) : base(inPorts, outPorts)
        {
        }

        public MultiOperation()
        {
            RegisterAllPorts();
        }

        public override IEnumerable<AssociativeNode> BuildOutputAst(List<AssociativeNode> inputAsNodes)
        {
            if (!InPorts[0].Connectors.Any() || !InPorts[1].Connectors.Any())
            {
                return new[] {
                    AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), AstFactory.BuildIntNode(1)),
                    AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(1), AstFactory.BuildIntNode(2)),
                    AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(2), AstFactory.BuildIntNode(3)),
                    AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(3), AstFactory.BuildIntNode(4))
                };
            }

            var multiplyFunctionNode =
                AstFactory.BuildFunctionCall(
                    new Func<double, double, double>(NodeModelsEssentialsFunctions.Multiply),
                    new List<AssociativeNode> { inputAsNodes[0], inputAsNodes[1] });

            var addFunctionNode =
                AstFactory.BuildFunctionCall(
                    new Func<double, double, double>(NodeModelsEssentialsFunctions.Add),
                    new List<AssociativeNode> { inputAsNodes[0], inputAsNodes[1] });
            
            var subtractFunctionNode =
                AstFactory.BuildFunctionCall(
                    new Func<double, double, double>(NodeModelsEssentialsFunctions.Subtract),
                    new List<AssociativeNode> { inputAsNodes[0], inputAsNodes[1] });
            
            var divideFunctionNode =
                AstFactory.BuildFunctionCall(
                    new Func<double, double, double>(NodeModelsEssentialsFunctions.Divide),
                    new List<AssociativeNode> { inputAsNodes[0], inputAsNodes[1] });

            return new[] {
                AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), multiplyFunctionNode),
                AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(1), addFunctionNode),
                AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(2), subtractFunctionNode),
                AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(3), divideFunctionNode)
            };
        }
    }
}
