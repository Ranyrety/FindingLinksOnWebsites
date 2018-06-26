using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.IO;
using System.Text.RegularExpressions;

namespace FindStringInHTML
{
	class Program
	{
		static void Main(string[] args)
		{
			TestClass test = new TestClass("https://www.resetera.com/forums/video-games.7/", "^<a.+>\\b");

			test.WriteToConsole();

			Console.ReadLine();
			
		}
	}


	public class TestClass
	{
		string adress;
		Regex regex;
		List<string> lines = new List<string>();
		List<string> matches = new List<string>();

		public TestClass(string adr, string reg)
		{
			adress = adr;
			regex = new Regex(reg);
		}

		public async void WriteToConsole()
		{
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
							Match match = Regex.Match(line, regex.ToString());
							lines.Add(match.Value);
							Match m = Regex.Match(match.Value, "(?!=href=)\\w+\\W{1}\\w+");
							Console.WriteLine(m.Value);
							matches.Add(m.Value);
						}
					}
				}
			}
			foreach(string l in matches)
			{
				Console.WriteLine(l);
				Console.WriteLine();
			}

		}
	}


}
