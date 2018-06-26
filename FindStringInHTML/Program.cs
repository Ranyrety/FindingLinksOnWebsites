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
			TestClass test = new TestClass("https://www.resetera.com/forums/video-games.7/", "href");

			test.WriteToConsole();

			Console.ReadLine();
			
		}
	}


	public class TestClass
	{
		string adress;
		Regex regex; 

		public TestClass(string adr, string reg)
		{
			adress = adr;
			regex = new Regex(reg);
		}

		public async void WriteToConsole()
		{
			HttpClient client = new HttpClient();
			List<string> lines = new List<string>();
			using (var stream = await client.GetStreamAsync(adress))
			{
				using(var reader = new StreamReader(stream))
				{
					while(!reader.EndOfStream)
					{
						string line = await reader.ReadLineAsync();
						if (regex.IsMatch(line))
						{
							lines.Add(line);
							//Console.WriteLine(line);
						}
					}
				}
			}
			foreach(string l in lines)
			{
				Console.WriteLine(l);
				Console.WriteLine();
			}

		}
	}


}
