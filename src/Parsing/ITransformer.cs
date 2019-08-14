namespace Codacy.CSharpCoverage.Parsing
{
    using Models;
    using Models.Result;
    public interface ITransformer<T> where T : IReport
    {
        CodacyReport Transform(T report);
    }
}
