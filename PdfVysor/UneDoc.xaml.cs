using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using Windows.Data.Pdf;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using WinRT.Interop;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace PdfVysor
{
    public sealed partial class UneDoc : Page
    {
        public UneDoc()
        {
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
        }

        private async void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFile.IsEnabled = false;
            FileOpenPicker openPicker = new();
            Window window = new();
            var hwnd = WindowNative.GetWindowHandle(window);
            InitializeWithWindow.Initialize(openPicker, hwnd);
            openPicker.FileTypeFilter.Add(".pdf");
            FilePickerSelectedFilesArray files = (FilePickerSelectedFilesArray)await openPicker.PickMultipleFilesAsync();
            OpenFile.IsEnabled = true;
        }

        private void CloseFile_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SaveFile_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
