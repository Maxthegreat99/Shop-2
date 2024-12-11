using Terraria;
using TShockAPI;
using Wolfje.Plugins.SEconomy.Journal;

namespace Shop2;

public class CategoryCommand
{
    public static void Category(CommandArgs args, DB.ShopRegion region, IBankAccount acc)
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

        int index = 0;
        string inputCat = null;

        if (args.Parameters.Count > 1)
        {
            bool parsed = false;
            try
            {
                index = int.Parse(args.Parameters[1]);
                parsed = true;
            }
            catch (Exception ex)
            {
                index = 0;
            }

            if (!parsed)
                inputCat = args.Parameters[1];
        }

        var cats = StoreCommand.CategorizeItems(region);

        List<DB.SellingItem> noneCat = null;
        if (cats.ContainsKey("none"))
        {
            noneCat = cats["none"];
            cats.Remove("none");
        }
        var catNames = cats.Keys;

        var choosenCat = "";

        if (inputCat != null)
        {
            choosenCat = catNames.FirstOrDefault(i => i.StartsWith(inputCat));

            if (choosenCat == null)
            {
                args.Player.SendInfoMessage(Shop2.FormatMessage("CategoryMessage1").SFormat(inputCat));
                return;
            }

            index = catNames.ToList().IndexOf(choosenCat) + 1;
        }
        else
        {
            if ((index - 1 >= cats.Count || index - 1 < 0) && index != 0)
            {
                args.Player.SendInfoMessage(Shop2.FormatMessage("CategoryMessage2"));
                return;
            }

            if (index == 0) choosenCat = "Default";
            else choosenCat = cats.Keys.ElementAt(index - 1);
        }

        var items = (choosenCat == "Default") ? noneCat : cats[choosenCat];

        args.Player.SetData<(string, int)>(Handler.CATEGORY_DATA, (region.RegionName, index));

        args.Player.SendInfoMessage(Shop2.FormatMessage("CategoryMessage3").SFormat(choosenCat));

        if (choosenCat != "Default")
        {
            args.Player.SendInfoMessage(Shop2.FormatMessage("ListItemsMessage2").SFormat(choosenCat, region.RegionName));

            StoreCommand.SendListItems(args, region, acc, items);
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
                if (cats.Count == 0) args.Player.SendInfoMessage(Shop2.FormatMessage("ListItemsMessage17").SFormat(region.RegionName));

                args.Player.SendInfoMessage(Shop2.FormatMessage("ListItemsMessage8"));

                StoreCommand.SendListItems(args, region, acc, noneCat);
            }
        }
    }
}