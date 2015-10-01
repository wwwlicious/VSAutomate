namespace VSAutomate
{
    using System.IO;
    using System.Linq;

    using EnvDTE;

    public static class AnalysisRuleset
    {
        private const string SettingsFileName = "default.ruleset";

        private static readonly string[] ExtraSettings = { "CodeAnalysisRuleSetDirectories", "CodeAnalysisIgnoreBuiltInRuleSets", "CodeAnalysisRuleDirectories", "CodeAnalysisIgnoreBuiltInRules" };

        /// <summary>
        /// Sets up a solution level code analysis ruleset and links all solution projects to it
        /// </summary>
        /// <param name="solution">The solution object</param>
        /// <param name="buildFolderPath">The folder to store the ruleset file</param>
        public static void Configure(Solution solution, string buildFolderPath)
        {
            // Add file if missing (should not overwrite per solution changes)
            var rulesetFile = Path.Combine(buildFolderPath, SettingsFileName);
            if (!File.Exists(rulesetFile))
            {
                File.WriteAllText(rulesetFile, RS.Default_ruleset);
            }
            
            foreach (var project in solution.GetProjects().Select(x => x.AsMSBuildProject()).Where(x => x != null))
            {
                // strip out additional codeanalysis settings that we never need
                var propertyGroups = project.Xml.PropertyGroups.ToList();
                foreach (var prop in propertyGroups.SelectMany(group => group.Properties.Where(x => ExtraSettings.Contains(x.Name))))
                {
                    prop.Value = string.Empty;
                }
                
                // ignore code analysis on generated code for all build configurations
                var buildConfigProps = propertyGroups.Where(x => x.Condition.Contains("$(Configuration)|$(Platform)"));
                foreach (var prop in buildConfigProps)
                {
                    prop.AddOrSetProperty("CodeAnalysisIgnoreGeneratedCode", true);
                }

                // link all projects to solution ruleset
                foreach (var prop in buildConfigProps)
                {
                    prop.AddOrSetProperty("CodeAnalysisRuleSet", project.DirectoryPath.GetRelativePath(rulesetFile));
                }
                
                // enforce code analysis for all projects
                foreach (var prop in buildConfigProps)
                {
                    prop.AddOrSetProperty("RunCodeAnalysis", true);
                }
            }
        }
    }
}