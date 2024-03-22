namespace Levers
{
    /// <summary>
    /// Specifies how an arc is filled/stroked.
    /// </summary>
    public enum ArcDrawMode
    {
        /// <summary>
        /// Center - Pie segment (default)
        /// </summary>
        Center,
        /// <summary>
        /// Open - Semi-circle
        /// </summary>
        Open,
        /// <summary>
        /// Chord - Closed semi-circle
        /// </summary>
        Chord,
        /// <summary>
        /// Pie - Closed pie segment
        /// </summary>
        Pie,
    }
}
