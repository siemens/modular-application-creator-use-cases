using Siemens.Automation.ModularApplicationCreator.Core;
using Siemens.Automation.ModularApplicationCreator.Modules;
using Siemens.Automation.ModularApplicationCreator.Modules.UI;
using Siemens.Automation.ModularApplicationCreator.Tia.Modules;
using Siemens.Automation.ModularApplicationCreator.UI.UserControls;
using System.Windows;
using System.Windows.Controls;

namespace UseCaseBasedDoku.UI
{
    public partial class MultiPageFrame : BaseEditor
    {

        int _lastSelectedTabIndex;

        public MultiPageFrame(Module module) : base(module)
        {
            LoadXaml(this, "UI");
            InitializeComponent();

            CreatePages();

            NavigateTab.SelectedIndex = (NavigateTab.Items.Count == 0 ? -1 : 0);
            LoadNewPage();
            MacManagement.LanguageService.LanguageChanged += LanguageService_LanguageChanged;
            ValidatePages();
        }

        private void CreatePages()
        {
            // TODO Modify the FirstPage/SecondPage class or add a new BaseEditor class and enhance this function accordingly
            // The Name of the Page must be available as language Key in all language tables (e.g. Lang_de.xaml in the UI subfolder),
            // the language-dependent value to be displayed in the editor has to be specified there
            var page = new FirstPage(Module);
            CreateAndAddTab(page, "FirstPage");
            CreateAndAddTab(new SecondPage(Module), "SecondPage");
        }

        #region UI events

        private void LanguageService_LanguageChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            ValidatePages();
        }

        private void ButtonNext_Click(object sender, RoutedEventArgs e)
        {
            GoNext();
        }

        private void CreateAndAddTab(BaseEditor editor, string headerKey)
        {
            var item = new ValidatedTabItem();
            if (NavigateTab.Items.Count == 0)
            {
                item.Style = FindResource("Nav_big_default_left") as Style;
            }
            else
            {
                item.Style = FindResource("Nav_big_default") as Style;
            }

            item.Content = editor;
            item.Header = CreateHeader(headerKey);
            item.SetValue(System.Windows.Automation.AutomationProperties.AutomationIdProperty, "MultiPageFrame_Em" + (NavigateTab.Items.Count + 1) + "_TabItem");
            NavigateTab.Items.Add(item);
        }

        private StackPanel CreateHeader(string headerKey)
        {
            StackPanel panel = new StackPanel();
            panel.Orientation = Orientation.Horizontal;
            TextBlock headerText = new TextBlock();
            headerText.Name = "HeaderText";
            headerText.SetResourceReference(TextBlock.TextProperty, headerKey);
            panel.Children.Add(headerText);
            return panel;
        }


        private void ButtonBack_Click(object sender, RoutedEventArgs e)
        {
            if (NavigateTab.SelectedIndex > 0)
            {
                NavigateTab.SelectedIndex--;
            }
        }

        private void NavigateTab_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TabControl tabControl = sender as TabControl;
            if (tabControl == null || _lastSelectedTabIndex == tabControl.SelectedIndex)
            {
                return;
            }
            _lastSelectedTabIndex = tabControl.SelectedIndex;

            foreach (var removedItem in e.RemovedItems)
            {
                TabItem tab = removedItem as TabItem;
                if (tab != null && NavigateTab.Items.Contains(tab))
                {
                    BaseEditor editor = tab.Content as BaseEditor;
                    editor.Finish(true);
                }
            }
            LoadNewPage();
        }

        public void ValidatePages()
        {
            Dispatcher.Invoke(() =>
            {
                foreach (ValidatedTabItem item in NavigateTab.Items)
                {
                    item.Validate();
                }
            });
        }

        #endregion UI events

        #region public Methods
        public void GoNext()
        {
            if (NavigateTab.SelectedIndex + 1 < NavigateTab.Items.Count)
            {
                NavigateTab.SelectedIndex++;
            }
        }

        public override void Finish(bool save)
        {
            foreach (TabItem tab in NavigateTab.Items)
            {
                BaseEditor editor = tab.Content as BaseEditor;
                editor.Finish(true);
            }
        }

        public override void Load()
        {
            LoadNewPage();
        }

        protected void LoadNewPage()
        {

            TabItem currentTab = NavigateTab.SelectedItem as TabItem;
            if (currentTab != null)
            {
                BaseEditor editor = currentTab.Content as BaseEditor;
                if (editor != null)
                {
                    TiaEquipmentModuleBase tiaEM = Module as TiaEquipmentModuleBase;
                    if (tiaEM != null)
                    {
                        tiaEM.SetOnlineHelp(editor.Name);
                    }

                    BackButton.Visibility = Visibility.Visible;
                    NextButton.Visibility = Visibility.Visible;

                    if (NavigateTab.SelectedIndex == 0)
                    {
                        BackButton.Visibility = Visibility.Hidden;

                    }
                    if (NavigateTab.SelectedIndex == NavigateTab.Items.Count - 1)
                    {
                        NextButton.Visibility = Visibility.Hidden;
                    }
                }
            }
            else
            {
                BackButton.Visibility = Visibility.Hidden;
                NextButton.Visibility = Visibility.Hidden;
            }
        }

        public void OnLoad(object sender, RoutedEventArgs e)
        {
            Module.Validate();
        }
        #endregion public Methods

    }
}
