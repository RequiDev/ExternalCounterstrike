using ExternalCounterstrike.ConsoleSystem;
using System.Collections.Generic;
using System.Linq;

namespace ExternalCounterstrike.CommandSystem
{
    internal static class CommandHandler
    {
        public static List<Command> Commands = new List<Command>();
        public static void Worker()
        {
            while (ExternalCounterstrike.IsAttached)
            {
                var fullCommand = Console.ReadLine();
                var commandArray = fullCommand.ToLower().Split(' ');
                var command = commandArray[0];
                var param = commandArray.Length > 1 ? commandArray[1] : "";
                var value = commandArray.Length > 2 ? commandArray[2] : "";
                HandleCommand(command, param, value);
                Console.WriteCommandLine();
            }
        }

        public static void Setup()
        {
            Console.Title = Utils.RandomString(new System.Random().Next(10, 32));
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteWatermark();
            Commands.Add(new Command("aimbot", "Auto. aims for you by pressing the set key when the enemy is in the set fov."));
            Commands.Add(new Command("misc", "Misc options for fun etc."));
            AddParameter("aimbot", "key", "1", "Key for aimbot activation");
            AddParameter("aimbot", "fov", "1", "Field of view of the aimbot");
            AddParameter("aimbot", "bone", "6", "Bone to aim at");
            AddParameter("misc", "norecoil", "0", "Controls the recoil of the existing gun");
        }

        private static void AddParameter(string command, string parameter, string defaultValue, string desc = "This is a basic parameter")
        {
            GetCommand(command).Parameters.Add(new CommandParameter(parameter, new CommandParameterValue(defaultValue), desc));
        }

        private static void HandleCommand(string command, string parameter, string value)
        {
            var cmd = GetCommand(command);
            if (!cmd)
            {
                Console.WriteSuccess($"Could not find command '{command}'.", false);
                return;
            }
            if (parameter == "")
            {
                Console.WriteNotification($"  - {cmd.Name} ({cmd.Description})\n");
                return;
            }
            var param = GetParameter(command, parameter);
            if (!param)
            {
                Console.WriteSuccess($"Could not find parameter '{parameter}' in command '{command}'.", false);
                return;
            }
            if(value == "")
            {
                Console.WriteNotification($"  - {cmd.Name} {param.Name} ({param.Description})\n    Current value of '{command} {parameter}' is {GetParameter(command, parameter).Value}\n");
                return;
            }
            if (!param.IsFunction)
            {
                param.Value = new CommandParameterValue(value);
                if(param.Value.ToFloat() < 0.0f)
                {
                    Console.WriteSuccess($"Value has to be convertable to a digit", false);
                    return;
                }
                Console.WriteNotification($"Set value of '{command} {parameter}' to '{value}'.");
                return;
            }

            switch(command)
            {
                case "load":
                    //load settings
                    break;
            }
        }

        private static Command GetCommand(string command)
        {
            return Commands.FirstOrDefault(com => com.Name == command);

        }
        public static CommandParameter GetParameter(string command, string parameter)
        {
            return GetCommand(command).Parameters.FirstOrDefault(param => param.Name == parameter);
        }
    }
}
