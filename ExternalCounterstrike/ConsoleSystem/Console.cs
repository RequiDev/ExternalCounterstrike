namespace ExternalCounterstrike.ConsoleSystem
{
    internal class Console
    {
        #region VARIABLES
        private static string _commandLine = ">" + System.Environment.UserName + ": ";
        public static string CommandLine
        {
            get
            {
                return _commandLine;
            }
            set
            {
                _commandLine = value;
            }
        }

        private static string _waterMark = "ExternalCounterstrike v0.1 by Requi";
        public static string WaterMark
        {
            get
            {
                return _waterMark;
            }
            set
            {
                _waterMark = value;
            }
        }
        #endregion

        #region PROPERTIES
        public static ConsoleColor BackgroundColor
        {
            get
            {
                return (ConsoleColor)System.Console.BackgroundColor;
            }
            set
            {
                System.Console.BackgroundColor = (System.ConsoleColor)value;
            }
        }

        public static int BufferHeight
        {
            get
            {
                return System.Console.BufferHeight;
            }
            set
            {
                System.Console.BufferHeight = value;
            }
        }

        public static int BufferWidth
        {
            get
            {
                return System.Console.BufferWidth;
            }
            set
            {
                System.Console.BufferWidth = value;
            }
        }

        public static bool CapsLock
        {
            get
            {
                return System.Console.CapsLock;
            }
        }

        public static int CursorLeft
        {
            get
            {
                return System.Console.CursorLeft;
            }
            set
            {
                System.Console.CursorLeft = value;
            }
        }

        public static int CursorSize
        {
            get
            {
                return System.Console.CursorSize;
            }
            set
            {
                System.Console.CursorSize = value;
            }
        }

        public static int CursorTop
        {
            get
            {
                return System.Console.CursorTop;
            }
            set
            {
                System.Console.CursorTop = value;
            }
        }

        public static bool CursorVisible
        {
            get
            {
                return System.Console.CursorVisible;
            }
            set
            {
                System.Console.CursorVisible = value;
            }
        }

        public static System.IO.TextWriter Error
        {
            get
            {
                return System.Console.Error;
            }
        }

        public static ConsoleColor ForegroundColor
        {
            get
            {
                return (ConsoleColor)System.Console.ForegroundColor;
            }
            set
            {
                System.Console.ForegroundColor = (System.ConsoleColor)value;
            }
        }

        public static System.IO.TextReader In
        {
            get
            {
                return System.Console.In;
            }
        }

        public static System.Text.Encoding InputEncoding
        {
            get
            {
                return System.Console.InputEncoding;
            }
            set
            {
                System.Console.InputEncoding = value;
            }
        }

        public static bool IsErrorRedirected
        {
            get
            {
                return System.Console.IsErrorRedirected;
            }
        }

        public static bool IsInputRedirected
        {
            get
            {
                return System.Console.IsInputRedirected;
            }
        }

        public static bool IsOutputRedirected
        {
            get
            {
                return System.Console.IsOutputRedirected;
            }
        }

        public static bool KeyAvailable
        {
            get
            {
                return System.Console.KeyAvailable;
            }
        }

        public static int LargestWindowHeight
        {
            get
            {
                return System.Console.LargestWindowHeight;
            }
        }

        public static int LargestWindowWidth
        {
            get
            {
                return System.Console.LargestWindowWidth;
            }
        }

        public static bool NumberLock
        {
            get
            {
                return System.Console.NumberLock;
            }
        }

        public static System.IO.TextWriter Out
        {
            get
            {
                return System.Console.Out;
            }
        }

        public static System.Text.Encoding OutputEncoding
        {
            get
            {
                return System.Console.OutputEncoding;
            }
            set
            {
                System.Console.OutputEncoding = value;
            }
        }
        public static string Title
        {
            get
            {
                return System.Console.Title;
            }
            set
            {
                System.Console.Title = value;
            }
        }

        public static bool TreatControlCAsInput
        {
            get
            {
                return System.Console.TreatControlCAsInput;
            }
            set
            {
                System.Console.TreatControlCAsInput = value;
            }
        }

        public static int WindowHeight
        {
            get
            {
                return System.Console.WindowHeight;
            }
            set
            {
                System.Console.WindowHeight = value;
            }
        }

        public static int WindowLeft
        {
            get
            {
                return System.Console.WindowLeft;
            }
            set
            {
                System.Console.WindowLeft = value;
            }
        }

        public static int WindowTop
        {
            get
            {
                return System.Console.WindowTop;
            }
            set
            {
                System.Console.WindowTop = value;
            }
        }

        public static int WindowWidth
        {
            get
            {
                return System.Console.WindowWidth;
            }
            set
            {
                System.Console.WindowWidth = value;
            }
        }
        #endregion

        #region METHODS
        public static void Beep()
        {
            System.Console.Beep();
        }
        public static void Beep(int frequency, int duration)
        {
            System.Console.Beep(frequency, duration);
        }
        public static void Clear()
        {
            System.Console.Clear();
        }
        public static void Beep(int sourceLeft, int sourceTop, int sourceWidth, int sourceHeight, int targetLeft, int targetTop)
        {
            System.Console.MoveBufferArea(sourceLeft, sourceTop, sourceWidth, sourceHeight, targetLeft, targetTop);
        }
        public static void Beep(int sourceLeft, int sourceTop, int sourceWidth, int sourceHeight, int targetLeft, int targetTop, char sourceChar, System.ConsoleColor sourceForeColor, System.ConsoleColor sourceBackColor)
        {
            System.Console.MoveBufferArea(sourceLeft, sourceTop, sourceWidth, sourceHeight, targetLeft, targetTop, sourceChar, sourceForeColor, sourceBackColor);
        }
        public static System.IO.Stream OpenStandardError()
        {
            return System.Console.OpenStandardError();
        }
        public static System.IO.Stream OpenStandardError(int bufferSize)
        {
            return System.Console.OpenStandardError(bufferSize);
        }
        public static System.IO.Stream OpenStandardInput()
        {
            return System.Console.OpenStandardInput();
        }
        public static System.IO.Stream OpenStandardInput(int bufferSize)
        {
            return System.Console.OpenStandardInput(bufferSize);
        }
        public static System.IO.Stream OpenStandardOutput()
        {
            return System.Console.OpenStandardOutput();
        }
        public static System.IO.Stream OpenStandardOutput(int bufferSize)
        {
            return System.Console.OpenStandardOutput(bufferSize);
        }
        public static int Read()
        {
            return System.Console.Read();
        }
        public static System.ConsoleKeyInfo ReadKey()
        {
            return System.Console.ReadKey();
        }
        public static System.ConsoleKeyInfo ReadKey(bool intercept)
        {
            return System.Console.ReadKey(intercept);
        }
        public static string ReadLine()
        {
            return System.Console.ReadLine();
        }
        public static void ResetColor()
        {
            System.Console.ResetColor();
        }
        public static void SetBufferSize(int width, int height)
        {
            System.Console.SetBufferSize(width, height);
        }
        public static void SetCursorPosition(int left, int top)
        {
            System.Console.SetCursorPosition(left, top);
        }
        public static void SetError(System.IO.TextWriter newError)
        {
            System.Console.SetError(newError);
        }
        public static void SetIn(System.IO.TextReader newIn)
        {
            System.Console.SetIn(newIn);
        }
        public static void SetOut(System.IO.TextWriter newOut)
        {
            System.Console.SetOut(newOut);
        }
        public static void SetWindowPosition(int left, int top)
        {
            System.Console.SetWindowPosition(left, top);
        }
        public static void SetWindowSize(int width, int height)
        {
            System.Console.SetWindowSize(width, height);
        }
        public static void WriteLine(string value)
        {
            System.Console.WriteLine(value);
        }
        public static void Write(string value)
        {
            System.Console.Write(value);
        }
        #endregion

        #region MY METHODS
        public static void WriteSuccess(string value, bool sucess = true)
        {
            ForegroundColor = sucess ? ConsoleColor.Green : ConsoleColor.Red;
            WriteLine(value);
            ForegroundColor = ConsoleColor.White;
        }

        public static void WriteNotification(string value)
        {
            ForegroundColor = ConsoleColor.Yellow;
            WriteLine(value);
            ForegroundColor = ConsoleColor.White;
        }

        public static void WriteOffset(string name, int offset, bool netvar = false)
        {
            WriteSuccess($"  \t{name}\t0x{offset.ToString("X").PadLeft(netvar ? 4 : 8, '0')}", offset != 0);
        }

        public static void WriteCommandLine()
        {
            Write(CommandLine);
        }

        public static void WriteCustomLine(string value)
        {
            WriteLine(value);
            WriteCommandLine();
        }

        public static void WriteWatermark()
        {
            Console.Clear();
            Console.WriteLine(string.Format("{0," + ((Console.WindowWidth / 2) + (WaterMark.Length / 2)) + "}", WaterMark));
            Console.Write("\n\n\n");
        }
        #endregion
    }
}
