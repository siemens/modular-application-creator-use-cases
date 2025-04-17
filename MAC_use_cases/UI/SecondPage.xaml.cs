using MAC_use_cases.Model;
using Siemens.Automation.ModularApplicationCreator.Modules;
using Siemens.Automation.ModularApplicationCreator.Modules.UI;

namespace MAC_use_cases.UI;

/// <summary>
///     Interaction logic for FirstPage.xaml
/// </summary>
public partial class SecondPage : BaseEditor
{
    public SecondPage(Module module) : base(module)
    {
        LoadXaml(this, "UI");
        Name = "SecondPage";
        InitializeComponent();
        DataContext = module;
    }

    protected new MAC_use_casesEM Module
    {
        get => base.Module as MAC_use_casesEM;
    }

    public override void Load()
    {
        //ToDo: Init Properties
    }


    public override void Finish(bool save)
    {
        if (save)
        {
            //ToDo: Save Properties
        }
    }
}
