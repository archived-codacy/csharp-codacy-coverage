using System;
using CommandLine;
using cs_codacy_coverage.Models;
using cs_codacy_coverage.Parsers;

using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;


namespace cs_codacy_coverage
{
    class Program
    {
        public class Options
        {
            [Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.", Default = false)]
            public bool Verbose { get; set; }

            [Option('p', "partial", Required = false, HelpText = "Is a partial report", Default = false)]
            public bool Partial { get; set; }

            [Option('e', "engine", Required = true, HelpText = "Engine Report Type (dotcover, opencover).")]
            public string ReportType { get; set; }

            [Option('c', "commituuid", Required = true, HelpText = "The CommitUUID")]
            public string CommitUuid { get; set; }

            [Option('t', "projecttoken", Required = true, HelpText = "The Project Token")]
            public string ProjectToken { get; set; }

            [Option('r', "report", Required = true, HelpText = "Path to the coverage report")]
            public string CoverageFile { get; set; }
        }

        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                   .WithParsed<Options>(o =>
                   {
                       BaseParser parser;
                       CoverageReport report;
                       switch (o.ReportType)
                       {
                           case "dotcover":
                               parser = new DotCoverParser()
                               {
                                   CoverageFile = @"/Users/hjrocha/dev/cs_coverage_samples/CoverageReport.xml"

                               };
                               break;
                           case "opencover":
                               parser = new OpenCoverParser()
                               {

                               };
                               break;
                           default:
                               Console.WriteLine("Unrecognized report format, please choose dotcover or opencover");
                               return;
                        

                       }
                       report = parser.Process();
                       SendReport(report, o.CommitUuid, o.ProjectToken);




                   });
                   
        }

        private static void SendReport(CoverageReport coverage, string commitUuid, string projectToken)
        {
            try
            {
                var obj = JsonConvert.SerializeObject(coverage);

                var response = MakeRequest(obj, commitUuid, projectToken).Result;
                Console.WriteLine("Coverage report:");
                Console.WriteLine(obj);

                Console.WriteLine(response.Content);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        static async Task<HttpResponseMessage> MakeRequest(string json, string commitUuid, string projectToken)
        {
            var url = $"https://api.codacy.com/2.0/coverage/{commitUuid}/CSharp?partial=false";
            var destUri = new Uri(url);
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Add("project_token", projectToken);
            var res = await client.PostAsync(destUri, new StringContent(json, Encoding.UTF8, "application/json"));
            return res;
        }
    }
}
