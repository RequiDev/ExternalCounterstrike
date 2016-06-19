namespace ExternalCounterstrike.CommandSystem
{
    internal class CommandParameter
    {
        public string Name { get; set; }
        public CommandParameterValue Value { get; set; }
        public bool IsFunction { get; set; }
        public string Description { get; set; }

        public CommandParameter(string name, CommandParameterValue value, string desc = "This is a basic parameter", bool isFunc = false)
        {
            Name = name;
            Description = desc;
            IsFunction = isFunc;
            Value = value;
        }

        public static bool operator !(CommandParameter cmdParam)
        {
            return cmdParam == null;
        }
    }
}
