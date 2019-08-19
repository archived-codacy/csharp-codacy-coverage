using System;
using System.Collections.Generic;
using Codacy.CSharpCoverage.Models.DotCover;
using Codacy.CSharpCoverage.Models.Result;
using Codacy.CSharpCoverage.Parsing.Processors;
using Xunit;

namespace Codacy.CSharpCoverage.Tests
{
    public class DotCoverTests
    {
        public DotCoverTests()
        {
            processor = new DotCoverProcessor();
        }

        private readonly DotCoverProcessor processor;

        [Fact]
        public void BadParseTest()
        {
            Assert.ThrowsAny<Exception>(() => processor.Parse(@"Resources/Bad.xml"));
        }

        [Fact]
        public void BadReportTest()
        {
            Assert.ThrowsAny<Exception>(() => processor.Transform(new Report()));
        }

        [Fact]
        public void GoodParseTest()
        {
            var reportParsed = processor.Parse(@"Resources/GoodDotCover.xml");
            var expectedResult = new Report
            {
                FilesList = new Dictionary<int, string>
                {
                    {1, "foo.cs"},
                    {2, "bar.cs"},
                    {3, "foobar.cs"}
                },
                ClassCoverages = new List<ClassCoverage>(new[]
                {
                    new ClassCoverage
                    {
                        FileId = 1,
                        CoveragePercent = 100.0,
                        CoveredLines = new List<LineCoverage>(new[]
                        {
                            new LineCoverage
                            {
                                Line = 10,
                                EndLine = 10,
                                Covered = true
                            }
                        })
                    },
                    new ClassCoverage
                    {
                        FileId = 2,
                        CoveragePercent = 0.0,
                        CoveredLines = new List<LineCoverage>(new[]
                        {
                            new LineCoverage
                            {
                                Line = 10,
                                EndLine = 10,
                                Covered = false
                            }
                        })
                    },
                    new ClassCoverage
                    {
                        FileId = 3,
                        CoveragePercent = 50.0,
                        CoveredLines = new List<LineCoverage>(new[]
                        {
                            new LineCoverage
                            {
                                Line = 10,
                                EndLine = 10,
                                Covered = false
                            },
                            new LineCoverage
                            {
                                Line = 20,
                                EndLine = 20,
                                Covered = true
                            }
                        })
                    }
                })
            };

            Assert.Equal(expectedResult, reportParsed);
        }

        [Fact]
        public void GoodReportTest()
        {
            var reportParsed = processor.Parse(@"Resources/GoodDotCover.xml");
            var result = processor.Transform(reportParsed);

            var fileInfo = new List<FileInfo>(new[]
            {
                new FileInfo
                {
                    Filename = "foo.cs",
                    Total = 100,
                    Coverage = new Dictionary<int, int>
                    {
                        {10, 1}
                    }
                },
                new FileInfo
                {
                    Filename = "bar.cs",
                    Total = 0,
                    Coverage = new Dictionary<int, int>
                    {
                        {10, 0}
                    }
                },
                new FileInfo
                {
                    Filename = "foobar.cs",
                    Total = 50,
                    Coverage = new Dictionary<int, int>
                    {
                        {10, 0},
                        {20, 1}
                    }
                }
            });
            var expectedResult = new CodacyReport(fileInfo);
            Assert.Equal(expectedResult, result);
        }
    }
}
