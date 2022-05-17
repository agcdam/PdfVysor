using System;
using System.Collections.Generic;
using Windows.Storage;

namespace PdfVysor
{
    /// <summary>
    /// Enum where it is specified the type of task.
    /// </summary>
    public enum Type { Split, Merge}

    /// <summary>
    /// Class where the tasks is managed.
    /// </summary>
    public class Tasks
    {
        public String Name { set; get; }
        private List<GroupTask> m_groupTasks;
        public List<GroupTask> GroupTasks
        {
            get
            {
                if (m_groupTasks == null) m_groupTasks = new List<GroupTask>();
                return m_groupTasks;
            }
            set => m_groupTasks = value;
        }
        public Tasks() { }

        /// <summary>
        /// Creates new <see cref="GroupTask"/> with <paramref name="name"/> as <see cref="GroupTask.Name"/>, and <paramref name="simpleTasks"/> as <see cref="GroupTask.Tasks"/>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="simpleTasks"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public void AddGroupTasks(String name, List<SimpleTask> simpleTasks)
        {
            // Management of the possible errors
            if (name == null || simpleTasks == null) throw new ArgumentNullException();
            if (name.Length == 0) throw new ArgumentOutOfRangeException();

            // Add new GroupTask in the Tasks
            GroupTask gp = new()
            {
                Name = name,
                Tasks = simpleTasks
            };
            if (m_groupTasks == null) m_groupTasks = new List<GroupTask> { };
            m_groupTasks.Add(gp);
        }

        /// <summary>
        /// Add new <see cref="GroupTask"/> in <see cref="GroupTasks"/> with a empty <seealso cref="List{SimpleTask}"/> and the <paramref name="name"/> given
        /// </summary>
        /// <param name="name"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public void AddGroupTasks(String name)
        {
            try { AddGroupTasks(name, new List<SimpleTask>()); }
            catch { throw; }
        }

        /// <summary>
        /// Remove <paramref name="gt"/> from <see cref="GroupTasks"/> if exists
        /// </summary>
        /// <param name="gt"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public void RemoveGrouptask(GroupTask gt)
        {
            if (gt == null) throw new ArgumentNullException();
            m_groupTasks.Remove(gt);
        }

        /// <summary>
        /// Add <paramref name="task"/> in <see cref="GroupTasks"/> in the <paramref name="groupTaskPosition"/> position in <see cref="GroupTasks"/>
        /// </summary>
        /// <param name="task"></param>
        /// <param name="groupTaskPosition"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public void AddSimpleTask(SimpleTask task, int groupTaskPosition)
        {
            if (task == null) throw new ArgumentNullException();
            if (groupTaskPosition > m_groupTasks.Count) throw new ArgumentNullException();
            m_groupTasks[groupTaskPosition].AddSimpleTask(task);
        }

        /// <summary>
        /// Returns true if <paramref name="name"/> exists in <see cref="GroupTasks"/>,
        /// otherwise false if there's nothing with the same name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Boolean NameExists(String name)
        {
            foreach (GroupTask gt in m_groupTasks)
            {
                if (gt.Name.Equals(name)) return true;
                foreach (var v in gt.Tasks) if (v.Name.Equals(name)) return true;
            }
            return false;
        }

        /// <summary>
        /// Return the <see cref="GroupTask"/> that corresponds in position with <see cref="GroupTask"/> with <paramref name="position"/>
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public GroupTask GetGroupByPosition(int position)
        {
            if (position >= m_groupTasks.Count || position < 0) throw new ArgumentOutOfRangeException();

            return m_groupTasks[position];
        }

        /// <summary>
        /// Returns the <see cref="SimpleTask"/> in <see cref="GroupTasks"/> if there's one that coincide with the name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public SimpleTask GetSimpleTaskByName(String name)
        {
            foreach (GroupTask gt in m_groupTasks)
            {
                foreach (SimpleTask v in gt.Tasks) if (v.Name.Equals(name)) return v;
            }
            return null;
        }

        /// <summary>
        /// Remove <see cref="SimpleTask"/> or <see cref="GroupTask"/> if exists in <see cref="GroupTasks"/>
        /// </summary>
        /// <param name="name"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public void RemoveItemsByName(String name)
        {
            if (String.IsNullOrEmpty(name)) throw new ArgumentNullException();
            foreach (GroupTask gt in m_groupTasks)
            {
                if (gt.Name.Equals(name))
                {

                    m_groupTasks.Remove(gt);
                    return;
                } else
                {
                    foreach (SimpleTask st in gt.Tasks)
                    {
                        if (st.Name.Equals(name))
                        {
                            gt.RemoveSimpleTask(st);
                            return;
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Class that manage the list of <see cref="SimpleTask"/>
    /// </summary>
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

        /// <summary>
        /// Add <see cref="SimpleTask"/> into the <see cref="GroupTask"/>
        /// </summary>
        /// <param name="task"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public void AddSimpleTask(SimpleTask task)
        {
            if (task == null) throw new ArgumentNullException();
            m_simpleTasks.Add(task);
        }

        /// <summary>
        /// Remove <see cref="SimpleTask"/> from <see cref="Tasks"/>
        /// </summary>
        /// <param name="simpleTask"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public void RemoveSimpleTask(SimpleTask simpleTask)
        {
            if (simpleTask == null) throw new ArgumentNullException();
            m_simpleTasks.Remove(simpleTask);
        }
    }

    /// <summary>
    /// Class that represents a <see cref="SimpleTask"/>
    /// </summary>
    public class SimpleTask
    {
        public Type TaskType { set; get; }
        public String Name { set; get; }
        public int FirstPage { set; get; }
        public int LastPage { set; get; }
        public List<String> FileOrigPaths { set; get; }
        public String FileResultPath { set; get; }
        public SimpleTask() { }
    }
}
