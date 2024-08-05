using LTAssignment.Models;
using LTAssignment.Models.Account;
using LTAssignment.Models.Category;
using LTAssignment.Models.DBConnection;
using LTAssignment.Models.Product;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;
using System.Security.Claims;
using static System.Net.Mime.MediaTypeNames;

namespace LTAssignment.Controllers
{
    public class ProductsController : Controller
    {
        private readonly Common _common;
        public ProductsController(Common common)
        {
            _common = common;
        }
       
        public IActionResult GetProductList(ProductMaster  productMaster)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.PrimarySid)?.Value;
            var userType = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            if (!string.IsNullOrEmpty(userId) && int.TryParse(userId, out int parsedUserId))
            {
                productMaster.CreatedBy = parsedUserId;
                productMaster.UserType = userType;
            }

            var (dataSet, success, message) = _common.ExecuteStoreProcedure(productMaster, "sp_ProductMaster", 41);
            if (!success)
            {
                ViewBag.ErrorMessage = message;
                return View("Error");
            }

            

            var productList = new List<ProductMaster>();
            if (dataSet.Tables.Count > 0)
            {
                var productTable = dataSet.Tables[0];
                foreach (DataRow row in productTable.Rows)
                {
                      productMaster = new ProductMaster
                    {
                        ID = Convert.ToInt32(row["ID"]),
                          ProductName = row["ProductName"].ToString(),
                        Description = row["Description"].ToString(),
                        Price = Convert.ToDecimal(row["Price"]),
                        CategoryID = Convert.ToInt32(row["CategoryID"]),
                        Quantity = Convert.ToInt32(row["Quantity"]),
                        CategoryName = row["CategoryName"].ToString()
                          //CreatedBy = Convert.ToInt32(row["CreatedBy"])
                      };
                    productList.Add(productMaster);
                }
            }

