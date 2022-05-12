using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Storage;
using System.Text.Json;

namespace PdfVysor
{
    internal class JsonController
    {
        private const string kFileName = "ConfigTasks.json";
        private StorageFile m_jsonFile;
        private StorageFolder m_localFolder;
        public JsonController()
        {
            m_localFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
        }

        public async Task<Tasks> LoadData()
        {
            await OpenFile();
            String data = await ReadData();
            Debug.WriteLine(data + "----------");
            Tasks task;
            if (String.IsNullOrEmpty(data))
            {
                task = new()
                {
                    Name = "Tareas",
                    GroupTasks = new()
                };

            } else
            {
                task = JsonSerializer.Deserialize<Tasks>(data);
            }

            Debug.WriteLine(task.Name + "Nombre maquinaaaaa");
            //Tasks tasks = JsonSerializer.Deserialize<Tasks>(data);
            ////if (tasks == null) tasks = new Tasks() { Name = "Tareas", GroupTasks = new() };
            return task;
        }

        public async void SaveData(Tasks tasks)
        {
            await Task.Run(() =>
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                String dataJson = JsonSerializer.Serialize<Tasks>(tasks, options);
                WriteData(dataJson);
            });
        }

        //public async Task<Tasks> LoadData()
        //{
        //    if (m_localFolder == null)
        //    {
        //        m_localFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
        //    }
        //    if (m_jsonFile == null)
        //    {
        //        m_jsonFile = await m_localFolder.GetFileAsync(kFileName);
        //    }
        //    //String dataJson = await ReadData();
        //    //Tasks tasks = JsonSerializer.Deserialize<Tasks>(dataJson);
        //    //return tasks;
        //}

        private async Task<String> ReadData()
        {
            if (m_jsonFile == null) await OpenFile();
            String data = await FileIO.ReadTextAsync(m_jsonFile);
            return data;
        }

        private async void WriteData(String data)
        {
            if (string.IsNullOrEmpty(data)) return;
            if (m_jsonFile == null) await OpenFile();
            await FileIO.WriteTextAsync(m_jsonFile, data);
        }

        private async Task OpenFile()
        {
            if (m_localFolder == null) m_localFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
            try
            {
                m_jsonFile = await m_localFolder.GetFileAsync(kFileName);
                Debug.WriteLine("Archivo json existe------");
            }
            catch
            {
                Debug.WriteLine("Creando archivo json-----");
                m_jsonFile = await m_localFolder.CreateFileAsync(kFileName, CreationCollisionOption.ReplaceExisting);
            }


        }
    }
}
