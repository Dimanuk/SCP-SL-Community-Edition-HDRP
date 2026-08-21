namespace Achievements
{
    public static class AchievementManager
    {
        public struct AchievementMessage : global::Mirror.NetworkMessage
        {
            public byte AchievementId;
        }

        public static readonly global::Achievements.AchievementHandlerBase[] Handlers = new global::Achievements.AchievementHandlerBase[8]
        {
            new global::Achievements.Handlers.DidntEvenFeelThatHandler(),
            new global::Achievements.Handlers.EscapeHandler(),
            new global::Achievements.Handlers.ItemPickupHandler(),
            new global::Achievements.Handlers.PewPewHandler(),
            new global::Achievements.Handlers.RespawnHandler(),
            new global::Achievements.Handlers.Scp914UpgradeHandler(),
            new global::Achievements.Handlers.TurnThemAllHandler(),
            new global::Achievements.Handlers.GeneralKillsHandler()
        };

        public static readonly global::System.Collections.Generic.Dictionary<global::Achievements.AchievementName, global::Achievements.Achievement> Achievements = new global::System.Collections.Generic.Dictionary<global::Achievements.AchievementName, global::Achievements.Achievement>
        {
            [global::Achievements.AchievementName.TurnThemAll] = new global::Achievements.Achievement("turnthemall", 559416662751707317L, byServer: true),
            [global::Achievements.AchievementName.InvoluntaryRageQuit] = new global::Achievements.Achievement("unvoluntaryragequit", 559416663020142679L, byServer: true),
            [global::Achievements.AchievementName.Newb] = new global::Achievements.Achievement("newb", 559417282514780170L, byServer: true),
            [global::Achievements.AchievementName.FirstTime] = new global::Achievements.Achievement("firsttime", 559417282594471936L, byServer: true),
            [global::Achievements.AchievementName.GearUp] = new global::Achievements.Achievement("arescue", 559417357718781955L, byServer: true),
            [global::Achievements.AchievementName.ItsAlwaysLeft] = new global::Achievements.Achievement("awayout", 559417906258247700L, byServer: true),
            [global::Achievements.AchievementName.Betrayal] = new global::Achievements.Achievement("betrayal", 559417906199265281L, byServer: true),
            [global::Achievements.AchievementName.Chaos] = new global::Achievements.Achievement("chaos", 559418104510152720L, byServer: true),
            [global::Achievements.AchievementName.DeepFried] = new global::Achievements.Achievement("electrocuted", 559418104552095768L, byServer: true),
            [global::Achievements.AchievementName.Friendship] = new global::Achievements.Achievement("friendship", 564138635457331343L, byServer: true),
            [global::Achievements.AchievementName.ForScience] = new global::Achievements.Achievement("forscience", 564138635566514176L, byServer: true),
            [global::Achievements.AchievementName.IsThisThingOn] = new global::Achievements.Achievement("isthisthingon", 564138635448942717L),
            [global::Achievements.AchievementName.Rocket] = new global::Achievements.Achievement("iwanttobearocket", 564138635402936330L, byServer: true),
            [global::Achievements.AchievementName.LarryFriend] = new global::Achievements.Achievement("larryisyourfriend", 564138674611290119L, byServer: true),
            [global::Achievements.AchievementName.PewPew] = new global::Achievements.Achievement("pewpew", 564138635105140767L, byServer: true),
            [global::Achievements.AchievementName.Power] = new global::Achievements.Achievement("power", 564139505062379520L, byServer: true),
            [global::Achievements.AchievementName.SecureContainProtect] = new global::Achievements.Achievement("securecontainprotect", 564139504848601118L, byServer: true),
            [global::Achievements.AchievementName.TMinus] = new global::Achievements.Achievement("tminus", 564139505112580131L),
            [global::Achievements.AchievementName.TablesHaveTurned] = new global::Achievements.Achievement("tableshaveturned", 564139505834000508L, byServer: true),
            [global::Achievements.AchievementName.ThatCanBeUseful] = new global::Achievements.Achievement("thatcanbeusefull", 564139505045733386L, byServer: true),
            [global::Achievements.AchievementName.JustResources] = new global::Achievements.Achievement("justresources", "dboys_killed", 909163354143211570L, 50, byServer: true),
            [global::Achievements.AchievementName.ThatWasClose] = new global::Achievements.Achievement("thatwasclose", 564140195134439474L, byServer: true),
            [global::Achievements.AchievementName.SomethingDoneRight] = new global::Achievements.Achievement("timetodoitmyself", 564140195465658378L, byServer: true),
            [global::Achievements.AchievementName.Gravity] = new global::Achievements.Achievement("gravity", 564140195017129984L, byServer: true),
            [global::Achievements.AchievementName.WowReally] = new global::Achievements.Achievement("wowreally", 564140195382034434L, byServer: true),
            [global::Achievements.AchievementName.MicrowaveMeal] = new global::Achievements.Achievement("zap", 564140195000090635L, byServer: true),
            [global::Achievements.AchievementName.EscapeArtist] = new global::Achievements.Achievement("escapeartist", 564140574047731713L, byServer: true),
            [global::Achievements.AchievementName.Escape207] = new global::Achievements.Achievement("escape207", 638780450600386580L, byServer: true),
            [global::Achievements.AchievementName.CrisisAverted] = new global::Achievements.Achievement("crisisaverted", 638780450612969483L, byServer: true),
            [global::Achievements.AchievementName.DidntEvenFeelThat] = new global::Achievements.Achievement("didntevenfeelthat", 638780488433139752L, byServer: true),
            [global::Achievements.AchievementName.IllPassThanks] = new global::Achievements.Achievement("illpassthanks", 638780450311110658L, byServer: true),
            [global::Achievements.AchievementName.Overcurrent] = new global::Achievements.Achievement("attemptedrecharge", 638780450776809472L, byServer: true),
            [global::Achievements.AchievementName.PropertyOfChaos] = new global::Achievements.Achievement("propertyofchaos", 638780450667626506L, byServer: true)
        };

        [global::UnityEngine.RuntimeInitializeOnLoadMethod]
        private static void Init()
        {
            global::InventorySystem.Inventory.OnLocalClientStarted += LocalPlayerStarted;
            global::Achievements.AchievementHandlerBase[] handlers = Handlers;
            for (int i = 0; i < handlers.Length; i++)
            {
                handlers[i].OnInitialize();
            }
        }

        private static void LocalPlayerStarted()
        {
            global::Mirror.NetworkClient.ReplaceHandler<global::Achievements.AchievementManager.AchievementMessage>(ClientMessageReceived);
            global::Achievements.AchievementHandlerBase[] handlers = Handlers;
            for (int i = 0; i < handlers.Length; i++)
            {
                handlers[i].OnRoundStarted();
            }
        }

        private static void ClientMessageReceived(global::Achievements.AchievementManager.AchievementMessage msg)
        {
            if (Achievements.TryGetValue((global::Achievements.AchievementName)msg.AchievementId, out var value) && value.ActivatedByServer)
            {
                value.Achieve();
            }
        }
    }
}
