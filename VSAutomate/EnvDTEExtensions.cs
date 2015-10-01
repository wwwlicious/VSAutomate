// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnvDTEExtensions.cs" company="wwwlicious">
//   Copyright (c) All Rights Reserved
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace VSAutomate
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;

    using EnvDTE;
    using EnvDTE80;

    using Microsoft.Build.Construction;
    using Microsoft.Build.Evaluation;

    using Project = EnvDTE.Project;
    using ProjectItem = EnvDTE.ProjectItem;

    /// <summary>
    /// A set of useful extensions for working with EnvDTE Solutions, Projects and ProjectItems
    /// </summary>
    public static class EnvDTEExtensions
    {
        public static string GetDirectory(this Solution solution)
        {
            return solution.FileName.GetDirectory();
        }

        public static string GetDirectory(this Project project)
        {
            return project.FileName.GetDirectory();
        }

        public static string GetDirectory(this ProjectItem item)
        {
            return Path.GetDirectoryName(item.GetFilePath());
        }

        public static string GetDirectory(this string filePath)
        {
            var directory = Path.GetDirectoryName(filePath);
            Debug.Assert(directory != null, "directory != null");
            return directory;
        }

        public static string GetName(this Solution solution)
        {
            return Path.GetFileNameWithoutExtension(solution.FullName);
        }

        public static string GetFilePath(this ProjectItem item)
        {
            return item.Properties.Item("FullPath").Value as string;
        }

        public static void AddSolutionFolder(this Solution solution, string folderName)
        {
            var solution2 = solution as Solution2;
            if (solution2 == null)
            {
                throw new InvalidCastException("Could not add solution folder, error casting EnvDTE.Solution to EnvDTE.Solution2 interface");
            }
            Debug.WriteLine("Adding solution folder {0}", folderName);
            solution2.AddSolutionFolder(folderName);
        }

        public static void AddLinkItem(this Project project, string buildAction, string include, string fileName)
        {
            // convert the DTE to an MSBuild Project type
            var msbuildProject = project.AsMSBuildProject();
            if (msbuildProject == null) return; 

            // Check if item already exists before trying to add it
            var items = msbuildProject.GetItems(buildAction);
            var itemExists = items.Any(x => x.EvaluatedInclude.Equals(include, StringComparison.InvariantCultureIgnoreCase));

            if (!itemExists)
            {
                Debug.WriteLine("Adding link item {0} to {1}", fileName, include);
                msbuildProject.Xml.AddItem(
                    buildAction,
                    include,
                    new[] { new KeyValuePair<string, string>("Link", fileName) });
            }
        }

        public static void RemoveLinkItem(Solution sol, string buildAction, string include, string fileName)
        {
            foreach (var project in GetProjects(sol).Select(x => x.AsMSBuildProject()).Where(x => x != null))
            {
                // Check if item already exists before trying to add it
                var items = project.AllEvaluatedItems.ToList();
                var itemExists = items.Find(x => x.EvaluatedInclude.Equals(include));
                if (itemExists != null)
                {
                    // Remove the item (should only be one)
                    Debug.WriteLine("Removing link item {0}", itemExists.Xml.Include);
                    project.RemoveItem(itemExists);
                }
            }
        }

        public static IList<Project> GetProjects(this Solution sol)
        {
            var projects = sol.Projects;
            var list = new List<Project>();
            var item = projects.GetEnumerator();
            while (item.MoveNext())
            {
                var project = item.Current as Project;
                if (project == null)
                {
                    continue;
                }

                if (project.Kind == ProjectKinds.vsProjectKindSolutionFolder)
                {
                    list.AddRange(project.GetSolutionFolderProjects());
                }
                else
                {
                    list.Add(project);
                }
            }
            return list;
        }

        public static Project GetProject(this Solution solution, string projectName)
        {
            return GetProjects(solution).SingleOrDefault(x => x.Name.Equals(projectName));
        }

        public static SolutionConfiguration ByName(this SolutionConfigurations configs, string name)
        {
            return configs.Cast<SolutionConfiguration>().ToList().FirstOrDefault(config => config.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
        }

        public static void AddOrUpdate(this ConfigurationManager manager, SolutionConfiguration config, string copyFrom)
        {
            try
            {
                var names = ((Array)manager.ConfigurationRowNames)?.Cast<string>().ToList();
                if (!names.Any(x => x.Equals(config.Name, StringComparison.InvariantCultureIgnoreCase)))
                {
                    Debug.WriteLine("Adding project configuration {0} copied from {1}", config.Name, copyFrom);
                    manager.AddConfigurationRow(config.Name, copyFrom, true);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error adding or updating solution config {config.Name}");
            }
        }

        private static IEnumerable<Project> GetSolutionFolderProjects(this Project solutionFolder)
        {
            var list = new List<Project>();
            for (var i = 1; i <= solutionFolder.ProjectItems.Count; i++)
            {
                var subProject = solutionFolder.ProjectItems.Item(i).SubProject;
                if (subProject == null)
                {
                    continue;
                }

                // If this is another solution folder, do a recursive call, otherwise add
                if (subProject.Kind == ProjectKinds.vsProjectKindSolutionFolder)
                {
                    list.AddRange(GetSolutionFolderProjects(subProject));
                }
                else
                {
                    list.Add(subProject);
                }
            }

            return list;
        }

        public static void Delete(this IEnumerable<ProjectElement> elements)
        {
            foreach (var importElement in elements)
            {
                Debug.WriteLine("Deleting import {0}", importElement);
                importElement.Parent.RemoveChild(importElement);
            }
        }

        public static ProjectItem[] GetParentItems(this ProjectItem config)
        {
            var parent = config.Collection.Parent;

            var parentItem = parent as ProjectItem;
            ProjectItems siblings = null;
            if (parentItem != null)
            {
                siblings = parentItem.ProjectItems;
            }

            var parentProject = parent as Project;
            if (parentProject != null)
            {
                siblings = parentProject.ProjectItems;
            }

            return siblings.Cast<ProjectItem>().ToArray();
        }

        public static Microsoft.Build.Evaluation.Project AsMSBuildProject(this Project project)
        {
            try
            {
                if (string.IsNullOrEmpty(project.FullName)) return null;

                return
                ProjectCollection.GlobalProjectCollection.GetLoadedProjects(project.FullName).FirstOrDefault()
                ?? ProjectCollection.GlobalProjectCollection.LoadProject(project.FullName);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static void AddOrSetProperty<T>(this ProjectPropertyGroupElement props, string name, T value)
        {
            if (props.Properties.All(x => x.Name != name))
            {
                Debug.WriteLine("Adding property {0}: {1}", name, value);
                props.AddProperty(name, value.ToString());
            }
            else
            {
                Debug.WriteLine("Setting property {0}: {1}", name, value);
                props.SetProperty(name, value.ToString());
            }
        }

        public static void SaveAll(this Solution solution)
        {
            try
            {
                var projects = solution.GetProjects();
                foreach (var project in projects)
                {
                    if (!project.Saved)
                    {
                        Debug.WriteLine("Saving project {0}", project.Name);
                        project.Save();
                    }

                    project.ProjectItems.SaveAll();
                }
            }
            catch (Exception)
            {
                // Do nothing as this is a type of project we dont care for.               
            }
        }

        public static void SaveAll(this ProjectItems items)
        {
            foreach (var item in items.Cast<ProjectItem>())
            {
                if (!item.Saved)
                {
                    Debug.WriteLine("Saving item {0}", item.Name);
                    item.Save();
                }

                if (item.ProjectItems.Count > 0)
                {
                    item.ProjectItems.SaveAll();
                }
            }
        }
    }
}