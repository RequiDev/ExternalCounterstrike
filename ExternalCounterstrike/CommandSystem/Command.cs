using System.Collections.Generic;

namespace ExternalCounterstrike.CommandSystem
{
    internal class Command
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<CommandParameter> Parameters { get; set; }

        public Command(string name, string desc = "This is a basic command")
        {
            Name = name;
            Description = desc;
            Parameters = new List<CommandParameter>();
        }

        public static bool operator !(Command cmd)
        {
            return cmd == null;
        }
    }
}
