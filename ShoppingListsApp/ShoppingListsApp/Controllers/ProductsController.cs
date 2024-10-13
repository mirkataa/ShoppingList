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
    [Authorize]
    ///<summary>
    ///Controller for managing products in the shopping list application.
    ///This controller provides actions for creating, reading, updating, and deleting products.
    ///</summary>
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;

        ///<summary>
        ///Initializes a new instance of the <see cref="ProductsController"/> class.
        ///</summary>
        ///<param name="context">The database context used for product management.</param>
        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Products
        ///<summary>
        ///Displays a list of all products.
        ///</summary>
        ///<returns>A view containing a list of products.</returns>
        [Authorize]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Products.ToListAsync());
        }

        // GET: Products/Details/5
        ///<summary>
        ///Displays the details of a specific product.
        ///</summary>
        ///<param name="id">The ID of the product to display.</param>
        ///<returns>A view containing the product details or a not found result if the product does not exist.</returns>
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.FirstOrDefaultAsync(c => c.CategoryId == product.CategoryId);

            ViewData["CategoryName"] = category != null ? category.Name : "Unknown";

            return View(product);
        }

        // GET: Products/Create
        ///<summary>
        ///Displays the form for creating a new product.
        ///This action requires Admin role.
        ///</summary>
        ///<returns>A view for creating a product.</returns>
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewBag.CategoryId = new SelectList(_context.Categories, "CategoryId", "Name");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        ///<summary>
        ///Handles the creation of a new product.
        ///This action requires Admin role.
        ///Validates input and checks for duplicates.
        ///</summary>
        ///<param name="product">The product object to create.</param>
        ///<returns>A redirect to the index view on success, or the create view on failure.</returns>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductId,Name,CategoryId")] Product product)
        {
            if (ModelState.IsValid)
            {

                var existingProduct = await _context.Products.FirstOrDefaultAsync(p => p.Name.ToLower() == product.Name.ToLower());

                if (existingProduct != null)
                {
                    ModelState.AddModelError("Name", "A product with the same name already exists.");
                }
                else
                {
                    // Find the selected category
                    var category = await _context.Categories.Include(c => c.Products).FirstOrDefaultAsync(c => c.CategoryId == product.CategoryId);

                    if (category == null)
                    {
                        ModelState.AddModelError("CategoryId", "Category not found.");
                        // Reload the category list for the dropdown
                        ViewBag.CategoryId = new SelectList(_context.Categories, "CategoryId", "Name", product.CategoryId);
                        return View(product);
                    }

                    category.Products.Add(product);
                    //  _context.Update(category);
                    _context.Add(product);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            ViewBag.CategoryId = new SelectList(_context.Categories, "CategoryId", "Name", product.CategoryId);
            return View(product);
        }

        // GET: Products/Edit/5
        ///<summary>
        ///Displays the form for editing a specific product.
        ///This action requires Admin role.
        ///</summary>
        ///<param name="id">The ID of the product to edit.</param>
        ///<returns>A view for editing the product or a not found result if the product does not exist.</returns>
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            ViewBag.CategoryId = new SelectList(_context.Categories, "CategoryId", "Name", product.CategoryId);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        ///<summary>
        ///Handles the editing of a specific product.
        ///Validates input and checks for duplicates.
        ///This action requires Admin role.
        ///</summary>
        ///<param name="id">The ID of the product to edit.</param>
        ///<param name="product">The updated product object.</param>
        ///<returns>A redirect to the index view on success, or the edit view on failure.</returns>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,Name,CategoryId")] Product product)
        {
            if (id != product.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Find the original product with its current category
                    var existingProduct = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == id);
                    if (existingProduct == null)
                    {
                        return NotFound();
                    }

                    // Check if a product with the same name already exists, excluding the current product
                    var duplicateProduct = await _context.Products.FirstOrDefaultAsync(p => p.Name.ToLower() == product.Name.ToLower() && p.ProductId != id);

                    if (duplicateProduct != null)
                    {
                        ModelState.AddModelError("Name", "A product with the same name already exists.");
                        ViewBag.CategoryId = new SelectList(_context.Categories, "CategoryId", "Name", product.CategoryId);
                        return View(product);
                    }

                    // If the category has changed
                    if (existingProduct.CategoryId != product.CategoryId)
                    {
                        var prevCategory = await _context.Categories.FirstOrDefaultAsync(c => c.CategoryId == existingProduct.CategoryId);
                        prevCategory.Products.Remove(existingProduct);

                        var newCategory = await _context.Categories.Include(c => c.Products).FirstOrDefaultAsync(c => c.CategoryId == product.CategoryId);

                        if (newCategory == null)
                        {
                            ModelState.AddModelError("CategoryId", "Selected category not found.");
                            ViewBag.CategoryId = new SelectList(_context.Categories, "CategoryId", "Name", product.CategoryId);
                            return View(product);
                        }

                        newCategory.Products.Add(existingProduct);
                        // _context.Update(newCategory);

                        existingProduct.CategoryId = product.CategoryId;
                    }



                    // Update product name in shopping lists
                    var shoppingLists = await _context.ShoppingLists
                                        .Where(sl => sl.Items.Any(item => item == existingProduct.Name ||   item == "__ACQUIRED__" + existingProduct.Name))
                                        .ToListAsync();

                    foreach (var shoppingList in shoppingLists)
                    {
                        // Update each item's name
                        for (int i = 0; i < shoppingList.Items.Count; i++)
                        {
                            if (shoppingList.Items[i] == existingProduct.Name)
                            {
                                shoppingList.Items[i] = product.Name; // Update the name
                            }
                            else if (shoppingList.Items[i] == "__ACQUIRED__" + existingProduct.Name)
                            {
                                shoppingList.Items[i] = "__ACQUIRED__" + product.Name; // Update acquired item name
                            }
                        }
                    }

                    existingProduct.Name = product.Name;

                    //_context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductId))
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
            return View(product);
        }

        // GET: Products/Delete/5
        ///<summary>
        ///Displays the confirmation page for deleting a specific product.
        ///This action requires Admin role.
        ///</summary>
        ///<param name="id">The ID of the product to delete.</param>
        ///<returns>A view for confirming the deletion or a not found result if the product does not exist.</returns>
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        ///<summary>
        ///Handles the deletion of a specific product. Updates related shopping lists.
        ///This action requires Admin role.
        ///</summary>
        ///<param name="id">The ID of the product to delete.</param>
        ///<returns>A redirect to the index view after deletion.</returns>
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                if (product.CategoryId != 0) // Assuming CategoryId is non-zero for existing categories
                {
                    var category = await _context.Categories.Include(c => c.Products).FirstOrDefaultAsync(c => c.CategoryId == product.CategoryId);
                    if (category != null)
                    {
                        category.Products.Remove(product);
                        // _context.Update(category);
                    }
                }

                var productName = product.Name;
                _context.Products.Remove(product);

                // Fetch all shopping lists that might contain the product name
                var shoppingLists = await _context.ShoppingLists.ToListAsync();

                // Iterate through each shopping list and remove the product name from the Items list
                foreach (var shoppingList in shoppingLists)
                {
                    // Remove the product from the Items list (considering __ACQUIRED__ prefix)
                    shoppingList.Items = shoppingList.Items
                        .Where(item => !item.Equals(productName) && !item.Equals($"__ACQUIRED__{productName}"))
                        .ToList();

                    // Update the shopping list in the database
                    _context.ShoppingLists.Update(shoppingList);
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        ///<summary>
        ///Checks if a product exists by its ID.
        ///</summary>
        ///<param name="id">The ID of the product to check.</param>
        ///<returns>True if the product exists, otherwise false.</returns>
        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }
    }
}
