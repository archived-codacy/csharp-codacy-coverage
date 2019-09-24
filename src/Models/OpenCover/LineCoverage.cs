namespace Codacy.CSharpCoverage.Models.OpenCover
{
    /// <summary>
    ///     Line coverage.
    ///     This contains the line number and the visit count of that line
    ///     in terms of coverage. This is based OpenCover file format.
    /// </summary>
    public sealed class LineCoverage
    {
        /// <summary>
        ///     Parsed line number
        /// </summary>
        public int LineNumber { get; set; }

        /// <summary>
        ///     Parsed coverage visit count
        /// </summary>
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
