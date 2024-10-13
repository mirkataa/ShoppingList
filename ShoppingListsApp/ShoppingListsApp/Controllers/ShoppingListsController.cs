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
    ///Controller for managing shopping lists for users.
    ///</summary>
    public class ShoppingListsController : Controller
    {
        private readonly ApplicationDbContext _context;

        ///<summary>
        ///Initializes a new instance of the <see cref="ShoppingListsController"/> class.
        ///</summary>
        ///<param name="context">The database context used for shopping list management.</param>
        public ShoppingListsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ShoppingLists
        ///<summary>
        ///Displays a list of shopping lists for the logged-in user.
        ///</summary>
        ///<returns>A view containing the user's shopping lists.</returns>
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var userName = User.Identity.Name; // Get the logged-in user's username
            var shoppingLists = await _context.ShoppingLists
                .Where(s => s.UserName == userName)  // Filter by username
                .ToListAsync();
            return View(shoppingLists);
            /*return View(await _context.ShoppingLists.ToListAsync());*/
        }

        // GET: ShoppingLists/Details/5
        ///<summary>
        ///Displays the details of a specific shopping list.
        /// If the list is not found or does not belong to the user, returns a not found result.
        ///</summary>
        ///<param name="id">The ID of the shopping list to display.</param>
        ///<returns>A view containing the shopping list details or a not found result.</returns>
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userName = User.Identity.Name;
            var shoppingListModel = await _context.ShoppingLists
                .FirstOrDefaultAsync(m => m.ShoppingListId == id && m.UserName == userName);
            if (shoppingListModel == null)
            {
                return NotFound();
            }

            return View(shoppingListModel);
        }

        ///<summary>
        ///Updates the acquisition status of an item in a shopping list.
        ///</summary>
        ///<param name="id">The ID of the shopping list.</param>
        ///<param name="item">The name of the item to update.</param>
        ///<param name="isAcquired">True if the item is acquired; otherwise, false.</param>
        ///<returns>An OK result on success or a not found result if the list does not exist.</returns>
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateAcquiredStatus(int id, string item, bool isAcquired)
        {
            var shoppingListModel = await _context.ShoppingLists
                .FirstOrDefaultAsync(m => m.ShoppingListId == id);

            if (shoppingListModel == null)
            {
                return NotFound();
            }

            // Update the acquired status of the item
            for (int i = 0; i < shoppingListModel.Items.Count; i++)
            {
                if (shoppingListModel.Items[i] == item || shoppingListModel.Items[i] == "__ACQUIRED__" + item)
                {
                    if (isAcquired)
                    {
                        shoppingListModel.Items[i] = "__ACQUIRED__" + item;  // Mark as acquired
                    }
                    else
                    {
                        shoppingListModel.Items[i] = item;  // Remove acquired marker
                    }
                    break;
                }
            }

            // Save the changes
            _context.Update(shoppingListModel);
            await _context.SaveChangesAsync();

            return Ok();  // Return success
        }

        ///<summary>
        ///Deletes an item from a shopping list.
        ///</summary>
        ///<param name="id">The ID of the shopping list.</param>
        ///<param name="item">The name of the item to delete.</param>
        ///<returns>An OK result on success or a not found result if the list does not exist.</returns>
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteItemDetails(int id, string item)
        {
            var shoppingListModel = await _context.ShoppingLists
                .FirstOrDefaultAsync(m => m.ShoppingListId == id);

            if (shoppingListModel == null)
            {
                return NotFound();
            }

            // Remove the item from the list
            shoppingListModel.Items.Remove(item);
            shoppingListModel.Items.Remove("__ACQUIRED__" + item);  // Also remove if it's acquired

            // Save the changes
            _context.Update(shoppingListModel);
            await _context.SaveChangesAsync();

            return Ok();  // Return success
        }

        // GET: ShoppingLists/Create
        ///<summary>
        ///Displays the form for creating a new shopping list.
        ///</summary>
        ///<returns>A view for creating a shopping list.</returns>
        [Authorize]
        public IActionResult Create()
        {
            var shoppingListModel = new ShoppingListModel
            {
                UserName = User.Identity.Name // Set the username to the logged-in user
            };

            ViewBag.Categories = _context.Categories.Include(c => c.Products).ToList();
            return View(shoppingListModel);
        }

        // POST: ShoppingLists/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        ///<summary>
        ///Displays the form for creating a new shopping list.
        ///</summary>
        ///<returns>A view for creating a shopping list.</returns>
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ShoppingListId,UserName,Name,Items")] ShoppingListModel shoppingListModel, int[] selectedProducts)
        {
            shoppingListModel.UserName = User.Identity.Name;

            var selectedProductNames = _context.Products
           .Where(p => selectedProducts.Contains(p.ProductId))
           .Select(p => p.Name)
           .ToList();

            shoppingListModel.Items = selectedProductNames;

            if (ModelState.IsValid)
            {
                _context.Add(shoppingListModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            /*return View(shoppingListModel);*/

            ViewBag.Categories = _context.Categories.Include(c => c.Products).ToList();
            return View(shoppingListModel);
        }

        // GET: ShoppingLists/Edit/5
        ///<summary>
        ///Displays the form for editing a specific shopping list.
        ///Returns a not found result if the list does not exist or does not belong to the user.
        ///</summary>
        ///<param name="id">The ID of the shopping list to edit.</param>
        ///<returns>A view for editing the shopping list or a not found result.</returns>
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userName = User.Identity.Name;
            var shoppingListModel = await _context.ShoppingLists.FirstOrDefaultAsync(m => m.ShoppingListId == id && m.UserName == userName);
            if (shoppingListModel == null)
            {
                return NotFound();
            }

            var categories = await _context.Categories.Include(c => c.Products).ToListAsync();
            ViewBag.Categories = categories;

            return View(shoppingListModel);
        }

        ///<summary>
        ///Adds an item to the shopping list.
        ///</summary>
        ///<param name="id">The ID of the shopping list.</param>
        ///<param name="item">The name of the item to add.</param>
        ///<returns>An OK result on success or a not found result if the list does not exist.</returns>
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddItem(int id, string item)
        {
            var shoppingListModel = await _context.ShoppingLists
                .FirstOrDefaultAsync(m => m.ShoppingListId == id);

            if (shoppingListModel == null)
            {
                return NotFound();
            }

            // Add the item to the shopping list
            if (!shoppingListModel.Items.Contains(item))
            {
                shoppingListModel.Items.Add(item);
                _context.Update(shoppingListModel);
                await _context.SaveChangesAsync();
            }

            return Ok();  // Return success to AJAX call
        }

        ///<summary>
        ///Deletes an item from the shopping list during edit.
        ///</summary>
        ///<param name="id">The ID of the shopping list.</param>
        ///<param name="item">The name of the item to delete.</param>
        ///<returns>An OK result on success or a not found result if the list does not exist.</returns>
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteItemEdit(int id, string item)
        {
            var shoppingListModel = await _context.ShoppingLists
                .FirstOrDefaultAsync(m => m.ShoppingListId == id);

            if (shoppingListModel == null)
            {
                return NotFound();
            }

            // Remove the item from the shopping list
            shoppingListModel.Items.Remove(item);
            _context.Update(shoppingListModel);
            await _context.SaveChangesAsync();

            return Ok();  // Return success to AJAX call
        }

        // POST: ShoppingLists/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        ///<summary>
        ///Edits an existing shopping list. 
        ///Updates the shopping list name if the model state is valid.
        ///</summary>
        ///<param name="id">The ID of the shopping list to edit.</param>
        ///<param name="shoppingListModel">The shopping list model with updated information.</param>
        ///<returns>A redirect to the edit view on success, or the edit view with validation errors on failure.</returns>
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ShoppingListId,UserName,Name")] ShoppingListModel shoppingListModel)
        {
            if (id != shoppingListModel.ShoppingListId)
            {
                return NotFound();
            }

            shoppingListModel.UserName = User.Identity.Name;

            if (ModelState.IsValid)
            {
                try
                {
                    // Load the existing shopping list from the database
                    var existingShoppingList = await _context.ShoppingLists.FirstOrDefaultAsync(m => m.ShoppingListId == id);

                    if (existingShoppingList == null)
                    {
                        return NotFound();
                    }

                    // Update only the name (and other non-Items fields if necessary)
                    existingShoppingList.Name = shoppingListModel.Name;

                    _context.Update(existingShoppingList);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ShoppingListModelExists(shoppingListModel.ShoppingListId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Edit));
            }
            return View(shoppingListModel);
        }

        // GET: ShoppingLists/Delete/5
        ///<summary>
        ///Displays the confirmation view for deleting a specific shopping list.
        ///Returns a not found result if the list does not exist or does not belong to the user.
        ///</summary>
        ///<param name="id">The ID of the shopping list to delete.</param>
        ///<returns>A view for confirming deletion or a not found result.</returns>
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userName = User.Identity.Name;
            var shoppingListModel = await _context.ShoppingLists.FirstOrDefaultAsync(m => m.ShoppingListId == id && m.UserName == userName);
            if (shoppingListModel == null)
            {
                return NotFound();
            }

            return View(shoppingListModel);
        }

        // POST: ShoppingLists/Delete/5
        ///<summary>
        ///Confirms the deletion of a shopping list.
        ///Removes the shopping list from the database if it exists and belongs to the user.
        ///</summary>
        ///<param name="id">The ID of the shopping list to delete.</param>
        ///<returns>A redirect to the index view on success or a forbidden result if the list does not belong to the user.</returns>
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userName = User.Identity.Name;
            //var shoppingListModel = await _context.ShoppingLists.FindAsync(id);
            var shoppingListModel = await _context.ShoppingLists.FirstOrDefaultAsync(m => m.ShoppingListId == id && m.UserName == userName);

            if (shoppingListModel == null)
            {
                return Forbid();

            }

            _context.ShoppingLists.Remove(shoppingListModel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        ///<summary>
        ///Checks if a shopping list exists in the database.
        ///</summary>
        ///<param name="id">The ID of the shopping list to check.</param>
        ///<returns>True if the shopping list exists; otherwise, false.</returns>
        private bool ShoppingListModelExists(int id)
        {
            return _context.ShoppingLists.Any(e => e.ShoppingListId == id);
        }
    }
}
