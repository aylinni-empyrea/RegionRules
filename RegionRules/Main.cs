using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;
using static RegionRules.PluginCommands;

namespace RegionRules
{
  [ApiVersion(2, 0)]
  public class RegionRules : TerrariaPlugin
  {
    internal static Config Config;
    internal static List<Rule> ActiveRules = new List<Rule>();

    public RegionRules(Main game) : base(game)
    {
      Order = 5;
    }

    public override void Initialize()
    {
      Config = Config.Read();

      ServerApi.Hooks.GamePostInitialize.Register(this, OnGamePostInitialize, -1);

      GeneralHooks.ReloadEvent += OnReload;

      RegionHooks.RegionCreated += OnRegionCreate;
      RegionHooks.RegionDeleted += OnRegionDelete;
    }

    private static void OnReload(ReloadEventArgs reloadEventArgs)
    {
      Config = Config.Read();

      foreach (var r in ActiveRules.ToList())
        r.Deregister();

      Config.Rules.Values.ForEach(r => r.Register());
    }

    private static void OnRegionCreate(RegionHooks.RegionCreatedEventArgs args)
    {
      lock (TShock.Regions)
      {
        Config.Rules.Values.Where(r => r.TSRegion?.Name == args.Region.Name).ForEach(r => r?.Register());
      }
    }

    private static void OnRegionDelete(RegionHooks.RegionDeletedEventArgs args)
    {
      lock (TShock.Regions)
      {
        ActiveRules.Where(r => r.TSRegion?.Name == args.Region.Name).ForEach(r => r?.Deregister());
      }
    }

    private static void OnGamePostInitialize(EventArgs args)
    {
      Config.Rules.Values.ForEach(r => r?.Register());

      Commands.ChatCommands.Add(new Command(PluginCommands.Permissions.Root, CommandRouter, "regionrules")
      {
        HelpText = HELPDESC[0],
        HelpDesc = HELPDESC
      });
    }

    protected override void Dispose(bool disposing)
    {
      ServerApi.Hooks.GameInitialize.Deregister(this, OnGamePostInitialize);

      foreach (var r in ActiveRules.ToList())
        r.Deregister();

      base.Dispose(disposing);
    }

    #region Meta

    public override string Name => "RegionRules";
    public override string Description => "Allows various tweaks to regions.";
    public override string Author => "Newy";

    public override Version Version =>
      Assembly.GetExecutingAssembly().GetName().Version;

    #endregion
  }
}