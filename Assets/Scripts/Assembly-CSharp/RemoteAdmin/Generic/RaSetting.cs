using RemoteAdmin.Menus;
using System;
using System.Collections.Generic;

namespace RemoteAdmin.Generic
{
    [Serializable]
    public abstract class RaSetting<T>
    {
        private T _value;

        public virtual T Value
        {
            get => _value;
            set
            {
                if (!EqualityComparer<T>.Default.Equals(_value, value))
                {
                    _value = value;
                }
            }
        }

        public virtual T DefaultValue { get; }

        public abstract string Path { get; }

        public RaSetting()
        {
            RaSettings.OnReset += Reset;
            RaSettings.OnSave += Save;
            RaSettings.OnLoad += Load;
        }

        public void Load() => OnLoad();

        public void Save() => OnSave();

        public virtual void Reset()
        {
            Value = DefaultValue;
        }

        protected abstract void OnLoad();

        protected abstract void OnSave();

        ~RaSetting()
        {
            RaSettings.OnLoad -= Load;
            RaSettings.OnSave -= Save;
            RaSettings.OnReset -= Reset;
        }
    }
}