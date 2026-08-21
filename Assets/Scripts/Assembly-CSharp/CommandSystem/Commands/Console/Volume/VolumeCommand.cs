using System;

namespace CommandSystem.Commands.Console.Volume
{
    public class VolumeCommand : ParentCommand
    {
        public override string Command { get; } = "volume";

        public override string[] Aliases { get; }

        public override string Description { get; } = "Modify the volume.";

        public static VolumeCommand Create()
        {
            VolumeCommand volumeCommand = new VolumeCommand();
            volumeCommand.LoadGeneratedCommands();
            return volumeCommand;
        }

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            response = "SYNTAX: volume (master, voice or effect) 0 to 1.0.\n         Example: \"volume master 0.5\" makes master volume 50%.";
            return false;
        }

        public override void LoadGeneratedCommands()
        {
            RegisterCommand(new EffectCommand());
            RegisterCommand(new MasterCommand());
            RegisterCommand(new VoiceCommand());
        }
    }
}
