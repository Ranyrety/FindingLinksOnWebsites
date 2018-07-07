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
		static void Main(string[] args)
		{
			TestClass test = new TestClass("https://www.resetera.com/threads/steamspy-is-now-sharing-specific-numbers-again-experimental.52068/", "^<a.+/>");
			//TestClass test = new TestClass("https://twitter.com/DarkCrystal_HQ", " ^<a.+/>");
			//TestClass test = new TestClass("http://pokis.polanow.pl/", "^< a.+/>");
			Task t = Task.Run(() => test.PrintMediaPaths());

			while(!t.IsCompleted)
			{
				Console.Write(" Processing... ");
			}
			foreach(string i in test.MediaPath)
			{
				Console.WriteLine($"{i}");
			}
			Console.WriteLine($"Press any key to quit");

			Console.ReadKey();

		}

		static string GetFileNameAndExtansion(string path)
		{
			string result = String.Empty;
			//get last slash
			//int lastSlash = path.LastIndexOf(path);
			result = path.Substring(path.LastIndexOf('/')+1);
			return result;
		}

		static string FullPathToMedia(string inp)
		{
			string result = String.Empty;
			if (inp.StartsWith("https") || inp.StartsWith("http"))
			{
				return result;
			}
			else return result;
		}
	}


	public class TestClass
	{
		string adress;
		Regex regex;
		List<string> lines = new List<string>();
		public string domainName
		{
			get
			{
				int a = adress.IndexOf('/', adress.Length - 1);
				int b = adress.IndexOf("//");
				int lengthofSubstring = a - b;

				return adress.Substring(7,lengthofSubstring);
			}
		}
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

		//prototype for function that will return list of strings containing specific markup 
		private async Task FindLinkStrings()
		{
			//List<string> Matches = new List<string>();
			MatchCollection matchCollectin;

			using (HttpClient client = new HttpClient())
			{
				using (var stream = await client.GetStreamAsync(adress))
				{
					using (var reader = new StreamReader(stream))
					{
						while (!reader.EndOfStream)
						{
							Regex r;
							MatchCollection linksCollection;
							string htmlBody = await reader.ReadToEndAsync();
							
							r = new Regex(@"<a.+</a>?");
							linksCollection = r.Matches(htmlBody);
							r = new Regex(@"<img.+(</img>?|/>)");
							MatchCollection imagesCollection = r.Matches(htmlBody);

							r = /*new Regex("<img.+>?"); */ new Regex("(?<=src=).+\\..{3}\"?\\b?");
							matchCollectin = r.Matches(htmlBody);
							//if (regex.IsMatch(line))
							//{
							//	if (Regex.Match(line, "<img.+>").Success)
							//	{
							//		Match match = Regex.Match(line, "<img.+>");
							//		lines.Add(match.Value);
							//		Match m = Regex.Match(match.Value, "(?<=src=)\".+?\\..+?\\b\"");
							//		Console.WriteLine(m.Value);
							//		Matches.Add(m.Value);
							//	}
							//}
							foreach(Match i in matchCollectin)
							{
								Matches.Add(i.Value);
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
				if(i.Contains(".jpg") || i.Contains(".gif") || i.Contains(".png"))
				{
					Match m = null;
					string l = TrimSomeStuff(i);
					//Match  m = Regex.Match(l, @"(?:(?<protocol>http(?:s?)|ftp)(?:\:\/\/))(?:(?<usrpwd>\w +\:\w +)(?:\@))? (?<domain>[^/\r\n\:]+)?(?<port>\:\d+)?(?<path>(?:\/.*)*\/)?(?<filename>.*?\.(?<ext>\w{2,4}))?(?<qrystr>\??(?:\w+\=[^\#]+)(?:\&?\w+\=\w+)*)*(?<bkmrk>\#.*)?");
					try
					{
						 m = Regex.Match(l, @"^((https?|ftp)\://((\[?(\d{1,3}\.){3}\d{1,3}\]?)|(([-a-zA-Z0-9]+\.)+[a-zA-Z]{2,4}))(\:\d+)?(/[-a-zA-Z0-9._?,'+&amp;%$#=~\\]+)*/?)$");
					}
					catch(Exception e)
					{
						Console.Clear();
						Console.WriteLine($"There was exception thrown in {e.Source} --- {e.Message}");
					}
					Console.WriteLine($"{m.Success}");
					MediaPath.Add(l);
				}
			}
		}
		
		public async Task PrintMediaPaths()
		{
			await FindLinkStrings();
			await GetMediaPath();
			Console.Clear();
			foreach(var i in MediaPath)
			{
				Console.WriteLine(i);
			}
		}

		public string TrimSomeStuff(string input)
		{
			string result = "";
			if (input.StartsWith("\\") || input.StartsWith("\""))
			{
				result = input.TrimStart('"', '\\', '\"');
				result = result.TrimEnd('"', '/');
			}
			else return input;
			return result;
			
		}

		public async Task DownloadFile()
		{
			using (HttpClient clien = new HttpClient())
			{
				string path = MediaPath[1];
				
				byte[] data = await clien.GetByteArrayAsync(path);
				if (data.Length > 0)
				{
					using (Stream destStream = File.OpenWrite("file.jpg"))
					{
						//await stream.CopyToAsync(destStream);
						for (int i = 0; i < data.Length; i++)
						{
							destStream.WriteByte(data[i]);
						}
					}
				}
			}
		}
	}


}
