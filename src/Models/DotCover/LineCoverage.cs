namespace Codacy.CSharpCoverage.Models.DotCover
{
    public sealed class LineCoverage
    {
        public int Line { get; set; }
        public int EndLine { get; set; }
        public bool Covered { get; set; }

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
                return Line == l.Line && EndLine == l.EndLine && Covered == l.Covered;
            }
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = (int) 2166136261;
                hash = (hash * 16777619) ^ Line.GetHashCode();
                hash = (hash * 16777619) ^ EndLine.GetHashCode();
                return (hash * 16777619) ^ Covered.GetHashCode();
            }
        }
    }
}
