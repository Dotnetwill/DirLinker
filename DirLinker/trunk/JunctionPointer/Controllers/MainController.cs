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
        private Func<IProgressController> _progressFactory;
        protected ILinkerView _View;
        
        public MainController(ILinkerView view, Func<IProgressController> progressFactory)
        {
            _View = view;
            _progressFactory = progressFactory;
        }

        public Form Start()
        {
           
            _View.PerformOperation += PerformOperation;
            _View.ValidatePath += ValidatePath;
            _View.CopyBeforeDelete = true;

            _View.Setup();

            return _View.MainForm;
        }

        public void ValidatePath(object sender, ValidationArgs e)
        {
            //String errorMessage;
            //if (m_Linker.ValidDirectoryPath(e.PathToValidate, out errorMessage))
            //{
            //    e.Valid = true;
            //}
            //else
            //{
            //    e.Valid = false;
            //    e.ErrorMessge = errorMessage;
            //}
        }

        public void PerformOperation(object sender, EventArgs e)
        {
            var progressController = _progressFactory();
            progressController.PerformLink(_View.LinkPoint, _View.LinkTo, _View.CopyBeforeDelete, _View.OverWriteTargetFiles);
        }

     
    }
}
