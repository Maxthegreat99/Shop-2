using NuGet.Protocol;
using Terraria;
using TShockAPI;
using Wolfje.Plugins.SEconomy;
using Wolfje.Plugins.SEconomy.Journal;

namespace Shop2;

public static class ModifyShopCommand
{
    
    public static void ModifyShop(CommandArgs args, DB.ShopRegion region, IBankAccount acc)
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
            args.Player.SendInfoMessage(Shop2.FormatMessage("ModifyShopMessage1").SFormat(TShock.Config.Settings.CommandSpecifier, Shop2.Configs.Settings.ShopCommand, Shop2.Configs.Settings.ModifyShopSubCommand, Shop2.Configs.Settings.CreateShopSubCommand));
            return;
        }

        var subcommand = args.Parameters[1].ToLower();

        switch (subcommand)
        {
            case "description":

                if (args.Parameters.Count < 3)
                {
                    args.Player.SendInfoMessage(Shop2.FormatMessage("ModifyShopMessage2"));
                    return;
                }

                string description = String.Join(" ", args.Parameters.GetRange(2, args.Parameters.Count - 2));

                if (description.Length > Shop2.Configs.Settings.MaxShopDescriptionCharacters)
                {
                    args.Player.SendInfoMessage(Shop2.FormatMessage("ModifyShopMessage3").SFormat(Shop2.Configs.Settings.MaxShopDescriptionCharacters));
                    return;
                }

                region.Description = description;
                DB.ModifyShopRegion(region.ID, "description", description);

                args.Player.SendInfoMessage(Shop2.FormatMessage("ModifyShopMessage4").SFormat(description));

                return;
            case "greet":

                if (args.Parameters.Count < 3)
                {
                    args.Player.SendInfoMessage(Shop2.FormatMessage("ModifyShopMessage6"));
                    return;
                }

                string greet = String.Join(" ", args.Parameters.GetRange(2, args.Parameters.Count - 2));

                if (greet.Length > Shop2.Configs.Settings.MaxShopGreetCharacters)
                {
                    args.Player.SendInfoMessage(Shop2.FormatMessage("ModifyShopMessage7").SFormat(Shop2.Configs.Settings.MaxShopGreetCharacters));
                    return;
                }

                region.Greet = greet;
                DB.ModifyShopRegion(region.ID, "greet", greet);

                args.Player.SendInfoMessage(Shop2.FormatMessage("ModifyShopMessage5").SFormat(greet));

                return;

            case "removeitem":

                if (args.Parameters.Count < 3)
                {
                    args.Player.SendInfoMessage(Shop2.FormatMessage("ModifyShopMessage8"));
                    return;
                }

                bool success = int.TryParse(args.Parameters[2], out int id);

                if (!success || region.SellingItems.FirstOrDefault(i => i.ID == id) == null)
                {
                    args.Player.SendInfoMessage(Shop2.FormatMessage("ModifyShopMessage9"));
                    return;
                }

                var item = region.SellingItems.FirstOrDefault(i => i.ID == id);

                region.SellingItems.Remove(item);
                DB.RemoveItem(id);
                DB.ModifyShopRegion(region.ID, "sellingitems", region.SellingItems.Select(i => i.ID).ToJson());


                if (item.Sold)
                {
                    args.Player.SendInfoMessage(Shop2.FormatMessage("ModifyShopMessage10").SFormat(item.ItemID, item.ID));
                    return;
                }

                if (item.Stock > 0)
                {
                    int amount = item.Stock;


                    var tItem = TShock.Utils.GetItemById(item.ItemID);

                    double _slotsToFill = amount / tItem.maxStack;
                    int slotsToFill = (int)Math.Ceiling(_slotsToFill);
                    if (amount < tItem.maxStack)
                        slotsToFill = 1;
                    for (int l = 0; l < slotsToFill; l++)
                    {
                        int amountToFill = tItem.maxStack;
                        if (l == slotsToFill - 1)
                            amountToFill = amount - (l * tItem.maxStack);

                        var titem = TShock.Utils.GetItemById(item.ItemID);
                        titem.stack = amountToFill;

                        args.Player.GiveItem(titem.netID, titem.stack);
                    }

                }

                args.Player.SendInfoMessage(Shop2.FormatMessage("ModifyShopMessage11").SFormat(item.ItemID, item.ID));
                return;

            case "deleteshop":
                args.Player.SetData(Handler.MODIFY_SHOP_CONFIRM_DATA, ("delete", Shop2.Timer, region.RegionName));

                var reg = TShock.Regions.Regions.FirstOrDefault(i => i.Name == region.RegionName);

                args.Player.SendInfoMessage(Shop2.FormatMessage("ModifyShopMessage12").SFormat(TShock.Config.Settings.CommandSpecifier, Shop2.Configs.Settings.ShopCommand, Shop2.Configs.Settings.ModifyShopSubCommand, region.RegionName, region.SellingItems.Count, reg.Area.Width * reg.Area.Height));

                return;
            case "resizeshop":

                var set1Data = args.Player.GetData<(bool, int, int)>(Handler.SET_SHOP_POINT_1_DATA);
                var set2Data = args.Player.GetData<(bool, int, int)>(Handler.SET_SHOP_POINT_2_DATA);

                if (set1Data.Item2 == -1 || set1Data.Item3 == -1 || set2Data.Item2 == -1 || set2Data.Item3 == -1)
                {
                    args.Player.SendInfoMessage(Shop2.FormatMessage("ModifyShopMessage13").SFormat(TShock.Config.Settings.CommandSpecifier, Shop2.Configs.Settings.ShopCommand, Shop2.Configs.Settings.CreateShopSubCommand));
                    return;
                }

                var x = Math.Min(set1Data.Item2, set2Data.Item2);
                var y = Math.Min(set1Data.Item3, set2Data.Item3);
                var width = Math.Abs(set1Data.Item2 - set2Data.Item2);
                var height = Math.Abs(set1Data.Item3 - set2Data.Item3);

                if (!args.Player.HasPermission(Shop2.Configs.Settings.AdminCommandPerm))
                {

                    bool collides = false;
                    foreach (var r in TShock.Regions.Regions)
                    {
                        if ((!r.HasPermissionToBuildInRegion(args.Player) || DB.regions.Any(i => i.RegionName == r.Name && i.Owner != args.Player.Name)) && Handler.CheckCollision(x, y, width, height, r.Area.X, r.Area.Y, r.Area.Width, r.Area.Height))
                        {
                            collides = true;
                            break;
                        }
                    }

                    if (Handler.CheckCollision(x, y, width, height, Main.spawnTileX - Shop2.Configs.Settings.SpawnProtectionRadius / 2, Main.spawnTileY - Shop2.Configs.Settings.SpawnProtectionRadius / 2, Shop2.Configs.Settings.SpawnProtectionRadius, Shop2.Configs.Settings.SpawnProtectionRadius))
                    {
                        args.Player.SendInfoMessage(Shop2.FormatMessage("ModifyShopMessage14"));
                        return;
                    }

                    if (collides)
                    {
                        args.Player.SendInfoMessage(Shop2.FormatMessage("ModifyShopMessage15"));
                        return;
                    }
                }

                var oldregion = TShock.Regions.Regions.First(i => i.Name == region.RegionName);

                Money price = (width * height * Shop2.Configs.Settings.CostPerBlockShop) - (oldregion.Area.Width * oldregion.Area.Height * Shop2.Configs.Settings.CostPerBlockShop);

                if (price < 0) price = 0;

                else
                {
                    if (acc.Balance < price)
                    {
                        args.Player.SendInfoMessage(Shop2.FormatMessage("ModifyShopMessage16").SFormat(price.ToString(), (width * height) - (oldregion.Area.Width * oldregion.Area.Height), ((Money)price - acc.Balance).ToString()));
                        return;
                    }

                    acc.TransferTo(SEconomyPlugin.Instance.WorldAccount, price, BankAccountTransferOptions.AnnounceToSender | BankAccountTransferOptions.IsPayment, Shop2.Configs.Settings.Messages["ModifyShopMessage17"], Shop2.Configs.Settings.Messages["ModifyShopMessage18"].SFormat((width * height) - (oldregion.Area.Width * oldregion.Area.Height), region.RegionName));
                }

                args.Player.SendInfoMessage(Shop2.FormatMessage("ModifyShopMessage19").SFormat(oldregion.Area.Width * oldregion.Area.Height, width * height, price.ToString()));
                TShock.Regions.PositionRegion(region.RegionName, x, y, width, height);

                return;

            case "transferownership":

                if (args.Parameters.Count < 3)
                {
                    args.Player.SendInfoMessage(Shop2.FormatMessage("ModifyShopMessage20"));
                    return;
                }

                var playername = args.Parameters[2];

                if (!TShock.UserAccounts.GetUserAccounts().Any(i => i.Name == playername))
                {
                    args.Player.SendInfoMessage(Shop2.FormatMessage("ModifyShopMessage21").SFormat(playername));
                    return;
                }

                args.Player.SetData<(string, long, string)>(Handler.MODIFY_SHOP_CONFIRM_DATA, ("transferownership", Shop2.Timer, playername));

                args.Player.SendInfoMessage(Shop2.FormatMessage("ModifyShopMessage22").SFormat(TShock.Config.Settings.CommandSpecifier, Shop2.Configs.Settings.ShopCommand, Shop2.Configs.Settings.ModifyShopSubCommand, region.RegionName, playername));
                return;

            case "confirm":

                var confirmData = args.Player.GetData<(string, long, string)>(Handler.MODIFY_SHOP_CONFIRM_DATA);

                if (String.IsNullOrEmpty(confirmData.Item1))
                {
                    args.Player.SendInfoMessage(Shop2.FormatMessage("ModifyShopMessage23"));
                    return;
                }

                switch (confirmData.Item1)
                {
                    case "delete":
                        if (region.RegionName != confirmData.Item3)
                        {
                            args.Player.SendInfoMessage(Shop2.FormatMessage("ModifyShopMessage23"));
                            return;
                        }

                        var items = region.SellingItems;

                        foreach (var i in items)
                        {
                            if (!i.Sold && i.Stock > 0)
                            {
                                int amount = i.Stock;


                                var tItem = TShock.Utils.GetItemById(i.ItemID);

                                double _slotsToFill = amount / tItem.maxStack;
                                int slotsToFill = (int)Math.Ceiling(_slotsToFill);
                                if (amount < tItem.maxStack)
                                    slotsToFill = 1;
                                for (int l = 0; l < slotsToFill; l++)
                                {
                                    int amountToFill = tItem.maxStack;
                                    if (l == slotsToFill - 1)
                                        amountToFill = amount - (l * tItem.maxStack);

                                    var titem = TShock.Utils.GetItemById(i.ItemID);
                                    titem.stack = amountToFill;

                                    args.Player.GiveItem(titem.netID, titem.stack);
                                }
                            }

                            DB.RemoveItem(i.ID);
                        }


                        DB.regions.Remove(region);
                        DB.RemoveShopRegion(region.ID);

                        TShock.Regions.DeleteRegion(TShock.Regions.GetRegionByName(region.RegionName).ID);

                        args.Player.SendInfoMessage(Shop2.FormatMessage("ModifyShopMessage24").SFormat(region.RegionName));

                        return;

                    case "transferownership":

                        var plyName = confirmData.Item3;

                        if (!TShock.UserAccounts.GetUserAccounts().Any(i => i.Name == plyName))
                        {
                            args.Player.SendInfoMessage(Shop2.FormatMessage("ModifyShopMessage21").SFormat(plyName));
                            return;
                        }

                        var usr = TShock.UserAccounts.GetUserAccounts().First(i => i.Name == plyName);

                        if (!TShock.Groups.groups.First(i => i.Name == usr.Group).HasPermission(Shop2.Configs.Settings.ShopOwnerPerm))
                        {
                            args.Player.SendInfoMessage(Shop2.FormatMessage("ModifyShopMessage25").SFormat(usr.Name));
                            return;
                        }

                        region.Owner = usr.Name;
                        DB.ModifyShopRegion(region.ID, "owner", usr.Name);
                        TShock.Regions.ChangeOwner(region.RegionName, usr.Name);

                        args.Player.SendInfoMessage(Shop2.FormatMessage("ModifyShopMessage26").SFormat(usr.Name, region.RegionName));

                        if (TShock.Players.Any(i => i.Name == usr.Name)) TShock.Players.First(i => i.Name == usr.Name).SendInfoMessage(Shop2.FormatMessage("ModifyShopMessage27").SFormat(region.RegionName));

                        return;
                    default:
                        args.Player.SendInfoMessage(Shop2.FormatMessage("ModifyShopMessage23"));
                        return;
                }

            case "trust":

                if (args.Parameters.Count < 3)
                {
                    args.Player.SendInfoMessage(Shop2.FormatMessage("ModifyShopMessage28"));
                    return;
                }

                var ply = TShock.Players.FirstOrDefault(i => i.Name.StartsWith(args.Parameters[2]));

                if (ply == null)
                {
                    args.Player.SendInfoMessage(Shop2.FormatMessage("ModifyShopMessage29").SFormat(args.Parameters[2]));
                    return;
                }

                if (TShock.Regions.GetRegionByName(region.RegionName).HasPermissionToBuildInRegion(ply))
                {
                    TShock.Regions.RemoveUser(region.RegionName, ply.Name);
                    args.Player.SendInfoMessage(Shop2.FormatMessage("ModifyShopMessage30").SFormat(ply.Name));
                    return;
                }
                
                TShock.Regions.AddNewUser(region.RegionName, ply.Name);
                args.Player.SendInfoMessage(Shop2.FormatMessage("ModifyShopMessage31").SFormat(ply.Name));
                return;

            default:
                args.Player.SendInfoMessage(Shop2.FormatMessage("ModifyShopMessage1").SFormat(TShock.Config.Settings.CommandSpecifier, Shop2.Configs.Settings.ShopCommand, Shop2.Configs.Settings.ModifyShopSubCommand, Shop2.Configs.Settings.CreateShopSubCommand));
                return;
        }
    }
}