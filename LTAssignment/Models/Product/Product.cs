using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace LTAssignment.Models.Product
{
     public class ProductMaster
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Product Name is required")]

        public string? ProductName { get; set; }
        public string? Description { get; set; }
        [Required(ErrorMessage = "Price is required")]

        public decimal Price { get; set; }

        [Required(ErrorMessage = "Category is required")]
        public int CategoryID { get; set; }
        [Required(ErrorMessage = "Category is required")]
        public string? CategoryName { get; set; }
        [Required(ErrorMessage = "Quantity is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }
        public int CreatedBy { get; set; }
        public List<IFormFile> ?ProductImg { get; set; }
       public string? FinalDTFile { get; set; }
        public string? UserType { get; set; }
        public string? UserName { get; set; }
        public string? ProductImages { get; set; }

    }

    public class ProductImage
    {
        public List<IFormFile>? ProductImg { get; set; }
    }
    public class ProductImageD
    {
        public string ProductImages { get; set; }
    }
}
