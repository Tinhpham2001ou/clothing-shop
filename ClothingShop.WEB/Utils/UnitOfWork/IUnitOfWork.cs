namespace ClothingShop.WEB.Utils.UnitOfWork
{
    public interface IUnitOfWork<TContext>
    {
        Task CompleteAsync();
    }
}
