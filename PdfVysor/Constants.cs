namespace PdfVysor
{
    internal class Constants
    {
        /*
         * MainWindow.xaml.cs
         */

        /// <summary>
        /// Title of the application
        /// </summary>
        public static string kTitleWindow = "PdfVysor";


        /*
         * Visor.xaml.cs
         */

        /// <summary>
        /// Base height of the pdf pages
        /// </summary>
        public const int kHeightImage = 1188;

        /// <summary>
        /// Base width of the pdf pages
        /// </summary>
        public const int kWidthImage = 840;

        /// <summary>
        /// Quality of the rendering pages
        /// </summary>
        public const double kPageRenderQuality = 2.0;

        /// <summary>
        /// Minimum zoom factor in the visor page.
        /// </summary>
        public const float kMinZoom = 0.20f;

        /// <summary>
        /// Maximum zoom factor in the visor page
        /// </summary>
        public const float kMaxZoom = 5.0f;

        /// <summary>
        /// Default zoom value
        /// </summary>
        public const float kDefaultZoomValue = 1.0f;

        /// <summary>
        /// Step value when increase or decrease
        /// </summary>
        public const float kStepFrequencyZoom = (float)0.1;


        /*
         * UneDoc.xaml.cs
         */

        /// <summary>
        /// Number of zoom levels
        /// </summary>
        public const int kZoomLevels = 4; // Number of zoom levels


        /*
         * JsonController.xaml.cs
         */

        /// <summary>
        /// Name of configuration file
        /// </summary>
        public const string kFileJsonName = "DocConfig.json";


        /*
         * Tasks.xaml.cs
         */

        /// <summary>
        /// Name of <see cref="Tasks"/> object
        /// </summary>
        public const string kTasksName = "Tareas";


        /*
         * CreaDoc.xaml.cs
         */

        /// <summary>
        /// Maximum of messages to show at once
        /// </summary>
        public const int kMaxMessages = 4;

        /// <summary>
        /// Time in ms. to hide the message automatically
        /// </summary>
        public const int kTimeToHideMessages = 6500;

        /// <summary>
        /// Default rotation angle for the water mark
        /// </summary>
        public const int kDefaultAngle = 305;

        /// <summary>
        /// Default opacity for the water mark
        /// </summary>
        public const int kDefaultOpacity = 50;

        /// <summary>
        /// One degree corresponds to this value in radians
        /// </summary>
        public const double kFactorDegreesToRadians = 0.017453;
    }
}
