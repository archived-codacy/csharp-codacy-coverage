namespace Codacy.CSharpCoverage.Models.DotCover
{
    /// <summary>
    ///     Line Coverage.
    ///     This represents a line coverage parsed from
    ///     DotCover format.
    /// </summary>
    public sealed class LineCoverage
    {
        /// <summary>
        ///     Parsed start line number
        /// </summary>
        public int Line { get; set; }

        /// <summary>
        ///     Parsed end line number
        /// </summary>
        public int EndLine { get; set; }

        /// <summary>
        ///     Parsed boolean of the coverage hit.
        /// </summary>
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
