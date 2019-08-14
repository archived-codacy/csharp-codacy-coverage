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
            } else if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            else
            {
                var r = (Report) obj;

                return ModuleElements.SequenceEqual(r.ModuleElements);
            }
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = (int) 2166136261;
                return (hash * 16777619) ^ (ModuleElements != null ? ModuleElements.GetHashCode() : 0);
            }
        }
    }
}
