using System;
using Mirror;

namespace Security
{
    public class PlayerRateLimitHandler : NetworkBehaviour
    {
        public void Awake()
        {
            this.RateLimits = RateLimitCreator.CreateRateLimit(base.connectionToClient, base.isServer && base.isLocalPlayer);
        }

        public RateLimit[] RateLimits;
    }
}
