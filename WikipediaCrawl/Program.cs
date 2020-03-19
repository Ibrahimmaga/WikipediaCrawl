using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WikipediaCrawl
{
    class Program
    {
		public static string RemoveHtmlTags(string html)
		{
			string htmlRemoved = Regex.Replace(html, @"<script[^>]*>[\s\S]*?</script>|<[^>]+>| ", " ").Trim();
			string normalised = Regex.Replace(htmlRemoved, @"\s{2,}", " ");
			return normalised;
		}

		public static async Task WebCrawlHtmlAgilityPack()
		{
			var url = "https://en.wikipedia.org/wiki/Microsoft";
			var httpClient = new HttpClient();
			var html = await httpClient.GetStringAsync(url);

			var htmldocument = new HtmlDocument();
			htmldocument.LoadHtml(html);

			var divs = htmldocument.DocumentNode.Descendants("div")
				.Where(node => node.GetAttributeValue("class", "")
				.Equals("mw-parser-output")).ToList();

			foreach(var div in divs)
			{
				var h3 = div.Descendants("h3").FirstOrDefault();
				Console.WriteLine(h3.InnerText);
			}

			
		}

		public static void Main()
		{
			using (WebClient client = new WebClient())
			{
				
				string html = client.DownloadString("https://en.wikipedia.org/wiki/Microsoft").ToLower();

				
				html = RemoveHtmlTags(html);

				
				List<string> list = html.Split(' ').ToList();

				//remove any non alphabet characters
				var onlyAlphabetRegEx = new Regex(@"[A-z]+$");
				list = list.Where(f => onlyAlphabetRegEx.IsMatch(f)).ToList();

				//distict keywords by key and count, and then order by count.
				var keywords = list.GroupBy(x => x).OrderByDescending(x => x.Count()).Take(10);

				//print each keyword to console.
				foreach (var word in keywords)
				{
					Console.WriteLine("{0} {1}", word.Key, word.Count());
				}

				
			}

			Console.WriteLine("\nweb scaper using HtmlAgilityPack");
			WebCrawlHtmlAgilityPack();

			Console.ReadLine();
		}	
	}
}
