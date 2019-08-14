using System.Collections.Generic;
using System;
using System.Linq;

namespace Codacy.CSharpCoverage.Models.OpenCover
{
    public sealed class ClassCoverage
    {
        public double SequenceCoverage { get; set; }
		public int FileId { get; set; }
        public List<LineCoverage> CoveredLines { get; set; }
        
        public override bool Equals(object obj)
        {
            if (this == obj)
            {
                return true;
            }

            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            } else {
                ClassCoverage c = (ClassCoverage) obj;

                foreach(var it in CoveredLines.Zip(c.CoveredLines, (a,b) => a.Equals(b)))
                {
                    if(!it)
                    {
                        return false;
                    }
                }

                return FileId == c.FileId &&
                    Math.Abs(SequenceCoverage - c.SequenceCoverage) < 0.001;
            }
		}

        public override int GetHashCode()
        {
            unchecked {
                int hash = (int) 2166136261;
                hash = (hash * 16777619) ^ SequenceCoverage.GetHashCode();
                hash = (hash * 16777619) ^ FileId.GetHashCode();
                return hash = (hash * 16777619) ^ (CoveredLines != null ? CoveredLines.GetHashCode() : 0);
            }
        }
    }
}
