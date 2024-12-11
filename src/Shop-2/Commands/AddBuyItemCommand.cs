using NuGet.Protocol;
using Terraria;
using TShockAPI;
using Wolfje.Plugins.SEconomy;

namespace Shop2;

public static class AddBuyItemCommand
{
    
    public static void AddBuyItem(CommandArgs args, DB.ShopRegion region)
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

        if (args.Player.SelectedItem == null || !args.Player.SelectedItem.active || args.Player.SelectedItem.stack == 0)
        {
            args.Player.SendInfoMessage(Shop2.FormatMessage("AddBuyMessage2"));
            return;
        }

        int itemID = args.Player.SelectedItem.netID;

        bool isItemAlreadyAdded = false;

        if (region.SellingItems.FindAll(i => !i.Sold).Any(i => i.ItemID == itemID))
            isItemAlreadyAdded = true;

        if (!isItemAlreadyAdded && args.Parameters.Count < 2)
        {
            args.Player.SendInfoMessage(Shop2.FormatMessage("AddBuyMessage3"));
            return;
        }

        if (!isItemAlreadyAdded && args.Player.HasPermission(Shop2.Configs.Settings.AdminCommandPerm) && region.SellingItems.Count > Shop2.Configs.Settings.MaxShopItems)
        {
            args.Player.SendInfoMessage(Shop2.FormatMessage("AddBuyMessage8"));
            return;
        }

        if (isItemAlreadyAdded)
        {
            int amount = args.Player.SelectedItem.stack;

            int InvItem = Array.IndexOf(args.Player.TPlayer.inventory, args.Player.SelectedItem);

            bool isSSC = Main.ServerSideCharacter;

            if (!isSSC)
            {
                Main.ServerSideCharacter = true;
                NetMessage.SendData(7, args.Player.Index, -1, null, 0, 0.0f, 0.0f, 0.0f, 0, 0, 0);
                args.Player.IgnoreSSCPackets = true;
            }

            args.TPlayer.inventory[InvItem].stack = 0;

            NetMessage.SendData((int)PacketTypes.PlayerSlot, -1, -1, null, args.Player.Index, InvItem, args.TPlayer.inventory[InvItem].stack, args.TPlayer.inventory[InvItem].prefix, args.TPlayer.inventory[InvItem].netID, 0, 0);

            if (!isSSC)
            {
                Main.ServerSideCharacter = false;
                NetMessage.SendData(7, args.Player.Index, -1, null, 0, 0.0f, 0.0f, 0.0f, 0, 0, 0);
                args.Player.IgnoreSSCPackets = false;
            }

            var sellingItem = region.SellingItems.First(i => i.ItemID == itemID && !i.Sold);
            sellingItem.Stock += amount;

            DB.ModifyItem(sellingItem.ID, "stock", sellingItem.Stock);
            DB.regions.Remove(region);
            DB.regions.Add(DB.GetUpdatedRegion(region));

            args.Player.SendInfoMessage(Shop2.FormatMessage("AddBuyMessage4").SFormat(sellingItem.ItemID, sellingItem.Stock, sellingItem.ID));
            return;
        }
        else
        {
            Money price = 0;

            bool success = Money.TryParse(args.Parameters[1], out price);

            if (!success || price < 1)
            {
                args.Player.SendInfoMessage(Shop2.FormatMessage("AddBuyMessage5"));
                return;
            }

            string category = "";

            if (args.Parameters.ElementAtOrDefault(2) != null)
                category = args.Parameters[2];

            int amount = args.Player.SelectedItem.stack;

            int InvItem = Array.IndexOf(args.Player.TPlayer.inventory, args.Player.SelectedItem);

            bool isSSC = Main.ServerSideCharacter;

            if (!isSSC)
            {
                Main.ServerSideCharacter = true;
                NetMessage.SendData(7, args.Player.Index, -1, null, 0, 0.0f, 0.0f, 0.0f, 0, 0, 0);
                args.Player.IgnoreSSCPackets = true;
            }

            args.TPlayer.inventory[InvItem].stack = 0;

            NetMessage.SendData((int)PacketTypes.PlayerSlot, -1, -1, null, args.Player.Index, InvItem, args.TPlayer.inventory[InvItem].stack, args.TPlayer.inventory[InvItem].prefix, args.TPlayer.inventory[InvItem].netID, 0, 0);

            if (!isSSC)
            {
                Main.ServerSideCharacter = false;
                NetMessage.SendData(7, args.Player.Index, -1, null, 0, 0.0f, 0.0f, 0.0f, 0, 0, 0);
                args.Player.IgnoreSSCPackets = false;
            }

            int ID = DB.InsertItem((int)price, new List<int>(), itemID, amount, 0, 0, category, 0, 0, 0, 0, false);
            region.SellingItems.Add(DB.GetItem(ID));
            DB.ModifyShopRegion(region.ID, "sellingitems", region.SellingItems.Select(i2 => i2.ID).ToJson());

            args.Player.SendInfoMessage(Shop2.FormatMessage("AddBuyMessage6").SFormat(amount, itemID, price.ToString(), ID));
            args.Player.SendInfoMessage(Shop2.FormatMessage("AddBuyMessage7").SFormat(TShock.Config.Settings.CommandSpecifier, Shop2.Configs.Settings.ShopCommand, Shop2.Configs.Settings.ModifyItemsSubCommand));
            return;
        }
    }
}