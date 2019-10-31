using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Codacy.CSharpCoverage.Models.DotCover;
using Codacy.CSharpCoverage.Models.Result;

namespace Codacy.CSharpCoverage.Parsing.Processors
{
    /// <summary>
    ///     DotCover Processor.
    ///     This process the DotCover format into Codacy format.
    /// </summary>
    public class DotCoverProcessor : IProcessor<Report>
    {
        public Report Parse(string file)
        {
            var reportXml = XDocument.Load(file);
            var modules = reportXml.Root;
            var namespaces = modules.Elements("Assembly").SelectMany(x => x.Elements("Namespace"));

            var report = new Report
            {
                ClassCoverages = new List<ClassCoverage>(),
                FilesList = GetProjectFilesList(modules.Element("FileIndices")?.Elements("File") ??
                                                modules.Elements("File"))
            };

            if (report.FilesList.Count == 0) {
                Console.WriteLine ("Warning: Got a report without File elements. Consider report dotCover with --ReportType=DetailedXML");
            }

            foreach (var module in namespaces)
            {
                var classes = module.Elements("Type");

                foreach (var projectClass in classes)
                {
                    var coverageInfo = new ClassCoverage
                    {
                        CoveragePercent = Convert.ToDouble(projectClass.Attribute("CoveragePercent").Value)
                    };

                    int? fileId = null;
                    var methodCoverage = new List<LineCoverage>();
                    var methods = projectClass.Elements("Member");
                    if (!methods.Any())
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
                                    Line = int.Parse(line.Attribute("Line").Value),
                                    EndLine = int.Parse(line.Attribute("EndLine").Value),
                                    Covered = line.Attribute("Covered").Value == "True"
                                });
                            }
                        }

                        if (fileId != null && report.FilesList.ContainsKey(fileId.Value))
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
                .Select(x => new FileInfo
                {
                    Filename = x.Key.Replace("\\", "/"),
                    Total = Convert.ToInt32(Math.Round(x.Average(a => a.CoveragePercent), 2,
                        MidpointRounding.AwayFromZero)),
                    Coverage = x.SelectMany(s => s.CoveredLines.Select(cl =>
                        {
                            var lines = new List<(int LineNumber, bool Covered)>();
                            do
                            {
                                lines.Add((LineNumber: cl.Line, cl.Covered));
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

        /// <summary>
        ///     Get the project files list.
        ///     This function maps a XElement into a KeyValuePair. It has
        ///     a unique file id and a file path for that id.
        /// </summary>
        /// <param name="files">list of files in XElement format</param>
        /// <returns>dictionary of file id and file path</returns>
        private static Dictionary<int, string> GetProjectFilesList(IEnumerable<XElement> files)
        {
            var fileList = new Dictionary<int, string>();

            foreach (var file in files ?? Enumerable.Empty<XElement>())
            {
                var filePath = file.Attribute("Name").Value;
                fileList.Add(Convert.ToInt32(file.Attribute("Index").Value), filePath);
            }

            return fileList;
        }
    }
}
