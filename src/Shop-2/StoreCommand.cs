using NuGet.Protocol;
using Terraria;
using TShockAPI;
using Wolfje.Plugins.SEconomy;
using Wolfje.Plugins.SEconomy.Journal;

namespace Shop2;

public class StoreCommand
{
    public static void Store(CommandArgs args)
    {
        var acc = SEconomyPlugin.Instance.GetBankAccount(args.Player);
        if (acc == null || !acc.IsAccountEnabled)
        {
            args.Player.SendErrorMessage(Shop2.FormatMessage("NoAcc"));
            return;
        }

        var rg = TShock.Regions.InAreaRegionName(args.Player.TileX, args.Player.TileY);
        DB.ShopRegion region = null;

        if (rg != null && rg.Count() != 0)
        {
            for (int i = 0; i < rg.Count(); i++)
                if (DB.regions.Any(k => k.RegionName.Equals(rg.ElementAt(i))))
                    region = DB.regions.First(k => k.RegionName.Equals(rg.ElementAt(i)));
        }

        if (args.Parameters.Count == 0)
        {
            if (args.Player.HasPermission(Shop2.Configs.Settings.PlayerCommandPerm))
                args.Player.SendInfoMessage(Shop2.FormatMessage("PlayerCommandHelp").SFormat(TShock.Config.Settings.CommandSpecifier, Shop2.Configs.Settings.ShopCommand, Shop2.Configs.Settings.ListRegionSubCommand));

            if (args.Player.HasPermission(Shop2.Configs.Settings.ShopOwnerPerm))
            {
                args.Player.SendInfoMessage(Shop2.FormatMessage("PlayerCommandHelp2", false).SFormat(TShock.Config.Settings.CommandSpecifier, Shop2.Configs.Settings.ShopCommand, Shop2.Configs.Settings.CreateShopSubCommand));

            }
            if (region != null && args.Player.HasPermission(Shop2.Configs.Settings.PlayerCommandPerm)) {
                args.Player.SendInfoMessage(Shop2.FormatMessage("PlayerCommandHelp1", false).SFormat(TShock.Config.Settings.CommandSpecifier, Shop2.Configs.Settings.ShopCommand, Shop2.Configs.Settings.ListItemsSubCommand));
                args.Player.SendInfoMessage(Shop2.FormatMessage("PlayerCommandHelp3", false).SFormat(TShock.Config.Settings.CommandSpecifier, Shop2.Configs.Settings.ShopCommand, Shop2.Configs.Settings.CategorySubCommand));
                args.Player.SendInfoMessage(Shop2.FormatMessage("PlayerCommandHelp4", false).SFormat(TShock.Config.Settings.CommandSpecifier, Shop2.Configs.Settings.ShopCommand, Shop2.Configs.Settings.BuyItemsSubCommand));
                args.Player.SendInfoMessage(Shop2.FormatMessage("PlayerCommandHelp5", false).SFormat(TShock.Config.Settings.CommandSpecifier, Shop2.Configs.Settings.ShopCommand, Shop2.Configs.Settings.SellSubCommand));
                args.Player.SendInfoMessage(Shop2.FormatMessage("PlayerCommandHelp6", false).SFormat(TShock.Config.Settings.CommandSpecifier, Shop2.Configs.Settings.ShopCommand, Shop2.Configs.Settings.ConfirmSubCommand));
            }

            if (region != null && args.Player.HasPermission(Shop2.Configs.Settings.ShopOwnerPerm) && args.Player.Name == region.Owner)
            {
                args.Player.SendInfoMessage(Shop2.FormatMessage("ShopOwnerCommandHelp7", false).SFormat(TShock.Config.Settings.CommandSpecifier, Shop2.Configs.Settings.ShopCommand, Shop2.Configs.Settings.AddBuyItemsSubCommand));
                args.Player.SendInfoMessage(Shop2.FormatMessage("ShopOwnerCommandHelp8", false).SFormat(TShock.Config.Settings.CommandSpecifier, Shop2.Configs.Settings.ShopCommand, Shop2.Configs.Settings.AddSellItemSubCommand));
                args.Player.SendInfoMessage(Shop2.FormatMessage("ShopOwnerCommandHelp9", false).SFormat(TShock.Config.Settings.CommandSpecifier, Shop2.Configs.Settings.ShopCommand, Shop2.Configs.Settings.ModifyItemsSubCommand));
                args.Player.SendInfoMessage(Shop2.FormatMessage("ShopOwnerCommandHelp10", false).SFormat(TShock.Config.Settings.CommandSpecifier, Shop2.Configs.Settings.ShopCommand, Shop2.Configs.Settings.ModifyShopSubCommand));
            }

            args.Player.SendInfoMessage(Shop2.FormatMessage("ShopOwnerCommandHelp11", false));
            args.Player.SendInfoMessage("\n");
            return;
        }

        string subCommand = args.Parameters[0];

        if (args.Player.HasPermission(Shop2.Configs.Settings.PlayerCommandPerm)
            && (subCommand == Shop2.Configs.Settings.ListRegionSubCommand
            || subCommand == "listregs"
            || subCommand == "lr"))
        {
            ListRegionsCommand.ListRegions(args);
            args.Player.SendInfoMessage("\n");
            return;
        }

        if (args.Player.HasPermission(Shop2.Configs.Settings.PlayerCommandPerm)
            && (subCommand == Shop2.Configs.Settings.ListItemsSubCommand
            || subCommand == "listi"
            || subCommand == "li"))
        {
            ListItemsCommand.ListItems(args, region, acc);
            args.Player.SendInfoMessage("\n");
            return;
        }

        if (args.Player.HasPermission(Shop2.Configs.Settings.PlayerCommandPerm)
            && (subCommand == Shop2.Configs.Settings.CategorySubCommand
            || subCommand == "cat"
            || subCommand == "ct"))
        {
            CategoryCommand.Category(args, region, acc);
            args.Player.SendInfoMessage("\n");
            return;
        }

        if (args.Player.HasPermission(Shop2.Configs.Settings.PlayerCommandPerm)
            && (subCommand == Shop2.Configs.Settings.BuyItemsSubCommand
            || subCommand == "b"))
        {
            SendBuyConfirmMessageCommand.SendBuyConfirmMessage(args, region, acc);
            args.Player.SendInfoMessage("\n");
            return;
        }

        if (args.Player.HasPermission(Shop2.Configs.Settings.PlayerCommandPerm)
            && (subCommand == Shop2.Configs.Settings.SellSubCommand
            || subCommand == "s"))
        {
            SendBuyConfirmMessageCommand.SendBuyConfirmMessage(args, region, acc);
            args.Player.SendInfoMessage("\n");
            return;
        }

        if (args.Player.HasPermission(Shop2.Configs.Settings.ShopOwnerPerm)
            && (subCommand == Shop2.Configs.Settings.AddBuyItemsSubCommand
            || subCommand == "abi"))
        {
            AddBuyItemCommand.AddBuyItem(args, region);
            args.Player.SendInfoMessage("\n");
            return;
        }

        if (args.Player.HasPermission(Shop2.Configs.Settings.ShopOwnerPerm)
            && (subCommand == Shop2.Configs.Settings.ModifyItemsSubCommand
            || subCommand == "mi"))
        {
            ModifyBuyItemsCommand.ModifyBuyItem(args, region);
            args.Player.SendInfoMessage("\n");
            return;
        }

        if (args.Player.HasPermission(Shop2.Configs.Settings.ShopOwnerPerm)
            && (subCommand == Shop2.Configs.Settings.CreateShopSubCommand
            || subCommand == "c"))
        {
            CreateShopCommand.CreateShop(args, acc);
            args.Player.SendInfoMessage("\n");
            return;
        }

        if (args.Player.HasPermission(Shop2.Configs.Settings.ShopOwnerPerm)
            && (subCommand == Shop2.Configs.Settings.ModifyShopSubCommand
            || subCommand == "ms"))
        {
            ModifyShopCommand.ModifyShop(args, region, acc);
            args.Player.SendInfoMessage("\n");
            return;
        }

        if (args.Player.HasPermission(Shop2.Configs.Settings.ShopOwnerPerm)
            && (subCommand == Shop2.Configs.Settings.AddSellItemSubCommand
            || subCommand == "asi"))
        {
            AddSellItemCommand.AddSellItem(args, region);
            args.Player.SendInfoMessage("\n");
            return;
        }

        if (args.Player.HasPermission(Shop2.Configs.Settings.PlayerCommandPerm)
            && (subCommand == Shop2.Configs.Settings.ConfirmSubCommand
            || subCommand == "co"))
        {
            ConfirmCommand.Confrim(args, region, acc);
            args.Player.SendInfoMessage("\n");
            return;
        }


        if (args.Player.HasPermission(Shop2.Configs.Settings.PlayerCommandPerm))
            args.Player.SendInfoMessage(Shop2.FormatMessage("PlayerCommandHelp").SFormat(TShock.Config.Settings.CommandSpecifier, Shop2.Configs.Settings.ShopCommand, Shop2.Configs.Settings.ListRegionSubCommand));

        if (args.Player.HasPermission(Shop2.Configs.Settings.ShopOwnerPerm))
        {
            args.Player.SendInfoMessage(Shop2.FormatMessage("PlayerCommandHelp2", false).SFormat(TShock.Config.Settings.CommandSpecifier, Shop2.Configs.Settings.ShopCommand, Shop2.Configs.Settings.CreateShopSubCommand));

        }
        if (region != null && args.Player.HasPermission(Shop2.Configs.Settings.PlayerCommandPerm))
        {
            args.Player.SendInfoMessage(Shop2.FormatMessage("PlayerCommandHelp1", false).SFormat(TShock.Config.Settings.CommandSpecifier, Shop2.Configs.Settings.ShopCommand, Shop2.Configs.Settings.ListItemsSubCommand));
            args.Player.SendInfoMessage(Shop2.FormatMessage("PlayerCommandHelp3", false).SFormat(TShock.Config.Settings.CommandSpecifier, Shop2.Configs.Settings.ShopCommand, Shop2.Configs.Settings.CategorySubCommand));
            args.Player.SendInfoMessage(Shop2.FormatMessage("PlayerCommandHelp4", false).SFormat(TShock.Config.Settings.CommandSpecifier, Shop2.Configs.Settings.ShopCommand, Shop2.Configs.Settings.BuyItemsSubCommand));
            args.Player.SendInfoMessage(Shop2.FormatMessage("PlayerCommandHelp5", false).SFormat(TShock.Config.Settings.CommandSpecifier, Shop2.Configs.Settings.ShopCommand, Shop2.Configs.Settings.SellSubCommand));
            args.Player.SendInfoMessage(Shop2.FormatMessage("PlayerCommandHelp6", false).SFormat(TShock.Config.Settings.CommandSpecifier, Shop2.Configs.Settings.ShopCommand, Shop2.Configs.Settings.ConfirmSubCommand));
        }

        if (region != null && args.Player.HasPermission(Shop2.Configs.Settings.ShopOwnerPerm) && args.Player.Name == region.Owner)
        {
            args.Player.SendInfoMessage(Shop2.FormatMessage("ShopOwnerCommandHelp7", false).SFormat(TShock.Config.Settings.CommandSpecifier, Shop2.Configs.Settings.ShopCommand, Shop2.Configs.Settings.AddBuyItemsSubCommand));
            args.Player.SendInfoMessage(Shop2.FormatMessage("ShopOwnerCommandHelp8", false).SFormat(TShock.Config.Settings.CommandSpecifier, Shop2.Configs.Settings.ShopCommand, Shop2.Configs.Settings.AddSellItemSubCommand));
            args.Player.SendInfoMessage(Shop2.FormatMessage("ShopOwnerCommandHelp9", false).SFormat(TShock.Config.Settings.CommandSpecifier, Shop2.Configs.Settings.ShopCommand, Shop2.Configs.Settings.ModifyItemsSubCommand));
            args.Player.SendInfoMessage(Shop2.FormatMessage("ShopOwnerCommandHelp10", false).SFormat(TShock.Config.Settings.CommandSpecifier, Shop2.Configs.Settings.ShopCommand, Shop2.Configs.Settings.ModifyShopSubCommand));
        }

        args.Player.SendInfoMessage(Shop2.FormatMessage("ShopOwnerCommandHelp11", false));
        args.Player.SendInfoMessage("\n");
        return;
    }
    
