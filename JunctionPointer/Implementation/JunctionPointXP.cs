using DirLinker.Interfaces;
using System;

namespace DirLinker.Implementation
{

    public class JunctionPointXp : IJunctionPointXp
    {
        public void Create(string junctionPoint, string target)
        {
            var service = ElevatedWorker.Client.Connect(Program.PipeName);
            using ((IDisposable)service) service.CreateJunctionPoint(junctionPoint, target);
        }
    }
}