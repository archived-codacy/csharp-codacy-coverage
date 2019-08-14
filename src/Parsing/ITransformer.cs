using Codacy.CSharpCoverage.Models;
using Codacy.CSharpCoverage.Models.Result;

namespace Codacy.CSharpCoverage.Parsing
{
    public interface ITransformer<in T> where T : IReport
    {
        CodacyReport Transform(T report);
    }
}
