using System.IO;
using System.Reflection;

using NUnit.Framework;

using MAC_use_cases.Model;

using Siemens.Engineering;
using Siemens.ModularApplicationCreator.Testenvironment;

namespace MAC_use_cases.Tests.TestEnvironment
{
    public class MacGenerationTestBaseMAC_use_cases : MacGenerationTestBase<MAC_use_casesEM>
    {
        [TearDown]
        public override void Teardown()
        {
        }
        protected override void InitializeModuleClassesForTesting()
        {
        }

        protected override string DirectoryNameOfViewTestResources =>
            this.Get(() => "MAC_use_cases.Tests.Resources");

protected override string NameOfTiaZapFileFromResources => this.Get(() => "MacTestenvironmentBaseProject.zap20");
        protected override Assembly Assembly => this.Get(() => typeof(MacGenerationTestBaseMAC_use_cases).Assembly);

        protected override string PlcName => this.Get(() => "S7-1500/ET200MP station_1");
        protected override string PlcCPUName => this.Get(() => "TestenvironmentPLC");
        protected override string OrderNumberPLC => this.Get(() => "");


        protected void OpenProjectInNewTiaPortalWithUi()
        {
            var projectPath = new FileInfo(ExistingProjectTemplate.ProjectPath);

            var newTiaPortal = new TiaPortal(TiaPortalMode.WithUserInterface);
            var project = newTiaPortal.Projects.Open(projectPath);
        }
    }
}
