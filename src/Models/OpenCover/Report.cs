using System.Collections.Generic;
using System.Linq;

namespace Codacy.CSharpCoverage.Models.OpenCover
{
    public sealed class Report : IReport
    {
        public List<ModuleElement> ModuleElements { get; set; }

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
				Report r = (Report)obj;

				foreach(var it in ModuleElements.Zip(r.ModuleElements, (a,b) => a.Equals(b)))
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
            return hash = (hash * 16777619) ^ (ModuleElements != null ? ModuleElements.GetHashCode() : 0);
            }
        }
	}
}
