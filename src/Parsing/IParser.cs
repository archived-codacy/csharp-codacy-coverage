namespace Codacy.CSharpCoverage.Parsing
{
    using Models;

    public interface IParser<T> where T : IReport
    {
        T Parse(string file);
    }
}
