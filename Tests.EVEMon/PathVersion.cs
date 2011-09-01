using System;

namespace Tests.EVEMon
{
    /// <summary>
    /// Relationship class for Paths and Versions.
    /// </summary>
    internal class PathVersion
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PathVersion"/> class.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="version">The version.</param>
        internal PathVersion(String path, Version version)
        {
            Path = path;
            Version = version;
        }

        /// <summary>
        /// Gets or sets the path.
        /// </summary>
        /// <value>The path.</value>
        internal String Path { get; private set; }

        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        /// <value>The version.</value>
        internal Version Version { get; private set; }
    }
}