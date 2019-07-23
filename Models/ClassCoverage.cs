using System;
using System.Collections.Generic;

namespace cs_codacy_coverage.Models
{
    public class ClassCoverage
    {
        public string FileName { get; set; }
        public string ClassName { get; set; }
        public int Total { get; set; }
        public List<LineCoverage> CoveredLines { get; set; }
    }
}