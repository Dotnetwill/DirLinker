using System;
using JunctionPointer.Interfaces.Views;
using JunctionPointer.Interfaces;
using System.Windows.Forms;

namespace JunctionPointer.Interfaces.Controllers
{
    public interface IMainController
    {
        void ValidatePath(object sender, ValidationArgs e);
        void PerformOperation(object sender, EventArgs e);

        Form Start();

    }
}
