using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Linq;

using cs_codacy_coverage.Models;

//Coverage tool: OpenCover
//Usage (regular): https://github.com/OpenCover/opencover/wiki/Usage
//Usage (dotnet core): opencover.console.exe -register:user -target:"C:/Program Files/dotnet/dotnet.exe" -targetargs:"test UnitTestProject" -output:"coverage.xml"

namespace cs_codacy_coverage.Parsers
{
    class OpenCoverParser : BaseParser
    {
        public override CoverageReport Process()
        {

            var classCoverages = new List<ClassCoverage>();

            var report = XDocument.Load(CoverageFile);
            var modules = report.Root.Elements("Modules").SelectMany(x => x.Elements("Module"));

            var moduleIndex = 0;
            foreach (var module in modules)
            {
                moduleIndex++;
                var fileList = GetProjectFilesList(module.Element("Files")?.Elements("File"));

                var classes = module.Element("Classes").Elements("Class");

                var classIndex = 0;
                foreach (var projectClass in classes)
                {
                    classIndex++;
                    var summaryElem = projectClass.Element("Summary");
                    if (summaryElem == null)
                    {
                        continue;
                    }

                    var coverage = Convert.ToDouble(summaryElem.Attribute("sequenceCoverage").Value);
                    var coverageInfo = new ClassCoverage
                    {
                        Total = Convert.ToInt32(Math.Round(coverage, 0, MidpointRounding.AwayFromZero)),
                        ClassName = projectClass.Element("FullName").Value
                    };

                    string fileId = String.Empty;
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
                            fileId = method.Element("FileRef").Attribute("uid").Value;
                            var lines = method.Element("SequencePoints");
                            if (lines != null && lines.Elements("SequencePoint").Any())
                            {
                                foreach (var line in lines.Elements("SequencePoint"))
                                {
                                    methodCoverage.Add(new LineCoverage { LineNumber = line.Attribute("sl").Value, VisitCount = Convert.ToInt32(line.Attribute("vc").Value) });
                                }
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(fileId) && fileList.ContainsKey(fileId))
                    {
                        coverageInfo.FileName = fileList[fileId];
                        coverageInfo.CoveredLines = methodCoverage.ToList();
                        classCoverages.Add(coverageInfo);
                    }

                }

            }


            var coverageReport = new CoverageReport
            {
                total = 0,
                fileReports = classCoverages.GroupBy(g => g.FileName)
                    .Select(x => new CoverageFileInfo
                    {
                        filename = x.Key,
                        total = Convert.ToInt32(Math.Round(x.Average(a => a.Total), 2, MidpointRounding.AwayFromZero)),
                        coverage = x.SelectMany(s => s.CoveredLines)
                            .GroupBy(g => g.LineNumber)
                            .Select(s => new { LineNumber = s.Key, VisitCount = s.Sum(l => l.VisitCount) })
                            .OrderBy(o => Convert.ToInt32(o.LineNumber))
                            .ToDictionary(k => k.LineNumber, v => v.VisitCount)
                    }).ToList()
            };


            return coverageReport;
        }

        private Dictionary<string, string> GetProjectFilesList(IEnumerable<XElement> files)
        {
            var fileList = new Dictionary<string, string>();
            if (files == null)
            {
                return fileList;
            }
            foreach (var file in files)
            {
                var filePath = file.Attribute("fullPath").Value;
                var srcPos = filePath.IndexOf(@"\src\");
                filePath = filePath.Substring(srcPos + 1, filePath.Length - srcPos - 1).Replace("\\", "/");
                fileList.Add(file.Attribute("uid").Value, filePath);
            }

            return fileList;
        }

        
    }










}
