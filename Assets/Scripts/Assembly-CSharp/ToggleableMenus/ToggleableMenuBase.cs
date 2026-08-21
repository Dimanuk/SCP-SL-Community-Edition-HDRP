using CursorManagement;
using System;
using System.Xml.Linq;
using UnityEngine;

namespace ToggleableMenus
{
    public abstract class ToggleableMenuBase : MonoBehaviour, IRegisterableMenu, ICursorOverride
    {
        public abstract bool CanToggle { get; }

        public virtual CursorOverrideMode CursorOverride => IsEnabled ? CursorOverrideMode.Free : CursorOverrideMode.NoOverride;

        public virtual bool LockMovement => false;

        private bool _isEnabled;
        public virtual bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                if (value == _isEnabled) return;
                _isEnabled = value;
                OnToggled();
            }
        }

        public ActionName MenuActionKey;

        protected abstract void OnToggled();

        protected virtual void Awake()
        {
            CursorManager.Register(this);
            ToggleableMenuController.RegisteredMenus.Add(this);
        }

        protected virtual void OnDestroy()
        {
            CursorManager.Unregister(this);
            ToggleableMenuController.RegisteredMenus.Remove(this);
        }
    }
}