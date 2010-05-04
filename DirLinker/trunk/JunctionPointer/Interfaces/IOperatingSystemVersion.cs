using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DirLinker.Interfaces
{
    public interface IOperatingSystemVersion
    {
        Boolean IsVistaOrLater();
        Boolean IsXp();
    }
}
