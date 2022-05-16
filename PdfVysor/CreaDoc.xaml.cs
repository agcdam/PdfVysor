using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Windows.Data.Pdf;
using Windows.Storage;
using Windows.Storage.Pickers;
using WinRT.Interop;
using mux = Microsoft.UI.Xaml.Controls;

namespace PdfVysor
{

    //----------------------------------------------------------------------------------------------------------
    /*
        Data class for a FilesList
     */
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

        private const int kMaxMessages = 4;
        private const int kTimeToHideMessages = 6500;

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

        //----------------------------------------------------------------------------------------------------------
        /*
            Set up the enviroment while data is loading
         */
        private void LoadingData(Boolean status)
        {
            ProgressStatus.IsActive = status;
            ListMach.IsEnabled = !status;
        }

        //----------------------------------------------------------------------------------------------------------
        /*
            Update the data of the UI
         */
        private void UpdateData()
        {
            // Remove all the nodes of the treeview
            ListMach.RootNodes.Clear();

            // Create new nodes for the treeview
            mux.TreeViewNode tasks = new() { Content = m_tasks.Name };
            foreach (GroupTask gTask in m_tasks.GroupTasks)
            {
                // Adding children nodes to the root node
                mux.TreeViewNode node = new() { Content = gTask.Name };
                if (gTask.Tasks != null) 
                    foreach (SimpleTask simpleTask in gTask.Tasks) 
                        node.Children.Add(new() { Content = simpleTask.Name });
                node.IsExpanded = true;
                tasks.Children.Add(node);
            }
            tasks.IsExpanded = true;
            // Adding the root node to the treeview
            ListMach.RootNodes.Add(tasks);

            // If there's no groups in tasks, the header of ListBox is collapsed, otherwise the header of ListBox is visible
            HeaderGropuTasks.Visibility = (m_tasks.GroupTasks.Count > 0) ? Visibility.Visible : Visibility.Collapsed;

            // Setting the itemsource to the ListBox
            ListGroups.ItemsSource = GroupTasksList;
            if (ListGroups.SelectedIndex == -1) ListGroups.SelectedIndex = 0;

            // Saving the actual configuration into the JSON file whith a thread
            _ = Task.Run(async () =>
            {
                if (m_controller != null) m_controller.SaveData(m_tasks);
            });
        }

        //----------------------------------------------------------------------------------------------------------
        /*
            Add new GroupTask to the enviroment
         */
        private void AddGroup_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Checking if exists a group with that name yet
                if (m_tasks.NameExists(GroupName.Text) || SimplyTaskName.Text.Equals(m_tasks.Name))
                {
                    ShowInfo("Ya existe un grupo con ese nombre", InfoBarSeverity.Informational);
                    return;
                }

