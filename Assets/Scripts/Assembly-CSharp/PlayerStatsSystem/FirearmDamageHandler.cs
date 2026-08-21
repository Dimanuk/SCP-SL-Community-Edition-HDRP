using System.Collections.Generic;
using System.Runtime.CompilerServices;

using Footprinting;
using InventorySystem.Items.Firearms;
using InventorySystem.Items.Firearms.Ammo;
using Mirror;

namespace PlayerStatsSystem
{
	public class FirearmDamageHandler : AttackerDamageHandler
	{
		public ItemType WeaponType;

		private string _ammoName;

		private ItemType _ammoType;

		private sbyte _hitDirectionX;

		private sbyte _hitDirectionZ;

		private readonly float _penetration;

		private readonly string _deathReasonFormat;

		private readonly bool _useHumanHitboxes;

        private static readonly global::System.Collections.Generic.Dictionary<HitboxType, float> HitboxToForce = new global::System.Collections.Generic.Dictionary<HitboxType, float>
        {
            [HitboxType.Body] = 0.08f,
            [HitboxType.Headshot] = 0.08f,
            [HitboxType.Limb] = 0.016f
        };

        private static readonly global::System.Collections.Generic.Dictionary<HitboxType, float> HitboxDamageMultipliers = new global::System.Collections.Generic.Dictionary<HitboxType, float>
        {
            [HitboxType.Headshot] = 2f,
            [HitboxType.Limb] = 0.7f
        };

        private static readonly global::System.Collections.Generic.Dictionary<ItemType, float> AmmoToForce = new global::System.Collections.Generic.Dictionary<ItemType, float>
        {
            [ItemType.Ammo12gauge] = 1.9f,
            [ItemType.Ammo44cal] = 1.2f,
            [ItemType.Ammo9x19] = 0.7f
        };

        private const float UpwardVelocityFactor = 0.1f;

		public override float Damage { get; protected set; }

        public override global::Footprinting.Footprint Attacker { get; protected set; }

        public override bool AllowSelfDamage => 0 != 0;

		public override string RagdollInspectText => string.Format(_deathReasonFormat, _ammoName);

		public override string DeathScreenText => string.Empty;

        public override string ServerLogsText => "Shot by " + Attacker.Nickname + " with " + WeaponType.ToString() + " to the '" + Hitbox.ToString() + "' hitbox.";
        public FirearmDamageHandler()
        {
            _deathReasonFormat = DeathTranslations.BulletWounds.RagdollTranslation;
        }
        public FirearmDamageHandler(global::InventorySystem.Items.Firearms.Firearm firearm, float damage, bool useHumanMutlipliers = true)
        {
            _deathReasonFormat = DeathTranslations.BulletWounds.RagdollTranslation;
            SetWeapon(firearm.ItemTypeId);
            Damage = damage;
            _penetration = firearm.ArmorPenetration;
            Attacker = firearm.Footprint;
            _useHumanHitboxes = useHumanMutlipliers;
            global::UnityEngine.Vector3 forward = firearm.Owner.PlayerCameraReference.forward;
            _hitDirectionX = (sbyte)global::UnityEngine.Mathf.RoundToInt(forward.x * 127f);
            _hitDirectionZ = (sbyte)global::UnityEngine.Mathf.RoundToInt(forward.z * 127f);
        }

        public override void WriteAdditionalData(global::Mirror.NetworkWriter writer)
        {
            base.WriteAdditionalData(writer);
            writer.WriteByte((byte)WeaponType);
            writer.WriteByte((byte)Hitbox);
            global::Mirror.NetworkWriterExtensions.WriteSByte(writer, _hitDirectionX);
            global::Mirror.NetworkWriterExtensions.WriteSByte(writer, _hitDirectionZ);
        }

        public override void ReadAdditionalData(global::Mirror.NetworkReader reader)
        {
            base.ReadAdditionalData(reader);
            SetWeapon((ItemType)reader.ReadByte());
            Hitbox = (HitboxType)reader.ReadByte();
            _hitDirectionX = global::Mirror.NetworkReaderExtensions.ReadSByte(reader);
            _hitDirectionZ = global::Mirror.NetworkReaderExtensions.ReadSByte(reader);
        }


        public override void ProcessRagdoll(BasicRagdoll ragdoll)
        {
            base.ProcessRagdoll(ragdoll);
            if (!HitboxToForce.TryGetValue(Hitbox, out var value) || !(ragdoll is global::PlayerRoles.Ragdolls.DynamicRagdoll dynamicRagdoll))
            {
                return;
            }
            float value2;
            float num = value * (AmmoToForce.TryGetValue(_ammoType, out value2) ? value2 : 1f);
            global::UnityEngine.Rigidbody[] linkedRigidbodies = dynamicRagdoll.LinkedRigidbodies;
            for (int i = 0; i < linkedRigidbodies.Length; i++)
            {
                linkedRigidbodies[i].AddForce(num * 127f * 0.1f * global::UnityEngine.Vector3.up, global::UnityEngine.ForceMode.VelocityChange);
            }
            global::UnityEngine.Vector3 force = new global::UnityEngine.Vector3(_hitDirectionX, 0f, _hitDirectionZ) * num;
            global::PlayerRoles.Ragdolls.HitboxData[] hitboxes = dynamicRagdoll.Hitboxes;
            for (int i = 0; i < hitboxes.Length; i++)
            {
                global::PlayerRoles.Ragdolls.HitboxData hitboxData = hitboxes[i];
                if (hitboxData.RelatedHitbox == Hitbox)
                {
                    hitboxData.Target.AddForce(force, global::UnityEngine.ForceMode.VelocityChange);
                }
            }
        }

        private void SetWeapon(ItemType weapon)
        {
            if (weapon != WeaponType)
            {
                WeaponType = weapon;
                if (global::InventorySystem.InventoryItemLoader.TryGetItem<global::InventorySystem.Items.Firearms.Firearm>(WeaponType, out var result))
                {
                    _ammoType = result.AmmoType;
                    if (global::InventorySystem.InventoryItemLoader.TryGetItem<AmmoItem>(_ammoType, out var ammoItem))
                        _ammoName = ammoItem.Name;
                }
            }
        }

        protected override void ProcessDamage(ReferenceHub ply)
        {
            if (!_useHumanHitboxes && global::PlayerRoles.PlayerRolesUtils.IsHuman(ply))
            {
                Hitbox = HitboxType.Body;
            }
            if (ply.roleManager.CurrentRole is global::PlayerRoles.IArmoredRole armoredRole)
            {
                int armorEfficacy = armoredRole.GetArmorEfficacy(Hitbox);
                int bulletPenetrationPercent = global::UnityEngine.Mathf.RoundToInt(_penetration * 100f);
                Damage = global::InventorySystem.Items.Armor.BodyArmorUtils.ProcessDamage(armorEfficacy, Damage, bulletPenetrationPercent);
            }
            if (_useHumanHitboxes && HitboxDamageMultipliers.TryGetValue(Hitbox, out var value))
            {
                Damage *= value;
            }
            base.ProcessDamage(ply);
        }
	}
}
