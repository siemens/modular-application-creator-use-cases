using NUnit.Framework;

using MAC_use_cases.Tests.TestEnvironment;

using Siemens.ModularApplicationCreator.Testenvironment.TestCategoryAttributes;

namespace MAC_use_cases.Tests.Builder.Tests
{
    public class GenerateTiaPortalTest : MacGenerationTestBaseMAC_use_cases
    {
        [Test, MacGenerationTest]
        public void GenerateTiaPortal_Test()
        {
            GenerateTiaPortal();
            OpenProjectInNewTiaPortalWithUi();
        }
    }
}
