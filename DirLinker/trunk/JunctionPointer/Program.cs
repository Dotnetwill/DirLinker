using System;
using System.Windows.Forms;
using DirLinker.Views;
using DirLinker.Controllers;
using DirLinker.Implemenation;
using DirLinker.Interfaces;
using DirLinker.Interfaces.Controllers;
using DirLinker.Interfaces.Views;
using OCInject;

namespace DirLinker

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

            //IMainController mainController = new MainController(classFactory);
            //Application.Run(mainController.Start());
        }

        private static void FillIoCContainer(ClassFactory classFactory)
        {

            //classFactory.RegisterType<IDirLinker, DirLinker.Implemenation.DirLinker>();
           // classFactory.RegisterType<IWorkingController, WorkingDialogController>();
            classFactory.RegisterType<ILinkerView, DirLinkerView>();
         //   classFactory.RegisterType<IWorkingView, ProgessView>();
            classFactory.RegisterType<IBackgroundWorker, BackgroundWorkerImp>();
            classFactory.RegisterType<IFolder, FolderImp>()
                .WithFactory<IFolderFactoryForPath>();

            classFactory.RegisterType<IFile, FileImp>()
                .WithFactory<IFileFactoryForPath>();
            
        }
    }
}
