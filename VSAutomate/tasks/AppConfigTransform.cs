namespace VSAutomate
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using EnvDTE;

    using Project = Microsoft.Build.Evaluation.Project;

    public static class AppConfigTransform
    {
        private const string TransformFileName = "Config.transform.targets";

        // TODO refactor this
        public static void Configure(Solution solution, string toolsPath)
        {
            // if projects contains app.config, we should have one nested transform file per build configuration
            // app.config build action should be TransformOnBuild and config transform import target should be
            // added to project
            var solutionConfigNames = solution.SolutionBuild.SolutionConfigurations.Cast<SolutionConfiguration>().Select(x => x.Name).ToArray();
            var transformMetadata = "TransformOnBuild";

            foreach (var project in solution.GetProjects().Select(x => x.AsMSBuildProject()).Where(x => x != null))
            {
                var targetPath = Path.Combine(toolsPath, "net45", "targets", TransformFileName);
                var targetRelativePath = project.DirectoryPath.GetRelativePath(targetPath);

                // any app.configs in project?
                var allItems = project.AllEvaluatedItems.ToList();
                var configs = allItems.Where(x => x.Xml.Include.Equals("app.config", StringComparison.InvariantCultureIgnoreCase) || x.Xml.Include.EndsWith("\\app.config", StringComparison.InvariantCultureIgnoreCase)).ToList();
                if (!configs.Any()) continue;

                foreach (var config in configs)
                {
                    // add transform command to app config if not already there
                    if (config.Xml.Metadata.Any(x => x.Name.Equals(transformMetadata)))
                        config.Xml.AddMetadata(transformMetadata, "True");

                    // create transforms for each config
                    foreach (var solutionConfig in solutionConfigNames)
                    {
                        var configFileName = Path.GetFileName(config.Xml.Include);
                        var configTransformFileName = configFileName.Replace(".", $".{solutionConfig}.");
                        var transform = allItems.Single(x => x.Xml.Include.Equals(configTransformFileName, StringComparison.InvariantCultureIgnoreCase));
                        if (transform == null)
                        {
                            // Add transform, creates a new file if one doesn't exist
                            var transformFilePath = Path.Combine(project.DirectoryPath, configTransformFileName);
                            if (!File.Exists(TransformFileName)) File.WriteAllText(transformFilePath, @"<?xml version=""1.0"" encoding=""utf - 8""?>");
                            project.AddItem("None", configTransformFileName, new[] { new KeyValuePair<string, string>("DependentUpon", "configFileName"), });
                        }
                    }
                }

                // sort out msbuild import which transforms the files on build
                ConfigProjectImport(project, targetRelativePath, TransformFileName);
            }
        }

        private static void ConfigProjectImport(Project project, string targetRelativePath, string configTransformTargetFile)
        {
            // clear out any existing value
            var importPathMSBuildProperty = "ConfigTransformTargetPath";
            var msbuildProp = project.Xml.Properties.Where(x => x.Name == importPathMSBuildProperty).ToArray();
            msbuildProp.Delete();

            // creates a property to store the target file path
            var importPath = $"$({importPathMSBuildProperty})\\{configTransformTargetFile}";
            project.Xml.AddProperty(importPathMSBuildProperty, targetRelativePath);

            // create import element with exists condition and add as last project import
            var importXmlElement = project.Xml.CreateImportElement(importPath);
            importXmlElement.Condition = $"Exists('{importPath}')";
            project.Xml.InsertAfterChild(importXmlElement, project.Xml.Imports.Last());
        }
    }
}