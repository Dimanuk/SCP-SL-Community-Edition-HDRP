using System;
using System.Text;
using NorthwoodLib.Pools;
using RemoteAdmin.Communication;
using UnityEngine;
using UnityEngine.UI;

namespace RemoteAdmin.Menus
{
    public class DataRequestMenu : RaCommandMenu
    {
        public enum RequestType
        {
            Info = 0,
            ShortInfo = 1,
            Auth = 2,
            ExternalLookup = 3
        }

        [SerializeField]
        private Button _externalLookup;

        public void Query(RequestType requestType)
        {
            StringBuilder sb = StringBuilderPool.Shared.Rent();

            try
            {
                foreach (PlayerRecord record in PlayerRecord.Instances)
                {
                    if (!record.IsSelected)
                        continue;

                    if (requestType != RequestType.ExternalLookup)
                    {
                        sb.Append(record.PlayerId);
                        sb.Append('.');
                    }
                    else
                    {
                        sb.Append("EXTERNALLOOKUP ");
                        sb.Append(record.PlayerId);
                    }
                }

                if (sb.Length == 0 && requestType != RequestType.ExternalLookup)
                    return;

                string built = StringBuilderPool.Shared.ToStringReturn(sb);
                sb = null; 

                ReferenceHub localHub = ReferenceHub.LocalHub;
                if (localHub == null)
                    throw new NullReferenceException();

                QueryProcessor queryProcessor = localHub.queryProcessor;

                switch (requestType)
                {
                    case RequestType.Info:
                        RaClipboard.Reset();
                        RaPlayer.Request(false, built);
                        break;

                    case RequestType.ShortInfo:
                        RaClipboard.Reset();
                        RaPlayer.Request(true, built);
                        break;

                    case RequestType.Auth:
                        if (ReferenceHub.TryGetLocalHub(out ReferenceHub hub))
                            hub.queryProcessor.CmdSendQuery("$3 " + built, false);
                        break;

                    case RequestType.ExternalLookup:
                        string mode = ServerConfigSynchronizer.Singleton.RemoteAdminExternalPlayerLookupMode;
                        if (mode == "fullauth" || mode == "urlonly")
                        {
                            queryProcessor.ExpectingURL = DateTime.Now.AddSeconds(10.0);
                            queryProcessor.CmdSendQuery(built, false);
                        }
                        break;
                }
            }
            finally
            {
                if (sb != null)
                    StringBuilderPool.Shared.Return(sb);
            }
        }

        public void Query(string requestType)
        {
            // Scene buttons pass lowercase names ("info", "auth", "shortinfo",
            // "externallookup"); the original parses case-insensitively.
            if (Enum.TryParse(requestType, true, out RequestType type))
                Query(type);
        }

        protected override void OnStart()
        {
            string mode = ServerConfigSynchronizer.Singleton?.RemoteAdminExternalPlayerLookupMode;
            if (mode != null && mode.Contains("disabled"))
            {
                if (_externalLookup != null)
                    _externalLookup.interactable = false;
            }
        }
    }
}