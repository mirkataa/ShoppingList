namespace ShoppingListApp.Models
{
    ///<summary>
    ///Represents a category of products in the shopping list application.
    ///</summary>
    public class Category
    {
        ///<summary>
        ///Gets or sets the unique identifier for the category.
        ///</summary>
        public int CategoryId { get; set; }

        ///<summary>
        ///Gets or sets the name of the category.
        ///</summary>
        public string Name { get; set; }

        ///<summary>
        ///Gets or sets the list of products associated with this category.
        ///</summary>
        public List<Product> Products { get; set; }

        ///<summary>
        ///Initializes a new instance of the <see cref="Category"/> class and sets the Products list to a new instance.
        ///</summary>
        public Category()
        {
            Products = new List<Product>();
        }
    }
}
