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
    public string SellSubCommand = string.Empty;
    public string AddBuyItemsSubCommand = string.Empty;
    public string ModifyItemsSubCommand = string.Empty;
    public string ShopPerm = string.Empty;
    public string PlayerCommandPerm = string.Empty;
    public string AdminCommandPerm = string.Empty;
    public string ShopOwnerPerm = string.Empty;
    public int MaxShopItems = 0;
    
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
    private static string _sellCommand = "sell";
    private static string _addBuyItemCommand = "addbuyitem";
    private static string _shopPerm = "shop2.shop";
    private static string _playerCommandsPerm = "shop2.player";
    private static string _showOwnerPerm = "shop2.shopowner";
    private static string _adminCommandPerm = "shop2.admin";
    private static string _modifyItemCommand = "modifyitem";
    private static int _maxShopItems = 50;

    private static Dictionary<string, string> _message = new Dictionary<string, string>
    {
        {"NoAcc", "Couldn't access your bank account!"},
        {"NoPerm", "You dont have the permission to use this command!" },
        {"NotReal", "Please join the game to use this command" },
        {"PlayerCommandHelp", "Store Command Usage:\n {0}{1} {2} (page) - lists the shops currently open in the world"},
        {"PlayerCommandHelp1", "{0}{1} {2} - lists the buyable/sellable items in the shop you are currently in"},
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
        {"ListItemsMessage6", " [c/FF00FF:ID: {0} Items virtually left: {1}]" },
        {"ListItemsMessage7", "【[c/008000:CATEGORIES]】" },
        {"ListItemsMessage8", "【[c/008000:BUY ITEMS]】" },
        {"ListItemsMessage9", "Type {0}{1} {2} (category name / index) to choose a category!" },
        {"ListItemsMessage10", "Price:"},
        {"ListItemsMessage11", "Warning: keep the space in your inventory while buying if you dont want items to drop on you!" },
        {"ListItemsMessage12", "【[c/008000:SELL ITEMS]】"},
        {"ListItemsMessage13", "Selling Price" },
        {"ListItemsMessage14", " [c/FF00FF:ID: {0} Currently buying: {1}]" },
        {"ListItemsMessage15", "Type {0}{1} {2} to sell an item!" },
        {"ListItemsMessage16", "Number of items in shop: {0}/{1}" },
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
        {"BuyMessage17", "Successfully bought {0} [i:{1}] for {2}" },
        {"BuyMessage18", "{0} bought {1} {2} at {3} for {4}" },
        {"BuyMessage19", "There are currently no buyable items in your current category!"},
        {"SellMessage1", "Cannot sell items to your own shop!" },
        {"SellMessage3", "There are currently no sellable items your current category" },
        {"SellMessage4", "This shop only buys this item past a certain progression point" },
        {"SellMessage5", "Could not find the item you are trying to sell!" },
        {"SellMessage6", "Please hold an item to sell!" },
        {"SellMessage7", "The owner does not have enough money for the item you are trying to sell!" },
        {"SellMessage8", "The shop's selling chest has not been set" },
        {"SellMessage9", "The shop's selling chest is gone!" },
        {"SellMessage10", "The shop's selling chest has no free slots left!" },
        {"SellMessage11", "Sale" },
        {"SellMessage12", "For selling {0} {1}"},
        {"SellMessage13", "Successfully sold {0} [i:{1}] for {2}" },
        {"SellMessage14", "{0} sold {1} {2} at {3} for {4}" },
        {"AddBuyMessage1", "You arent the owner of this shop!" },
        {"AddBuyMessage2", "Please hold an item to add to the shop" },
        {"AddBuyMessage3", "Please specify the price and/or category of the item you are trying to add"},
        {"AddBuyMessage4", "Updated selling item [i:{0}] with a new stock of {1}, ID: {2}" },
        {"AddBuyMessage5", "Please enter a valid price" },
        {"AddBuyMessage6", "Successfully added {0} [i:{1}] to the shop with the price {2} ID: {3}" },
        {"AddBuyMessage7", "Use {0}{1} {2} to modify the item furthermore" },
        {"AddBuyMessage8", "You have reached the max item limit of {0} in your shop!" },
        {"ModifyItemMessage1", "Command Usage:\n{0}{1} {2} (Shopping item ID) price (price you want the item to be set to) - modifies the price of the specified item in the shop\n{0}{1} {2} (Shopping item ID) category (category you want the item to be set to) - modifies the category of the item in the shop\n{0}{1} {2} (Shopping item ID) chest - allows you to modify/set the location of the stocks chest if the item is being sold, if the item is an item your shop is buying it will set the location of the chest in which items will apear when players sell to you\n{0}{1} {2} (Shopping item ID) priceitem (item ID) (the amount of said price items needed to but the shopping item) - sets an item as price for the specified shopping item on top of the already existing money price, the amount argument specifies how many of the price item you have said must the player pay to buy your item, the item is taken from their selected item hotbar slot, the item amount cannot exceed the item's max stack, this will also have you set a price chest for where these price items will appear\n{0}{1} {2} (Shopping item ID) pricechest - if the item has a priceitem already set it allows you to reset the chest in which the price items will apear when the players buy from you." },
        {"ModifyItemMessage2", "{0}{1} {2} (Shopping item ID) bossreq (NPC IDs in a list ex. {0}{1} {2} 23 bossreq 50 13 222) - sets the bosses that need to have been killed already for the item to be buyable / sellable" },
        {"ModifyItemMessage3", "Please enter a valid ID" },
        {"ModifyItemMessage4", "Could not find Shopping item!" },
        {"ModifyItemMessage5", "Please specify a value to modify!" },
        {"ModifyItemMessage6", "Please specify a price" },
        {"ModifyItemMessage7", "Please specify a valid price" },
        {"ModifyItemMessage8", "Please specify a category" },
        {"ModifyItemMessage9", "Successfully set chest for [i:{0}] ID: {1} at X: {2} Y: {3}" },
        {"ModifyItemMessage10", "Successfully set the price of [i:{0}] ID: {1} to {2}" },
        {"ModifyItemMessage11", "Successfully set the category of [i:{0}] ID: {1} to {2}" },
        {"ModifyItemMessage12", "Please place a chest somewhere inside your shop to set the stock chest" },
        {"ModifyItemMessage13", "Please specify an item id and item amount" },
        {"ModifyItemMessage14", "Please specify a valid id" },
        {"ModifyItemMessage15", "Please enter a valid item amount and thats not higher than the item's max stack"},
        {"ModifyItemMessage16", "Successfully set the price item for [i:{0}] ID: {1} to {2} [i:{3}] at X: {4} Y: {5}" },
        {"ModifyItemMessage17", "Please place a chest somewhere inside your shop to set the price chest" },
        {"ModifyItemMessage18", "Cannot modify this item's pricechest, give it a price item first please." },
        {"ModifyItemMessage19", "Invalid subcommand!" },
        {"ModifyItemMessage20", "Please give a list of bosses to add to the item" },
        {"ModifyItemMessage21", "Please give a valid boss NPC id" },
        {"ModifyItemMessage22", "Successfully added the following bosses to the item [i:{0}] ID: {1} : {2}" }
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
        AdminCommandPerm = _adminCommandPerm,
        SellSubCommand = _sellCommand,
        AddBuyItemsSubCommand = _addBuyItemCommand,
        ShopOwnerPerm = _showOwnerPerm,
        ModifyItemsSubCommand = _modifyItemCommand,
        MaxShopItems = _maxShopItems
    };
}