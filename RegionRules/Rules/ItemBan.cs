using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using RegionRules.Extensions;
using Terraria;
using TShockAPI;

namespace RegionRules
{
  public class ItemBan : Rule
  {
    [JsonProperty(Order = 2)] public List<string> BannedItems = new List<string>();
    public override string Type => "Itemban";
    public override string IgnorePermission => "regionrules.ignore.itemban";

    internal override string Info
    {
      get
      {
        var bans =
          Region.Rules.Where(r => string.Equals(r.Type, "itemban", StringComparison.InvariantCultureIgnoreCase))
            .SelectMany(r => ((ItemBan) r).BannedItems).ToList();

        return string.Format("Item ban{1}: {0}", bans.JoinWithVerb(), bans.Count > 1 ? "s" : string.Empty);
      }
    }

    internal override void Register()
    {
      RegionRules.ActiveRules.Add(this);
      GetDataHandlers.PlayerUpdate += OnPlayerUpdate;
    }

    internal override void Deregister()
    {
      RegionRules.ActiveRules.Remove(this);
      GetDataHandlers.PlayerUpdate -= OnPlayerUpdate;
    }

    private void OnPlayerUpdate(object sender, GetDataHandlers.PlayerUpdateEventArgs e)
    {
      var tsplayer = TShock.Players[e.PlayerId];
      var item = tsplayer.TPlayer.inventory[e.Item].name;

      if (!BannedItems.Contains(item) || tsplayer.CurrentRegion != TSRegion || tsplayer.HasPermission(IgnorePermission))
        return;

      BitsByte control = e.Control;
      if (!control[5]) return;

      if (BannedItems.Contains(item))
      {
        control[5] = false;
        e.Control = control;

        tsplayer.Disable($"using banned {item} in restricted region {Region.Region}.");
        tsplayer.SendErrorMessage($"You can't use {item} in this region.");
      }
    }
  }
}