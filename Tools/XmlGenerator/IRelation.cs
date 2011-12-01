namespace EVEMon.XmlGenerator
{
    /// <summary>
    /// Implementors support two components, a left hand side and a right hand side that are related.
    /// </summary>
    public interface IRelation
    {
        long Left { get; }
        long Right { get; }
    }
}