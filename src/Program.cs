using System;
using System.Net;
using System.Net.Http;
using System.Text;
using Codacy.CSharpCoverage.Models.Result;
using Codacy.CSharpCoverage.Parsing.Processors;
using CommandLine;

namespace Codacy.CSharpCoverage
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            // parse the option arguments
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(opt =>
                {
                    if (opt.Final)
                    {
                        MakeFinalRequest(opt.CommitUUID, opt.Token);
                    }
                    else if (string.IsNullOrEmpty(opt.ReportFile))
                    {
                        throw new FormatException(
                            "Unspecified report file! Use -r or type --help");
                    }
                    else if (string.IsNullOrEmpty(opt.ReportType))
                    {
                        throw new FormatException(
                            "Unspecified report type! Use -e or type --help");
                    }
                    else
                    {
                        CodacyReport report;

                        switch (opt.ReportType)
                        {
                            case "dotcover":
                                var dotCoverProcessor = new DotCoverProcessor();
                                var dotCoverParsed = dotCoverProcessor.Parse(opt.ReportFile);
                                report = dotCoverProcessor.Transform(dotCoverParsed);
                                break;
                            case "opencover":
                                var openCoverProcessor = new OpenCoverProcessor();
                                var openCoverParsed = openCoverProcessor.Parse(opt.ReportFile);
                                report = openCoverProcessor.Transform(openCoverParsed);
                                break;

                            default:
                                throw new FormatException(
                                    "Unrecognized report format, please choose dotcover or opencover");
                        }

                        SendReport(report, opt.CommitUUID, opt.Token, opt.Partial);
                    }
                });
        }

        /// <summary>
        ///     Send the report using Codacy API
        /// </summary>
        /// <param name="report">codacy report result</param>
        /// <param name="commitUuid">commit uuid</param>
        /// <param name="projectToken">project token</param>
        /// <param name="isPartial">partial flag</param>
        /// <exception cref="FormatException">if it's passed an invalid commit uuid or project token</exception>
        /// <exception cref="HttpRequestException">if the api response status code is not 200.</exception>
        private static void SendReport(CodacyReport report, string commitUuid, string projectToken, bool isPartial)
        {
            if (string.IsNullOrEmpty(commitUuid))
            {
                throw new FormatException("Invalid commit UUID");
            }

            if (string.IsNullOrEmpty(projectToken))
            {
                throw new FormatException("Invalid project token");
            }

            MakeReportRequest(report.ToString(), commitUuid, projectToken, isPartial);

            Console.WriteLine(report.GetStats());
        }

        /// <summary>
        ///     Get base API URL or the default as a fallback.
        /// </summary>
        /// <returns>Return Codacy base API URL</returns>
        /// <exception cref="FormatException">If environment variable has an invalid URL</exception>
        private static string GetBaseApiOrDefault()
        {
            // gets the environment variable
            var envApi = Environment.GetEnvironmentVariable("CODACY_API_BASE_URL");

            // check if the url is valid
            if (!(envApi?.StartsWith("http") ?? true))
            {
                throw new FormatException(
                    "Invalid custom API base URL! Need to start w/ http");
            }

            // return environment variable or default as a fallback
            return envApi ?? "https://api.codacy.com";
        }

        /// <summary>
        ///     Make the final request to the API.
        ///     To complete a coverage report, you need to send this, otherwise,
        ///     the partial coverage reports will not be aggregated and then not available.
        /// </summary>
        /// <param name="commitUuid">commit uuid</param>
        /// <param name="projectToken">project token</param>
        /// <returns>an async http response</returns>
        private static HttpResponseMessage MakeFinalRequest(string commitUuid, string projectToken)
        {
            return MakeRequest("{}", $"/2.0/commit/{commitUuid}/coverageFinal",
                projectToken);
        }

        /// <summary>
        ///     Make the coverage report request to the API.
        ///     To send a coverage report, you need to send this request.
        /// </summary>
        /// <param name="json">string-based json (body content of the request)</param>
        /// <param name="commitUuid">commit uuid</param>
        /// <param name="projectToken">project token</param>
        /// <param name="isPartial">partial flag</param>
        /// <returns></returns>
        private static HttpResponseMessage MakeReportRequest(string json, string commitUuid,
            string projectToken,
            bool isPartial)
        {
            var partial = isPartial ? "true" : "false";

            return MakeRequest(json,
                $"/2.0/coverage/{commitUuid}/CSharp?partial={partial}", projectToken);
        }

        /// <summary>
        ///     Make a generic request to Codacy API.
        ///     This sets the required request headers and send the specified content to
        ///     a certain endpoint.
        /// </summary>
        /// <param name="content">string-based json content</param>
        /// <param name="endpoint">api endpoint</param>
        /// <param name="projectToken">project token</param>
        /// <returns></returns>
        private static HttpResponseMessage MakeRequest(string content, string endpoint, string projectToken)
        {
            //prepare url with base api url and the endpoint
            var destUri = new Uri($"{GetBaseApiOrDefault()}{endpoint}");

            //required headers
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Add("project_token", projectToken);

            //post request
            var res = client.PostAsync(destUri, new StringContent(content, Encoding.UTF8, "application/json")).Result;

            Console.WriteLine(res.Content);
            Console.WriteLine("Response status: " + res.StatusCode);

            if (res.StatusCode != HttpStatusCode.OK)
            {
                throw new HttpRequestException("Unexpected response status code!");
            }
            else
            {
                return res;
            }
        }

        /// <summary>
        ///     Program options.
        ///     This class specifies the arguments passed to the program when it's called.
        /// </summary>
        public class Options
        {
            [Option('p', "partial", Required = false, HelpText = "Send report as a partial report", Default = false)]
            public bool Partial { get; set; }

            [Option('f', "final", Required = false, HelpText = "Send final coverage report request", Default = false)]
            public bool Final { get; set; }

            [Option('c', "commit", Required = true, HelpText = "Specify the commit UUID")]
            public string CommitUUID { get; set; }

            [Option('t', "token", Required = true, HelpText = "Specify the project token")]
            public string Token { get; set; }

            [Option('r', "report", Required = false, HelpText = "Path to the coverage report")]
            public string ReportFile { get; set; }

            [Option('e', "engine", Required = false, HelpText = "Engine Report Type (dotcover, opencover).")]
            public string ReportType { get; set; }
        }
    }
}
