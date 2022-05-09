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

namespace PdfVysor
{

    public sealed partial class DivideDoc : Page
    {
        private enum Search
        {
            FirstStart,
            PreviousStart,
            NextStart,
            LastStart,
            SearchStart,

            FirstEnd,
            PreviousEnd,
            NextEnd,
            LastEnd,
            SearchEnd,

            Initial
        };

        private enum Updated
        {
            Start,
            End,
            Initial
        };

        private PdfDocument m_pdfDocument;
        private StorageFile m_file;
        private int m_startPage = 0;
        private int m_endPage;
        private Dictionary<int, BitmapImage> m_pageCache = new();
        private Updated m_lastUpdated;


        public DivideDoc()
        {
            InitializeComponent();
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
            StorageFile file = await openPicker.PickSingleFileAsync();
            PdfDocument pdf = null;
            if (file != null)
            {
                try
                {
                    pdf = await PdfDocument.LoadFromFileAsync(file);
                    m_file = file;
                }
                catch
                {
                    ShowErrorDialog();
                }
            }

            if (pdf != null)
            {
                NewPdfLoaded(pdf);
            }

            if (m_pdfDocument != null)
            {
                PageControllersVisibily(Visibility.Visible);
                UpdatePagesValues();
                CheckPageControllers();
                m_pageCache.Clear();
                NavigatesPages(Search.Initial);
            }


            OpenFile.IsEnabled = true;

        }

        //----------------------------------------------------------------------------------------------------------
        /*
            Shows reading document error dialog
         */
        private async void ShowErrorDialog()
        {
            ContentDialog errorDialog = new()
            {
                Title = "Error de lectura",
                Content = "Ha ocurrido un error mientras se abria el documento, compruebe que el documento es correcto o no este protegido.",
                CloseButtonText = "Aceptar",
                PrimaryButtonText = "Abrir otro archivo",
                DefaultButton = ContentDialogButton.Primary

            };
            errorDialog.XamlRoot = this.Content.XamlRoot;
            ContentDialogResult result = await errorDialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                OpenFile_Click(null, null);
            }
        }

        private void CheckPageControllers()
        {
            /*
                doc = 1 page index 1
                pagina inicial = 1
                pagina final = 1
             */
            if (m_pdfDocument.PageCount == 1)
            {
                FirstPageStartPage.IsEnabled = false;
                PreviousPageStartPage.IsEnabled = false;
                NextPageStartPage.IsEnabled = false;
                LastPageStartPage.IsEnabled = false;
                FirstPageEndPage.IsEnabled = false;
                PreviousPageEndPage.IsEnabled = false;
                NextPageEndPage.IsEnabled = false;
                LastPageEndPage.IsEnabled = false;
                return;
            }


            /*
                doc = 10 page index 1
                pagina inicial = 1
                pagina final = 1
             */
            if (m_startPage == 0 && m_endPage == m_startPage)
            {
                FirstPageStartPage.IsEnabled = false;
                PreviousPageStartPage.IsEnabled = false;
                NextPageStartPage.IsEnabled = false;
                LastPageStartPage.IsEnabled = false;
                FirstPageEndPage.IsEnabled = false;
                PreviousPageEndPage.IsEnabled = false;
                NextPageEndPage.IsEnabled = true;
                LastPageEndPage.IsEnabled = true;
                return;
            }

            /*
                doc = 10 page index 1
                pagina inicial = 10
                pagina final = 10
             */
            if (m_startPage == (m_pdfDocument.PageCount - 1))
            {
                FirstPageStartPage.IsEnabled = true;
                PreviousPageStartPage.IsEnabled = true;
                NextPageStartPage.IsEnabled = false;
                LastPageStartPage.IsEnabled = false;
                FirstPageEndPage.IsEnabled = false;
                PreviousPageEndPage.IsEnabled = false;
                NextPageEndPage.IsEnabled = false;
                LastPageEndPage.IsEnabled = false;
                return;
            }


            if (m_endPage == m_startPage)
            {
                FirstPageEndPage.IsEnabled = false;
                PreviousPageEndPage.IsEnabled = false;
                NextPageEndPage.IsEnabled = true;
                LastPageEndPage.IsEnabled = true;
            }
            else if (m_endPage == m_pdfDocument.PageCount - 1)
            {
                FirstPageEndPage.IsEnabled = true;
                PreviousPageEndPage.IsEnabled = true;
                NextPageEndPage.IsEnabled = false;
                LastPageEndPage.IsEnabled = false;
            }
            else
            {
                FirstPageEndPage.IsEnabled = true;
                PreviousPageEndPage.IsEnabled = true;
                NextPageEndPage.IsEnabled = true;
                LastPageEndPage.IsEnabled = true;
            }


            if (m_startPage == 0)
            {
                FirstPageStartPage.IsEnabled = false;
                PreviousPageStartPage.IsEnabled = false;
                NextPageStartPage.IsEnabled = true;
                LastPageStartPage.IsEnabled = true;
            }
            else if (m_startPage == m_endPage)
            {
                FirstPageStartPage.IsEnabled = true;
                PreviousPageStartPage.IsEnabled = true;
                NextPageStartPage.IsEnabled = false;
                LastPageStartPage.IsEnabled = false;
            }
            else
            {
                FirstPageStartPage.IsEnabled = true;
                PreviousPageStartPage.IsEnabled = true;
                NextPageStartPage.IsEnabled = true;
                LastPageStartPage.IsEnabled = true;
            }
        }

