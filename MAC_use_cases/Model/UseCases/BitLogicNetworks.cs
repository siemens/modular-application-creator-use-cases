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
    ///     Reusable helper methods for building bit-logic AND and RS networks using the
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
        ///     and a third network in SCL implementing the same RS logic, then imports it into the given
        ///     <paramref name="plcDevice"/>.
        ///     <para>
        ///     Network 1 (FBD): RS driven by <c>InputBool_R</c> / <c>InputBool_S1</c>; result written to
        ///     <c>OutputBool</c> (via Coil) and <c>RSOperand</c> (operand pin).
        ///     </para>
        ///     <para>
        ///     Network 2 (FBD): RS driven by <c>InputBool_R2</c> / <c>OutputBool</c> (chained from Network 1);
        ///     result written to <c>OutputBool2</c> and <c>RSOperand2</c>.
        ///     </para>
        ///     <para>
        ///     Network 3 (SCL): Simple assignment <c>#TempVariable := #OutputBool</c> demonstrating
        ///     how an SCL network can be mixed into an FBD block.
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
            // FBD RS pair (Networks 1 & 2)
            staticItf.Add(new InterfaceParameter("RSOperand", "Bool"));
            staticItf.Add(new InterfaceParameter("OutputBool", "Bool"));
            staticItf.Add(new InterfaceParameter("RSOperand2", "Bool"));
            staticItf.Add(new InterfaceParameter("OutputBool2", "Bool"));
            // SCL network temp variable
            staticItf.Add(new InterfaceParameter("TempVariable", "Bool"));

            // Network 1: first RS flip-flop (FBD)
            var rsNetwork1 = CreateRSNetwork("#InputBool_R", "#InputBool_S1", "#OutputBool", "#RSOperand", opnsProgrammingLanguage);
            block.Networks.Add(rsNetwork1.GenerateFixNetwork());

            // Network 2: second RS flip-flop (FBD) – S1 driven by #OutputBool written in Network 1
            var rsNetwork2 = CreateRSNetwork("#InputBool_R2", "#OutputBool", "#OutputBool2", "#RSOperand2", opnsProgrammingLanguage);
            block.Networks.Add(rsNetwork2.GenerateFixNetwork());

            // Network 3: simple SCL assignment – reads the result of Network 1 into TempVariable
            var sclCode = "#TempVariable := #OutputBool;";

            var sclNetworks = new Parser().ParseSclSnippet(sclCode, block, plcDevice, GroupBlockCalls.NOGROUPING);
            foreach (var sclNw in sclNetworks)
            {
                block.Networks.Add(sclNw);
            }

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
        ///     <para>
        ///     Network 3 (SCL): Simple assignment <c>#TempVariable := #RSOperand</c> demonstrating
        ///     how an SCL network can be mixed into a LAD block.
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
            // SCL network temp variable
            staticItf.Add(new InterfaceParameter("TempVariable", "Bool"));

            // Network 1: RS flip-flop in LAD
            var rsNetwork1 = CreateRSNetworkLAD("#InputBool_R", "#InputBool_S1", "#RSOperand");
            block.Networks.Add(rsNetwork1.GenerateFixNetwork());

            // Network 2: RS flip-flop in LAD – S1 driven by the operand written in Network 1
            var rsNetwork2 = CreateRSNetworkLAD("#InputBool_R2", "#RSOperand", "#RSOperand2");
            block.Networks.Add(rsNetwork2.GenerateFixNetwork());

            // Network 3: simple SCL assignment – reads the result of Network 1 into TempVariable
            var sclCode = "#TempVariable := #RSOperand;";
            var sclNetworks = new Parser().ParseSclSnippet(sclCode, block, plcDevice, GroupBlockCalls.NOGROUPING);
            foreach (var sclNw in sclNetworks)
            {
                block.Networks.Add(sclNw);
            }

            block.GenerateXmlBlock(plcDevice);
        }

        // ----------------------------------------------------------------
        // AND network helpers
        // ----------------------------------------------------------------

        /// <summary>
        ///     Adds an AND (<c>"A"</c>) system block call to the given network.
        ///     <para>
        ///     The call has <paramref name="cardinality"/> input pins (<c>in1</c>…<c>inN</c>)
        ///     and one output pin (<c>out</c>). Following the reference project convention the
        ///     output and the last input are negated.
        ///     </para>
        /// </summary>
        /// <param name="parent">Network to add the call to.</param>
        /// <param name="cardinality">Number of boolean inputs.</param>
        public static SystemBlockCall CreateANDCall(XmlNetwork parent, int cardinality)
        {
            var andCall = parent.AddSystemBlockCall("A");
            andCall.AddSystemParameter("Card", "Cardinality", cardinality.ToString());
            andCall.AddCallParameter("out", "Bool", Section.Output);
            for (int i = 1; i <= cardinality; i++)
            {
                andCall.AddCallParameter("in" + i, "Bool", Section.Input);
            }
            // Negate the output and the last input (mirrors reference project behaviour)
            andCall.CallParameters.First(x => x.Section == Section.Output).Negated = true;
            andCall.CallParameters.Last(x => x.Section == Section.Input).Negated = true;
            return andCall;
        }

        /// <summary>
        ///     Creates a network that ANDs all <paramref name="inputVariableNames"/> together and
        ///     writes the result to <paramref name="outputVariableName"/> via a Coil element.
        /// </summary>
        /// <param name="inputVariableNames">Variables connected to the AND inputs (e.g. <c>#InputBool0</c>).</param>
        /// <param name="outputVariableName">Variable written by the Coil (AND result).</param>
        /// <param name="opnsProgrammingLanguage">Network language – use <see cref="MEPlang.FBD"/>.</param>
        public static XmlNetwork CreateANDCallNetwork(
            System.Collections.Generic.List<string> inputVariableNames,
            string outputVariableName,
            MEPlang opnsProgrammingLanguage)
        {
            var xmlNw = new XmlNetwork(opnsProgrammingLanguage);
            var andCall = CreateANDCall(xmlNw, inputVariableNames.Count);

            var currentIdx = 1;
            foreach (var varName in inputVariableNames)
            {
                var inputVar = xmlNw.AddVariable(varName);
                xmlNw.AddConnection(inputVar, andCall.CallParameters.Single(x => x.Name == "in" + currentIdx));
                currentIdx++;
            }

            var assignmentCall = CreateAssignmentCall(xmlNw);
            xmlNw.AddConnection(andCall.CallParameters.Single(x => x.Name.Contains("out")),
                                assignmentCall.CallParameters.First(x => x.Name.Contains("in")));
            var assignmentOperandOutput = xmlNw.AddVariable(outputVariableName);
            xmlNw.AddConnection(assignmentCall.CallParameters.First(x => x.Name.Contains("operand")),
                                assignmentOperandOutput);

            return xmlNw;
        }

        /// <summary>
        ///     Creates a <strong>single</strong> network that contains two independent parallel
        ///     AND→RS circuits side by side (FBD only).
        ///     <para>
        ///     Each circuit: AND(2) → RS.S1, with a shared R input and a shared DB operand.
        ///     The RS Q output is explicitly left unconnected (<c>null</c>).
        ///     </para>
        ///     <para>
        ///     <strong>Important:</strong> all elements of circuit 1 must be added before
        ///     starting circuit 2 – this is an <see cref="XmlNetwork"/> requirement.
        ///     </para>
        /// </summary>
        /// <param name="inputVariableNames">Two variable names used as AND inputs for both circuits.</param>
        /// <param name="rInputName">Variable connected to the R (reset) input of both RS calls.</param>
        /// <param name="operandVariableName">DB variable connected to the operand output of both RS calls.</param>
        public static XmlNetwork CreateNetworkWith2Circuits(
            System.Collections.Generic.List<string> inputVariableNames,
            string rInputName,
            string operandVariableName)
        {
            var xmlNw = new XmlNetwork(MEPlang.FBD);

            // ---- Circuit 1 ----
            var andCall1 = CreateANDCall(xmlNw, 2);
            var idx = 1;
            foreach (var varName in inputVariableNames)
            {
                var inputVar = xmlNw.AddVariable(varName);
                xmlNw.AddConnection(inputVar, andCall1.CallParameters.Single(x => x.Name == "in" + idx));
                idx++;
            }
            var rsCall1 = CreateRSCall(xmlNw);
            xmlNw.AddConnection(andCall1.CallParameters.Single(x => x.Name == "out"),
                                rsCall1.CallParameters.Single(x => x.Name == "s1"));
            xmlNw.AddConnection(xmlNw.AddVariable(rInputName),
                                rsCall1.CallParameters.Single(x => x.Name == "r"));
            xmlNw.AddConnection(rsCall1.CallParameters.Single(x => x.Name == "operand"),
                                xmlNw.AddVariable(operandVariableName));
            // Q left unconnected – must be declared explicitly
            xmlNw.AddConnection(rsCall1.CallParameters.Single(x => x.Name == "q"), null);

            // ---- Circuit 2 (start only after circuit 1 is complete) ----
            idx = 1;
            var andCall2 = CreateANDCall(xmlNw, 2);
            foreach (var varName in inputVariableNames)
            {
                var inputVar = xmlNw.AddVariable(varName);
                xmlNw.AddConnection(inputVar, andCall2.CallParameters.Single(x => x.Name == "in" + idx));
                idx++;
            }
            var rsCall2 = CreateRSCall(xmlNw);
            xmlNw.AddConnection(andCall2.CallParameters.Single(x => x.Name == "out"),
                                rsCall2.CallParameters.Single(x => x.Name == "s1"));
            xmlNw.AddConnection(xmlNw.AddVariable(rInputName),
                                rsCall2.CallParameters.Single(x => x.Name == "r"));
            xmlNw.AddConnection(rsCall2.CallParameters.Single(x => x.Name == "operand"),
                                xmlNw.AddVariable(operandVariableName));
            xmlNw.AddConnection(rsCall2.CallParameters.Single(x => x.Name == "q"), null);

            return xmlNw;
        }

        // ----------------------------------------------------------------
        // FB generators – AND networks
        // ----------------------------------------------------------------

        /// <summary>
        ///     Generates an FB in FBD language with two networks and imports it into
        ///     <paramref name="plcDevice"/>:
        ///     <list type="bullet">
        ///     <item>Network 1: AND of three inputs written to <c>OutputBool</c> via Coil.</item>
        ///     <item>Network 2: Two parallel AND→RS circuits in a single network (see
        ///     <see cref="CreateNetworkWith2Circuits"/>).</item>
        ///     </list>
        /// </summary>
        /// <param name="blockName">Name of the FB to create in TIA Portal.</param>
        /// <param name="plcDevice">Target PLC device.</param>
        public static void GenerateFbWithANDNetworkFBD(string blockName, PlcDevice plcDevice)
        {
            var block = new XmlFB(blockName);
            block.BlockAttributes.ProgrammingLanguage = MacPLang.FBD;

            block.Interface[InterfaceSections.Input].Add(new InterfaceParameter("InputBool1", "Bool"));
            block.Interface[InterfaceSections.Input].Add(new InterfaceParameter("InputBool2", "Bool"));
            block.Interface[InterfaceSections.Input].Add(new InterfaceParameter("InputBool3", "Bool"));

            var staticItf = block.Interface[InterfaceSections.Static];
            staticItf.Add(new InterfaceParameter("OutputBool", "Bool"));
            staticItf.Add(new InterfaceParameter("AndOperand", "Bool"));

            // Network 1: AND of all three inputs → OutputBool
            var inputVars = new System.Collections.Generic.List<string>
                { "#InputBool1", "#InputBool2", "#InputBool3" };
            var andNetwork = CreateANDCallNetwork(inputVars, "#OutputBool", MEPlang.FBD);
            block.Networks.Add(andNetwork.GenerateFixNetwork());

            // Network 2: two parallel AND→RS circuits in a single FBD network
            var circuitNetwork = CreateNetworkWith2Circuits(
                new System.Collections.Generic.List<string> { "#InputBool1", "#InputBool2" },
                "#InputBool3",
                "#AndOperand");
            block.Networks.Add(circuitNetwork.GenerateFixNetwork());

            block.GenerateXmlBlock(plcDevice);
        }

        /// <summary>
        ///     Generates an FB in LAD language with two networks and imports it into
        ///     <paramref name="plcDevice"/>:
        ///     <list type="bullet">
        ///     <item>Network 1: AND of three inputs written to <c>OutputBool</c> via Coil.</item>
        ///     <item>Network 2: Two parallel AND→RS circuits (Q left unconnected – LAD convention).</item>
        ///     </list>
        ///     <para>
        ///     Like all LAD blocks using <see cref="XmlNetwork"/>, the networks are built with
        ///     <see cref="MEPlang.FBD"/> at the network level. The block-level
        ///     <c>ProgrammingLanguage = LAD</c> attribute controls the TIA Portal display.
        ///     </para>
        /// </summary>
        /// <param name="blockName">Name of the FB to create in TIA Portal.</param>
        /// <param name="plcDevice">Target PLC device.</param>
        public static void GenerateFbWithANDNetworkLAD(string blockName, PlcDevice plcDevice)
        {
            var block = new XmlFB(blockName);
            block.BlockAttributes.ProgrammingLanguage = MacPLang.LAD;

            block.Interface[InterfaceSections.Input].Add(new InterfaceParameter("InputBool1", "Bool"));
            block.Interface[InterfaceSections.Input].Add(new InterfaceParameter("InputBool2", "Bool"));
            block.Interface[InterfaceSections.Input].Add(new InterfaceParameter("InputBool3", "Bool"));

            var staticItf = block.Interface[InterfaceSections.Static];
            staticItf.Add(new InterfaceParameter("OutputBool", "Bool"));
            staticItf.Add(new InterfaceParameter("AndOperand", "Bool"));

            // Network 1: AND of all three inputs → OutputBool (FBD graph XML, LAD display)
            var inputVars = new System.Collections.Generic.List<string>
                { "#InputBool1", "#InputBool2", "#InputBool3" };
            var andNetwork = CreateANDCallNetwork(inputVars, "#OutputBool", MEPlang.FBD);
            block.Networks.Add(andNetwork.GenerateFixNetwork());

            // Network 2: two parallel AND→RS circuits (Q unconnected – LAD convention)
            var circuitNetwork = CreateNetworkWith2Circuits(
                new System.Collections.Generic.List<string> { "#InputBool1", "#InputBool2" },
                "#InputBool3",
                "#AndOperand");
            block.Networks.Add(circuitNetwork.GenerateFixNetwork());

            block.GenerateXmlBlock(plcDevice);
        }
    }
}
