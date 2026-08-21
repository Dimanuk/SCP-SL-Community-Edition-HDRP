public class RoundPlayerHistory : global::UnityEngine.MonoBehaviour
{
    [global::System.Serializable]
    public class PlayerHistoryLog
    {
        public string Nickname;

        public int PlayerId;

        public string UserId;

        public int ConnectionStatus;

        public int LastAliveClass;

        public int CurrentClass;

        public global::System.DateTime ConnectionStart;

        public global::System.DateTime ConnectionStop;
    }

    public static RoundPlayerHistory singleton;

    public global::System.Collections.Generic.List<RoundPlayerHistory.PlayerHistoryLog> historyLogs = new global::System.Collections.Generic.List<RoundPlayerHistory.PlayerHistoryLog>();

    private void Awake()
    {
        singleton = this;
    }

    public RoundPlayerHistory.PlayerHistoryLog GetData(int playerId)
    {
        foreach (RoundPlayerHistory.PlayerHistoryLog historyLog in historyLogs)
        {
            if (historyLog.PlayerId == playerId)
            {
                return historyLog;
            }
        }
        return null;
    }

    public void SetData(int playerId, string newNick, int newPlayerId, string newUserId, int newConnectionStatus, int newAliveClass, int newCurrentClass, global::System.DateTime newStartTime, global::System.DateTime newStopTime)
    {
        int num = -1;
        if (playerId == -1)
        {
            historyLogs.Add(new RoundPlayerHistory.PlayerHistoryLog
            {
                Nickname = "Player",
                PlayerId = 0,
                UserId = string.Empty,
                ConnectionStatus = 0,
                LastAliveClass = -1,
                CurrentClass = -1,
                ConnectionStart = global::System.DateTime.Now,
                ConnectionStop = new global::System.DateTime(0, 0, 0)
            });
            num = historyLogs.Count - 1;
        }
        else
        {
            for (int i = 0; i < historyLogs.Count; i++)
            {
                if (historyLogs[i].PlayerId == playerId)
                {
                    num = i;
                }
            }
        }
        if (num >= 0)
        {
            if (newNick != string.Empty)
            {
                historyLogs[num].Nickname = newNick;
            }
            if (newPlayerId != 0)
            {
                historyLogs[num].PlayerId = newPlayerId;
            }
            if (newUserId != string.Empty)
            {
                historyLogs[num].UserId = newUserId;
            }
            if (newConnectionStatus != 0)
            {
                historyLogs[num].ConnectionStatus = newConnectionStatus;
            }
            if (newAliveClass != 0)
            {
                historyLogs[num].LastAliveClass = newAliveClass;
            }
            if (newCurrentClass != 0)
            {
                historyLogs[num].CurrentClass = newCurrentClass;
            }
            if (newStartTime.Year != 0)
            {
                historyLogs[num].ConnectionStart = newStartTime;
            }
            if (newStopTime.Year != 0)
            {
                historyLogs[num].ConnectionStop = newStopTime;
            }
        }
    }
}
