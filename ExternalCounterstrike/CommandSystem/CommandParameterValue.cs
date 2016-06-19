namespace ExternalCounterstrike.CommandSystem
{
    internal class CommandParameterValue
    {
        public string Value { get; set; }
        public CommandParameterValue(string value)
        {
            Value = value;
        }

        public override string ToString()
        {
            return Value;
        }

        public int ToInt32()
        {
            try
            {
                var refVal = int.Parse(Value);
                return refVal;
            }
            catch { }

            return -1;
        }

        public float ToFloat()
        {
            try
            {
                var refVal = float.Parse(Value);
                return refVal;
            }
            catch { }
            return -1.0f;
        }

        public bool ToBool()
        {
            return !(ToInt32() < 1);
        }

        public static bool operator !(CommandParameterValue cmdParamValue)
        {
            return cmdParamValue == null;
        }
    }
}