    public static Dictionary<string, List<DB.SellingItem>> CategorizeItems(DB.ShopRegion region)
    {
        Dictionary<string, List<DB.SellingItem>> dic = new Dictionary<string, List<DB.SellingItem>>();

        foreach (DB.SellingItem i in region.SellingItems)
        {
            if (String.IsNullOrEmpty(i.Category))
            {
                if (!dic.ContainsKey("none")) dic.Add("none", new List<DB.SellingItem> { i });
                else dic["none"].Add(i);
                continue;
            }

            if (!dic.ContainsKey(i.Category)) dic.Add(i.Category, new List<DB.SellingItem> { i });
            else dic[i.Category].Add(i);
        }

        return dic;
    }

    public static void SendListItems(CommandArgs args, DB.ShopRegion region, IBankAccount acc, List<DB.SellingItem> cat)
    {
        var buyItems = cat.FindAll(i => !i.Sold);
        var sellItems = cat.FindAll(i => i.Sold);

        int i = 0;

        for (int l = 0; l < buyItems.Count; l++)
        {
            DB.SellingItem? item = buyItems[l];
            i++;

            if (args.Player.Name != region.Owner)
            {
                bool show = true;

                foreach (int k in item.DefeatedBossesReq)
                {
                    if (!Shop2.DefeatedBosses.Contains(k) && args.Player.HasPermission(Shop2.Configs.Settings.AdminCommandPerm))
                    {
                        show = false;
                        break;
                    }
                }

                if (!show)
                    continue;
            }

            Item tItem = TShock.Utils.GetItemById(item.ItemID);

            Money buyingPriceToShow = item.Price;

            string priceShown = (item.PriceItemID == 0) ? Shop2.Configs.Settings.Messages["ListItemsMessage10"] : Shop2.Configs.Settings.Messages["ListItemsMessage10"] + " [i/s{0}:{1}] +".SFormat(item.PriceItemAmount, item.PriceItemID);
            string itemShown = string.Format("[i:{0}]", item.ItemID);
            args.Player.SendInfoMessage("- {0} 【{1}】 - ([c/{2}:{3}]) {4} {5}" + ((region.Owner == args.Player.Name || args.Player.HasPermission(Shop2.Configs.Settings.AdminCommandPerm)) ? Shop2.Configs.Settings.Messages["ListItemsMessage6"].SFormat(item.ID, item.Stock) : ""), i, itemShown,
                                        Microsoft.Xna.Framework.Color.Gold.Hex3(), tItem.HoverName,
                                        priceShown,
                                        buyingPriceToShow.ToString());
        }

        if (sellItems.Count == 0)
        {
            if (region.Owner == args.Player.Name) args.Player.SendInfoMessage(Shop2.FormatMessage("ListItemsMessage16").SFormat(region.SellingItems.Count, Shop2.Configs.Settings.MaxShopItems));
            args.Player.SendInfoMessage(Shop2.FormatMessage("ListItemsMessage4").SFormat(TShock.Config.Settings.CommandSpecifier, Shop2.Configs.Settings.ShopCommand, Shop2.Configs.Settings.BuyItemsSubCommand));
            args.Player.SendInfoMessage(Shop2.FormatMessage("ListItemsMessage11"));
            args.Player.SendInfoMessage(Shop2.FormatMessage("ListItemsMessage5").SFormat(acc.Balance.ToString()));

            return;
        }

        args.Player.SendInfoMessage(Shop2.FormatMessage("ListItemsMessage12"));
        i = 0;
        for (int l = 0; l < sellItems.Count; l++)
        {
            DB.SellingItem? item = sellItems[l];
            i++;

            if (args.Player.Name != region.Owner)
            {
                bool show = true;

                foreach (int k in item.DefeatedBossesReq)
                {
                    if (!Shop2.DefeatedBosses.Contains(k))
                    {
                        show = false;
                        break;
                    }
                }

                if (!show)
                    continue;
            }

            Item tItem = TShock.Utils.GetItemById(item.ItemID);

            Money buyingPriceToShow = item.Price;

            string priceShown = Shop2.Configs.Settings.Messages["ListItemsMessage13"];
            string itemShown = string.Format("[i:{0}]", item.ItemID);
            args.Player.SendInfoMessage("- {0} 【{1}】 - ([c/{2}:{3}]) [c/{4}:{5}] {6}" + ((region.Owner == args.Player.Name || args.Player.HasPermission(Shop2.Configs.Settings.AdminCommandPerm)) ? Shop2.Configs.Settings.Messages["ListItemsMessage14"].SFormat(item.ID, item.Stock) : ""), i, itemShown,
                                        Microsoft.Xna.Framework.Color.Gold.Hex3(), tItem.HoverName,
                                        Microsoft.Xna.Framework.Color.Magenta.Hex3(),
                                        priceShown,
                                        buyingPriceToShow.ToString());
        }

        if (region.Owner == args.Player.Name) args.Player.SendInfoMessage(Shop2.FormatMessage("ListItemsMessage16").SFormat(region.SellingItems.Count, Shop2.Configs.Settings.MaxShopItems));
        args.Player.SendInfoMessage(Shop2.FormatMessage("ListItemsMessage4").SFormat(TShock.Config.Settings.CommandSpecifier, Shop2.Configs.Settings.ShopCommand, Shop2.Configs.Settings.BuyItemsSubCommand));
        args.Player.SendInfoMessage(Shop2.FormatMessage("ListItemsMessage15").SFormat(TShock.Config.Settings.CommandSpecifier, Shop2.Configs.Settings.ShopCommand, Shop2.Configs.Settings.SellSubCommand));
        args.Player.SendInfoMessage(Shop2.FormatMessage("ListItemsMessage11"));
        args.Player.SendInfoMessage(Shop2.FormatMessage("ListItemsMessage5").SFormat(acc.Balance.ToString()));
    }

