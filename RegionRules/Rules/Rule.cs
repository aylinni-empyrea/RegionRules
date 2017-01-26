using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using TShockAPI.DB;

namespace RegionRules
{
  public abstract class Rule : IDisposable
  {
    internal RegionRule Region;
    internal Region TSRegion;

    [JsonProperty(Order = 0)]
    public abstract string Type { get; }

    [JsonProperty(Order = 1)]
    public abstract string IgnorePermission { get; }

    internal virtual string Info => Type;

    internal abstract void Register();
    internal abstract void Deregister();

    protected static string Format(string input, Dictionary<string, string> variables)
    {
      var output = new StringBuilder(input);

      foreach (var v in variables)
        output.Replace(v.Key, v.Value);

      return output.ToString();
    }

    protected virtual void Dispose(bool disposing)
    {
      if (disposing)
      {
        Deregister();
      }
    }

    public void Dispose()
    {
      Dispose(true);
    }
  }
}