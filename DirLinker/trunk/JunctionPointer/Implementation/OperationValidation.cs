using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirLinker.Data;
using DirLinker.Interfaces;

namespace DirLinker.Implementation
{
    public class OperationValidation : IOperationValidation
    {
        private readonly IFileFactoryForPath _fileFactory;

        public OperationValidation(Interfaces.IFileFactoryForPath fileFactory)
        {
            _fileFactory = fileFactory;
        }

        public Boolean ValidOperation(LinkOperationData linkData, out String errorMessage)
        {
            errorMessage = String.Empty;

            if(PathsMatch(linkData))
            {
                errorMessage = "A path can not be linked to itself";
                return false;
            }
            if(!IsValidFileLink(linkData))
            {
                errorMessage = "When creating a file link the linked to file must exist";
                return false;
            }

            return true;
        }

        private Boolean IsValidFileLink(LinkOperationData linkData)
        {
            IFile createLinkAtFile = _fileFactory(linkData.CreateLinkAt);
            IFile linkToFile = _fileFactory(linkData.LinkTo);

            if(createLinkAtFile.Exists())
            {
                return linkToFile.Exists() || linkData.CopyBeforeDelete;
            }

            return true;
        }

        private Boolean PathsMatch(LinkOperationData linkData)
        {
            return linkData.CreateLinkAt.Equals(linkData.LinkTo, StringComparison.CurrentCultureIgnoreCase);
        }
    }
}
