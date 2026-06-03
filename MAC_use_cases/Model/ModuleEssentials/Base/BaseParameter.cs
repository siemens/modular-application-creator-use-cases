using Newtonsoft.Json;
using Siemens.Automation.ModularApplicationCreator.ControlModules.ModuleEssentials.Enums;
using Siemens.Automation.ModularApplicationCreator.ControlModules.ModuleEssentials.Objects.EssentialParameter;

namespace MAC_use_cases.Model.ModuleEssentials.Base;

public class BaseParameter : EssentialParameter
{
    [JsonConstructor]
    public BaseParameter()
    {

    }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="BaseParameter"/> class with the specified type, default value, and
    /// position unit.
    /// </summary>
    /// <param name="type">The type of the parameter, which determines its essential characteristics.</param>
    /// <param name="defaultValue">The default value assigned to the parameter. This value is used if no other value is provided.</param>
    /// <param name="posUnit">The position unit for the parameter in the user interface. Defaults to <see cref="PositionunitForUI.None"/> if
    /// not specified.</param>
    public BaseParameter(EssentialParameterType type, string defaultValue, PositionunitForUI posUnit = PositionunitForUI.None) : base(null!, type, defaultValue, posUnit)
    {

    }

    // Add code here for validation, additional logic, or other functionality as needed.
}
