namespace Codacy.CSharpCoverage.Models.OpenCover
{
    public sealed class LineCoverage
    {
        public int LineNumber { get; set; }
        public int VisitCount { get; set; }

        public override bool Equals(object obj)
        {
            if (this == obj)
            {
                return true;
            } else if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            else
            {
                var l = (LineCoverage) obj;

                return LineNumber == l.LineNumber &&
                       VisitCount == l.VisitCount;
            }
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = (int) 2166136261;
                hash = (hash * 16777619) ^ LineNumber.GetHashCode();
                return (hash * 16777619) ^ VisitCount.GetHashCode();
            }
        }
    }
}
