using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;

namespace FindStringInHTML
{
	class Program
	{
		static  void Main(string[] args)
		{
			//TestClass test = new TestClass("https://www.resetera.com/threads/steamspy-is-now-sharing-specific-numbers-again-experimental.52068/", "^<a.+/>");
			TestClass test = new TestClass("https://twitter.com/DarkCrystal_HQ", " ^<a.+/>");

			List <string> mediaList = new List<string>();
			Task task = Task.Run(() => test.PrintMediaPaths());
			while(!task.IsCompleted)
			{
				Console.WriteLine("waiting data to be processed");
			}

			Console.WriteLine($"Data processed there was {test.Matches.Count} matches ");
			Console.WriteLine($"Press any key to quit");
			
			Console.ReadKey();
			
		}
	}


	public class TestClass
	{
		string adress;
		Regex regex;
		List<string> lines = new List<string>();
		//List<string> matches = new List<string>();
		public List<string> MediaPath { get; private set; }

		public List<string> Matches { get; private set; }

		public TestClass(string adr, string reg)
		{
			adress = adr;
			regex = new Regex(reg);
			MediaPath = new List<string>();
			Matches = new List<string>();
		}

		//prototyp for function that will return list of strings containing specific markup 
		private async Task FindLinkStrings()
		{
			List<string> matches = new List<string>();


			using (HttpClient client = new HttpClient())
			{
				using (var stream = await client.GetStreamAsync(adress))
				{
					using (var reader = new StreamReader(stream))
					{
						while (!reader.EndOfStream)
						{
							string line = await reader.ReadLineAsync();
							if (regex.IsMatch(line))
							{
								if (Regex.Match(line, "<img.+>").Success)
								{
									Match match = Regex.Match(line, "<img.+>");
									lines.Add(match.Value);
									Match m = Regex.Match(match.Value, "(?<=src=)\".+?\\..+?\\b\"");
									Console.WriteLine(m.Value);
									Matches.Add(m.Value);
								}
							}
						}
					}
				}
			}
		}

		//prototype function for finding if link is to some resources which interest us
		private async Task GetMediaPath()
		{
			
			foreach(var i in Matches)
			{
				if(i.Contains(".jpg") | i.Contains(".gif") | i.Contains(".png"))
				{
					MediaPath.Add(i);
				}
			}
		}
		
		public async Task PrintMediaPaths()
		{
			await FindLinkStrings();
			await GetMediaPath();
			foreach(var i in MediaPath)
			{
				Console.WriteLine(i);
			}
		}
	}


}
