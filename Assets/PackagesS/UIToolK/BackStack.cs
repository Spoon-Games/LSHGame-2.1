using System;

namespace UINavigation
{
    public class BackStack
    {
        private TaskStack TaskStack { get; set; }

        public Action OnBeforPopListener; 

        public BackStack(Application application)
        {
            TaskStack = application.TaskStack;
        }

        public void Clear()
        {
            TaskStack.Clear();
        }

        public void Pop()
        {
            OnBeforPopListener?.Invoke();

            bool pop = true;

            if (TaskStack.GetCurrant() is IPopListener listener)
                pop = !listener.OnPop();

            if(pop)
                TaskStack.Pop();
        }

        public void PopTill(Task task)
        {
            TaskStack.PopTill(task);
        }

        public interface IPopListener
        {
            bool OnPop();
        }
    }
}
