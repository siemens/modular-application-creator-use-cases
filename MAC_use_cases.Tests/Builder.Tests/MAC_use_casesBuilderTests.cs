using NUnit.Framework;

using MAC_use_cases.Tests.TestEnvironment;

using Siemens.ModularApplicationCreator.Testenvironment.TestCategoryAttributes;
using Siemens.ModularApplicationCreator.Testenvironment.UnitTestBaseClasses.Enums;

namespace MAC_use_cases.Tests.Builder.Tests
{
    public class MAC_use_casesBuilderTests : MacFunctionTestBaseMAC_use_cases
    {
        protected string LanguageSetting =>
            this.Get(() => ProjectOpenness.LanguageSettings.EditingLanguage.Culture.Name, NunitTestContext.TestCase);


[Test, MacFunctionTest]
public void GenerateOBMainTest()
{
    //Test could look like the following

    //GenericBlockCreation.GenerateOB_Main("MyMultiInstanceDBName", EquipmentModule, LanguageSetting, PlcDeviceMacSimulated);
}

[Test, MacFunctionTest]
public void GenerateS120Test()
{
    //A test for the hardware generation could look like the following.
    //Necessary to create the openness objects is to call the function GenerateHardware after configuration of the MAC objects.

    //HardwareGeneration.GenerateS120(EquipmentModule, "Test", "Test");
    //GenerateHardware();
}

}
}
