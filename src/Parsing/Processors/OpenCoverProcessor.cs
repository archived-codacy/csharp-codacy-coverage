using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Codacy.CSharpCoverage.Models.OpenCover;
using Codacy.CSharpCoverage.Models.Result;

//Coverage tool: OpenCover
//Usage (regular): https://github.com/OpenCover/opencover/wiki/Usage
//Usage (dotnet core): opencover.console.exe -register:user -target:"C:/Program Files/dotnet/dotnet.exe" -targetargs:"test UnitTestProject" -output:"coverage.xml"

namespace Codacy.CSharpCoverage.Parsing.Processors
{
    /// <summary>
    ///     OpenCover Processor.
    ///     This process the OpenCover format into Codacy format.
    /// </summary>
    public class OpenCoverProcessor : IProcessor<Report>
    {
        public Report Parse(string file)
        {
            var coverageReport = new Report
            {
                ModuleElements = new List<ModuleElement>()
            };

            var report = XDocument.Load(file);
            var modules = report.Root.Elements("Modules").SelectMany(x => x.Elements("Module"));

            foreach (var module in modules)
            {
                var moduleInfo = new ModuleElement
                {
                    FilesList = GetProjectFilesList(module.Element("Files")?.Elements("File")),
                    ClassCoverages = new List<ClassCoverage>()
                };

                var classes = module.Element("Classes").Elements("Class");

                foreach (var projectClass in classes)
                {
                    var summaryElem = projectClass.Element("Summary");
                    if (summaryElem == null)
                    {
                        continue;
                    }

                    var coverageInfo = new ClassCoverage
                    {
                        SequenceCoverage = Convert.ToDouble(summaryElem.Attribute("sequenceCoverage").Value)
                    };

                    int? fileId = null;
                    var methodCoverage = new List<LineCoverage>();

                    var methods = projectClass.Element("Methods");
                    if (methods != null && methods.Elements("Method").Any())
                    {
                        foreach (var method in methods.Elements("Method"))
                        {
                            if (method.Element("FileRef") == null)
                            {
                                continue;
                            }

                            fileId = Convert.ToInt32(method.Element("FileRef").Attribute("uid").Value);
                            var lines = method.Element("SequencePoints");
                            if (lines != null && lines.Elements("SequencePoint").Any())
                            {
                                foreach (var line in lines.Elements("SequencePoint"))
                                {
                                    methodCoverage.Add(new LineCoverage
                                    {
                                        LineNumber = Convert.ToInt32(line.Attribute("sl").Value),
                                        VisitCount = Convert.ToInt32(line.Attribute("vc").Value)
                                    });
                                }
                            }
                        }
                    }

                    if (fileId != null && moduleInfo.FilesList.ContainsKey(fileId.Value))
                    {
                        coverageInfo.FileId = fileId.Value;
                        coverageInfo.CoveredLines = methodCoverage.ToList();
                        moduleInfo.ClassCoverages.Add(coverageInfo);
                    }
                }

                coverageReport.ModuleElements.Add(moduleInfo);
            }

            return coverageReport;
        }

        public CodacyReport Transform(Report report)
        {
            //report.ClassCoverages
            return new CodacyReport(report.ModuleElements
                .Select(m => m.ClassCoverages
                    .GroupBy(g => m.FilesList[g.FileId]))
                .SelectMany(x => x)
                .Select(x => new FileInfo
                {
                    Filename = x.Key.Replace("\\", "/"),
                    Total = Convert.ToInt32(Math.Round(x.Average(a => a.SequenceCoverage), 2,
                        MidpointRounding.AwayFromZero)),
                    Coverage = x.SelectMany(s => s.CoveredLines)
                        .GroupBy(g => g.LineNumber)
                        .Select(s => new {LineNumber = s.Key, VisitCount = s.Sum(l => l.VisitCount)})
                        .OrderBy(o => o.LineNumber)
                        .ToDictionary(k => k.LineNumber, v => v.VisitCount)
                }));
        }

        /// <summary>
        ///     Get the project files list.
        ///     This function maps a XElement into a KeyValuePair. It has
        ///     a unique file id and a file path for that id.
        /// </summary>
        /// <param name="files">list of files in XElement format</param>
        /// <returns>dictionary of file id and file path</returns>
        private Dictionary<int, string> GetProjectFilesList(IEnumerable<XElement> files)
        {
            var fileList = (files ?? Enumerable.Empty<XElement>()).Aggregate(new Dictionary<int, string>(),
                (accum, next) =>
                {
                    var filePath = next.Attribute("fullPath").Value;
                    accum.Add(Convert.ToInt32(next.Attribute("uid").Value), filePath);

                    return accum;
                });

            return fileList;
        }
    }
}
