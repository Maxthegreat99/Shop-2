using Terraria;
using TShockAPI;
using Wolfje.Plugins.SEconomy.Journal;

namespace Shop2;

public class ListItemsCommand
{
    
    public static void ListItems(CommandArgs args, DB.ShopRegion region, IBankAccount acc)
    {
        if (!args.Player.RealPlayer)
        {
            args.Player.SendInfoMessage(Shop2.FormatMessage("NotReal"));
            return;
        }

        if (region == null)
        {
            args.Player.SendInfoMessage(Shop2.FormatMessage("NotInAShop"));
            return;
        }

        (string, int) currentCategory = args.Player.GetData<(string, int)>(Handler.CATEGORY_DATA);

        bool listFromCategory = false;

        if (!String.IsNullOrEmpty(currentCategory.Item1) && currentCategory.Item1 == region.RegionName && currentCategory.Item2 != 0)
            listFromCategory = true;

        var cats = StoreCommand.CategorizeItems(region);

        List<DB.SellingItem> noneCat = null;

        if (cats.ContainsKey("none"))
        {
            noneCat = cats["none"];
            cats.Remove("none");
        }

        if (listFromCategory)
        {
            var chosenCat = cats.ToList().ElementAt(currentCategory.Item2 - 1);
            args.Player.SendInfoMessage(Shop2.FormatMessage("ListItemsMessage2").SFormat(chosenCat.Key, region.RegionName));

            StoreCommand.SendListItems(args, region, acc, chosenCat.Value);
        }
        else
        {
            if (cats.Count == 0 && noneCat == null)
            {
                args.Player.SendInfoMessage(Shop2.FormatMessage("ListItemsMessage3"));
                return;
            }

            if (cats.Count > 0)
            {
                args.Player.SendInfoMessage(Shop2.FormatMessage("ListItemsMessage1").SFormat(region.RegionName));
                args.Player.SendInfoMessage(Shop2.FormatMessage("ListItemsMessage7"));

                int i = 0;
                foreach (var cat in cats)
                {
                    i++;
                    args.Player.SendInfoMessage("- {0} 【[c/{1}:{2}]】".SFormat(i, Microsoft.Xna.Framework.Color.Gold.Hex3(), cat.Key));
                }
                args.Player.SendInfoMessage(Shop2.FormatMessage("ListItemsMessage9").SFormat(TShock.Config.Settings.CommandSpecifier, Shop2.Configs.Settings.ShopCommand, Shop2.Configs.Settings.CategorySubCommand));
            }

            if (noneCat != null)
            {
                args.Player.SendInfoMessage(Shop2.FormatMessage("ListItemsMessage8"));

                StoreCommand.SendListItems(args, region, acc, noneCat);
            }
        }
    }

}