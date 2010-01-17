using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JunctionPointer.Interfaces.Views;

namespace DirLinker.Tests.Helpers
{
    public class StubMainView : ILinkerView
    {
        #region ILinkerView Members

        public string LinkPoint { get; set; }

        public string LinkTo { get; set; }

        public bool CopyBeforeDelete { get; set; }

        public bool OverWriteTargetFiles { get; set; }

        public string TempPath { get; set; }

        public System.Windows.Forms.Form MainForm { get; private set; }

        public void Setup()
        {
            throw new NotImplementedException();
        }

        public event PathValidater ValidatePath;

        public event PerformLink PerformOperation;

        #endregion
    }
}
