using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;

namespace PdfVysor
{
    
    public sealed partial class ConfigDoc : Page
    {
        public ConfigDoc()
        {
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
            //ApplicationDataContainer roamingSettings = ApplicationData.Current.RoamingSettings;
            //Object obj = roamingSettings.Values[Constants.kNombreKey];
            //if (obj != null) test.Text = obj.ToString();
        }
    }
}
