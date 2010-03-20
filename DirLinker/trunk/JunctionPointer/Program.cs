using System;
using System.Windows.Forms;
using JunctionPointer.Views;
using JunctionPointer.Controllers;
using JunctionPointer.Implemenation;
using JunctionPointer.Interfaces;
using JunctionPointer.Interfaces.Controllers;
using JunctionPointer.Interfaces.Views;
using JunctionPointer.Helpers.Interfaces;
using OCInject;

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

            ClassFactory classFactory = new ClassFactory();
            FillIoCContainer(classFactory);

            IMainController mainController = new MainController(classFactory);
            Application.Run(mainController.Start());
        }

        private static void FillIoCContainer(ClassFactory classFactory)
        {

            classFactory.RegisterType<IDirLinker, JunctionPointer.Implemenation.DirLinker>();
            classFactory.RegisterType<IWorkingController, WorkingDialogController>();
            classFactory.RegisterType<ILinkerView, DirLinkerView>();
            classFactory.RegisterType<IWorkingView, ProgessView>();
            classFactory.RegisterType<IBackgroundWorker, BackgroundWorkerImp>();
            classFactory.RegisterType<IFolder, FolderImp>()
                .WithFactory<IFolderFactoryForPath>();

            classFactory.RegisterType<IFile, FileImp>()
                .WithFactory<IFileFactoryForPath>();
            
        }
    }
}
