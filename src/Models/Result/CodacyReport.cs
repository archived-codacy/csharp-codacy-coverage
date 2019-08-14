using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Codacy.CSharpCoverage.Models.Result
{
    public class CodacyReport
    {
        public CodacyReport(IEnumerable<FileInfo> fileReports)
        {
            var totalTuple = fileReports
                .Select(f => (Covered: f.Coverage.Count(l => l.Value > 0), Total: f.Coverage.Count))
                .Aggregate((Covered: 0, Total: 0),
                    (t, n) => t = (Covered: t.Covered + n.Covered, Total: t.Total + n.Total));

            Total = Convert.ToInt32(Math.Round(((double) totalTuple.Covered / totalTuple.Total * 100)));
            FileReports = fileReports;
        }

        [JsonProperty(PropertyName = "total")] public int Total { get; set; }

        [JsonProperty(PropertyName = "fileReports")]
        public IEnumerable<FileInfo> FileReports { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this,
                Formatting.None,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });
        }

        public string GetStats()
        {
            var stringBuilder = new StringBuilder($"Coverage report:{Environment.NewLine}{Environment.NewLine}");

            foreach (var file in FileReports)
                stringBuilder.Append($" {file.Total} %\t{file.Filename}{Environment.NewLine}");

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
