using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

namespace ElevatedWorker
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            AppDomain.CurrentDomain.UnhandledException += (s, e) => MessageBox.Show((e.ExceptionObject as Exception).Message);

            using (var Signal = new ManualResetEvent(false))
            {
                new Thread(() =>
                {
                    Server.Startup("DirLinker_ElevatedWorker", Signal);
                }).Start();

                Process.GetCurrentProcess().Parent().WaitForExit();
                Signal.Set();
            }
        }
    }




}
