using Siemens.Automation.ModularApplicationCreator.Tia;
using Siemens.Automation.ModularApplicationCreator.Tia.Helper;
using Siemens.Automation.ModularApplicationCreator.Tia.Helper.Create_XML_Block;
using Siemens.Automation.ModularApplicationCreator.Tia.Helper.Create_XML_Block.XmlBlocks.BlockFrames;
using Siemens.Automation.ModularApplicationCreator.Tia.Openness;
using Siemens.Automation.ModularApplicationCreator.Tia.TiaAttributeFuncs;
using System.IO;
using MAC_use_cases.TiaImports;
using ProgrammingLanguage = Siemens.Automation.ModularApplicationCreator.Tia.Helper.Create_XML_Block.ProgrammingLanguage;
using System.Linq;

namespace MAC_use_cases.Model.UseCases
{
    /// <summary>
    /// All the functions to configure and generate Blocks are defined here.
    /// </summary>
    public class GenericBlockCreation
    {
        /// <summary>
        /// This is the name of the generated DB 
        /// </summary>
        public static string dbName = "myDB";
        /// <summary>
        /// This is the name of the generated parameter
        /// </summary>
        public static string parameterName = "myParameterName";
        /// <summary>
        /// This is the name of the generated datatype
        /// </summary>
        public static string dataType = "myDataType";

        /// <summary>
        /// This function creates a FB in the target plc (folder under program blocks). Additional titles or comments can be added here too.
        /// \image html CreateFB.png
        /// </summary>
        /// <param name="blockName">The desired name</param>
        /// <param name="instanceDbName">Name of the instanceDB</param>
        /// <param name="plcDevice">The Plc in which it should be created</param>
        public static void CreateFB(string blockName, string instanceDbName, PlcDevice plcDevice)
        {
            var myFB = new XmlFB(blockName);

            BlockCall myFBCall = new BlockCall(instanceDbName, plcDevice)
            {
                ["Input1"] = "1", // inputVariable1
                ["Input2"] = "0", // inputVariable1
                //["Input2"] = "inputVariable1", 

                //["Output1"] = "outputVariable1", 
            };

            BlockNetwork myFbBlockNetwork = new BlockNetwork();

            myFbBlockNetwork.NetworkTitles[TypeMapper.BaseCulture.Name] = "myFB Network Title"; // If necessary

            myFbBlockNetwork.Blocks.Add(myFBCall);
            myFB.Networks.Add(myFbBlockNetwork);
            myFB.BlockAttributes.ProgrammingLanguage = ProgrammingLanguage.FBD;

            myFB.BlockComments[TypeMapper.BaseCulture.Name] = "myFB Block Comment"; // If necessary
            myFB.BlockTitles[TypeMapper.BaseCulture.Name] = "myFB Block Title";     // If necessary

            myFB.GenerateXmlBlock(plcDevice);
        }


        /// <summary>
        /// This function creates a Data Block with interface (sub-)parameters in a desired folder. The variables in the interface can have a standard data types or a data type defined in the project.
        /// \image html GenerateDB.png
        /// </summary>
        /// <param name="plcDevice">The PLC on which the equipment module is implemented</param>
        /// <param name="module">The corresponding equipment module</param>
        public static void GenerateDB(string dbName, PlcDevice plcDevice, MAC_use_casesEM module)
        {
            var axesDataDB = new XmlGlobalDB(dbName);
            var itf = axesDataDB.Interface[InterfaceSections.Static];

            //Create new parameters. The custom parameter means you are using your own user defined data type.
            var standardParam = new InterfaceParameter(parameterName, "Int");
            var customParam1 = new InterfaceParameter("var_struct", "Struct");
            customParam1.SubParameter.Add(new InterfaceParameter("Var1", "Bool"));
            customParam1.SubParameter.Add(new InterfaceParameter("Var2", "Int"));
            var customParam2 = new InterfaceParameter("datatyp_from_lib", "\"" + "myDataType" + "\"");

            //Add your parameters 
            itf.Add(standardParam);
            itf.Add(customParam1);
            itf.Add(customParam2);

            axesDataDB.GenerateXmlBlock(plcDevice);
        }

        /// <summary>
        /// This function creates a Block on the targetDevice in the targetContainer (folder under program blocks)
        /// </summary>
        /// <param name="code">The Block code as .scl or .xml </param>
        /// <param name="targetDevice">The PLC on which the equipment module is implemented</param>
        /// <param name="targetContainer">The corresponding equipment module</param>
        /// <param name="isScl">true: code string represents a scl block</param>
        private static void GenerateBlock(string code, PlcDevice targetDevice, ITarget targetContainer = null, bool isScl = true)
        {
            //Get the right file extension depending on the block language
            string extension = ".scl";
            if (!isScl)
            {
                extension = ".xml";
            }
            //Create a temporary file
            var filePath = Path.ChangeExtension(Path.GetTempFileName(), extension);

            //Write the code to this file
            File.WriteAllText(filePath, code);

            //Create the block on the plc using the file
            var block = OpennessFuncs.ImportBlockToPlc(filePath, targetDevice, targetContainer);

            //Delete the file
            File.Delete(filePath);
        }

