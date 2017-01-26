using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using RegionRules.Extensions;
using TShockAPI;

namespace RegionRules
{
  public struct ConfigColor
  {
    private static readonly Regex RGBRegex =
      new Regex(@"(\d{1,3})(?:,|, )(\d{1,3})(?:,|, )(\d{1,3})", RegexOptions.Compiled);

    public string Color;

    public ConfigColor(string color)
    {
      Color = color;
    }

    public ConfigColor(Color color)
    {
      Color = $"{color.R}, {color.G}, {color.B}";
    }

    public static Color ToColor(string c) => c[0] == '#' ? c.ToColor() : ParseRGB(c);

    public static implicit operator ConfigColor(Color color) => new ConfigColor(color);
    public static implicit operator ConfigColor(string color) => new ConfigColor(color);

    private static Color ParseRGB(string color)
    {
      var parsed = RGBRegex.Split(color);

      return new Color(int.Parse(parsed[1]), int.Parse(parsed[2]), int.Parse(parsed[3]));
    }
  }

  public class Config
  {
    public static readonly string ConfigPath = Path.Combine(TShock.SavePath, "RegionRules.json");

    public Dictionary<string, RegionRule> Rules = new Dictionary<string, RegionRule>
    {
      {
        "ArenaRules", new RegionRule
        {
          Region = "arena",
          Rules = new List<Rule>
          {
            new Message
            {
              EnterMessage = "Welcome to my ${region}, ${player}!",
              ExitMessage = "Bye ${player}!"
            },
            new Message
            {
              EnterMessage = "I said welcome, ${region}, ${player}!",
              ExitMessage = "Really bye ${player}!"
            },
            new ItemBan
            {
              BannedItems = new List<string>
              {
                "Meowmere",
                "Terrarian"
              }
            },
            new ItemBan
            {
              BannedItems = new List<string>
              {
                "Star Wrath"
              }
            }
          }
        }
      }
    };

    public static Config Read()
    {
      Config ret;

      try
      {
        var data = File.ReadAllText(ConfigPath);
        if (string.IsNullOrWhiteSpace(data))
        {
          Backup("RegionRules.json is empty, creating a new one...");

          ret = new Config();
          ret.Write();
          return ret;
        }

        ret = JsonConvert.DeserializeObject<Config>(data,
          new JsonSerializerSettings {TypeNameHandling = TypeNameHandling.Auto});
      }
      catch (FileNotFoundException)
      {
        TShock.Log.ConsoleInfo("RegionRules.json not found, creating a new one...");
        ret = new Config();
        ret.Write();
      }
      catch (Exception e)
      {
        Backup(e.ToString());

        ret = new Config();
        ret.Write();
      }

      return ret;
    }

    private static void Backup(string stack)
    {
      var backup = ConfigPath + DateTime.Now.ToString(".yy-mm-dd_HH-mm-ss") + ".bak";

      File.Copy(ConfigPath, backup);

      TShock.Log.ConsoleError("An error has occurred while reading RegionRules.json:");
      TShock.Log.ConsoleError(stack);
      TShock.Log.ConsoleInfo("A new config file has been generated.");
      TShock.Log.ConsoleInfo("Old file saved in: " + backup);
    }

    public void Write()
    {
      Task.Factory.StartNew(() =>
        {
          lock (RegionRules.Config)
          {
            File.WriteAllText(ConfigPath,
              JsonConvert.SerializeObject(this, Formatting.Indented,
                new JsonSerializerSettings {TypeNameHandling = TypeNameHandling.Auto}));
          }
        }
      );
    }
  }
}