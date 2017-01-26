using System.Collections.Generic;
using Newtonsoft.Json;
using TShockAPI;
using TShockAPI.DB;

namespace RegionRules
{
  public class RegionRule
  {
    [JsonProperty(Order = 0)] public string Region;

    [JsonProperty(Order = 1)] public List<Rule> Rules;
    internal Region TSRegion => TShock.Regions.GetRegionByName(Region);

    internal void Register()
    {
      if (TSRegion == null)
      {
        TShock.Log.ConsoleError($"Region {Region} couldn't be resolved. Ignoring rule.");
        return;
      }

      foreach (var rule in Rules)
      {
        rule.Region = this;
        rule.TSRegion = TSRegion;
        rule.Register();
      }
    }

    internal void Deregister()
    {
      foreach (var rule in Rules)
        rule.Deregister();
    }
  }
}