namespace ShoppingListsApp.Models
{
    ///<summary>
    ///Represents the error view model used for displaying error information.
    ///</summary>
    public class ErrorViewModel
    {
        ///<summary>
        ///Gets or sets the unique identifier for the request.
        ///</summary>
        public string? RequestId { get; set; }

        ///<summary>
        ///Gets a value indicating whether the RequestId should be shown.
        ///</summary>
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
