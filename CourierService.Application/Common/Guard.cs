namespace CourierService.Application.Common
{
    public static class Guard
    {
        public static void NotNull(object value, string name)
        {
            if (value == null) throw new ArgumentNullException(name);
        }

        public static void NotNullOrEmpty<T>(ICollection<T> collection, string name)
        {
            if (collection == null) throw new ArgumentNullException(name);
            if (collection.Count == 0) throw new ArgumentException("Collection must not be empty", name);
        }

        public static void NonNegative(decimal value, string name)
        {
            if (value < 0) throw new ArgumentException($"{name} must be non-negative", name);
        }

        public static void GreaterThanZero(decimal value, string name)
        {
            if (value <= 0) throw new ArgumentException($"{name} must be greater than zero", name);
        }
    }
}
