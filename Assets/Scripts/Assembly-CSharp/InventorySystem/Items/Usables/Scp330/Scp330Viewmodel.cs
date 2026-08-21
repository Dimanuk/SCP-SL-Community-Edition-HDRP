using System;
using CursorManagement;
using CustomPlayerEffects;
using InventorySystem.GUI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace InventorySystem.Items.Usables.Scp330
{
    public class Scp330Viewmodel : UsableItemViewmodel, ICursorOverride
    {
        [Serializable]
        private struct CandyObject
        {
            public CandyKindID KindID;
            public GameObject HandObject;
            public Texture Icon;
            public AudioClip EatingSound;
        }

        [SerializeField]
        private RadialInventory _selector;

        [SerializeField]
        private RawImage[] _selectorSlots;

        [SerializeField]
        private CanvasGroup _selectorGroup;

        [SerializeField]
        private CanvasGroup _descriptionGroup;

        [SerializeField]
        private CandyObject[] _candies;

        [SerializeField]
        private TextMeshProUGUI _title;

        [SerializeField]
        private TextMeshProUGUI _description;

        [SerializeField]
        private TextMeshProUGUI _effects;

        private Scp330Bag _bag;
        private bool _openDelay;
        private bool _cancelled;
        private CandyKindID _displayedCandy;

        public CursorOverrideMode CursorOverride { get; private set; }

        public bool LockMovement => false;

        public override void InitLocal(ItemBase parent)
        {
            base.InitLocal(parent);
            _bag = parent as Scp330Bag;
        }

        public override void InitSpectator(ReferenceHub ply, ItemIdentifier id, bool wasEquipped)
        {
            Scp330NetworkHandler.OnClientSelectMessageReceived += HandleSelectMessage;
            base.InitSpectator(ply, id, wasEquipped);
            OnEquipped();

            // v12 records every candy selection into ReceivedSelectedCandies but never reads it
            // back, so a spectator who starts watching mid-hold is stuck on the closed bag.
            // Completing the mechanism (as later game versions do) restores the held candy.
            if (wasEquipped
                && Scp330NetworkHandler.ReceivedSelectedCandies.TryGetValue(id.SerialNumber, out CandyKindID heldCandy)
                && heldCandy != CandyKindID.None)
            {
                SetCandyModel(heldCandy);
            }
        }

        internal override void OnEquipped()
        {
            _openDelay = true;
            _cancelled = false;
            
            if (_descriptionGroup != null)
            {
                _descriptionGroup.alpha = 0;
            }
            
            _displayedCandy = 0;

            if (IsLocal)
            {
                CursorManager.Register(this);
                CursorOverride = CursorOverrideMode.NoOverride;
            }
            else
            {
                if (_selectorGroup != null)
                {
                    _selectorGroup.gameObject.SetActive(false);
                }
            }

            base.OnEquipped();
            
            float speedMultiplier = UsableItemModifierEffectExtensions.GetSpeedMultiplier(ItemId.TypeId, Hub);
            AnimatorSetFloat(UsableItemViewmodel.SpeedModifierHash, speedMultiplier);
            
            if (EquipSoundSource != null)
            {
                EquipSoundSource.pitch = speedMultiplier * _originalPitch;
            }
        }

        protected override void LateUpdate()
        {
            base.LateUpdate();

            if (!IsLocal)
            {
                return;
            }

            // NewInput.GetKey returns the *binding*, not a press state — check whether that
            // key was actually pressed this frame, else CancelSelector runs every frame and
            // CmdSelectItem(0) continuously deselects the bag (can't hold it).
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(NewInput.GetKey(ActionName.Inventory, KeyCode.None)))
            {
                CancelSelector();
                return;
            }

            if (_bag != null && _bag.IsCandySelected)
            {
                var candies = _bag.Candies;
                CandyKindID selectedCandy = candies[_bag.SelectedCandyId];
                SetCandyModel(selectedCandy);
                DisplaySelector(selectedCandy);

                // Only stop here once a real candy has been resolved; a None result (bad
                // index) must fall through so the radial selector can still be shown/used.
                if (selectedCandy != CandyKindID.None)
                {
                    return;
                }
            }
            else
            {
                SetCandyModel(0);
                DisplaySelector(0);
            }

            // Update radial inventory organized content — trailing slots must be cleared to 0,
            // not left with stale indices from a previous, larger candy count.
            if (_selector != null && _bag != null)
            {
                var organizedContent = _selector.OrganizedContent;
                var candies = _bag.Candies;
                for (int i = 0; i < organizedContent.Length; i++)
                {
                    organizedContent[i] = (ushort)((i < candies.Count) ? (i + 1) : 0);
                }
            }

            if (!_openDelay)
            {
                if (_selector != null)
                {
                    ushort selectedItem = 0;
                    // targetInventory must be null: this radial menu encodes candy indices via
                    // OrganizedContent above, not real inventory items. Passing the real
                    // OwnerInventory makes the shared radial-menu code resolve item slots against
                    // the actual inventory instead, which is why the bag wouldn't open/select.
                    InventoryGuiAction action = _selector.DisplayAndSelectItems(null, out selectedItem);

                    // NOTE: None (0) is the common case (nothing hovered) and MUST NOT cancel
                    // the selector — an earlier reconstruction cancelled on selectedItem == 0
                    // every frame, which fired CmdSelectItem(0) and instantly unequipped the bag.
                    if (action == InventoryGuiAction.Drop)
                    {
                        // Right-click drops a single candy; index 0 means nothing under the cursor.
                        if (selectedItem == 0)
                            return;

                        _bag.SendClientMessage(selectedItem - 1, true);
                    }
                    else if (action == InventoryGuiAction.Select)
                    {
                        if (Hub.playerEffectsController.TryGetEffect(out AmnesiaItems amnesia) && amnesia.ParamsActive)
                        {
                            amnesia.ExecutePulse();
                            CancelSelector();
                        }
                        else
                        {
                            if (selectedItem == 0)
                                CancelSelector();

                            if (_bag.Candies.Count > 0)
                            {
                                int index = Mathf.Clamp(selectedItem - 1, 0, _bag.Candies.Count - 1);
                                _bag.UsingSfxClip = GetClipForCandy(_bag.Candies[index]);
                            }
                            _bag.SelectedCandyId = selectedItem - 1;
                            _bag.SendClientMessage(selectedItem - 1, false);
                        }
                    }

                    // Description panel is refreshed for every action (including None) so the
                    // bag stays equipped while simply hovering the selector.
                    if (selectedItem == 0 || _bag.Candies.Count == 0 || selectedItem > _bag.Candies.Count)
                    {
                        DisplayDescriptions(0);
                    }
                    else
                    {
                        DisplayDescriptions(_bag.Candies[selectedItem - 1]);
                    }
                }
            }
            else
            {
                _openDelay = false;
            }
        }

        private void HandleSelectMessage(SelectScp330Message msg)
        {
            if (msg.Serial == ItemId.SerialNumber)
            {
                SetCandyModel((CandyKindID)msg.CandyID);
                OnUsingStarted();
            }
        }

        private void OnDisable()
        {
            if (IsLocal)
            {
                CursorManager.Unregister(this);
            }
            else
            {
                Scp330NetworkHandler.OnClientSelectMessageReceived -= HandleSelectMessage;
            }
        }

        private void SetCandyModel(CandyKindID id)
        {
            if (_candies == null) return;
            
            foreach (var candy in _candies)
            {
                if (candy.HandObject != null)
                {
                    candy.HandObject.SetActive(candy.KindID == id);
                }
            }
        }

        private void DisplaySelector(CandyKindID id)
        {
            if (_selectorGroup == null) return;

            bool show = id == 0 && !_cancelled;
            float rate = show ? 10f : -10f;

            _selectorGroup.alpha = Mathf.Clamp01(_selectorGroup.alpha + rate * Time.deltaTime);

            if (_selectorGroup.gameObject != null)
            {
                _selectorGroup.gameObject.SetActive(_selectorGroup.alpha > 0);
            }

            CursorOverride = show ? CursorOverrideMode.Confined : CursorOverrideMode.NoOverride;
            
            if (_selectorSlots == null || _bag == null) return;
            
            // Iterate over EVERY slot (not just up to Candies.Count) so slots left over from an
            // eaten candy get disabled — otherwise the eaten candy's icon stays on screen.
            var candies = _bag.Candies;
            for (int i = 0; i < _selectorSlots.Length; i++)
            {
                if (i < candies.Count && TryGetCandyObject(candies[i], out CandyObject candyObj))
                {
                    _selectorSlots[i].texture = candyObj.Icon;
                    _selectorSlots[i].enabled = true;
                }
                else
                {
                    _selectorSlots[i].enabled = false;
                }
            }
        }

        private void DisplayDescriptions(CandyKindID candy)
        {
            // Only skip when an actual candy is already shown; the None case must keep running so the
            // panel can finish fading out (and reset _displayedCandy) after the pointer leaves.
            if (_displayedCandy == candy && candy != CandyKindID.None) return;
            
            if (candy != 0)
            {
                if (_descriptionGroup != null)
                {
                    float currentAlpha = _descriptionGroup.alpha;
                    _descriptionGroup.alpha = Mathf.Clamp01(currentAlpha + Time.deltaTime * 10f);
                    
                    if (_descriptionGroup.alpha >= 1f)
                    {
                        Scp330Translations.GetCandyTranslation(candy, out string name, out string desc, out string fx);
                        
                        if (_title != null) _title.text = name;
                        if (_description != null) _description.text = desc;
                        if (_effects != null) _effects.text = fx;
                        
                        _displayedCandy = candy;
                    }
                }
            }
            else
            {
                if (_descriptionGroup != null)
                {
                    _descriptionGroup.alpha = Mathf.Clamp01(_descriptionGroup.alpha - Time.deltaTime * 10f);
                }

                // Clear the cached candy so hovering the SAME candy again re-triggers the fade-in.
                // Without this the top-of-method "_displayedCandy == candy" guard short-circuits and
                // the description never comes back after the pointer passed over an empty slot.
                _displayedCandy = candy;
            }
        }

        private bool TryGetCandyObject(CandyKindID id, out CandyObject val)
        {
            if (_candies == null)
            {
                val = default;
                return false;
            }
            
            foreach (var candy in _candies)
            {
                if (candy.KindID == id)
                {
                    val = candy;
                    return true;
                }
            }
            
            val = default;
            return false;
        }

        private void CancelSelector(bool bringBackInventory = false)
        {
            _cancelled = true;
            
            if (_bag != null)
            {
                var inventory = _bag.OwnerInventory;
                if (inventory != null)
                {
                    inventory.CmdSelectItem(0);
                }
            }
            
            bool inventoryVisible = InventoryGuiController.InventoryVisible;
            InventoryGuiController.InventoryVisible = inventoryVisible || bringBackInventory;
        }

        public static AudioClip GetClipForCandy(CandyKindID kind)
        {
            if (!InventoryItemLoader.TryGetItem<Scp330Bag>(ItemType.SCP330, out Scp330Bag bag)) return null;
            
            if (bag != null)
            {
                var viewmodel = bag.ViewModel as Scp330Viewmodel;
                if (viewmodel != null && viewmodel._candies != null)
                {
                    foreach (var candy in viewmodel._candies)
                    {
                        if (candy.KindID == kind)
                        {
                            return candy.EatingSound;
                        }
                    }
                }
            }
            
            return null;
        }

        public Scp330Viewmodel()
        {
            _originalPitch = 1f;
        }
    }
}
