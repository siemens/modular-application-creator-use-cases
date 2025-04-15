using Siemens.Automation.ModularApplicationCreator.Tia.Openness;
using Siemens.Automation.ModularApplicationCreator.Tia.Openness.SoftwareUnit;

namespace MAC_use_cases.Model.UseCases;

public class SoftwareUnits
{
    /// <summary>
    ///     Retrieves an existing software unit or creates a new one if it doesn't exist in the specified PLC device.
    /// </summary>
    /// <param name="plcDevice">The PLC device where the software unit should be located or created</param>
    /// <param name="myUnitName">The name of the software unit to get or create</param>
    /// <param name="macUseCasesEm">The MAC use case enumeration specifying the unit's configuration</param>
    /// <returns>An interface to the existing or newly created software unit</returns>
    /// <remarks>
    ///     This method provides a convenient way to ensure a software unit exists, creating it if necessary.
    /// </remarks>
    public static ISoftwareUnit GetOrCreateSoftwareUnit(PlcDevice
        plcDevice, string myUnitName, MAC_use_casesEM macUseCasesEm)
    {
        return plcDevice.SoftwareUnits.GetOrCreateSoftwareUnit(myUnitName, macUseCasesEm);
    }

    /// <summary>
    ///     Retrieves the safety software unit from the specified PLC device.
    /// </summary>
    /// <param name="plcDevice">The PLC device containing the safety software unit</param>
    /// <returns>An interface to the safety software unit</returns>
    /// <remarks>
    ///     This method only retrieves an existing safety software unit and does not create one if it doesn't exist.
    /// </remarks>
    public static ISafetySoftwareUnit GetSafetySoftwareUnit(PlcDevice
        plcDevice)
    {
        return plcDevice.SoftwareUnits.GetSafetySoftwareUnit();
    }
}
