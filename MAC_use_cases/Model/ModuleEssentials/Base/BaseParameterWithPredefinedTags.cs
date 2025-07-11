using Newtonsoft.Json;
using Siemens.Automation.ModularApplicationCreator.ControlModules.ModuleEssentials.Objects.EssentialParameter;

namespace MAC_use_cases.Model.ModuleEssentials.Base;


public class BaseParameterWithPredefinedTags : EssentialParameterWithPredefinedTags,    IEssentialParameterWithPredefinedTags
{
    [JsonConstructor]
    public BaseParameterWithPredefinedTags()
    {

    }

    public BaseParameterWithPredefinedTags(string defaultValue) : base(defaultValue)
    {

    }

    // Add code here for validation, additional logic, or other functionality as needed.

}
