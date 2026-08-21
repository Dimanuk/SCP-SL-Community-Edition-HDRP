using System;
using UnityEngine;

namespace InventorySystem.Items
{
    public class ItemViewmodelBase : MonoBehaviour
    {
        private ItemBase _parentItem;
        private ReferenceHub _hub;
        private bool _idSet;
        private ItemIdentifier _itemId;

        public virtual float ViewmodelCameraFOV => 60f;

        public ItemBase ParentItem
        {
            get => _parentItem;
            protected set => _parentItem = value;
        }

        public ItemIdentifier ItemId
        {
            get
            {
                if (!_idSet)
                {
                    if (IsLocal)
                    {
                        if (ParentItem != null && ParentItem.ItemSerial != 0)
                        {
                            _idSet = true;
                            _itemId = new ItemIdentifier(ParentItem.ItemTypeId, ParentItem.ItemSerial);
                            return _itemId;
                        }

                        return _itemId;
                    }

                    throw new InvalidOperationException("ItemId could not be set.");
                }

                return _itemId;
            }
        }

        public ReferenceHub Hub
        {
            get => _hub;
            private set => _hub = value;
        }

        public bool IsLocal { get; protected set; }

        public bool IsSpectator { get; protected set; }

        public virtual void InitLocal(ItemBase ib)
        {
            if (ib == null)
                throw new NullReferenceException();

            Hub = ib.Owner;
            ParentItem = ib;
            IsLocal = true;

            InitAny();
        }

        public virtual void InitSpectator(ReferenceHub ply, ItemIdentifier id, bool wasEquipped)
        {
            Hub = ply;
            IsSpectator = true;
            _idSet = true;
            _itemId = id;

            InitAny();
        }

        public virtual void InitAny()
        {
        }

        internal virtual void OnEquipped()
        {
        }
    }
}