using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TagInterpreterR
{
    public class TagChain
    {
        private List<BaseTag> tags = new List<BaseTag>();
        private int _index = -1;
        public int Index => _index;
        public int TagCount => tags.Count;
        public bool IsIterating => Index >= 0 && !IsEnd;
        public bool IsEnd => Index >= tags.Count;
        public BaseTag Current { get; private set; }

        public Action<BaseTag> OnCreate;
        public Action<BaseTag> OnActivate;
        public Func<BaseTag, bool> OnUpdate;
        public Action<BaseTag> OnDeactivate;
        public Action<BaseTag, bool> OnDestroy;
        

        #region Iterate
        public void Start()
        {
            if (Index == -1)
            {
                Reset();

                _index = 0;
                if (Index >= tags.Count)
                    return;
                Current = tags[Index];
                Create();
            }
        }

        public bool GetNext(bool skip = false)
        {
            if(!IsIterating)
            {
                return false;
            }

            if (Update() && !skip)
                return false;

            if (Current.IsSingle)
                Destroy();

            if (Current is EndTag endTag)
            {
                Destroy(endTag.ReferenceTag);
                ActivatePreviousTag(endTag.ReferenceTag.Name, true);
            }

            _index++;
            if(Index >= tags.Count)
            {
                Current = null;
                return true;
            }

            Current = tags[Index];

            ActivatePreviousTag(Current.Name, false); // Deactivate previous
            Create();

            return true;
        }




        #endregion

        #region Tag Lifecycle
        private void Create()
        {
            if (!Current.Exists)
            {
                Current.Exists = true;
                Current.OnCreate();
                OnCreate(Current);
                Activate();
            }
        }

        private void Activate()
        {
            if (!Current.Active)
            {
                Current.Active = true;
                Current.OnActivate();
                OnActivate(Current);
            }
        }

        private bool Update() {
            bool isOnUpdate = OnUpdate(Current);
            //Debug.Log("Update: "+index+" C: "+ Current + " is " + isOnUpdate);
            return Current.OnUpdate() || isOnUpdate;
        }

        private void Deactivate() => Deactivate(Current);
        private void Deactivate(BaseTag tag)
        {
            if (tag.Active)
            {
                tag.Active = false;
                tag.OnDeactivate();
                OnDeactivate(tag);
            }
        }

        private void Destroy() => Destroy(Current);
        private void Destroy(BaseTag tag)
        {
            if (tag.Exists)
            {
                bool returnToDefault = FindActive(tag.Name, out BaseTag t, false); // If an inactive exists

                Deactivate(tag);
                tag.Exists = false;
                tag.OnDestroy(returnToDefault);
                OnDestroy(tag, returnToDefault);
            }
        }
        #endregion

        #region Helper Methods
        public bool FindActive(string name, out BaseTag tag)
        {
            return FindActive(name, out tag, true);
        }

        public bool FindActive<T>(out T tag) where T : BaseTag
        {
            for (int i = Index; i >= 0; i--)
            {
                if (tags[i].Exists && tags[i].Active && tags[i] is T t)
                {
                    tag = t;
                    return true;
                }
            }
            tag = null;
            return false;
        }

        private void Reset()
        {
            _index =-1;
            foreach (var tag in tags)
            {
                tag.Reset();
            }
        }

        private bool FindActive(string name, out BaseTag tag, bool isActive
)
        {
            tag = null;
            if (name == null)
                return false;

            for (int i = Index; i >= 0; i--)
            {
                if (Equals(tags[i].Name, name) && tags[i].Exists && (tags[i].Active ^ isActive) && !tags[i].IsSingle)
                {
                    tag = tags[i];
                    return true;
                }
            }
            return false;
        }

        private void ActivatePreviousTag(string name, bool activate)
        {
            if(FindActive(name,out BaseTag tag, activate)){
                if (activate)
                {
                    Activate();
                }
                else
                {
                    Deactivate();
                }
            }
        }

        public override string ToString()
        {
            string s = "TagChain:\n";
            foreach (var tag in tags)
            {
                s += tag.ToString() + "\n";
            }
            return s;
        }
        #endregion

        #region Create
        internal void Add(BaseTag tag)
        {
            tags.Add(tag);
        }

        internal void AddEnd(string name)
        {
            for(int i = tags.Count - 1; i >= 0; i--)
            {
                if (Equals(tags[i].Name, name) && !tags[i].IsSingle)
                {
                    tags.Add(new EndTag(tags[i]));
                    return;
                }
            }
            throw new InterpreterException("There is no tag for the end-tag: " + name);
        }
        #endregion
    }

    public abstract class BaseTag
    {
        public bool Exists { get; internal set; }
        public bool Active { get; internal set; }

        public bool IsSingle { get; internal set; }
        public string Name;

        internal void Reset()
        {
            Exists = false;
            Active = false;
            OnReset();
        }
        public virtual void OnReset() { }

        public virtual void OnCreate() { }

        public virtual void OnActivate() { }

        public virtual bool OnUpdate() { return false; }

        public virtual void OnDeactivate() { }

        public virtual void OnDestroy(bool returnToDefault) { }

        public override string ToString()
        {
            return "Tag: "+Name;
        }
    }

    public sealed class EndTag : BaseTag
    {
        public readonly BaseTag ReferenceTag;

        public EndTag(BaseTag referenceTag)
        {
            ReferenceTag = referenceTag;
            IsSingle = true;
            Name = null;
        }

        public override string ToString()
        {
            return "End to "+ReferenceTag.Name;
        }
    }
}
