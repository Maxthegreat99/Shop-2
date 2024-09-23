using System.Reflection;
using Terraria;
using Terraria.ID;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

namespace Shop2;

[ApiVersion(2, 1)]
public class Shop2 : TerrariaPlugin
{
    public override string Author => "Maxthegreat99";

    public override string Description => "A re-implementation and rework of Bokmako's Shop plugin, inspired by Soof's ShopSigns for ComfyEconomy.";

    public override string Name => "Shop-2";

    private static string configPath = Path.Combine(TShock.SavePath, "Shop-2.json");

    public static ShopConfig Configs { get; set; } = new ShopConfig();

    public override Version Version => Assembly.GetExecutingAssembly().GetName().Version;

    public static List<int> DefeatedBosses = new List<int>();
    public static List<int> ValidBossIDs = new List<int>() { NPCID.KingSlime,NPCID.EyeofCthulhu,NPCID.EaterofWorldsHead,NPCID.BrainofCthulhu,NPCID.QueenBee,NPCID.SkeletronHead,NPCID.Deerclops,NPCID.WallofFlesh,NPCID.QueenSlimeBoss,
                                                             NPCID.TheDestroyer,NPCID.Spazmatism,NPCID.SkeletronPrime,NPCID.Plantera,NPCID.Golem,NPCID.DukeFishron,NPCID.HallowBoss,NPCID.CultistBoss,NPCID.MoonLordCore};

    public static DB Database;

    public Shop2(Main game) : base(game)
    {
    }

    public override void Initialize()
    {
        Database = new DB();

        LoadConfigs();
        Database.Initialize();

        GeneralHooks.ReloadEvent += OnReload;

        ServerApi.Hooks.NpcKilled.Register(this, OnNpcKilled);
        ServerApi.Hooks.GamePostInitialize.Register(this, OnGamePostInitialize);
        ServerApi.Hooks.NetGreetPlayer.Register(this, Handler.OnNetGreet);
        ServerApi.Hooks.NetGetData.Register(this, Handler.OnGetData);

        TShock.Log.ConsoleInfo("Shop-2 by Maxthegreat99 successfully loaded {0} shop regions in!".SFormat(DB.regions.Count));
        TShock.Log.ConsoleInfo("Consider joining the discord for support: https://discord.gg/e465y7Xeba");

        TShockAPI.Commands.ChatCommands.Add(new Command(
            permissions: new List<string> { Configs.Settings.ShopPerm, },
            cmd: Commands.store,
            "store", "str"));
    }

    private void OnReload(ReloadEventArgs args)
    {
        LoadConfigs();
    }

    public void LoadConfigs()
    {
        bool writeConfig = true;
        if (File.Exists(configPath))
            Configs.Read(configPath, out writeConfig);

        if (writeConfig)
        {
            Configs.Settings = Lang.DefaultConfig;
            Configs.Write(configPath);
        }
    }

    public void OnGamePostInitialize(EventArgs args)
    {
        /**
         * code to check for defeated bosses, copied from soof's bagger plugin
         * (thank you soof for your works)
         */

        DefeatedBosses.Clear();
        if (IsDefeated(NPCID.KingSlime)) DefeatedBosses.Add(NPCID.KingSlime);
        if (IsDefeated(NPCID.EyeofCthulhu)) DefeatedBosses.Add(NPCID.EyeofCthulhu);
        if (IsDefeated(NPCID.EaterofWorldsHead)) DefeatedBosses.Add(NPCID.EaterofWorldsHead);
        if (IsDefeated(NPCID.BrainofCthulhu)) DefeatedBosses.Add(NPCID.BrainofCthulhu);
        if (IsDefeated(NPCID.QueenBee)) DefeatedBosses.Add(NPCID.QueenBee);
        if (IsDefeated(NPCID.SkeletronHead)) DefeatedBosses.Add(NPCID.SkeletronHead);
        if (IsDefeated(NPCID.Deerclops)) DefeatedBosses.Add(NPCID.Deerclops);
        if (IsDefeated(NPCID.WallofFlesh)) DefeatedBosses.Add(NPCID.WallofFlesh);
        if (IsDefeated(NPCID.QueenSlimeBoss)) DefeatedBosses.Add(NPCID.QueenSlimeBoss);
        if (IsDefeated(NPCID.TheDestroyer)) DefeatedBosses.Add(NPCID.TheDestroyer);
        if (IsDefeated(NPCID.Spazmatism)) DefeatedBosses.Add(NPCID.Spazmatism);
        if (IsDefeated(NPCID.SkeletronPrime)) DefeatedBosses.Add(NPCID.SkeletronPrime);
        if (IsDefeated(NPCID.Plantera)) DefeatedBosses.Add(NPCID.Plantera);
        if (IsDefeated(NPCID.Golem)) DefeatedBosses.Add(NPCID.Golem);
        if (IsDefeated(NPCID.DukeFishron)) DefeatedBosses.Add(NPCID.DukeFishron);
        if (IsDefeated(NPCID.HallowBoss)) DefeatedBosses.Add(NPCID.HallowBoss);
        if (IsDefeated(NPCID.CultistBoss)) DefeatedBosses.Add(NPCID.CultistBoss);
        if (IsDefeated(NPCID.MoonLordCore)) DefeatedBosses.Add(NPCID.MoonLordCore);
        
        DB.regions.Clear();
        DB.regions.AddRange(DB.GetAllShopRegions());

    }

    private bool IsDefeated(int type)
    {
        var unlockState = Main.BestiaryDB.FindEntryByNPCID(type).UIInfoProvider.GetEntryUICollectionInfo().UnlockState;
        return unlockState == Terraria.GameContent.Bestiary.BestiaryEntryUnlockState.CanShowDropsWithDropRates_4;
    }

    private void OnNpcKilled(NpcKilledEventArgs args)
    {
        if (!args.npc.boss || DefeatedBosses.Contains(args.npc.type) || !IsDefeated(args.npc.type))
        {
            return;
        }

        DefeatedBosses.Add(args.npc.type);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            ServerApi.Hooks.NpcKilled.Deregister(this, OnNpcKilled);
            ServerApi.Hooks.GamePostInitialize.Deregister(this, OnGamePostInitialize);
            ServerApi.Hooks.NetGreetPlayer.Deregister(this, Handler.OnNetGreet);
            ServerApi.Hooks.NetGetData.Deregister(this, Handler.OnGetData);
            GeneralHooks.ReloadEvent -= OnReload;
        }

        base.Dispose(disposing);
    }

    public static string FormatMessage(string str)
    {
        return "[Shop2] " + Configs.Settings.Messages[str];
    }
}