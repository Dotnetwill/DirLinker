using System;

namespace JunctionPointer.Interfaces.Controllers
{
    public interface IWorkingController
    {
        void DoDirectoryLinkWithFeedBack(string linkpoint, string linkTo, bool copyBeforeDelete, Boolean overwriteTargetFiles);
    }
}
