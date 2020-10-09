using System.Collections.Generic;

namespace UINavigation
{
    public class Application
    {
        internal TaskStack TaskStack { get; private set; }

        public BackStack BackStack { get; private set; }

        public Task Currant => TaskStack.GetCurrant();

        public Application()
        {
            TaskStack = new TaskStack();

            BackStack = new BackStack(this);
        }
    }

    public class Task
    {
        public Application Application { get; private set; }
        internal TaskStack TaskStack => Application.TaskStack;

        public bool IsRunning => TaskStack.IsCurrant(this);

        public Task(Application application)
        {
            Application = application;
        }

        public void Run()
        {
            TaskStack.AddTask(this);
        }

        internal void Create()
        {
            OnCreate();
        }

        protected virtual void OnCreate() { }

        internal void Enter()
        {
            OnEnter();
        }

        protected virtual void OnEnter() { }

        internal void Leave()
        {
            OnLeave();
        }

        protected virtual void OnLeave() { }

        internal void Destroy()
        {
            OnDestroy();
        }

        protected virtual void OnDestroy() { }
    }

    //public interface ITaskable
    //{
    //    void OnCreate();

    //    void OnEnter();

    //    void OnLeave();

    //    void OnDestroy();
    //}
}
