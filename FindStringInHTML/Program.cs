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
			TestClass test = new TestClass("https://www.resetera.com/threads/steamspy-is-now-sharing-specific-numbers-again-experimental.52068/", "^<a.+/>");

			//test.WriteToConsole();
			List<string> mediaList = new List<string>();
			Task task = Task.Run(() => test.PrintMediaPaths());
			while(!task.IsCompleted)
			{
				Console.WriteLine("waiting data to be processed");
			}
			
			//iterate over collection and check if it contains multimedia files links
			
			
			Console.ReadLine();
			
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

		private async Task WriteToConsole()
		{
			List<string> matches = new List<string>();
			HttpClient client = new HttpClient();
			
			using (var stream = await client.GetStreamAsync(adress))
			{
				using(var reader = new StreamReader(stream))
				{
					while(!reader.EndOfStream)
					{
						string line = await reader.ReadLineAsync();
						if (regex.IsMatch(line))
						{
							if (Regex.Match(line, "<img.+>").Success)
							{
								Match match = Regex.Match(line, "<img.+>");
								lines.Add(match.Value);
								Match m = Regex.Match(match.Value, "(?<=src=\")\\w+\\W{1}\\.+\"");
								Console.WriteLine(m.Value);
								Matches.Add(m.Value);
							}
						}
					}
				}
			}
			//foreach(string l in matches)
			//{
			//	Console.WriteLine(l);
			//	Console.WriteLine();
			//}

		}

		private async Task GetMediaPath()
		{
			//Task task = Task.Run(() => WriteToConsole());
			

			foreach(var i in Matches)
			{
				if(i.Contains(".jpg") | i.Contains(".gif"))
				{
					MediaPath.Add(i);
				}
			}
		}

		public async Task PrintMediaPaths()
		{
			await WriteToConsole();
			await GetMediaPath();
			foreach(var i in MediaPath)
			{
				Console.WriteLine(i);
			}
		}
	}


}
