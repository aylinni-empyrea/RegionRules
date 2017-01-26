using System.Collections.Generic;
using System.Linq;
using TShockAPI;

namespace RegionRules
{
  internal static partial class PluginCommands
  {
    private static void InfoCommand(CommandArgs args)
    {
      if (args.Parameters.Count < 2 || args.Parameters.Count > 3 || string.IsNullOrEmpty(args.Parameters[1]))
      {
        args.Player.SendErrorMessage(INVALID_USAGE);
        args.Player.SendErrorMessage("Usage: {0}regionrules info <region/rule>", Commands.Specifier);
        return;
      }

      var region = RegionRules.Config.Rules.FirstOrDefault(rule => rule.Value.Region == args.Parameters[1]).Value;

      if (region == null)
      {
        args.Player.SendErrorMessage($"Region {args.Parameters[1]} doesn't have any defined rules!");
        return;
      }

      var terms = new List<string>();
      var rules = region.Rules.GroupBy(r => r.Type).Select(g => g.First().Info);

      terms.Add($"Region: {region.Region}");

      terms.Add(string.Join("\n", rules));

      int page;
      PaginationTools.TryParsePageNumber(args.Parameters, 2, args.Player, out page);

      PaginationTools.SendPage(args.Player, page, terms, terms.Count, new PaginationTools.Settings {});
    }
  }
}