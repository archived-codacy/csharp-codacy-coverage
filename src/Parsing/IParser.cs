using Codacy.CSharpCoverage.Models;

namespace Codacy.CSharpCoverage.Parsing
{
    public interface IParser<out T> where T : IReport
    {
        T Parse(string file);
    }
}
