using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Caching;

namespace DireSeaerch
{
	class Program
	{
		static void Main(string[] args)
		{
			string word, path, cacheKey = "file";
			path = null;
			while(!Directory.Exists(path))
			{
				Console.WriteLine("ENTER THE DIRECTORY TO SEARCH: ");
				path = Console.ReadLine();
			}
			Console.WriteLine("DIRECTORY EXISTS!");
			foreach (string file in Directory.EnumerateFiles(path, "*.txt"))
			{
				string content = File.ReadAllText(file);
				LocalCache.UpdateCache(cacheKey, content, TimeSpan.FromSeconds(300));
			}
			Console.WriteLine("ENTER THE WORD TO SEARCH: ");
			word = Console.ReadLine();
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
			lock (loc)
			{
				loc.Value = value;
				mem.Set(key, loc, new DateTimeOffset(DateTime.Now.Add(expiration)));
			}
		}
	}
	public class LocalCacheItem
	{
		public object Value;
	}
}
