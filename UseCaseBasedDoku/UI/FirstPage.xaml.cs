using Siemens.Automation.ModularApplicationCreator.Modules;
using Siemens.Automation.ModularApplicationCreator.Modules.UI;
using UseCaseBasedDoku.Model;

namespace UseCaseBasedDoku.UI
{
    /// <summary>
    /// Interaction logic for FirstPage.xaml
    /// </summary>
    public partial class FirstPage : BaseEditor
    {
        protected new UseCaseBasedDokuEM Module
        {
            get
            {
                return base.Module as UseCaseBasedDokuEM;
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