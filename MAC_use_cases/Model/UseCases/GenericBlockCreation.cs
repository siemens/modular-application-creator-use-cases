using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using MAC_use_cases.TiaImports;
using Siemens.Automation.ModularApplicationCreator.Tia.Helper;
using Siemens.Automation.ModularApplicationCreator.Tia.Helper.Create_XML_Block;
using Siemens.Automation.ModularApplicationCreator.Tia.Helper.Create_XML_Block.XmlBlocks.BlockFrames;
using Siemens.Automation.ModularApplicationCreator.Tia.Openness;
using Siemens.Automation.ModularApplicationCreator.Tia.Openness.SoftwareUnit;
using Siemens.Automation.ModularApplicationCreator.Tia.TiaAttributeFuncs;
using ProgrammingLanguage =
    Siemens.Automation.ModularApplicationCreator.Tia.Helper.Create_XML_Block.ProgrammingLanguage;

namespace MAC_use_cases.Model.UseCases;

/// <summary>
///     All the functions to configure and generate Blocks are defined here.
/// </summary>
public class GenericBlockCreation
{
    /// <summary>
    ///     This is the name of the generated parameter
    /// </summary>
    public static string ParameterName = "myParameterName";

    /// <summary>
    ///     Creates a Function Block (FB) in the target PLC's program blocks folder.
    ///     \image html CreateFB.png
    /// </summary>
    /// <remarks>
    ///     This method performs the following steps:
    ///     <list type="number">
    ///         <item>Creates a block call with default input values (Input1=1, Input2=0)</item>
    ///         <item>Sets up a network with titles and the block call</item>
    ///         <item>Creates a standard FB</item>
    ///         <item>Configures the FB with network, attributes, comments, and titles</item>
    ///         <item>Generates the final block in either XML or SCL format</item>
    ///     </list>
    /// </remarks>
    /// <param name="blockName">Name of the FB to create</param>
    /// <param name="instanceDbName">Name of the instance data block</param>
    /// <param name="programmingLanguage">Programming language for the FB (supports LAD, FBD, SCL)</param>
    /// <param name="plcDevice">Target PLC device where the FB will be created</param>
    /// <exception cref="ArgumentNullException">Thrown when blockName, instanceDbName, or plcDevice is null</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when programming language is not supported</exception>
    /// <example>
    ///     Creating a standard LAD function block:
    ///     <code>
    /// CreateFB("MyFB", "MyFB_DB", ProgrammingLanguage.LAD, plcDevice);
    /// </code>
    /// </example>
    // TODO expand for sw-units
    public static void CreateFunctionBlock(string blockName, string instanceDbName,
        ProgrammingLanguage programmingLanguage,
        PlcDevice plcDevice)
    {
        // Input validation
        if (string.IsNullOrEmpty(blockName))
        {
            throw new ArgumentNullException(nameof(blockName));
        }

        if (string.IsNullOrEmpty(instanceDbName))
        {
            throw new ArgumentNullException(nameof(instanceDbName));
        }

        if (plcDevice == null)
        {
            throw new ArgumentNullException(nameof(plcDevice));
        }

        if (programmingLanguage != ProgrammingLanguage.LAD && programmingLanguage != ProgrammingLanguage.FBD &&
            programmingLanguage != ProgrammingLanguage.SCL)
        {
            throw new ArgumentException(nameof(programmingLanguage));
        }

        // Create block call with default values
        var fbCall = new BlockCall(instanceDbName, plcDevice) { ["Input1"] = "1", ["Input2"] = "0" };

        // Create and configure network
        var fbNetwork = new BlockNetwork { NetworkTitles = { [TypeMapper.BaseCulture.Name] = "myFB Network Title" } };
        fbNetwork.Blocks.Add(fbCall);

        // Create and Configure FB properties
        var fb = new XmlFB(blockName);
        fb.Networks.Add(fbNetwork);
        fb.BlockAttributes.ProgrammingLanguage = programmingLanguage;
        fb.BlockComments[TypeMapper.BaseCulture.Name] = "myFB Block Comment";
        fb.BlockTitles[TypeMapper.BaseCulture.Name] = "myFB Block Title";

        // Generate the block in appropriate format
        if (programmingLanguage == ProgrammingLanguage.SCL)
        {
            fb.GenerateSclBlock(plcDevice);
        }
        else
        {
            fb.GenerateXmlBlock(plcDevice);
        }
    }

