using System;
using LiteNetLib.Utils;
public readonly struct PreauthCancellationData
{
    public bool IsCancelled { get; }

    public static PreauthCancellationData RejectDelay(byte seconds, bool isForced)
    {
        if (seconds < 1 || seconds > 25)
        {
            throw new Exception("Delay duration must be between 1 and 25 seconds.");
        }
        return new PreauthCancellationData(RejectionReason.Delay, isForced, null, 0L, seconds, 0, null, true, false);
    }

    public static PreauthCancellationData RejectRedirect(ushort port, bool isForced)
    {
        return new PreauthCancellationData(RejectionReason.Redirect, isForced, null, 0L, 0, port, null, true, false);
    }

    public static PreauthCancellationData RejectBanned(string banReason, DateTime expiration, bool isForced)
    {
        return PreauthCancellationData.RejectBanned(banReason, expiration.Ticks, isForced);
    }

    public static PreauthCancellationData RejectBanned(string banReason, long expiration, bool isForced)
    {
        if (banReason.Length > 400)
        {
            throw new ArgumentOutOfRangeException("banReason", "Reason can't be longer than 400 characters.");
        }
        return new PreauthCancellationData(RejectionReason.Banned, isForced, banReason, expiration, 0, 0, null, true, false);
    }

    public static PreauthCancellationData Reject(string customReason, bool isForced)
    {
        if (string.IsNullOrEmpty(customReason) || customReason.Length > 400)
        {
            throw new ArgumentOutOfRangeException("customReason", "Reason can't be null, empty or longer than 400 characters.");
        }
        return new PreauthCancellationData(RejectionReason.Custom, isForced, customReason, 0L, 0, 0, null, true, false);
    }

    public static PreauthCancellationData Reject(RejectionReason reason, bool isForced)
    {
        if (reason == RejectionReason.Banned || reason == RejectionReason.Custom || reason - RejectionReason.Redirect <= 1)
        {
            throw new Exception("Specified reason requires extra parameters. Please use the appropriate method.");
        }
        return new PreauthCancellationData(reason, isForced, null, 0L, 0, 0, null, true, false);
    }

    public static PreauthCancellationData Reject(NetDataWriter writer, bool isForced)
    {
        return new PreauthCancellationData(RejectionReason.NotSpecified, isForced, null, 0L, 0, 0, writer, true, false);
    }

    public static PreauthCancellationData Accept()
    {
        return new PreauthCancellationData(RejectionReason.NotSpecified, false, null, 0L, 0, 0, null, false, false);
    }

    public static PreauthCancellationData HandledManually()
    {
        return new PreauthCancellationData(RejectionReason.NotSpecified, false, null, 0L, 0, 0, null, true, true);
    }

    private PreauthCancellationData(RejectionReason rejectionReason, bool isForced, string customReason = null, long expiration = 0L, byte seconds = 0, ushort port = 0, NetDataWriter writer = null, bool isCancelled = true, bool handledManually = false)
    {
        this.IsCancelled = isCancelled;
        this._customWriter = null;
        this._reason = rejectionReason;
        this._isForced = isForced;
        this._customReason = customReason;
        this._expiration = expiration;
        this._seconds = seconds;
        this._port = port;
        this._customWriter = writer;
        this._handledManually = handledManually;
    }

    public NetDataWriter GenerateWriter(out bool forced)
    {
        forced = this._isForced;
        if (!this.IsCancelled || this._handledManually)
        {
            return null;
        }
        if (this._reason == RejectionReason.NotSpecified && this._customWriter != null)
        {
            return this._customWriter;
        }
        NetDataWriter netDataWriter = new NetDataWriter();
        netDataWriter.Put((byte)this._reason);
        RejectionReason reason = this._reason;
        if (reason <= RejectionReason.Custom)
        {
            if (reason != RejectionReason.Banned)
            {
                if (reason == RejectionReason.Custom)
                {
                    netDataWriter.Put(this._customReason);
                }
            }
            else
            {
                netDataWriter.Put(this._expiration);
                netDataWriter.Put(this._customReason);
            }
        }
        else if (reason != RejectionReason.Redirect)
        {
            if (reason == RejectionReason.Delay)
            {
                netDataWriter.Put(this._seconds);
            }
        }
        else
        {
            netDataWriter.Put(this._port);
        }
        return netDataWriter;
    }

    private readonly bool _handledManually;

    private readonly NetDataWriter _customWriter;

    private readonly RejectionReason _reason;

    private readonly bool _isForced;

    private readonly byte _seconds;

    private readonly long _expiration;

    private readonly string _customReason;

    private readonly ushort _port;
}
