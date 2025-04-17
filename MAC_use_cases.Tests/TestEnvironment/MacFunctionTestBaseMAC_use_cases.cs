using System.Reflection;
using MAC_use_cases.Model;
using MAC_use_cases.TiaImports;
using NUnit.Framework;
using Siemens.ModularApplicationCreator.Testenvironment;
using Siemens.ModularApplicationCreator.Testenvironment.UnitTestBaseClasses.Enums;

namespace MAC_use_cases.Tests.TestEnvironment
{
    public class MacFunctionTestBaseMAC_use_cases : MacFunctionTestBase<MAC_use_casesEM>
    {
        protected ResourceManagement ResourceManagement
        {
            get => Get(() => new ResourceManagement(), NunitTestContext.TestCase);
        }


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
            get => Get(() => typeof(MacFunctionTestBaseMAC_use_cases).Assembly);
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
    }
}
