public abstract class CommandSender : IOutput, global::CommandSystem.ICommandSender
{
    public abstract string SenderId { get; }

    public abstract string Nickname { get; }

    public abstract ulong Permissions { get; }

    public abstract byte KickPower { get; }

    public abstract bool FullPermissions { get; }

    public virtual string LogName => Nickname;

    public abstract void RaReply(string text, bool success, bool logToConsole, string overrideDisplay);

    public abstract void Print(string text);

    public virtual void Print(string text, global::System.ConsoleColor c)
    {
        Print(text);
    }

    public virtual void Print(string text, global::System.ConsoleColor c, global::UnityEngine.Color rgbColor)
    {
        Print(text, c);
    }

    public virtual void Respond(string message, bool success = true)
    {
        RaReply(message, success, logToConsole: true, string.Empty);
    }
}
