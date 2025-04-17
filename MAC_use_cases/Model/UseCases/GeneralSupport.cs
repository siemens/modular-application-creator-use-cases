using System.Linq;
using Siemens.Automation.ModularApplicationCreator.Core;
using Siemens.Automation.ModularApplicationCreatorBasics.Logging;
using Siemens.Engineering.HW;
using Device = Siemens.Automation.ModularApplicationCreator.Tia.Openness.Device;
using Project = Siemens.Engineering.Project;

namespace MAC_use_cases.Model.UseCases;

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
    public static DeviceItem GetOpennessDeviceItem(Device device)
    {
        var opennessDevice = (Siemens.Engineering.HW.Device)device;
        return opennessDevice.DeviceItems.FirstOrDefault(x => x.Classification == DeviceItemClassifications.CPU);
    }

    /// <summary>
    ///     This call returns the openness object of the TIA Portal project
    /// </summary>
    /// <param name="tiaProject">This object represents the TIA Portal Project in the Modular Application Creator context</param>
    public static Project GetOpennessProject(
        Siemens.Automation.ModularApplicationCreator.Tia.Openness.Project tiaProject)
    {
        return (Project)tiaProject;
    }
}