    /// <summary>
    ///     Creates a Fail-Safe Function Block (F-FB) in the target PLC's program blocks folder.
    /// </summary>
    /// <remarks>
    ///     This method performs the following steps:
    ///     <list type="number">
    ///         <item>Creates a block call with default input values (Input1=1, Input2=0)</item>
    ///         <item>Sets up a network with titles and the block call</item>
    ///         <item>Creates a fail-safe FB</item>
    ///         <item>Configures the F-FB with network, attributes, comments, and titles</item>
    ///         <item>Generates the final block in XML format</item>
    ///     </list>
    /// </remarks>
    /// <param name="blockName">Name of the F-FB to create</param>
    /// <param name="instanceDbName">Name of the instance data block</param>
    /// <param name="programmingLanguage">Programming language for the F-FB (supports F-LAD, F-FBD)</param>
    /// <param name="plcDevice">Target PLC device where the F-FB will be created</param>
    /// <exception cref="ArgumentNullException">Thrown when blockName, instanceDbName, or plcDevice is null</exception>
    /// <exception cref="ArgumentException">Thrown when programming language is not F-LAD or F-FBD</exception>
    /// <example>
    ///     Creating a fail-safe LAD function block:
    ///     <code>
    /// CreateFailSafeFunctionBlock("MyFFB", "MyFFB_DB", ProgrammingLanguage.F_LAD, plcDevice);
    /// </code>
    /// </example>
    public static void CreateFailSafeFunctionBlock(string blockName, string instanceDbName,
        ProgrammingLanguage programmingLanguage,
        PlcDevice plcDevice)
    {
        // Input validation
        if (string.IsNullOrEmpty(blockName))
        {
            throw new ArgumentNullException(nameof(blockName));
        }

        if (string.IsNullOrEmpty(instanceDbName))
        {
            throw new ArgumentNullException(nameof(instanceDbName));
        }

        if (plcDevice == null)
        {
            throw new ArgumentNullException(nameof(plcDevice));
        }

        if (programmingLanguage != ProgrammingLanguage.F_LAD && programmingLanguage != ProgrammingLanguage.F_FBD)
        {
            throw new ArgumentException(nameof(programmingLanguage));
        }

        // Create block call with default values
        var fbCall = new BlockCall(instanceDbName, plcDevice) { ["Input1"] = "1", ["Input2"] = "0" };

        // Create and configure network
        var fbNetwork = new BlockNetwork { NetworkTitles = { [TypeMapper.BaseCulture.Name] = "myFB Network Title" } };
        fbNetwork.Blocks.Add(fbCall);

        //Create and configure FB properties
        var failSafeBlock = new XmlFailSafeFB(blockName);
        failSafeBlock.Networks.Add(fbNetwork);
        failSafeBlock.BlockAttributes.ProgrammingLanguage = programmingLanguage;
        failSafeBlock.BlockComments[TypeMapper.BaseCulture.Name] = "myFB Block Comment";
        failSafeBlock.BlockTitles[TypeMapper.BaseCulture.Name] = "myFB Block Title";

        // Generate the block in appropriate format
        failSafeBlock.GenerateXmlBlock(plcDevice, programmingLanguage);
    }


    /// <summary>
    ///     This function creates a Data Block with interface (sub-)parameters in a desired folder. The variables in the
    ///     interface can have a standard data types or a data type defined in the project.
    ///     \image html GenerateDB.png
    /// </summary>
    /// <param name="dbName"></param>
    /// <param name="plcDevice">The PLC on which the equipment module is implemented</param>
    /// <param name="module">The corresponding equipment module</param>
    public static XmlGlobalDB GenerateGlobalDataBlock(string dbName, PlcDevice plcDevice, MAC_use_casesEM module)
    {
        var dataBlock = new XmlGlobalDB(dbName);
        var itf = dataBlock.Interface[InterfaceSections.Static];

        //Create new parameters. The custom parameter means you are using your own user defined data type.
        var standardParam = new InterfaceParameter(ParameterName, "Int");
        var customParam1 = new InterfaceParameter("var_struct", "Struct");
        customParam1.SubParameter.Add(new InterfaceParameter("Var1", "Bool"));
        customParam1.SubParameter.Add(new InterfaceParameter("Var2", "Int"));
        var customParam2 = new InterfaceParameter("datatyp_from_lib", "\"" + "myDataType" + "\"");

        //Add your parameters 
        itf.Add(standardParam);
        itf.Add(customParam1);
        itf.Add(customParam2);

        dataBlock.GenerateXmlBlock(plcDevice);
        return dataBlock;
    }

