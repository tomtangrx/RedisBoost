﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RedisBoost.Cmd
{
    public class Program
    {
		private static string[] _stringRegExps = new[] { @"^[^""\s]+", @"^""([^""]|\"")+""", @"^'([^']|\')+'" };
		private static string _base64RegExp = @"^([A-Za-z0-9+/]{4})*([A-Za-z0-9+/]{4}|[A-Za-z0-9+/]{3}=|[A-Za-z0-9+/]{2}==)$";
		public static void Main(string[] args)
        {
			var client = (args.Length > 0)
				? RedisClient.ConnectAsync(args[0], int.Parse(args[1])).Result
				: RedisClient.ConnectAsync("data source = 127.0.0.1:6379; initial catalog = 0;").Result;

			using (client)
			{
				while (true)
				{
					try
					{
						Console.Write("> ");
						var cmd = "";

						while (true)
						{
							cmd += Console.ReadLine();
							if (cmd.EndsWith("`"))
							{
								cmd = cmd.TrimEnd('`');
								continue;
							}
							break;
						}

						var arguments = SplitCmd(cmd);

						var parsedCmd = ParseCmd(arguments);

						if (IsQuit(parsedCmd))
							break;
						var result = client.ExecuteAsync(parsedCmd.Name, parsedCmd.Arguments).Result;

						OutResult(result);
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.Message);
					}
				}
			}
		}
		private static bool IsQuit(CommandDescriptor parsedCmd)
		{
			var cmd = parsedCmd.Name.ToLower();
			return cmd == "quit" || cmd == "q";

		}

		private static void OutResult(RedisResponse result)
		{
			if (result.ResponseType == ResponseType.Error)
				Console.WriteLine(result.AsError());
			else if (result.ResponseType == ResponseType.Status)
				Console.WriteLine(result.AsStatus());
			else if (result.ResponseType == ResponseType.Integer)
				Console.WriteLine(result.AsInteger());
			else if (result.ResponseType == ResponseType.Bulk)
				Console.WriteLine(result.AsBulk().As<string>());
			else if (result.ResponseType == ResponseType.MultiBulk)
			{
				var mb = result.AsMultiBulk();
				foreach (var r in mb)
				{
					Console.WriteLine(r.As<string>());
				}
			}
		}


		private static CommandDescriptor ParseCmd(string[] arguments)
		{
			byte btemp; short stemp; int temp; long ltemp; decimal dtemp; byte[] b64temp;

			var args = new List<object>();
			foreach (var a in arguments.Skip(1))
			{
				if (a.StartsWith("\"") && a.EndsWith("\""))
					args.Add(a.Trim('"'));
				else if (a.StartsWith("'") && a.EndsWith("'"))
					args.Add(a.Trim('\''));
				else args.Add(a);
			}

			return new CommandDescriptor(arguments[0], args.ToArray());
		}

		private static bool TryParseBase64String(string value, out byte[] result)
		{
			result = null;
			try
			{
				result = Convert.FromBase64String(value);
				return true;
			}
			catch
			{
				return false;
			}
		}
		private static string[] SplitCmd(string cmd)
		{
			var parts = new List<string>();
			while (!string.IsNullOrWhiteSpace(cmd))
			{
				if (cmd.StartsWith("\"\""))
				{
					parts.Add(string.Empty);
					cmd = cmd.Substring(2).TrimStart(' ');
					continue;
				}

				foreach (var regEx in _stringRegExps)
				{
					var m = Regex.Match(cmd, regEx);
					if (!m.Success) continue;

					parts.Add(m.Value);
					cmd = Regex.Replace(cmd, regEx, "").TrimStart(' ');
					break;
				}
			}
			return parts.ToArray();
		}
	}
}
