using System;
using JunctionPointer.Interfaces.Controllers;
using JunctionPointer.Interfaces.Views;
using JunctionPointer.Interfaces;
using System.ComponentModel;
using OCInject;

namespace JunctionPointer.Controllers
{
    public class WorkingDialogController : IWorkingController
    {
        protected IWorkingView m_View;
        protected IDirLinker m_Linker;
        protected IBackgroundWorker m_BackgroundWorker;

        public WorkingDialogController(IDirLinker dirLinker, IWorkingView workingView, IBackgroundWorker bgWorker)
        {
            m_Linker = dirLinker;
            m_View = workingView;
            m_BackgroundWorker = bgWorker;
        }

        public void DoDirectoryLinkWithFeedBack(String linkPoint, String linkTo, Boolean copyContentsToTarget, Boolean overwriteTargetFiles)
        {
            try
            {
                m_Linker.ReportFeedback += OnFeebackReported;
                m_Linker.UserMessage += OnUserResponseRequired;

                m_BackgroundWorker.DoWork += (sender, e) => m_Linker.CreateSymbolicLinkFolder(linkPoint, linkTo, copyContentsToTarget, overwriteTargetFiles);
                m_BackgroundWorker.RunWorkerCompleted += OnThreadComplete;
                m_BackgroundWorker.RunWorkerAsync();
                System.Windows.Forms.Form mainForm = null;
                if (System.Windows.Forms.Application.OpenForms.Count > 0)
                {
                    mainForm = System.Windows.Forms.Application.OpenForms[0];
                }
                m_View.ShowDialog(mainForm);
               
            }
            finally
            {
                m_Linker.ReportFeedback -= OnFeebackReported;
                m_Linker.UserMessage -= OnUserResponseRequired;
            }
        }

        void OnThreadComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            m_View.Close();
        }


        void OnUserResponseRequired(object sender, UserMessageArgs e)
        {
            e.Response = m_View.AskUser(e.Message, e.ResponseOptions);
        }

        public void OnFeebackReported(object sender, FeedbackArgs e)
        {
            m_View.FeedbackTitle = e.ProgressTitle;
            m_View.FeedbackCaption = e.Progress;
        }
    }
}
