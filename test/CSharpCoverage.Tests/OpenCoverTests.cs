using System.Collections.Generic;
using System.Linq;
using System;
using Xunit;

namespace Codacy.CSharpCoverage.Tests {
    using Models.OpenCover;
    using Parsing.Processors;
	using Models.Result;

	public class OpenCoverTests {
        private readonly OpenCoverProcessor processor;

		public OpenCoverTests () {
            processor = new OpenCoverProcessor ();
        }

        [Fact]
        public void GoodReportTest () {
            Report reportParsed = processor.Parse (@"Resources/GoodOpenCover.xml");
			CodacyReport result = processor.Transform(reportParsed);

			var fileInfo = new List<FileInfo>(new[] {
                new FileInfo {
                    Filename = "foo.cs",
                    Total = 100,
                    Coverage = new Dictionary<int, int>{
                        {10,1}
                    }
                },
                new FileInfo {
                    Filename = "bar.cs",
                    Total = 0,
                    Coverage = new Dictionary<int, int>{
                        {10,0}
                    }
                },
                new FileInfo {
                    Filename = "foobar.cs",
                    Total = 50,
                    Coverage = new Dictionary<int, int>{
                        {10,0},
                        {20,1}
                    }
                }
			});
			var expectedResult = new CodacyReport(fileInfo);
            Assert.Equal(expectedResult, result);
		}

        [Fact]
        public void BadReportTest () => Assert.ThrowsAny<Exception>(() => processor.Transform(new Report()));

        [Fact]
        public void GoodParseTest () {
            Report reportParsed = processor.Parse (@"Resources/GoodOpenCover.xml");
            var expectedResult = new Report
			{
                ModuleElements = new List<ModuleElement>(new [] {
                    new ModuleElement {
                        FilesList = new Dictionary<int, string>()
                        {
                            {1, "foo.cs"},
                            {2, "bar.cs"},
                            {3, "foobar.cs"}
                        },
                        ClassCoverages = new List<ClassCoverage>(new [] {
                            new ClassCoverage {
                                FileId = 1,
                                SequenceCoverage = 100.0,
                                CoveredLines = new List<LineCoverage>(new [] {
                                    new LineCoverage {
                                        LineNumber = 10,
                                        VisitCount = 1
                                    }
                                })
                            },
                            new ClassCoverage {
                                FileId = 2,
                                SequenceCoverage = 0.0,
                                CoveredLines = new List<LineCoverage>(new [] {
                                    new LineCoverage {
                                        LineNumber = 10,
                                        VisitCount = 0
                                    }
                                })
                            },
                            new ClassCoverage {
                                FileId = 3,
                                SequenceCoverage = 50.0,
                                CoveredLines = new List<LineCoverage>(new [] {
                                    new LineCoverage {
                                        LineNumber = 10,
                                        VisitCount = 0
                                    },
                                    new LineCoverage {
                                        LineNumber = 20,
                                        VisitCount = 1
                                    }
                                })
                            }
                        })
                    }
                })
			};

			Assert.Equal(expectedResult, reportParsed);
		}

        [Fact]
        public void BadParseTest () => Assert.ThrowsAny<Exception>(() => processor.Parse(@"Resources/Bad.xml"));
    }
}
