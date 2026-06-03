using MAC_use_cases.Model.ModuleEssentials.Example.Parameter;

namespace MAC_use_cases.Model.ModuleEssentials.Example;

public class TechnologyObjectViewModel
{
    public TechnologyObjectDataModel Model { get; set; }

    public TechnologyObjectViewModel(TechnologyObjectDataModel model)
    {
        Model = model;
    }

    public Parameter_IsVirtualAxis IsVirtualAxis => Model.GetParameter<Parameter_IsVirtualAxis>();
}
