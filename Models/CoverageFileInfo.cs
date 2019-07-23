using System;
using System.Collections.Generic;

namespace cs_codacy_coverage.Models
{
    public class CoverageFileInfo
    {
        public string filename { get; set; }
        public int total { get; set; }
        public Dictionary<string, int> coverage { get; set; }
    }

}