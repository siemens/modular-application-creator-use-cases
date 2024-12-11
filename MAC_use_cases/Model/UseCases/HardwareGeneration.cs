using Siemens.Automation.ModularApplicationCreator.Tia.Helper.TypeIdentifier;
using Siemens.Automation.ModularApplicationCreator.Tia.Helper.TypeIdentifier.Enums;
using Siemens.Automation.ModularApplicationCreator.Tia.Openness.DO;
using Siemens.Engineering.Hmi;
using Siemens.Engineering.HW.Features;
using System.Linq;
using Siemens.Automation.ModularApplicationCreator.MacPublishedObjects.Subnet;
using Siemens.Engineering;

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
        /// This function generates an S210 Drive based on a MasterCopy
        /// \image html GenerateS210.png
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
        /// This call returns the openness object of an HMI with the desired name. If it exists, it will be returned. If not, it will create a new one.
        /// \image html GetOrCreateHMI.png
        /// </summary>
        /// <param name="project">This object is the openness object of the TIA Portal project</param>
        /// <param name="name">Thats the desired name of the HMI Device</param>
        public static HmiTarget GetOrCreateHMISoftware(Project project, string name)
        {
            var hmiDevice = project.Devices.FirstOrDefault(x => x.Name == name);
            if (hmiDevice == null)
            {
                hmiDevice = project.Devices.CreateWithItem("OrderNumber:6AV2 125-2JB23-0AX0/17.0.0.0", name, name);
            }
            var hmiSoftwareContainer = hmiDevice.DeviceItems.FirstOrDefault(x => x.Name.Contains("HMI_RT")).GetService<SoftwareContainer>();
            return hmiSoftwareContainer.Software as HmiTarget;
        }

        /// <summary>
        /// Gets or creates a subnet with the desired name
        /// \image html GetOrCreateSubnet.png
        /// </summary>
        /// <param name="subnetsManager">The Modular Application Creator helper object</param>
        /// <param name="name">The name of the subnet</param>
        public static ISubnetInfo GetOrCreateSubnet(ISubnetsManager subnetsManager, string name)
        {
            return subnetsManager.GetOrCreateProfinet(name);
        }

        /// <summary>
        /// Connectes the desired drive to the desired subnet
        /// \image html ConnectDriveToSubnet.png
        /// </summary>
        /// <param name="drive">The desired drive</param>
        /// <param name="subnet">The desired subnet</param>
        /// <param name="module">The module</param>
        public static void ConnectDriveToSubnet(ProfiDriveObjectInfo drive, ISubnetInfo subnet, MAC_use_casesEM module)
        {
            var device = module.SynchronizedCollection.HardwareInterfaces.OfType<ProfiDriveObjectInfo>().FirstOrDefault(x => x.Name.Equals(drive.Name));

            var plcNwItf = module.ParentDeviceAsHardwareInfo.ControllerProfinetInterfaces.First();
            plcNwItf.ConnectedSubnetInfo = subnet;

            if (device.GetType() == typeof(S120PNDriveInfo))
            {
                (device as S120PNDriveInfo).ProfinetInterface.ConnectedSubnetInfo = subnet;
                var ioSystem = plcNwItf.ConnectedSubnetInfo.GetOrCreateIoSystem(plcNwItf, "NewIoSystem");
                ioSystem.ConnectIoDevice((device as S120PNDriveInfo).ProfinetInterface.IoConnectors.First().Value);

                plcNwItf.Ports.First().Connect((device as S120PNDriveInfo).ProfinetInterface.Ports.First());
            }
            if (device.GetType() == typeof(S210DriveInfo))
            {
                (device as S210DriveInfo).ProfinetInterface.ConnectedSubnetInfo = subnet;
                var ioSystem = plcNwItf.ConnectedSubnetInfo.GetOrCreateIoSystem(plcNwItf, "NewIoSystem");
                ioSystem.ConnectIoDevice((device as S210DriveInfo).ProfinetInterface.IoConnectors.First().Value);

                plcNwItf.Ports.First().Connect((device as S210DriveInfo).ProfinetInterface.Ports.First());
            }


        }
    }
}