        /// <summary>
        /// This function sets the default value of a parameter in a DB
        /// \image html SetDefaultValue.png
        /// </summary>
        /// <param name="dbName">The DB in which the value should be set</param>
        /// <param name="parameterName">The name of the parameter</param>
        /// <param name="dataType">DataType of the parameter</param>
        /// <param name="value">Value that should be set</param>
        /// <param name="parent">The module</param>
        public static void SetDefaultValue(string dbName, string parameterName, TIATYPE dataType, string value, MAC_use_casesEM parent)
        {
            TiaAttribute myParameterNameAttribute = TiaAttributeProvider.CreateAttribute(dataType, SOURCETYPE.BLOCKVAR,
                dbName, parameterName, parent);
            myParameterNameAttribute.Value = value;
        }

        /// <summary>
        /// This function creates an multiInstanceFB
        /// \image html MultiInstanceFB.png
        /// </summary>
        /// <param name="plcDevice">The plcDevice</param>
        /// <param name="tiaTemplateContext">The tiaTemplateContext</param>
        /// <param name="module">The Module</param>
        public static void GenerateMultiInstanceFB(PlcDevice plcDevice, string languagesettings, MAC_use_casesEM module)
        {
            var MyFB = new XmlFB("MyMultiInstanceFB");
            MyFB.BlockAttributes.ProgrammingLanguage = ProgrammingLanguage.LAD;

            MultiInstanceCall myMultiInstanceFBCall = new MultiInstanceCall("MyMultiInstanceDbName", "MAC_use_casesFB", plcDevice)
            {
                ["Input1"] = "1", // inputVariable1
                ["Input2"] = "0", // inputVariable1
                //["Input2"] = "inputVariable1", 

                //["Output1"] = "outputVariable1", 
            };

            //manipulate interface
            //var axesDataDB = new XmlFB (dbName);
            var itf = MyFB.Interface[InterfaceSections.Static];

            //Define two interface parameters
            InterfaceParameter customParam;

            //Create new parameters. The custom parameter means you are using your own user defined data type.

            customParam = new InterfaceParameter("MyMultiInstanceDbName", "\"" + "MAC_use_casesFB" + "\"") { Remanence = RemanenceSettings.IgnoreRemanence };

            //Add your parameters 
            itf.Add(customParam);

            BlockNetwork Network_Multiinstance = new BlockNetwork();
            Network_Multiinstance.Blocks.Add(myMultiInstanceFBCall);
            Network_Multiinstance.GenerationLabel = new GenerationLabel(module.Name, "Generated by MAC2", module.ModuleID.ToString());
            MyFB.Networks.Add(Network_Multiinstance);
            AddCodeBlockToOB(MyFB, module.ResourceManagement,languagesettings, plcDevice);
        }

        /// <summary>
        /// This function generates an OB and creates an block call in FBD and SCL
        /// \image html GenerateOB.png
        /// </summary>
        /// <param name="instanceDbName">The name of the generated OB</param>
        /// <param name="module">The Module</param>
        /// <param name="tiaTemplateContext">The tiaTemplateContext</param>
        /// <param name="plcDevice">The plcDevice</param>
        public static void GenerateOB_Main(string instanceDbName, MAC_use_casesEM module, string languagesettings, PlcDevice plcDevice)
        {
            var Main = new XmlOB("Main");
            Main.BlockAttributes.ProgrammingLanguage = ProgrammingLanguage.LAD;
            ((OB_BlockAttributes)Main.BlockAttributes).BlockSecondaryType = "ProgramCycle";

            var itf = Main.Interface[InterfaceSections.Temp];
            var temp    = new InterfaceParameter("Temp", "Bool");
            var tempint = new InterfaceParameter("TempInt", "Int");
            itf.Add(temp);
            itf.Add(tempint);

            //create block call of instance DB 
            BlockCall myFBCall = new BlockCall(instanceDbName, plcDevice)
            {
                ["Input1"] = "#Temp", // inputVariable1
                ["Input2"] = "0", // inputVariable1
                //["Input2"] = "inputVariable1", 

                //["Output1"] = "outputVariable1", 
            };
            BlockNetwork Network = new BlockNetwork();
            Network.Blocks.Add(myFBCall);
            Network.GenerationLabel = new GenerationLabel(module.Name, "Generated by MAC", module.ModuleID.ToString());
            Main.Networks.Add(Network);

            ///Create a SCL-network with IF and ELSE Statements
            var content = "\"myDB\".myParameterName := 100;\n";
            content += "IF #Temp THEN\n";
            content += "#TempInt := 5;\n";
            content += "ELSE\n";
            content += "TempInt := 1;\n";
            content += "END_IF;\n\n";
            string sclContent = content;
            var sclcode_network = parseSingleSCLCall(sclContent, plcDevice, Main);
            sclcode_network.GenerationLabel = new GenerationLabel(module.Name, "Virtual_Master_scl_code", module.ModuleID.ToString());
            Main.Networks.Add(sclcode_network);

            AddCodeBlockToOB(Main, module.ResourceManagement, languagesettings, plcDevice);
        }

