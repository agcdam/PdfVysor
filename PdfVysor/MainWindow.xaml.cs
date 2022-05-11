using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Linq;

namespace PdfVysor
{
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
            this.Title = Constants.kTitleWindow;
        }

        private void Nav_Loaded(object sender, RoutedEventArgs e)
        {
            Nav.SelectedItem = Nav.MenuItems.First();
            ContentFram.Navigate(typeof(Visor));
        }

        private void Nav_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            var selectedItem = (NavigationViewItem)args.SelectedItem;
            if (selectedItem != null)
            {
                string selectedItemTag = selectedItem.Tag as string;
                switch (selectedItemTag)
                {
                    case "visor":
                        ContentFram.Navigate(typeof(Visor));
                        break;
                    case "divide":
                        ContentFram.Navigate(typeof(DivideDoc));
                        break;
                    case "unir":
                        ContentFram.Navigate(typeof(UneDoc));
                        break;
                    case "crea":
                        ContentFram.Navigate(typeof(CreaDoc));
                        break;
                    case "config":
                        ContentFram.Navigate(typeof(ConfigDoc));
                        break;
                }
            }
        }
    }
}
