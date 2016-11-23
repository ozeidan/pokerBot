using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Bot.MyExceptions;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Bot
{
    class HandleGetter
    {
        [DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);



        private static Queue<IntPtr> handles = new Queue<IntPtr>();
        private static List<IntPtr> usedHandles = new List<IntPtr>();
        
        public static IntPtr Handle
        {
            get
            {
                if(handles.Count != 0)
                {
                    return handles.Dequeue();
                }
                else
                {
                    throw new WindowNotFoundException();
                }
            }
        }

        public static void getHandle()
        {
            while (true)
            {
                var processlist = Process.GetProcesses().Where(p => p.MainWindowHandle != IntPtr.Zero && p.ProcessName != "explorer");

                foreach (Process process in processlist)
                {
                    if (process.ProcessName.Equals("PokerStars"))
                    {
                        IntPtr handle = process.MainWindowHandle;
                        const int nChars = 256;
                        StringBuilder Buff = new StringBuilder(nChars);

                        if (GetWindowText(handle, Buff, nChars) > 0)
                        {
                            string s = Buff.ToString().Substring(0, 4);
                            if (s.Equals("NLHE") && !usedHandles.Contains(handle))
                            {
                                handles.Enqueue(handle);
                                usedHandles.Add(handle);
                            }
                        }
                    }
                }
                Thread.Sleep(500);
            }

        }
    }
}
