namespace BookShop.Models.Repository
{
    public interface IUnitOfWork
    {
        IRepositoryBase<TEntity> RepositoryBase<TEntity>() where TEntity : class;
        Task SaveAsync();
        void Dispose();
        BookShopContext BookShopContext { get; }
        IBooksRepository BooksRepository { get; }
    }
}