            return View(productList);
        }
         
        public IActionResult Create()
        {
            CategoryMaster categoryMaster = new();
            var (dataSet, success, message) = _common.ExecuteStoreProcedure(categoryMaster, "sp_ProductMaster", 42);
            if (!success)
            {
                ViewBag.ErrorMessage = message;
                return View("Error");
            }

            var categoryList = new List<CategoryMaster>();
            if (dataSet.Tables.Count > 0)
            {
                var productTable = dataSet.Tables[0];
                foreach (DataRow row in productTable.Rows)
                {
                    categoryMaster = new CategoryMaster
                    {
                        ID = Convert.ToInt32(row["ID"]),
                        CategoryName = row["CategoryName"].ToString()
                    };
                    categoryList.Add(categoryMaster);
                }
            }

            // Create the SelectList from the category list
            
            ViewBag.CategorySelectList = new SelectList(categoryList, "ID", "CategoryName");
            return View();
        }

        [HttpPost]
        public IActionResult Create(ProductMaster productMaster)
        {
             
                if (productMaster.ProductImg != null && productMaster.ProductImg.Count > 0)
                {
                    string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/ProductImages");
                    string setPath = "";
                    string DTFile = "";
                    
                    // Create folder if not exist
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);

                    foreach (var file in productMaster.ProductImg)
                    {
                        if (file.Length > 0)
                        {
                            // Get file extension
                            FileInfo fileInfo = new FileInfo(file.FileName);
                            string fileName = Guid.NewGuid().ToString() + fileInfo.Extension;
                            setPath = "wwwroot/ProductImages/" + fileName;
                            string fileNameWithPath = Path.Combine(path, fileName);
                            DTFile += "{\"filepath\":\""+setPath+"\"},";

                            using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                            {
                                file.CopyTo(stream);
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(DTFile))
                    {
                        DTFile = DTFile.TrimEnd(',');
                    }

                    // Construct the final JSON array
                    productMaster.FinalDTFile = "[" + DTFile + "]";
                 productMaster.ProductImg = null;
                    var (dataSet, success, message) = _common.ExecuteStoreProcedure(productMaster, "sp_ProductMaster", 11);
                    if (!success)
                    {
                        ViewBag.ErrorMessage = message;
                        return View("Error");
                    }

                    ViewBag.SuccessMessage = "File(s) uploaded successfully";
                    return RedirectToAction("GetProductList", "Products");
                }
                else
                {
                var (dataSet, success, message) = _common.ExecuteStoreProcedure(productMaster, "sp_ProductMaster", 11);
                if (!success)
                {
                    ViewBag.ErrorMessage = message;
                    return View("Error");
                }
            }
            
            return RedirectToAction("GetProductList", "Products");
        }
        public IActionResult Delete(ProductMaster productMaster)
        {
            var (dataSet, success, message) = _common.ExecuteStoreProcedure(productMaster, "sp_ProductMaster", 31);
            if (!success)
            {
                ViewBag.ErrorMessage = message;
                return View("Error");
            }
            return RedirectToAction("GetProductList", "Products");
        }


        public IActionResult Details(int ID)
        {
            var prm = new { ID = ID };
            var (dataSet, success, message) = _common.ExecuteStoreProcedure(prm, "sp_ProductMaster", 43);
            if (!success)
            {
                ViewBag.ErrorMessage = message;
                return View("Error");
            }
            var productMaster = dataSet.Tables[0].AsEnumerable().Select(row => new ProductMaster
            {
                ID = Convert.ToInt32(row["ID"]),
                ProductName = row["ProductName"].ToString(),
                Description = row["Description"].ToString(),
                Price = Convert.ToDecimal(row["Price"]),
                CategoryID = Convert.ToInt32(row["CategoryID"]),
                Quantity = Convert.ToInt32(row["Quantity"]),
                CategoryName = row["CategoryName"].ToString(),
                UserName = row["UserName"].ToString(),
                ProductImages = row["ProductImages"].ToString()
            }).FirstOrDefault();

            if (productMaster == null)
            {
                return NotFound();
            }


            return View(productMaster);

        }
        public IActionResult Edit(int ID)
        {
            var prm = new { ID = ID };
            var (dataSet, success, message) = _common.ExecuteStoreProcedure(prm, "sp_ProductMaster", 43);
            if (!success)
            {
                ViewBag.ErrorMessage = message;
                return View("Error");
            }
            var productMaster = dataSet.Tables[0].AsEnumerable().Select(row => new ProductMaster
            {
                ID = Convert.ToInt32(row["ID"]),
                ProductName = row["ProductName"].ToString(),
                Description = row["Description"].ToString(),
                Price = Convert.ToDecimal(row["Price"]),
                CategoryID = Convert.ToInt32(row["CategoryID"]),
                Quantity = Convert.ToInt32(row["Quantity"]),
                CategoryName = row["CategoryName"].ToString(),
                ProductImages = row["ProductImages"].ToString()

            }).FirstOrDefault();

            if (productMaster == null)
            {
                return NotFound();
            }
            CategoryMaster categoryMaster = new();
            var (dataSet1, success1, message1) = _common.ExecuteStoreProcedure(categoryMaster, "sp_ProductMaster", 42);
            if (!success1)
            {
                ViewBag.ErrorMessage = message1;
                return View("Error");
            }

            var categoryList = new List<CategoryMaster>();
            if (dataSet1.Tables.Count > 0)
            {
                var productTable = dataSet1.Tables[0];
                foreach (DataRow row in productTable.Rows)
                {
                    categoryMaster = new CategoryMaster
                    {
                        ID = Convert.ToInt32(row["ID"]),
                        CategoryName = row["CategoryName"].ToString()
                    };
                    categoryList.Add(categoryMaster);
                }
            }


            ViewBag.CategorySelectList = new SelectList(categoryList, "ID", "CategoryName");
            return View(productMaster);

        }

        public IActionResult UpdateProduct(ProductMaster productMaster)
        { 
            var (dataSet, success, message) = _common.ExecuteStoreProcedure(productMaster, "sp_ProductMaster", 21);
            if (!success)
            {
                ViewBag.ErrorMessage = message;
                return View("Error");
            }
             
            return RedirectToAction("GetProductList", "Products");
        }
    }
}
