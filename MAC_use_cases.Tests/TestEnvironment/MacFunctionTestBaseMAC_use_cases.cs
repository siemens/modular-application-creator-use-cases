using System.Reflection;

using NUnit.Framework;

using MAC_use_cases.Model;
using MAC_use_cases.TiaImports;

using Siemens.ModularApplicationCreator.Testenvironment;
using Siemens.ModularApplicationCreator.Testenvironment.UnitTestBaseClasses.Enums;



namespace MAC_use_cases.Tests.TestEnvironment
{
    public class MacFunctionTestBaseMAC_use_cases : MacFunctionTestBase<MAC_use_casesEM>
    {
        [TearDown]
        public override void Teardown()
        {
        }

protected ResourceManagement ResourceManagement => this.Get(() => new ResourceManagement(), NunitTestContext.TestCase);

        protected override void InitializeModuleClassesForTesting()
        {
        }


        protected override string DirectoryNameOfViewTestResources =>
            this.Get(() => "MAC_use_cases.Tests.Resources");

protected override string NameOfTiaZapFileFromResources => this.Get(() => "MacTestenvironmentBaseProject.zap20");
        protected override Assembly Assembly => this.Get(() => typeof(MacFunctionTestBaseMAC_use_cases).Assembly);

        protected override string PlcName => this.Get(() => "S7-1500/ET200MP station_1");
        protected override string PlcCPUName => this.Get(() => "TestenvironmentPLC");
        protected override string OrderNumberPLC => this.Get(() => "");
    }

}
