using Terraria;
using TShockAPI;
using Wolfje.Plugins.SEconomy;
using Wolfje.Plugins.SEconomy.Journal;

namespace Shop2;

public static class SendBuyConfirmMessageCommand
{
    // performs the requirement checks for the item and sends to the player a confirmation for the player to see if the item they typed was infact what they were trying to buy
    public static void SendBuyConfirmMessage(CommandArgs args, DB.ShopRegion region, IBankAccount acc)
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

            if (i == index || (inputName != null && (Terraria.Lang.GetItemNameValue(item.ID).StartsWith(inputName) || tItem.HoverName.ToLower().StartsWith(inputName.ToLower()) || tItem.Name.ToLower().StartsWith(inputName.ToLower()))))
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

        args.Player.SendInfoMessage(Shop2.FormatMessage("ConfirmBuyMessage1").SFormat(searchedItem.ItemID));

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

            if (stock < amount)
            {
                amount = stock;
                args.Player.SendInfoMessage(Shop2.FormatMessage("ConfirmBuyMessage3"));
            }
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

            }
        }
        else
        {
            if (searchedItem.Stock < amount)
            {
                amount = searchedItem.Stock;
                args.Player.SendInfoMessage(Shop2.FormatMessage("ConfirmBuyMessage2"));
            }
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
            }

        }

        args.Player.SetData(Handler.COMFIRM_ACTION_DATA, (args, "buy"));
        args.Player.SendInfoMessage(Shop2.FormatMessage("ConfirmBuyMessage4").SFormat(TShock.Config.Settings.CommandSpecifier, Shop2.Configs.Settings.ShopCommand, Shop2.Configs.Settings.ConfirmSubCommand, amount, searchedItem.ItemID, ((Money)(searchedItem.Price * amount)).ToString()));
    }
}