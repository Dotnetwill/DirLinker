using System;
using System.Windows.Forms;

namespace JunctionPointer.Interfaces
{
    public delegate DialogResult RequestUserReponse(String question, MessageBoxButtons options);

    public interface ICommand
    {
        
        void Execute();
        void Undo();
        
        String Status { get; }

        event RequestUserReponse AskUser;
    }
}
