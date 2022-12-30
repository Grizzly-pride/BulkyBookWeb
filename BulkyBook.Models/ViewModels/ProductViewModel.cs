using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyBook.Models.ViewModels
{
	public sealed class ProductViewModel
	{
		public Product Product { get; set; }
		public IEnumerable<SelectListItem> CategoryList {get; set;}
		public IEnumerable<SelectListItem> CoverTypeList {get; set;}

	}
}
