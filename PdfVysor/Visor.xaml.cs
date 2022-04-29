using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
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
    public enum Search { First, Previous, Next, Last, Search }

    public sealed partial class Visor : Page
    {
        private PdfDocument m_pdfDocument;
        private int m_actualPage = 0;
        private float m_zoomValue = 1.0f;
        private Dictionary<int, BitmapImage> m_pageCache = new();
        private bool m_documentIsLoaded = false;

        public Visor()
        {
            this.InitializeComponent();
            // setting the cache enable to save the state of the page
            this.NavigationCacheMode = NavigationCacheMode.Enabled;
        }

        /*
         * Pages
         */

        //----------------------------------------------------------------------------------------------------------
        /*
            Open new dialog to pick new pdf file and load it in the enviroment 
         */
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
                }
                catch
                {
                }
            }

            if (pdf != null)
            {
                NewPdfLoaded(pdf);
            }

            if (m_pdfDocument != null)
            {
                PageControllersVisibily(Visibility.Visible);
                TotalPages.Content = m_pdfDocument.PageCount;
                UpdatePageValue(Search.First);
                CheckPageControls();
            }

            OpenFile.IsEnabled = true;
        }

        //----------------------------------------------------------------------------------------------------------
        /*
            Checks if the actual page is rendered yet, if not, render it and show it
         */
        private async void RenderPage()
        {
            BitmapImage src = new();
            if (m_pageCache.ContainsKey(m_actualPage)
                && m_pageCache.TryGetValue(m_actualPage, out src))
            {
                LoadingPage(false);
                ShowPage(src);
                return;
            }
            PdfPage page = m_pdfDocument.GetPage((uint)m_actualPage);
            InMemoryRandomAccessStream stream = new();
            var options = new PdfPageRenderOptions
            {
                BackgroundColor = Windows.UI.Color.FromArgb(255, 255, 255, 255)
            };
            options.DestinationHeight = (uint)(page.Size.Height * Constants.kPageRenderQuality);
            options.DestinationWidth = (uint)(page.Size.Width * Constants.kPageRenderQuality);
            await page.RenderToStreamAsync(stream, options);
            await src.SetSourceAsync(stream);
            m_pageCache.TryAdd(m_actualPage, src);
            LoadingPage(false);
            ShowPage(src);
        }

        //----------------------------------------------------------------------------------------------------------
        /*
            Sets the image (src) as source of image viewer
         */
        private void ShowPage(BitmapImage src)
        {
            Output.Source = src;
            Output.Height = Constants.kHeightImage;
            Output.Width = Constants.kWidthImage;
            m_documentIsLoaded = true;
        }

        //----------------------------------------------------------------------------------------------------------
        /*
            Check what page controls are necesarily to block
         */
        private void CheckPageControls()
        {
            if (m_pdfDocument.PageCount == 1)
            {
                PreviousPage.IsEnabled = false;
                FirstPage.IsEnabled = false;
                NextPage.IsEnabled = false;
                LastPage.IsEnabled = false;
                return;
            }

            if (m_actualPage == 0)
            {
                PreviousPage.IsEnabled = false;
                FirstPage.IsEnabled = false;
                NextPage.IsEnabled = true;
                LastPage.IsEnabled = true;
            }
            else if (m_actualPage == (m_pdfDocument.PageCount - 1))
            {
                PreviousPage.IsEnabled = true;
                FirstPage.IsEnabled = true;
                NextPage.IsEnabled = false;
                LastPage.IsEnabled = false;
            }
            else
            {
                PreviousPage.IsEnabled = true;
                FirstPage.IsEnabled = true;
                NextPage.IsEnabled = true;
                LastPage.IsEnabled = true;
            }
        }

        //----------------------------------------------------------------------------------------------------------
        /*
            Depends on the demand of page do different things 
            like show first, previous, next, last or the searched page
         */
        private void UpdatePageValue(Search page)
        {
            switch (page)
            {
                case Search.First:
                    m_actualPage = 0;
                    break;
                case Search.Previous:
                    if (m_actualPage > 0) m_actualPage--;
                    break;
                case Search.Next:
                    if (m_actualPage < m_pdfDocument.PageCount) m_actualPage++;
                    break;
                case Search.Last:
                    m_actualPage = (int)(m_pdfDocument.PageCount - 1);
                    break;
                case Search.Search:
                    FlyoutPage.Hide();
                    break;
            }
            LoadingPage(true);
            ActualPage.Content = m_actualPage + 1;
            CheckPageControls();
            RenderPage();
        }

        //----------------------------------------------------------------------------------------------------------
        /*
            Block all the controls to avoid overload of events, unnecessary controls or overload of memory and process
         */
        private void LoadingPage(bool v)
        {
            progressBar.IsActive = v;
            if (v) progressBar.Visibility = Visibility.Visible;
            if (!v) progressBar.Visibility = Visibility.Collapsed;
            v = !v;
            CommandBar.IsEnabled = v;
        }

        //----------------------------------------------------------------------------------------------------------
        /*
            Set visible all the controls when the document has been loaded
         */
        private void PageControllersVisibily(Visibility visibility)
        {
            FirstPage.Visibility = visibility;
            PreviousPage.Visibility = visibility;
            ActualPage.Visibility = visibility;
            TotalPages.Visibility = visibility;
            NextPage.Visibility = visibility;
            LastPage.Visibility = visibility;
            ZoomOut.Visibility = visibility;
            ZoomValue.Visibility = visibility;
            ZoomIn.Visibility = visibility;
            ResetZoom.Visibility = visibility;
            Separator1.Visibility = visibility;
            Separator2.Visibility = visibility;
            ScrollerPage.Visibility = visibility;
            AdjustZoom.Visibility = visibility;
        }

        //----------------------------------------------------------------------------------------------------------
        /*
            When new document is loaded, set up the enviroment
         */
        private void NewPdfLoaded(PdfDocument pdf)
        {
            m_pdfDocument = null;
            m_pdfDocument = pdf;
            m_actualPage = 0;
            m_zoomValue = 1;
            m_pageCache.Clear();
        }

        //----------------------------------------------------------------------------------------------------------
        /*
            Navigates to the first page
         */
        private void FirstPage_Click(object sender, RoutedEventArgs e)
        {
            UpdatePageValue(Search.First);
        }

        //----------------------------------------------------------------------------------------------------------
        /*
            Navigates to the previous page
         */
        private void PreviousPage_Click(object sender, RoutedEventArgs e)
        {
            UpdatePageValue(Search.Previous);
        }

        //----------------------------------------------------------------------------------------------------------
        /*
            Search a especific page and shows it if exists
         */
        private void SearchPage_Click(object sender, RoutedEventArgs e)
        {
            string page = InputPageBox.Text;
            if (!string.IsNullOrEmpty(page))
            {
                if (int.TryParse(page, out int newPage) && IsInRange(newPage))
                {
                    m_actualPage = newPage - 1;
                    UpdatePageValue(Search.Search);
                }
                else
                {
                    //pagina introducida errornia
                    InputPageBox.Text = (m_actualPage + 1).ToString();
                }

            }
        }

        //----------------------------------------------------------------------------------------------------------
        /*
            Check if the page it's between first and last page
         */
        private bool IsInRange(int newPage)
        {
            if (newPage > 0 && newPage <= (int)m_pdfDocument.PageCount) return true;
            return false;
        }

        //----------------------------------------------------------------------------------------------------------
        /*
            Navigates to the next page
         */
        private void NextPage_Click(object sender, RoutedEventArgs e)
        {
            UpdatePageValue(Search.Next);
        }

        //----------------------------------------------------------------------------------------------------------
        /*
            Navigates to the last page
         */
        private void LastPage_Click(object sender, RoutedEventArgs e)
        {
            UpdatePageValue(Search.Last);
        }


        /*
         * Zoom
         */


        //----------------------------------------------------------------------------------------------------------
        /*
            When AdjustZoom is clicked, set the property zoom to adjust the page with the frame
         */
        private void AdjustZoom_Click(object sender, RoutedEventArgs e)
        {
            AdjustPageZoom();
        }

        //----------------------------------------------------------------------------------------------------------
        /*
            If AdjustZoom is selected, calculate the necessary zoom needed to adjust the page
         */
        private void AdjustPageZoom()
        {
            if (AdjustZoom.IsChecked.Value == true)
            {
                float actualSizeScroller = ScrollerPage.ActualSize.Y;

                // with 10 extra in the size of the page, 
                // the page is 10 smaller than the scroller (5 on top and 5 on bottom)
                // letting this margin the page's border isn't joined with the borders
                m_zoomValue = actualSizeScroller / (Constants.kHeightImage + 10);
                UpdateScrollerViewer();
            }
        }

        //----------------------------------------------------------------------------------------------------------
        /*
            Updates the zoom of the scroller page, 
         */
        private void UpdateScrollerViewer()
        {
            if (ScrollerPage.IsLoaded) ScrollerPage.ChangeView(null, null, m_zoomValue);
        }

        //----------------------------------------------------------------------------------------------------------
        /*
            Decrease the zoom value one step the value stablished (0.10 || 10%)
         */
        private void ZoomOut_Click(object sender, RoutedEventArgs e)
        {
            if (m_zoomValue > (Constants.kMinZoom + Constants.kStepFrequencyZoom))
            {
                m_zoomValue -= Constants.kStepFrequencyZoom;
            }
            else if (m_zoomValue > Constants.kMinZoom)
            {
                m_zoomValue -= m_zoomValue - Constants.kMinZoom;
            }
            UncheckAdjustZoom();
            UpdateZoomValue();
            UpdateScrollerViewer();
        }

        //----------------------------------------------------------------------------------------------------------
        /*
            Sets the actual zoom value to the indicator
         */
        private void UpdateZoomValue()
        {
            ZoomValue.Content = ((int)(m_zoomValue * 100)) + " %"; // showing up the zoom value on base 100%
        }

        //----------------------------------------------------------------------------------------------------------
        /*
            If the togle of adjust zoom us checked, unchecked it
         */
        private void UncheckAdjustZoom()
        {
            if (AdjustZoom.IsChecked.Value) AdjustZoom.IsChecked = false;
        }

        //----------------------------------------------------------------------------------------------------------
        /*
            When the slider of zoom change, updates zoomValue, scrollerPage zoom and calls to set the zoom indicator
         */
        private void ZoomSelector_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            m_zoomValue = (float)e.NewValue / 100; // converting the value from base 100% to base 1
            UncheckAdjustZoom();
            UpdateZoomValue();
            if (m_documentIsLoaded) UpdateScrollerViewer();
        }

        //----------------------------------------------------------------------------------------------------------
        /*
            Increase the zoom value one step the value stablished (0.10 || 10%)
         */
        private void ZoomIn_Click(object sender, RoutedEventArgs e)
        {
            if (m_zoomValue < (Constants.kMaxZoom - Constants.kStepFrequencyZoom))
            {
                m_zoomValue += Constants.kStepFrequencyZoom;
            }
            else if (m_zoomValue < Constants.kMaxZoom)
            {
                m_zoomValue += Constants.kMaxZoom - m_zoomValue;
            }
            UncheckAdjustZoom();
            UpdateZoomValue();
            UpdateScrollerViewer();
        }

        //----------------------------------------------------------------------------------------------------------
        /*
            Set the zoom value to (1.0 || 100%) being this value by default
         */
        private void ResetZoom_Click(object sender, RoutedEventArgs e)
        {
            UncheckAdjustZoom();
            m_zoomValue = Constants.kDefaultZoomValue;
            UpdateScrollerViewer();
            UpdateZoomValue();
        }

        //----------------------------------------------------------------------------------------------------------
        /*
            When in the scroller page changes his size and adjust zoom is enable, change the zoom to adjust the page
         */
        private void ScrollerPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            AdjustPageZoom();
        }

        //----------------------------------------------------------------------------------------------------------
        /*
            Update the zoom value and the indicator of zoom to the new value
         */
        private void ScrollerPage_ViewChanging(object sender, ScrollViewerViewChangingEventArgs e)
        {
            m_zoomValue = ScrollerPage.ZoomFactor;
            UpdateZoomValue();
        }

        //----------------------------------------------------------------------------------------------------------
        /*
            Sets the actual zoom value to the slider when the flyout that contains it it's opening
         */
        private void FlyoutZoom_Opening(object sender, object e)
        {
            ZoomSelector.Value = ((int)(m_zoomValue * 100));
        }
    }
}
