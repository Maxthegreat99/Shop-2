using TerrariaApi.Server;
using TShockAPI;

namespace Shop2
{
    public class Handler
    {
        public const string CATEGORY_DATA = "shopcategory";

        public static void OnNetGreet(GreetPlayerEventArgs args)
        {
            if (TShock.Players.ElementAtOrDefault(args.Who) == null)
            {
                return;
            }

            TShock.Players.ElementAtOrDefault(args.Who).SetData(CATEGORY_DATA, ("", 0));
        }
    }
}