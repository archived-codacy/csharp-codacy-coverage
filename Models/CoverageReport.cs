using System;
using System.Collections.Generic;

namespace cs_codacy_coverage.Models
{
    public class CoverageReport
    {
        public int total { get; set; }
        public List<CoverageFileInfo> fileReports { get; set; }
    }

}