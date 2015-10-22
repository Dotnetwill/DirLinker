using System.ServiceModel;

namespace ElevatedWorker
{
    [ServiceContract]
    public interface IElevatedWorker
    {
        [OperationContract]
        void CreateJunctionPoint(string junctionPoint, string target);

        [OperationContract]
        bool CreateLinkToFolderAt(string linkToBeCreated, string folder);

        [OperationContract]
        bool CreateLinkToFileAt(string linkToBeCreated, string path);
    }
}