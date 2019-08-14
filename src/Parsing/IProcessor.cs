using Codacy.CSharpCoverage.Models;

namespace Codacy.CSharpCoverage.Parsing
{
    public interface IProcessor<T> : ITransformer<T>, IParser<T> where T : IReport
    {
    }
}
