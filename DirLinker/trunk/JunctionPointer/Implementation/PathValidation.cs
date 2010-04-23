using System;
using System.Linq;
using DirLinker.Interfaces;
using System.Text.RegularExpressions;

namespace DirLinker.Implementation
{
    public class PathValidation : IPathValidation
    {
        private readonly IFolderFactoryForPath _folderFactory;
        private readonly String regexForDrive = @"^([a-zA-Z]\:)";

        public PathValidation(IFolderFactoryForPath folderFactory)
        {
            _folderFactory = folderFactory;
        }


        public Boolean ValidPath(String path, out String errorMessage)
        {
            errorMessage = String.Empty;
            IFolder folder = _folderFactory(String.Empty);

            if (!String.IsNullOrEmpty(path))
            {
                if (IsDriveLetter(path))
                {
                    errorMessage = "Only folder paths allowed";
                    return false;
                }

                if (path.Length > folder.MaxPath())
                {
                    errorMessage = "Selected folder path is longer than the maximum allowable Windows path";
                    return false;
                }

                String[] pathParts = path.Split('\\');
                if (pathParts.Length == 0 || !Regex.IsMatch(pathParts[0], regexForDrive))
                {
                    errorMessage = "The folder path is not well formed";
                    return false;
                }
                else if (pathParts.Length > 1)
                {
                    String pathWithoutDrive = path.Replace(pathParts[0], String.Empty);

                    Int32 count = folder.GetIllegalPathChars().Count(c => pathWithoutDrive.Contains(c));

                    if (count > 0)
                    {
                        errorMessage = "Folder path contains illegal characters";
                        return false;
                    }
                }

            }
            else
            {
                errorMessage = "Please enter a path";
                return false;
            }

            return true;
        }
        
         private bool IsDriveLetter(String path)
        {
            if (path.Length < 4 && Regex.IsMatch(path, regexForDrive))
            {
                return true;
            }

            return false;
        }
        
    }
}
