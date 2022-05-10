using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.Data.Pdf;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using WinRT.Interop;

namespace PdfVysor
{
    public sealed partial class UneDoc : Page
    {

        private List<Document> m_documents = new();
        private int m_actualZoom = 2;

        public UneDoc()
        {
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;

        }

        //----------------------------------------------------------------------------------------------------------
        /*
            Open new file
         */
        private async void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFile.IsEnabled = false;
            FileOpenPicker openPicker = new();
            Window window = new();
            var hwnd = WindowNative.GetWindowHandle(window);
            InitializeWithWindow.Initialize(openPicker, hwnd);
            openPicker.FileTypeFilter.Add(".pdf");
            var files = (FilePickerSelectedFilesArray)await openPicker.PickMultipleFilesAsync();
            LoadingContent(true);
            foreach (var file in files)
            {
                // every file is converted and saved the first page, path and name of Pdf
                if (file != null)
                {
                    try
                    {
                        PdfDocument doc = await PdfDocument.LoadFromFileAsync(file);
                        BitmapImage src = new();
                        InMemoryRandomAccessStream stream = new();
                        PdfPage page = doc.GetPage(0);
                        var options = new PdfPageRenderOptions
                        {
                            BackgroundColor = Windows.UI.Color.FromArgb(255, 255, 255, 255)
                        };
                        options.DestinationHeight = Constants.kHeightImage;
                        options.DestinationWidth = Constants.kWidthImage;
                        await page.RenderToStreamAsync(stream, options);
                        await src.SetSourceAsync(stream);
                        m_documents.Add(new Document()
                        {
                            Title = file.Name,
                            Path = file.Path,
                            FirstPage = src
                        });
                    }
                    catch
                    {
                        // do something when one document its wrong
                    }
                }
            }

            // load the files in the GridView
            ObservableCollection<Document> data = new(m_documents);
            GridDocuments.ItemsSource = data;

            // set up zoom dependig if there are files
            if (m_documents.Count > 0)
            {
                ZoomOut.IsEnabled = true;
                ZoomIn.IsEnabled = true;
            }
            else
            {
                ZoomOut.IsEnabled = false;
                ZoomIn.IsEnabled = false;
            }
            LoadingContent(false);
            OpenFile.IsEnabled = true;
        }

        //----------------------------------------------------------------------------------------------------------
        /*
            Set the enviroment while the content its loading
         */
        private void LoadingContent(bool v)
        {
            ProgressControl.IsActive = v;
            ProgressControl.Visibility = (v ? Visibility.Visible : Visibility.Collapsed);
            v = !v;
            FileController.IsEnabled = v;
            GridDocuments.IsEnabled = v;
        }

        //----------------------------------------------------------------------------------------------------------
        /*
            Closing all the selected files in the GridView
         */
        private void CloseFile_Click(object sender, RoutedEventArgs e)
        {
            LoadingContent(true);
            var data = GridDocuments.SelectedItems;
            foreach (Document doc in data)
            {
                m_documents.Remove(doc);
            }
            GridDocuments.ItemsSource = new ObservableCollection<Document>(m_documents);
            LoadingContent(false);
        }

        //----------------------------------------------------------------------------------------------------------
        /*
            Creating new document based on the files that are selected
         */
        private async void SaveFile_Click(object sender, RoutedEventArgs e)
        {
            var savePicker = new FileSavePicker();

            savePicker.FileTypeChoices.Add("Portable Document Format", new List<string>() { ".pdf" });

            savePicker.SuggestedFileName = "New Document";
            Window window = new();
            var hwnd = WindowNative.GetWindowHandle(window);
            InitializeWithWindow.Initialize(savePicker, hwnd);
            StorageFile file = await savePicker.PickSaveFileAsync();
            if (file != null)
            {
                LoadingContent(true);
                var pages = GridDocuments.SelectedItems; // obtain the selected files
                List<String> paths = new();
                String orig = (pages[0] as Document).Path; // getting the first 
                pages.RemoveAt(0); // removing the first

                foreach (Document page in pages) paths.Add(page.Path); // adding all the paths on a list
                PdfLibrary.Merger.Merge(orig, file.Path, paths); // creating the new document
                LoadingContent(false);
            }
        }

        //----------------------------------------------------------------------------------------------------------
        /*
            Increase the zoom
         */
        private void ZoomIn_Click(object sender, RoutedEventArgs e)
        {
            if (m_actualZoom < 4) m_actualZoom++;
            SetZoomValue();
        }

        //----------------------------------------------------------------------------------------------------------
        /*
            Managing the zoom level by changing the ItemTemplate on the GridDocument
         */
        private void SetZoomValue()
        {
            switch (m_actualZoom)
            {
                case 1:
                    GridDocuments.ItemTemplate = (DataTemplate)this.Resources["PdfDocuments1"];
                    ZoomIn.IsEnabled = true;
                    ZoomOut.IsEnabled = false;
                    break;
                case 2:
                    GridDocuments.ItemTemplate = (DataTemplate)this.Resources["PdfDocuments2"];
                    ZoomIn.IsEnabled = true;
                    ZoomOut.IsEnabled = true;
                    break;
                case 3:
                    GridDocuments.ItemTemplate = (DataTemplate)this.Resources["PdfDocuments3"];
                    ZoomIn.IsEnabled = true;
                    ZoomOut.IsEnabled = true;
                    break;
                case 4:
                    GridDocuments.ItemTemplate = (DataTemplate)this.Resources["PdfDocuments4"];
                    ZoomIn.IsEnabled = false;
                    ZoomOut.IsEnabled = true;
                    break;
            }
        }

        //----------------------------------------------------------------------------------------------------------
        /*
            Decrease the zoom
         */
        private void ZoomOut_Click(object sender, RoutedEventArgs e)
        {
            if (m_actualZoom > 1) m_actualZoom--;
            SetZoomValue();
        }

        //----------------------------------------------------------------------------------------------------------
        /*
            Checking if there are files selected to enable or disable some buttons
         */
        private void GridDocuments_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GridDocuments.SelectedItems.Count > 0)
            {
                CloseFile.IsEnabled = true;
                SaveFile.IsEnabled = true;
            }
            else
            {
                CloseFile.IsEnabled = false;
                SaveFile.IsEnabled = false;
            }
        }


    }

    //Data class of document
    public class Document
    {
        public String Path { get; set; }
        public String Title { get; set; }
        public BitmapImage FirstPage { get; set; }


        public Document()
        {
        }
    }
}
