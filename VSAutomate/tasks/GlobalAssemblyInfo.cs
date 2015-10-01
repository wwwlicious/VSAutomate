namespace VSAutomate
{
    using System;
    using System.Globalization;
    using System.IO;

    using EnvDTE;

    public static class GlobalAssemblyInfo
    {
        private const string GlobalFileName = "GlobalAssemblyInfo.cs";

        /// <summary>
        /// Sets up a globalassemblyinfo file in the solution root and links this to all solution projects
        /// </summary>
        /// <param name="solution">The solution object</param>
        /// <param name="companyName">The company name to add in the GlobalAssemblyInfo</param>
        public static void Configure(Solution solution, string companyName)
        {
            // Adds a globalassemblyinfo file to a solution directory and links it to all projects
            var globalFilePath = Path.Combine(solution.GetDirectory(), GlobalFileName);

            if (!File.Exists(globalFilePath))
            {
                var fileText = RS.GlobalAssemblyInfo;
                var formattedFileText = string.Format(fileText, solution.GetName(), "[Enter description]", companyName, DateTime.Today.Year, CultureInfo.CurrentCulture);
                File.WriteAllText(globalFilePath, formattedFileText);
            }

            foreach (var project in solution.GetProjects())
            {
                project.AddLinkItem("compile", project.GetDirectory().GetRelativePath(globalFilePath), "Properties\\" + GlobalFileName);
            }
        }
    }
}