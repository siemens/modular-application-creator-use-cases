using System;
using System.Linq;
using Siemens.Automation.ModularApplicationCreator.ControlModules.ModuleEssentials.Objects.Generation.Openness.XML;
using Siemens.Automation.ModularApplicationCreator.ControlModules.ModuleEssentials.Objects.Generation.Openness.XML.Parts;
using Siemens.Automation.ModularApplicationCreator.Tia.Helper.Create_XML_Block;
using Siemens.Automation.ModularApplicationCreator.Tia.Helper.Create_XML_Block.XmlBlocks.BlockFrames;
using Siemens.Automation.ModularApplicationCreator.Tia.Openness;
using MacPLang = Siemens.Automation.ModularApplicationCreator.Tia.Helper.Create_XML_Block.ProgrammingLanguage;
using MEPlang = Siemens.Automation.ModularApplicationCreator.ControlModules.ModuleEssentials.Objects.Generation.Openness.XML.ProgrammingLanguage;

namespace MAC_use_cases.Model.UseCases
{
    /// <summary>
    ///     Reusable helper methods for building bit-logic RS networks using the
    ///     Module Essentials <see cref="XmlNetwork"/> API.
    /// </summary>
    public static class BitLogicNetworks
    {
        /// <summary>
        ///     Adds an RS flip-flop system block call to the given network and returns it.
        ///     Pins: Q (output), R (input), S1 (input), operand (output).
        /// </summary>
        public static SystemBlockCall CreateRSCall(XmlNetwork parent)
        {
            var rsCall = parent.AddSystemBlockCall("Rs");
            rsCall.AddCallParameter("q", "Bool", Section.Output);
            rsCall.AddCallParameter("r", "Bool", Section.Input);
            rsCall.AddCallParameter("s1", "Bool", Section.Input);
            rsCall.AddCallParameter("operand", "Bool", Section.Output);

            return rsCall;
        }

        /// <summary>
        ///     Adds a Coil (assignment) system block call to the given network and returns it.
        ///     Used in FBD RS networks to write the Q output to a local variable.
        ///     Pins: in (input), out (output), operand (output).
        /// </summary>
        public static SystemBlockCall CreateAssignmentCall(XmlNetwork parent)
        {
            var coil = parent.AddSystemBlockCall("Coil");
            coil.CallParameters.Add(new CustomBlockCallParameter(coil, "in", "Bool", Section.Input));
            coil.CallParameters.Add(new CustomBlockCallParameter(coil, "out", "Bool", Section.Output));
            coil.CallParameters.Add(new CustomBlockCallParameter(coil, "operand", "Bool", Section.Output));
            return coil;
        }

        /// <summary>
        ///     Creates an RS flip-flop network for use in an FBD block.
        ///     The RS Q output is routed through a Coil element to write the result to
        ///     <paramref name="outputVariableName"/>. The operand pin writes to
        ///     <paramref name="rsOperandVariableName"/>.
        /// </summary>
        /// <param name="rInputName">Variable name connected to the R (reset) input.</param>
        /// <param name="s1InputName">Variable name connected to the S1 (set) input.</param>
        /// <param name="outputVariableName">Variable name written by the Coil (Q result).</param>
        /// <param name="rsOperandVariableName">Variable name written by the RS operand pin (bit memory).</param>
        /// <param name="opnsProgrammingLanguage">Network programming language (use <see cref="MEPlang.FBD"/>).</param>
        public static XmlNetwork CreateRSNetwork(
            string rInputName,
            string s1InputName,
            string outputVariableName,
            string rsOperandVariableName,
            MEPlang opnsProgrammingLanguage)
        {
            var xmlNw = new XmlNetwork(opnsProgrammingLanguage);

            var rsCall = CreateRSCall(xmlNw);

            var rInputVar = xmlNw.AddVariable(rInputName);
            xmlNw.AddConnection(rInputVar, rsCall.CallParameters.Single(x => x.Name == "r"));

            var s1InputVar = xmlNw.AddVariable(s1InputName);
            xmlNw.AddConnection(s1InputVar, rsCall.CallParameters.Single(x => x.Name == "s1"));

            var operandOutputVar = xmlNw.AddVariable(rsOperandVariableName);
            xmlNw.AddConnection(rsCall.CallParameters.Single(x => x.Name == "operand"), operandOutputVar);

            var assignmentCall = CreateAssignmentCall(xmlNw);
            xmlNw.AddConnection(rsCall.CallParameters.Single(x => x.Name.Contains("q")), assignmentCall.CallParameters.First(x => x.Name.Contains("in")));
            var assignmentOperandOutput = xmlNw.AddVariable(outputVariableName);
            xmlNw.AddConnection(assignmentCall.CallParameters.First(x => x.Name.Contains("operand")), assignmentOperandOutput);

            return xmlNw;
        }

