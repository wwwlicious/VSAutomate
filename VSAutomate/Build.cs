namespace VSAutomate
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    using EnvDTE;

    public static class Nuget
    {
        public static void Init(object envDTE, string toolsPath)
        {
#if DEBUG
            System.Diagnostics.Debugger.Launch();
#endif
            var envDte = (EnvDTE80.DTE2)envDTE;
            var solution = envDte.Solution;
            if (solution == null)
            {
                throw new ArgumentNullException(nameof(solution), "this package should be installed within visual studio as it relies on the the Visual Studio automation object model EnvDTE");
            }

            if (toolsPath == null)
            {
                throw new ArgumentNullException(nameof(toolsPath));
            }

            try
            {
                // build folder and solution configurations should be created first as the others will rely on these
                var solutionDir = solution.GetDirectory();
                var buildFolderPath = Path.Combine(solutionDir, ".build");
                var docsFolderPath = Path.Combine(solutionDir, ".docs");
                var companyName = "ACME Corp";


                SolutionFolders.Configure(solution, true, buildFolderPath, docsFolderPath);
                SolutionBuildConfig.Configure(solution);
#if DEBUG
                Readme.Configure(solution, docsFolderPath);
                GlobalAssemblyInfo.Configure(solution, companyName);
                StylecopSettings.Configure(solution, buildFolderPath, companyName);
                AnalysisRuleset.Configure(solution, buildFolderPath);
                CustomDictionary.Configure(solution, buildFolderPath);
                AppConfigTransform.Configure(solution, toolsPath);
#else
                // ordering of these don't matter, run as async tasks
                var configTask = Task.Run(() => AppConfigTransform.Configure(solution, toolsPath));
                var readMeTask = Task.Run(() => Readme.Configure(solution));
                var assemblyinfoTask = Task.Run(() => GlobalAssemblyInfo.Configure(solution));
                var stylecopTask = Task.Run(() => StylecopSettings.Configure(solution, SolutionFolders.BuildFolderPath));
                var fxcopTask = Task.Run(() => AnalysisRuleset.Configure(solution, SolutionFolders.BuildFolderPath));
                var dictionaryTask = Task.Run(() => CustomDictionary.Configure(solution, SolutionFolders.BuildFolderPath));

                // wait!
                Task.WaitAll(configTask, readMeTask, assemblyinfoTask, stylecopTask, fxcopTask, dictionaryTask);
#endif
                // Persist all changes
                solution.SaveAll();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.ToString());
            }
        }
    }
}