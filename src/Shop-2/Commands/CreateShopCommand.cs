using Terraria;
using TShockAPI;
using Wolfje.Plugins.SEconomy;
using Wolfje.Plugins.SEconomy.Journal;

namespace Shop2;

public static class CreateShopCommand
{
    
    public static void CreateShop(CommandArgs args, IBankAccount acc)
    {
        if (!args.Player.RealPlayer)
        {
            args.Player.SendInfoMessage(Shop2.FormatMessage("NotReal"));
            return;
        }

        if (args.Parameters.Count < 2)
        {
            args.Player.SendInfoMessage(Shop2.FormatMessage("CreateShopMessage1").SFormat(TShock.Config.Settings.CommandSpecifier, Shop2.Configs.Settings.ShopCommand, Shop2.Configs.Settings.CreateShopSubCommand, Shop2.Configs.Settings.ListRegionSubCommand));
            return;
        }

        string subcommand = args.Parameters[1].ToLower();

        switch (subcommand)
        {
            case "set":
                if (args.Parameters.Count < 3 || !int.TryParse(args.Parameters[2], out int set) || set < 1 || set > 2)
                {
                    args.Player.SendInfoMessage(Shop2.FormatMessage("CreateShopMessage2"));
                    return;
                }

                if (set == 2 && ( args.Player.GetData<(bool, int, int)>(Handler.SET_SHOP_POINT_1_DATA).Item2 == -1 || args.Player.GetData<(bool, int, int)>(Handler.SET_SHOP_POINT_1_DATA).Item3 == -1))
                {
                    args.Player.SendInfoMessage(Shop2.FormatMessage("CreateShopMessage3"));
                    return;
                }

                if (set == 1)
                {
                    args.Player.SetData(Handler.SET_SHOP_POINT_2_DATA, (false, -1, -1));
                    args.Player.SetData(Handler.SET_SHOP_POINT_1_DATA, (true, -1, -1));
                    args.Player.SendInfoMessage(Shop2.FormatMessage("CreateShopMessage4"));
                    return;
                }
                else
                {
                    args.Player.SetData(Handler.SET_SHOP_POINT_2_DATA, (true, -1, -1));
                    args.Player.SendInfoMessage(Shop2.FormatMessage("CreateShopMessage5"));
                    return;
                }

            case "confirm":

                if (args.Parameters.Count < 3)
                {
                    args.Player.SendInfoMessage(Shop2.FormatMessage("CreateShopMessage1").SFormat(TShock.Config.Settings.CommandSpecifier, Shop2.Configs.Settings.ShopCommand, Shop2.Configs.Settings.CreateShopSubCommand, Shop2.Configs.Settings.ListRegionSubCommand));
                    return;
                }

                string regionName = String.Join(" ", args.Parameters.GetRange(2, args.Parameters.Count - 2));

                if (regionName.Length > Shop2.Configs.Settings.MaxShopNameCharacters)
                {
                    args.Player.SendInfoMessage(Shop2.FormatMessage("CreateShopMessage25").SFormat(Shop2.Configs.Settings.MaxShopNameCharacters));
                    return;
                }

                if (DB.regions.Any(i => i.RegionName == regionName) || TShock.Regions.Regions.Any(i => i.Name == regionName))
                {
                    args.Player.SendInfoMessage(Shop2.FormatMessage("CreateShopMessage15"));
                    return;
                }

                var set1Data = args.Player.GetData<(bool, int, int)>(Handler.SET_SHOP_POINT_1_DATA);
                var set2Data = args.Player.GetData<(bool, int, int)>(Handler.SET_SHOP_POINT_2_DATA);

                if (set1Data.Item2 == -1 || set1Data.Item3 == -1 || set2Data.Item2 == -1 || set2Data.Item3 == -1)
                {
                    args.Player.SendInfoMessage(Shop2.FormatMessage("CreateShopMessage14"));
                    return;
                }

                var x = Math.Min(set1Data.Item2, set2Data.Item2);
                var y = Math.Min(set1Data.Item3, set2Data.Item3);
                var width = Math.Abs(set1Data.Item2 - set2Data.Item2);
                var height = Math.Abs(set1Data.Item3 - set2Data.Item3);

                if (!args.Player.HasPermission(Shop2.Configs.Settings.AdminCommandPerm))
                {
                    if (DB.regions.Count(i => i.Owner == args.Player.Name) > Shop2.Configs.Settings.MaxOwnableShop)
                    {
                        args.Player.SendInfoMessage(Shop2.FormatMessage("CreateShopMessage21").SFormat(Shop2.Configs.Settings.MaxOwnableShop));
                        return;
                    }

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
                        args.Player.SendInfoMessage(Shop2.FormatMessage("CreateShopMessage16"));
                        return;
                    }

                    if (collides)
                    {
                        args.Player.SendInfoMessage(Shop2.FormatMessage("CreateShopMessage17"));
                        return;
                    }
                }

                Money price = width * height * Shop2.Configs.Settings.CostPerBlockShop;

                if (price > acc.Balance)
                {
                    args.Player.SendInfoMessage(Shop2.FormatMessage("CreateShopMessage18").SFormat(((Money)(price - acc.Balance)).ToString()));
                    return;
                }

                acc.TransferTo(SEconomyPlugin.Instance.WorldAccount, price, BankAccountTransferOptions.IsPayment | BankAccountTransferOptions.AnnounceToSender, Shop2.Configs.Settings.Messages["CreateShopMessage19"], Shop2.Configs.Settings.Messages["CreateShopMessage20"].SFormat(width * height, regionName));

                TShock.Regions.AddRegion(x, y, width, height, regionName, args.Player.Name, Main.worldID.ToString());
                DB.InsertShopRegion(regionName, args.Player.Name, new List<int>(), Shop2.Configs.Settings.Messages["CreateShopMessage23"].SFormat(args.Player.Name), Shop2.Configs.Settings.Messages["CreateShopMessage24"].SFormat(args.Player.Name));

                DB.Update();

                args.Player.SendInfoMessage(Shop2.FormatMessage("CreateShopMessage22").SFormat(regionName, width * height, TShock.Config.Settings.CommandSpecifier, Shop2.Configs.Settings.ShopCommand, Shop2.Configs.Settings.ModifyShopSubCommand));

                args.Player.SetData(Handler.SET_SHOP_POINT_2_DATA, (false, -1, -1));
                args.Player.SetData(Handler.SET_SHOP_POINT_1_DATA, (false, -1, -1));

                return;

            default:
                args.Player.SendInfoMessage(Shop2.FormatMessage("CreateShopMessage1").SFormat(TShock.Config.Settings.CommandSpecifier, Shop2.Configs.Settings.ShopCommand, Shop2.Configs.Settings.CreateShopSubCommand, Shop2.Configs.Settings.ListRegionSubCommand));
                return;
        }
    }
}