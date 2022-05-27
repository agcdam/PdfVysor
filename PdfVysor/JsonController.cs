using System;
using System.IO;
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
        private StorageFile m_jsonFile;
        private StorageFolder m_localFolder;


        public JsonController()
        {
            OpenFolder();
        }

        /// <summary>
        /// Opens <see cref="m_localFolder"/> when a new instance of <see cref="JsonController"/> is created
        /// </summary>
        private async void OpenFolder()
        {
            m_localFolder = await StorageFolder.GetFolderFromPathAsync(Path.GetTempPath());
        }

        /// <summary>
        /// Deserialize the JSON file <see cref="m_jsonFile"/> into <see cref="Tasks"/>
        /// </summary>
        /// <returns><see cref="Task"/> with a <see cref="Tasks"/> object</returns>
        public async Task<Tasks> LoadData()
        {
            await OpenFile();
            String data = await ReadData();
            Tasks task;
            if (String.IsNullOrEmpty(data))
            {
                task = Tasks.GetDefaultTasks();
            }
            else
            {
                task = JsonSerializer.Deserialize<Tasks>(data);
            }
            return task;
        }

        /// <summary>
        /// Serialize <paramref name="tasks"/> to JSON and write it into <see cref="m_jsonFile"/>
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
        /// Obtain the data from <see cref="m_jsonFile"/>
        /// </summary>
        /// <returns><see cref="Task"/> with a <see cref="String"/> where is the data located</returns>
        private async Task<String> ReadData()
        {
            await OpenFile();
            String data = await FileIO.ReadTextAsync(m_jsonFile);
            return data;
        }

        /// <summary>
        /// Write in <see cref="m_jsonFile"/> the data given in <paramref name="data"/>
        /// </summary>
        /// <param name="data"></param>
        private async void WriteData(String data)
        {
            if (string.IsNullOrEmpty(data)) return;
            await OpenFile();
            await FileIO.WriteTextAsync(m_jsonFile, data);
        }

        /// <summary>
        /// Open <see cref="m_localFolder"/> and open <see cref="m_jsonFile"/>
        /// </summary>
        /// <returns></returns>
        private async Task OpenFile()
        {
            if (m_localFolder == null) OpenFolder();
            try
            {
                if (m_jsonFile == null)
                {
                    m_jsonFile = await m_localFolder.GetFileAsync(Constants.kFileJsonName);
                }
            }
            catch
            {
                m_jsonFile = await m_localFolder.CreateFileAsync(Constants.kFileJsonName, CreationCollisionOption.ReplaceExisting);
            }
        }
    }
}
