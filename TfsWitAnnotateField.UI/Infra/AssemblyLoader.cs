using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TfsWitAnnotateField.UI.Infra
{

    public class AssemblyLoader
    {
        private string _productInstallationFolder = null;
        /// <summary>
        /// Returns C:\Program Files\MyCompany\MyProduct or C:\Program Files (x86)\MyCompany\MyProduct depending on the platform.
        /// </summary>
        public string ProductInstallationFolder
        {
            get
            {
                if (string.IsNullOrEmpty(_productInstallationFolder))
                {
                    var programFilesFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ProgramFilesX86);
                    _productInstallationFolder = Path.Combine(programFilesFolder, @"Microsoft Visual Studio 14.0\Common7\IDE\CommonExtensions\Microsoft\TeamFoundation\Team Explorer\");
                }
                return _productInstallationFolder;
            }
            set
            {
                _productInstallationFolder = value;
            }
        }
        /// <summary>
        /// Add an evenhandler for adding a folder to the .NET assemlby search path.
        /// </summary>
        public void BindAssemblyResolveEventHandler()
        {
            var currentDomain = AppDomain.CurrentDomain;
            currentDomain.AssemblyResolve += this.LoadAssemlbyFromProductInstallationFolder;
        }
        /// <summary>
        /// This function is called when the .NET runtime searches for an assemlby to load and can't find that assembly in the current search path.
        /// The current search path includes "bin folder application", the global assemlby cache, system32 folder etc.
        ///
        /// This function adds a folder to the current search path at runtime.
        ///
        /// An assembly can be a dll or exe, the ResolveEventArgs argument does not cotain this information.
        /// The code will first check if a dll exist in the given folder, if found it loads the dll.
        /// If the dll is not found, the code checks if an executable exists in the given folder, if found it loads the exe.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public Assembly LoadAssemlbyFromProductInstallationFolder(object sender, ResolveEventArgs args)
        {
            Assembly result = null;
                if (args != null && !string.IsNullOrEmpty(args.Name))
                {
                    var folderPath = (new FileInfo(this.ProductInstallationFolder)).DirectoryName;
                    var assemblyName = args.Name.Split(new string[] { "," }, StringSplitOptions.None)[0];
                    var assemblyExtension = "dll";
                    var assemblyPath = Path.Combine(folderPath, string.Format("{0}.{1}", assemblyName, assemblyExtension));
                    if (!File.Exists(assemblyPath))
                    {
                    return null;
                    }

                    result = Assembly.LoadFrom(assemblyPath);
                }
            return result;
        }
    }
}