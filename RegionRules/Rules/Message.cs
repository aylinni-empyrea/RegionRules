using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using RegionRules.Extensions;
using TShockAPI;
using TShockAPI.DB;
using TShockAPI.Hooks;

namespace RegionRules
{
  public class Message : Rule
  {
    [JsonProperty(Order = 4)] public string EnterColor = "255, 255, 0";

    [JsonProperty(Order = 2)] public string EnterMessage;
    [JsonProperty(Order = 5)] public string ExitColor = "255, 0, 0";
    [JsonProperty(Order = 3)] public string ExitMessage;
    public override string Type => "Message";
    public override string IgnorePermission => "regionrules.ignore.message";

    internal override string Info
    {
      get
      {
        var enter =
          Region.Rules.Where(r => string.Equals(r.Type, "message", StringComparison.InvariantCultureIgnoreCase))
            .Select(r => ConfigColor.ToColor(((Message) r).EnterColor).Colorize(((Message) r).EnterMessage)).ToList();

        var exit =
          Region.Rules.Where(r => string.Equals(r.Type, "message", StringComparison.InvariantCultureIgnoreCase))
            .Select(r => ConfigColor.ToColor(((Message) r).ExitColor).Colorize(((Message) r).ExitMessage)).ToList();

        var ret = new StringBuilder();

        if (enter.Count >= 1)
          ret.AppendFormat("Enter message{1}: {0}", string.Join(", ", enter), enter.Count > 1 ? "s" : string.Empty);

        if (enter.Count >= 1 && exit.Count >= 1)
          ret.Append("\n");

        if (exit.Count >= 1)
          ret.AppendFormat("Exit message{1}: {0}", string.Join(", ", exit), exit.Count > 1 ? "s" : string.Empty);

        return ret.ToString();
      }
    }

    private void Send(TSPlayer player, Region region, string message, Color color)
    {
      if (!string.IsNullOrWhiteSpace(message) && !player.HasPermission(IgnorePermission))
        player.SendMessage(Format(message, new Dictionary<string, string>
          {
            {"${player}", player.Name},
            {"${region}", region.Name}
          }),
          color);
    }

    internal override void Register()
    {
      RegionHooks.RegionEntered += OnRegionEnter;
      RegionHooks.RegionLeft += OnRegionLeave;
    }

    internal override void Deregister()
    {
      RegionHooks.RegionEntered -= OnRegionEnter;
      RegionHooks.RegionLeft -= OnRegionLeave;
    }

    private void OnRegionEnter(RegionHooks.RegionEnteredEventArgs args)
    {
      if (args.Region.Name == TSRegion.Name)
        Send(args.Player, args.Region, EnterMessage, ConfigColor.ToColor(EnterColor));
    }

    private void OnRegionLeave(RegionHooks.RegionLeftEventArgs args)
    {
      if (args.Region.Name == TSRegion.Name)
        Send(args.Player, args.Region, ExitMessage, ConfigColor.ToColor(ExitColor));
    }
  }
}