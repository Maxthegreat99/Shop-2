using Terraria;
using TShockAPI;
using Wolfje.Plugins.SEconomy;
using Wolfje.Plugins.SEconomy.Journal;

namespace Shop2;

public static class BuyCommand
{
    
    public static void Buy(CommandArgs args, DB.ShopRegion region, IBankAccount acc)
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

        var cats = StoreCommand.CategorizeItems(region);

        List<DB.SellingItem> noneCat = null;

        if (cats.ContainsKey("none"))
        {
            noneCat = cats["none"];
            cats.Remove("none");
        }

        if (!listFromCategory && noneCat == null)
        {
            args.Player.SendInfoMessage(Shop2.FormatMessage("BuyMessage22"));
            return;
        }

        var items = (listFromCategory) ? cats.Values.ElementAt(currentCategory.Item2 - 1).FindAll(i => !i.Sold) : noneCat.FindAll(i => !i.Sold);

        if (noneCat == null && items.Count == 0)
        {
            args.Player.SendInfoMessage(Shop2.FormatMessage("BuyMessage19"));
            return;
        }

        int i = 0;

        bool found = false;
        DB.SellingItem searchedItem = null;

        foreach (var item in items)
        {
            i++;

            Item tItem = TShock.Utils.GetItemById(item.ItemID);

            if (i == index || (inputName != null && (Terraria.Lang.GetItemNameValue(item.ID).StartsWith(inputName)|| tItem.HoverName.ToLower().StartsWith(inputName.ToLower()) || tItem.Name.ToLower().StartsWith(inputName.ToLower()))))
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

        bool itemsGottenFromChest = false;

        List<Item> itemsToGive = new();
        if (searchedItem.Stock == 0)
        {
            if (searchedItem.ChestPosY == 0 && searchedItem.ChestPosX == 0)
            {
                args.Player.SendInfoMessage(Shop2.FormatMessage("BuyMessage9"));
                return;
            }

            if (!TShock.Regions.InAreaRegionName(searchedItem.ChestPosX, searchedItem.ChestPosY).Contains(region.RegionName))
            {
                args.Player.SendInfoMessage(Shop2.FormatMessage("BuyMessage20"));
                return;
            }

            int chest = StoreCommand.GetChestIdByPos(searchedItem.ChestPosX, searchedItem.ChestPosY);

            if (chest == -1)
            {
                args.Player.SendInfoMessage(Shop2.FormatMessage("BuyMessage10"));
                return;
            }

            int stock = 0;
            
            foreach (var k in Main.chest[chest].item)
                if (k.netID == searchedItem.ItemID && k.stack > 0 && k.active != false) stock += k.stack;
                
            if (stock == 0)
            {
                args.Player.SendInfoMessage(Shop2.FormatMessage("BuyMessage9"));
                return;
            }

            if (stock < amount) amount = stock;

            if (searchedItem.Price * amount > acc.Balance)
            {
                args.Player.SendInfoMessage(Shop2.FormatMessage("BuyMessage6").SFormat(((Money)(searchedItem.Price * amount - acc.Balance)).ToString(), "[i:{0}]".SFormat(searchedItem.ItemID), amount));
                return;
            }

            if (searchedItem.PriceItemID != 0 && searchedItem.PriceItemAmount != 0)
            {
                if (searchedItem.PriceChestPosX == 0 && searchedItem.PriceChestPosY == 0)
                {
                    args.Player.SendInfoMessage(Shop2.FormatMessage("BuyMessage12"));
                    return;
                }

                if (!TShock.Regions.InAreaRegionName(searchedItem.PriceChestPosX, searchedItem.PriceChestPosY).Contains(region.RegionName))
                {
                    args.Player.SendInfoMessage(Shop2.FormatMessage("BuyMessage21"));
                    return;
                }

                int priceChest = StoreCommand.GetChestIdByPos(searchedItem.PriceChestPosX, searchedItem.PriceChestPosY);

                if (priceChest == -1)
                {
                    args.Player.SendInfoMessage(Shop2.FormatMessage("BuyMessage13"));
                    return;
                }

                int chestSlot = StoreCommand.GetFreeSlotInChest(priceChest);

                if (chestSlot == -1)
                {
                    args.Player.SendInfoMessage(Shop2.FormatMessage("BuyMessage14"));
                    return;
                }

                if (args.Player.SelectedItem == null || !args.Player.SelectedItem.active || args.Player.SelectedItem.netID != searchedItem.PriceItemID || args.Player.SelectedItem.stack < searchedItem.PriceItemAmount * amount)
                {
                    args.Player.SendInfoMessage(Shop2.FormatMessage("BuyMessage11"));
                    return;
                }

                int InvItem = Array.IndexOf(args.Player.TPlayer.inventory, args.Player.SelectedItem);

                bool isSSC = Main.ServerSideCharacter;

                if (!isSSC)
                {
                    Main.ServerSideCharacter = true;
                    NetMessage.SendData(7, args.Player.Index, -1, null, 0, 0.0f, 0.0f, 0.0f, 0, 0, 0);
                    args.Player.IgnoreSSCPackets = true;
                }

                StoreCommand.AddItemToChest(priceChest, chestSlot, args.Player.SelectedItem.netID, searchedItem.PriceItemAmount * amount, args.Player.SelectedItem.prefix);
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
                var l = Main.chest[chest].item.First(i => i.netID == searchedItem.ItemID && i.active && i.stack > 0);

                var j = TShock.Utils.GetItemById(l.netID);
                j.stack = l.stack;

                if (j.stack > _amount) j.stack = _amount;
                j.Prefix(l.prefix);

                itemsToGive.Add(j);

                _amount -= j.stack;

                StoreCommand.DeleteItemsFromChest(chest, j.netID, j.stack);
            }

            itemsGottenFromChest = true;
        }
        else
        {
            if (searchedItem.Stock < amount) amount = searchedItem.Stock;

            if (searchedItem.Price * amount > acc.Balance)
            {
                args.Player.SendInfoMessage(Shop2.FormatMessage("BuyMessage6").SFormat(((Money)(searchedItem.Price * amount - acc.Balance)).ToString(), "[i:{0}]".SFormat(searchedItem.ItemID), amount));
                return;
            }

            if (searchedItem.PriceItemID != 0 && searchedItem.PriceItemAmount != 0)
            {
                if (searchedItem.PriceChestPosX == 0 && searchedItem.PriceChestPosY == 0)
                {
                    args.Player.SendInfoMessage(Shop2.FormatMessage("BuyMessage12"));
                    return;
                }

                if (!TShock.Regions.InAreaRegionName(searchedItem.PriceChestPosX, searchedItem.PriceChestPosY).Contains(region.RegionName))
                {
                    args.Player.SendInfoMessage(Shop2.FormatMessage("BuyMessage21"));
                    return;
                }

                int priceChest = StoreCommand.GetChestIdByPos(searchedItem.PriceChestPosX, searchedItem.PriceChestPosY);

                if (priceChest == -1)
                {
                    args.Player.SendInfoMessage(Shop2.FormatMessage("BuyMessage13"));
                    return;
                }

                int chestSlot = StoreCommand.GetFreeSlotInChest(priceChest);

                if (chestSlot == -1)
                {
                    args.Player.SendInfoMessage(Shop2.FormatMessage("BuyMessage14"));
                    return;
                }

                if (args.Player.SelectedItem == null || !args.Player.SelectedItem.active || args.Player.SelectedItem.netID != searchedItem.PriceItemID || args.Player.SelectedItem.stack < searchedItem.PriceItemAmount * amount)
                {
                    args.Player.SendInfoMessage(Shop2.FormatMessage("BuyMessage11"));
                    return;
                }

                int InvItem = Array.IndexOf(args.Player.TPlayer.inventory, args.Player.SelectedItem);

                bool isSSC = Main.ServerSideCharacter;

                if (!isSSC)
                {
                    Main.ServerSideCharacter = true;
                    NetMessage.SendData(7, args.Player.Index, -1, null, 0, 0.0f, 0.0f, 0.0f, 0, 0, 0);
                    args.Player.IgnoreSSCPackets = true;
                }

                StoreCommand.AddItemToChest(priceChest, chestSlot, args.Player.SelectedItem.netID, searchedItem.PriceItemAmount * amount, args.Player.SelectedItem.prefix);
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
                titem.stack = amountToFill;

                itemsToGive.Add(titem);
            }
        }

        acc.TransferTo(SEconomyPlugin.Instance.RunningJournal.BankAccounts.First(i => i.UserAccountName == region.Owner), amount * searchedItem.Price, Wolfje.Plugins.SEconomy.Journal.BankAccountTransferOptions.AnnounceToSender | Wolfje.Plugins.SEconomy.Journal.BankAccountTransferOptions.IsPayment,
                       Shop2.Configs.Settings.Messages["BuyMessage15"], string.Format(Shop2.Configs.Settings.Messages["BuyMessage16"], amount, Terraria.Lang.GetItemNameValue(searchedItem.ItemID)));

        foreach (var l in itemsToGive)
            args.Player.GiveItem(l.netID, l.stack, l.prefix);

        args.Player.SendSuccessMessage(Shop2.FormatMessage("BuyMessage17").SFormat(amount, searchedItem.ItemID, ((Money)(amount * searchedItem.Price)).ToString()) + ((searchedItem.PriceItemID != 0) ? $" + {searchedItem.PriceItemAmount * amount} [i:{searchedItem.PriceItemID}]" : ""));

        args.Player.SendInfoMessage(Shop2.FormatMessage("ListItemsMessage5").SFormat(acc.Balance.ToString()));

        TShock.Log.Write(Shop2.FormatMessage("BuyMessage18").SFormat(args.Player.Name, amount, Terraria.Lang.GetItemNameValue(searchedItem.ItemID), region.RegionName, amount * searchedItem.Price), System.Diagnostics.TraceLevel.Info);

        if (itemsGottenFromChest) return;

        searchedItem.Stock -= amount;

        DB.ModifyItem(searchedItem.ID, "stock", searchedItem.Stock);
        DB.regions.Remove(region);
        DB.regions.Add(DB.GetUpdatedRegion(region));
    }

}