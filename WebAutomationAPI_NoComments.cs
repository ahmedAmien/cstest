using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using CsvHelper;
using System.Globalization;

namespace WebAutomationAPI
{
    public class WebAutomationAPIClient
    {
        private const string API = "https://api.webautomation.io/api";
        private const string USER = "Your_username";
        private const string PASSWORD = "Your_password";
        private static readonly HttpClient client = new HttpClient();

        public static void Main(string[] args)
        {
            // You can call any of your methods here, for demonstration I'm calling GetAllBatches
            var apiClient = new WebAutomationAPIClient();
            apiClient.GetAllBatches().Wait();
        }

        public async Task GetAllBatches()
        {
            var url = $"{API}/batch/";
            var response = await client.GetStringAsync(url);
            Console.WriteLine(response);
        }

        public async Task GetBatchDetails()
        {
            var url = $"{API}/batch/status/";
            var data = new
            {
                options = new
                {
                    batchid = "1693464873416946832"
                }
            };
            var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(url, content);
            var result = await response.Content.ReadAsStringAsync();
            Console.WriteLine(result);
        }

        public async Task<Dictionary<string, object>> GetDataOfBatch()
        {
            var url = $"{API}/batch/data/";
            var data = new
            {
                options = new
                {
                    batchid = "1693569365463776731"
                }
            };
            var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(url, content);
            var result = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Dictionary<string, object>>(result);
        }

        public async Task<Dictionary<string, object>> GetDataOfOneRequestInBatch()
        {
            var url = $"{API}/batch/data/";
            var data = new
            {
                options = new
                {
                    requestid = "1693569365463776731-7"
                }
            };
            var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(url, content);
            var result = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Dictionary<string, object>>(result);
        }

        public Dictionary<string, object> CreateDataDict(Dictionary<string, string> row)
        {
            var start_date = $"{row["Pickup Date"]}T{row["Pickup Time"]}";
            var end_date = $"{row["Dropoff Date"]}T{row["Dropoff Time"]}";
            return new Dictionary<string, object>
            {
                { "url", new List<string> { "https://www.skyscanner.com.au" } },
                { "use_js_rendering", true },
                { "return_raw_data", false },
                { "options", new
                    {
                        domain = "www.skyscanner.com",
                        job_id = row["Request_id"],
                        site = "https://www.skyscanner.com.au/",
                        language = row["Language"].ToLower(),
                        airport_code = row["Pickup Location Code"],
                        currency = row["currency"],
                        start_date = start_date,
                        end_date = end_date
                    }
                }
            };
        }

        public async Task InsertYourBatchFromCSV()
        {
            var url = $"{API}/batch/";
            var dataList = new List<Dictionary<string, object>>();

            using (var reader = new StreamReader("path of the file"))
            using (var csv = new CsvReader(reader))
            {
                var records = csv.GetRecords<Dictionary<string, string>>();
                foreach (var record in records)
                {
                    dataList.Add(CreateDataDict(record));
                }
            }

            var content = new StringContent(JsonConvert.SerializeObject(dataList), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(url, content);
            Console.WriteLine($"resp status: {response.StatusCode}");
            Console.WriteLine($"resp status: {response.IsSuccessStatusCode}");
            Console.WriteLine($"resp test: {await response.Content.ReadAsStringAsync()}");
        }
    }
}
