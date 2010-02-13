using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using JunctionPointer.Views;
using JunctionPointer.Controllers;
using JunctionPointer.Implemenation;
using JunctionPointer.Interfaces;
using JunctionPointer.Interfaces.Controllers;
using JunctionPointer.Interfaces.Views;
using JunctionPointer.Helpers.Interfaces;
using JunctionPointer.Helpers.OCInject;

namespace JunctionPointer
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            IClassFactory classFactory = new ClassFactory();
            FillIoCContainer(classFactory);

            IMainController mainController = new MainController(classFactory);
            Application.Run(mainController.Start());
        }
        private static void FillIoCContainer(IClassFactory classFactory)
        {

            classFactory.RegisterType<IDirLinker, DirLinker>();
            classFactory.RegisterType<IFile, FileImp>();
            classFactory.RegisterType<IWorkingController, WorkingDialogController>();
            classFactory.RegisterType<ILinkerView, DirLinkerView>();
            classFactory.RegisterType<IWorkingView, ProgessView>();

            classFactory.RegisterType<IFolder, FolderImp>()
                .WithFactory<IFolderFactoryForPath>();
            
        }
    }
}
