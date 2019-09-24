using Codacy.CSharpCoverage.Models;

namespace Codacy.CSharpCoverage.Parsing
{
    /// <summary>
    ///     Processor interface.
    ///     This implements a Transformer and a Parser.
    /// </summary>
    /// <typeparam name="T">report model type</typeparam>
    public interface IProcessor<T> : ITransformer<T>, IParser<T> where T : IReport
    {
    }
}