    /// <summary>
    ///     Creates a new Function Block (FB) within the specified software unit with predefined interface parameters.
    ///     The FB will contain two boolean inputs and one boolean output, all with remanence ignored.
    /// </summary>
    /// <param name="softwareUnitBase">The base software unit where the Function Block will be created</param>
    /// <param name="fbName">The name for the new Function Block</param>
    /// <param name="programmingLanguage">The programming language to be used for the Function Block</param>
    /// <remarks>
    ///     The created Function Block will have the following interface:
    ///     - Inputs:
    ///     - Input_1 (Bool)
    ///     - Input_2 (Bool)
    ///     - Output:
    ///     - Output (Bool)
    ///     All parameters are configured with IgnoreRemanence setting.
    /// </remarks>
    /// <example>
    ///     <code>
    /// CreateFunctionBlockInSoftwareUnit(mySoftwareUnit, "MyNewFB", ProgrammingLanguage.LAD);
    /// </code>
    /// </example>
    public static void CreateFunctionBlockInSoftwareUnit(ISoftwareUnitBase softwareUnitBase, string fbName,
        ProgrammingLanguage programmingLanguage, PlcDevice plcDevice)
    {
        var fb = new XmlFB(fbName);

        fb.Interface[InterfaceSections.Input].Add(new InterfaceParameter("Input_1", "Bool")
        {
            Remanence = RemanenceSettings.IgnoreRemanence
        });
        fb.Interface[InterfaceSections.Input].Add(new InterfaceParameter("Input_2", "Bool")
        {
            Remanence = RemanenceSettings.IgnoreRemanence
        });
        fb.Interface[InterfaceSections.Output].Add(new InterfaceParameter("Output", "Bool")
        {
            Remanence = RemanenceSettings.IgnoreRemanence
        });

        //here an example how to create a multi instance call in the function block
        if (programmingLanguage == ProgrammingLanguage.SCL)
        {
            fb.GenerateSclBlock(softwareUnitBase);

        }
        else
        {
            fb.GenerateXmlBlock(softwareUnitBase, programmingLanguage);
        }
        var block = softwareUnitBase.Blocks.ToList().FirstOrDefault(b => b.Value.Name == fbName);
        block.Value.Namespace = "test";

        //softwareUnitBase.NamespacePreset = "BlockNamespace";

        //softwareUnitBase.SetNamespacePresetOfBlocksAndTypes();
    }

