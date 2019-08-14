using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System;
using System.Text;

namespace Codacy.CSharpCoverage.Models.Result
{
    public class CodacyReport
    {
        public CodacyReport(IEnumerable<FileInfo> fileReports)
        {
            var totalTuple = fileReports
                .Select(f => (Covered: f.Coverage.Where(l => l.Value > 0).Count(), Total: f.Coverage.Count))
                .Aggregate((Covered:0, Total: 0), (t, n) => t = (Covered: t.Covered + n.Covered, Total: t.Total + n.Total));

            Total = (int) (((double)totalTuple.Covered / totalTuple.Total) * 100);
            FileReports = fileReports;
        }

        [JsonProperty(PropertyName = "total")]
        public int Total { get; set; }

        [JsonProperty(PropertyName = "fileReports")]
        public IEnumerable<FileInfo> FileReports { get; set; }
        
        override public string ToString()
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
            StringBuilder stringBuilder = new StringBuilder($"Coverage report:{Environment.NewLine}{Environment.NewLine}");

            foreach(var file in FileReports)
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
            }

            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            } else {
                CodacyReport cr = (CodacyReport) obj;

                foreach(var it in FileReports.Zip(cr.FileReports, (a,b) => a.Equals(b)))
                {
                    if(!it)
                    {
                        return false;
                    }
                }
                return Total == cr.Total;
            }
		}

        public override int GetHashCode()
        {
            unchecked {
                int hash = (int) 2166136261;
                hash = (hash * 16777619) ^ Total.GetHashCode();
                return hash = (hash * 16777619) ^ (FileReports != null ? FileReports.GetHashCode() : 0);
            }
        }
    }

}
