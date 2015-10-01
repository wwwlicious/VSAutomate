// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Init.cs" company="wwwlicious">
//   Copyright (c) All Rights Reserved
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace VSAutomate
{
    using System.IO;
    using System.Linq;

    using EnvDTE;

    public static class CustomDictionary
    {
        private const string DictionaryFileName = "customdictionary.xml";

        /// <summary>
        /// Sets up a solution level custom dictionary and links all project to it
        /// </summary>
        /// <param name="solution">The solution object</param>
        /// <param name="buildFolderPath">The folder to store the dictionary file</param>
        public static void Configure(Solution solution, string buildFolderPath)
        {
            // Add file if missing (should not overwrite per solution changes)
            var dictionaryFilePath = Path.Combine(buildFolderPath, DictionaryFileName);
            if (!File.Exists(dictionaryFilePath))
            {
                File.WriteAllText(dictionaryFilePath, RS.CustomDictionary);
            }

            // Make every project link to this dictionary
            foreach (var project in solution.GetProjects().Select(x => x.AsMSBuildProject()).Where(x => x != null))
            {
                // find the first dictionary elements
                var dictionaryRelativePath = project.DirectoryPath.GetRelativePath(dictionaryFilePath);
                var codeAnalysisDictionaryItems = project.GetItems("CodeAnalysisDictionary");
                if (codeAnalysisDictionaryItems != null && codeAnalysisDictionaryItems.Count != 0)
                {
                    // update existing dictionary links, useful if project folder structures change
                    foreach (var item in codeAnalysisDictionaryItems)
                    {
                        item.Xml.Include = dictionaryRelativePath;
                    }
                }
                else
                {
                    var newCompile = project.Xml.AddItem("CodeAnalysisDictionary", dictionaryRelativePath);
                    newCompile.AddMetadata("Link", DictionaryFileName);
                }
            }
        }
    }
}