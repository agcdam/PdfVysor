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

        /// <summary>
        /// Navigates to <see cref="Visor"/> page when <see cref="Nav"/> is loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Nav_Loaded(object sender, RoutedEventArgs e)
        {
            Nav.SelectedItem = Nav.MenuItems.First();
            ContentFram.Navigate(typeof(Visor));
        }


        /// <summary>
        /// Load new <see cref="Frame"/> into <see cref="ContentFram"/>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
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
                }
            }
        }
    }
}
