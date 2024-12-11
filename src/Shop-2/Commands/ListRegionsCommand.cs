using TShockAPI;

namespace Shop2;

public class ListRegionsCommand
{
    
    public static void ListRegions(CommandArgs args)
    {
        int page = 1;
        if (args.Parameters.Count > 1)
        {
            try
            {
                page = int.Parse(args.Parameters[1]);
            }
            catch (Exception e)
            {
                page = 1;
            }
        }

        if (page < 1) page = 1;

        List<string> list = new List<string>();

        foreach (var shop in DB.regions)
        {
            string str = Shop2.Configs.Settings.Messages["ListRegionsMessage2"].SFormat(shop.RegionName, shop.Description, shop.Owner);

            list.Add(str);
        }

        PaginationTools.SendPage(args.Player, page, list,
            new PaginationTools.Settings()
            {
                HeaderFormat = Shop2.Configs.Settings.Messages["ListRegionsMessage1"],
                FooterFormat = Shop2.Configs.Settings.Messages["ListRegionsMessage3"].SFormat(TShock.Config.Settings.CommandSpecifier, Shop2.Configs.Settings.ShopCommand, Shop2.Configs.Settings.ListRegionSubCommand),
                NothingToDisplayString = Shop2.Configs.Settings.Messages["ListRegionsMessage4"]
            });
    }

}