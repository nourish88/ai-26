namespace Juga.Abstractions.Data.Entities
{
    /// <summary>
    /// Lookup entityleri için base class
    /// </summary>
    public abstract class LookUpEntityBase : ILookUpEntity
    {
        public long Id { get; set; }
        public string Description { get; set; }
    }
}