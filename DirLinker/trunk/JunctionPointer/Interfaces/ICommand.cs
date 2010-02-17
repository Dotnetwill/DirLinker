using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JunctionPointer.Interfaces
{
    public interface ICommand
    {
        void Execute();
        void Undo();
        String Status { get; }

    }
}
