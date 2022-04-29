namespace PdfVysor
{
    internal class Constants
    {
        /*
         * MainWindow.xaml.cs
         */

        // title of the application
        public static string kTitleWindow = "PdfVysor";


        /*
         * Visor.xaml.cs
         */
        public const int kHeightImage = 1188; // base height of the pdf page
        public const int kWidthImage = 840; // base width of the pdf page
        public const double kPageRenderQuality = 2.0; // quality of the page's render

        // zoom level based on 1 as 100% (in the indicators is on % based)
        public const float kMinZoom = 0.20f; // min factor of zoom level in the scroller page
        public const float kMaxZoom = 5.0f; // max factor of zoom level in the scroller page
        public const float kDefaultZoomValue = (float)1; // default zoom value

        // every click of the buttons increase or
        // decrease 0.10 (10%) zoom value
        public const float kStepFrequencyZoom = (float)0.1; // every click of the buttons increase or
    }
}
