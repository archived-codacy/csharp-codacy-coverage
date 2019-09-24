using Codacy.CSharpCoverage.Models;

namespace Codacy.CSharpCoverage.Parsing
{
    /// <summary>
    ///     Parser interface.
    ///     This implements a parse function to parse the
    ///     report file.
    /// </summary>
    /// <typeparam name="T">report model type</typeparam>
    public interface IParser<out T> where T : IReport
    {
        /// <summary>
        ///     Parse.
        ///     This parse a report file into the report
        ///     model.
        /// </summary>
        /// <param name="file">report file</param>
        /// <returns>report model</returns>
        T Parse(string file);
    }
}
