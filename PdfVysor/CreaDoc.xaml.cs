using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using Windows.Data.Pdf;
using Windows.UI.Popups;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using WinRT.Interop;
using mux = Microsoft.UI.Xaml.Controls;


namespace PdfVysor
{
    #region JSON Models
    public class Documentacion
    {
        public String Name { get; set; }
        public List<Maquina> Maquinas { get; set; }
    }

    public class Maquina
    {
        public String Name { set; get; }
        public List<Documento> Partes { get; set; }
    }

    public class Documento
    {
        public String Name { set; get; }
        public String Path { set; get; }
    }
    #endregion
    public class Tasks
    {
        public String Name { set; get; }
        public List<GroupTasks> GroupTasks { get; set; }

        public Tasks() { }
    }

    public class GroupTasks
    {
        public String Name { set; get; }
        public List<SimpleTask> Tasks { get; set; }

        public GroupTasks() { }
    }

    public class SimpleTask
    {
        public String Name { set; get; }

        public SimpleTask() { }
    }

    public class Split : SimpleTask
    {
        public StorageFile File { get; set; }
        public int FirstPage { get; set; }
        public int LastPage { get; set; }
        public Split() { }
    }

    public class Merge : SimpleTask
    {
        public List<StorageFile> Files { get; set; }
        public Merge() { }
    }
    

    public sealed partial class CreaDoc : Page
    {
        public Tasks m_tasks;
        public CreaDoc()
        {
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;

            //InitializeData();
        }

        private void InitializeData()
        {
            Documentacion doc = new Documentacion();
            doc.Name = "Tareas";
            List<Maquina> maquinaList = new List<Maquina>();
            for (int i = 0; i < 3; i++)
            {
                maquinaList.Add(new Maquina() { 
                    Name = $"Grupo de tareas {i + 1}",
                    Partes = new List<Documento>()
                });
                for (int j = 0; j < 5; j++) { 
                    maquinaList[i].Partes.Add(new Documento() { Name = $"Tarea {j + 1}" }); 
                }
            }
            doc.Maquinas = maquinaList;

            mux.TreeViewNode docu = new() { Content = doc.Name };
            foreach (Maquina maquina in doc.Maquinas)
            {
                mux.TreeViewNode node = new() { Content = maquina.Name };
                foreach (Documento d in maquina.Partes)
                {
                    node.Children.Add(new() { Content = d.Name });
                }
                node.IsExpanded = true;
                docu.Children.Add(node);
            }
            docu.IsExpanded = true;
            ListMach.RootNodes.Add(docu);
        }

        //private void RadioButtons_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    switch (Options.SelectedIndex)
        //    {
        //        case 0:
        //            Dividir.Visibility = Visibility.Visible;
        //            Unir.Visibility = Visibility.Collapsed;
        //            break;
        //        case 1:
        //            Dividir.Visibility = Visibility.Collapsed;
        //            Unir.Visibility = Visibility.Visible;
        //            break;
        //        case 2:
        //            Dividir.Visibility = Visibility.Collapsed;
        //            Unir.Visibility = Visibility.Collapsed;
        //            break;
        //    }
        //}

        //private void Guardar_Click(object sender, RoutedEventArgs e)
        //{
        //    ApplicationDataContainer roamingSettings = ApplicationData.Current.RoamingSettings;
        //    roamingSettings.Values[Constants.kNombreKey] = Nombre.Text;


        //}


    }


}
