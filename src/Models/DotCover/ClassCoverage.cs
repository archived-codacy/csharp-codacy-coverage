using System;
using System.Collections.Generic;
using System.Linq;

namespace Codacy.CSharpCoverage.Models.DotCover
{
    public sealed class ClassCoverage
    {
        public double CoveragePercent { get; set; }
        public int FileId { get; set; }
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
                       Math.Abs(CoveragePercent - c.CoveragePercent) < 0.001;
            }
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = (int) 2166136261;
                hash = (hash * 16777619) ^ CoveragePercent.GetHashCode();
                hash = (hash * 16777619) ^ FileId.GetHashCode();
                return (hash * 16777619) ^ (CoveredLines != null ? CoveredLines.GetHashCode() : 0);
            }
        }
    }
}
