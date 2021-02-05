using System;
using System.Collections.Generic;
using System.Linq;

namespace UINavigation
{
    public class BackStack
    {
        private List<ITaskable> stacked = new List<ITaskable>();

        public Action OnBeforPopListener;

        #region Public

        public void Pop()
        {
            OnBeforPopListener?.Invoke();

            bool pop = true;

            if (GetCurrant() is IPopListener listener)
                pop = !listener.OnPop();

            if (pop)
                RemoveLast();
        }

        public void Clear()
        {
            while (stacked.Count > 1)
            {
                RemoveAt(0);
            }
        }

        public void PopTill(ITaskable task)
        {
            if (task == null)
                return;

            if (stacked.Count > 0 && !Equals(stacked.Last(), task))
            {

                RemoveLast(false);

                while (stacked.Count > 0 && !Equals(stacked.Last(), task))
                {
                    //stacked.Last().Destroy();
                    stacked.RemoveAt(stacked.Count - 1);
                }
            }

            if (stacked.Count > 0)
            {
                stacked.Last().OnEnter();
            }
            else
            {
                AddTask(task);
            }
        }

        #endregion

        #region Internal

        internal void AddTask(ITaskable task)
        {
            if (stacked.Count != 0)
                stacked.Last().OnLeave();

            stacked.Add(task);
            //task.Create();
            task.OnEnter();
        }

        internal void RemoveLast(bool enterNew = true)
        {
            if (stacked.Count == 0)
                return;

            stacked.Last().OnLeave();
            //stacked.Last().Destroy();

            stacked.RemoveAt(stacked.Count - 1);

            if (stacked.Count != 0 && enterNew)
                stacked.Last().OnEnter();
        }

        internal void RemoveLast(ITaskable task)
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
                //stacked[index].Destroy();
                stacked.RemoveAt(index);
            }
        }

        internal ITaskable GetCurrant()
        {
            if (stacked.Count == 0)
                return null;
            else return stacked.Last();
        }

        internal bool IsCurrant(ITaskable task)
        {
            return Equals(GetCurrant(), task);
        }

        #endregion

        public interface IPopListener
        {
            bool OnPop();
        }
    }
}
