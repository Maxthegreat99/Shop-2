using IL.Terraria;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using Wolfje.Plugins.SEconomy;
using Main = Terraria.Main;

namespace Shop2
{
    public class Handler
    {
        public const string CATEGORY_DATA = "shopcategory";
        public const string PLACE_SHOP_CHEST_DATA = "placeshopchest";
        public const string PLACE_SHOP_PRICE_CHEST_DATA = "placepriceshopchest";
        public const string SET_SHOP_POINT_1_DATA = "setshop1";
        public const string SET_SHOP_POINT_2_DATA = "setshop2";

        public static void OnNetGreet(GreetPlayerEventArgs args)
        {
            if (TShock.Players.ElementAtOrDefault(args.Who) == null)
            {
                return;
            }

            TShock.Players.ElementAt(args.Who).SetData(CATEGORY_DATA, ("", 0));
            TShock.Players.ElementAt(args.Who).SetData(PLACE_SHOP_CHEST_DATA, (-1, ""));
            TShock.Players.ElementAt(args.Who).SetData(PLACE_SHOP_PRICE_CHEST_DATA, (-1, -1, -1, ""));
            TShock.Players.ElementAt(args.Who).SetData(SET_SHOP_POINT_1_DATA, (false, -1, -1));
            TShock.Players.ElementAt(args.Who).SetData(SET_SHOP_POINT_2_DATA, ("",false, -1, -1));
        }

