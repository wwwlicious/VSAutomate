namespace VSAutomate
{
    using System.IO;

    /// <summary>
    /// The utility.
    /// </summary>
    public static class Utility
    {
        /// <summary>
        /// The get file path resolution.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="sourceRootPath">
        /// The source root path.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetFilePathResolution(string source, string sourceRootPath)
        {
            if (Path.IsPathRooted(source) || string.IsNullOrEmpty(sourceRootPath))
                return source;
            return Path.Combine(sourceRootPath, source);
        }
    }
}