    /// <summary>
    ///     This function creates a Block on the targetDevice in the targetContainer (folder under program blocks)
    /// </summary>
    /// <param name="code">The Block code as .scl or .xml </param>
    /// <param name="targetDevice">The PLC on which the equipment module is implemented</param>
    /// <param name="targetContainer">The corresponding equipment module</param>
    /// <param name="isScl">true: code string represents a scl block</param>
    private static void GenerateBlock(string code, PlcDevice targetDevice, ITarget targetContainer = null,
        bool isScl = true)
    {
        //Get the right file extension depending on the block language
        var extension = ".scl";
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
    ///     This function sets the default value of a parameter in a DB
    ///     \image html SetDefaultValue.png
    /// </summary>
    /// <param name="dbName">The DB in which the value should be set</param>
    /// <param name="parameterName">The name of the parameter</param>
    /// <param name="dataType">DataType of the parameter</param>
    /// <param name="value">Value that should be set</param>
    /// <param name="parent">The module</param>
    public static void SetDefaultValue(string dbName, string parameterName, TIATYPE dataType, string value,
        MAC_use_casesEM parent)
    {
        var myParameterNameAttribute = TiaAttributeProvider.CreateAttribute(dataType, SOURCETYPE.BLOCKVAR,
            dbName, parameterName, parent);
        myParameterNameAttribute.Value = value;
    }

    /// <summary>
    ///     This function creates an multiInstanceFB
    ///     \image html MultiInstanceFB.png
    /// </summary>
    /// <param name="plcDevice">The plcDevice</param>
    /// <param name="languageSettings">The tiaTemplateContext language key</param>
    /// <param name="module">The Module</param>
    public static void GenerateMultiInstanceFB(PlcDevice plcDevice, string languageSettings, MAC_use_casesEM module)
    {
        var myFb = new XmlFB("MyFunctionBlock_MultiInstance")
        {
            BlockAttributes = { ProgrammingLanguage = ProgrammingLanguage.LAD }
        };

        var multiInstanceDataBlockName = myFb.Name + "_Instance";
        var myMultiInstanceFbCall =
            new MultiInstanceCall(multiInstanceDataBlockName, module.ResourceManagement.MyFunctionBlock.Name, plcDevice)
            {
                ["Input1"] = "1", // inputVariable1
                ["Input2"] = "0" // inputVariable1
            };

        //manipulate interface
        var interfaceParameters = myFb.Interface[InterfaceSections.Static];

        //Create new parameters. The custom parameter means you are using your own user defined data type.
        var customParam =
            new InterfaceParameter(multiInstanceDataBlockName,
                module.ResourceManagement.MyFunctionBlock.Name) { Remanence = RemanenceSettings.IgnoreRemanence };

        //Add your parameters 
        interfaceParameters.Add(customParam);

        var networkMultiInstance = new BlockNetwork();
        networkMultiInstance.Blocks.Add(myMultiInstanceFbCall);
        networkMultiInstance.GenerationLabel =
            new GenerationLabel(module.Name, "Generated by MAC", module.ModuleID.ToString());
        myFb.Networks.Add(networkMultiInstance);
        AddCodeBlockToOB(myFb, module.ResourceManagement, languageSettings, plcDevice);
    }

    /// <summary>
    ///     This function generates an OB and creates a block call in FBD and SCL
    ///     \image html GenerateOB.png
    /// </summary>
    /// <param name="instanceDbName">The name of the generated OB</param>
    /// <param name="module">The Module</param>
    /// <param name="languageSettings">The tiaTemplateContext language key</param>
    /// <param name="plcDevice">The plcDevice</param>
    public static void GenerateOB_Main(string instanceDbName, MAC_use_casesEM module, string languageSettings,
        PlcDevice plcDevice)
    {
        var mainOb = new XmlOB("Main");
        mainOb.BlockAttributes.ProgrammingLanguage = ProgrammingLanguage.LAD;
        ((OB_BlockAttributes)mainOb.BlockAttributes).BlockSecondaryType = "ProgramCycle";

        var itf = mainOb.Interface[InterfaceSections.Temp];
        var temp = new InterfaceParameter("Temp", "Bool");
        var tempInt = new InterfaceParameter("TempInt", "Int");
        itf.Add(temp);
        itf.Add(tempInt);

        //create block call of instance DB 
        var myFBCall = new BlockCall(instanceDbName, plcDevice)
        {
            ["Input1"] = "#Temp", // inputVariable1
            ["Input2"] = "0" // inputVariable1
            //["Input2"] = "inputVariable1", 

            //["Output1"] = "outputVariable1", 
        };
        var blockNetwork = new BlockNetwork();
        blockNetwork.Blocks.Add(myFBCall);
        blockNetwork.GenerationLabel =
            new GenerationLabel(module.Name, "Generated by MAC", module.ModuleID.ToString());
        mainOb.Networks.Add(blockNetwork);

        ///Create a SCL-network with IF and ELSE Statements
        var content = "\"MyDataBlock_Global\".myParameterName := 100;\n";
        content += "IF #Temp THEN\n";
        content += "#TempInt := 5;\n";
        content += "ELSE\n";
        content += "TempInt := 1;\n";
        content += "END_IF;\n\n";
        var sclContent = content;
        var sclcode_network = ParseSingleSCLCall(sclContent, plcDevice, mainOb);
        sclcode_network.GenerationLabel =
            new GenerationLabel(module.Name, "Virtual_Master_scl_code", module.ModuleID.ToString());
        mainOb.Networks.Add(sclcode_network);

        AddCodeBlockToOB(mainOb, module.ResourceManagement, languageSettings, plcDevice);
    }

    /// <summary>
    ///     This function checks if OB is already in TIA Project and merge the new one into the existing one depending on the
    ///     language.
    /// </summary>
    /// <param name="codeBlock">The new OB</param>
    /// <param name="resourceManagement">The resourceManagement</param>
    /// <param name="languageSettings">The tiaTemplateContext language key</param>
    /// <param name="plcDevice">The plcDevice</param>
    public static void AddCodeBlockToOB(XmlLogicalBlock codeBlock, ResourceManagement resourceManagement,
        string languageSettings, PlcDevice plcDevice)
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
                codeBlock.GenerateXmlBlock(plcDevice, null, languageSettings);
            }
        }
        else
            //Insert Code to existing OB
        {
            var blockAsDoc =
                OpennessFuncs.ExportBlockAsXml(existingOB.Name, plcDevice, OpennessFuncs.CachingOptions.DISABLED);
            var parser = new Parser();
            var xmlOB = parser.ParseXml(blockAsDoc);
            xmlOB.MergeBlock(codeBlock);
            //add SCL-Code
            if (xmlOB.BlockAttributes.ProgrammingLanguage == ProgrammingLanguage.SCL)
            {
                xmlOB.GenerateSclBlock(plcDevice);
            }
            else //insert networks to KOP/FUP
            {
                xmlOB.GenerateXmlBlock(plcDevice, null, languageSettings);
            }
        }
    }

    /// <summary>
    ///     This function Generates the OB with the given number of calls
    ///     \image html DynamicCall.png
    /// </summary>
    /// <param name="name"></param>
    /// <param name="numberOfCalls">The number of calls in the OB</param>
    /// <param name="languageSettings">The tiaTemplateContext language key</param>
    /// <param name="plcDevice"></param>
    /// <param name="module"></param>
    public static void GenerateOBWithMultipleCalls(string name, int numberOfCalls, string languageSettings,
        PlcDevice plcDevice, MAC_use_casesEM module)
    {
        var myOb = new XmlOB(name);
        myOb.BlockAttributes.BlockAutoNumber = false;
        myOb.BlockAttributes.BlockNumber = 555.ToString();

        for (var i = 1; i <= numberOfCalls; i++)
        {
            var blockName = $"{nameof(module.ResourceManagement.MyFunctionBlock)}DB" + i;
            IntegrateLibraries.CreateInstanceDataBlock(module, module.ResourceManagement.MyFunctionBlock,
                blockName, module.ResourceManagement.ModuleBlocksRootGroup);
            var myOBCall = new BlockCall(blockName, plcDevice)
            {
                ["Input1"] = "1", // inputVariable1
                ["Input2"] = "0" // inputVariable1
            };

            var myObBlockNetwork = new BlockNetwork();
            myObBlockNetwork.Blocks.Add(myOBCall);
            myObBlockNetwork.GenerationLabel = new GenerationLabel(blockName,
                "Generated by MAC. Blockname: \"" + blockName + "\"", module.ModuleID.ToString());
            myOb.Networks.Add(myObBlockNetwork);
        }

        AddCodeBlockToOB(myOb, module.ResourceManagement, languageSettings, plcDevice);
    }

    private static INetwork ParseSingleSCLCall(string sclCall, PlcDevice plc, XmlBlock xmlBlock)
    {
        var parsed = new Parser().ParseSclSnippet(sclCall, xmlBlock, plc, GroupBlockCalls.NOGROUPING)
            .FirstOrDefault();
        if (parsed is BlockNetwork blockNetwork)
        {
            return blockNetwork;
        }

        if (parsed is FixNetwork fixNetwork)
        {
            return fixNetwork;
        }

        return null;
    }
}
