using TShockAPI.Configuration;

namespace Shop2;

public class Config
{
    public Dictionary<string, string> Messages = new Dictionary<string, string>();
    public string ShopCommand = string.Empty;
    public string ListRegionSubCommand = string.Empty;
    public string ListItemsSubCommand = string.Empty;
    public string BuyItemsSubCommand = string.Empty;
    public string CategorySubCommand = string.Empty;
    public string ShopPerm = string.Empty;
    public string PlayerCommandPerm = string.Empty;
    public string AdminCommandPerm = string.Empty;
}

public class ShopConfig : ConfigFile<Config>
{ }

public class Lang
{
    private static string _shopCommand = "store";
    private static string _listRegsCommand = "listregions";
    private static string _listItemsCommand = "listitems";
    private static string _buyItemsCommand = "buy";
    private static string _catCommand = "category";
    private static string _shopPerm = "shop2.shop";
    private static string _playerCommandsPerm = "shop2.player";
    private static string _adminCommandPerm = "shop2.admin";

    private static Dictionary<string, string> _message = new Dictionary<string, string>
    {
        {"NoAcc", "Couldn't access your bank account!"},
        {"NoPerm", "You dont have the permission to use this command!" },
        {"PlayerCommandHelp", "Store Command Usage:\n {0}{1} {2} (page) - lists the shops currently open in the world"},
        {"PlayerCommandHelp1", "{0}{1} {2} - lists the items in the shop you are currently in"},
        {"NotInAShop", "You are currently not in a shop!"},
        {"ListRegionsMessage1", "Shops ({0}/{1}):" },
        {"ListRegionsMessage2","【{0}】 Description: [c/008000:{1}]\nOwned by: [c/008000:{2}]" },
        {"ListRegionsMessage3","Type {0}{1} {2} {{0}} for more." },
        {"ListRegionsMessage4","There are no shops created yet." },
        {"ListItemsMessage1", "These are the items and categories currently present in 【[c/008000:{0}]】" },
        {"ListItemsMessage2", "These are the items from the category 【[c/008000:{0}]】 currently present in 【[c/008000:{1}]】" },
        {"ListItemsMessage3", "There are currently no items in this shop!" },
        {"ListItemsMessage4", "Type {0}{1} {2} (item/index) to buy an item!" },
        {"ListItemsMessage5", "You have {0}" },
        {"ListItemsMessage6", " ID: {0} Items virtually left: {1}" },
        {"ListItemsMessage7", "【[c/008000:CATEGORIES]】" },
        {"ListItemsMessage8", "【[c/008000:ITEMS]】" },
        {"ListItemsMessage9", "Type {0}{1} {2} (category name / index) to choose a category!" },
        {"ListItemsMessage10", "Price:"},
        {"ListItemsMessage11", "Warning: keep the space in your inventory while buying if you dont want items to drop on you!" },
        {"CategoryMessage1", "Could not find category with the name {0}" },
        {"CategoryMessage2", "The index you gave is invalid!" },
        {"CategoryMessage3", "Successfully switched to category 【[c/008000:{0}]】!" },
        {"BuyMessage1",  "You cannot buy from your own shop!"},
        {"BuyMessage2", "Please enter the name or index of the item you want to buy." },
        {"BuyMessage3", "Please enter a valid amount to buy." },
        {"BuyMessage4", "The item you are trying to buy is locked behind progression." },
        {"BuyMessage5", "Could not find the item you are trying to buy." },
        {"BuyMessage6", "You are missing {0} this item: {1} x {2}" },
        {"BuyMessage7", "This article costs {0} + {1}, you must hold said item to buy the article." },
        {"BuyMessage8", "The owner of this shop could not be found!" },
        {"BuyMessage9", "There are no items left!"},
        {"BuyMessage10", "This shop's stock chest is gone!" },
        {"BuyMessage11", "Cannot buy item, unsufficient or invalid price item being held!" },
        {"BuyMessage12", "The owner has not set a chest to store price items!" },
        {"BuyMessage13", "The price item chest is gone!" },
        {"BuyMessage14", "There are no free slots in the price chest"},
        {"BuyMessage15", "Purchase" },
        {"BuyMessage16", "For buying {0} {1}" },
        {"BuyMessage17", "Successfully bought {0} [i/p{1}:{2}] for {3}" },
        {"BuyMessage18", "{0} bought {1} {2} for {3}" }
    };

    public static Config DefaultConfig = new Config
    {
        Messages = _message,
        ListRegionSubCommand = _listRegsCommand,
        ShopCommand = _shopCommand,
        ShopPerm = _shopPerm,
        PlayerCommandPerm = _playerCommandsPerm,
        ListItemsSubCommand = _listItemsCommand,
        BuyItemsSubCommand = _buyItemsCommand,
        CategorySubCommand = _catCommand,
        AdminCommandPerm = _adminCommandPerm
    };
}