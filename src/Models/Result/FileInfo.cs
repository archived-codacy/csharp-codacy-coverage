using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Codacy.CSharpCoverage.Models.Result
{
    /// <summary>
    ///     File info.
    ///     This represents the coverage information of
    ///     a single file.
    /// </summary>
    public class FileInfo
    {
        /// <summary>
        ///     File name
        /// </summary>
        [JsonProperty(PropertyName = "filename")]
        public string Filename { get; set; }

        /// <summary>
        ///     Total of coverage (in percentage)
        /// </summary>
        [JsonProperty(PropertyName = "total")]
        public int Total { get; set; }

        /// <summary>
        ///     Dictionary of file line and coverage hits.
        /// </summary>
        [JsonProperty(PropertyName = "coverage")]
        public Dictionary<int, int> Coverage { get; set; }

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
                var f = (FileInfo) obj;

                return Coverage.SequenceEqual(f.Coverage) && Total == f.Total && Filename == f.Filename;
            }
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = (int) 2166136261;
                hash = (hash * 16777619) ^ Total.GetHashCode();
                hash = (hash * 16777619) ^ (Filename != null ? Filename.GetHashCode() : 0);
                return (hash * 16777619) ^ (Coverage != null ? Coverage.GetHashCode() : 0);
            }
        }
    }
}
