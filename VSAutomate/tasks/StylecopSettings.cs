namespace VSAutomate
{
    using System.Globalization;
    using System.IO;
    using System.Linq;

    using EnvDTE; 

    public static class StylecopSettings
    {
        private const string SettingsFileName = "settings.stylecop";

        /// <summary>
        /// Sets up a default solution level stylecop.settings file, 
        /// then creates project level settings file for every proect that
        /// link to the solution level settings
        /// </summary>
        /// <param name="solution">the solution folder</param>
        /// <param name="buildFolderPath">the folder to store the solution stylecop.settings</param>
        /// <param name="companyName"></param>
        public static void Configure(Solution solution, string buildFolderPath, string companyName)
        {
            // Solution settings are copied if new but not overwritten
            var solutionSettingsPath = Path.Combine(buildFolderPath, SettingsFileName);
            if (!File.Exists(solutionSettingsPath))
            {
                File.WriteAllText(solutionSettingsPath, string.Format(RS.settings_stylecop_solution, CultureInfo.CurrentCulture, companyName));
            }

            // project settings are always linked to solution settings so should be copied if edited
            foreach (var project in solution.GetProjects())
            {
                // work out the project settings.stylecop filePath relative to the solution settings file
                var projDirectory = project.FullName.GetDirectory();
                var projectSettingsPath = Path.Combine(projDirectory, SettingsFileName);
                var relativePathToSolutionSettings = projDirectory.GetRelativePath(solutionSettingsPath);

                // check the hash, if it matches don't replace, otherwise, create or overwrite
                var stylecopProjectFileText = string.Format(RS.settings_stylecop_project, relativePathToSolutionSettings);
                var expectedFileHash = FileExtensions.GetFileHash(stylecopProjectFileText);
                if (!File.Exists(projectSettingsPath) || !FileExtensions.GetFileHash(projectSettingsPath).SequenceEqual(expectedFileHash))
                {
                    File.WriteAllText(projectSettingsPath, stylecopProjectFileText);
                }
            }
        }
    }
}