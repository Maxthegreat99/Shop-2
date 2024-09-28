using Microsoft.Data.Sqlite;
using MySql.Data.MySqlClient;
using NuGet.Protocol;
using System.Data;
using TShockAPI;
using TShockAPI.DB;

namespace Shop2;

public class DB
{
    public static List<ShopRegion> regions = new List<ShopRegion>();

    public class SellingItem
    {
        public int ID;

        public int Price;

        public int PriceItemID;

        public int PriceItemAmount;

        public List<int> DefeatedBossesReq;

        public int ItemID;

        public int Stock;

        public int ChestPosX;

        public int ChestPosY;

        public int PriceChestPosX;

        public int PriceChestPosY;

        public string Category;

        public bool Sold;
    }

    public class ShopRegion
    {
        public int ID;

        public string RegionName;

        public string Owner;

        public List<SellingItem> SellingItems;

        public string Description;

        public string Greet;
    }

    private static IDbConnection db;

    public void Initialize()
    {
        switch (TShock.Config.Settings.StorageType.ToLower())
        {
            case "mysql":
                var dbHost = TShock.Config.Settings.MySqlHost.Split(':');

                db = new MySqlConnection($"Server={dbHost[0]};" +
                                            $"Port={(dbHost.Length == 1 ? "3306" : dbHost[1])};" +
                                            $"Database={TShock.Config.Settings.MySqlDbName};" +
                                            $"Uid={TShock.Config.Settings.MySqlUsername};" +
                                            $"Pwd={TShock.Config.Settings.MySqlPassword};");

                break;

            case "sqlite":
                db = new SqliteConnection($"Data Source={Path.Combine(TShock.SavePath, "Shop.sqlite")}");
                break;

            default:
                throw new ArgumentException("Invalid storage type in config.json!");
        }
        SqlTableCreator creator = new SqlTableCreator(db, db.GetSqlType() == SqlType.Sqlite ? (IQueryBuilder)new SqliteQueryCreator() : new MysqlQueryCreator());
        creator.EnsureTableStructure(new SqlTable("shopregions",
            new SqlColumn("id", MySqlDbType.Int32) { Primary = true, AutoIncrement = true },
            new SqlColumn("regionname", MySqlDbType.Text),
            new SqlColumn("owner", MySqlDbType.Text),
            new SqlColumn("sellingitems", MySqlDbType.Text),
            new SqlColumn("description", MySqlDbType.Text),
            new SqlColumn("greet", MySqlDbType.Text)));

        creator.EnsureTableStructure(new SqlTable("sellitems",
            new SqlColumn("id", MySqlDbType.Int32) { Primary = true, AutoIncrement = true },
            new SqlColumn("price", MySqlDbType.Int32),
            new SqlColumn("defeatedbossreq", MySqlDbType.Text),
            new SqlColumn("itemid", MySqlDbType.Int32),
            new SqlColumn("stock", MySqlDbType.Int32),
            new SqlColumn("chestposx", MySqlDbType.Int32),
            new SqlColumn("chestposy", MySqlDbType.Int32),
            new SqlColumn("pricechestposx", MySqlDbType.Int32),
            new SqlColumn("pricechestposy", MySqlDbType.Int32),
            new SqlColumn("priceitemid", MySqlDbType.Int32),
            new SqlColumn("priceitemamount", MySqlDbType.Int32),
            new SqlColumn("category", MySqlDbType.Text),
            new SqlColumn("sold", MySqlDbType.Int32)));
    }

    public static SellingItem GetItem(int id)
    {
        using (var result = db.QueryReader("SELECT * FROM sellitems WHERE id = @0;", id))
        {
            if (result.Read())
            {
                var item = new SellingItem()
                {
                    ID = result.Get<int>("id"),
                    Price = result.Get<int>("price"),
                    DefeatedBossesReq = (List<int>)result.Get<string>("defeatedbossreq").FromJson(typeof(List<int>)),
                    ItemID = result.Get<int>("itemid"),
                    Stock = result.Get<int>("stock"),
                    ChestPosX = result.Get<int>("chestposx"),
                    ChestPosY = result.Get<int>("chestposy"),
                    PriceItemID = result.Get<int>("priceitemid"),
                    Category = result.Get<string>("category"),
                    PriceItemAmount = result.Get<int>("priceitemamount"),
                    PriceChestPosX = result.Get<int>("pricechestposx"),
                    PriceChestPosY = result.Get<int>("pricechestposy"),
                    Sold = (result.Get<int>("sold") == 1) ? true : false
                };
                return item;
            }
        }
        return null;
    }

