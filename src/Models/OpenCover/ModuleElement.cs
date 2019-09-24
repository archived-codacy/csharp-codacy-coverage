using System.Collections.Generic;
using System.Linq;

namespace Codacy.CSharpCoverage.Models.OpenCover
{
    /// <summary>
    ///     Module element.
    ///     This contains Module elements parsed from OpenCover format.
    ///     This has a dictionary of file ids and file paths. It also has
    ///     a list of class coverages for each file id.
    /// </summary>
    public sealed class ModuleElement
    {
        /// <summary>
        ///     Dictionary of file ids and file paths.
        /// </summary>
        public Dictionary<int, string> FilesList { get; set; }

        /// <summary>
        ///     List of class coverages for each file id.
        /// </summary>
        public List<ClassCoverage> ClassCoverages { get; set; }

        public override bool Equals(object obj)
        {
            if (this == obj)
            {
                return true;
            } else if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            else
            {
                var m = (ModuleElement) obj;

                return FilesList.SequenceEqual(m.FilesList) && ClassCoverages.SequenceEqual(m.ClassCoverages);
            }
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = (int) 2166136261;
                hash = (hash * 16777619) ^ (FilesList != null ? FilesList.GetHashCode() : 0);
                return (hash * 16777619) ^ (ClassCoverages != null ? ClassCoverages.GetHashCode() : 0);
            }
        }
    }
}
