using System.Linq;
using MAC_use_cases.Model.ModuleEssentials.Base;
using MAC_use_cases.Model.ModuleEssentials.Example.Parameter;
using MAC_use_cases.Model.UseCases;
using Newtonsoft.Json;
using Siemens.Automation.ModularApplicationCreator.ControlModules.ModuleEssentials.Objects.EssentialParameter;
using Siemens.Automation.ModularApplicationCreator.ControlModules.ModuleEssentials.Objects.EssentialParameter.Generation;
using Siemens.Automation.ModularApplicationCreator.Tia.Openness;
using Siemens.Automation.ModularApplicationCreator.Tia.Openness.TO;

namespace MAC_use_cases.Model.ModuleEssentials.Example;

public class TechnologyObjectDataModel : BaseModelParameterOwner
{
    [JsonConstructor]
    public TechnologyObjectDataModel()
    {
        
    }

    public string MotionControlVersion => "8.0"; // Example version for TIA V19
    public TechnologicalObjectInfo TechObjectInfo { get; set; }

    public TechnologyObjectDataModel(MAC_use_casesEM parentModel) : base(parentModel)
    {
        TechObjectInfo = new TechnologicalObjectInfo("MyEssentialTechnologyObject", OpennessConstants.TO_POSITIONING_AXIS, MotionControlVersion, null);
        parentModel.ProvidedTechnologicalObjects.Add(TechObjectInfo); // Register the technological object blueprint in the parent model.
                                                                      // This step is necessary to ensure the object is recognized in other MAC structures like drive browser (UI Control).
    }

    protected override void setup()
    {
        base.setup();
        CreateParameters();
    }

    protected override void CreateParameters()
    {
        RegisterParameter(new Parameter_IsVirtualAxis()); // Example of registering a parameter
                                                          // Add other parameters as needed
    }

    public void AddTechnologicalObjectParams()
    {
        foreach (var parameter in this.Parameters.OfType<ITOParameter>())
        {
            this.TechObjectInfo.AdditionalParameter[parameter.ToPath] = parameter.GetValueForGeneration();
        }
    }
}
