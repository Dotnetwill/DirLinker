using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace JunctionPointer.Interfaces
{
    public delegate DialogResult RequestUserReponse(String question, MessageBoxButtons options);

    public interface ICommand
    {
        
        void Execute();
        void Undo();
        
        String Status { get; }
        Boolean Executed { get; }

        event RequestUserReponse AskUser;
    }
}
