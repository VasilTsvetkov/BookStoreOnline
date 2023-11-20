using BookStoreOnline.Data.Data;
using BookStoreOnline.Data.Repositories.IRepositories;
using BookStoreOnline.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BookStoreOnline.Data.Repositories
{
    public class ShoppingCartRepository : GenericRepository<ShoppingCart>, IShoppingCartRepository
    {
        private ApplicationDbContext db;

        public ShoppingCartRepository(ApplicationDbContext db)
            : base(db)
        {
            this.db = db;
        }

        public void Update(ShoppingCart shoppingCart)
        {
            db.ShoppingCarts.Update(shoppingCart);
        }
    }
}