        public static void OnGetData(GetDataEventArgs args)
        {
            if (args.MsgID != PacketTypes.PlaceChest && args.MsgID != PacketTypes.Tile) return;

            if (args.MsgID == PacketTypes.PlaceChest)
            {
                var flag = args.Msg.readBuffer[args.Index];
                int X = BitConverter.ToInt16(args.Msg.readBuffer, args.Index + 1);
                int Y = BitConverter.ToInt16(args.Msg.readBuffer, args.Index + 3);
                int style = BitConverter.ToInt16(args.Msg.readBuffer, args.Index + 5);
                var style2 = (byte)style;
                var player = TShock.Players.ElementAtOrDefault(args.Msg.whoAmI);

                if (player == null) return;

                if (flag != 0) return;

                var placeChestData = player.GetData<(int, string)>(PLACE_SHOP_CHEST_DATA);
                var placePriceChestData = player.GetData<(int, int, int, string)>(PLACE_SHOP_PRICE_CHEST_DATA);

                if ((placeChestData.Item1 == -1 || placeChestData.Item2 == "") && (placePriceChestData.Item1 == -1 || placePriceChestData.Item2 == -1 || placePriceChestData.Item3 == -1 || placePriceChestData.Item4 == "")) return;

                bool isPlaceChestValid = !(placeChestData.Item1 == -1 || placeChestData.Item2 == "");

                if (isPlaceChestValid)
                {
                    var reg = DB.regions.FirstOrDefault(i => i.RegionName == placeChestData.Item2);

                    if (reg == null || reg.Owner != player.Name)
                    {
                        player.SetData(PLACE_SHOP_CHEST_DATA, (-1, ""));
                        return;
                    }

                    if (TShock.Regions.InAreaRegionName(X, Y).FirstOrDefault(i => i == placeChestData.Item2) == null) return;

                    var item = reg.SellingItems.FirstOrDefault(i => i.ID == placeChestData.Item1);
                    if (item == null)
                    {
                        player.SetData(PLACE_SHOP_CHEST_DATA, (-1, ""));
                        return;
                    }

                    if (X == 0 || Y == 0) return;

                    item.ChestPosX = X;
                    item.ChestPosY = Y;

                    DB.ModifyItem(item.ID, "chestposx", X);
                    DB.ModifyItem(item.ID, "chestposy", Y);

                    player.SetData(PLACE_SHOP_CHEST_DATA, (-1, ""));

                    player.SendInfoMessage(Shop2.FormatMessage("ModifyItemMessage9").SFormat(item.ItemID, item.ID, X, Y));

                }
                else
                {
                    var reg = DB.regions.FirstOrDefault(i => i.RegionName == placePriceChestData.Item4);

                    if (reg == null || reg.Owner != player.Name)
                    {
                        player.SetData(PLACE_SHOP_PRICE_CHEST_DATA, (-1, -1, -1, ""));
                        return;
                    }

                    if (TShock.Regions.InAreaRegionName(X, Y).FirstOrDefault(i => i == placePriceChestData.Item4) == null) return;

                    var item = reg.SellingItems.FirstOrDefault(i => i.ID == placePriceChestData.Item3);
                    if (item == null)
                    {
                        player.SetData(PLACE_SHOP_PRICE_CHEST_DATA, (-1, -1, -1, ""));
                        return;
                    }

                    if (X == 0 || Y == 0) return;

                    item.ChestPosX = X;
                    item.ChestPosY = Y;
                    item.PriceItemID = placePriceChestData.Item1;
                    item.PriceItemAmount = placePriceChestData.Item2;

                    DB.ModifyItem(item.ID, "pricechestposx", X);
                    DB.ModifyItem(item.ID, "pricechestposy", Y);
                    DB.ModifyItem(item.ID, "priceitemid", placePriceChestData.Item1);
                    DB.ModifyItem(item.ID, "priceitemamount", placePriceChestData.Item2);



                    player.SendInfoMessage(Shop2.FormatMessage("ModifyItemMessage16").SFormat(item.ItemID, item.ID, placePriceChestData.Item2, placePriceChestData.Item1, X, Y));

                    player.SetData(PLACE_SHOP_PRICE_CHEST_DATA, (-1, -1, -1, ""));
                }

                return;
            }

            int X1 = BitConverter.ToInt16(args.Msg.readBuffer, args.Index + 1);
            int Y1 = BitConverter.ToInt16(args.Msg.readBuffer, args.Index + 3);

            var ply = TShock.Players.ElementAtOrDefault(args.Msg.whoAmI);

            if (X1 == 0 || Y1 == 0 || ply == null) return;

            var set1Data = ply.GetData<(bool, int, int)>(SET_SHOP_POINT_1_DATA);
            var set2Data = ply.GetData<(string,bool, int, int)>(SET_SHOP_POINT_2_DATA);

            if (!set1Data.Item1 && !set2Data.Item2) return;

            if (set1Data.Item1)
            {
                ply.SetData(SET_SHOP_POINT_1_DATA, (false, X1, Y1));
                ply.SetData(SET_SHOP_POINT_2_DATA, ("",false, -1, -1));
                ply.SendInfoMessage(Shop2.FormatMessage("CreateShopMessage6").SFormat(X1, Y1));
                return;
            }

            var x = Math.Min(set1Data.Item2, X1);
            var y = Math.Min(set1Data.Item3, Y1);
            var width = Math.Abs(set1Data.Item2 - X1);
            var height = Math.Abs(set1Data.Item3 - Y1);

            if (!ply.HasPermission(Shop2.Configs.Settings.AdminCommandPerm))
            {
                var area = width * height;

                if (area > Shop2.Configs.Settings.MaxShopArea)
                {
                    ply.SendInfoMessage(Shop2.FormatMessage("CreateShopMessage7").SFormat(Shop2.Configs.Settings.MaxShopArea, area));
                    return;
                }

                if (area < Shop2.Configs.Settings.MinShopArea)
                {
                    ply.SendInfoMessage(Shop2.FormatMessage("CreateShopMessage8").SFormat(Shop2.Configs.Settings.MinShopArea, area));
                    return;
                }


                bool collides = false;
                foreach (var r in TShock.Regions.Regions)
                {
                    if ((!r.HasPermissionToBuildInRegion(ply) || DB.regions.Any(i => i.RegionName == r.Name)) && CheckCollision(x, y, width, height, r.Area.X, r.Area.Y, r.Area.Width, r.Area.Height))
                    {
                        collides = true;
                        break;
                    }
                }

                if (CheckCollision(x, y, width, height, Main.spawnTileX - Shop2.Configs.Settings.SpawnProtectionRadius / 2 ,Main.spawnTileY - Shop2.Configs.Settings.SpawnProtectionRadius / 2, Shop2.Configs.Settings.SpawnProtectionRadius, Shop2.Configs.Settings.SpawnProtectionRadius ))
                {
                    ply.SendInfoMessage(Shop2.FormatMessage("CreateShopMessage9"));
                    return;
                }

                if (collides)
                {
                    ply.SendInfoMessage(Shop2.FormatMessage("CreateShopMessage10"));
                    return;
                }
            }


            ply.SetData(SET_SHOP_POINT_2_DATA, (set2Data.Item1, false, X1, Y1));

            ply.SendInfoMessage(Shop2.FormatMessage("CreateShopMessage11").SFormat(X1, Y1, height * width));

            if (set2Data.Item1 != "" && TShock.Regions.GetRegionByName(set2Data.Item1) != null)
            {
                var r = TShock.Regions.GetRegionByName(set2Data.Item1);

                int area = r.Area.Width * r.Area.Height;

                Money Price = (width * height * Shop2.Configs.Settings.CostPerBlockShop) - (area * Shop2.Configs.Settings.CostPerBlockShop);

                if (Price < 0) Price = 0;

                ply.SendInfoMessage(Shop2.FormatMessage("CreateShopMessage13").SFormat(Price.ToString()));
                return;
            }

            Money price = width * height * Shop2.Configs.Settings.CostPerBlockShop;

            ply.SendInfoMessage(Shop2.FormatMessage("CreateShopMessage12").SFormat(price.ToString()));


        }

        public static bool CheckCollision(int x, int y, int w, int h, int x2, int y2, int w2, int h2)
        {

            return (x < x2 + w2
                && x + w > x2
                && y < y2 + h2
                && y + h > y2);

        }
    }
} 