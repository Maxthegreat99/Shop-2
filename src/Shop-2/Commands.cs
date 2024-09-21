using System.Collections;
using System.Collections.Generic;
using Terraria;
using TShockAPI;
using Wolfje.Plugins.SEconomy;
using Wolfje.Plugins.SEconomy.Journal;

namespace Shop2;

public class Commands
{
    public static async void store(CommandArgs args)
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

            if (region != null && args.Player.HasPermission(Shop2.Configs.Settings.PlayerCommandPerm))
                args.Player.SendInfoMessage(Shop2.FormatMessage("PlayerCommandHelp1").SFormat(TShock.Config.Settings.CommandSpecifier, Shop2.Configs.Settings.ShopCommand, Shop2.Configs.Settings.ListItemsSubCommand));

            return;
        }

        string subCommand = args.Parameters[0];

        if (args.Player.HasPermission(Shop2.Configs.Settings.PlayerCommandPerm)
            && (subCommand == Shop2.Configs.Settings.ListRegionSubCommand
            || subCommand == "listregs"
            || subCommand == "lr"))
        {
            ListRegions(args);
            return;
        }

        if (args.Player.HasPermission(Shop2.Configs.Settings.PlayerCommandPerm)
            && (subCommand == Shop2.Configs.Settings.ListItemsSubCommand
            || subCommand == "listi"
            || subCommand == "li"))
        {
            ListItems(args, region, acc);
            return;
        }

        if (args.Player.HasPermission(Shop2.Configs.Settings.PlayerCommandPerm)
            && (subCommand == Shop2.Configs.Settings.CategorySubCommand
            || subCommand == "cat"
            || subCommand == "ct"))
        {
            Category(args, region, acc);
            return;
        }

        if (args.Player.HasPermission(Shop2.Configs.Settings.PlayerCommandPerm)
            && (subCommand == Shop2.Configs.Settings.BuyItemsSubCommand
            || subCommand == "b"))
        {
        }
    }

    private static void Buy(CommandArgs args, DB.ShopRegion region, IBankAccount acc)
    {
        if (region == null)
        {
            args.Player.SendInfoMessage("NotInAShop");
            return;
        }

        if (args.Player.Name == region.Owner)
        {
            args.Player.SendInfoMessage(Shop2.FormatMessage("BuyMessage1"));
            return;
        }

        if (args.Parameters.Count < 2)
        {
            args.Player.SendInfoMessage(Shop2.FormatMessage("BuyMessage2"));
            return;
        }

        if (!SEconomyPlugin.Instance.RunningJournal.BankAccounts.Any(i => i.UserAccountName == region.Owner))
        {
            args.Player.SendInfoMessage(Shop2.FormatMessage("BuyMessage8"));
            return;
        }

        int index = 0;
        string inputName = null;

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
            inputName = args.Parameters[1];

        int amount = 1;

        if (args.Parameters.Count > 2)
        {
            try
            {
                amount = int.Parse(args.Parameters[2]);
            }
            catch (Exception ex)
            {
                amount = 1;
            }
        }

        if (amount < 1)
        {
            args.Player.SendInfoMessage(Shop2.FormatMessage("BuyMessage3"));
            return;
        }

        (string, int) currentCategory = args.Player.GetData<(string, int)>(Handler.CATEGORY_DATA);

        bool listFromCategory = false;

        if (!String.IsNullOrEmpty(currentCategory.Item1) && currentCategory.Item1 == region.RegionName && currentCategory.Item2 != 0)
            listFromCategory = true;

        var cats = CategorizeItems(region);

        List<DB.SellingItem> noneCat = null;

        if (cats.ContainsKey("none"))
        {
            noneCat = cats["none"];
            cats.Remove("none");
        }

        if (noneCat == null && cats.Count == 0)
        {
            args.Player.SendInfoMessage(Shop2.FormatMessage("ListItemsMessage3"));
            return;
        }

        var items = (listFromCategory) ? cats.Values.ElementAt(currentCategory.Item2) : noneCat;

        int i = 0;

        bool found = false;
        DB.SellingItem searchedItem = null;

        foreach (var item in items)
        {
            i++;

            Item tItem = TShock.Utils.GetItemById(item.ItemID);
            tItem.Prefix(item.Prefix);

            if (i == index || (inputName != null && (tItem.HoverName.ToLower().StartsWith(inputName.ToLower()) || tItem.Name.ToLower().StartsWith(inputName.ToLower()))))
            {
                found = true;
                searchedItem = item;
            }

            if (found)
            {
                bool show = true;

                foreach (int l in item.DefeatedBossesReq)
                {
                    if (!Shop2.DefeatedBosses.Contains(l))
                    {
                        show = false;
                        break;
                    }
                }

                if (!show)
                {
                    args.Player.SendInfoMessage(Shop2.FormatMessage("BuyMessage4"));
                    return;
                }

                break;
            }
        }

        if (!found)
        {
            args.Player.SendInfoMessage(Shop2.FormatMessage("BuyMessage5"));
            return;
        }

        List<Item> itemsToGive = new();
        if (searchedItem.Stock == 0)
        {
            if (searchedItem.ChestPosY == 0 && searchedItem.ChestPosX == 0)
            {
                args.Player.SendInfoMessage(Shop2.FormatMessage("BuyMessage9"));
                return;
            }

            
            int chest = GetChestIdByPos(searchedItem.ChestPosX, searchedItem.ChestPosY);

            if (chest == -1)
            {
                args.Player.SendInfoMessage("BuyMessage10");
                return;
            }

            int stock = Main.chest[chest].item.Count(i => i.netID == searchedItem.ItemID);

            if (stock == 0)
            {
                args.Player.SendInfoMessage(Shop2.FormatMessage("BuyMessage9"));
                return;
            }

            if (stock < amount) amount = stock;
            
            if (searchedItem.Price * amount > acc.Balance)
            {
                args.Player.SendInfoMessage(Shop2.FormatMessage("BuyMessage6").SFormat(((Money)(searchedItem.Price * amount - acc.Balance)).ToString(), "[i/p{0}:{1}]".SFormat(searchedItem.Prefix, searchedItem.ItemID), amount));
                return;
            }



            if (searchedItem.PriceItemID != 0 && searchedItem.PriceItemAmount != 0)
            {
                if (searchedItem.PriceChestPosX == 0 && searchedItem.PriceChestPosY == 0)
                {
                    args.Player.SendInfoMessage(Shop2.FormatMessage("BuyMessage12"));
                    return;
                }

                int priceChest = GetChestIdByPos(searchedItem.PriceChestPosX, searchedItem.PriceChestPosY);

                if (priceChest == -1)
                {
                    args.Player.SendInfoMessage(Shop2.FormatMessage("BuyMessage13"));
                    return;
                }

                int chestSlot = GetFreeSlotInChest(priceChest);
                
                if (chestSlot == -1)
                {
                    args.Player.SendInfoMessage(Shop2.FormatMessage("BuyMessage14"));
                    return;
                    
                }

                int InvItem = Array.IndexOf(args.Player.TPlayer.inventory, args.Player.SelectedItem);
                
                if (args.Player.SelectedItem == null || !args.Player.SelectedItem.active || args.Player.SelectedItem.netID != searchedItem.PriceItemID || args.Player.SelectedItem.stack < searchedItem.PriceItemAmount * amount)
                {
                    args.Player.SendInfoMessage(Shop2.FormatMessage("BuyMessage11"));
                    return;
                }


                bool isSSC = Main.ServerSideCharacter;

                if (!isSSC)
                {
                    Main.ServerSideCharacter = true;
                    NetMessage.SendData(7, args.Player.Index, -1, null, 0, 0.0f, 0.0f, 0.0f, 0, 0, 0);
                    args.Player.IgnoreSSCPackets = true;
                }

                AddItemToChest(priceChest, chestSlot, args.Player.SelectedItem.netID, searchedItem.PriceItemAmount * amount, args.Player.SelectedItem.prefix);
                args.TPlayer.inventory[InvItem].stack -= searchedItem.PriceItemAmount * amount;

                NetMessage.SendData((int)PacketTypes.PlayerSlot, -1, -1, null, args.Player.Index, InvItem, args.TPlayer.inventory[InvItem].stack, args.TPlayer.inventory[InvItem].prefix, args.TPlayer.inventory[InvItem].netID, 0, 0);

                if (!isSSC)
                {
                    Main.ServerSideCharacter = false;
                    NetMessage.SendData(7, args.Player.Index, -1, null, 0, 0.0f, 0.0f, 0.0f, 0, 0, 0);
                    args.Player.IgnoreSSCPackets = false;
                }
            }

            int _amount = amount;

            for (int k = 0; _amount > 0;)
            {
                var l = Main.chest[chest].item.First(i => i.netID == searchedItem.ItemID);

                var j = TShock.Utils.GetItemById(l.netID);
                j.stack = l.stack;

                if (j.stack > _amount) j.stack = _amount;
                j.Prefix(l.prefix);

                itemsToGive.Add(j);

                _amount -= j.stack;

                DeleteItemsFromChest(chest, j.netID, j.stack);
            }
        }
        else
        {
            if (searchedItem.Stock < amount) amount = searchedItem.Stock;

            if (searchedItem.Price * amount > acc.Balance)
            {
                args.Player.SendInfoMessage(Shop2.FormatMessage("BuyMessage6").SFormat(((Money)(searchedItem.Price * amount - acc.Balance)).ToString(), "[i/p{0}:{1}]".SFormat(searchedItem.Prefix, searchedItem.ItemID), amount));
                return;
            }


            if (searchedItem.PriceItemID != 0 && searchedItem.PriceItemAmount != 0)
            {
                if (searchedItem.PriceChestPosX == 0 && searchedItem.PriceChestPosY == 0)
                {
                    args.Player.SendInfoMessage(Shop2.FormatMessage("BuyMessage12"));
                    return;
                }

                int priceChest = GetChestIdByPos(searchedItem.PriceChestPosX, searchedItem.PriceChestPosY);

                if (priceChest == -1)
                {
                    args.Player.SendInfoMessage(Shop2.FormatMessage("BuyMessage13"));
                    return;
                }

                int chestSlot = GetFreeSlotInChest(priceChest);

                if (chestSlot == -1)
                {
                    args.Player.SendInfoMessage(Shop2.FormatMessage("BuyMessage14"));
                    return;

                }

                int InvItem = Array.IndexOf(args.Player.TPlayer.inventory, args.Player.SelectedItem);

                if (args.Player.SelectedItem == null || !args.Player.SelectedItem.active || args.Player.SelectedItem.netID != searchedItem.PriceItemID || args.Player.SelectedItem.stack < searchedItem.PriceItemAmount * amount)
                {
                    args.Player.SendInfoMessage(Shop2.FormatMessage("BuyMessage11"));
                    return;
                }


                bool isSSC = Main.ServerSideCharacter;

                if (!isSSC)
                {
                    Main.ServerSideCharacter = true;
                    NetMessage.SendData(7, args.Player.Index, -1, null, 0, 0.0f, 0.0f, 0.0f, 0, 0, 0);
                    args.Player.IgnoreSSCPackets = true;
                }

                AddItemToChest(priceChest, chestSlot, args.Player.SelectedItem.netID, searchedItem.PriceItemAmount * amount, args.Player.SelectedItem.prefix);
                args.TPlayer.inventory[InvItem].stack -= searchedItem.PriceItemAmount * amount;

                NetMessage.SendData((int)PacketTypes.PlayerSlot, -1, -1, null, args.Player.Index, InvItem, args.TPlayer.inventory[InvItem].stack, args.TPlayer.inventory[InvItem].prefix, args.TPlayer.inventory[InvItem].netID, 0, 0);

                if (!isSSC)
                {
                    Main.ServerSideCharacter = false;
                    NetMessage.SendData(7, args.Player.Index, -1, null, 0, 0.0f, 0.0f, 0.0f, 0, 0, 0);
                    args.Player.IgnoreSSCPackets = false;
                }
            }

            var tItem = TShock.Utils.GetItemById(searchedItem.ItemID);

            tItem.Prefix(searchedItem.Prefix);

            double _slotsToFill = amount / tItem.maxStack;
            int slotsToFill = (int)Math.Ceiling(_slotsToFill);
            if (amount < tItem.maxStack)
                slotsToFill = 1;
            for (int l = 0; l < slotsToFill; l++)
            {
                int amountToFill = tItem.maxStack;
                if (l == slotsToFill - 1)
                    amountToFill = amount - (l * tItem.maxStack);

                var titem = TShock.Utils.GetItemById(searchedItem.ItemID);
                titem.Prefix(searchedItem.Prefix);
                titem.stack = amountToFill;

                itemsToGive.Add(titem);
            }

        }

        acc.TransferTo(SEconomyPlugin.Instance.RunningJournal.BankAccounts.First(i => i.UserAccountName == region.Owner), amount * searchedItem.Price, Wolfje.Plugins.SEconomy.Journal.BankAccountTransferOptions.AnnounceToSender | Wolfje.Plugins.SEconomy.Journal.BankAccountTransferOptions.IsPayment,
                       Shop2.Configs.Settings.Messages["BuyMessage15"], string.Format(Shop2.Configs.Settings.Messages["BuyMessage16"], amount, Terraria.Lang.GetItemNameValue(searchedItem.ItemID)));
        
        foreach(var l in itemsToGive)
            args.Player.GiveItem(l.netID, l.stack, l.prefix);


        args.Player.SendSuccessMessage(Shop2.FormatMessage("BuyMessage17").SFormat(amount, searchedItem.Prefix, searchedItem.ItemID, ((Money)amount * searchedItem.Price).ToString()) + ((searchedItem.PriceItemID != 0) ? $" + {searchedItem.PriceItemAmount} [i:{searchedItem.PriceItemID}]" : ""));

        args.Player.SendInfoMessage(Shop2.FormatMessage("ListItemsMessage5").SFormat(acc.Balance.ToString()));

        TShock.Log.Write(Shop2.FormatMessage("BuyMessage18").SFormat(args.Player.Name, amount, Terraria.Lang.GetItemNameValue(searchedItem.ItemID), amount * searchedItem.Price), System.Diagnostics.TraceLevel.Info);
    }

    private static void Category(CommandArgs args, DB.ShopRegion region, IBankAccount acc)
    {
        if (region == null)
        {
            args.Player.SendInfoMessage("NotInAShop");
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

        var cats = CategorizeItems(region);

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
            choosenCat = catNames.FirstOrDefault(i => i.ToLower().StartsWith(inputCat.ToLower()));

            if (choosenCat == null)
            {
                args.Player.SendInfoMessage(Shop2.FormatMessage("CategoryMessage1").SFormat(inputCat));
                return;
            }
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

        args.Player.SetData<(string, int)>(Handler.CATEGORY_DATA, (region.RegionName, (index == 0) ? 0 : index - 1));

        args.Player.SendInfoMessage(Shop2.FormatMessage("CategoryMessage3").SFormat(choosenCat));

        if (choosenCat != "Default")
        {
            args.Player.SendInfoMessage(Shop2.FormatMessage("ListItemsMessage2").SFormat(choosenCat, region.RegionName));

            SendListItems(args, region, acc, items);
        }
        else
        {
            if (cats.Count == 0 && noneCat == null)
            {
                args.Player.SendInfoMessage("ListItemsMessage3");
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
                    args.Player.SendInfoMessage("- {0}. 【{1}】".SFormat(i, cat));
                }
                args.Player.SendInfoMessage(Shop2.FormatMessage("ListItemsMessage9").SFormat(TShock.Config.Settings.CommandSpecifier, Shop2.Configs.Settings.ShopCommand, Shop2.Configs.Settings.CategorySubCommand));
            }

            if (noneCat != null)
            {
                args.Player.SendInfoMessage(Shop2.FormatMessage("ListItemsMessage8"));

                SendListItems(args, region, acc, noneCat);
            }
        }
    }

    private static void ListRegions(CommandArgs args)
    {
        int page = 0;
        if (args.Parameters.Count > 1)
        {
            try
            {
                page = int.Parse(args.Parameters[1]);
            }
            catch (Exception e)
            {
                page = 0;
            }
        }

        List<string> list = new List<string>();

        foreach (var shop in DB.regions)
        {
            string str = Shop2.Configs.Settings.Messages["ListRegionsMessage2"].SFormat(shop.RegionName, shop.Description, shop.Owner);

            list.Add(str);
        }

        PaginationTools.SendPage(args.Player, page, list, 5,
                                 new PaginationTools.Settings()
                                 {
                                     HeaderFormat = Shop2.Configs.Settings.Messages["ListRegionsMessage1"],
                                     FooterFormat = Shop2.Configs.Settings.Messages["ListRegionsMessage3"].SFormat(TShock.Config.Settings.CommandSpecifier, Shop2.Configs.Settings.ShopCommand, Shop2.Configs.Settings.ListRegionSubCommand),
                                     NothingToDisplayString = Shop2.Configs.Settings.Messages["ListRegionsMessage4"]
                                 });
    }

    private static void ListItems(CommandArgs args, DB.ShopRegion region, IBankAccount acc)
    {
        if (region == null)
        {
            args.Player.SendInfoMessage("NotInAShop");
            return;
        }

        (string, int) currentCategory = args.Player.GetData<(string, int)>(Handler.CATEGORY_DATA);

        bool listFromCategory = false;

        if (!String.IsNullOrEmpty(currentCategory.Item1) && currentCategory.Item1 == region.RegionName && currentCategory.Item2 != 0)
            listFromCategory = true;

        var cats = CategorizeItems(region);

        List<DB.SellingItem> noneCat = null;

        if (cats.ContainsKey("none"))
        {
            noneCat = cats["none"];
            cats.Remove("none");
        }

        if (listFromCategory)
        {
            var chosenCat = cats.ToList().ElementAt(currentCategory.Item2);
            args.Player.SendInfoMessage(Shop2.FormatMessage("ListItemsMessage2").SFormat(chosenCat.Key, region.RegionName));

            SendListItems(args, region, acc, chosenCat.Value);
        }
        else
        {
            if (cats.Count == 0 && noneCat == null)
            {
                args.Player.SendInfoMessage("ListItemsMessage3");
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
                    args.Player.SendInfoMessage("- {0}. 【{1}】".SFormat(i, cat));
                }
                args.Player.SendInfoMessage(Shop2.FormatMessage("ListItemsMessage9").SFormat(TShock.Config.Settings.CommandSpecifier, Shop2.Configs.Settings.ShopCommand, Shop2.Configs.Settings.CategorySubCommand));
            }

            if (noneCat != null)
            {
                args.Player.SendInfoMessage(Shop2.FormatMessage("ListItemsMessage8"));

                SendListItems(args, region, acc, noneCat);
            }
        }
    }
    private static Dictionary<string, List<DB.SellingItem>> CategorizeItems(DB.ShopRegion region)
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
        int i = 0;

        for (int l = 0; l < cat.Count; l++)
        {
            DB.SellingItem? item = cat[l];
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
            tItem.Prefix(item.Prefix);
            Money buyingPriceToShow = item.Price;

            string priceShown = (item.PriceItemID == 0) ? Shop2.Configs.Settings.Messages["ListItemsMessage10"] : Shop2.Configs.Settings.Messages["ListItemsMessage10"] + " [i/s{0}:{1}] +".SFormat(item.PriceItemAmount, item.PriceItemID);
            string itemShown = string.Format((item.Prefix != 0) ? "[i/p{0}:{1}]" : "[i:{1}]", tItem.prefix, item.ItemID);
            args.Player.SendInfoMessage("- {0} [{1}] - ([c/{2}:{3}]) [c/{4}:{5}] {6}" + ((region.Owner == args.Player.Name || args.Player.HasPermission(Shop2.Configs.Settings.AdminCommandPerm)) ? Shop2.Configs.Settings.Messages["ListItemsMessage6"].SFormat(item.ID, item.Stock) : ""), i, itemShown,
                                        Microsoft.Xna.Framework.Color.Gold.Hex3(), tItem.HoverName,
                                        Microsoft.Xna.Framework.Color.Magenta.Hex3(),
                                        priceShown,
                                        buyingPriceToShow.ToString());

            args.Player.SendInfoMessage(Shop2.FormatMessage("ListItemsMessage4").SFormat(TShock.Config.Settings.CommandSpecifier, Shop2.Configs.Settings.ShopCommand, Shop2.Configs.Settings.BuyItemsSubCommand));
            args.Player.SendInfoMessage(Shop2.FormatMessage("ListItemsMessage11"));
            args.Player.SendInfoMessage(Shop2.FormatMessage("ListItemsMessage5").SFormat(acc.Balance.ToString()));
        }
    }

    // source: ComfyEconomy's Utils.cs -> https://github.com/Soof4/ComfyEconomy/blob/main/ComfyEconomy/Utils.cs
    public static int GetChestIdByPos(int x, int y)
    {
        for (int i = 0; i < Main.maxChests; i++)
        {
            if (Main.chest[i] != null && Main.chest[i].x == x && Main.chest[i].y == y)
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