using Codacy.CSharpCoverage.Models;
using Codacy.CSharpCoverage.Models.Result;

namespace Codacy.CSharpCoverage.Parsing
{
    /// <summary>
    ///     Transformer interface.
    ///     This implements a tranform function, to convert a report into
    ///     codacy report.
    /// </summary>
    /// <typeparam name="T">report model type</typeparam>
    public interface ITransformer<in T> where T : IReport
    {
        /// <summary>
        ///     Transform.
        ///     This transform a T report, into codacy report format.
        /// </summary>
        /// <param name="report">report</param>
        /// <returns>codacy report</returns>
        CodacyReport Transform(T report);
    }
}
