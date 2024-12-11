using NuGet.Protocol;
using TShockAPI;
using Wolfje.Plugins.SEconomy;

namespace Shop2;

public static class ModifyBuyItemsCommand
{
    
    public static void ModifyBuyItem(CommandArgs args, DB.ShopRegion region)
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

        if (args.Player.Name != region.Owner && !args.Player.HasPermission(Shop2.Configs.Settings.AdminCommandPerm))
        {
            args.Player.SendInfoMessage(Shop2.FormatMessage("AddBuyMessage1"));
            return;
        }

        if (args.Parameters.Count < 2)
        {
            args.Player.SendInfoMessage(Shop2.FormatMessage("ModifyItemMessage1").SFormat(TShock.Config.Settings.CommandSpecifier, Shop2.Configs.Settings.ShopCommand, Shop2.Configs.Settings.ModifyItemsSubCommand));
            if (args.Player.HasPermission(Shop2.Configs.Settings.AdminCommandPerm))
                args.Player.SendInfoMessage(Shop2.FormatMessage("ModifyItemMessage2", false).SFormat(TShock.Config.Settings.CommandSpecifier, Shop2.Configs.Settings.ShopCommand, Shop2.Configs.Settings.ModifyItemsSubCommand));
            return;
        }

        int ID;

        bool success = int.TryParse(args.Parameters[1], out ID);

        if (!success)
        {
            args.Player.SendInfoMessage(Shop2.FormatMessage("ModifyItemMessage3"));
            return;
        }

        var item = DB.GetItem(ID);

        if (item == null || (!args.Player.HasPermission(Shop2.Configs.Settings.AdminCommandPerm) && !region.SellingItems.Any(i => i.ID == ID)))
        {
            args.Player.SendInfoMessage(Shop2.FormatMessage("ModifyItemMessage4"));
            return;
        }

        if (args.Parameters.Count < 3)
        {
            args.Player.SendInfoMessage(Shop2.FormatMessage("ModifyItemMessage5"));
            return;
        }

        var value = args.Parameters[2];

        switch (value.ToLower())
        {
            case "price":

                if (args.Parameters.Count < 4)
                {
                    args.Player.SendInfoMessage(Shop2.FormatMessage("ModifyItemMessage6"));
                    return;
                }

                Money price;

                bool success1 = Money.TryParse(args.Parameters[3], out price);

                if (!success1 || price < 1)
                {
                    args.Player.SendInfoMessage(Shop2.FormatMessage("ModifyItemMessage7"));
                    return;
                }

                DB.ModifyItem(ID, "price", (int)price);
                var i = region.SellingItems.First(k => k.ID == ID);

                i.Price = (int)price;

                args.Player.SendInfoMessage(Shop2.FormatMessage("ModifyItemMessage10").SFormat(item.ItemID, item.ID, price.ToString()));

                return;

            case "category":

                if (args.Parameters.Count < 4)
                {
                    args.Player.SendInfoMessage(Shop2.FormatMessage("ModifyItemMessage8"));
                    return;
                }

                var cat = args.Parameters[3];

                DB.ModifyItem(ID, "category", cat);

                var i2 = region.SellingItems.First(k => k.ID == ID);

                i2.Category = cat;

                args.Player.SendInfoMessage(Shop2.FormatMessage("ModifyItemMessage11").SFormat(item.ItemID, item.ID, cat));

                return;

            case "chest":
                args.Player.SetData<(int, string)>(Handler.PLACE_SHOP_CHEST_DATA, (item.ID, region.RegionName));

                args.Player.SendInfoMessage(Shop2.FormatMessage("ModifyItemMessage12"));

                return;

            case "priceitem":

                if (args.Parameters.Count < 5)
                {
                    args.Player.SendInfoMessage(Shop2.FormatMessage("ModifyItemMessage13"));
                    return;
                }

                bool success2 = int.TryParse(args.Parameters[3], out int itemid);

                if (!success2 || TShock.Utils.GetItemById(itemid) == null || TShock.Utils.GetItemById(itemid).netID < 1 || TShock.Utils.GetItemById(itemid).netID > 5455)
                {
                    args.Player.SendInfoMessage(Shop2.FormatMessage("ModifyItemMessage14"));
                    return;
                }

                var item1 = TShock.Utils.GetItemById(itemid);

                success = int.TryParse(args.Parameters[4], out int amount);

                if (!success || amount > item1.maxStack)
                {
                    args.Player.SendInfoMessage(Shop2.FormatMessage("ModifyItemMessage15"));
                    return;
                }

                args.Player.SetData<(int, int, int, string)>(Handler.PLACE_SHOP_PRICE_CHEST_DATA, (item1.netID, amount, ID, region.RegionName));
                args.Player.SendInfoMessage(Shop2.FormatMessage("ModifyItemMessage17"));

                return;

            case "pricechest":

                var i4 = region.SellingItems.First(k => k.ID == ID);

                if (i4.PriceItemAmount == 0 || i4.PriceItemID < 1)
                {
                    Shop2.FormatMessage("ModifyItemMessage18");
                    return;
                }

                args.Player.SetData<(int, int, int, string)>(Handler.PLACE_SHOP_PRICE_CHEST_DATA, (i4.ItemID, i4.PriceItemAmount, ID, region.RegionName));
                args.Player.SendInfoMessage(Shop2.FormatMessage("ModifyItemMessage17"));

                return;

            case "bossreq":

                if (!args.Player.HasPermission(Shop2.Configs.Settings.AdminCommandPerm))
                {
                    args.Player.SendInfoMessage(Shop2.FormatMessage("ModifyItemMessage19"));
                    return;
                }

                args.Parameters.RemoveRange(0, 3);

                List<int> bosses = new List<int>();

                if (args.Parameters.Count == 0)
                {
                    args.Player.SendInfoMessage(Shop2.FormatMessage("ModifyItemMessage20"));
                    return;
                }

                foreach (string s1 in args.Parameters)
                {
                    bool success3 = int.TryParse(s1, out int boss);

                    if (!success3 || !Shop2.ValidBossIDs.Contains(boss))
                    {
                        args.Player.SendInfoMessage(Shop2.FormatMessage("ModifyItemMessage21"));
                        return;
                    }

                    bosses.Add(boss);
                }

                var i5 = region.SellingItems.First(k => k.ID == ID);
                i5.DefeatedBossesReq = bosses;

                DB.ModifyItem(ID, "defeatedbossreq", bosses.ToJson());

                List<string> strings = new List<string>();

                bosses.ForEach(k => strings.Add(Terraria.Lang.GetNPCNameValue(k)));

                string s = String.Join(", ", strings);

                args.Player.SendInfoMessage(Shop2.FormatMessage("ModifyItemMessage22").SFormat(i5.ItemID, ID, s));

                return;

            default:
                args.Player.SendInfoMessage(Shop2.FormatMessage("ModifyItemMessage19"));
                return;
        }
    }
}