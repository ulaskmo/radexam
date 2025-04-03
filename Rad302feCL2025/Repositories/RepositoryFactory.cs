using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rad302feCL2025.Repositories
{
    public class RepositoryFactory : IRepositoryFactory
    {
        private readonly MovieContext _context;

        public RepositoryFactory(MovieContext context)
        {
            _context = context;
        }

        public IMovieRepository CreateMovieRepository()
        {
            return new MovieRepository(_context);
        }
    }
}
