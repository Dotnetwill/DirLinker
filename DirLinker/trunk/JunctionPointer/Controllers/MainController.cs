using System;
using DirLinker.Interfaces.Views;
using DirLinker.Interfaces;
using System.Windows.Forms;
using DirLinker.Interfaces.Controllers;
using DirLinker.Helpers.Interfaces;
using OCInject;

namespace DirLinker.Controllers
{
    public class MainController : IMainController
    {
        protected ILinkerView m_View;
        protected IDirLinker m_Linker;
        protected ClassFactory m_ClassFactory;

 
        public MainController(ClassFactory classFactory)
        {
            m_ClassFactory = classFactory;
            m_View = m_ClassFactory.ManufactureType<ILinkerView>();
            m_Linker = m_ClassFactory.ManufactureType<IDirLinker>();
        }

        public Form Start()
        {
           
            m_View.PerformOperation += PerformOperation;
            m_View.ValidatePath += ValidatePath;
            m_View.CopyBeforeDelete = true;
            m_View.Setup();

            return m_View.MainForm;
        }

        public void ValidatePath(object sender, ValidationArgs e)
        {
            String errorMessage;
            if (m_Linker.ValidDirectoryPath(e.PathToValidate, out errorMessage))
            {
                e.Valid = true;
            }
            else
            {
                e.Valid = false;
                e.ErrorMessge = errorMessage;
            }
        }

        public void PerformOperation(object sender, EventArgs e)
        {
            IWorkingController workingController = m_ClassFactory.ManufactureType<IWorkingController>();
            workingController.DoDirectoryLinkWithFeedBack(m_View.LinkPoint, m_View.LinkTo, m_View.CopyBeforeDelete, m_View.OverWriteTargetFiles);
        }

     
    }
}
