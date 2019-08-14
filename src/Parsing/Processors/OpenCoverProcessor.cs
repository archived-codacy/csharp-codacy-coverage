using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Linq;

using Codacy.CSharpCoverage.Models.Result;
using Codacy.CSharpCoverage.Models.OpenCover;

//Coverage tool: OpenCover
//Usage (regular): https://github.com/OpenCover/opencover/wiki/Usage
//Usage (dotnet core): opencover.console.exe -register:user -target:"C:/Program Files/dotnet/dotnet.exe" -targetargs:"test UnitTestProject" -output:"coverage.xml"

namespace Codacy.CSharpCoverage.Parsing.Processors
{

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

            var moduleIndex = 0;
            foreach (var module in modules)
            {
                moduleIndex++;

				var moduleInfo = new ModuleElement
				{
					FilesList = GetProjectFilesList(module.Element("Files")?.Elements("File")),
                    ClassCoverages = new List<ClassCoverage>()
				};

				var classes = module.Element("Classes").Elements("Class");

                var classIndex = 0;
                foreach (var projectClass in classes)
                {
                    classIndex++;
                    var summaryElem = projectClass.Element("Summary");
                    if (summaryElem is null)
                    {
                        continue;
                    }

                    var coverageInfo = new Models.OpenCover.ClassCoverage
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
                            if (method.Element("FileRef") is null)
                            {
                                continue;
                            }

                            fileId = Convert.ToInt32(method.Element("FileRef").Attribute("uid").Value);
                            var lines = method.Element("SequencePoints");
                            if (lines != null && lines.Elements("SequencePoint").Any())
                            {
                                foreach (var line in lines.Elements("SequencePoint"))
                                {
                                    methodCoverage.Add(new LineCoverage { LineNumber = Convert.ToInt32(line.Attribute("sl").Value), VisitCount = Convert.ToInt32(line.Attribute("vc").Value) });
                                }
                            }
                        }
                    }

                    if (!(fileId is null) && moduleInfo.FilesList.ContainsKey(fileId.Value))
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
                    Total = Convert.ToInt32(Math.Round(x.Average(a => a.SequenceCoverage), 2, MidpointRounding.AwayFromZero)),
                    Coverage = x.SelectMany(s => s.CoveredLines)
                        .GroupBy(g => g.LineNumber)
                        .Select(s => new { LineNumber = s.Key, VisitCount = s.Sum(l => l.VisitCount) })
                        .OrderBy(o => o.LineNumber)
                        .ToDictionary(k => k.LineNumber, v => v.VisitCount)
                }));
        }

        private Dictionary<int, string> GetProjectFilesList(IEnumerable<XElement> files)
        {
            var fileList = new Dictionary<int, string>();

            foreach (var file in files ?? Enumerable.Empty<XElement>())
            {
                var filePath = file.Attribute("fullPath").Value;
                fileList.Add(Convert.ToInt32(file.Attribute("uid").Value), filePath);
            }

            return fileList;
        }
    }
}
