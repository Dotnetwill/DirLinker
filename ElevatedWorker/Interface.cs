using System.ServiceModel;

namespace ElevatedWorker
{
    [ServiceContract]
    public interface IElevatedWorker
    {
        [OperationContract]
        void CreateJunctionPoint(string junctionPoint, string target);

        [OperationContract]
        void CreateLinkToFolderAt(string linkToBeCreated, string folder);
    }
}