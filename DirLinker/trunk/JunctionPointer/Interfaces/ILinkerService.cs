using System;
using DirLinker.Data;

namespace DirLinker.Interfaces
{
    public interface ILinkerService
    {
        void PerformLinkOperation(LinkOperationData operation);

    }
}
