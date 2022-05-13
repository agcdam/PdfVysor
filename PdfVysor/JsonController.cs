using System;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.Storage;

namespace PdfVysor
{

    /// <summary>
    /// Class to control the data between the program and the configutarion JSON file
    /// </summary>
    internal class JsonController
    {
        private const string kFileName = "ConfigTasks.json";
        private StorageFile m_jsonFile;
        private StorageFolder m_localFolder;

        /// <summary>
        /// Opens the local folder when a new instance of JsonController is created
        /// </summary>
        public JsonController()
        {
            m_localFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
        }

        /// <summary>
        /// Open the file and obtain the data deserializing it into Tasks objects
        /// </summary>
        /// <returns>Task with a Tasks object</returns>
        public async Task<Tasks> LoadData()
        {
            await OpenFile();
            String data = await ReadData();
            Tasks task;
            if (String.IsNullOrEmpty(data))
            {
                task = new()
                {
                    Name = "Tareas",
                    GroupTasks = new()
                };

            }
            else
            {
                task = JsonSerializer.Deserialize<Tasks>(data);
            }
            return task;
        }

        /// <summary>
        /// Serialize the object given to JSON and write it into the configuration file
        /// </summary>
        /// <param name="tasks"></param>
        public async void SaveData(Tasks tasks)
        {
            await OpenFile();
            await Task.Run(() =>
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                String dataJson = JsonSerializer.Serialize<Tasks>(tasks, options);
                WriteData(dataJson);
            });
        }

        /// <summary>
        /// Open the file and obtain the data
        /// </summary>
        /// <returns>Task with a String where is the data located</returns>
        private async Task<String> ReadData()
        {
            await OpenFile();
            String data = await FileIO.ReadTextAsync(m_jsonFile);
            return data;
        }

        /// <summary>
        /// Write in the configuration file the data given in String
        /// </summary>
        /// <param name="data"></param>
        private async void WriteData(String data)
        {
            if (string.IsNullOrEmpty(data)) return;
            await OpenFile();
            await FileIO.WriteTextAsync(m_jsonFile, data);
        }

        /// <summary>
        /// Open the folder and open the configuration file
        /// </summary>
        /// <returns></returns>
        private async Task OpenFile()
        {
            if (m_localFolder == null) m_localFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
            try
            {
                if (m_jsonFile == null)
                {
                    m_jsonFile = await m_localFolder.GetFileAsync(kFileName);
                }
            }
            catch
            {
                m_jsonFile = await m_localFolder.CreateFileAsync(kFileName, CreationCollisionOption.ReplaceExisting);
            }
        }
    }
}
