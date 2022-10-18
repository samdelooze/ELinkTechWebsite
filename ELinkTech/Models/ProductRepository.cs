using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ELinkTech.Models
{
    public class ProductRepository: IPostRepository
    {

        private readonly DataContext db;
        public ProductRepository(DataContext db)
        {
            this.db = db;
        }

        public IEnumerable<Product> listPost()
        {
            return db.products.ToList<Product>();
        }
    }
}
