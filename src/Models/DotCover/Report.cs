using System.Collections.Generic;
using System.Linq;

namespace Codacy.CSharpCoverage.Models.DotCover
{
    public sealed class Report : IReport
    {
        public List<ClassCoverage> ClassCoverages { get; set; }
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
