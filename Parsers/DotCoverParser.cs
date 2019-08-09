using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Linq;

using cs_codacy_coverage.Models;

namespace cs_codacy_coverage.Parsers
{
    class DotCoverParser : BaseParser
    {
        public override CoverageReport Process()
        {
            var classCoverages = new List<ClassCoverage>();

            var report = XDocument.Load(CoverageFile);
            var modules = report.Root;
            var namespaces = modules.Elements("Assembly").SelectMany(x => x.Elements("Namespace"));
            //modules.Elements("SolutionFolder").Elements("Project").SelectMany(x => x.Elements("Namespace"));
            var moduleIndex = 0;
            Dictionary<string, string> fileList;
            if(modules.Element("FileIndices") != null)
            {
                fileList = GetProjectFilesList(modules.Element("FileIndices").Elements("File"));
            }
            else
            {
                fileList = GetProjectFilesList(modules.Elements("File"));
            }
          

            foreach (var module in namespaces)
            {
                moduleIndex++;
                var classes = module.Elements("Type");
                var classIndex = 0;

                foreach (var projectClass in classes)
                {
                    classIndex++;
                    var coverage = Convert.ToDouble(projectClass.Attribute("CoveragePercent").Value);
                    var coverageInfo = new ClassCoverage
                    {
                        Total = Convert.ToInt32(Math.Round((double)coverage, 0, MidpointRounding.AwayFromZero)),
                        ClassName = projectClass.Attribute("Name").Value
                    };

                    string fileId = String.Empty;
                    var methodCoverage = new List<LineCoverage>();
                    var methods = projectClass.Elements("Member");
                    if(methods.Count() == 0)
                    {
                        methods = projectClass.Elements("Method");
                    }
                    foreach (var method in methods)
                    {

                        var lines = method.Elements("Statement");
                        if (lines.Count() > 0)
                        {
                            fileId = lines.First().Attribute("FileIndex").Value;
                            foreach (var line in lines)
                            {
                                int firstLine = Int32.Parse(line.Attribute("Line").Value);
                                int endLine = Int32.Parse(line.Attribute("EndLine").Value);
                                do
                                {
                                    methodCoverage.Add(new LineCoverage { LineNumber = firstLine.ToString(), VisitCount = (line.Attribute("Covered").Value == "True") ? 1 : 0 });
                                    firstLine++;
                                } while (firstLine < endLine);
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
                var filePath = file.Attribute("Name").Value;
                filePath = filePath.Replace("\\", "/");
                //var srcPos = filePath.IndexOf(@"\src\", StringComparison.OrdinalIgnoreCase);
                //filePath = filePath.Substring(srcPos + 1, filePath.Length - srcPos - 1).Replace("\\", "/");
                fileList.Add(file.Attribute("Index").Value, filePath);
            }

            return fileList;
        }
    }
}