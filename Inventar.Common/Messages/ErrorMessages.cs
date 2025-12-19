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

        public static class Stock
        {
            public const string InsufficientStock = "ERROR: Insufficient stock! Current stock is {0}, but you are trying to remove {1}.";
            public const string CapacityExceeded = "ERROR: Capacity exceeded! Base limit is {0}. Current stock is {1}. You can only add {2}.";
            public const string NoCapacityDefined = "This base has no defined capacity limit for this material.";
        }

        public static class Material
        {
            public const string MaterialInUseCannotBeDeleted = "You cannot delete material that already has a movement history!";
        }
    }
}
