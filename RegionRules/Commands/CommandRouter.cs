using RegionRules.Extensions;
using TShockAPI;

namespace RegionRules
{
  internal static partial class PluginCommands
  {
    internal const string INVALID_USAGE = "Invalid usage!";

    internal const string HELP = "Usage: {0}regionrules <add/del/info> <region> <rule> [rule parameters]";

    internal const string NOACCESS = "You don't have access to this command!";

    internal static readonly string[] HELPDESC =
    {
      "Assigns various properties to regions.",
      HELP.FormatWith(Commands.Specifier)
    };

    internal static void CommandRouter(CommandArgs args)
    {
      if (!args.Player.HasPermission(Permissions.Root))
      {
        args.Player.SendErrorMessage(NOACCESS);
        return;
      }

      if (args.Parameters.Count < 1 || string.IsNullOrEmpty(args.Parameters[0]))
      {
        args.Player.SendErrorMessage(INVALID_USAGE);
        args.Player.SendErrorMessage(HELP, Commands.Specifier);
        return;
      }

      switch (args.Parameters[0].ToLowerInvariant())
      {
        case "add":
        {
          if (!args.Player.HasPermission(Permissions.Edit))
          {
            args.Player.SendErrorMessage(NOACCESS);
            return;
          }
          AddCommand(args);
          break;
        }
        case "del":
        case "delete":
        case "remove":
        {
          if (!args.Player.HasPermission(Permissions.Edit))
          {
            args.Player.SendErrorMessage(NOACCESS);
            return;
          }
          DeleteCommand(args);
          break;
        }
        case "info":
        {
          if (!args.Player.HasPermission(Permissions.Info))
          {
            args.Player.SendErrorMessage(NOACCESS);
            return;
          }
          InfoCommand(args);
          break;
        }
        default:
        {
          args.Player.SendErrorMessage(INVALID_USAGE);
          args.Player.SendErrorMessage(HELP, Commands.Specifier);
          return;
        }
      }
    }

    internal static class Permissions
    {
      internal const string Root = "regionrules";
      internal const string Edit = Root + ".edit";
      internal const string Info = Root + ".info";
    }
  }
}