        private void UpdatePagesValues()
        {
            ActualPageStartPage.Content = m_startPage + 1;
            ActualPageEndPage.Content = m_endPage + 1;
        }

        private void NewPdfLoaded(PdfDocument pdf)
        {
            m_pdfDocument = null;
            m_pdfDocument = pdf;
            m_startPage = 0;
            m_endPage = (int)m_pdfDocument.PageCount - 1;
        }

        private void PageControllersVisibily(Visibility visibility)
        {
            PageController.Visibility = visibility;
            Pages.Visibility = visibility;
            if (visibility == Visibility.Visible)
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

        /*
         *  Start page
         */


        private void FirstPageStartPage_Click(object sender, RoutedEventArgs e)
        {
            NavigatesPages(Search.FirstStart);
        }

        private void PreviousPageStartPage_Click(object sender, RoutedEventArgs e)
        {
            NavigatesPages(Search.PreviousStart);
        }

        private void SearchPageStartPage_Click(object sender, RoutedEventArgs e)
        {
            string page = InputPageBoxStartPage.Text;
            if (!string.IsNullOrEmpty(page))
            {
                if (int.TryParse(page, out int index) && IsInRangeStart(index))
                {
                    m_startPage = index - 1;
                    NavigatesPages(Search.SearchStart);
                }
                else
                {
                    InputPageBoxStartPage.Text = (m_startPage + 1).ToString();
                }
            }
        }

        private bool IsInRangeStart(int index)
        {
            return index > 0 && index < m_endPage;
        }

        private void NextPageStartPage_Click(object sender, RoutedEventArgs e)
        {
            NavigatesPages(Search.NextStart);
        }

        private void LastPageStartPage_Click(object sender, RoutedEventArgs e)
        {
            NavigatesPages(Search.LastStart);
        }


        /*
         *  End page
         */


        private void FirstPageEndPage_Click(object sender, RoutedEventArgs e)
        {
            NavigatesPages(Search.FirstEnd);
        }

        private void PreviousPageEndPage_Click(object sender, RoutedEventArgs e)
        {
            NavigatesPages(Search.PreviousEnd);
        }

        private void SearchPageEndPage_Click(object sender, RoutedEventArgs e)
        {
            string page = InputPageBoxEndPage.Text;
            if (!string.IsNullOrEmpty(page))
            {
                if (int.TryParse(page, out int index) && IsInRangeEnd(index))
                {
                    m_endPage = index - 1;
                    NavigatesPages(Search.SearchEnd);
                }
                else
                {
                    InputPageBoxEndPage.Text = (m_endPage + 1).ToString();
                }
            }

        }

        private bool IsInRangeEnd(int index)
        {
            return index > m_startPage && index <= (m_pdfDocument.PageCount);
        }

        private void NextPageEndPage_Click(object sender, RoutedEventArgs e)
        {
            NavigatesPages(Search.NextEnd);
        }

        private void LastPageEndPage_Click(object sender, RoutedEventArgs e)
        {
            NavigatesPages(Search.LastEnd);
        }

        private void NavigatesPages(Search firstStart)
        {
            switch (firstStart)
            {
                case Search.FirstStart:
                    m_startPage = 0;
                    m_lastUpdated = Updated.Start;
                    break;

                case Search.PreviousStart:
                    if (m_startPage > 0) m_startPage--;
                    m_lastUpdated = Updated.Start;
                    break;

                case Search.NextStart:
                    if (m_startPage < m_endPage) m_startPage++;
                    m_lastUpdated = Updated.Start;
                    break;

                case Search.LastStart:
                    m_startPage = m_endPage;
                    m_lastUpdated = Updated.Start;
                    break;

                case Search.SearchStart:
                    FlyoutPageStartPage.Hide();
                    m_lastUpdated = Updated.Start;
                    break;


                case Search.FirstEnd:
                    if (m_endPage > m_startPage) m_endPage = m_startPage;
                    m_lastUpdated = Updated.End;
                    break;

                case Search.PreviousEnd:
                    if (m_endPage > m_startPage) m_endPage--;
                    m_lastUpdated = Updated.End;
                    break;

                case Search.NextEnd:
                    if (m_endPage < (m_pdfDocument.PageCount - 1)) m_endPage++;
                    m_lastUpdated = Updated.End;
                    break;

                case Search.LastEnd:
                    m_endPage = (int)(m_pdfDocument.PageCount - 1);
                    m_lastUpdated = Updated.End;
                    break;

                case Search.SearchEnd:
                    FlyoutPageEndPage.Hide();
                    m_lastUpdated = Updated.End;
                    break;


                case Search.Initial:
                    m_startPage = 0;
                    m_endPage = (int)(m_pdfDocument.PageCount - 1);
                    m_lastUpdated = Updated.Initial;
                    break;
            }
            LoadingPage(true);
            UpdatePagesValues();
            CheckPageControllers();
            ManageRendering();
        }

        private void ManageRendering()
        {
            if (m_startPage == m_endPage)
            {
                RenderPage(m_startPage, StartPage);
                EndPage.Source = null;
            }
            else
            {
                switch (m_lastUpdated)
                {
                    case Updated.Initial:
                        RenderPage(m_startPage, StartPage);
                        RenderPage(m_endPage, EndPage);
                        break;

                    case Updated.Start:
                        RenderPage(m_startPage, StartPage);
                        if (EndPage.Source == null) RenderPage(m_endPage, EndPage);
                        break;

                    case Updated.End:
                        RenderPage(m_endPage, EndPage);
                        break;
                }
            }
            LoadingPage(false);
        }

        private void ShowPage(BitmapImage src, Image dest)
        {
            dest.Source = src;
        }

        private async void RenderPage(int pageNum, Image output)
        {
            BitmapImage src = new();
            if (m_pageCache.ContainsKey(pageNum) && m_pageCache.TryGetValue(pageNum, out src))
            {
                ShowPage(src, output);
                return;
            }
            PdfPage page = m_pdfDocument.GetPage((uint)pageNum);
            InMemoryRandomAccessStream stream = new();
            var options = new PdfPageRenderOptions
            {
                BackgroundColor = Windows.UI.Color.FromArgb(255, 255, 255, 255)
            };
            await page.RenderToStreamAsync(stream, options);
            await src.SetSourceAsync(stream);
            m_pageCache.TryAdd(pageNum, src);
            ShowPage(src, output);
        }

        private void LoadingPage(bool v)
        {
            ProgressBar.IsActive = v;
            if (v) ProgressBar.Visibility = Visibility.Visible;
            if (!v) ProgressBar.Visibility = Visibility.Collapsed;
            v = !v;
            FileController.IsEnabled = v;
            PageController.IsEnabled = v;
        }

        private void CloseFile_Click(object sender, RoutedEventArgs e)
        {
            m_pdfDocument = null;
            m_file = null;
            PageControllersVisibily(Visibility.Collapsed);
            StartPage.Source = null;
            EndPage.Source = null;
            m_pageCache.Clear();
        }

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
                PdfLibrary.Splitter.SplitPdfByPage(m_file.Path, file.Path, m_startPage + 1, m_endPage + 1);
            }
        }
    }
}
