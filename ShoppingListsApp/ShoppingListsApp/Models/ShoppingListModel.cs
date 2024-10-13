namespace ShoppingListApp.Models
{
    ///<summary>
    ///Represents a shopping list in the shopping list application.
    ///</summary>
    public class ShoppingListModel
    {
        ///<summary>
        ///Gets or sets the unique identifier for the shopping list.
        ///</summary>
        public int ShoppingListId { get; set; }

        ///<summary>
        ///Gets or sets the username of the user who owns the shopping list.
        ///This acts as a foreign key to the user.
        ///</summary>
        public string UserName { get; set; } // FK to user

        ///<summary>
        ///Gets or sets the name of the shopping list.
        ///</summary>
        public string Name { get; set; }

        ///<summary>
        ///Gets or sets the list of items in the shopping list.
        ///</summary>
        public List<string>? Items { get; set; }

        ///<summary>
        ///Initializes a new instance of the <see cref="ShoppingListModel"/> class.
        ///</summary>
        public ShoppingListModel()
        {
            Items = new List<string>();
        }
    }
}
