using System;
using System.Text;
using RemoteAdmin.Interfaces;

namespace RemoteAdmin.Communication
{
    public abstract class RaClientDataRequest : IServerCommunication, IClientCommunication
    {
        private readonly StringBuilder _stringBuilder = new StringBuilder();

        public abstract int DataId { get; }

        public event Action<string> OnClientReceiveData;

        public virtual void ReceiveData(string data, bool secure)
        {
            OnClientReceiveData?.Invoke(data);
        }

        public void Request()
        {
            if (!ReferenceHub.TryGetLocalHub(out ReferenceHub hub))
                return;

            hub.queryProcessor.CmdSendQuery($"${DataId} 0", false);
        }

        public virtual void ReceiveData(CommandSender sender, string data)
        {
            _stringBuilder.Clear();
            _stringBuilder.Append('$').Append(DataId).Append(' ');
            GatherData();
            sender.RaReply($"${DataId} {_stringBuilder}", success: true, logToConsole: false, string.Empty);
        }

        protected abstract void GatherData();

        protected void AppendData(object data)
        {
            _stringBuilder.Append(data).Append(',');
        }

        protected int CastBool(bool value)
        {
            return value ? 1 : 0;
        }
    }
}