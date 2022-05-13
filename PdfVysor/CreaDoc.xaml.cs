using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Windows.Data.Pdf;
using Windows.Storage;
using Windows.Storage.Pickers;
using WinRT.Interop;
using mux = Microsoft.UI.Xaml.Controls;


namespace PdfVysor
{
    public class Item
    {
        public string Name { get; set; }
    }

    public sealed partial class CreaDoc : Page
    {
        private Tasks m_tasks;
        private List<StorageFile> m_filesOrig;
        private StorageFile m_fileOut;
        private JsonController m_controller;

        public List<Tuple<string, GroupTask>> GroupTasksList
        {
            get
            {
                List<Tuple<string, GroupTask>> result = new();
                if (m_tasks.GroupTasks != null)
                {
                    foreach (var gTask in m_tasks.GroupTasks)
                    {
                        result.Add(new Tuple<string, GroupTask>(gTask.Name, gTask));
                    }
                }
                return result;
            }
        }


        public CreaDoc()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;

            InitializeData();
        }

        private void InitializeData()
        {
            LoadingData(true);
            if (m_controller == null) m_controller = new JsonController();
        }

        private void LoadingData(Boolean status)
        {
            ProgressStatus.IsActive = status;
            ListMach.IsEnabled = !status;
        }

        private void UpdateInfo()
        {
            ListMach.RootNodes.Clear();
            mux.TreeViewNode tasks = new() { Content = m_tasks.Name };
            foreach (GroupTask gTask in m_tasks.GroupTasks)
            {
                mux.TreeViewNode node = new() { Content = gTask.Name };
                if (gTask.Tasks != null) foreach (SimpleTask simpleTask in gTask.Tasks) node.Children.Add(new() { Content = simpleTask.Name });
                node.IsExpanded = true;
                tasks.Children.Add(node);
            }
            tasks.IsExpanded = true;
            if (m_tasks.GroupTasks.Count > 0)
            {
                HeaderGropuTasks.Visibility = Visibility.Visible;
            }
            else
            {
                HeaderGropuTasks.Visibility = Visibility.Collapsed;
            }
            ListMach.RootNodes.Add(tasks);
            ListGroups.ItemsSource = GroupTasksList;
            if (ListGroups.SelectedIndex == -1) ListGroups.SelectedIndex = 0;
            _ = Task.Run(async () =>
            {
                if (m_controller != null) m_controller.SaveData(m_tasks);
            });
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (m_tasks.NameExists(GroupName.Text) || SimplyTaskName.Text.Equals(m_tasks.Name))
                {
                    ShowInfo("Ya existe un grupo con ese nombre", InfoBarSeverity.Informational);
                    return;
                }
                m_tasks.AddGroupTasks(GroupName.Text);
                UpdateInfo();
            }
            catch
            {
                ShowInfo("Nombre de grupo vacio", InfoBarSeverity.Informational);
            }

        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            LoadingData(true);
            FlyOutDelete.Hide();
            //var list = ListMach.SelectedNodes;

            foreach (var task in ListMach.SelectedNodes)
            {
                Debug.WriteLine((string)task.Content + m_tasks.NameExists((string)task.Content));
                m_tasks.RemoveItemsByName((string)task.Content);
            }

            UpdateInfo();

