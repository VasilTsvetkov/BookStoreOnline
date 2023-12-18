using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace BookStoreOnline.Models
{
	public class Product
	{
        public int Id { get; set; }

		[Required]
		public string Title { get; set; }

		public string Description { get; set; }

		[Required]
		public string ISBN { get; set; }

		[Required]
		public string Author { get; set; }

		[Required]
		[DisplayName("List Price")]
		[Range(3, 1000)]
		public decimal ListPrice { get; set; }

		[Required]
		[DisplayName("Price for 1-50 copies")]
		[Range(3, 1000)]
		public decimal Price { get; set; }

		[Required]
		[DisplayName("Price for 51-100 copies")]
		[Range(3, 1000)]
		public decimal Price51To100 { get; set; }

		[Required]
		[DisplayName("Price 100+ copies")]
		[Range(3, 1000)]
		public decimal PriceOver100 { get; set; }

		[DisplayName("Category")]
        public int CategoryId { get; set; }

		[ForeignKey(nameof(CategoryId))]
		[ValidateNever]
        public Category Category { get; set; }

		[ValidateNever]
		public List<ProductImage> ProductImages { get; set; }
    }
}
