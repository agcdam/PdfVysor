using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System.Linq;

namespace PdfVysor
{
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Title = Constants.kTitleWindow;

            // Custom title bar 
            ExtendsContentIntoTitleBar = true;
            SetTitleBar(AppTitleBar);
            AppTitleTextBlock.Text = Constants.kTitleWindow;
            Activated += MainWindow_Activated;
        }

        /// <summary>
        /// Change the state of <see cref="AppTitleBar"/> when the window isn't active
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void MainWindow_Activated(object sender, WindowActivatedEventArgs args)
        {
            if (args.WindowActivationState == WindowActivationState.Deactivated)
            {
                AppTitleTextBlock.Foreground = (SolidColorBrush)App.Current.Resources["WindowCaptionForegroundDisabled"];
            }
            else
            {
                AppTitleTextBlock.Foreground =
            (SolidColorBrush)App.Current.Resources["WindowCaptionForeground"];
            }
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
                    case "crea":
                        ContentFram.Navigate(typeof(CreaDoc));
                        break;
                }
            }
        }
    }
}
