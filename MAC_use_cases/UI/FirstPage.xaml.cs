using MAC_use_cases.Model;
using System.Windows;
using MAC_use_cases.Model.UseCases;
using Siemens.Automation.ModularApplicationCreator.Modules;
using Siemens.Automation.ModularApplicationCreator.Modules.UI;

namespace MAC_use_cases.UI
{
    /// <summary>
    ///     Interaction logic for FirstPage.xaml
    /// </summary>
    public partial class FirstPage : BaseEditor
    {
        public FirstPage(Module module) : base(module)
        {
            LoadXaml(this, "UI");
            Name = "FirstPage";
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

        private void GenerateRtuButton_Click(object sender, RoutedEventArgs e)
        {
            if (Module?.TargetDevice == null)
            {
                MessageBox.Show("Target PLC device is not available. Please ensure a device is selected.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                // Note: The order of generation is important.
                // 1. Create the data structures (DBs, UDTs)
                // 2. Create the blocks that use those structures
                // 3. Create the main program that calls the block instances

                // Step 1: Generate Global Settings DB
                RtuGeneration.GenerateSystemSettingsDB(Module.TargetDevice);

                // Step 2: Generate all Equipment Module FBs
                RtuGeneration.Generate_EM100_SupplyFan(Module.TargetDevice);
                RtuGeneration.Generate_EM200_CoolingControl(Module.TargetDevice);
                RtuGeneration.Generate_EM300_HeatingControl(Module.TargetDevice);
                RtuGeneration.Generate_EM400_DamperControl(Module.TargetDevice);
                RtuGeneration.Generate_EM500_SystemMonitoring(Module.TargetDevice);

                // Step 3: Generate the Main OB1 to coordinate everything
                RtuGeneration.Generate_OB1_Main(Module.TargetDevice, Module);

                MessageBox.Show("Basic RTU Project generation complete!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"An error occurred during project generation:\n\n{ex.Message}", "Generation Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
