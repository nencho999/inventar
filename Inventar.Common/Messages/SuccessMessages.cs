namespace Inventar.Common.Messages;

public class SuccessMessages
{
    public static class Expense
    {
        public const string ExpenseCreatedSuccessfully = "The expense has been registered successfully!";
    }

    public static class PrimaryMaterialBase
    {
        public const string BaseSavedSuccessfully = "The database has been saved successfully!";
        public const string BaseDeletedSuccessfully = "The database has been deleted successfully!";
    }

    public static class Stock
    {
        public const string StockAddedSuccessfully = "Stock added successfully!";
        public const string StockRemovedSuccessfully = "Stock removed successfully!";
    }
}