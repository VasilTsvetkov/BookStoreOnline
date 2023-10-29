using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BookStoreOnlineWeb.Models
{
	public class Category
	{
		public int Id { get; set; }

		[Required]
		[DisplayName("Category Name")]
		[MaxLength(40)]
		[MinLength(3)]
		public string Name { get; set; }

		[DisplayName("Display Order")]
		[Range(1, 100, ErrorMessage = "The field Display Order must be in the range 1-100.")]
		public int DisplayOrder { get; set; }
	}
}
