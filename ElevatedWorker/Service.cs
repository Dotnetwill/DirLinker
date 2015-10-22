using System;
using System.Runtime.InteropServices;
using System.ServiceModel;
using System.Threading;
using System.Xml;

namespace ElevatedWorker
{
    public enum SYMBOLIC_LINK_FLAG
    {
        File = 0,
        Directory = 1
    }

    [ServiceBehavior(
        ConcurrencyMode = ConcurrencyMode.Multiple,
        InstanceContextMode = InstanceContextMode.Single,
        IncludeExceptionDetailInFaults = true,
        MaxItemsInObjectGraph = int.MaxValue)]
    internal sealed class Server : IElevatedWorker
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.I1)]

        private static extern bool CreateSymbolicLink(String lpSymlinkFileName, String lpTargetFileName, SYMBOLIC_LINK_FLAG dwFlags);

        public void CreateJunctionPoint(string junctionPoint, string target)
        {
            throw new NotImplementedException();
        }

        public bool CreateLinkToFileAt(string linkToBeCreated, string path)
        {
            return CreateSymbolicLink(linkToBeCreated, path, SYMBOLIC_LINK_FLAG.File);
        }

        public bool CreateLinkToFolderAt(string linkToBeCreated, string folder)
        {
            return CreateSymbolicLink(linkToBeCreated, folder, SYMBOLIC_LINK_FLAG.Directory);
        }

        public static void Startup(string AppName, ManualResetEvent StopSignal)
        {
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
                }, string.Format("net.pipe://localhost/{0}", AppName));
                srv.Open();
                StopSignal.WaitOne();
            }
        }
    }

    public static class Client
    {
        private static readonly ChannelFactory<IElevatedWorker> SyncFactory = new ChannelFactory<IElevatedWorker>(
            new NetNamedPipeBinding()
            {
                ReceiveTimeout = TimeSpan.FromMinutes(1),
                SendTimeout = TimeSpan.FromMinutes(1),
                CloseTimeout = TimeSpan.FromMinutes(1),
                OpenTimeout = TimeSpan.FromMinutes(1),
                ReaderQuotas = new XmlDictionaryReaderQuotas() { MaxArrayLength = int.MaxValue, MaxBytesPerRead = int.MaxValue },
                MaxReceivedMessageSize = int.MaxValue
            });

        public static IElevatedWorker Connect(string AppName)
        {
            return (IElevatedWorker)SyncFactory.CreateChannel(new EndpointAddress(string.Format("net.pipe://localhost/{0}", AppName)));
        }
    }
}