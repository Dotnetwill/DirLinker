using System;
using System.Collections.Generic;


namespace DirLinker.Interfaces
{
    
    public interface ICommandDiscovery
    {
        List<ICommand> GetCommandListForTask(IFolder linkTo, IFolder linkFrom, Boolean copyBeforeDelete, Boolean overwriteTargetFiles);
    }
}
