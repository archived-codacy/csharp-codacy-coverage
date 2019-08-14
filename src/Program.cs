using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using CommandLine;
using Newtonsoft.Json;

namespace Codacy.CSharpCoverage {
    using Models.Result;
    using Models;
    using Parsing.Processors;

    class Program {
        public class Options {
            [Option ('p', "partial", Required = false, HelpText = "Send report as a partial report", Default = false)]
            public bool Partial { get; set; }

            [Option ('c', "commit", Required = true, HelpText = "Specify the commit UUID")]
            public string CommitUUID { get; set; }

            [Option ('t', "token", Required = true, HelpText = "Specify the project token")]
            public string Token { get; set; }

            [Option ('r', "report", Required = true, HelpText = "Path to the coverage report")]
            public string ReportFile { get; set; }

            [Option ('e', "engine", Required = true, HelpText = "Engine Report Type (dotcover, opencover).")]
            public string ReportType { get; set; }
        }

        static void Main (string[] args) {
            Parser.Default.ParseArguments<Options> (args)
                .WithParsed<Options> (opt => {
                    CodacyReport report;

                    switch (opt.ReportType) {
                        case "dotcover":
                            var dotCoverProcessor = new DotCoverProcessor ();
                            var dotCoverParsed = dotCoverProcessor.Parse (opt.ReportFile);
                            report = dotCoverProcessor.Transform (dotCoverParsed);
                            break;
                        case "opencover":
                            var openCoverProcessor = new OpenCoverProcessor ();
                            var openCoverParsed = openCoverProcessor.Parse (opt.ReportFile);
                            report = openCoverProcessor.Transform (openCoverParsed);
                            break;

                        default:
                            throw new Exception ("Unrecognized report format, please choose dotcover or opencover");

                    }

					SendReport (report, opt.CommitUUID, opt.Token, opt.Partial);
                });
        }

        private static void SendReport (CodacyReport result, string commitUuid, string projectToken, bool isPartial) {
            try {
				var response = MakeRequest (result.ToString (), commitUuid, projectToken, isPartial).Result;
				Console.WriteLine (result.GetStats ());

                Console.WriteLine (response.Content);
                Console.WriteLine ("Response status: " + response.StatusCode);
            } catch (Exception e) {
                Console.WriteLine (e.StackTrace);
            }
        }

        static async Task<HttpResponseMessage> MakeRequest (string json, string commitUuid, string projectToken, bool isPartial) {
            var partial = isPartial ? "true" : "false";
            var url = $"https://api.codacy.com/2.0/coverage/{commitUuid}/CSharp?partial={partial}";
            var destUri = new Uri (url);
            var client = new HttpClient ();
            client.DefaultRequestHeaders.Add ("Accept", "application/json");
            client.DefaultRequestHeaders.Add ("project_token", projectToken);
            var res = await client.PostAsync (destUri, new StringContent (json, Encoding.UTF8, "application/json"));
            return res;
        }
    }
}
