using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Linq;

namespace Codacy.CSharpCoverage.Parsing.Processors
{
	using Models.DotCover;
	using Models.Result;

	public class DotCoverProcessor : IProcessor<Report>
    {
        public Report Parse(string file)
        {
            var reportXml = XDocument.Load(file);
            var modules = reportXml.Root;
            var namespaces = modules.Elements("Assembly").SelectMany(x => x.Elements("Namespace"));
            //modules.Elements("SolutionFolder").Elements("Project").SelectMany(x => x.Elements("Namespace"));
            var moduleIndex = 0;

            var report = new Report
            {
                ClassCoverages = new List<ClassCoverage>(),
                FilesList = GetProjectFilesList(modules.Element("FileIndices")?.Elements("File") ?? modules.Elements("File"))
            };

            foreach (var module in namespaces)
            {
                moduleIndex++;
                var classes = module.Elements("Type");
                var classIndex = 0;

                foreach (var projectClass in classes)
                {
                    classIndex++;
                    var coverageInfo = new ClassCoverage
                    {
                        CoveragePercent = Convert.ToDouble(projectClass.Attribute("CoveragePercent").Value)
                    };

                    int? fileId = null;
                    var methodCoverage = new List<LineCoverage>();
                    var methods = projectClass.Elements("Member");
                    if(!methods.Any())
                    {
                        methods = projectClass.Elements("Method");
                    }

                    foreach (var method in methods)
                    {

                        var lines = method.Elements("Statement");
                        if (lines.Any())
                        {
                            fileId = Convert.ToInt32(lines.First().Attribute("FileIndex").Value);
                            foreach (var line in lines)
                            {
                                methodCoverage.Add(new LineCoverage
                                {
                                    Line = Int32.Parse(line.Attribute("Line").Value),
                                    EndLine = Int32.Parse(line.Attribute("EndLine").Value),
                                    Covered = (line.Attribute("Covered").Value == "True") ? true : false
                                });
                            }
                        }

                        if (!(fileId is null) && report.FilesList.ContainsKey(fileId.Value))
                        {
                            coverageInfo.FileId = fileId.Value;
                            coverageInfo.CoveredLines = methodCoverage.ToList();
                            report.ClassCoverages.Add(coverageInfo);
                        }
                    }
                }
            }

			return report;
		}

        public CodacyReport Transform(Report report)
        {
            var coverageReport = new CodacyReport(report.ClassCoverages
                .GroupBy(g => report.FilesList[g.FileId])
                .Select(x => new Models.Result.FileInfo
                {
                    Filename = x.Key.Replace("\\", "/"),
                    Total = Convert.ToInt32(Math.Round(x.Average(a => a.CoveragePercent), 2, MidpointRounding.AwayFromZero)),
                    Coverage = x.SelectMany(s => s.CoveredLines.Select(cl =>
                    {
                        var lines = new List<(int LineNumber, bool Covered)>();
                        do
                        {
                            lines.Add((LineNumber: cl.Line, Covered: cl.Covered));
                            cl.Line++;
                        } while (cl.Line < cl.EndLine);
                        return lines;
                    }))
                        .SelectMany(i => i)
                        .GroupBy(g => g.LineNumber)
                        .Select(s => (LineNumber: s.Key, VisitCount: s.Sum(l => Convert.ToInt32(l.Covered))))
                        .OrderBy(o => Convert.ToInt32(o.LineNumber))
                        .ToDictionary(k => k.LineNumber, v => v.VisitCount)
                }).ToList());

            return coverageReport;
        }

        private Dictionary<int, string> GetProjectFilesList(IEnumerable<XElement> files)
        {
            var fileList = new Dictionary<int, string>();

            foreach (var file in files ?? Enumerable.Empty<XElement>())
            {
                var filePath = file.Attribute("Name").Value;
                //var srcPos = filePath.IndexOf(@"\src\", StringComparison.OrdinalIgnoreCase);
                //filePath = filePath.Substring(srcPos + 1, filePath.Length - srcPos - 1).Replace("\\", "/");
                fileList.Add(Convert.ToInt32(file.Attribute("Index").Value), filePath);
            }

            return fileList;
        }
    }
}
