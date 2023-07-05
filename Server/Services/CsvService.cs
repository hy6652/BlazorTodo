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
            filePath = "C:\\Users\\고현영\\Desktop\\Blazor\\Server\\Services\\";
        }

        public async Task WriteCsv(CsvDto dto)
        {
            var records = dto.Records;
            var fileName = dto.FileName;

            var path = filePath + fileName;
            using (var writer = new StreamWriter(path))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(records);
            }
        }

        public async Task<List<CsvModel>> ReadCsv()
        {
            List<CsvModel> data = new List<CsvModel>();

            var file = filePath + "MovieList.csv";
            using (var reader = new StreamReader(file))
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

        public async Task<List<CsvModel>> ReadSelectedCsv(IFormFile file)
        {
            List<CsvModel> data = new List<CsvModel>();

            using (var reader = new StreamReader(file.OpenReadStream()))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csv.Read();
                csv.ReadHeader();
                while (csv.Read())
                {
                    var record = csv.GetRecord<CsvModel>();
                    data.Add(record);
                }
            }
            return data;
        }
    }
}
