using Newtonsoft.Json;
using Siemens.Automation.ModularApplicationCreator.Modules;
using Siemens.Automation.ModularApplicationCreator.Serializer;
using Siemens.Automation.ModularApplicationCreator.Tia;
using Siemens.Automation.ModularApplicationCreator.Tia.Helper;
using Siemens.Automation.ModularApplicationCreator.Tia.Modules;
using Siemens.Automation.ModularApplicationCreator.Tia.Openness;
using Siemens.Automation.ModularApplicationCreator.Tia.TiaAttributeFuncs;
using MAC_use_cases.Model.UseCases;
using MAC_use_cases.Model.UseCases.SoftwareUnits;
using Siemens.Automation.ModularApplicationCreator.Tia.Helper.Create_XML_Block.XmlBlocks.BlockFrames;
using Siemens.Automation.ModularApplicationCreatorBasics.Logging;
using Siemens.Automation.ModularApplicationCreator.Core;
using Siemens.Automation.ModularApplicationCreator.Tia.Openness.DO;

namespace MAC_use_cases.Model
{
    /// <summary>
    /// This is the main class in which the workflow starts. Here are all sections for creating or generating the TIA Portal Project
    /// </summary>
    public class MAC_use_casesEM : BaseMAC_use_casesEM
    {
        //-------------------------------------------
        // This ModuleStart property is generated.
        // It must be at the top of the class. It is used during module update.
        //
        // CAUTION: Do not use public instance variables as they overtake the properties in serialization.
        //-------------------------------------------
        public ModuleEdge ModuleStart { get { return TopLevelEM ? new ModuleEdge() : null; } set { } }

        /// <summary>
        /// This attribute is the instance of the plcDevice
        /// </summary>
        private PlcDevice m_plcDevice;

        /// <summary>
        /// This attribute is the instance of the TechnologyObjectClass
        /// </summary>
        public TechnologyObjectClass myTO { get; set; }

        /// <summary>
        /// This attribute is the instance for the serialization
        /// </summary>
        public NonTIAProjectBased NonTiaProjectBased { get; set; } = new NonTIAProjectBased("myModel");

        /// <summary>
        /// This attribute is the string which is used for the renaming of the FB.
        /// This string can be changed in the View 
        /// </summary>
        public string NameOfMyFb { get; set; } = "myFB";

        /// <summary>
        /// This attribute is the string which is used for the renaming of the safety-FB.
        /// This string can be changed in the View 
        /// </summary>
        public string SafetyFb { get; set; } = "SafetyFB";

        [JsonIgnore]
        public string NameAndType => NamingConventions.CreateModuleNameAndTypeForEM("MAC_use_cases", this);

        // <auto-generated>
        //     This code was generated by a tool. Do not remove or modify the syntax of it.
        //     This constructor is used at first creation of this object.
        // </auto-generated>
        public MAC_use_casesEM()
        {
            // This Function can be modified to accommodate the Control Modules used in this Equipment Module
            CreateControlModules();
            // Add any code here which is needed to be used only when this object is created the first time.

            InitAfterFirstCreationOrDeserialization();

            myTO = new TechnologyObjectClass(this);
        }

        // <auto-generated>
        //     This code was generated by a tool. Do not remove or modify the syntax of it.
        //     This constructor is used only for deserialization.
        // </auto-generated>
        [JsonConstructor]
        public MAC_use_casesEM(JsonConstructorMarker nullObj)
        {
            // Add any code here which is only necessary when this object is deserialized.

            InitAfterFirstCreationOrDeserialization();

            myTO = new TechnologyObjectClass(this);
        }

