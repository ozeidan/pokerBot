using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Bot.Exceptions;

namespace Bot
{
    internal class HandleGetter
    {
        private static readonly Queue<IntPtr> Handles = new Queue<IntPtr>();
        private static readonly List<IntPtr> UsedHandles = new List<IntPtr>();

        public static IntPtr Handle
        {
            get
            {
                if (Handles.Count != 0)
                    return Handles.Dequeue();
                throw new WindowNotFoundException();
            }
        }

        [DllImport("user32.dll")]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        public static void FindHandles(string processName, string windowName, ref Semaphore sema)
        {
            while (true)
            {
                var processList =
                    Process.GetProcesses().Where(p => p.MainWindowHandle != IntPtr.Zero && p.ProcessName == processName);

                foreach (var process in processList)
                {
                    var handle = process.MainWindowHandle;
                    const int nChars = 256;
                    var buff = new StringBuilder(nChars);

                    if (GetWindowText(handle, buff, nChars) <= 0)
                        break;

                    var name = buff.ToString();

                    if (!name.Contains(windowName) || UsedHandles.Contains(handle))
                        break;

                    Handles.Enqueue(handle);
                    UsedHandles.Add(handle);
                    sema.Release();
                }
                Thread.Sleep(500);
            }
        }
    }
}