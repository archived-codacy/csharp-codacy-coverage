using cs_codacy_coverage.Models;

namespace cs_codacy_coverage.Parsers
{
    public abstract class BaseParser
    {
        public string ProjectToken { get; set; }
        public string CommitUuid { get; set; }
        public string CoverageFile { get; set; }
        
        public abstract CoverageReport Process();
    }
}