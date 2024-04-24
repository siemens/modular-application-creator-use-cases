using Siemens.Automation.ModularApplicationCreator.Modules;
using Siemens.Automation.ModularApplicationCreator.Modules.UI;
using MAC_use_cases.Model;

namespace MAC_use_cases.UI
{
    /// <summary>
    /// Interaction logic for FirstPage.xaml
    /// </summary>
    public partial class FirstPage : BaseEditor
    {
        protected new MAC_use_casesEM Module
        {
            get
            {
                return base.Module as MAC_use_casesEM;
            }
        }

        public FirstPage(Module module) : base(module)
        {
            LoadXaml(this, "UI");
            Name = "FirstPage";
            InitializeComponent();
            DataContext = module;
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
}