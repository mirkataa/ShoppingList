namespace ShoppingListApp.Models
{
    ///<summary>
    ///Represents a product in the shopping list application.
    ///</summary>
    public class Product
    {
        ///<summary>
        ///Gets or sets the unique identifier for the product.
        ///</summary>
        public int ProductId { get; set; }

        ///<summary>
        ///Gets or sets the name of the product.
        ///</summary>
        public string Name { get; set; }

        ///<summary>
        ///Gets or sets the unique identifier for the category to which the product belongs.
        ///</summary>
        public int CategoryId { get; set; }  
    }
    
}