    public static ShopRegion GetShopRegion(int id)
    {
        ShopRegion shop = null;
        using (var result = db.QueryReader("SELECT * FROM shopregions WHERE id = @0;", id))
        {
            if (result.Read())
            {
                shop = new ShopRegion()
                {
                    ID = result.Get<int>("id"),
                    RegionName = result.Get<string>("regionname"),
                    Owner = result.Get<string>("owner"),
                    Description = result.Get<string>("description"),
                    Greet = result.Get<string>("greet")
                };

                List<int> items = (List<int>)result.Get<string>("sellingitems").FromJson(typeof(List<int>));
                shop.SellingItems = new List<SellingItem>();
                foreach (int i in items)
                {
                    var item = GetItem(i);

                    if (item == null) continue;
                    shop.SellingItems.Add(item);
                }
            }
        }

        if (shop != null)
            ModifyShopRegion(shop.ID, "sellingitems", shop.SellingItems.Select(i => i.ID).ToJson());

        return shop;
    }

    public static IEnumerable<SellingItem> GetAllItems()
    {
        using (var result = db.QueryReader("SELECT * FROM sellitems;"))
        {
            while (result.Read())
            {
                var item = new SellingItem()
                {
                    ID = result.Get<int>("id"),
                    Price = result.Get<int>("price"),
                    DefeatedBossesReq = (List<int>)result.Get<string>("defeatedbossreq").FromJson(typeof(List<int>)),
                    ItemID = result.Get<int>("itemid"),
                    Stock = result.Get<int>("stock"),
                    ChestPosX = result.Get<int>("chestposx"),
                    ChestPosY = result.Get<int>("chestposy"),
                    PriceItemID = result.Get<int>("priceitemid"),
                    Category = result.Get<string>("category"),
                    PriceItemAmount = result.Get<int>("priceitemamount"),
                    PriceChestPosX = result.Get<int>("pricechestposx"),
                    PriceChestPosY = result.Get<int>("pricechestposy"),
                    Sold = (result.Get<int>("sold") == 1) ? true : false
                };

                yield return item;
            }
        }
    }

    public static IEnumerable<ShopRegion> GetAllShopRegions()
    {
        List<DB.ShopRegion> regs = new List<ShopRegion>();

        using (var result = db.QueryReader("SELECT * FROM shopregions;"))
        {
            while (result.Read())
            {
                var shop = new ShopRegion()
                {
                    ID = result.Get<int>("id"),
                    RegionName = result.Get<string>("regionname"),
                    Owner = result.Get<string>("owner"),
                    Description = result.Get<string>("description"),
                    Greet = result.Get<string>("greet")
                };

                List<int> items = (List<int>)result.Get<string>("sellingitems").FromJson(typeof(List<int>));
                shop.SellingItems = new List<SellingItem>();
                foreach (int i in items)
                {
                    var item = GetItem(i);

                    if (item == null) continue;

                    shop.SellingItems.Add(item);
                }

                regs.Add(shop);
            }
        }
        foreach (var s in regs)
            ModifyShopRegion(s.ID, "sellingitems", s.SellingItems.Select(i => i.ID).ToJson());

        return regs;
    }

    public static int InsertItem(int price, List<int> reqDefeatedBosses, int itemid, int stock, int chestposy, int chestposx, string category, int priceitemid, int priceitemamount, int pricechestposx, int pricechestposy, bool sold)
    {
        db.Query("INSERT INTO sellitems (price, defeatedbossreq, itemid, stock, chestposx, chestposy, category, priceitemid, priceitemamount, pricechestposx, pricechestposy, sold) VALUES (@0, @1, @2, @3, @4, @5, @6, @7, @8, @9, @10, @11);", price, reqDefeatedBosses.ToJson(), itemid, stock, chestposx, chestposy, category, priceitemid, priceitemamount, pricechestposx, pricechestposy, (sold) ? 1 : 0);
        return db.QueryScalar<int>("select last_insert_rowid();");
    }

    public static void RemoveItem(int id)
        => db.Query("DELETE FROM sellitems WHERE id = @0;", id);

    public static int InsertShopRegion(string regionName, string owner, List<int> sellingItems, string description, string greet)
    {
        db.Query("INSERT INTO shopregions (regionname, owner, sellingitems, description, greet) VALUES (@0, @1, @2, @3, @4);", regionName, owner, sellingItems.ToJson(), description, greet);
        return db.QueryScalar<int>("select last_insert_rowid();");
    }

    public static void RemoveShopRegion(int id)
        => db.Query("DELETE FROM shopregions WHERE id = @0;", id);

    public static void ModifyShopRegion(int id, string column, object value)
        => db.Query($"UPDATE shopregions SET {column} = @0 WHERE id = @1", value, id);

    public static void ModifyItem(int id, string column, object value)
        => db.Query($"UPDATE sellitems SET {column} = @0 WHERE id = @1", value, id);

    public static void Update()
    {
        regions.Clear();
        regions.AddRange(GetAllShopRegions());
    }

    public static ShopRegion GetUpdatedRegion(ShopRegion reg)
    {
        return GetShopRegion(reg.ID);
    }
}