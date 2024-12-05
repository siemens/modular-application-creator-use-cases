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

namespace MAC_use_cases.Model.UseCases.SoftwareUnits.SafetyUnit
{
    public class SoftwareUnitsUseCases
    {
        public PlcDevice PlcDevice { get; set; }
        public TiaEquipmentModule Module { get; set; }

        public SoftwareUnitsUseCases(TiaEquipmentModule module, PlcDevice plcDevice)
        {
            Module = module;
            PlcDevice = plcDevice;
        }

        /// <summary>
        /// Get the safety software unit
        /// </summary>
        /// <returns></returns>
        public ISafetySoftwareUnit GetSafetySoftwareUnit()
        {
            return PlcDevice.SoftwareUnits.GetSafetySoftwareUnit();
        }

        /// <summary>
        /// get the software unit by name
        /// </summary>
        /// <param name="name"></param>
        /// <param name="module"></param>
        /// <returns></returns>
        public ISoftwareUnit GetSoftwareUnitByName(string name)
        {
            return PlcDevice.SoftwareUnits.GetOrCreateSoftwareUnit(name, Module);
        }

        /// <summary>
        /// Example how to create a failsafe function block within a safety software unit
        /// </summary>
        public void CrateFailSafeFunctionBlock()
        {
            var safetySoftwareUnit = GetSafetySoftwareUnit();

            var interfaceName = "myParameter"; //TODO name of the parameter in the function block
            var interfaceType = "\"ToEdit\""; //TODO type of the parameter in the function block

            var failSafeFb = new XmlFailSafeFB("MyFailSafeFB");
            failSafeFb.Interface[InterfaceSections.Static].Add(new InterfaceParameter(interfaceName, interfaceType)
            {
                Remanence = RemanenceSettings.IgnoreRemanence
            });

            //here an example how to create a multi instance call in the function block

            var nameOfTheBlockFromTheLibrary = "BlockNameToCall"; //name of the block from the library

            var blockCall = new MultiInstanceCall(interfaceName, nameOfTheBlockFromTheLibrary, safetySoftwareUnit)
            {
                //TODO connect in and outputs of the block
            };

            var network = new BlockNetwork();
            network.Blocks.Add(blockCall);

            failSafeFb.Networks.Add(network);
            failSafeFb.GenerateXmlBlock(safetySoftwareUnit);
        }

    }
}
