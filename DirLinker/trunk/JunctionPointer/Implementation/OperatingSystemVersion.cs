using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirLinker.Interfaces;

namespace DirLinker.Implementation
{
    class OperatingSystemVersion : IOperatingSystemVersion
    {
        public Boolean IsXp()
        {
            var osVersion = Environment.OSVersion;
            return osVersion.Version.Major == 5 && osVersion.Version.Minor > 0;
        }

        public Boolean IsVistaOrLater()
        {
            var osVersion = Environment.OSVersion;
            return osVersion.Version.Major >= 6;
        }
    }
}
