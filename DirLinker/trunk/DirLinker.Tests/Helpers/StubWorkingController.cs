using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirLinker.Interfaces.Controllers;

namespace DirLinker.Tests.Helpers
{
    class StubWorkingController : IWorkingController
    {
        #region IWorkingController Members

        public String LinkPoint;
        public String LinkTo;
        public Boolean CopyBeforeDelete;

        public object OverWriteTargetFiles { get; private set; }

        public void DoDirectoryLinkWithFeedBack(string linkpoint, string linkTo, bool copyBeforeDelete, Boolean overwriteTargetFiles)
        {
            LinkPoint = linkpoint;
            LinkTo = linkTo;
            CopyBeforeDelete = copyBeforeDelete;
            OverWriteTargetFiles = overwriteTargetFiles;
        }

        #endregion
    }
}
