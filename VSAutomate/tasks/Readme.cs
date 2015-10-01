namespace VSAutomate
{
    using System.IO;

    using EnvDTE;

    public static class Readme
    {
        private const string ReadmeFileName = "readme.md";

        /// <summary>
        /// Creates a default readme.md document for the solution
        /// </summary>
        /// <param name="solution">the solution object</param>
        /// <param name="docsFolderName">the solution folder to add the readme to</param>
        public static void Configure(Solution solution, string docsFolderName)
        {
            // create a default readme in the solution root
            var readmePath = Path.Combine(solution.GetDirectory(), ReadmeFileName);
            var readmeTemplate = string.Format(RS.readme, solution.GetName());
            if (!File.Exists(readmePath))
            {
                File.WriteAllText(readmePath, readmeTemplate);
            }

            // add as link to docs folder
            var readmeRelatiePath = readmePath.GetRelativePath(docsFolderName);
            var docsProject = solution.GetProject(docsFolderName);
            docsProject?.AddLinkItem("None", readmeRelatiePath, ReadmeFileName);
        }
    }
}