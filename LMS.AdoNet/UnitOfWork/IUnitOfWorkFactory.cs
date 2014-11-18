namespace RFD.FMS.AdoNet.UnitOfWork
{
    /// <summary>
    /// Create a unit of work based on a transaction definition.
    /// </summary>
    public interface IUnitOfWorkFactory
    {
        IUnitOfWork GetInstance(IUnitOfWorkDefinition definition);
    }
}