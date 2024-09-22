using TerrariaApi.Server;
using TShockAPI;

namespace Shop2
{
    public class Handler
    {
        public const string CATEGORY_DATA = "shopcategory";
        public const string PLACE_SHOP_CHEST_DATA = "placeshopchest";

        public static void OnNetGreet(GreetPlayerEventArgs args)
        {
            if (TShock.Players.ElementAtOrDefault(args.Who) == null)
            {
                return;
            }

            TShock.Players.ElementAt(args.Who).SetData(CATEGORY_DATA, ("", 0));
            TShock.Players.ElementAt(args.Who).SetData(PLACE_SHOP_CHEST_DATA, (-1, ""));
        }

        public static void OnGetData(GetDataEventArgs args)
        {
            if (args.MsgID != PacketTypes.PlaceChest) return;

            var flag = args.Msg.readBuffer[args.Index];
            int X = BitConverter.ToInt16(args.Msg.readBuffer, args.Index + 1);
            int Y = BitConverter.ToInt16(args.Msg.readBuffer, args.Index + 3);
            int style = BitConverter.ToInt16(args.Msg.readBuffer, args.Index + 5);
            var style2 = (byte)style;
            var player = TShock.Players.ElementAtOrDefault(args.Msg.whoAmI);

            if (player == null) return;

            if (flag != 0) return;

            var placeChestData = player.GetData<(int, string)>(PLACE_SHOP_CHEST_DATA);

            if (placeChestData.Item1 == -1 || placeChestData.Item2 == "") return;
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
    }
} 