        /// <summary>
        ///     Creates a single RS network intended for use inside a LAD-language FB.
        ///     <para>
        ///     Note: <see cref="XmlNetwork"/> must be constructed with <see cref="MEPlang.FBD"/> even for
        ///     LAD blocks. LAD and FBD share the same underlying graph XML schema in TIA Portal's
        ///     Openness import format. Passing <see cref="MEPlang.LAD"/> to <see cref="XmlNetwork"/>
        ///     produces XML that TIA Portal rejects with a <see cref="System.Xml.XmlException"/> at
        ///     import time. The block-level <c>ProgrammingLanguage</c> attribute set on the
        ///     <see cref="XmlNetwork"/> owner block is what causes TIA Portal to display it in LAD view.
        ///     </para>
        ///     <para>
        ///     The RS Q output is left explicitly unconnected (<c>null</c>) because LAD does not use a
        ///     downstream Coil element to write the result — the operand pin is the bit-memory output.
        ///     </para>
        /// </summary>
        /// <param name="rInputName">Variable name connected to the R (reset) input.</param>
        /// <param name="s1InputName">Variable name connected to the S1 (set) input.</param>
        /// <param name="rsOperandVariableName">Variable name written by the RS operand pin (bit memory).</param>
        public static XmlNetwork CreateRSNetworkLAD(
            string rInputName,
            string s1InputName,
            string rsOperandVariableName)
        {
            // XmlNetwork must use FBD here: LAD and FBD share the same underlying graph XML in
            // TIA Portal. XmlNetwork(MEPlang.LAD) generates a schema TIA Portal rejects on import.
            var xmlNw = new XmlNetwork(MEPlang.FBD);

            var rsCall = CreateRSCall(xmlNw);

            var rInputVar = xmlNw.AddVariable(rInputName);
            xmlNw.AddConnection(rInputVar, rsCall.CallParameters.Single(x => x.Name == "r"));

            var s1InputVar = xmlNw.AddVariable(s1InputName);
            xmlNw.AddConnection(s1InputVar, rsCall.CallParameters.Single(x => x.Name == "s1"));

            var operandOutputVar = xmlNw.AddVariable(rsOperandVariableName);
            xmlNw.AddConnection(rsCall.CallParameters.Single(x => x.Name == "operand"), operandOutputVar);

            // Q output left unconnected – required for RS in LAD (no downstream Coil element)
            xmlNw.AddConnection(rsCall.CallParameters.Single(x => x.Name == "q"), null);

            return xmlNw;
        }

