using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Siemens.Automation.ModularApplicationCreator.Tia;
using Siemens.Automation.ModularApplicationCreator.Tia.Helper.Create_XML_Block.XmlBlocks.BlockFrames;
using Siemens.Automation.ModularApplicationCreator.Tia.Helper.Create_XML_Block;
using Siemens.Automation.ModularApplicationCreator.Tia.Modules;
using Siemens.Automation.ModularApplicationCreator.Tia.Openness;
using Siemens.Automation.ModularApplicationCreator.Tia.Openness.SoftwareUnit;
using System.Xml.Linq;

namespace MAC_use_cases.Model.UseCases.SoftwareUnits
{
    public class SoftwareUnitsUseCases
    {
        /// <summary>
        /// Example how to create a failsafe function block within a safety software unit
        /// \image html CreateInstanceDB_via_XmlInstDB.png
        /// </summary>
        /// <param name="name">The name of the SoftwareUnit</param>
        /// <param name="plcDevice">The plc device</param>
        /// <param name="equipmentModule">The corresponding equipment module</param>
        public static void CreateFailSafeFunctionBlock(string softwareUnitName, PlcDevice plcDevice, MAC_use_casesEM module)
        {
            var safetySoftwareUnit = plcDevice.SoftwareUnits.GetSafetySoftwareUnit();

            var interfaceName = "myParameter"; //TODO name of the parameter in the function block
            var interfaceType = "\"ToEdit\""; //TODO type of the parameter in the function block

            var failSafeFb = new XmlFailSafeFB("MyFailSafeFB");
            failSafeFb.Interface[InterfaceSections.Static].Add(new InterfaceParameter(interfaceName, interfaceType)
            {
                Remanence = RemanenceSettings.IgnoreRemanence
            });

            //here an example how to create a multi instance call in the function block

            failSafeFb.GenerateXmlBlock(safetySoftwareUnit);
        }

    }
}
