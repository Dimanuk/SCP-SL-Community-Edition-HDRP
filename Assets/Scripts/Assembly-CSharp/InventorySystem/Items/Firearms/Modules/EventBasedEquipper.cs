using InventorySystem.Items.Firearms;
using System.Diagnostics;

namespace InventorySystem.Items.Firearms.Modules
{
    public class EventBasedEquipper : IEquipperModule, IFirearmModuleBase
    {
        private bool _ready;

        private const float ServerTolerance = 0.1f;

        private readonly Firearm _firearm;

        private readonly Stopwatch _stopwatch;

        public bool Standby
        {
            get
            {
                if (!_firearm.IsLocalPlayer)
                    return true;

                if (_ready || _firearm.IsSpectated)
                    return true;

                if (!_stopwatch.IsRunning || _stopwatch.Elapsed.TotalSeconds < ServerTolerance)
                    return false;

                _stopwatch.Stop();
                _ready = true;
                return true;
            }
        }

        public EventBasedEquipper(Firearm firearm)
        {
            _ready = true;
            _firearm = firearm;
            _stopwatch = new Stopwatch();
        }

        public void OnEquipped()
        {
            _ready = false;
            _stopwatch.Stop();
        }

        public void Equip()
        {
            if (_firearm.IsLocalPlayer)
            {
                _stopwatch.Restart();
            }
            else
            {
                _ready = true;
            }
        }
    }
}