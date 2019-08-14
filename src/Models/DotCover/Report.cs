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
            }

            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            } else {
                Report r = (Report) obj;
                if (FilesList.Count == r.FilesList.Count)
                {
                    foreach (var pair in FilesList)
                    {
                        string value;
                        if (r.FilesList.TryGetValue(pair.Key, out value))
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

                foreach(var it in ClassCoverages.Zip(r.ClassCoverages, (a,b) => a.Equals(b)))
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
