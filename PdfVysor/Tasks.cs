using System;
using System.Collections.Generic;
using Windows.Storage;

namespace PdfVysor
{
    public enum Type { Split, Merge}
    public class Tasks
    {
        public String Name { set; get; }
        private List<GroupTask> m_tasks;
        public List<GroupTask> GroupTasks
        {
            get
            {
                if (m_tasks == null) m_tasks = new List<GroupTask>();
                return m_tasks;
            }
            set => m_tasks = value;
        }
        public Tasks() { }

        public void AddGroupTasks(String name, List<SimpleTask> simpleTasks)
        {
            if (name == null) throw new ArgumentNullException();
            if (name.Length == 0) throw new ArgumentOutOfRangeException();
            if (simpleTasks == null) throw new ArgumentNullException();
            GroupTask gp = new()
            {
                Name = name,
                Tasks = simpleTasks
            };
            if (m_tasks == null) m_tasks = new List<GroupTask> { };
            m_tasks.Add(gp);
        }

        public void AddGroupTasks(String name)
        {
            try { AddGroupTasks(name, new List<SimpleTask>()); }
            catch { throw; }
        }

        public void DeleteGroupTask(GroupTask gt)
        {
            foreach (var a in m_tasks)
            {
                if (gt.Equals(a))
                {
                    m_tasks.Remove(a);
                    break;
                }
            }
        }

        public void AddSimpleTask(SimpleTask task, int groupTaskPosition)
        {
            if (task == null) throw new ArgumentNullException();
            if (groupTaskPosition > m_tasks.Count) throw new ArgumentNullException();
            m_tasks[groupTaskPosition].AddSimpleTask(task);
        }

        public Boolean GroupExists(String name)
        {
            foreach (GroupTask gt in m_tasks)
            {
                if (name.Equals(gt.Name)) return true;
            }
            return false;
        }

        public Boolean TaskExist(String name)
        {
            foreach (GroupTask gt in m_tasks)
            {
                if (gt.Name.Equals(name)) return true;
                foreach (var v in gt.Tasks) if (v.Name.Equals(name)) return true;
            }
            return false;
        }
    }

    public class GroupTask
    {
        public String Name { set; get; }
        private List<SimpleTask> m_simpleTasks;
        public List<SimpleTask> Tasks
        {
            get
            {
                if (m_simpleTasks == null) m_simpleTasks = new List<SimpleTask>();
                return m_simpleTasks;
            }
            set => m_simpleTasks = value;
        }
        public GroupTask() { }

        public void AddSimpleTask(SimpleTask task)
        {
            if (task == null) throw new ArgumentNullException();
            m_simpleTasks.Add(task);
        }
    }

    public class SimpleTask
    {
        
        public Type TaskType { set; get; }
        public String Name { set; get; }
        public int FirstPage { set; get; }
        public int LastPage { set; get; }
        public List<String> FileOrigPaths { set; get; }
        public String FileResultPath { set; get; }
        public SimpleTask() { }

        public void Execute()
        {

        }
    }

    //public class SplitTask : SimpleTask
    //{
    //    public StorageFile File { get; set; }
    //    public int FirstPage { get; set; }
    //    public int LastPage { get; set; }
    //    public SplitTask() { }

    //    public override void Execute()
    //    {
    //        // dividir el fichero
    //        throw new NotImplementedException();
    //    }
    //}

    //public class Mergetask : SimpleTask
    //{
    //    public List<StorageFile> Files
    //    {
    //        get
    //        {
    //            if (m_files == null) m_files = new List<StorageFile>();
    //            return m_files;
    //        }
    //        set => m_files = value;
    //    }
    //    private List<StorageFile> m_files;
    //    public Mergetask() { }

    //    public void AddFiles(List<StorageFile> files)
    //    {
    //        if (m_files == null) m_files = new List<StorageFile>();
    //        m_files = files;
    //    }

    //    public override void Execute()
    //    {
    //        // unir ficheros
    //        throw new NotImplementedException();
    //    }
    //}
}
