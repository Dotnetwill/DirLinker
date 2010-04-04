using System;
using DirLinker.Interfaces.Views;
using System.Windows.Forms;
using DirLinker.Interfaces.Controllers;
using DirLinker.Interfaces;
using DirLinker.Data;

namespace DirLinker.Controllers
{
    public class MainController : IMainController
    {
        private readonly ILinkerService _linkerService;
        private readonly IPathValidation _pathValidator;
        private readonly ILinkerView _view;

        protected LinkOperationData _operationData;

        public MainController(ILinkerView view, IPathValidation pathValidator, ILinkerService linkerService)
        {
            _view = view;
            _pathValidator = pathValidator;
            _linkerService = linkerService;
        }

        public Form Start()
        {
            _operationData = new LinkOperationData();
            
            _view.SetOperationData(_operationData);
            _view.ValidatePath += ValidatePath;

            return _view.MainForm;
        }

        public void ValidatePath(object sender, ValidationArgs e)
        {
            String errorMessage;
            if (_pathValidator.ValidPath(e.PathToValidate, out errorMessage))
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
        }

     
    }
}
