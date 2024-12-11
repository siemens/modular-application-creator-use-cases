using System.Linq;
using Siemens.Automation.ModularApplicationCreator.Core;
using Siemens.Automation.ModularApplicationCreator.Tia.Openness;
using Siemens.Automation.ModularApplicationCreatorBasics.Logging;
using Siemens.Engineering.Hmi;
using Siemens.Engineering.HW;
using Siemens.Engineering.HW.Features;
using Siemens.Engineering.SW;
using Project = Siemens.Engineering.Project;

namespace MAC_use_cases.Model.UseCases
{
    /// <summary>
    /// This are functions that support you in different ways while using Modular Application Creator
    /// </summary>
    public class GeneralSupport
    {
        /// <summary>
        /// This call generates a log message while generating a project
        /// \image html LogMessage.png
        /// </summary>
        /// <param name="logType">Defines the type of the message (Info, Warning, GenerationError, GenerationInfo, ...)</param>
        /// <param name="logMessage">The message that should be shown</param>
        /// <param name="equipmentModule">The corresponding equipment module</param>
        public static void LogMessage(LogTypes logType, string logMessage, MAC_use_casesEM equipmentModule)
        {
            MacManagement.LoggingService.LogMessage(logType, logMessage, equipmentModule.Name);
        }

        /// <summary>
        /// This call returns the openness object of the TIA Portal project CPU
        /// </summary>
        /// <param name="device">This object represents the TIA Portal PLC in the Modular Application Creator context</param>
        public static Siemens.Engineering.HW.DeviceItem GetOpennessDeviceItem(Siemens.Automation.ModularApplicationCreator.Tia.Openness.Device device)
        {
            /// <summary>
            /// The object tiaTemplateContext.TiaDevice gets castet to an openness object of an device. Because of that, its now posssible to navigate threw the openness objects
            /// </summary>
            /// 
            var opennesDevice = (Siemens.Engineering.HW.Device)device;
            return opennesDevice.DeviceItems.FirstOrDefault(x => x.Classification == DeviceItemClassifications.CPU);
        }

        /// <summary>
        /// This call returns the openness object of the TIA Portal project
        /// </summary>
        /// <param name="tiaProject">This object represents the TIA Portal Project in the Modular Application Creator context</param>
        public static Siemens.Engineering.Project GetOpennessProject(Siemens.Automation.ModularApplicationCreator.Tia.Openness.Project tiaProject)
        {
            return (Siemens.Engineering.Project)tiaProject;
        }
    }
}
