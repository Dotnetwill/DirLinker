using System;
using JunctionPointer.Interfaces;

namespace JunctionPointer.Exceptions
{
    public class DirLinkerException : Exception
    {
        public DirLinkerStage Stage { get; set; }

        public DirLinkerException(Exception ex, DirLinkerStage dirLinkerStage) : base("Unknown Exception Caught", ex)
        {
            Stage = dirLinkerStage;
        }

        public DirLinkerException(string errorMessage, DirLinkerStage dirLinkerStage) : base(errorMessage)
        {
            Stage = dirLinkerStage;
        }

        public static object ManufactureExpcetion(Exception ex, DirLinkerStage dirLinkerStage)
        {
            return new DirLinkerException(ex, dirLinkerStage);
        }
    }
}