    // source: ComfyEconomy's Utils.cs -> https://github.com/Soof4/ComfyEconomy/blob/main/ComfyEconomy/Utils.cs
    public static int GetChestIdByPos(int x, int y)
    {
        for (int i = 0; i < Main.maxChests; i++)
        {
            if (Main.chest[i] != null && ((Main.chest[i].x == x && Main.chest[i].y == y) || (Main.chest[i].x + 1 == x && Main.chest[i].y == y) || (Main.chest[i].x == x && Main.chest[i].y + 1 == y) || (Main.chest[i].x + 1 == x && Main.chest[i].y + 1 == y)))
            {
                return i;
            }
        }
        return -1;
    }

    // source: ComfyEconomy's Utils.cs -> https://github.com/Soof4/ComfyEconomy/blob/main/ComfyEconomy/Utils.cs#L30
    public static void DeleteItemsFromChest(int chestID, int itemID, int amount)
    {
        for (int i = 0; i < 40; i++)
        {
            Item item = Main.chest[chestID].item[i];

            if (item == null || !item.active || item.stack < 1) continue;

            if (item.netID == itemID)
            {
                if (item.stack < amount)
                {
                    amount -= item.stack;
                    item.stack = 0;
                    TSPlayer.All.SendData(PacketTypes.ChestItem, "", chestID, i, item.stack, item.prefix, item.netID);
                }
                else
                {
                    item.stack -= amount;
                    TSPlayer.All.SendData(PacketTypes.ChestItem, "", chestID, i, item.stack, item.prefix, item.netID);
                    break;
                }
            }
        }
    }

    public static int GetFreeSlotInChest(int chestID)
    {
        for (int i = 0; i < 40; i++)
        {
            Item item = Main.chest[chestID].item[i];
            if (item == null || !item.active || item.netID < 1 || item.stack == 0) return i;
        }

        return -1;
    }

    public static void AddItemToChest(int chestId, int slot, int itemid, int amount, int prefix)
    {
        Item item = TShock.Utils.GetItemById(itemid);
        item.stack = amount;
        item.Prefix(prefix);

        Main.chest[chestId].item[slot] = item;

        TSPlayer.All.SendData(PacketTypes.ChestItem, "", chestId, slot, item.stack, item.prefix, item.netID);
    }
}