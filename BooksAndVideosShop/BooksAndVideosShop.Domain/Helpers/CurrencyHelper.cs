namespace BooksAndVideosShop.Domain.Helpers
{
    public static class CurrencyHelper
    {
        public static decimal ToBankersRounding(decimal amount)
        {
            return Math.Round(amount, 2, MidpointRounding.ToEven);
        }
    }
}