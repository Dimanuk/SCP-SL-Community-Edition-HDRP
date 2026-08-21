using System;
using Mirror;

namespace Hints
{
    public abstract class NetworkObject<TData> : NetworkMessage, IDisposable
    {
        public TData Data { get; protected set; }

        public bool Awoken { get; private set; }

        public abstract void Deserialize(NetworkReader reader);

        public abstract void Serialize(NetworkWriter writer);

        public void Awake(TData data)
        {
            if (this.Awoken)
            {
                throw new InvalidOperationException("Hint has already been awoken.");
            }

            this.Awoken = true;
            this.Data = data;

            this.Start();
        }

        protected virtual void Start()
        {
        }

        protected virtual void Destroy()
        {
        }

        public void Dispose()
        {
            this.Destroy();

            this.Awoken = false;
            this.Data = default(TData);
        }
    }
}