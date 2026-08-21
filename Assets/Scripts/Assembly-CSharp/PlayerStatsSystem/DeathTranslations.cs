using System.Collections.Generic;

namespace PlayerStatsSystem
{
    public static class DeathTranslations
    {
        public static readonly Dictionary<byte, DeathTranslation> TranslationsById = new();

        public static readonly DeathTranslation Recontained;
        public static readonly DeathTranslation Warhead;
        public static readonly DeathTranslation Scp049;
        public static readonly DeathTranslation Unknown;
        public static readonly DeathTranslation Asphyxiated;
        public static readonly DeathTranslation Bleeding;
        public static readonly DeathTranslation Falldown;
        public static readonly DeathTranslation PocketDecay;
        public static readonly DeathTranslation Decontamination;
        public static readonly DeathTranslation Poisoned;
        public static readonly DeathTranslation Scp207;
        public static readonly DeathTranslation SeveredHands;
        public static readonly DeathTranslation MicroHID;
        public static readonly DeathTranslation Tesla;
        public static readonly DeathTranslation Explosion;
        public static readonly DeathTranslation Scp096;
        public static readonly DeathTranslation Scp173;
        public static readonly DeathTranslation Scp939Lunge;
        public static readonly DeathTranslation Zombie;
        public static readonly DeathTranslation BulletWounds;
        public static readonly DeathTranslation Crushed;
        public static readonly DeathTranslation UsedAs106Bait;
        public static readonly DeathTranslation FriendlyFireDetector;
        public static readonly DeathTranslation Hypothermia;
        public static readonly DeathTranslation CardiacArrest;
        public static readonly DeathTranslation Scp939Other;

        static DeathTranslations()
        {
            Recontained = new DeathTranslation(0, 2, 2, "Recontained.");
            Warhead = new DeathTranslation(1, 3, 3, "Vaporized by the Alpha Warhead.");
            Scp049 = new DeathTranslation(2, 4, 4, "Died to SCP-049.");
            Unknown = new DeathTranslation(3, 5, 5, "Unknown cause of death.");
            Asphyxiated = new DeathTranslation(4, 4, 7, "Asphyxiated.");
            Bleeding = new DeathTranslation(5, 8, 9, "Bleeding.");
            Falldown = new DeathTranslation(6, 10, 11, "Fall damage.");
            PocketDecay = new DeathTranslation(7, 12, 12, "Decayed in the Pocket Dimension.");
            Decontamination = new DeathTranslation(8, 13, 13, "Melted by a highly corrosive substance.");
            Poisoned = new DeathTranslation(9, 14, 15, "Poison.");
            Scp207 = new DeathTranslation(10, 16, 17, "SCP-207.");
            SeveredHands = new DeathTranslation(11, 8, 18, "Severed Hands from SCP-330.");
            MicroHID = new DeathTranslation(12, 6, 6, "Micro H.I.D.");
            Tesla = new DeathTranslation(13, 6, 19, "Tesla.");
            Explosion = new DeathTranslation(14, 20, 21, "Explosion.");
            Scp096 = new DeathTranslation(15, 22, 22, "Died to SCP-096.");
            Scp173 = new DeathTranslation(16, 23, 23, "Died to SCP-173.");
            Scp939Lunge = new DeathTranslation(17, 24, 24, "Lunged by SCP-939.");
            Zombie = new DeathTranslation(18, 25, 25, "Died to SCP-049-2.");
            BulletWounds = new DeathTranslation(19, 26, 26, "{0} bullet wounds.");
            Crushed = new DeathTranslation(20, 27, 27, "Crushed.");
            UsedAs106Bait = new DeathTranslation(21, 28, 29, "Used as bait for SCP-106.");
            FriendlyFireDetector = new DeathTranslation(22, 30, 30, "Automatically killed for friendly fire.");
            Hypothermia = new DeathTranslation(23, 31, 32, "Died to hypothermia.");
            CardiacArrest = new DeathTranslation(24, 4, 4, "Died to a heart attack.");
            Scp939Other = new DeathTranslation(25, 33, 33, "Died to SCP-939.");
        }
    }
}
