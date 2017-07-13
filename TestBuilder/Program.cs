using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Data.SqlClient;
using System.Configuration;

namespace TestBuilder
{
	class Program
	{
		static void Main(string[] args)
		{
			DateTime beginning = DateTime.Parse("7-14-2017");

			for (int i = 1000; i < 1020; i++)
			{
				string currentExtract = @"D:\Alteryx Training\install\riley\cs_test\test_data\samples\" + i;
				if(!Directory.Exists(currentExtract))
				{
					Directory.CreateDirectory(currentExtract);
				}

				//RecordDB(i.ToString(), 17, beginning);
				string fileName = "X" + i + ".pii.rng0";
				generateSamples(currentExtract + "\\" + fileName);

				if (i % 3 == 0)
				{
					//RecordDB(i.ToString(), 3, beginning);
					fileName = "X" + i + ".pii.irsg.rng0";
					generateSamples(currentExtract + "\\" + fileName);

					//RecordDB(i.ToString(), 7, beginning);
					fileName = "E" + i + ".pii.rng0";
					generateSamples(currentExtract + "\\" + fileName);

					//RecordDB(i.ToString(), 9, beginning);
					fileName = "E" + i + ".pii.irsg.rng0";
					generateSamples(currentExtract + "\\" + fileName);

				}
				beginning = beginning.AddDays(1);
			}
		}

		public static void generateSamples(string outputPath)
		{
			string sample = ConfigurationManager.AppSettings["sample"];

			for (int j = 1; j <= 8; j++)
			{
				if(!File.Exists(outputPath + j))
				{
					File.Copy(sample, outputPath + j);
				}
			}
		}
		
		public static void RecordDB(string extractNumber, int fileId, DateTime beginning)
		{
			//manipulate data so current day is always 1 day between available and extract
			DateTime AvailableDate = beginning.AddDays(1);
			DateTime ExtractDate = beginning.AddDays(-1);

			using(SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["umbrella_dev"].ToString()))
			{
				conn.Open();

				string query = "insert into UmbrellaI.dbo.BaseFileAvailability (BaseFileId, AvailableDate, ExtractDate, ExtractNumber) " +
					"values (@id, @availableDate, @extractDate, @extractNumber)";

				SqlCommand cmd = new SqlCommand(query, conn);
				cmd.Parameters.AddWithValue("@id", fileId);
				cmd.Parameters.AddWithValue("@availableDate", AvailableDate);
				cmd.Parameters.AddWithValue("@extractDate", ExtractDate);
				cmd.Parameters.AddWithValue("@extractNumber", extractNumber);

				cmd.ExecuteNonQuery();
			}
		}
	}
}
