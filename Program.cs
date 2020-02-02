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
				var doc = new HtmlWeb().Load("https://www.tsp.gov/InvestmentFunds/FundPerformance/index.html");

				var rowNodes = doc.DocumentNode.SelectNodes("//table[@class='tspStandard']/tr").ToArray();

				// first row is headers
				var headers = rowNodes[0].SelectNodes("th").Select(n => n.InnerText.Trim()).ToArray();

				// subsequent rows are quotes by date
				for (int iRow = 1; iRow < rowNodes.Length; iRow++)
				{ 
					// get the cell text
					var data = rowNodes[iRow].SelectNodes("td").Select(n => n.InnerText.Trim().Replace("\n", "")).ToArray();

					// Date is always in the form Feb 01, 2020
					var date = DateTime.ParseExact(data[0], "MMM dd, yyyy", CultureInfo.InvariantCulture);

					// Date in the csv file is in the form 02/01/20
					var outputDate = date.ToString("MM/dd/yy");

					for (int i = 1; i < data.Length; i++)
						csvWriter.WriteLine($"TSP {headers[i]},{outputDate},{data[i]}");
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
