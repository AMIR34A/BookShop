namespace BookShop.Models.Repository
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        //IRepositoryBase<BookShopContext> repositoryBase;
        BookShopContext _context;
        IBooksRepository booksRepository;
        private bool disposedValue;
        public BookShopContext BookShopContext { get => _context; }

        public UnitOfWork(BookShopContext context)
        {
            _context = context;
        }

        public IRepositoryBase<TEntity> RepositoryBase<TEntity>() where TEntity : class
        {
            return new RepositoryBase<TEntity, BookShopContext>(_context);
        }

        public IBooksRepository BooksRepository 
        {
            get
            {
                if (booksRepository is null)
                    booksRepository = new BooksRepository(this);
                return booksRepository;
            }
        }

        //public IRepositoryBase<BookShopContext> RepositoryBase
        //{
        //    get
        //    {
        //        if (repositoryBase is null)
        //            repositoryBase = new RepositoryBase<TEntity, BookShopContext>(_context);
        //        return repositoryBase;
        //    }
        //}
        public async Task SaveAsync() => await _context.SaveChangesAsync();


        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _context.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
