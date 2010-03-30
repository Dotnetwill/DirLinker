using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DirLinker.Interfaces.Controllers
{
    public interface IProgressController
    {
        void PerformLink(String linkAt, String linkTo, Boolean moveBeforeDelete, Boolean overwriteTargetFiles);
    }
}
