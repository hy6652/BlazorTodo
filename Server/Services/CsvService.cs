using BlazorTodo.Shared;
using CsvHelper;
using System.Diagnostics;
using System.Globalization;

namespace BlazorTodo.Server.Services
{
    public class CsvService
    {
        private readonly string filePath;
        public CsvService()
        {
            filePath = "C:\\Users\\고현영\\Desktop\\Blazor\\Server\\Services\\MovieList.csv";
        }
        public async Task<string> WriteCsv(List<CsvModel> records)
        {
            using (var writer = new StreamWriter(filePath))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(records);
                writer.Flush();
            }

            string success = "SUCCESS";
            return success;
        }

        public async Task<List<CsvModel>> ReadCsv()
        {
            List<CsvModel> data = new List<CsvModel>();

            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csv.Read();
                csv.ReadHeader();
                while(csv.Read())
                {
                    var record = csv.GetRecord<CsvModel>();
                    data.Add(record);
                }
            }
            return data;
        }
    }
}
