using System;
using HtmlAgilityPack;
using System.Linq;
using System.Globalization;
using System.IO;

namespace Tsp2Quicken
{
	class Program
	{
		static void Main(string[] args)
		{
			TextWriter csvWriter = (args.Length > 0) ? new StreamWriter(args[0]) : Console.Out;

			try
			{
				var url = "https://www.tsp.gov/InvestmentFunds/FundPerformance/index.html";
				var web = new HtmlWeb();

				var doc = web.Load(url);

				var rowNodes = doc.DocumentNode.SelectNodes("//table[@class='tspStandard']/tr").ToArray();
				var headers = rowNodes[0].SelectNodes("th").Select(n => n.InnerText.Trim()).ToArray();

				for (int iRow = 1; iRow < rowNodes.Length; iRow++)
				{ 
					var columns = rowNodes[iRow].SelectNodes("td").Select(n => n.InnerText.Trim().Replace("\n", "")).ToArray();
					var date = DateTime.ParseExact(columns[0], "MMM dd, yyyy", CultureInfo.InvariantCulture);
					var outputDate = date.ToString("MM/dd/yy");

					for (int i = 1; i < columns.Length; i++)
						csvWriter.WriteLine($"TSP {headers[i]},{outputDate},{columns[i]}");
				}
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine(ex.ToString());
			}
			finally
			{
				csvWriter.Close();
			}
		}
	}
}
