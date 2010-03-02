using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JunctionPointer.Interfaces
{
    
    public interface ICommandDiscovery
    {
        List<ICommand> GetCommandListForTask(IFolder linkTo, Boolean copyBeforeDelete, Boolean overwriteTargetFiles);
    }
}
