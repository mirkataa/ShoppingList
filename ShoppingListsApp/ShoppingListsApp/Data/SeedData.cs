using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ShoppingListApp.Models;
using ShoppingListsApp.Data;

namespace ShoppingListsApp.Data
{
    ///<summary>
    ///Contains methods for seeding initial data into the application database.
    ///</summary>
    public static class SeedData
    {
        ///<summary>
        ///Initializes the database with default data, including an admin user and sample categories and products.
        ///</summary>
        ///<param name="serviceProvider">The service provider used to retrieve the application services.</param>
        ///<returns>A task that represents the asynchronous operation.</returns>
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

            // Create an admin user and assign the role
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var adminEmail = "admin@email.com";
            var adminUserName = "admina";
            var adminPassword = "Test1234_";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new IdentityUser { UserName = adminUserName, Email = adminEmail };
                var result = await userManager.CreateAsync(adminUser, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin"); // Assign Admin role
                }
            }

            // Seed Categories
            var categories = new List<Category>
            {
                new Category { Name = "Fruits" },
                new Category { Name = "Vegetables" },
                new Category { Name = "Dairy" },
                new Category { Name = "Meat" },
                new Category { Name = "Bakery" },
                new Category { Name = "Beverages" },
                new Category { Name = "Snacks" },
                new Category { Name = "Frozen Foods" },
                new Category { Name = "Condiments" },
                new Category { Name = "Seafood" },
                new Category { Name = "Grains" },
                new Category { Name = "Spices" },
                new Category { Name = "Nuts" },
                new Category { Name = "Legumes" },
                new Category { Name = "Herbs" }
            };

            if (!context.Categories.Any())
            {
                context.Categories.AddRange(categories);
                await context.SaveChangesAsync();
            }

            // Seed Products
            var products = new List<Product>
            {
                new Product { Name = "Apple", CategoryId = categories.First(c => c.Name == "Fruits").CategoryId },
                new Product { Name = "Banana", CategoryId = categories.First(c => c.Name == "Fruits").CategoryId },
                new Product { Name = "Carrot", CategoryId = categories.First(c => c.Name == "Vegetables").CategoryId },
                new Product { Name = "Broccoli", CategoryId = categories.First(c => c.Name == "Vegetables").CategoryId },
                new Product { Name = "Milk", CategoryId = categories.First(c => c.Name == "Dairy").CategoryId },
                new Product { Name = "Cheese", CategoryId = categories.First(c => c.Name == "Dairy").CategoryId },
                new Product { Name = "Chicken", CategoryId = categories.First(c => c.Name == "Meat").CategoryId },
                new Product { Name = "Venison", CategoryId = categories.First(c => c.Name == "Meat").CategoryId},
                new Product { Name = "Bread", CategoryId = categories.First(c => c.Name == "Bakery").CategoryId },
                new Product { Name = "Kalakukko", CategoryId = categories.First(c => c.Name == "Bakery").CategoryId },
                new Product { Name = "Coffee", CategoryId = categories.First(c => c.Name == "Beverages").CategoryId },
                new Product { Name = "Water", CategoryId = categories.First(c => c.Name == "Beverages").CategoryId },
                new Product { Name = "Chips", CategoryId = categories.First(c => c.Name == "Snacks").CategoryId },
                new Product { Name = "Pretzels", CategoryId = categories.First(c => c.Name == "Snacks").CategoryId },
                new Product { Name = "Ice Cream", CategoryId = categories.First(c => c.Name == "Frozen Foods").CategoryId },
                new Product { Name = "Ketchup", CategoryId = categories.First(c => c.Name == "Condiments").CategoryId },
                new Product { Name = "Guacamole", CategoryId = categories.First(c => c.Name == "Condiments").CategoryId },
                new Product { Name = "Salmon", CategoryId = categories.First(c => c.Name == "Seafood").CategoryId },
                new Product { Name = "Herring", CategoryId = categories.First(c => c.Name == "Seafood").CategoryId },
                new Product { Name = "Rice", CategoryId = categories.First(c => c.Name == "Grains").CategoryId },
                new Product { Name = "Bulgur", CategoryId = categories.First(c => c.Name == "Grains").CategoryId },
                new Product { Name = "Cinnamon", CategoryId = categories.First(c => c.Name == "Spices").CategoryId },
                new Product { Name = "Cardamom", CategoryId = categories.First(c => c.Name == "Spices").CategoryId }
            };

            if (!context.Products.Any())
            {
                context.Products.AddRange(products);
                await context.SaveChangesAsync();
            }
        }
    }
}