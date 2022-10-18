using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ELinkTech.Models
{
    public interface IPostRepository
    {
        public IEnumerable<Product>listPost();
    }
}
