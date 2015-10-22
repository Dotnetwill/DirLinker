using System;
using System.ServiceModel;
using System.Threading;
using System.Xml;

namespace ElevatedWorker
{
    [ServiceBehavior(
        ConcurrencyMode = ConcurrencyMode.Multiple,
        InstanceContextMode = InstanceContextMode.Single,
        IncludeExceptionDetailInFaults = true,
        MaxItemsInObjectGraph = int.MaxValue)]
    internal sealed class Server : IElevatedWorker
    {
        public void CreateJunctionPoint(string junctionPoint, string target)
        {
            throw new NotImplementedException();
        }

        public void CreateLinkToFolderAt(string linkToBeCreated, string folder)
        {
            throw new NotImplementedException();
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