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
        Data class for a FilesListMerg
     */
    public class Item
    {
        public string Name { get; set; }
    }

    public sealed partial class CreaDoc : Page
    {
        private Tasks m_tasks;
        private List<String> m_filesOrig;
        private String m_fileOut;
        private SimpleTask m_actualEditTask;
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
            AddSimpleTask.Click += AddSimpleTask_Click;
            NavigationCacheMode = NavigationCacheMode.Enabled;
            InitializeData();
        }

        #region Data
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
            var selected = ListGroups.SelectedIndex;
            ListGroups.ItemsSource = GroupTasksList;
            if (selected == -1)
            {
                ListGroups.SelectedIndex = 0;
            }
            else
            {
                ListGroups.SelectedIndex = selected;
            }

            // Saving the actual configuration into the JSON file whith a thread
            _ = Task.Run(async () =>
            {
                if (m_controller != null) m_controller.SaveData(m_tasks);
            });

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
            if (m_filesOrig != null) m_filesOrig.Clear();
            FilesListMerg.ItemsSource = new ObservableCollection<string>();
            SaveUrl.Text = String.Empty;
            OrigFilePath.Text = String.Empty;
            Angle.Value = Constants.kDefaultAngle;
            Opacity.Value = Constants.kDefaultOpacity;
            FileDivPath.Text = String.Empty;
            TextMark.TextDocument.SetText(Microsoft.UI.Text.TextSetOptions.None, String.Empty);
        }

        //----------------------------------------------------------------------------------------------------------
        /*
            Check if the document given is a Pdf document
         */
        private static async Task<Boolean> PdfValidator(String path)
        {
            try
            {
                StorageFile file = await StorageFile.GetFileFromPathAsync(path);
                PdfDocument doc = await PdfDocument.LoadFromFileAsync(file);
                return true;
            }
            catch
            {
                return false;
            }
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

            int itemsDeleted = 0;

            // Removing the tasks from the system
            foreach (var task in ListMach.SelectedNodes)
            {
                m_tasks.RemoveItemsByName((string)task.Content);
                itemsDeleted++;
            }

            if (itemsDeleted > 0)
            {
                ShowInfo("Eliminacion tareas", "Tareas eliminadas correctamente", InfoBarSeverity.Informational);
            }
            else
            {
                ShowInfo("Eliminacion tareas", "No hay tareas seleccionadas", InfoBarSeverity.Informational);
            }
            UpdateData();

            LoadingData(false);
        }

        //----------------------------------------------------------------------------------------------------------
        /*
            Add new SimpleTask to the enviroment
         */
        private void AddSimpleTask_Click(object sender, RoutedEventArgs e)
        {

            #region GeneralChecks
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
            #endregion

            SimpleTask task = GetTaskInDisplay();

            if (task != null)
            {
                m_tasks.AddSimpleTask(task, ListGroups.SelectedIndex);
                ShowInfo("Tarea añadida correctamente",
                $"Tarea \"{SimplyTaskName.Text}\" añadida en el grupo \"{m_tasks.GetGroupByPosition(ListGroups.SelectedIndex).Name}\"",
                InfoBarSeverity.Success);

                // Setting up all the controls to the default state
                ClearInfo();

                // Updating the data
                UpdateData();
            }

        }

        //----------------------------------------------------------------------------------------------------------
        /*
            Method to handle the click event on the button when the edit mode is active
         */
        private void EditSimpleTask_Click(object sender, RoutedEventArgs e)
        {
            SimpleTask task = GetTaskInDisplay();

            if (task != null)
            {

                m_tasks.EditSimpleTaskByTask(task, m_actualEditTask);
                UpdateData();
                ShowInfo("Editar tarea",
                    "La tarea ha sido modificada correctamente",
                    InfoBarSeverity.Success);
            }
        }
        //----------------------------------------------------------------------------------------------------------
        /*
            Activates or disactivates the edit state depending on the Edit togle button state
         */
        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)Edit.IsChecked)
            {
                ListMach.SelectionMode = TreeViewSelectionMode.Single;
                ListMach.ItemInvoked += ListMach_ItemInvoked;

                AddSimpleTask.Click -= AddSimpleTask_Click;
                AddSimpleTask.Content = "Guardar tarea";
                AddSimpleTask.Click += EditSimpleTask_Click;
            }
            else
            {
                ListMach.SelectionMode = TreeViewSelectionMode.Multiple;
                UpdateData();
                ClearInfo();
                ListMach.ItemInvoked -= ListMach_ItemInvoked;

                AddSimpleTask.Click -= EditSimpleTask_Click;
                AddSimpleTask.Click += AddSimpleTask_Click;
                AddSimpleTask.Content = "Añadir tarea";

            }
        }

        //----------------------------------------------------------------------------------------------------------
        /*
            Loads in the controls the information of the SimpleTask selected
         */
        private void ListMach_ItemInvoked(TreeView sender, TreeViewItemInvokedEventArgs args)
        {
            var node = args.InvokedItem as mux.TreeViewNode;
            SimpleTask task = m_tasks.GetSimpleTaskByName((string)node.Content);
            m_actualEditTask = task;
            if (task != null)
            {
                ListGroups.SelectedIndex = task.Group;
                SimplyTaskName.Text = task.Name;

                switch (task.TaskType)
                {
                    case Type.Split:
                        TaskType.SelectedIndex = 0;
                        FileDivPath.Text = task.FileOrigPaths[0];
                        FirstPage.Value = task.FirstPage;
                        LastPage.Value = task.LastPage;
                        SaveUrl.Text = task.FileResultPath;
                        break;
                    case Type.Merge:
                        TaskType.SelectedIndex = 1;

                        ObservableCollection<Item> items = new();
                        m_filesOrig = new(task.FileOrigPaths);
                        foreach (var v in m_filesOrig)
                        {
                            items.Add(new Item()
                            {
                                Name = v
                            });
                        }
                        FilesListMerg.ItemsSource = items;

                        SaveUrl.Text = task.FileResultPath;
                        break;
                    case Type.WaterMark:
                        TaskType.SelectedIndex = 2;

                        OrigFilePath.Text = task.FileOrigPaths[0];
                        TextMark.TextDocument.SetText(Microsoft.UI.Text.TextSetOptions.None, task.TextWaterMark);
                        Angle.Value = DegreesFromRadians(task.AngleRadians);
                        Opacity.Value = (int)(task.Opacity * 100);
                        SaveUrl.Text = task.FileResultPath;
                        break;
                }
            }
        }

        //----------------------------------------------------------------------------------------------------------
        /*
            Returns new SimpleTask with the information in the display
         */
        private SimpleTask GetTaskInDisplay()
        {
            // Creating a task, params are different between the options
            switch (TaskType.SelectedIndex)
            {
                #region Split
                case 0: // Split

                    // User didn't selected the original file yet
                    if (FileDivPath.Text.Length == 0)
                    {
                        ShowInfo("Selecciona un archivo", InfoBarSeverity.Informational);
                        return null;
                    }

                    // User's selected Original file don't exists
                    if (!File.Exists(FileDivPath.Text))
                    {
                        ShowInfo("El archivo seleccionado no existe", InfoBarSeverity.Informational);
                        return null;
                    }

                    return new SimpleTask()
                    {
                        Name = SimplyTaskName.Text,
                        FileOrigPaths = new() { FileDivPath.Text },
                        FirstPage = (int)FirstPage.Value,
                        LastPage = (int)LastPage.Value,
                        FileResultPath = SaveUrl.Text,
                        TaskType = Type.Split,
                        Group = ListGroups.SelectedIndex
                    };
                #endregion
                #region Merge
                case 1: // Merge

                    // User didn't selected the original files yet
                    if (m_filesOrig.Count == 0)
                    {
                        ShowInfo("Selecciona al menos un archivo", InfoBarSeverity.Informational);
                        return null;
                    }

                    // List of paths from the list of files with the order selected
                    List<String> filesOrig2 = new();
                    foreach (Item v in FilesListMerg.Items) filesOrig2.Add(v.Name);

                    return new SimpleTask()
                    {
                        Name = SimplyTaskName.Text,
                        FileOrigPaths = filesOrig2,
                        FileResultPath = SaveUrl.Text,
                        TaskType = Type.Merge,
                        Group = ListGroups.SelectedIndex
                    };
                #endregion
                #region WaterMark
                case 2: // WaterMark
                    // There's not files selected
                    if (String.IsNullOrEmpty(OrigFilePath.Text))
                    {
                        ShowInfo("Selecciona un archivo", InfoBarSeverity.Informational);
                        return null;
                    }

                    // There's not text in the WaterMark
                    string textWaterMark;
                    TextMark.TextDocument.GetText(Microsoft.UI.Text.TextGetOptions.UseCrlf, out textWaterMark);
                    if (String.IsNullOrEmpty(textWaterMark))
                    {
                        ShowInfo("Introduce texto para la marca de agua", InfoBarSeverity.Informational);
                        return null;
                    }

                    return new SimpleTask()
                    {
                        Name = SimplyTaskName.Text,
                        FileOrigPaths = new() { OrigFilePath.Text },
                        FileResultPath = SaveUrl.Text,
                        TaskType = Type.WaterMark,
                        TextWaterMark = textWaterMark,
                        AngleRadians = RadiansFromDegree(Angle.Value),
                        Opacity = ((float)Opacity.Value / 100),
                        Group = ListGroups.SelectedIndex
                    };
                    #endregion
            }

            return null;
        }

        //----------------------------------------------------------------------------------------------------------
        /*
            Converts the angle given in degrees to radians
            From Degrees to Radians
         */
        private static double RadiansFromDegree(double degrees)
        {
            double angleResult = (360 - degrees) * Constants.kFactorDegreesToRadians;
            return angleResult;
        }

        //----------------------------------------------------------------------------------------------------------
        /*
            Converts the angle given in radians to degrees
            From Radians to Degrees
         */
        private static double DegreesFromRadians(double radians)
        {
            double radiansResult = (360 - (radians / Constants.kFactorDegreesToRadians));
            return radiansResult;
        }

        //----------------------------------------------------------------------------------------------------------
        /*
            Open a FileOpenPicker to select the file where the result is going to be saved
         */
        private async void SaveFileSelect_Click(object sender, RoutedEventArgs e)
        {
            // Setting the FileSavePicker
            var savePicker = new FileSavePicker();
            savePicker.FileTypeChoices.Add("Portable Document Format", new List<string>() { ".pdf" });
            if (!String.IsNullOrEmpty(SimplyTaskName.Text))
            {
                savePicker.SuggestedFileName = SimplyTaskName.Text;
            }
            else
            {
                savePicker.SuggestedFileName = "New Document";
            }

            Window window = new();
            var hwnd = WindowNative.GetWindowHandle(window);
            InitializeWithWindow.Initialize(savePicker, hwnd);
            StorageFile file = await savePicker.PickSaveFileAsync();
            if (file != null)
            {
                m_fileOut = file.Path;
                SaveUrl.Text = m_fileOut;
            }

            // Deleting the file created by default 
            //File.Delete(m_fileOut.Path);
        }

        //----------------------------------------------------------------------------------------------------------
        /*
            When the TreeView is loaded, get the data from the configuration file
         */
        private async void ListMach_Loaded(object sender, RoutedEventArgs e)
        {
            // Creating new Object of Tasks
            m_tasks = Tasks.GetDefaultTasks();

            // Get the data from the configure file by asynchronous task
            Tasks aux = await m_controller.LoadData();
            m_tasks = aux;
            // Updating the UI's data
            UpdateData();
            LoadingData(false);
        }

        //----------------------------------------------------------------------------------------------------------
        /*
            Executes the tasks selected 
         */
        private async void Execute_Click(object sender, RoutedEventArgs e)
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
                List<String> results = new();
                List<String> errors = new();
                LoadingData(true);
                await Task.Run(async () =>
                {
                    foreach (var x in tasksSelected)
                    {
                        #region Documents validation

                        foreach (var j in x.FileOrigPaths)
                        {
                            Boolean result = await PdfValidator(j);
                            if (!result)
                            {
                                errors.Add(j);
                            }
                        }
                        if (errors.Count > 0) break;

                        #endregion

                        switch (x.TaskType)
                        {
                            #region Split
                            case Type.Split:

                                try
                                {
                                    PdfLibrary.Splitter.SplitPdfByPage(x.FileOrigPaths[0],
                                    x.FileResultPath,
                                    x.FirstPage,
                                    x.LastPage);
                                    results.Add(x.FileResultPath);
                                } catch 
                                {
                                    errors.Add(x.FileOrigPaths[0]);
                                }

                                break;
                            #endregion
                            #region Merge
                            case Type.Merge:

                                try
                                {
                                    // If merge only has 1 page, it will be the result
                                    if (x.FileOrigPaths.Count == 1)
                                    {
                                        FileInfo i = new(x.FileOrigPaths[0]);
                                        i.CopyTo(x.FileResultPath, true);
                                        results.Add(x.FileResultPath);
                                        break;
                                    }

                                    PdfLibrary.Merger.Merge(x.FileOrigPaths[0],
                                        x.FileResultPath,
                                        x.FileOrigPaths.GetRange(1, x.FileOrigPaths.Count - 1));
                                    results.Add(x.FileResultPath);
                                } catch
                                {
                                    errors.Add(x.FileOrigPaths[0]);
                                }

                                break;
                            #endregion
                            #region Watermark
                            case Type.WaterMark:
                                try
                                {
                                    PdfLibrary.WaterMark.AddWaterMark(x.FileOrigPaths[0],
                                    x.FileResultPath,
                                    x.TextWaterMark,
                                    x.AngleRadians,
                                    x.Opacity);
                                    results.Add(x.FileResultPath);
                                }
                                catch
                                {
                                    errors.Add(x.FileOrigPaths[0]);
                                }
                                
                                break;
                                #endregion
                        }
                    }
                });

                #region ResultsInfo
                // Showing up a message every time file is saved
                results.ForEach(x =>
                {
                    ShowInfo("Tareas ejecutadas",
                        $"Documento guardado en \"{x}\"",
                        "Abrir archivo",
                        x,
                        InfoBarSeverity.Success);
                });
                #endregion

                #region ResultsError
                // Showing up a error message
                errors.ForEach(x =>
                {
                    ShowInfo("Error en documento",
                        $"El documento \"{x}\" es erroneo o ya no existe, ejecucion cancelada",
                        InfoBarSeverity.Error);
                });
                #endregion
                LoadingData(false);
            }
        }

        //----------------------------------------------------------------------------------------------------------
        /*
            When changes type of SimpleTask clear the files selected
         */
        private void TaskType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (m_filesOrig != null) m_filesOrig.Clear();
        }

        #endregion

        #region Split
        //----------------------------------------------------------------------------------------------------------
        /*
            Open a FileOpenPicker to select the original file in the Split part
         */
        private async void OpenFileDiv_Click(object sender, RoutedEventArgs e)
        {
            // Initialize and show new OpenFilePicker
            FileOpenPicker openPicker = new();
            Window window = new();
            var hwnd = WindowNative.GetWindowHandle(window);
            InitializeWithWindow.Initialize(openPicker, hwnd);
            openPicker.FileTypeFilter.Add(".pdf");
            StorageFile file = await openPicker.PickSingleFileAsync();
            if (m_filesOrig == null) m_filesOrig = new List<String>();
            if (file != null)
            {
                m_filesOrig.Add(file.Path);
                FileDivPath.Text = m_filesOrig[0];
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
        #endregion

        #region Merge
        //----------------------------------------------------------------------------------------------------------
        /*
            Open a FileOpenPicker to select all the files to merge
         */
        private async void OpenFilesMerg_Click(object sender, RoutedEventArgs e)
        {
            // Setting up and showing up it
            FileOpenPicker openPicker = new();
            Window window = new();
            var hwnd = WindowNative.GetWindowHandle(window);
            InitializeWithWindow.Initialize(openPicker, hwnd);
            openPicker.FileTypeFilter.Add(".pdf");
            var files = await openPicker.PickMultipleFilesAsync();
            if (m_filesOrig == null) m_filesOrig = new List<String>();

            // Creating a ObservableCollection for the ListView
            ObservableCollection<Item> items = new();

            foreach (var v in files)
            {
                if (v != null)
                {
                    m_filesOrig.Add(v.Path);
                }
            }

            // Adding the files selected to the ObservableCollection
            foreach (var v in m_filesOrig) items.Add(new Item() { Name = v });
            FilesListMerg.ItemsSource = items;
        }

        //----------------------------------------------------------------------------------------------------------
        /*
            Remove the files from FilesListMerg that are selected, 
            if there's no one selected, removes all from the list
         */
        private void DeleteFileSelected_Click(object sender, RoutedEventArgs e)
        {
            List<Item> filesSelected = new();
            foreach (Item v in FilesListMerg.SelectedItems) filesSelected.Add(v);
            List<Item> items = new();
            if (filesSelected.Count > 0)
            {
                foreach (Item item in FilesListMerg.Items) items.Add(item);

                foreach (var item in filesSelected) items.Remove(item);
            }
            FilesListMerg.ItemsSource = items;
        }
        #endregion

        #region WaterMark
        //----------------------------------------------------------------------------------------------------------
        /*
            Open a file to add a WaterMark
         */
        private async void OpenFileMark_Click(object sender, RoutedEventArgs e)
        {
            // Initialize and show new OpenFilePicker
            FileOpenPicker openPicker = new();
            Window window = new();
            var hwnd = WindowNative.GetWindowHandle(window);
            InitializeWithWindow.Initialize(openPicker, hwnd);
            openPicker.FileTypeFilter.Add(".pdf");
            StorageFile file = await openPicker.PickSingleFileAsync();
            if (m_filesOrig == null) m_filesOrig = new List<String>();

            if (file != null)
            {
                m_filesOrig.Add(file.Path);
                OrigFilePath.Text = file.Path;
            }
        }

        //----------------------------------------------------------------------------------------------------------
        /*
            Updates the rotation of the show test whe the number box value change
         */
        private void Angle_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            if (Angle.IsLoaded && ShowText.IsLoaded)
            {
                ShowText.Rotation = (float)args.NewValue;
            }
        }

        //----------------------------------------------------------------------------------------------------------
        /*
            Updates the opacity of the show text when the number box value change
         */
        private void Opacity_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            if (Opacity.IsLoaded && ShowText.IsLoaded)
            {
                Double opacity = (Double)args.NewValue;
                if (opacity != 0)
                {
                    opacity /= 100;
                }
                ShowText.Opacity = opacity;
            }
        }
        #endregion

        #region Messages
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

            // Removing the most older InfoBar from the Error Panel when there are more than the Constants.kMaxMessages value
            if (ErrorPanel.Children.Count >= Constants.kMaxMessages) ErrorPanel.Children.RemoveAt(0);

            // Adding the InfoBar to the ErrorPanel
            ErrorPanel.Children.Add(info);

            // Creating a Asyncronus Operation to remove the InfoBar added before
            await Task.Run(() => Thread.Sleep(Constants.kTimeToHideMessages));
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

            // Removing the most older InfoBar from the Error Panel when there are more than the Constants.kMaxMessages value
            if (ErrorPanel.Children.Count >= Constants.kMaxMessages) ErrorPanel.Children.RemoveAt(0);

            // Adding the InfoBar to the ErrorPanel
            ErrorPanel.Children.Add(info);

            // Creating a Asyncronus Operation to remove the InfoBar added before
            await Task.Run(() => Thread.Sleep(Constants.kTimeToHideMessages));
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

            // Removing the most older InfoBar from the Error Panel when there are more than the Constants.kMaxMessages value
            if (ErrorPanel.Children.Count >= Constants.kMaxMessages) ErrorPanel.Children.RemoveAt(0);

            // Adding the InfoBar to the ErrorPanel
            ErrorPanel.Children.Add(info);

            // Creating a Asyncronus Operation to remove the InfoBar added before
            await Task.Run(() => Thread.Sleep(Constants.kTimeToHideMessages));
            ErrorPanel.Children.Remove(info);
        }
        #endregion




    }
}
