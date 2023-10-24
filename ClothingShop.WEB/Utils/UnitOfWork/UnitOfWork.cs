using ClothingShop.WEB.Models;

namespace ClothingShop.WEB.Utils.UnitOfWork
{
    public class UnitOfWork<TContext> : IUnitOfWork<TContext> where TContext : ClothingShopContext
    {
        private readonly TContext _context;
        public UnitOfWork(TContext context) => _context = context;

        public async Task CompleteAsync()
        {
            _context.SaveChanges();
        }
    }
}
