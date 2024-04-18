using Newtonsoft.Json;
using Siemens.Automation.ModularApplicationCreator.Core;
using Siemens.Automation.ModularApplicationCreator.Modules.UI;
using Siemens.Automation.ModularApplicationCreator.Tia;
using Siemens.Automation.ModularApplicationCreator.Tia.Helper;
using Siemens.Automation.ModularApplicationCreator.Tia.Modules;
using Siemens.Automation.ModularApplicationCreator.Tia.Openness;
using Siemens.Automation.ModularApplicationCreator.Tia.TiaAttributeFuncs;
using Siemens.Automation.ModularApplicationCreatorBasics.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using UseCaseBasedDoku.TiaImports;
using UseCaseBasedDoku.UI;

namespace UseCaseBasedDoku.Model
{
    public abstract partial class BaseUseCaseBasedDokuEM : TiaEquipmentModule, ITiaAttributeUser
    {

        private EquipmentModuleEditorProvider _editorProvider;

        [JsonIgnore] public List<TiaAttribute> Attributes { get; } = new List<TiaAttribute>();

        [JsonIgnore] public ResourceManagement ResourceManagement { get; private set; } = new ResourceManagement();

        protected BaseUseCaseBasedDokuEM()
        {
            // Add language resources
            string resourceName = typeof(BaseUseCaseBasedDokuEM).Assembly.ManifestModule.Name;
            resourceName = System.IO.Path.GetFileNameWithoutExtension(resourceName);
            ResourceDictionary dictionary_en = (ResourceDictionary)Application.LoadComponent(
                new Uri(@"/" + resourceName + ";;;component/Resources/Lang_en.xaml", UriKind.Relative));
            ResourceDictionary dictionary_de = (ResourceDictionary)Application.LoadComponent(
                new Uri(@"/" + resourceName + ";;;component/Resources/Lang_de.xaml", UriKind.Relative));
            ResourceDictionary dictionary_zh = (ResourceDictionary)Application.LoadComponent(
                new Uri(@"/" + resourceName + ";;;component/Resources/Lang_zh.xaml", UriKind.Relative));

            LanguageResources.Add("en", dictionary_en);
            LanguageResources.Add("de", dictionary_de);
            LanguageResources.Add("zh", dictionary_zh);
            InitOnlineHelp();
        }

        public override IEnumerable<TiaLibraryInfo> GetLibraryInfos()
        {
            List<TiaLibraryInfo> libraryInfos = ResourceManagement.GetLibraryInfos();
            foreach (var controlModule in ControlModules)
            {
                if (controlModule is TiaEquipmentModule tiaModule)
                {
                    libraryInfos.AddRange(tiaModule.GetLibraryInfos());
                }
            }

            return libraryInfos;
        }

        /// <inheritdoc/>
        public override void InitModule()
        {
            base.InitModule();
            RunGeneratedTasksInitModule();
        }

        /// <inheritdoc/>
        public override void AfterEquipmentModuleLoad()
        {
            base.AfterEquipmentModuleLoad();
            RunGeneratedTasksAfterEQMLoad();
        }

        /// <inheritdoc/>
        public override void BeforeModuleDeleted()
        {
            base.BeforeModuleDeleted();
            RunGeneratedBeforeDelete();
        }

        public override EquipmentModuleEditorProvider GetEditorProvider()
        {
            if (_editorProvider == null)
            {
                _editorProvider = new EquipmentModuleEditorProvider(new MultiPageFrame(this));
            }

            return _editorProvider;
        }

        protected string GetContentAdditionalResource(string name)
        {
            System.Reflection.Assembly asm = System.Reflection.Assembly.GetExecutingAssembly();
            string returnString =
                new StreamReader(asm.GetManifestResourceStream("UseCaseBasedDoku.TiaImports.AdditionalResources." + name))
                    .ReadToEnd();

            return returnString;
        }

        /// <summary>
        /// Every logic and setting must be here which is important at the first creation or
        /// deserialization of this object.
        /// </summary>
        protected virtual void InitAfterFirstCreationOrDeserialization()
        {
            RunGeneratedTasksConstructor();
        }

        /// <summary>
        /// Creates Control Modules and adds them to the project.
        /// </summary>
        protected virtual void CreateControlModules()
        {
            /// Example:
            /// YourControlModule = new YourControlModule();
            /// ControlModules.Add(YourControlModule);
        }

        protected PlcDevice GetPlcDevice(TiaTemplateContext tiaTemplateContext)
        {
            if (tiaTemplateContext == null)
            {
                throw new ArgumentNullException(nameof(tiaTemplateContext));
            }

            PlcDevice plcDevice = tiaTemplateContext.TiaDevice as PlcDevice;

            if (plcDevice == null)
            {
                if (tiaTemplateContext.TiaDevice != null)
                {
                    throw new ArgumentException(
                        $"{nameof(TiaTemplateContext.TiaDevice)} must be '{typeof(PlcDevice)}' but it was '{tiaTemplateContext.TiaDevice.GetType()}'");
                }

                throw new ArgumentNullException($"{nameof(TiaTemplateContext.TiaDevice)}");
            }

            if (plcDevice.Name != ParentDevice.Name)
            {
                throw new ArgumentException(
                    $"ParentDevice name: '{ParentDevice.Name}' of this module does not match with the name of the TiaDevice: '{plcDevice.Name}' in {nameof(TiaTemplateContext)}");
            }

            return plcDevice;
        }

        protected void LogGenerationWarning(string message, string source)
        {
            MacManagement.LoggingService.LogMessage(LogTypes.GenerationWarning, message, source);
        }

        /// <summary>
        /// Adds the GSD files to TIA Project in MAC project Generated directory. The GSD files must be added to the project as Content.
        /// </summary>
        /// <param name="gsdFiles">Names of the source GDS file.</param>
        /// <param name="resourceDirPath">The default directory name is Resources.
        /// If the GDS file is located at a different location inside the project then the full path must be added. </param>
        protected void AddGsdFiles(string[] gsdFiles, string resourceDirPath = "Resources")
        {
            string dllDirectory = Path.GetDirectoryName(GetType().Assembly.Location);
            var gsdFileInfos = new List<FileInfo>(gsdFiles.Length);

            foreach (string gsdFile in gsdFiles)
            {
                var f = new FileInfo(Path.Combine(dllDirectory, resourceDirPath, gsdFile));
                gsdFileInfos.Add(f);
            }

            AddGsdFileToTiaProject(gsdFileInfos);
        }
    }
}