        public override bool GenerateTiaPortal(TiaTemplateContext tiaTemplateContext, string generationPhaseName)
        {
            switch (generationPhaseName)
            {
                case TiaGenerationPhases.Init:
                    m_plcDevice = GetPlcDevice(tiaTemplateContext);
                    break;

                    // Add equipment module specific code for each generation phase here, which is
                    // necessary before the ResourceManagement is called.
            }

            ResourceManagement.Generate(tiaTemplateContext, this, generationPhaseName);

            foreach (var module in ControlModules)
            {
                if (module is ITiaGenerateable tiaGen)
                {
                    tiaGen.GenerateTiaPortal(tiaTemplateContext, generationPhaseName);
                }
            }

            switch (generationPhaseName)
            {
                case TiaGenerationPhases.Init:

                    //Hardware config has to be in the Init phase. Otherwise it can't be used in the MAC.
                    var s120 = HardwareGeneration.GenerateS120(this, "S120MACTest", "S120DeviceTest", "this drive is generated with MAC");

                    var s210 = HardwareGeneration.GenerateS210(this, "S210MACTest", "S210DeviceTest", "this drive is generated with MAC");
                    var subnet1 = HardwareGeneration.GetOrCreateSubnet(SubnetsManager, "PN/IE_1");

                    HardwareGeneration.ConnectDriveToSubnet(s120, subnet1, this);
                    HardwareGeneration.ConnectDriveToSubnet(s210, subnet1, this);

                    break;
                case TiaGenerationPhases.Build:

                    // Add equipment module specific code generation here.

                    var opennessTIAPortalProject = GeneralSupport.GetOpennessProject(tiaTemplateContext.TiaProject);
                    var opennessCPU = GeneralSupport.GetOpennessDeviceItem(tiaTemplateContext.TiaDevice);

                    myTO.ConfigureTO(myTO.TechnologicalObject, this);

                    GeneralSupport.LogMessage(LogTypes.GenerationInfo, "Generate technology objects", this);
                    TechnologyObjectClass.CreateTOs(m_plcDevice, this);

                    IntegrateLibraries.CreateInstanceDB(this, this.ResourceManagement.MAC_use_casesFB, "CreatedDbFromMasterCopy", this.ResourceManagement.ModuleBlocksRootGroup);

                    IntegrateLibraries.CreateInstanceDB_via_XmlInstDB(this, this.ResourceManagement.MAC_use_casesFB, "CreatedDbFromMasterCopy_XmlInstDB", this.ResourceManagement.ModuleBlocksRootGroup, m_plcDevice);

                    GenericBlockCreation.GenerateDB("myDB", m_plcDevice, this);

                    GenericBlockCreation.SetDefaultValue("myDB", "myParameterName", TIATYPE.INT, "99", this);

                    GenericBlockCreation.GenerateMultiInstanceFB(m_plcDevice, tiaTemplateContext.TiaProject.GetEditingLanguage(), this);

                    GenericBlockCreation.GenerateOB_Main("CreatedDbFromMasterCopy", this, tiaTemplateContext.TiaProject.GetEditingLanguage(), m_plcDevice);

                    GenericBlockCreation.GenerateMainOBWithMultipleCalls("myOB", 10, tiaTemplateContext.TiaProject.GetEditingLanguage(), m_plcDevice, this);

                    GenericBlockCreation.CreateFB(NameOfMyFb, "CreatedDbFromMasterCopy", m_plcDevice);

                    var myTagTable = CreateVariables.CreateTagTable(m_plcDevice, "myTagTable");

                    CreateVariables.CreateTagInTagTable(myTagTable, "%I", "187", "0", "myTag", "Bool", "myTagComment");

                    var hmiSoftware = HardwareGeneration.GetOrCreateHMISoftware(opennessTIAPortalProject, "HMI_1");
                    IntegrateLibraries.GenerateScreenFromMastercopy(hmiSoftware, ResourceManagement.Lib_MAC_use_cases.Lib_Screen_1);

                    provider.CollectAttributes(Attributes);
                    provider.WriteValues(m_plcDevice);
                    break;
            }

            return true;
        }
    }




    /*! \mainpage Overview
     *
     * \section classes_sec Classes
     *
     * The "Classes" section explains the classes used in the project.
     * In addition to the explanation of the class and a first overview of all functions used in the class, the page contains a detailed explanation of all functions of the class.
     *
     * \image html ClassExplanation.png
     *
     *
     * 
     * In all explanatory sections there is a short literal explanation of the function and its parameters, as well as a picture of the resulting result in the Tia Portal after generation.
     * Also included is the used code of the function and a link to the code of the whole class.
     *
     * \image html FunctionExplanation.png
     *
     *
     * 
     * \section files_sec Files
     *
     * The above-mentioned link to the class code then points to the files contained in the "Files" section.
     * There, all classes in the C# code are included again to show how the classes look as simple as possible.
     * It is also possible to copy the code to reuse it in your own modules.
     *
     * \image html CodeExplanation.png
     *
     *
     * 
     * \section install_sec Integrate Libraries
     *
     * With the help of the .tiares file, libraries can be easily integrated in the Module Builder. To do this, the file must first be opened in Visual Studio.
     *
     * \image html OpenTiares.png
     *
     * In the newly opened window, the selection menu can now be opened using the button in the top left-hand corner.
     * The desired library must then be selected there.
     *
     * \image html OpenLibrary.png
     *
     * By clicking on the "Mastercopies" area and then pressing the "Add" button, everything is included. This window can then be closed again, as everything is saved automatically.
     *
     * \image html AddLibrary.png
     *
     */
}
