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
            //.Elements("SolutionFolder").Elements("Project").SelectMany(x => x.Elements("Namespace"));
            var moduleIndex = 0;

            return null;
        }
    }
}