                // Add new GroupTask with the name given
                m_tasks.AddGroupTasks(GroupName.Text);
                UpdateData();
            }
            catch // Name of the group is null or empty
            {
                ShowInfo("Nombre de grupo vacio", InfoBarSeverity.Informational);
            }

        }

        //----------------------------------------------------------------------------------------------------------
        /*
            Deletes the tasks selected
         */
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            LoadingData(true);
            FlyOutDelete.Hide();

            // Removing the tasks from the system
            foreach (var task in ListMach.SelectedNodes)
            {
                m_tasks.RemoveItemsByName((string)task.Content);
            }

            ShowInfo("Eliminacion tareas", "Tareas eliminadas correctamente", InfoBarSeverity.Informational);
            UpdateData();

            LoadingData(false);
        }

        //----------------------------------------------------------------------------------------------------------
        /*
            Add new SimpleTask to the enviroment
         */
        private void AddSimpleTask_Click(object sender, RoutedEventArgs e)
        {
            // There's no Groups in the system
            if (ListGroups.SelectedIndex < 0)
            {
                ShowInfo("No hay grupos de tareas", "Cree un grupo de tareas primero.", InfoBarSeverity.Informational);
                return;
            }

            // The name of the SimpleTask is empty
            if (String.IsNullOrEmpty(SimplyTaskName.Text))
            {
                ShowInfo("Inserta un nombre de tarea", InfoBarSeverity.Informational);
                return;
            }

            // Already exists a task with that name
            if (m_tasks.NameExists(SimplyTaskName.Text) || SimplyTaskName.Text.Equals(m_tasks.Name))
            {
                ShowInfo("Ya existe un grupo o una tarea con ese nombre", InfoBarSeverity.Informational);
                return;
            }

            // User didn't selected a save file yet
            if (SaveUrl.Text.Length == 0)
            {
                ShowInfo("Indica un nombre de guardado", 
                    "Dado el caso de ser la ultima tarea seleccionada o no haber posibilidad de unirla al resto, el resultado se guardara en la ruta indicada.",
                    InfoBarSeverity.Informational);
                return;
            }

            // Creating a task, params are different between the options
            switch (TaskType.SelectedIndex)
            {
                case 0: // Dividir

                    // User didn't selected the original file yet
                    if (OrigUrl.Text.Length == 0)
                    {
                        ShowInfo("Selecciona un archivo", InfoBarSeverity.Informational);
                        return;
                    }

                    // User's selected Original file don't exists
                    if (!File.Exists(OrigUrl.Text))
                    {
                        ShowInfo("El archivo seleccionado no existe", InfoBarSeverity.Informational);
                        return;
                    }

                    // Adding new SimpleTask
                    m_tasks.AddSimpleTask(new SimpleTask()
                    {
                        Name = SimplyTaskName.Text,
                        FileOrigPaths = new() { m_filesOrig[0].Path },
                        FirstPage = (int)FirstPage.Value,
                        LastPage = (int)LastPage.Value,
                        FileResultPath = m_fileOut.Path,
                        TaskType = Type.Split
                    }, ListGroups.SelectedIndex);

                    break;
                case 1: // Unir

                    // User didn't selected the original files yet
                    if (m_filesOrig.Count == 0)
                    {
                        ShowInfo("Selecciona al menos un archivo", InfoBarSeverity.Informational);
                        return;
                    }

                    // List of paths from the originals files
                    List<String> filesOrig2 = new();
                    foreach (var v in m_filesOrig) filesOrig2.Add(v.Path);

                    // Adding new Simpletask
                    m_tasks.AddSimpleTask(new SimpleTask()
                    {
                        Name = SimplyTaskName.Text,
                        FileOrigPaths = filesOrig2,
                        FirstPage = 0,
                        LastPage = 0,
                        FileResultPath = m_fileOut.Path,
                        TaskType = Type.Merge
                    }, ListGroups.SelectedIndex);

                    break;
            }

            ShowInfo("Tarea añadida correctamente",
                $"Tarea \"{SimplyTaskName.Text}\" añadida en el grupo \"{m_tasks.GetGroupByPosition(ListGroups.SelectedIndex).Name}\"",
                InfoBarSeverity.Success);

            // Setting up all the controls to the default state
            ClearInfo();

            // Updating the data
            UpdateData();

        }

        //----------------------------------------------------------------------------------------------------------
        /*
            Clear all the controls
         */
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

        //----------------------------------------------------------------------------------------------------------
        /*
            Shows a InfoBar with a message and a button to open a file giving the path
         */
        private async void ShowInfo(string title, string message, string buttonContent, string pathFile, InfoBarSeverity type)
        {
            // Creates the button
            Button button = new()
            {
                Name = "OpenFileResult",
                Content = buttonContent,
                Tag = pathFile
            };
            // Linking the button to a handler
            button.Click += new RoutedEventHandler(OpenFileResult_Click);

            // Create the infobar with the params given
            mux.InfoBar info = new()
            {
                Title = title,
                Message = message,
                IsOpen = true,
                Severity = type,
                ActionButton = button
            };

            // Removing the most older InfoBar from the Error Panel when there are more than the kMaxMessages value
            if (ErrorPanel.Children.Count >= kMaxMessages) ErrorPanel.Children.RemoveAt(0);

            // Adding the InfoBar to the ErrorPanel
            ErrorPanel.Children.Add(info);

            // Creating a Asyncronus Operation to remove the InfoBar added before
            await Task.Run(() => Thread.Sleep(kTimeToHideMessages));
            ErrorPanel.Children.Remove(info);
        }

        //----------------------------------------------------------------------------------------------------------
        /*
            Open the default App to read Pdf documents of the system to visualize the document created
         */
        private async void OpenFileResult_Click(object sender, RoutedEventArgs e)
        {
            // Getting the path of the file from the Tag of the button
            var btn = sender as Button;
            if (btn == null) return;

            // Opening the file that represents the path
            var file = await StorageFile.GetFileFromPathAsync(btn.Tag.ToString());

            // Calling the System to open the File
            Windows.System.Launcher.LaunchFileAsync(file);
        }

        //----------------------------------------------------------------------------------------------------------
        /*
            Shows a InfoBar with a message
         */
        private async void ShowInfo(string title, string message, InfoBarSeverity type)
        {
            // Create the infobar with the params given
            mux.InfoBar info = new()
            {
                Title = title,
                Message = message,
                IsOpen = true,
                Severity = type
            };

            // Removing the most older InfoBar from the Error Panel when there are more than the kMaxMessages value
            if (ErrorPanel.Children.Count >= kMaxMessages) ErrorPanel.Children.RemoveAt(0);

            // Adding the InfoBar to the ErrorPanel
            ErrorPanel.Children.Add(info);

            // Creating a Asyncronus Operation to remove the InfoBar added before
            await Task.Run(() => Thread.Sleep(kTimeToHideMessages));
            ErrorPanel.Children.Remove(info);
        }

        //----------------------------------------------------------------------------------------------------------
        /*
            Shows a basic InfoBar
         */
        private async void ShowInfo(string title, InfoBarSeverity type)
        {
            // Create the infobar with the params given
            mux.InfoBar info = new()
            {
                Title = title,
                IsOpen = true,
                Severity = type,
            };

            // Removing the most older InfoBar from the Error Panel when there are more than the kMaxMessages value
            if (ErrorPanel.Children.Count >= kMaxMessages) ErrorPanel.Children.RemoveAt(0);

            // Adding the InfoBar to the ErrorPanel
            ErrorPanel.Children.Add(info);

            // Creating a Asyncronus Operation to remove the InfoBar added before
            await Task.Run(() => Thread.Sleep(kTimeToHideMessages));
            ErrorPanel.Children.Remove(info);
        }

        //----------------------------------------------------------------------------------------------------------
        /*
            Open a FileOpenPicker to select the original file in the Split part
         */
        private async void OpenOrig_Click(object sender, RoutedEventArgs e)
        {
            // Initialize and show new OpenFilePicker
            FileOpenPicker openPicker = new();
            Window window = new();
            var hwnd = WindowNative.GetWindowHandle(window);
            InitializeWithWindow.Initialize(openPicker, hwnd);
            openPicker.FileTypeFilter.Add(".pdf");
            StorageFile file = await openPicker.PickSingleFileAsync();
            if (m_filesOrig == null) m_filesOrig = new List<StorageFile>();
            if (file != null)
            {
                m_filesOrig.Add(file);
                OrigUrl.Text = m_filesOrig[0].Path;
            }


            if (file != null)
            {
                try
                {
                    // Getting the pages of the document
                    PdfDocument pdf = null;
                    pdf = await PdfDocument.LoadFromFileAsync(file);

                    // Setting limits to the NumberBox 
                    if (pdf != null)
                    {
                        FirstPage.Maximum = pdf.PageCount;
                        LastPage.Maximum = pdf.PageCount;
                        FirstPage.Value = 1;
                        LastPage.Value = pdf.PageCount;
                    }
                }
                catch // The file is wrong or its protected
                {
                    ShowInfo("Archivo erroneo", "El archivo abierto esta protegido o no es un archivo Pdf (.pdf).", InfoBarSeverity.Error);
                    m_filesOrig.Clear();
                    return;
                }
            }


        }

        //----------------------------------------------------------------------------------------------------------
        /*
            Every time change the LastPage value, updates maximum limit to the FirstPage
         */
        private void LastPage_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            FirstPage.Maximum = LastPage.Value;
        }

        //----------------------------------------------------------------------------------------------------------
        /*
            Every time change the FirstPage value, updates minimum limit to the LastPage
         */
        private void FirstPage_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            LastPage.Minimum = FirstPage.Value;
        }

        //----------------------------------------------------------------------------------------------------------
        /*
            Open a FileOpenPicker to select the file where the result is going to be saved
         */
        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Setting the FileSavePicker
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

            // Deleting the file created by default 
            File.Delete(m_fileOut.Path);
        }

        //----------------------------------------------------------------------------------------------------------
        /*
            When the TreeView is loaded, get the data from the configuration file
         */
        private async void ListMach_Loaded(object sender, RoutedEventArgs e)
        {
            // Creating new Object of Tasks
            m_tasks = new()
            {
                GroupTasks = new(),
                Name = "Tareas"
            };

            // Get the data from the configure file by asynchronous task
            Tasks aux = await m_controller.LoadData();
            m_tasks = aux;

            // Updating the UI's data
            UpdateData();
            LoadingData(false);
        }

        //----------------------------------------------------------------------------------------------------------
        /*
            Open a FileOpenPicker to select all the files to merge
         */
        private async void OpenFiles_Click(object sender, RoutedEventArgs e)
        {
            // Setting up and showing up it
            FileOpenPicker openPicker = new();
            Window window = new();
            var hwnd = WindowNative.GetWindowHandle(window);
            InitializeWithWindow.Initialize(openPicker, hwnd);
            openPicker.FileTypeFilter.Add(".pdf");
            var files = await openPicker.PickMultipleFilesAsync();
            if (m_filesOrig == null) m_filesOrig = new List<StorageFile>();

            // Creating a ObservableCollection for the ListView
            ObservableCollection<Item> items = new();

            foreach (var v in files)
            {
                if (v != null)
                {
                    m_filesOrig.Add(v as StorageFile);
                }
            }

            // Adding the files selected to the ObservableCollection
            foreach (var v in m_filesOrig) items.Add(new Item() { Name = v.Path });
            FilesList.ItemsSource = items;
        }

        //----------------------------------------------------------------------------------------------------------
        /*
            Executes the tasks selected 
         */
        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            // Getting the tasks selected
            List<SimpleTask> tasksSelected = new();
            var nodes = ListMach.SelectedNodes;
            foreach (var x in nodes)
            {
                SimpleTask task = m_tasks.GetSimpleTaskByName((string)x.Content);
                if (task != null) tasksSelected.Add(task);
            }

            if (tasksSelected.Count > 0)
            {
                // Every time a file is saved, show up new message
                List<String> results = new();
                LoadingData(true);
                await Task.Run(() =>
                {
                    // Setting the temp file
                    String origFileTemp = Path.Combine(Path.GetTempPath(), "origFileTemp.pdf");
                    String resultFileTemp = Path.Combine(Path.GetTempPath(), "destFileTemp.pdf");

                    //File.Create(resultFileTemp);
                    foreach (var x in tasksSelected)
                    {
                        switch (x.TaskType)
                        {
                            case Type.Split:

                                if (tasksSelected.IndexOf(x) == 0)
                                {
                                    // If it is the first task getting the orig file from the selected file from user
                                    PdfLibrary.Splitter.SplitPdfByPage(x.FileOrigPaths[0], resultFileTemp, x.FirstPage, x.LastPage);
                                }
                                else
                                {
                                    // If it isn't the first task getting the orig file from the result of the previous task
                                    PdfLibrary.Splitter.SplitPdfByPage(origFileTemp, resultFileTemp, x.FirstPage, x.LastPage);
                                }

                                break;
                            case Type.Merge:

                                if (tasksSelected.IndexOf(x) == 0)
                                {
                                    // If merge only has 1 orig file, that file will be the result
                                    if (x.FileOrigPaths.Count == 1)
                                    {
                                        FileInfo i = new(x.FileOrigPaths[0]);
                                        i.CopyTo(resultFileTemp, true);
                                        break;
                                    }

                                    // If it's the first task, the orig file will be the first
                                    String fileOrig = x.FileOrigPaths[0];
                                    List<String> files = new(x.FileOrigPaths);
                                    files.RemoveAt(0);
                                    PdfLibrary.Merger.Merge(fileOrig, resultFileTemp, files);
                                }
                                else
                                {
                                    // If it isn't the first task, the orig file will be the result of the previous tasks
                                    PdfLibrary.Merger.Merge(origFileTemp, resultFileTemp, x.FileOrigPaths);
                                }

                                break;
                        }

                        FileInfo info = new(resultFileTemp);
                        if (tasksSelected.IndexOf(x) == (tasksSelected.Count - 1) || tasksSelected[tasksSelected.IndexOf(x) + 1].TaskType == Type.Split)
                        {
                            // If is the last task, copy the result to the user's selected path or the following task is split
                            info.CopyTo(x.FileResultPath, true);
                            results.Add(x.FileResultPath);
                        }
                        else
                        {
                            // Set the result path like orig path for the next task
                            info.CopyTo(origFileTemp, true);
                        }

                    }
                });

                // Showing a message every time file is saved
                results.ForEach(x =>
                {
                    ShowInfo("Tareas ejecutadas",
                        $"Documento guardado en \"{x}\"",
                        "Abrir archivo",
                        x,
                        InfoBarSeverity.Success);
                });
                LoadingData(false);
            }

        }
    }
}
