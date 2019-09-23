using System.Collections.Generic;
using System.Linq;

namespace Codacy.CSharpCoverage.Models.DotCover
{
    /// <summary>
    ///     Report model based on DotCover file format.
    /// </summary>
    public sealed class Report : IReport
    {
        /// <summary>
        ///     List of class coverages for each file id.
        /// </summary>
        public List<ClassCoverage> ClassCoverages { get; set; }

        /// <summary>
        ///     Dictionary of file ids and file paths.
        /// </summary>
        public Dictionary<int, string> FilesList { get; set; }

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
                var r = (Report) obj;

                return FilesList.SequenceEqual(r.FilesList) && ClassCoverages.SequenceEqual(r.ClassCoverages);
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
