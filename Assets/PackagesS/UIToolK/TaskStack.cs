using System.Collections.Generic;
using System.Linq;

namespace UINavigation
{
    internal class TaskStack
    {
        private List<Task> stacked = new List<Task>();

        internal void Pop()
        {
            RemoveLast();
        }

        internal void Clear()
        {
            while(stacked.Count > 1)
            {
                RemoveAt(0);
            }
        }

        internal void PopTill(Task task)
        {
            if (task == null)
                return;

            if (stacked.Count > 0 && !Equals(stacked.Last(), task)) {

                RemoveLast(false);

                while (stacked.Count > 0 && !Equals(stacked.Last(), task))
                {
                    stacked.Last().Destroy();
                    stacked.RemoveAt(stacked.Count - 1);
                }
            }

            if(stacked.Count > 0)
            {
                stacked.Last().Enter();
            }
            else
            {
                task.Run();
            }
        }

        internal void AddTask(Task task)
        {
            if (stacked.Count != 0)
                stacked.Last().Leave();

            stacked.Add(task);
            task.Create();
            task.Enter();
        }

        internal void RemoveLast(bool enterNew = true)
        {
            if (stacked.Count == 0)
                return;

            stacked.Last().Leave();
            stacked.Last().Destroy();

            stacked.RemoveAt(stacked.Count - 1);

            if (stacked.Count != 0 && enterNew)
                stacked.Last().Enter();
        }

        internal void RemoveLast(Task task)
        {
            int index = stacked.LastIndexOf(task);
            RemoveAt(index);
        }

        internal void RemoveAt(int index)
        {
            if (index < 0 || stacked.Count == 0 || index >= stacked.Count)
                return;

            if (index == stacked.Count - 1)
            {
                RemoveLast();
            }
            else
            {
                stacked[index].Destroy();
                stacked.RemoveAt(index);
            }
        }

        internal Task GetCurrant()
        {
            if (stacked.Count == 0)
                return null;
            else return stacked.Last();
        }

        internal bool IsCurrant(Task task)
        {
            return Equals(GetCurrant(), task);
        }
    }
}
