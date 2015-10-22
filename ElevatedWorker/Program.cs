using System;
using System.Diagnostics;
using System.ServiceModel;
using System.Windows.Forms;
using System.Xml;

namespace ElevatedWorker
{
    static class Program
    {

        [STAThread]
        static void Main()
        {
            AppDomain.CurrentDomain.UnhandledException += (s, e) => MessageBox.Show((e.ExceptionObject as Exception).Message);

            using (var srv = new ServiceHost(new Server()))
            {
                srv.AddServiceEndpoint(typeof(IElevatedWorker), new NetNamedPipeBinding()
                {
                    ReceiveTimeout = TimeSpan.FromMinutes(1),
                    SendTimeout = TimeSpan.FromMinutes(1),
                    CloseTimeout = TimeSpan.FromMinutes(1),
                    OpenTimeout = TimeSpan.FromMinutes(1),
                    ReaderQuotas = new XmlDictionaryReaderQuotas() { MaxArrayLength = int.MaxValue, MaxBytesPerRead = int.MaxValue },
                    MaxReceivedMessageSize = int.MaxValue
                }, string.Format("net.pipe://localhost/{0}", "DirLinker_ElevatedWorker"));
                srv.Open();
                Process.GetCurrentProcess().Parent().WaitForExit();
            }
        }
    }




}
