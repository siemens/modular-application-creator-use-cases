using Siemens.Automation.ModularApplicationCreator.Tia.Helper.TypeIdentifier;
using Siemens.Automation.ModularApplicationCreator.Tia.Helper.TypeIdentifier.Enums;
using Siemens.Automation.ModularApplicationCreator.Tia.Openness.DO;
using System.Linq;

namespace MAC_use_cases.Model.UseCases
{
    /// <summary>
    /// All the functions to configure and generate Hardware are defined here.
    /// </summary>
    public class HardwareGeneration
    {
        /// <summary>
        /// This function generates an S120 Drive based on a MasterCopy
        /// \image html GenerateS120.png
        /// </summary>
        /// <param name="module">The Module</param>
        /// <param name="name">The name of the drive</param>
        /// <param name="deviceName">The name of the device</param>
        /// <param name="path">Path if necessary</param>
        /// <param name="comment">Comment if necessary</param>
        public static void GenerateS120(MAC_use_casesEM module, string name, string deviceName,
            string path = null, string comment = null)
        {
            if (!module.SynchronizedCollection.HardwareInterfaces.OfType<ProfiDriveObjectInfo>()
                    .Any(x => x.DriveDevice.Equals(name)))
            {
                var info = HardwareBlueprintFactory.CreateDrive(S120PNOrderNumbers.S120_CU_320_2_PN).LatestFirmware()
                    .CreateBlueprint(name, deviceName);

                info.DevicePath = path;
                info.Comment = comment;
                info.PlcName = module.ParentDevice.Name;
                info.CreateSingleAxis("SingleAxis", "OrderNumber:6SL3310-1TE32-1AAx", out _,
                    AxisDriveObjectType.Vector);

                info.GetAxis("SingleAxis").PlcName = module.ParentDevice.Name;

                module.SynchronizedCollection.HardwareInterfaces.Add(info);
            }
        }

        /// <summary>
        /// This function generates an S210 Drive based on a MasterCopy
        /// \image html GenerateS210.png
        /// </summary>
        /// <param name="module">The Module</param>
        /// <param name="name">The name of the drive</param>
        /// <param name="deviceName">The name of the device</param>
        /// <param name="path">Path if necessary</param>
        /// <param name="comment">Comment if necessary</param>
        public static void GenerateS210(MAC_use_casesEM module, string name, string deviceName,
            string path = null, string comment = null)
        {
            if (!module.SynchronizedCollection.HardwareInterfaces.OfType<ProfiDriveObjectInfo>()
                    .Any(x => x.DriveDevice.Equals(name)))
            {
                var info = HardwareBlueprintFactory.CreateDrive(S210OrderNumbers.S210_PN_3AC_7_kW).LatestFirmware()
                    .CreateBlueprint(name, deviceName);

                info.DevicePath = path;
                info.Comment = comment;
                info.PlcName = module.ParentDevice.Name;

                module.SynchronizedCollection.HardwareInterfaces.Add(info);
            }
        }
    }
}
