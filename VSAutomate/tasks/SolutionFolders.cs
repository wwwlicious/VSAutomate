namespace VSAutomate
{
    using System.IO;
    using System.Linq;

    using EnvDTE;

    public static class SolutionFolders
    {
        /// <summary>
        /// Sets up your conventional folders in a solution
        /// </summary>
        /// <param name="solution">The solution</param>
        /// <param name="addSolutionFolderInVS">whether you want to include these folders in the VS IDE</param>
        /// <param name="folderPaths">The folders to create</param>
        public static void Configure(Solution solution, bool addSolutionFolderInVS, params string[] folderPaths)
        {
            foreach (var folderPath in folderPaths.Where(folderPath => !Directory.Exists(folderPath)).AsParallel())
            {
                Directory.CreateDirectory(folderPath);
                if (addSolutionFolderInVS)
                {
                    solution.AddSolutionFolder(Path.GetFileName(folderPath));
                }
            }
        }
    }
}