// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetCurrentIDE.cs" company="wwwlicious">
//   Copyright (c) All Rights Reserved
// </copyright>
// <summary>
//   Defines the GetCurrentIde type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace VSAutomate
{
    using System;
    using System.Runtime.InteropServices;
    using System.Runtime.InteropServices.ComTypes;

    using EnvDTE80;

    using Process = System.Diagnostics.Process;

    /// <summary>
    /// Utility method to get the envDTE from a running instance of VS
    /// </summary>
    public static class Ide
    {
        public static DTE2 GetCurrent()
        {
            // root entry for visual studio running under current process.
            var rootEntry = $"!VisualStudio.DTE.12.0:{Process.GetCurrentProcess().Id}";
            IRunningObjectTable rot;
            NativeMethods.GetRunningObjectTable(0, out rot);
            IEnumMoniker enumMoniker;
            rot.EnumRunning(out enumMoniker);
            enumMoniker.Reset();
            var fetched = IntPtr.Zero;
            var moniker = new IMoniker[1];
            while (enumMoniker.Next(1, moniker, fetched) == 0)
            {
                IBindCtx bindCtx;
                NativeMethods.CreateBindCtx(0, out bindCtx);
                string displayName;
                moniker[0].GetDisplayName(bindCtx, null, out displayName);
                if (displayName == rootEntry)
                {
                    object comObject;
                    rot.GetObject(moniker[0], out comObject);
                    return (DTE2)comObject;
                }
            }
            return null;
        }

        private static class NativeMethods
        {
            [DllImport("ole32.dll")]
            public static extern int CreateBindCtx(int reserved, out IBindCtx ppbc);
            [DllImport("ole32.dll")]
            public static extern int GetRunningObjectTable(int reserved, out IRunningObjectTable prot);
        }
    }
}