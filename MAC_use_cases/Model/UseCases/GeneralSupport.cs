using Siemens.Automation.ModularApplicationCreator.Core;
using Siemens.Automation.ModularApplicationCreator.Tia.Helper.Create_XML_Block.XmlBlocks.BlockFrames;
using Siemens.Automation.ModularApplicationCreator.Tia.Openness;
using Siemens.Automation.ModularApplicationCreatorBasics.Logging;
using System.Xml.Linq;

namespace MAC_use_cases.Model.UseCases
{
    /// <summary>
    /// This are functions that support you in different ways while using Modular Application Creator
    /// </summary>
    public class GeneralSupport
    {
        /// <summary>
        /// This call generates a log message while generating a project
        /// </summary>
        /// <param name="logType">Defines the type of the message (Info, Warning, GenerationError, GenerationInfo, ...)</param>
        /// <param name="logMessage">The message that should be shown</param>
        /// <param name="equipmentModule">The corresponding equipment module</param>
        public static void LogMessage(LogTypes logType, string logMessage, MAC_use_casesEM equipmentModule)
        {
            MacManagement.LoggingService.LogMessage(logType, logMessage, equipmentModule.Name);
        }
    }
}
