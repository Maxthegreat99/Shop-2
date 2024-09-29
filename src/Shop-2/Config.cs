using TShockAPI.Configuration;

namespace Shop2;

public class Config
{
    public string ShopCommand = string.Empty;
    public string ListRegionSubCommand = string.Empty;
    public string ListItemsSubCommand = string.Empty;
    public string BuyItemsSubCommand = string.Empty;
    public string CategorySubCommand = string.Empty;
    public string SellSubCommand = string.Empty;
    public string AddBuyItemsSubCommand = string.Empty;
    public string ModifyItemsSubCommand = string.Empty;
    public string CreateShopSubCommand = string.Empty;
    public string ModifyShopSubCommand = string.Empty;
    public string AddSellItemSubCommand = string.Empty;
    public string ConfirmSubCommand = string.Empty;
    public string ShopPerm = string.Empty;
    public string PlayerCommandPerm = string.Empty;
    public string AdminCommandPerm = string.Empty;
    public string ShopOwnerPerm = string.Empty;
    public int MaxShopItems = 0;
    public int MinShopArea = 0;
    public int MaxShopArea = 0;
    public int CostPerBlockShop = 0;
    public int SpawnProtectionRadius;
    public int MaxOwnableShop = 0;
    public int MaxShopNameCharacters = 0;
    public int MaxShopDescriptionCharacters = 0;
    public int MaxShopGreetCharacters = 0;
    public Dictionary<string, string> Messages = new Dictionary<string, string>();
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
    private static string _createShopCommand = "create";
    private static string _modifyShopCommand = "modifyshop";
    private static string _addSellItemCommand = "addsellitem";
    private static string _confirmActionCommand = "confirm";
    private static int _maxShopItems = 50;
    private static int _minimumShopArea = 50;
    private static int _maximumShopArea = 550;
    private static int _costPerBlock = 175;
    private static int _spawnProtectionRadius = 250;
    private static int _maxOwnableShops = 3;
    private static int _maxCharactersShopName = 25;
    private static int _maxCharactersShopDescription = 200;
    private static int _maxChratctersShopGreet = 100;

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
        {"ListRegionsMessage4","There are no shops to display." },
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
        {"BuyMessage20", "Cannot buy item, the shop owner's stock chest is not located within the shop region"},
        {"BuyMessage21", "Cannot buy item, the shop owner's price stock chest is not located within the shop region" },
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
        {"SellMessage15", "cannot sell, the  shop owner's selling chest is not located in the shop region"},
        {"AddBuyMessage1", "You arent the owner of this shop!" },
        {"AddBuyMessage2", "Please hold an item to add to the shop" },
        {"AddBuyMessage3", "Please specify the price and/or category of the item you are trying to add"},
        {"AddBuyMessage4", "Updated selling item [i:{0}] with a new stock of {1}, ID: {2}" },
        {"AddBuyMessage5", "Please enter a valid price" },
        {"AddBuyMessage6", "Successfully added {0} [i:{1}] to the shop with the price {2} ID: {3}" },
        {"AddBuyMessage7", "Use {0}{1} {2} to modify the item furthermore" },
        {"AddBuyMessage8", "You have reached the max item limit of {0} in your shop!" },
        {"ModifyItemMessage1", "Command Usage:\n{0}{1} {2} (Shopping item ID) price (price you want the item to be set to) - modifies the price of the specified item in the shop (you can write it in numbers or like this : 1p for 1 platinum, 1g2s for 1 gold and 2 silver etc...)\n{0}{1} {2} (Shopping item ID) category (category you want the item to be set to) - modifies the category of the item in the shop\n{0}{1} {2} (Shopping item ID) chest - allows you to modify/set the location of the stocks chest if the item is being sold, if the item is an item your shop is buying it will set the location of the chest in which items will apear when players sell to you\n{0}{1} {2} (Shopping item ID) priceitem (item ID) (the amount of said price items needed to but the shopping item) - sets an item as price for the specified shopping item on top of the already existing money price, the amount argument specifies how many of the price item you have said must the player pay to buy your item, the item is taken from their selected item hotbar slot, the item amount cannot exceed the item's max stack, this will also have you set a price chest for where these price items will appear\n{0}{1} {2} (Shopping item ID) pricechest - if the item has a priceitem already set it allows you to reset the chest in which the price items will apear when the players buy from you." },
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
        {"ModifyItemMessage22", "Successfully added the following bosses to the item [i:{0}] ID: {1} : {2}" },
        {"CreateShopMessage1", "Command Usage:\n{0}{1} {2} set 1 - Sets the first point of the rectangle determining the shop region you want to create\n{0}{1} {2} set 2 - Sets the second point of the rectangle determining the shop region you want to create\n{0}{1} {2} comfirm (Shop name) - Attempts to create a shop region with the points you have set, you can user multiple parameters to set the shop name, like this: {0}{1} {2} confirm Shop Name." },
        {"CreateShopMessage2", "Please choose a proper point number to set" },
        {"CreateShopMessage3", "Please set the first point before the second" },
        {"CreateShopMessage4", "Please hit a block to set your shop's first point" },
        {"CreateShopMessage5", "Please hit a block to set your shop's second point"},
        {"CreateShopMessage6", "Successfully set the first point at X: {0} Y: {0}" },
        {"CreateShopMessage7", "Failed to set second point, area is too big, cannot exceed: {0} blocks! Area trying to set: {1} blocks" },
        {"CreateShopMessage8", "Failed to set second point, area is too small cannot go below: {0} blocks! Area trying to set: {1} blocks" },
        {"CreateShopMessage9", "Failed to set second point, your area overlaps with the spawn protection area" },
        {"CreateShopMessage10", "Failed to set second point, your area overlaps with another region"},
        {"CreateShopMessage11", "Successfully created second point at X: {0} Y: {1}, the area of your region will be {2}" },
        {"CreateShopMessage12", "Buying this region will cost: {0}" },
        {"CreateShopMessage14", "Please set both of your points before creating the shop" },
        {"CreateShopMessage15", "A shop or region already has that name" },
        {"CreateShopMessage16", "Cannot create shop, area overlaps with spawn" },
        {"CreateShopMessage17", "Cannot create shop, area overlaps with another region" },
        {"CreateShopMessage18", "Cannot create shop, not enough money, Missing {0}" },
        {"CreateShopMessage19", "Shop Creation" },
        {"CreateShopMessage20", "For creating a {0} blocks shop named {1}" },
        {"CreateShopMessage21", "Cannot own more than {0} shops!" },
        {"CreateShopMessage22", "Successfully created a new shop named {0} with an area of {1}! to modify properties of your shop further (description/greet message) do {2}{3} {4}"},
        {"CreateShopMessage23", "{0}'s shop" },
        {"CreateShopMessage24", "Welcome to {0}'s shop!" },
        {"CreateShopMessage25", "Name of shop cannot be more than {0} characters" },
        {"ModifyShopMessage1", "Command Usage:\n{0}{1} {2} description (shop description) - modifies the shop's description which is showed to players when listing all the different shops present in the world\n{0}{1} {2} greet (shop greeting) - modifies the message showed to players upon entering your shop\n{0}{1} {2} removeitem (shop item ID) - Removes the specified item from listing in the shop and gives back you the items stored virtually in the store if its a buying item\n{0}{1} {2} deleteshop - sends you a comfirmation request to delete the shop, you will be given the stored items back, the region will also be deleted\n{0}{1} {2} transferownership (player name) - sends you a comfirmation requet to transfers ownership of the shop to the specified player, you will no longer be able to modify this shop\n{0}{1} {2} resizeshop - sends a confirmation message to resize the shop using set points, you need to do {0]{1} {3} set <1/2> before typing out this command, the price of resizing the shop depends on the current area of the shop and the new area, if the area you have set with set <1/2> is bigger than the current area of your shop the price will be calculated using the extra area you are gaining, if its smaller the price is 0\n{0}{1} {2} confirm - confirmation for the delete and transferownership commands, actions cannot be reversed after this command is executed\n{0}{1} {2} trust (player name) - allows/prevents the specified player to build in your shop region, use this command on a player that can already build in your shop region to prevent them from building in it" },
        {"ModifyShopMessage2", "Please provide a description" },
        {"ModifyShopMessage3", "Shop description cannot exceed {0} characters" },
        {"ModifyShopMessage4", "Successfully changed the shop's description to:\n{0}"},
        {"ModifyShopMessage5", "Succesfully changed the shop's greet mesage to:\n{0}" },
        {"ModifyShopMessage6", "Please provide a greet message" },
        {"ModifyShopMessage7", "Shop greet message cannot exceed {0} characters" },
        {"ModifyShopMessage8", "Please provide the id of a shop item!" },
        {"ModifyShopMessage9", "Please provide a valid shop item ID!" },
        {"ModifyShopMessage10", "Successfully deleted sell item [i:{0}] ID: {1}" },
        {"ModifyShopMessage11", "Successfully deleted buy item [i:{0}] ID: {1}" },
        {"ModifyShopMessage12", "type {0}{1} {2} confirm to confirm that you want delete to your shop named {3}, currently holding {4} items and with an area of {5} blocks" },
        {"ModifyShopMessage13", "Please set both of your points using {0}{1} {2} set <1/2> before using this command"},
        {"ModifyShopMessage14", "The area you are trying to set is too close to spawn!" },
        {"ModifyShopMessage15", "The area you are trying to set collides with another region!" },
        {"ModifyShopMessage16", "Price of modifying the area: {0} to add {1} blocks of area to your current shop, currently missing: {2}" },
        {"ModifyShopMessage17", "Shop Resize" },
        {"ModifyShopMessage18", "For adding {0} blocks of area to shop named {1}" },
        {"ModifyShopMessage19", "Successfully resize shop region from {0} blocks to {1} blocks for {2}"},
        {"ModifyShopMessage20", "Please specify the player you want to transfer ownership to" },
        {"ModifyShopMessage21", "No players match the name of {0}" },
        {"ModifyShopMessage22", "Please type {0}{1} {2} confirm to confirm that you want to transfer your shop {3} to {4}" },
        {"ModifyShopMessage23", "Nothing to confirm, use this command after using resizeshop or transferownership commands to confirm their actions" },
        {"ModifyShopMessage24", "Successfully deleted your shop {0}!" },
        {"ModifyShopMessage25", "Player {0} does not have the permission to own a shop!"},
        {"ModifyShopMessage26", "{0} now owns {1}!" },
        {"ModifyShopMessage27", "You have earned ownership of the shop {0}!" },
        {"ModifyShopMessage28", "Please specify a player to allow to build in your shop" },
        {"ModifyShopMessage29", "Couldnt find a player containing name {0}" },
        {"ModifyShopMessage30", "Prevented {0} from building in your shop"},
        {"ModifyShopMessage31", "Allowed {0} to build in your shop" },
        {"AddSellItemMessage1", "Please specify an item id, amount and price of the item you want to buy from players" },
        {"AddSellItemMessage2", "Please provide a valid item ID" },
        {"AddSellItemMessage3", "Please provide a valid amount" },
        {"AddSellItemMessage4", "Please provide a valid price" },
        {"AddSellItemMessage5", "A selling item of the same id has already been added in your shop, modified the amount and price of said item." },
        {"AddSellItemMessage6", "Successfully added sell item [i:{0}] ID: {1} with an amount to buy of {2} each with a price of {3}!" },
        {"AddSellItemMessage7", "IMPORTANT: For your sell item to be valid you need to give it a chest for storing the items when players sell to you, do so using {0}{1} {2} (ID) chest" },
        {"ShopSign1", "Successfully setted shop sign for [i:{0}] ID {1}!" },
        {"ConfirmBuyMessage1", "Checking the stocks of item [i:{0}]..." },
        {"ConfirmBuyMessage2", "The amount of items virtually left was not enough to meet the amount you inserted, after confirming purchase type the command again to see if the item has the stocks you need in the shop's stock chest" },
        {"ConfirmBuyMessage3", "The shop's stock chest does not have enough to meet your required item amount" },
        {"ConfirmBuyMessage4", "Type {0}{1} {2} to confirm that you want to buy {3} [i:{4}] for {5}" },
        {"ConfirmSellMessage5", "Checking Stockc chest validity of item [i:{0}]" },
        {"ConfirmSellMessage6", "Type {0}{1} {2} to confirm that you want to sell {3} [i:{4}]" },
        {"Confirm1", "try to sell or buy something before typing this command"}

        
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
        MaxShopItems = _maxShopItems,
        CreateShopSubCommand = _createShopCommand,
        MinShopArea = _minimumShopArea,
        MaxShopArea = _maximumShopArea,
        CostPerBlockShop = _costPerBlock,
        SpawnProtectionRadius = _spawnProtectionRadius,
        MaxOwnableShop = _maxOwnableShops,
        ModifyShopSubCommand = _modifyShopCommand,
        MaxShopNameCharacters = _maxCharactersShopName,
        MaxShopDescriptionCharacters = _maxCharactersShopDescription,
        MaxShopGreetCharacters = _maxChratctersShopGreet,
        AddSellItemSubCommand = _addSellItemCommand,
        ConfirmSubCommand = _confirmActionCommand
    };
}