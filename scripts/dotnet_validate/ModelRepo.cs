using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace dotnetValidate
{
    public class ModelRepo
    {
        private static readonly HttpClient client = new HttpClient();

        public static async Task<bool> IsModelInPublicRepo(string modelId)
        {
            return !String.IsNullOrWhiteSpace(await GetMissingModel(modelId));
        }
        public static async Task<string> GetMissingModel(string modelId)
        {
            string apiVersion = "2020-05-01-preview";
            string baseUrl = "https://repo.azureiotrepository.com";
            var url = $"{baseUrl}/Models/{modelId}?api-version={apiVersion}";

            try
            {
                var httpResponse = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
                if (httpResponse.IsSuccessStatusCode)
                {
                    return await httpResponse.Content.ReadAsStringAsync();
                }
                return String.Empty;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return String.Empty;
            }
        }
    }
}