using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourT
{
    [System.Serializable]
    public class TaskArray : IEnumerable<Task>
    {
        [SerializeReference]
        private Task[] tasks = new Task[0];

        private int[] _taskPriorities = new int[0];
        public int[] TaskPriorities { get => _taskPriorities; set { _taskPriorities = value; InitPriorTasks(); } }

        public int TasksCount => tasks.Length;

        private SortedList<int, Task> _priorTasks;
        private SortedList<int, Task> PriorTasks
        {
            get
            {
                if (_priorTasks == null)
                {
                    _priorTasks = new SortedList<int, Task>(new DuplicateKeyComparer<int>());
                    InitPriorTasks();
                }
                return _priorTasks;
            }
        }

        private TaskEnumerator Enumerator
        {
            get
            {
                if (_taskEnumerator == null)
                    _taskEnumerator = new TaskArray.TaskEnumerator(PriorTasks.GetEnumerator());
                return _taskEnumerator;
            }
        }
        private TaskEnumerator _taskEnumerator;

        public IEnumerable<KeyValuePair<int, Task>> PriorEnumerable => PriorTasks;

        public TaskArray(Task[] tasks)
        {
            this.tasks = tasks;
        }

        //public Task this[int i]{
        //   get => tasks[i];
        //    internal set => tasks[i] = value;
        //}

        private void InitPriorTasks()
        {
            PriorTasks.Clear();
            if (tasks == null)
                tasks = new Task[0];

            if(TaskPriorities == null || TaskPriorities.Length < tasks.Length)
            {
                for (int i = 0; i < tasks.Length; i++)
                {
                    PriorTasks.Add(i, tasks[i]);
                }
            }
            else
            {
                for (int i = 0; i < tasks.Length; i++)
                {
                    PriorTasks.Add(TaskPriorities[i], tasks[i]);
                }
            }
        }

        #region Enumerator

        public IEnumerator<Task> GetEnumerator()
        {
            return Enumerator;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Enumerator;
        }

        //public IEnumerator GetEnumerator()
        //{
        //    return PriorTasks.GetEnumerator();
        //}

        private class TaskEnumerator : IEnumerator<Task>
        {
            private IEnumerator<KeyValuePair<int, Task>> baseEnumerator;

            public TaskEnumerator(IEnumerator<KeyValuePair<int, Task>> baseEnumerator)
            {
                this.baseEnumerator = baseEnumerator;
            }

            public Task Current { get => baseEnumerator.Current.Value; }
            object IEnumerator.Current { get => Current; }

            public void Dispose()
            {
                baseEnumerator.Dispose();
            }

            public bool MoveNext()
            {
                return baseEnumerator.MoveNext();
            }

            public void Reset()
            {
                baseEnumerator.Reset();
            }
        }
        #endregion

        #region Comperarer
        public class DuplicateKeyComparer<TKey> : IComparer<TKey> where TKey : IComparable
        {
            public int Compare(TKey x, TKey y)
            {
                int result = x.CompareTo(y);

                if (result == 0)
                    return -1;   // Handle equality as beeing greater
                else
                    return result;
            }
        }
        #endregion
    }
}
