using TShockAPI;
using Wolfje.Plugins.SEconomy.Journal;

namespace Shop2;

public static class ConfirmCommand
{
    
    public static void Confrim(CommandArgs args, DB.ShopRegion region, IBankAccount acc)
    {
        var data = args.Player.GetData<(CommandArgs, string)>(Handler.COMFIRM_ACTION_DATA);

        if(data.Item1 == null || data.Item2 == "")
        {
            args.Player.SendInfoMessage(Shop2.FormatMessage("Confirm1"));
            return;
        }

        switch(data.Item2.ToLower())
        {
            case "buy":
                BuyCommand.Buy(data.Item1, region, acc);
                args.Player.SetData<(CommandArgs, string)>(Handler.COMFIRM_ACTION_DATA, (null, ""));
                return;

            case "sell":
                SellCommand.Sell(data.Item1, region, acc);
                args.Player.SetData<(CommandArgs, string)>(Handler.COMFIRM_ACTION_DATA, (null, ""));
                return;

            default:
                args.Player.SendInfoMessage(Shop2.FormatMessage("Confirm1"));
                return;
        }
    }
}