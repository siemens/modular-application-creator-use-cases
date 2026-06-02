using System;
using System.Linq;
using Siemens.Automation.ModularApplicationCreator.MacPublishedObjects.Subnet;
using Siemens.Automation.ModularApplicationCreator.Tia.Helper.TypeIdentifier;
using Siemens.Automation.ModularApplicationCreator.Tia.Helper.TypeIdentifier.Enums;
using Siemens.Automation.ModularApplicationCreator.Tia.Openness.DO;
using Siemens.Engineering;
using Siemens.Engineering.Hmi;
using Siemens.Engineering.HW;
using Siemens.Engineering.HW.Features;

namespace MAC_use_cases.Model.UseCases
{
    /// <summary>
    ///     All the functions to configure and generate Hardware are defined here.
    /// </summary>
    public class HardwareGeneration
    {
        /// <summary>
        ///     This function generates an S120 Drive based on a MasterCopy
        ///     \image html GenerateS120.png
        /// </summary>
        /// <param name="module">The Module</param>
        /// <param name="name">The name of the drive</param>
        /// <param name="deviceName">The name of the device</param>
        /// <param name="path">Path if necessary</param>
        /// <param name="comment">Comment if necessary</param>
        public static S120PNDriveInfo GenerateS120(MAC_use_casesEM module, string name, string deviceName,
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

                return info;
            }

            return null;
        }

        /// <summary>
        ///     This function generates an S210 Drive based on a MasterCopy
        ///     \image html GenerateS210.png
        /// </summary>
        /// <param name="module">The Module</param>
        /// <param name="name">The name of the drive</param>
        /// <param name="deviceName">The name of the device</param>
        /// <param name="path">Path if necessary</param>
        /// <param name="comment">Comment if necessary</param>
        public static S210DriveInfo GenerateS210(MAC_use_casesEM module, string name, string deviceName,
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

                return info;
            }

            return null;
        }

        /// <summary>
        ///     Creates or retrieves an HMI device in the TIA Portal project.
        ///     \image html GetOrCreateHMI.png
        /// </summary>
        /// <param name="project">The TIA Portal project object where the HMI device should be created or found.</param>
        /// <param name="name">The name identifier for the HMI device.</param>
        /// <returns>The HmiTarget object representing the software container of the HMI device.</returns>
        /// <remarks>
        ///     This method performs the following steps:
        ///     - Searches for an existing HMI device with the specified name
        ///     - If not found, creates a new HMI device with default configuration (6AV2 125-2JB23-0AX0/17.0.0.0)
        ///     - Retrieves the software container for the HMI Runtime
        ///     - Returns the HMI target software object
        /// </remarks>
        /// <exception cref="Exception">Thrown when the HMI software container cannot be accessed or created.</exception>
        public static HmiTarget GetOrCreateHMISoftware(Project project, string name)
        {
            var hmiDevice = project.Devices.FirstOrDefault(x => x.Name == name) ??
                            project.Devices.CreateWithItem("OrderNumber:6AV2 125-2JB23-0AX0/17.0.0.0", name, name);
            var hmiSoftwareContainer = hmiDevice.DeviceItems.FirstOrDefault(x => x.Name.Contains("HMI_RT"))
                .GetService<SoftwareContainer>();
            return hmiSoftwareContainer.Software as HmiTarget;
        }

        /// <summary>
        ///     Creates or retrieves a device in the TIA Portal project based on the specified parameters.
        /// </summary>
        /// <param name="project">The TIA Portal project object where the device should be created or found.</param>
        /// <param name="typeIdentifier">The device type identifier in the format "OrderNumber:XXXX/Version[/Type]".</param>
        /// <param name="name">The unique identifier name for the device within the project.</param>
        /// <param name="deviceName">The display name of the device in the TIA Portal.</param>
        /// <returns>The Device object representing the created or retrieved device.</returns>
        /// <remarks>
        ///     The typeIdentifier parameter should follow the format:
        ///     - For standard devices: "OrderNumber:1234567-1AX00/V1.0"
        ///     - For typed devices: "OrderNumber:1234567-1AX00/V1.0/Type1"
        ///     If a device with the specified name already exists, it will be returned.
        ///     Otherwise, a new device will be created with the specified parameters.
        /// </remarks>
        /// <exception cref="Exception">Thrown when the device cannot be created with the specified parameters.</exception>
        public static Device GetOrCreateDevice(Project project, string typeIdentifier, string name, string deviceName)
        {
            var device = project.Devices.FirstOrDefault(x => x.Name == name) ??
                         project.Devices.CreateWithItem(typeIdentifier, name, deviceName);
            return device;
        }

        /// <summary>
        ///     Gets or creates a subnet with the desired name
        ///     \image html GetOrCreateSubnet.png
        /// </summary>
        /// <param name="subnetsManager">The Modular Application Creator helper object</param>
        /// <param name="name">The name of the subnet</param>
        public static ISubnetInfo GetOrCreateSubnet(ISubnetsManager subnetsManager, string name)
        {
            return subnetsManager.GetOrCreateProfinet(name);
        }

        /// <summary>
        ///     Connects the desired drive to the desired subnet
        ///     \image html ConnectDriveToSubnet.png
        /// </summary>
        /// <param name="drive">The desired drive</param>
        /// <param name="subnet">The desired subnet</param>
        /// <param name="module">The module</param>
        public static void ConnectDriveToSubnet(ProfiDriveObjectInfo drive, ISubnetInfo subnet, MAC_use_casesEM module)
        {
            var device = module.SynchronizedCollection.HardwareInterfaces.OfType<ProfiDriveObjectInfo>()
                .FirstOrDefault(x => x.Name.Equals(drive.Name));

            var plcNwItf = module.ParentDeviceAsHardwareInfo.ControllerProfinetInterfaces.First();
            plcNwItf.ConnectedSubnetInfo = subnet;

            var ioSystem = plcNwItf.ConnectedSubnetInfo.GetOrCreateIoSystem(plcNwItf, "PROFINET IO-System");

            if (device.GetType() == typeof(S120PNDriveInfo))
            {
                (device as S120PNDriveInfo).ProfinetInterface.ConnectedSubnetInfo = subnet;
                ioSystem.ConnectIoDevice((device as S120PNDriveInfo).ProfinetInterface.IoConnectors.First().Value);
                plcNwItf.Ports.First().Connect((device as S120PNDriveInfo).ProfinetInterface.Ports.First());
            }

            if (device.GetType() == typeof(S210DriveInfo))
            {
                (device as S210DriveInfo).ProfinetInterface.ConnectedSubnetInfo = subnet;
                ioSystem.ConnectIoDevice((device as S210DriveInfo).ProfinetInterface.IoConnectors.First().Value);
                plcNwItf.Ports.First().Connect((device as S210DriveInfo).ProfinetInterface.Ports.First());
            }
        }
    }
}
