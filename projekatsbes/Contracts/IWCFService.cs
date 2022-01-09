using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Security;
using System.Security;

namespace Contracts
{
    [ServiceContract]
    public interface IWCFService
    {

        [OperationContract]
        [FaultContract(typeof(SecurityException))]
        string ReadFile(string fileName);

        [OperationContract]
        [FaultContract(typeof(SecurityException))]
        void Delete(string fileName);

        [OperationContract]
        [FaultContract(typeof(SecurityException))]
        void Rename(string oldFileName, string newFileName);

        [OperationContract]
        [FaultContract(typeof(SecurityException))]
        string ShowFolderContent(string folderName);

        [OperationContract]
        [FaultContract(typeof(SecurityException))]
        void CreateFolder(string folderName);

        [OperationContract]
        [FaultContract(typeof(SecurityException))]
        void CreateFile(string fileName, string text);

        [OperationContract]
        [FaultContract(typeof(SecurityException))]
        void MoveTo(string startFoloder, string destinationFoloder);  



    }
}
