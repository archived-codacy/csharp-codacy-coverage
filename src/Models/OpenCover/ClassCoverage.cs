using System;
using System.Collections.Generic;
using System.Linq;

namespace Codacy.CSharpCoverage.Models.OpenCover
{
    /// <summary>
    ///     Class coverage.
    ///     This contains, for each file id, the class coverage
    ///     parsed from the OpenCover format.
    /// </summary>
    public sealed class ClassCoverage
    {
        /// <summary>
        ///     Parsed sequence coverage (in percentage)
        /// </summary>
        public double SequenceCoverage { get; set; }

        /// <summary>
        ///     Parsed file id.
        /// </summary>
        public int FileId { get; set; }

        /// <summary>
        ///     List of covered lines of the referred file id.
        /// </summary>
        public List<LineCoverage> CoveredLines { get; set; }

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
                var c = (ClassCoverage) obj;

                return CoveredLines.SequenceEqual(c.CoveredLines) && FileId == c.FileId &&
                       Math.Abs(SequenceCoverage - c.SequenceCoverage) < 0.001;
            }
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = (int) 2166136261;
                hash = (hash * 16777619) ^ SequenceCoverage.GetHashCode();
                hash = (hash * 16777619) ^ FileId.GetHashCode();
                return (hash * 16777619) ^ (CoveredLines != null ? CoveredLines.GetHashCode() : 0);
            }
        }
    }
}
