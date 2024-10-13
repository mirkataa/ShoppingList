using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ShoppingListApp.Models;

namespace ShoppingListsApp.Data
{
    ///<summary>
    ///Represents the application's database context, inheriting from IdentityDbContext to support ASP.NET Identity features.
    ///</summary>
    public class ApplicationDbContext : IdentityDbContext
    {
        ///<summary>
        ///Initializes a new instance of the <see cref="ApplicationDbContext"/> class.
        ///</summary>
        ///<param name="options">The options for configuring the context.</param>
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        ///<summary>
        ///Gets or sets the categories in the database.
        ///</summary>
        public DbSet<Category> Categories { get; set; }

        ///<summary>
        ///Gets or sets the products in the database.
        ///</summary>
        public DbSet<Product> Products { get; set; }

        ///<summary>
        ///Gets or sets the shopping lists in the database.
        ///</summary>
        public DbSet<ShoppingListModel> ShoppingLists { get; set; }

        ///<summary>
        ///Configures the model creating process for the database context.
        ///</summary>
        ///<param name="modelBuilder">The model builder used to build the model.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ShoppingListModel>()
                .HasKey(s => s.ShoppingListId);

            modelBuilder.Entity<Product>()
                .HasKey(p => p.ProductId);

        }
    }
}
