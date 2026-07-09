using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Siemens.Automation.ModularApplicationCreator.ControlModules.ModuleEssentials.Objects.Generation.Openness.XML;
using Siemens.Automation.ModularApplicationCreator.ControlModules.ModuleEssentials.Objects.Generation.Openness.XML.Parts;
using Siemens.Automation.ModularApplicationCreator.Tia.Helper;
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

            //var assignmentCall = CreateAssignmentCall(xmlNw);
            //xmlNw.AddConnection(rsCall.CallParameters.Single(x => x.Name.Contains("q")), assignmentCall.CallParameters.First(x => x.Name.Contains("in")));
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
            staticItf.Add(new InterfaceParameter("RSOperand2", "Bool"));
            // SCL network temp variable
            staticItf.Add(new InterfaceParameter("TempVariable", "Bool"));

            // Network 1: first RS flip-flop (FBD)
            var rsNetwork1 = CreateRSNetwork("#InputBool_R", "#InputBool_S1", "#RSOperand", opnsProgrammingLanguage);
            block.Networks.Add(rsNetwork1.GenerateFixNetwork());

            // Network 2: second RS flip-flop (FBD) – S1 driven by #OutputBool written in Network 1
            var rsNetwork2 = CreateRSNetwork("#InputBool_R2", "#RSOperand", "#RSOperand2", opnsProgrammingLanguage);
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
        ///     Network 3 (SCL): Simple assignment <c>#TempVariable := #RSOperand</c> demonstrating
        ///     how an SCL network can be mixed into a LAD block.
        ///     </para>
        ///     <para>
        ///     <b>LAD import mechanism:</b> Networks are built with <see cref="MEPlang.FBD"/> because
        ///     <see cref="XmlNetwork"/> only generates valid graph XML in FBD mode. After the initial
        ///     import the block is exported, every CompileUnit-level <c>ProgrammingLanguage</c> element
        ///     is changed from <c>FBD</c> to <c>LAD</c>, and the block is reimported so TIA Portal
        ///     displays it correctly as LAD. SCL networks are left unchanged.
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

            // Generate the XML in FBD mode (XmlNetwork only produces valid graph XML in FBD),
            // patch every CompileUnit language to LAD in memory, then import directly.
            ImportBlockAsLad(block, plcDevice);
        }

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

        /// <summary>
        ///     Creates one AND→RS circuit in a single network (FBD graph, usable for LAD import).
        ///     AND(2) drives RS.S1, a separate input drives RS.R, and RS.operand writes to
        ///     <paramref name="operandVariableName"/>. RS.Q is left unconnected.
        /// </summary>
        private static XmlNetwork CreateSingleAndRsCircuitNetwork(
            string andInput1,
            string andInput2,
            string rInputName,
            string operandVariableName)
        {
            var xmlNw = new XmlNetwork(MEPlang.FBD);

            var andCall = CreateANDCall(xmlNw, 2);
            xmlNw.AddConnection(xmlNw.AddVariable(andInput1), andCall.CallParameters.Single(x => x.Name == "in1"));
            xmlNw.AddConnection(xmlNw.AddVariable(andInput2), andCall.CallParameters.Single(x => x.Name == "in2"));

            var rsCall = CreateRSCall(xmlNw);
            xmlNw.AddConnection(andCall.CallParameters.Single(x => x.Name == "out"),
                                rsCall.CallParameters.Single(x => x.Name == "s1"));
            xmlNw.AddConnection(xmlNw.AddVariable(rInputName),
                                rsCall.CallParameters.Single(x => x.Name == "r"));
            xmlNw.AddConnection(rsCall.CallParameters.Single(x => x.Name == "operand"),
                                xmlNw.AddVariable(operandVariableName));
            xmlNw.AddConnection(rsCall.CallParameters.Single(x => x.Name == "q"), null);

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

            // Network 2: AndOperand := OutputBool
            var assignNetwork = new XmlNetwork(MEPlang.FBD);
            var assignCall = CreateAssignmentCall(assignNetwork);
            var outputBoolVar = assignNetwork.AddVariable("#OutputBool");
            assignNetwork.AddConnection(outputBoolVar,
                assignCall.CallParameters.First(x => x.Name.Contains("in")));
            var andOperandVar = assignNetwork.AddVariable("#AndOperand");
            assignNetwork.AddConnection(
                assignCall.CallParameters.First(x => x.Name.Contains("operand")),
                andOperandVar);
            block.Networks.Add(assignNetwork.GenerateFixNetwork());

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
        ///     <b>LAD import mechanism:</b> Networks are built with <see cref="MEPlang.FBD"/> because
        ///     <see cref="XmlNetwork"/> only generates valid graph XML in FBD mode. After the initial
        ///     import the block is exported, every CompileUnit-level <c>ProgrammingLanguage</c> element
        ///     is changed from <c>FBD</c> to <c>LAD</c>, and the block is reimported so TIA Portal
        ///     displays it correctly as LAD.
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

            // Network 2: AndOperand := OutputBool
            var assignNetwork = new XmlNetwork(MEPlang.FBD);
            var assignCall = CreateAssignmentCall(assignNetwork);
            var outputBoolVar = assignNetwork.AddVariable("#OutputBool");
            assignNetwork.AddConnection(outputBoolVar,
                assignCall.CallParameters.First(x => x.Name.Contains("in")));
            var andOperandVar = assignNetwork.AddVariable("#AndOperand");
            assignNetwork.AddConnection(
                assignCall.CallParameters.First(x => x.Name.Contains("operand")),
                andOperandVar);
            block.Networks.Add(assignNetwork.GenerateFixNetwork());

            // Generate the XML in FBD mode (XmlNetwork only produces valid graph XML in FBD),
            // patch every CompileUnit language to LAD in memory, then import directly.
            ImportBlockAsLad(block, plcDevice);
        }

        // ----------------------------------------------------------------
        // LAD import helper
        // ----------------------------------------------------------------

        // FlgNet namespace used in every CompileUnit network source
        private static readonly XNamespace FlgNetNs =
            "http://www.siemens.com/automation/Openness/SW/NetworkSource/FlgNet/v4";

        /// <summary>
        ///     Generates the block XML using <see cref="MacPLang.FBD"/> (the only mode in which
        ///     <see cref="XmlNetwork"/> produces valid graph XML), transforms the FBD wiring
        ///     topology of every LAD CompileUnit into LAD topology (inserts Contact elements
        ///     between variable accesses and instruction inputs), patches every
        ///     <c>ProgrammingLanguage</c> element from <c>FBD</c> to <c>LAD</c> in memory,
        ///     then imports the patched block.
        ///     SCL networks already carry <c>SCL</c> as their language and are untouched.
        /// </summary>
        private static void ImportBlockAsLad(XmlFB block, PlcDevice plcDevice)
        {
            // Generate the XML in FBD mode – the only mode that produces valid Openness graph XML.
            var xdoc = block.GenerateXmlFile(MacPLang.FBD);

            // Compute a document-wide UId ceiling so new elements added to any FlgNet
            // cannot collide with UIds in other compile units of the same FB.
            int nextUId = xdoc.Descendants()
                              .Select(e => e.Attribute("UId")?.Value)
                              .Where(v => v != null)
                              .Select(v => { int n; return int.TryParse(v, out n) ? n : 0; })
                              .DefaultIfEmpty(0)
                              .Max() + 1;

            // Transform each FBD CompileUnit to LAD topology, then patch the language label.
            foreach (var compileUnit in xdoc.Descendants("SW.Blocks.CompileUnit").ToList())
            {
                var langElem = compileUnit.Descendants("ProgrammingLanguage").FirstOrDefault();
                if (langElem == null || langElem.Value != "FBD")
                    continue; // leave SCL networks untouched

                var flgNet = compileUnit.Descendants(FlgNetNs + "FlgNet").FirstOrDefault();
                if (flgNet != null)
                    ConvertFlgNetToLad(flgNet, ref nextUId);

                langElem.Value = "LAD";
            }

            // Also patch block-level ProgrammingLanguage (FB header).
            foreach (var langElem in xdoc.Descendants("ProgrammingLanguage")
                                         .Where(e => e.Value == "FBD").ToList())
                langElem.Value = "LAD";

            // Import patched XML via a temporary file and clean it up immediately.
            var tempFile = Path.ChangeExtension(Path.GetTempFileName(), ".xml");
            try
            {
                xdoc.Save(tempFile);
                OpennessFuncs.ImportBlockToPlc(tempFile, plcDevice);
            }
            finally
            {
                if (File.Exists(tempFile))
                    File.Delete(tempFile);
            }
        }

        /// <summary>
        ///     Rewrites the wiring inside a single <c>FlgNet</c> element from FBD topology to
        ///     LAD topology:
        ///     <list type="bullet">
        ///     <item>AND (<c>"A"</c>) Parts are expanded into a series Contact chain; negated
        ///     AND inputs become NC Contacts; the (unsupported) negated <c>out</c> is dropped.</item>
        ///     <item>Remaining direct <c>IdentCon→NameCon</c> input wires each get a single Contact.</item>
        ///     <item>All Contact <c>"in"</c> pins that start a rung share one single
        ///     <c>&lt;Powerrail/&gt;</c> Wire (the only valid form in LAD).</item>
        ///     </list>
        /// </summary>
        private static void ConvertFlgNetToLad(XElement flgNet, ref int nextUId)
        {
            var parts = flgNet.Element(FlgNetNs + "Parts");
            var wires = flgNet.Element(FlgNetNs + "Wires");
            if (parts == null || wires == null)
                return;

            // ONE Powerrail Wire for the whole network; all rung-entry Contact "in" pins attach here.
            var powerrailWire = new XElement(FlgNetNs + "Wire",
                new XAttribute("UId", nextUId++),
                new XElement(FlgNetNs + "Powerrail"));

            // Step 1 – expand AND Parts into series Contact chains
            ExpandAndParts(parts, wires, powerrailWire, ref nextUId);

            // Step 2 – convert remaining direct IdentCon→NameCon(input) wires to single Contacts
            ConvertDirectInputWires(parts, wires, powerrailWire, ref nextUId);

            if (powerrailWire.Elements(FlgNetNs + "NameCon").Any())
                wires.AddFirst(powerrailWire);

            // Step 3 – reorder Parts and Wires in strict left-to-right signal-flow order
            SortFlgNetInFlowOrder(flgNet);
        }

        // Returns the integer value of an element's UId attribute (0 if absent/non-numeric).
        private static int GetUId(XElement e) =>
            int.TryParse(e.Attribute("UId")?.Value, out int v) ? v : 0;

        /// <summary>
        ///     Expands every AND (<c>"A"</c>) Part into a series Contact chain:
        ///     powerrail → C1 → C2 → … → Ck → (what AND.out was wired to).
        ///     Negated AND inputs become NC Contacts; negated AND output is discarded.
        /// </summary>
        private static void ExpandAndParts(XElement parts, XElement wires,
                                           XElement powerrailWire, ref int nextUId)
        {
            var andParts = parts.Elements(FlgNetNs + "Part")
                                .Where(p => p.Attribute("Name")?.Value == "A")
                                .ToList();

            foreach (var andPart in andParts)
            {
                int andUId = GetUId(andPart);

                var negatedPins = new System.Collections.Generic.HashSet<string>(
                    andPart.Elements(FlgNetNs + "Negated")
                           .Select(n => n.Attribute("Name")?.Value)
                           .Where(n => n != null));

                // Input wires: IdentCon(var) → NameCon(inN, andUId) – sorted by index
                var inputWires = wires.Elements(FlgNetNs + "Wire")
                                      .Where(w =>
                                      {
                                          var ch = w.Elements().ToList();
                                          return ch.Count == 2
                                              && ch[0].Name.LocalName == "IdentCon"
                                              && ch[1].Name.LocalName == "NameCon"
                                              && GetUId(ch[1]) == andUId;
                                      })
                                      .OrderBy(w =>
                                      {
                                          var name = w.Elements(FlgNetNs + "NameCon").First()
                                                       .Attribute("Name")?.Value ?? "";
                                          return int.TryParse(name.Replace("in", ""), out int idx) ? idx : 0;
                                      })
                                      .ToList();

                // Output wire: NameCon(out, andUId) → NameCon(downstreamPin, downstreamUId)
                var outputWire = wires.Elements(FlgNetNs + "Wire")
                                      .FirstOrDefault(w =>
                                      {
                                          var ch = w.Elements().ToList();
                                          return ch.Count == 2
                                              && ch[0].Name.LocalName == "NameCon"
                                              && ch[0].Attribute("Name")?.Value == "out"
                                              && GetUId(ch[0]) == andUId
                                              && ch[1].Name.LocalName == "NameCon";
                                      });

                if (inputWires.Count == 0 || outputWire == null)
                {
                    andPart.Remove();
                    outputWire?.Remove();
                    continue;
                }

                var downstreamCon = outputWire.Elements(FlgNetNs + "NameCon").Last();

                int prevContactUId = -1;
                foreach (var inWire in inputWires)
                {
                    var identCon = inWire.Elements(FlgNetNs + "IdentCon").First();
                    string pinName = inWire.Elements(FlgNetNs + "NameCon").First()
                                           .Attribute("Name")?.Value ?? "";
                    bool isNc = negatedPins.Contains(pinName);

                    int cUId = nextUId++;
                    var contactPart = new XElement(FlgNetNs + "Part",
                        new XAttribute("UId", cUId),
                        new XAttribute("Name", "Contact"));
                    if (isNc)
                        contactPart.Add(new XElement(FlgNetNs + "Negated",
                            new XAttribute("Name", "operand")));
                    parts.Add(contactPart);

                    // Variable → Contact.operand
                    wires.Add(new XElement(FlgNetNs + "Wire",
                        new XAttribute("UId", nextUId++),
                        new XElement(FlgNetNs + "IdentCon",
                            new XAttribute("UId", identCon.Attribute("UId").Value)),
                        new XElement(FlgNetNs + "NameCon",
                            new XAttribute("Name", "operand"),
                            new XAttribute("UId", cUId))));

                    if (prevContactUId == -1)
                    {
                        // First in chain: driven from powerrail
                        powerrailWire.Add(new XElement(FlgNetNs + "NameCon",
                            new XAttribute("Name", "in"),
                            new XAttribute("UId", cUId)));
                    }
                    else
                    {
                        // Chain link: prevContact.out → thisContact.in
                        wires.Add(new XElement(FlgNetNs + "Wire",
                            new XAttribute("UId", nextUId++),
                            new XElement(FlgNetNs + "NameCon",
                                new XAttribute("Name", "out"),
                                new XAttribute("UId", prevContactUId)),
                            new XElement(FlgNetNs + "NameCon",
                                new XAttribute("Name", "in"),
                                new XAttribute("UId", cUId))));
                    }
                    prevContactUId = cUId;
                    inWire.Remove();
                }

                // Last Contact.out → whatever AND.out was connected to
                wires.Add(new XElement(FlgNetNs + "Wire",
                    new XAttribute("UId", nextUId++),
                    new XElement(FlgNetNs + "NameCon",
                        new XAttribute("Name", "out"),
                        new XAttribute("UId", prevContactUId)),
                    new XElement(FlgNetNs + "NameCon",
                        new XAttribute("Name", downstreamCon.Attribute("Name")?.Value),
                        new XAttribute("UId", downstreamCon.Attribute("UId")?.Value))));

                andPart.Remove();
                outputWire.Remove();
            }
        }

        /// <summary>
        ///     Re-orders <c>&lt;Parts&gt;</c> and <c>&lt;Wires&gt;</c> inside a single
        ///     <c>FlgNet</c> element so that both collections are listed in strict left-to-right
        ///     signal-flow (topological) order, which TIA Portal requires for LAD import.
        ///
        ///     Critical requirement: The order of Parts in the Parts list must match the order
        ///     that Parts appear in the Powerrail wire's NameCon connections. After the Powerrail-connected
        ///     Parts, remaining Parts must be in topological order.
        /// </summary>
        private static void SortFlgNetInFlowOrder(XElement flgNet)
        {
            var partsEl = flgNet.Element(FlgNetNs + "Parts");
            var wiresEl = flgNet.Element(FlgNetNs + "Wires");
            if (partsEl == null || wiresEl == null) return;

            // Split Parts into Access/IdentCon nodes (variables) and instruction Part nodes.
            var accessNodes = partsEl.Elements()
                                     .Where(e => e.Name.LocalName == "Access")
                                     .ToList();
            var instrNodes  = partsEl.Elements()
                                     .Where(e => e.Name.LocalName == "Part")
                                     .ToList();

            if (instrNodes.Count > 0)
            {
                var instrUIds = new System.Collections.Generic.HashSet<int>(instrNodes.Select(GetUId));

                // Find the Powerrail wire and extract the order of Parts it connects to
                var powerrailOrder = new System.Collections.Generic.List<int>();
                var powerrailWire = wiresEl.Elements(FlgNetNs + "Wire")
                                           .FirstOrDefault(w => w.Elements(FlgNetNs + "Powerrail").Any());
                if (powerrailWire != null)
                {
                    foreach (var nameCon in powerrailWire.Elements(FlgNetNs + "NameCon"))
                    {
                        int uid = GetUId(nameCon);
                        if (instrUIds.Contains(uid))
                            powerrailOrder.Add(uid);
                    }
                }

                // Build connectivity maps
                var outgoing = new System.Collections.Generic.Dictionary<int, System.Collections.Generic.List<int>>();
                var incoming = new System.Collections.Generic.Dictionary<int, System.Collections.Generic.List<int>>();

                foreach (var uid in instrUIds)
                {
                    outgoing[uid] = new System.Collections.Generic.List<int>();
                    incoming[uid] = new System.Collections.Generic.List<int>();
                }

                // Analyze wires to build connectivity
                foreach (var wire in wiresEl.Elements(FlgNetNs + "Wire"))
                {
                    var ch = wire.Elements().ToList();
                    if (ch.Count < 2) continue;

                    // Skip Powerrail wires for connectivity analysis
                    if (ch[0].Name.LocalName == "Powerrail")
                        continue;

                    // Find Part output
                    int sourceUId = -1;
                    if (ch[0].Name.LocalName == "NameCon")
                    {
                        var pinName = ch[0].Attribute("Name")?.Value;
                        if (pinName == "out" || pinName == "q")
                        {
                            int uid = GetUId(ch[0]);
                            if (instrUIds.Contains(uid))
                                sourceUId = uid;
                        }
                    }

                    // Find Part inputs
                    foreach (var target in ch.Skip(1).Where(c => c.Name.LocalName == "NameCon"))
                    {
                        var pinName = target.Attribute("Name")?.Value ?? "";
                        if (pinName == "out" || pinName == "q" || pinName == "operand")
                            continue;

                        int dstUId = GetUId(target);
                        if (instrUIds.Contains(dstUId))
                        {
                            if (sourceUId > 0 && sourceUId != dstUId)
                            {
                                outgoing[sourceUId].Add(dstUId);
                                incoming[dstUId].Add(sourceUId);
                            }
                        }
                    }
                }

                // Build sorted list: Powerrail-connected Parts first (in their Powerrail order),
                // then topologically sort remaining Parts
                var sorted = new System.Collections.Generic.List<int>();
                var visited = new System.Collections.Generic.HashSet<int>();

                // Add Powerrail-connected Parts in their exact Powerrail order
                foreach (var uid in powerrailOrder)
                {
                    sorted.Add(uid);
                    visited.Add(uid);
                }

                // For each Powerrail-connected Part, add all its downstream dependencies
                // in topological order
                foreach (var rootUId in powerrailOrder)
                {
                    var toVisit = new System.Collections.Generic.Queue<int>();
                    toVisit.Enqueue(rootUId);

                    while (toVisit.Count > 0)
                    {
                        int uid = toVisit.Dequeue();
                        foreach (var nextUId in outgoing[uid])
                        {
                            if (!visited.Contains(nextUId))
                            {
                                // Check if all dependencies are visited before adding
                                bool allDepsVisited = incoming[nextUId].All(dep => visited.Contains(dep));
                                if (allDepsVisited)
                                {
                                    sorted.Add(nextUId);
                                    visited.Add(nextUId);
                                    toVisit.Enqueue(nextUId);
                                }
                            }
                        }
                    }
                }

                // Add any remaining Parts (shouldn't happen, but safeguard)
                foreach (var uid in instrUIds.Where(u => !visited.Contains(u)))
                    sorted.Add(uid);

                var uidToNode = instrNodes.ToDictionary(GetUId);
                partsEl.RemoveAll();
                foreach (var n in accessNodes)  partsEl.Add(n);
                foreach (var uid in sorted)
                    if (uidToNode.ContainsKey(uid)) partsEl.Add(uidToNode[uid]);
            }

            // Sort Wires: Powerrail first; then other wires in original order
            var allWires = wiresEl.Elements(FlgNetNs + "Wire").ToList();
            var powerrailWires = allWires.Where(w => w.Elements(FlgNetNs + "Powerrail").Any()).ToList();
            var otherWires = allWires.Except(powerrailWires).ToList();

            wiresEl.RemoveAll();
            foreach (var w in powerrailWires) wiresEl.Add(w);
            foreach (var w in otherWires) wiresEl.Add(w);
        }

        /// <summary>
        ///     Converts remaining direct <c>IdentCon→NameCon(input)</c> wires (e.g. variable
        ///     driving RS.r or RS.s1 directly) into single Contact elements fed from the powerrail.
        /// </summary>
        private static void ConvertDirectInputWires(XElement parts, XElement wires,
                                                    XElement powerrailWire, ref int nextUId)
        {
            var inputWires = wires.Elements(FlgNetNs + "Wire")
                                  .Where(w =>
                                  {
                                      var ch = w.Elements().ToList();
                                      return ch.Count == 2
                                          && ch[0].Name.LocalName == "IdentCon"
                                          && ch[1].Name.LocalName == "NameCon"
                                          && ch[1].Attribute("Name")?.Value != "operand"; // skip wires already created by ExpandAndParts
                                  })
                                  .ToList();

            foreach (var wire in inputWires)
            {
                var identCon = wire.Elements(FlgNetNs + "IdentCon").First();
                var nameCon  = wire.Elements(FlgNetNs + "NameCon").First();

                int cUId = nextUId++;
                parts.Add(new XElement(FlgNetNs + "Part",
                    new XAttribute("UId", cUId),
                    new XAttribute("Name", "Contact")));

                powerrailWire.Add(new XElement(FlgNetNs + "NameCon",
                    new XAttribute("Name", "in"),
                    new XAttribute("UId", cUId)));

                wires.Add(new XElement(FlgNetNs + "Wire",
                    new XAttribute("UId", nextUId++),
                    new XElement(FlgNetNs + "IdentCon",
                        new XAttribute("UId", identCon.Attribute("UId").Value)),
                    new XElement(FlgNetNs + "NameCon",
                        new XAttribute("Name", "operand"),
                        new XAttribute("UId", cUId))));

                wires.Add(new XElement(FlgNetNs + "Wire",
                    new XAttribute("UId", nextUId++),
                    new XElement(FlgNetNs + "NameCon",
                        new XAttribute("Name", "out"),
                        new XAttribute("UId", cUId)),
                    new XElement(FlgNetNs + "NameCon",
                        new XAttribute("Name", nameCon.Attribute("Name")?.Value),
                        new XAttribute("UId", nameCon.Attribute("UId")?.Value))));

                wire.Remove();
            }
        }
    }
}
