namespace Inventar.Common.Messages
{
    public static class ErrorMessages
    {
        public static class ProductionCenter
        {
            public const string CenterCannotBeNullErrorMessage = "The production center cannot be null.";
            public const string CenterNotFoundErrorMessage = "The selected center is not found";
        }
        public static class Admin
        {
            public const string AdminAuthorization = "You must be an administrator to perform this action.";
        }
    }
}
