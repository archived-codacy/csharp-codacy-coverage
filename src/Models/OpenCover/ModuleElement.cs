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
            }

            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            } else {
                ModuleElement m = (ModuleElement) obj;

                if (FilesList.Count == m.FilesList.Count)
                {
                    foreach (var pair in FilesList)
                    {
                        string value;
                        if (m.FilesList.TryGetValue(pair.Key, out value))
                        {
                            if (value != pair.Value)
                            {
                                return false;
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }
                } else {
                    return false;
                }

                foreach(var it in ClassCoverages.Zip(m.ClassCoverages, (a,b) => a.Equals(b)))
                {
                    if(!it)
                    {
                        return false;
                    }
                }

                return true;
            }
		}

        public override int GetHashCode()
        {
            unchecked {
                int hash = (int) 2166136261;
                hash = (hash * 16777619) ^ (FilesList != null ? FilesList.GetHashCode() : 0);
                return hash = (hash * 16777619) ^ (ClassCoverages != null ? ClassCoverages.GetHashCode() : 0);
            }
        }
	}
}
