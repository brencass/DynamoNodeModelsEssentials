﻿using System;
using System.Collections.Generic;
using System.Collections;
using Dynamo.Graph.Nodes;
using ProtoCore.AST.AssociativeAST;
using NodeModelsEssentials.Functions;
using System.Linq;
using Newtonsoft.Json;
using System.Windows;

namespace NodeModelsEssentials
{
    /// <summary>
    /// A node that displays how a data bridge can be used within a Node Model.
    /// Something to note is that the data bridge callback passes an expression list
    /// to pass all of the input nodes to the callback, thus providing the input of
    /// the node ports to the Node Model class.
    /// (This happens using AstFactory.BuildExprList(inputAstNodes), but you could
    /// also just pass a single node, or a list of nodes, like inputAstNodes[0].)
    /// </summary>
    [NodeName("Essentials.DataBridge")]
    [NodeDescription("How the data bridge pattern works in order to pass the data connected to input ports (or the data of generated for the output ports) to the NodeModel instance.")]
    [NodeCategory("NodeModelsEssentials")]
    [InPortNames("A", "B", "C")]
    [InPortTypes("string", "string", "string")]
    [InPortDescriptions("A string.", "Another string.", "Another string.")]
    [OutPortNames("Out")]
    [OutPortTypes("string")]
    [OutPortDescriptions(
        "Resulting string.")]
    [IsDesignScriptCompatible]
    class EssentialsDataBridge : NodeModel
    {
        #region constructor

        [JsonConstructor]
        private EssentialsDataBridge(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts) : base(inPorts, outPorts)
        {
        }

        public EssentialsDataBridge()
        {
            RegisterAllPorts();
        }

        #endregion

        #region data bridge callback

        /// <summary>
        /// Register the data bridge callback.
        /// </summary>
        protected override void OnBuilt()
        {
            base.OnBuilt();
            VMDataBridge.DataBridge.Instance.RegisterCallback(GUID.ToString(), DataBridgeCallback);
        }

        /// <summary>
        /// Unregister the data bridge callback.
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
            VMDataBridge.DataBridge.Instance.UnregisterCallback(GUID.ToString());
        }

        /// <summary>
        /// Callback method for DataBridge mechanism.
        /// This callback only gets called once after the BuildOutputAst Function is executed 
        /// This callback casts the response data object.
        /// </summary>
        /// <param name="data">The data passed through the data bridge.</param>
        private void DataBridgeCallback(object data)
        {
            ArrayList inputs = data as ArrayList;
            var str = "";
            foreach (var input in inputs)
            {
                str += input.ToString() + " ";
            }
            MessageBox.Show("Data bridge callback of node " + GUID.ToString().Substring(0,5) + ": " + str );
        }

        #endregion

        #region ast

        public override IEnumerable<AssociativeNode> BuildOutputAst(List<AssociativeNode> inputAstNodes)
        {
            if (!InPorts[0].Connectors.Any() || !InPorts[1].Connectors.Any() || !InPorts[2].Connectors.Any())
            {
                return new[]
                    {AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), AstFactory.BuildNullNode())};
            }

            var functionCallNode = AstFactory.BuildFunctionCall(
                new Func<string, string, string, string>(NodeModelsEssentialsFunctions.ConcatenateThree),
                new List<AssociativeNode> { inputAstNodes[0], inputAstNodes[1], inputAstNodes[2] });

            return new[]
            {
                    AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), functionCallNode),
                    AstFactory.BuildAssignment(
                        AstFactory.BuildIdentifier(AstIdentifierBase + "_dummy"),
                        // you can pass an expression list like this
                        VMDataBridge.DataBridge.GenerateBridgeDataAst(GUID.ToString(), AstFactory.BuildExprList(inputAstNodes))
                        // or build it manually like this
                        //VMDataBridge.DataBridge.GenerateBridgeDataAst(GUID.ToString(), AstFactory.BuildExprList(new List<AssociativeNode> { inputAstNodes[0], AstFactory.BuildStringNode("test") } ))
                    )
                };
        }

        #endregion

    }

}
