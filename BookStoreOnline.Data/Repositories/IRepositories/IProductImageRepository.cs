﻿using BookStoreOnline.Data.Repositories.IRepository;
using BookStoreOnline.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BookStoreOnline.Data.Repositories.IRepositories
{
	public interface IProductImageRepository : IGenericRepository<ProductImage>
	{
		void Update(ProductImage productImage);
	}
}
