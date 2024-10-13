using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShoppingListApp.Models;
using ShoppingListsApp.Data;

namespace ShoppingListsApp.Controllers
{
    ///<summary>
    ///Controller for managing categories in the shopping list application.
    ///This controller provides actions for creating, reading, updating, and deleting categories,
    ///as well as managing the association of products with categories.
    ///</summary>
    [Authorize]
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        ///<summary>
        ///Initializes a new instance of the <see cref="CategoriesController"/> class.
        ///</summary>
        ///<param name="context">The database context used for category management.</param>
        public CategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Categories
        ///<summary>
        ///Displays a list of all categories.
        ///</summary>
        ///<returns>A view containing a list of categories.</returns>
        [Authorize]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Categories.ToListAsync());
        }

        // GET: Categories/Details/5
        ///<summary>
        ///Displays the details of a specific category.
        ///</summary>
        ///<param name="id">The ID of the category to display.</param>
        ///<returns>A view containing the category details or a not found result if the category does not exist.</returns>
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .Include(c => c.Products)
                .FirstOrDefaultAsync(m => m.CategoryId == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // GET: Categories/Create
        ///<summary>
        ///Displays the form for creating a new category.
        ///This action requires Admin role.
        ///</summary>
        ///<returns>A view for creating a category.</returns>
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        ///<summary>
        ///Handles the creation of a new category.
        ///This action requires Admin role.
        ///</summary>
        ///<param name="category">The category object to create.</param>
        ///<returns>A redirect to the index view on success, or the create view on failure.</returns>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CategoryId,Name")] Category category)
        {
            if (ModelState.IsValid)
            {
                var existingCategory = await _context.Categories.FirstOrDefaultAsync(c => c.Name.ToLower() == category.Name.ToLower());

                if (existingCategory != null)
                {
                    ModelState.AddModelError("Name", "A category with the same name already exists.");
                }
                else
                {
                    _context.Add(category);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }  
            }
            return View(category);
        }

        // GET: Categories/Edit/5
        ///<summary>
        ///Displays the form for editing a specific category.
        ///This action requires Admin role.
        ///</summary>
        ///<param name="id">The ID of the category to edit.</param>
        ///<returns>A view for editing the category or a not found result if the category does not exist.</returns>
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            /* var category = await _context.Categories.FindAsync(id);*/
            var category = await _context.Categories.Include(c => c.Products).FirstOrDefaultAsync(c => c.CategoryId == id);

            if (category == null)
            {
                return NotFound();
            }

            // Get all products that are not in the current category
            var availableProducts = await _context.Products
                                                    .Where(p => p.CategoryId != id) 
                                                    .ToListAsync();

            ViewBag.AvailableProducts = availableProducts; // Pass the list of available products to the view
            ViewBag.ProductsInCategory = category.Products;

            return View(category);
        }

        ///<summary>
        ///Moves a product to a specified category.
        ///This action requires Admin role.
        ///</summary>
        ///<param name="productId">The ID of the product to move.</param>
        ///<param name="categoryId">The ID of the category to move the product to.</param>
        ///<returns>A JSON result indicating success or failure.</returns>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> MoveProductToCategory(int productId, int categoryId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                return Json(new { success = false, message = "Product not found" });
            }

            // Update the product's category
            product.CategoryId = categoryId;
            _context.Update(product);
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        // POST: Categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        ///<summary>
        ///Updates the details of a specific category.
        ///This action requires Admin role.
        ///</summary>
        ///<param name="id">The ID of the category to update.</param>
        ///<param name="category">The updated category object.</param>
        ///<returns>A redirect to the index view on success, or the edit view on failure.</returns>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CategoryId,Name")] Category category)
        {
            if (id != category.CategoryId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Check if a category with the same name already exists, excluding the current category
                    var duplicateCategory = await _context.Categories.FirstOrDefaultAsync(c => c.Name.ToLower() == category.Name.ToLower() && c.CategoryId != id);

                    if (duplicateCategory != null)
                    {
                        ModelState.AddModelError("Name", "A category with the same name already exists.");
                        //return View(category);
                        // Reload products and reassign ViewBag data
                        var categoryWithProducts = await _context.Categories
                            .Include(c => c.Products)
                            .FirstOrDefaultAsync(c => c.CategoryId == id);

                        ViewBag.AvailableProducts = await _context.Products
                            .Where(p => p.CategoryId != id)
                            .ToListAsync();

                        ViewBag.ProductsInCategory = categoryWithProducts?.Products;

                        return View(categoryWithProducts); // Pass the full category model
                    }

                    _context.Update(category);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.CategoryId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            /* return View(category);*/
            var originalCategory = await _context.Categories
                 .Include(c => c.Products)
                 .FirstOrDefaultAsync(c => c.CategoryId == id);

            ViewBag.AvailableProducts = await _context.Products
                .Where(p => p.CategoryId != id)
                .ToListAsync();

            ViewBag.ProductsInCategory = originalCategory?.Products;

            return View(originalCategory);
        }

        // GET: Categories/Delete/5
        ///<summary>
        ///Displays the confirmation page for deleting a specific category.
        ///This action requires Admin role.
        ///</summary>
        ///<param name="id">The ID of the category to delete.</param>
        ///<returns>A view for confirming the deletion or a not found result if the category does not exist.</returns>
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.CategoryId == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Categories/Delete/5
        ///<summary>
        ///Confirms the deletion of a specific category.
        ///Delete all products in the category's product list
        ///This action requires Admin role.
        ///</summary>
        ///<param name="id">The ID of the category to delete.</param>
        ///<returns>A redirect to the index view after deletion.</returns>
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _context.Categories.Include(c => c.Products).FirstOrDefaultAsync(c => c.CategoryId == id);

            if (category == null)
            {
                return NotFound();
            }

            // Collect product names from the category to delete from shopping lists
            var productNames = category.Products.Select(p => p.Name).ToList();

            // Delete all products in the category's product list
            if (category.Products != null && category.Products.Any())
            {
                _context.Products.RemoveRange(category.Products);
            }

            _context.Categories.Remove(category);

            var shoppingLists = await _context.ShoppingLists.ToListAsync();

            foreach (var shoppingList in shoppingLists)
            {
                // Remove products that belong to the deleted category from shopping list items
                shoppingList.Items = shoppingList.Items
                    .Where(item => !productNames.Contains(item.Replace("__ACQUIRED__", ""))) // Handle items with __ACQUIRED__
                    .ToList();

                // Update the shopping list
                _context.ShoppingLists.Update(shoppingList);
            }


            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        ///<summary>
        ///Checks if a category exists in the database.
        ///</summary>
        ///<param name="id">The ID of the category to check.</param>
        ///<returns>True if the category exists; otherwise, false.</returns>
        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.CategoryId == id);
        }
    }
}
