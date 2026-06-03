using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Siemens.Automation.ModularApplicationCreator.Core;
using Siemens.Automation.ModularApplicationCreator.Tia;
using Siemens.Automation.ModularApplicationCreatorBasics.Logging;
using Siemens.Engineering.HW;
using Siemens.Engineering.HW.Features;
using Siemens.Engineering.SW;
using Siemens.Engineering.SW.Blocks;
using Device = Siemens.Automation.ModularApplicationCreator.Tia.Openness.Device;
using Project = Siemens.Engineering.Project;

namespace MAC_use_cases.Model.UseCases
{
    /// <summary>
    ///     This are functions that support you in different ways while using Modular Application Creator
    /// </summary>
    public class GeneralSupport
    {
        /// <summary>
        ///     This call generates a log message while generating a project
        ///     \image html LogMessage.png
        /// </summary>
        /// <param name="logType">Defines the type of the message (Info, Warning, GenerationError, GenerationInfo, ...)</param>
        /// <param name="logMessage">The message that should be shown</param>
        /// <param name="equipmentModule">The corresponding equipment module</param>
        public static void LogMessage(LogTypes logType, string logMessage, MAC_use_casesEM equipmentModule)
        {
            MacManagement.LoggingService.LogMessage(logType, logMessage, equipmentModule.Name);
        }

        /// <summary>
        ///     Retrieves a localized string value from the language resource dictionary using the specified key.
        ///     This method provides access to language-specific text resources for internationalization.
        /// </summary>
        /// <param name="key">The resource key used to look up the localized string value in the language dictionary</param>
        /// <returns>
        ///     The localized string value corresponding to the specified key. If the key is not found, returns an empty
        ///     string or the key itself (depending on the LanguageService implementation)
        /// </returns>
        public static string GetLocalizedString(string key)
        {
            return MacManagement.LanguageService.GetString(key);
        }

        /// <summary>
        ///     Retrieves the CPU DeviceItem from a TIA Portal device using the Openness API.
        /// </summary>
        /// <param name="device">The TIA Portal device to analyze</param>
        /// <returns>The first DeviceItem classified as CPU, or null if no CPU is found</returns>
        /// <remarks>
        ///     This method performs a type conversion from the internal Device type to the Openness API Device type,
        ///     allowing access to the Openness object model for device navigation.
        /// </remarks>
        public static DeviceItem GetOpennessDeviceItem(Siemens.Engineering.HW.Device device)
        {
            var opennessDevice = (Siemens.Engineering.HW.Device)device;
            return opennessDevice.DeviceItems.FirstOrDefault(x => x.Classification == DeviceItemClassifications.CPU);
        }

        /// <summary>
        ///     This call returns the openness object of the TIA Portal project
        /// </summary>
        /// <param name="tiaProject">This object represents the TIA Portal Project in the Modular Application Creator context</param>
        public static Siemens.Engineering.Project GetOpennessProject(
            Siemens.Automation.ModularApplicationCreator.Tia.Openness.Project tiaProject)
        {
            return (Siemens.Engineering.Project)tiaProject;
        }

        /// <summary>
        ///     Reads additional information from the TIA Portal project and populates
        ///     <see cref="MAC_use_casesEM.PlcBlockNames"/> with the names of all PLC blocks
        ///     found across all block groups recursively.
        ///     This method is intended to be called from
        ///     <see cref="MAC_use_casesEM.ReadTiaPortalAfterAssignOrUpdate"/> when
        ///     <see cref="MAC_use_casesEM.IsAdditionalReadOutRequired"/> returns <c>true</c>.
        /// </summary>
        /// <param name="module">
        ///     The equipment module whose <see cref="MAC_use_casesEM.PlcBlockNames"/> collection
        ///     will be updated with the retrieved block names
        /// </param>
        /// <param name="tiaTemplateContext">
        ///     The TIA Portal context providing access to the project and its devices,
        ///     used to locate the CPU and its associated PLC software
        /// </param>
        public static void ReadAdditionalInformationsFromTIAPortalProject(MAC_use_casesEM module, TiaTemplateContext tiaTemplateContext)
        {
            DeviceItem plc = null;
            foreach (var device in ((Siemens.Engineering.Project)tiaTemplateContext.TiaProject).Devices)
            {
                foreach (var deviceItem in device.DeviceItems)
                {
                    if (deviceItem.Classification == DeviceItemClassifications.CPU)
                    {
                        plc = deviceItem;
                        continue;
                    }
                }
            }
            var softwareContainer = plc.GetService<SoftwareContainer>().Software as PlcSoftware;
            var allBlocks = GetAllBlocksRecursive(softwareContainer.BlockGroup);

            Application.Current.Dispatcher.Invoke(() =>
            {
                module.PlcBlockNames.Clear();
                foreach (var block in allBlocks)
                {
                    module.PlcBlockNames.Add(block.Name);
                }
            });
        }

        /// <summary>
        ///     Recursively collects all <see cref="PlcBlock"/> objects from the given
        ///     <see cref="PlcBlockGroup"/> and all its nested subgroups.
        /// </summary>
        /// <param name="group">The root block group to start traversal from</param>
        /// <returns>A flat list of all blocks found at every nesting level</returns>
        private static List<PlcBlock> GetAllBlocksRecursive(PlcBlockGroup group)
        {
            var result = new List<PlcBlock>();

            foreach (var block in group.Blocks)
            {
                result.Add(block);
            }

            foreach (var subGroup in group.Groups)
            {
                result.AddRange(GetAllBlocksRecursive(subGroup));
            }

            return result;
        }
    }
}
