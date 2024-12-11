using NuGet.Protocol;
using TShockAPI;
using Wolfje.Plugins.SEconomy;

namespace Shop2;

public static class AddSellItemCommand
{
    
    public static void AddSellItem(CommandArgs args, DB.ShopRegion region)
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

        if (args.Player.Name != region.Owner)
        {
            args.Player.SendInfoMessage(Shop2.FormatMessage("AddBuyMessage1"));
            return;
        }

        if (args.Parameters.Count < 4)
        {
            args.Player.SendInfoMessage(Shop2.FormatMessage("AddSellItemMessage1"));
            return;
        }

        bool success = int.TryParse(args.Parameters[1], out int itemID);

        if (!success || itemID < 1 || itemID > 5455)
        {
            args.Player.SendInfoMessage(Shop2.FormatMessage("AddSellItemMessage2"));
            return;
        }

        success = int.TryParse(args.Parameters[2], out int amount);

        if (!success || amount < 1)
        {
            args.Player.SendInfoMessage(Shop2.FormatMessage("AddSellItemMessage3"));
            return;
        }

        success = Money.TryParse(args.Parameters[3], out Money price);

        if (!success || price < 1)
        {
            args.Player.SendInfoMessage(Shop2.FormatMessage("AddSellItemMessage4"));
            return;
        }


        if (region.SellingItems.FindAll(i => i.Sold).Any(i => i.ItemID == itemID))
        {

            var item = region.SellingItems.FindAll(i => i.Sold).First(i => i.ItemID == itemID);

            item.Stock = amount;
            item.Price = (int)price;

            DB.ModifyItem(item.ID, "stock", amount);
            DB.ModifyItem(item.ID, "price", (int)price);

            args.Player.SendInfoMessage(Shop2.FormatMessage("AddSellItemMessage5"));
            return;
        }

        string category = "";
        if (args.Parameters.Count > 4) category = args.Parameters[4];

        var id = DB.InsertItem((int)price, new List<int>(), itemID, amount, 0, 0, category, 0, 0, 0, 0, true);
        region.SellingItems.Add(DB.GetItem(id));
        DB.ModifyShopRegion(region.ID, "sellingitems", region.SellingItems.Select(i => i.ID).ToJson());

        args.Player.SendInfoMessage(Shop2.FormatMessage("AddSellItemMessage6").SFormat(itemID, id, amount, price.ToString()));
        args.Player.SendInfoMessage(Shop2.FormatMessage("AddSellItemMessage7").SFormat(TShock.Config.Settings.CommandSpecifier, Shop2.Configs.Settings.ShopCommand, Shop2.Configs.Settings.ModifyItemsSubCommand));
    }
}