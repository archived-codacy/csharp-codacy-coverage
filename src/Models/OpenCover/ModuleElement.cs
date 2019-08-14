using System.Collections.Generic;
using System.Linq;

namespace Codacy.CSharpCoverage.Models.OpenCover
{
    public sealed class ModuleElement
    {
        public Dictionary<int, string> FilesList { get; set; }
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
