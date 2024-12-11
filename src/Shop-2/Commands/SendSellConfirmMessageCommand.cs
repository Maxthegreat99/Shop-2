using TShockAPI;
using Wolfje.Plugins.SEconomy;
using Wolfje.Plugins.SEconomy.Journal;

namespace Shop2;

public class SendSellConfirmMessageCommand
{
    public static void SendSellConfirmMessage(CommandArgs args, DB.ShopRegion region, IBankAccount acc)
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

        args.Player.SendInfoMessage(Shop2.FormatMessage("ConfirmSellMessage5").SFormat(searchedItem.ItemID));

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

        args.Player.SetData(Handler.COMFIRM_ACTION_DATA, (args, "sell"));
        args.Player.SendInfoMessage(Shop2.FormatMessage("ConfirmSellMessage6").SFormat(TShock.Config.Settings.CommandSpecifier, Shop2.Configs.Settings.ShopCommand, Shop2.Configs.Settings.ConfirmSubCommand, amount, searchedItem.ItemID));
    }
}