namespace VSAutomate
{
    using System;

    using EnvDTE;

    public static class SolutionBuildConfig
    {
        /// <summary>
        /// Sets up solution build configurations and ensure they exist across all projects
        /// </summary>
        /// <param name="solution">The solution object</param>
        public static void Configure(Solution solution)
        {
            try
            {
                // ensure solution level build configs for development, staging and release
                var solutionConfigs = solution.SolutionBuild.SolutionConfigurations;

                // change your default configurations here
                foreach (var configName in new[] { "Development", "Staging", "Release" })
                {
                    var copyFrom = configName != "release" ? "debug" : "release";
                    var config = solutionConfigs.ByName(configName) ?? solutionConfigs.Add(configName, copyFrom, true);

                    foreach (var project in solution.GetProjects())
                    {
                        project.ConfigurationManager?.AddOrUpdate(config, copyFrom);
                    }
                }
            }
            catch (Exception e)
            {
                Console.Error.Write("SolutionBuildConfig.Configure() threw an exception {0}", e);
            }
        }
    }
}