using System.Collections.Generic;
using MAC_use_cases.Model.ModuleEssentials.Base;
using Siemens.Automation.ModularApplicationCreator.ControlModules.ModuleEssentials.Enums;
using Siemens.Automation.ModularApplicationCreator.ControlModules.ModuleEssentials.Objects.EssentialParameter.Generation;
using Siemens.Automation.ModularApplicationCreator.Tia.Openness.TO;

namespace MAC_use_cases.Model.ModuleEssentials.Example.Parameter;

public class Parameter_IsVirtualAxis :BaseParameter, ITOParameter
{
    private const bool _defaultValue = false;
    private const EssentialParameterType _parameterType = EssentialParameterType.Bool;

    public string ToPath => "VirtualAxis.Mode"; // Path for the parameter, used for identification in Openness

    public Parameter_IsVirtualAxis() : base(_parameterType, _defaultValue.ToString(), PositionunitForUI.None)
    {
        // Constructor logic if needed
    }

    /// <summary>
    /// In Openness, this parameter is used to determine if the axis is virtual or not.
    /// Instead of using a boolean, Openness expects a string value of "1" for true and "0" for false.
    /// </summary>
    /// <returns></returns>
    public override string GetValueForGeneration()
    {
        return ValueAsBool ? "1" : "0";
    }

    public IEnumerable<TechnologicalObjectInfo> GetTargetTechnologicalObjectInfos()
    {
        //Additional logic to return the target technological object infos if needed
        return null;
    }

}
