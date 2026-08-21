using System;
using System.Collections.Generic;
using Footprinting;
using Mirror;
using PlayerRoles.PlayableScps.Scp939;
using UnityEngine;

namespace PlayerStatsSystem
{
    public static class DamageHandlers
    {
        public static readonly Func<DamageHandlerBase>[] DefinedConstructors = new Func<DamageHandlerBase>[14]
        {
            () => new RecontainmentDamageHandler(default),
            () => new FirearmDamageHandler(),
            () => new WarheadDamageHandler(),
            () => new UniversalDamageHandler(),
            () => new ScpDamageHandler(),
            () => new Scp096DamageHandler(),
            () => new Scp049DamageHandler(),
            () => new MicroHidDamageHandler(null, 0f),
            () => new CustomReasonDamageHandler(string.Empty),
            () => new ExplosionDamageHandler(default(Footprint), Vector3.zero, 0f, 0),
            () => new Scp018DamageHandler(null, 0f, ignoreFF: false),
            () => new DisruptorDamageHandler(default(Footprint), 0f),
            () => new JailbirdDamageHandler(),
            () => new Scp939DamageHandler(null, 0f)
        };

        public static readonly Dictionary<byte, Func<DamageHandlerBase>> ConstructorsById = new Dictionary<byte, Func<DamageHandlerBase>>();

        public static readonly Dictionary<int, byte> IdsByTypeHash = new Dictionary<int, byte>();

        [RuntimeInitializeOnLoadMethod]
        private static void PrepDictionaries()
        {
            byte b = 0;
            Func<DamageHandlerBase>[] definedConstructors = DefinedConstructors;
            foreach (Func<DamageHandlerBase> func in definedConstructors)
            {
                IdsByTypeHash.Add(func().GetType().FullName.GetStableHashCode(), b);
                ConstructorsById.Add(b, func);
                b++;
            }
        }
    }
}
