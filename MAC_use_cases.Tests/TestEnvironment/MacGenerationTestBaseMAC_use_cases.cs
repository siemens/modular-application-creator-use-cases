using System.IO;
using System.Reflection;
using MAC_use_cases.Model;
using NUnit.Framework;
using Siemens.Engineering;
using Siemens.ModularApplicationCreator.Testenvironment;

namespace MAC_use_cases.Tests.TestEnvironment
{
    public class MacGenerationTestBaseMAC_use_cases : MacGenerationTestBase<MAC_use_casesEM>
    {
        protected override string DirectoryNameOfViewTestResources
        {
            get => Get(() => "MAC_use_cases.Tests.Resources");
        }

        protected override string NameOfTiaZapFileFromResources
        {
            get => Get(() => "MacTestenvironmentBaseProject.zap19");
        }

        protected override Assembly Assembly
        {
            get => Get(() => typeof(MacGenerationTestBaseMAC_use_cases).Assembly);
        }

        protected override string PlcName
        {
            get => Get(() => "S7-1500/ET200MP station_1");
        }

        protected override string PlcCPUName
        {
            get => Get(() => "TestenvironmentPLC");
        }

        protected override string OrderNumberPLC
        {
            get => Get(() => "");
        }

        [TearDown]
        public override void Teardown()
        {
        }

        protected override void InitializeModuleClassesForTesting()
        {
        }


        protected void OpenProjectInNewTiaPortalWithUi()
        {
            var projectPath = new FileInfo(ExistingProjectTemplate.ProjectPath);

            var newTiaPortal = new TiaPortal(TiaPortalMode.WithUserInterface);
            var project = newTiaPortal.Projects.Open(projectPath);
        }
    }
}