            LoadingData(false);
        }

        private void AddSimpleTask_Click(object sender, RoutedEventArgs e)
        {
            if (ListGroups.SelectedIndex < 0)
            {
                ShowInfo("No hay grupos de tareas", "Cree un grupo de tareas primero.", InfoBarSeverity.Informational);
                return;
            }

            if (SimplyTaskName.Text.Length == 0)
            {
                ShowInfo("Inserta un nombre de tarea", InfoBarSeverity.Informational);
                return;
            }

            if (m_tasks.NameExists(SimplyTaskName.Text) || SimplyTaskName.Text.Equals(m_tasks.Name))
            {
                ShowInfo("Ya existe un grupo o una tarea con ese nombre", InfoBarSeverity.Informational);
                return;
            }

            if (SaveUrl.Text.Length == 0)
            {
                ShowInfo("Indica un nombre de guardado", "Dado el caso de ser la ultima tarea seleccionada o no haber posibilidad de unirla al resto, el resultado se guardara en la ruta indicada.", InfoBarSeverity.Informational);
                return;
            }
            switch (TaskType.SelectedIndex)
            {
                case 0: // Dividir
                    if (OrigUrl.Text.Length == 0)
                    {
                        ShowInfo("Selecciona un archivo", InfoBarSeverity.Informational);
                        return;
                    }
                    if (!File.Exists(OrigUrl.Text))
                    {
                        ShowInfo("El archivo seleccionado no existe", InfoBarSeverity.Informational);
                        return;
                    }
                    
                    List<String> filesOrig1 = new()
                    {
                        m_filesOrig[0].Path
                    };
                    m_tasks.AddSimpleTask(new SimpleTask()
                    {
                        Name = SimplyTaskName.Text,
                        FileOrigPaths = filesOrig1,
                        FirstPage = (int)FirstPage.Value,
                        LastPage = (int)LastPage.Value,
                        FileResultPath = m_fileOut.Path,
                        TaskType = Type.Split
                    }, ListGroups.SelectedIndex);

                    //UpdateInfo();
                    break;
                case 1: // Unir
                    if (m_filesOrig.Count == 0)
                    {
                        ShowInfo("Selecciona al menos un archivo", InfoBarSeverity.Informational);
                        return;
                    }

                    List<String> filesOrig2 = new();
                    foreach (var v in m_filesOrig) filesOrig2.Add(v.Path);
                    m_tasks.AddSimpleTask(new SimpleTask()
                    {
                        Name = SimplyTaskName.Text,
                        FileOrigPaths = filesOrig2,
                        FirstPage = 0,
                        LastPage = 0,
                        FileResultPath = m_fileOut.Path,
                        TaskType = Type.Merge
                    }, ListGroups.SelectedIndex);

                    UpdateInfo();
                    break;
            }

            ShowInfo("Tarea añadida correctamente", 
                $"Tarea \"{SimplyTaskName.Text}\" añadida en el grupo \"{m_tasks.GetGroupByPosition(ListGroups.SelectedIndex).Name}\"", 
                InfoBarSeverity.Success);
            Debug.WriteLine(m_tasks.GroupTasks[ListGroups.SelectedIndex].Name);

            ClearInfo();
            UpdateInfo();

        }

        private void ClearInfo()
        {
            SimplyTaskName.Text = String.Empty;
            GroupName.Text = String.Empty;
            FirstPage.Text = String.Empty;
            LastPage.Text = String.Empty;
            m_fileOut = null;
            m_filesOrig.Clear();
            FilesList.ItemsSource = new ObservableCollection<string>();
            SaveUrl.Text = String.Empty;
            OrigUrl.Text = String.Empty;
        }

        private void ShowInfo(string title, string message, InfoBarSeverity type)
        {
            mux.InfoBar info = new()
            {
                Title = title,
                Message = message,
                IsOpen = true,
                Severity = type
            };
            if (ErrorPanel.Children.Count >= 3) ErrorPanel.Children.RemoveAt(0);
            ErrorPanel.Children.Add(info);
        }

        private void ShowInfo(string title, InfoBarSeverity type)
        {
            mux.InfoBar info = new()
            {
                Title = title,
                IsOpen = true,
                Severity = type,
            };
            if (ErrorPanel.Children.Count >= 3) ErrorPanel.Children.RemoveAt(0);
            ErrorPanel.Children.Add(info);
        }

        private async void OpenLocal_Click(object sender, RoutedEventArgs e)
        { 
            FileOpenPicker openPicker = new();

            Window window = new();

            var hwnd = WindowNative.GetWindowHandle(window);
            InitializeWithWindow.Initialize(openPicker, hwnd);
            openPicker.FileTypeFilter.Add(".pdf");
            StorageFile file = await openPicker.PickSingleFileAsync();
            if (m_filesOrig == null) m_filesOrig = new List<StorageFile>();
            if (file != null) m_filesOrig.Add(file);
            OrigUrl.Text = m_filesOrig[0].Path;

            PdfDocument pdf = null;
            if (file != null)
            {
                try
                {
                    pdf = await PdfDocument.LoadFromFileAsync(file);
                }
                catch
                {
                    ShowInfo("Archivo erroneo", "El archivo abierto esta protegido o no es un archivo Pdf (.pdf).", InfoBarSeverity.Error);
                    m_filesOrig.Clear();
                    return;
                }
            }

            if (pdf != null)
            {
                FirstPage.Maximum = pdf.PageCount;
                LastPage.Maximum = pdf.PageCount;
                FirstPage.Value = 1;
                LastPage.Value = pdf.PageCount;
            }
        }

        private void LastPage_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            FirstPage.Maximum = LastPage.Value;
        }

        private void FirstPage_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            LastPage.Minimum = FirstPage.Value;
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
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
                m_fileOut = file;
                SaveUrl.Text = m_fileOut.Path;
            }
        }

        private async void ListMach_Loaded(object sender, RoutedEventArgs e)
        {
            //if (m_controller == null) m_controller = new JsonController();
            m_tasks = new()
            {
                GroupTasks = new(),
                Name = "Tareas"
            };
            Tasks aux = await m_controller.LoadData();
            m_tasks = aux;
            UpdateInfo();
            LoadingData(false);
        }

        private async void OpenFiles_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker openPicker = new();

            Window window = new();

            var hwnd = WindowNative.GetWindowHandle(window);
            InitializeWithWindow.Initialize(openPicker, hwnd);
            openPicker.FileTypeFilter.Add(".pdf");
            var files = await openPicker.PickMultipleFilesAsync();
            if (m_filesOrig == null) m_filesOrig = new List<StorageFile>();
            ObservableCollection<Item> items = new ObservableCollection<Item>();

            foreach (var v in files)
            {
                if (v != null)
                {
                    m_filesOrig.Add(v as StorageFile);
                    
                }
            }

            foreach (var v in m_filesOrig) items.Add(new Item() { Name = v.Path });
            FilesList.ItemsSource = items;
        }
    }
}
