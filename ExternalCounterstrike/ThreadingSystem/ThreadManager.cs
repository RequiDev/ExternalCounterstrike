using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace ExternalCounterstrike.ThreadingSystem
{
    internal static class ThreadManager
    {
        private static readonly Dictionary<Thread, ThreadFunction> Functions = new Dictionary<Thread, ThreadFunction>();

        public static void Add(ThreadFunction funcThread)
        {
            if (!Functions.ContainsValue(funcThread))
                Functions.Add(new Thread(new ThreadStart(funcThread.Function)), funcThread);
        }

        public static void RunAll()
        {
            foreach (var func in Functions)
            {
                Run(func.Value.Name);
            }
        }

        public static void Run(string name)
        {
            var foundFunc = Functions.FirstOrDefault(ft => ft.Value.Name == name);
            if (foundFunc.Key.IsAlive) return;
            foundFunc.Key.Start();
        }
    }
}
