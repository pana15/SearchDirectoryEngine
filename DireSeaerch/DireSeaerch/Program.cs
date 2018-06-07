using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Caching;

namespace DireSeaerch
{
	class Program
	{
		static int n, countA = 0, countB = 0, countC = 0;
		static string rank;
		static void Main(string[] args)
		{
			List<string> word = new List<string>();
			string path = null, continues = "Y";
			while(!Directory.Exists(path))
			{
				Console.WriteLine("ENTER THE DIRECTORY TO SEARCH: ");
				path = Console.ReadLine();
			}
			Console.WriteLine("DIRECTORY EXISTS!");
			
			AddCache(path);
			Console.WriteLine("{0} files found", countA);
			while (continues == "Y")
			{
				Console.WriteLine("HOW MANY WORD(S) WILL YOU SEARCH: ");
				Int32.TryParse(Console.ReadLine(), out n);
				Console.WriteLine("ENTER THE WORD(S) TO SEARCH: ");
				//Limited words to 50
				for (int i = 0; i < n; i++)
				{
					string x = Console.ReadLine();
					if (x != null)
						word.Add(x);
				}
				foreach (string file in Directory.EnumerateFiles(path, "*.txt"))
				{
					string line = LocalCache.GetData<string>(file);
					//Update Cache if needed
					if (line == null)
					{
						AddCache(path);
						line = LocalCache.GetData<string>(file);
					}
					var words = line.Split(' ', '\n', '\r');
					foreach (var item in word)
					{
						countB = words.Length;
						if (words.Contains(item))
							countC++;
						rank = (Decimal.Divide(countC, countB) * 100).ToString("0.00");
						Console.WriteLine("The rank of word {0} in {1} is: {2} \n", item, file, rank);
					}
				}
				Console.WriteLine("Continue Search? (Y/N)");
				continues = Console.ReadLine();
				while (continues != "Y" && continues != "N")
				{
					Console.WriteLine("Continue Search? (Y/N)");
					continues = Console.ReadLine();
				}
			}

		}

		public static void AddCache(string path)
		{
			//Write to Cache if it doesn't already exist
			foreach (string file in Directory.EnumerateFiles(path, "*.txt"))
			{
				string content = File.ReadAllText(file);
				//Caching for 5 minutes
				LocalCache.UpdateCache(file, content, TimeSpan.FromSeconds(300));
				countA++;
			}
		}

	}

	public static class LocalCache
	{
		static MemoryCache mem = new MemoryCache("Cache");
		public static T GetData<T>(string key) where T : class 
		{
			LocalCacheItem loc = mem.Get(key) as LocalCacheItem;
			if (loc == null)
				return null;
			return loc.Value as T;
		}

		public static void UpdateCache<T>(string key, T value, TimeSpan expiration)
		{
			LocalCacheItem loc = mem.Get(key) as LocalCacheItem;
			if(loc == null)
				loc = new LocalCacheItem();
			loc.Value = value;
			mem.Set(key, loc, new DateTimeOffset(DateTime.Now.Add(expiration)));
		}
	}
	public class LocalCacheItem
	{
		public object Value;
	}
}
