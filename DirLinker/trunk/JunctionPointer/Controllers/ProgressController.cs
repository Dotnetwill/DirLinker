using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirLinker.Interfaces.Controllers;
using DirLinker.Interfaces;
using DirLinker.Interfaces.Views;

namespace DirLinker.Controllers
{
    public class ProgressController : IProgressController
    {
        public ProgressController(IWorkingView view, ICommandDiscovery commandDiscovery)
        {
        }
        
        public void PerformLink(string linkAt, string linkTo, bool moveBeforeDelete, bool overwriteTargetFiles)
        {
        
        }
    }
}
