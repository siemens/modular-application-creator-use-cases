using Newtonsoft.Json;
using Siemens.Automation.ModularApplicationCreator.ControlModules.ModuleEssentials.Objects.EssentialParameterOwner;

namespace MAC_use_cases.Model.ModuleEssentials.Base;

public class BaseModelParameterOwner : EssentialParameterOwner
{
    [JsonIgnore] 
    protected MAC_use_casesEM _module;

    [JsonConstructor]
    public BaseModelParameterOwner()
    {

    }

    public BaseModelParameterOwner(MAC_use_casesEM module) : base(string.Empty)
    {
        _module = module;
    }

    protected override void setup()
    {
        base.setup();
        CreateParameters();
    }

    protected virtual void CreateParameters()
    {

    }
    // Add code here for validation, additional logic, or other functionality as needed.
}
