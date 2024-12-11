using NuGet.Protocol;
using Terraria;
using TShockAPI;
using Wolfje.Plugins.SEconomy;
using Wolfje.Plugins.SEconomy.Journal;

namespace Shop2;

public static class SellCommand
{
    
    public static void Sell(CommandArgs args, DB.ShopRegion region, IBankAccount acc)
    {
        if (!args.Player.RealPlayer)
        {
            args.Player.SendInfoMessage(Shop2.FormatMessage("NotReal"));
            return;
        }

        if (region == null)
        {
            args.Player.SendInfoMessage(Shop2.FormatMessage("BuyMessage1"));
            return;
        }

        if (args.Player.Name == region.Owner)
        {
            args.Player.SendInfoMessage(Shop2.FormatMessage("SellMessage1"));
            return;
        }

        if (!SEconomyPlugin.Instance.RunningJournal.BankAccounts.Any(i => i.UserAccountName == region.Owner))
        {
            args.Player.SendInfoMessage(Shop2.FormatMessage("BuyMessage8"));
            return;
        }

        if (args.Player.SelectedItem == null || !args.Player.SelectedItem.active || args.Player.SelectedItem.stack == 0)
        {
            args.Player.SendInfoMessage(Shop2.FormatMessage("SellMessage6"));
            return;
        }

        int itemID = args.Player.SelectedItem.netID;

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

        var items = (listFromCategory) ? cats.Values.ElementAt(currentCategory.Item2 - 1).FindAll(i => i.Sold) : noneCat.FindAll(i => i.Sold);

        if (noneCat == null && items.Count == 0)
        {
            args.Player.SendInfoMessage(Shop2.FormatMessage("SellMessage3"));
            return;
        }
;

        bool found = false;
        DB.SellingItem searchedItem = null;

        foreach (var item in items)
        {
            if (item.ItemID == itemID)
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
                    args.Player.SendInfoMessage(Shop2.FormatMessage("SellMessage4"));
                    return;
                }

                break;
            }
        }

        if (!found)
        {
            args.Player.SendInfoMessage(Shop2.FormatMessage("SellMessage5"));
            return;
        }

        int amount = args.Player.SelectedItem.stack;

        if (amount > searchedItem.Stock) amount = searchedItem.Stock;

        if (SEconomyPlugin.Instance.RunningJournal.BankAccounts.First(i => i.UserAccountName == region.Owner).Balance < searchedItem.Price * amount)
        {
            args.Player.SendInfoMessage(Shop2.FormatMessage("SellMessage7"));
            return;
        }

        if (searchedItem.ChestPosY == 0 && searchedItem.ChestPosX == 0)
        {
            args.Player.SendInfoMessage(Shop2.FormatMessage("SellMessage8"));
            return;
        }

        if (!TShock.Regions.InAreaRegionName(searchedItem.ChestPosX, searchedItem.ChestPosY).Contains(region.RegionName))
        {
            args.Player.SendInfoMessage(Shop2.FormatMessage("SellMessage15"));
            return;
        }

        int chest = StoreCommand.GetChestIdByPos(searchedItem.ChestPosX, searchedItem.ChestPosY);

        if (chest == -1)
        {
            args.Player.SendInfoMessage(Shop2.FormatMessage("SellMessage9"));
            return;
        }

        int chestSlot = StoreCommand.GetFreeSlotInChest(chest);

        if (chestSlot == -1)
        {
            args.Player.SendInfoMessage(Shop2.FormatMessage("SellMessage10"));
            return;
        }

        bool isSSC = Main.ServerSideCharacter;

        if (!isSSC)
        {
            Main.ServerSideCharacter = true;
            NetMessage.SendData(7, args.Player.Index, -1, null, 0, 0.0f, 0.0f, 0.0f, 0, 0, 0);
            args.Player.IgnoreSSCPackets = true;
        }

        int InvItem = Array.IndexOf(args.Player.TPlayer.inventory, args.Player.SelectedItem);

        StoreCommand.AddItemToChest(chest, chestSlot, args.Player.SelectedItem.netID, amount, args.Player.SelectedItem.prefix);
        args.TPlayer.inventory[InvItem].stack -= amount;

        NetMessage.SendData((int)PacketTypes.PlayerSlot, -1, -1, null, args.Player.Index, InvItem, args.TPlayer.inventory[InvItem].stack, args.TPlayer.inventory[InvItem].prefix, args.TPlayer.inventory[InvItem].netID, 0, 0);

        if (!isSSC)
        {
            Main.ServerSideCharacter = false;
            NetMessage.SendData(7, args.Player.Index, -1, null, 0, 0.0f, 0.0f, 0.0f, 0, 0, 0);
            args.Player.IgnoreSSCPackets = false;
        }

        SEconomyPlugin.Instance.RunningJournal.BankAccounts.First(i => i.UserAccountName == region.Owner).TransferTo(acc, amount * searchedItem.Price, Wolfje.Plugins.SEconomy.Journal.BankAccountTransferOptions.AnnounceToSender | Wolfje.Plugins.SEconomy.Journal.BankAccountTransferOptions.IsPlayerToPlayerTransfer,
               Shop2.Configs.Settings.Messages["SellMessage11"], string.Format(Shop2.Configs.Settings.Messages["SellMessage12"], amount, Terraria.Lang.GetItemNameValue(searchedItem.ItemID)));

        args.Player.SendSuccessMessage(Shop2.FormatMessage("SellMessage13").SFormat(amount, searchedItem.ItemID, ((Money)(amount * searchedItem.Price)).ToString()));

        args.Player.SendInfoMessage(Shop2.FormatMessage("ListItemsMessage5").SFormat(acc.Balance.ToString()));

        TShock.Log.Write(Shop2.FormatMessage("SellMessage14").SFormat(args.Player.Name, amount, Terraria.Lang.GetItemNameValue(searchedItem.ItemID), region.RegionName, amount * searchedItem.Price), System.Diagnostics.TraceLevel.Info);

        searchedItem.Stock -= amount;

        if (searchedItem.Stock < 1)
        {

            region.SellingItems.Remove(searchedItem);
            DB.RemoveItem(searchedItem.ID);
            DB.ModifyShopRegion(region.ID, "sellingitems", region.SellingItems.Select(i => i.ID).ToJson());

        }
        else
        {
            DB.ModifyItem(searchedItem.ID, "stock", searchedItem.Stock);
            DB.regions.Remove(region);
            DB.regions.Add(DB.GetUpdatedRegion(region));
        }
    }
}