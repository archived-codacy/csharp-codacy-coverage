namespace Codacy.CSharpCoverage.Parsing
{
    using Models;
	using Models.Result;
	public interface IProcessor<T> : ITransformer<T>, IParser<T> where T : IReport {}
}
