using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirLinker.Data;

namespace DirLinker.Implementation
{
    public class OperationValidation
    {
        public Boolean ValidOperation(LinkOperationData linkData, out String errorMessage)
        {
            errorMessage = String.Empty;
            return true;
        }
    }
}