        /// <summary>
        ///     Generates a Function Block (FB) containing two chained RS flip-flop networks in FBD language
        ///     and imports it into the given <paramref name="plcDevice"/>.
        ///     <para>
        ///     Network 1: RS driven by <c>InputBool_R</c> / <c>InputBool_S1</c>; result written to
        ///     <c>OutputBool</c> (via Coil) and <c>RSOperand</c> (operand pin).
        ///     </para>
        ///     <para>
        ///     Network 2: RS driven by <c>InputBool_R2</c> / <c>OutputBool</c> (chained from Network 1);
        ///     result written to <c>OutputBool2</c> and <c>RSOperand2</c>.
        ///     </para>
        /// </summary>
        /// <param name="blockName">Name of the FB to create in TIA Portal.</param>
        /// <param name="plcDevice">Target PLC device.</param>
        /// <param name="macProgrammingLanguage">Programming language for the block (use <see cref="MacPLang.FBD"/>).</param>
        public static void GenerateFbWithRSNetworkFBD(string blockName, PlcDevice plcDevice, MacPLang macProgrammingLanguage)
        {
            var opnsProgrammingLanguage = (MEPlang)Enum.Parse(typeof(MEPlang), macProgrammingLanguage.ToString());

            var block = new XmlFB(blockName);
            block.BlockAttributes.ProgrammingLanguage = macProgrammingLanguage;

            block.Interface[InterfaceSections.Input].Add(new InterfaceParameter("InputBool_R", "Bool"));
            block.Interface[InterfaceSections.Input].Add(new InterfaceParameter("InputBool_S1", "Bool"));
            block.Interface[InterfaceSections.Input].Add(new InterfaceParameter("InputBool_R2", "Bool"));

            var staticItf = block.Interface[InterfaceSections.Static];
            staticItf.Add(new InterfaceParameter("RSOperand", "Bool"));
            staticItf.Add(new InterfaceParameter("OutputBool", "Bool"));
            staticItf.Add(new InterfaceParameter("RSOperand2", "Bool"));
            staticItf.Add(new InterfaceParameter("OutputBool2", "Bool"));

            // Network 1: first RS flip-flop
            var rsNetwork1 = CreateRSNetwork("#InputBool_R", "#InputBool_S1", "#OutputBool", "#RSOperand", opnsProgrammingLanguage);
            block.Networks.Add(rsNetwork1.GenerateFixNetwork());

            // Network 2: second RS flip-flop – S1 driven by #OutputBool written in Network 1
            var rsNetwork2 = CreateRSNetwork("#InputBool_R2", "#OutputBool", "#OutputBool2", "#RSOperand2", opnsProgrammingLanguage);
            block.Networks.Add(rsNetwork2.GenerateFixNetwork());

            block.GenerateXmlBlock(plcDevice);
        }

        /// <summary>
        ///     Generates a Function Block (FB) containing two chained RS flip-flop networks in LAD language
        ///     and imports it into the given <paramref name="plcDevice"/>.
        ///     <para>
        ///     Network 1: RS driven by <c>InputBool_R</c> / <c>InputBool_S1</c>; result stored in <c>RSOperand</c>.
        ///     </para>
        ///     <para>
        ///     Network 2: RS driven by <c>InputBool_R2</c> / <c>RSOperand</c> (chained from Network 1);
        ///     result stored in <c>RSOperand2</c>.
        ///     </para>
        ///     <para>
        ///     In LAD, no Coil element is used — the operand pin of the RS instruction is the bit-memory output.
        ///     See <see cref="CreateRSNetworkLAD"/> for details on why <see cref="MEPlang.FBD"/> is used
        ///     at the network level.
        ///     </para>
        /// </summary>
        /// <param name="blockName">Name of the FB to create in TIA Portal.</param>
        /// <param name="plcDevice">Target PLC device.</param>
        public static void GenerateFbWithRSNetworkLAD(string blockName, PlcDevice plcDevice)
        {
            var block = new XmlFB(blockName);
            block.BlockAttributes.ProgrammingLanguage = MacPLang.LAD;

            block.Interface[InterfaceSections.Input].Add(new InterfaceParameter("InputBool_R", "Bool"));
            block.Interface[InterfaceSections.Input].Add(new InterfaceParameter("InputBool_S1", "Bool"));
            block.Interface[InterfaceSections.Input].Add(new InterfaceParameter("InputBool_R2", "Bool"));

            var staticItf = block.Interface[InterfaceSections.Static];
            staticItf.Add(new InterfaceParameter("RSOperand", "Bool"));
            staticItf.Add(new InterfaceParameter("RSOperand2", "Bool"));

            // Network 1: RS flip-flop in LAD
            var rsNetwork1 = CreateRSNetworkLAD("#InputBool_R", "#InputBool_S1", "#RSOperand");
            block.Networks.Add(rsNetwork1.GenerateFixNetwork());

            // Network 2: RS flip-flop in LAD – S1 driven by the operand written in Network 1
            var rsNetwork2 = CreateRSNetworkLAD("#InputBool_R2", "#RSOperand", "#RSOperand2");
            block.Networks.Add(rsNetwork2.GenerateFixNetwork());

            block.GenerateXmlBlock(plcDevice);
        }
    }
}