        /// <summary>
        /// This function checks if OB is already in TIA Project and merge the new one into the existing one depending to the language.
        /// </summary>
        /// <param name="codeBlock">The new OB</param>
        /// <param name="resourceManagement">The resourceManagement</param>
        /// <param name="tiaTemplateContext">The tiaTemplateContext</param>
        /// <param name="plcDevice">The plcDevice</param>
        public static void AddCodeBlockToOB(XmlLogicalBlock codeBlock, ResourceManagement resourceManagement, string languagesettings, PlcDevice plcDevice)
        {
            var existingOB = OpennessFuncs.TryGetOBFromPlcDevice(plcDevice, codeBlock.Name);
            //Create new OB in SCL with calling code
            if (existingOB == null)
            {
                if (codeBlock.BlockAttributes.ProgrammingLanguage == ProgrammingLanguage.SCL)
                {
                    codeBlock.GenerateSclBlock(plcDevice);
                }
                else //insert networks to KOP/FUP
                {
                    codeBlock.GenerateXmlBlock(plcDevice, null, languagesettings);
                }
            }
            else
            //Insert Code to existing OB
            {
                var blockAsDoc = OpennessFuncs.ExportBlockAsXml(existingOB.Name, plcDevice, OpennessFuncs.CachingOptions.DISABLED);
                Parser parser = new Parser();
                var xmlOB = parser.ParseXml(blockAsDoc);
                xmlOB.MergeBlock(codeBlock);
                //add SCL-Code
                if (xmlOB.BlockAttributes.ProgrammingLanguage == ProgrammingLanguage.SCL)
                {
                    xmlOB.GenerateSclBlock(plcDevice);
                }
                else //insert networks to KOP/FUP
                {
                    xmlOB.GenerateXmlBlock(plcDevice, null, languagesettings);
                }
            }
        }

        /// <summary>
        /// This function Generates the OB with the given number of calls
        /// </summary>
        /// <param name="numberOfCalls">The number of calls in the OB</param>
        /// <param name="tiaTemplateContext">The tiaTemplateContext</param>
        // Hauptfunktion, die alle oben definierten Funktionen aufruft
        public static void GenerateMainOBWithMultipleCalls(string name, int numberOfCalls, string languagesettings, PlcDevice plcDevice, MAC_use_casesEM Module)
        {
            var myOB = new XmlOB(name);
            myOB.BlockAttributes.BlockAutoNumber = false;
            myOB.BlockAttributes.BlockNumber = 555.ToString();

            for (int i = 1; i <= numberOfCalls; i++)
            {
                IntegrateLibraries.CreateInstanceDB(Module, Module.ResourceManagement.MAC_use_casesFB, "CreatedDbFromMasterCopy" + i.ToString(), Module.ResourceManagement.ModuleBlocksRootGroup);
                var blockname = "CreatedDbFromMasterCopy" + i.ToString();
                var myOBCall = new BlockCall(blockname, plcDevice)
                {
                    ["Input1"] = "1", // inputVariable1
                    ["Input2"] = "0", // inputVariable1
                };

                var myObBlockNetwork = new BlockNetwork();
                myObBlockNetwork.Blocks.Add(myOBCall);
                myObBlockNetwork.GenerationLabel = new GenerationLabel(blockname, "Generated by MAC. Blockname: \"" + blockname+ "\"", Module.ModuleID.ToString());
                myOB.Networks.Add(myObBlockNetwork);
            }

            AddCodeBlockToOB(myOB, Module.ResourceManagement, languagesettings, plcDevice);
        }

        private static INetwork parseSingleSCLCall(string sclCall, PlcDevice plc, XmlBlock xmlBlock)
        {
            //var parsed = new Parser().ParseSclSnippet(sclCall, plc, Siemens.Automation.ModularApplicationCreator.Tia.Helper.Create_XML_Block.BlockType.OB, GroupBlockCalls.NOGROUPING).FirstOrDefault();
            var parsed = new Parser().ParseSclSnippet(sclCall, xmlBlock, plc, GroupBlockCalls.NOGROUPING).FirstOrDefault();
            if (parsed is BlockNetwork blockNetwork)
            {
                return blockNetwork;
            }
            else if (parsed is FixNetwork fixNetwork)
            {
                return fixNetwork;
            }
            return null;
        }

    }
}
