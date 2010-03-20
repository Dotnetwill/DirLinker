using System;

namespace DirLinker.Interfaces.Controllers
{
    public interface IWorkingController
    {
        void DoDirectoryLinkWithFeedBack(string linkpoint, string linkTo, bool copyBeforeDelete, Boolean overwriteTargetFiles);
    }
}
