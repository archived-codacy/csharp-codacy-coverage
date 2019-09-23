using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Codacy.CSharpCoverage.Models.Result
{
    /// <summary>
    ///     Codacy report model.
    ///     This represents the codacy report format.
    /// </summary>
    public class CodacyReport
    {
        /// <summary>
        ///     File reports constructor.
        ///     This construct a codacy report using a list of
        ///     file reports.
        /// </summary>
        /// <param name="fileReports">list of file reports</param>
        public CodacyReport(IEnumerable<FileInfo> fileReports)
        {
            var totalTuple = fileReports
                .Select(f => (Covered: f.Coverage.Count(l => l.Value > 0), Total: f.Coverage.Count))
                .Aggregate((Covered: 0, Total: 0),
                    (t, n) => t = (Covered: t.Covered + n.Covered, Total: t.Total + n.Total));

            Total = Convert.ToInt32(Math.Round((double) totalTuple.Covered / totalTuple.Total * 100));
            FileReports = fileReports;
        }

        /// <summary>
        ///     Total of coverage (in percentage)
        /// </summary>
        [JsonProperty(PropertyName = "total")]
        public int Total { get; set; }

        /// <summary>
        ///     List of file reports
        /// </summary>
        [JsonProperty(PropertyName = "fileReports")]
        public IEnumerable<FileInfo> FileReports { get; set; }

        /// <summary>
        ///     Convert to a string-based json format.
        /// </summary>
        /// <returns>string-based json format</returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this,
                Formatting.None,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });
        }

        /// <summary>
        ///     Get a pretty summary of this coverage report file.
        /// </summary>
        /// <returns>stats summary</returns>
        public string GetStats()
        {
            var stringBuilder = new StringBuilder($"Coverage report:{Environment.NewLine}{Environment.NewLine}");

            foreach (var file in FileReports)
            {
                stringBuilder.Append($" {file.Total} %\t{file.Filename}{Environment.NewLine}");
            }

            stringBuilder.Append($"{Environment.NewLine}Total Coverage: {Total}%");

            return stringBuilder.ToString();
        }

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
                var cr = (CodacyReport) obj;

                return FileReports.SequenceEqual(cr.FileReports) && Total == cr.Total;
            }
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = (int) 2166136261;
                hash = (hash * 16777619) ^ Total.GetHashCode();
                return (hash * 16777619) ^ (FileReports != null ? FileReports.GetHashCode() : 0);
            }
        }
    }
}
