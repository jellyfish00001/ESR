using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace ERS.Services
{
    public class PermissionTest
    {
        private HttpClient _httpClient;
        public PermissionTest()
        {
            _httpClient = new HttpClient();
            string token = "eyJhbGciOiJSUzI1NiIsImtpZCI6ImE3YzBhYzAyNGZhYWQ1NTY2ZjljNDVhZTJjYmJlNmJlIiwidHlwIjoiYXQrand0In0.eyJuYmYiOjE2NzI4ODk2NTIsImV4cCI6MTY3Mjg5MzI1MiwiaXNzIjoiaHR0cHM6Ly9hdXRod3Qud2lzdHJvbi5jb20vYXV0aCIsImNsaWVudF9pZCI6ImlkZW50aXR5LW4iLCJzdWIiOiJaMTkwODE5NjEiLCJhdXRoX3RpbWUiOjE2NzI4ODk2NTAsImlkcCI6ImxvY2FsIiwibmFtZSI6IkNvZHkgSHVhbmciLCJlbWFpbCI6IkNvZHlfSHVhbmdAd2lzdHJvbi5jb20iLCJsb2NhdGlvbiI6IkExMyIsImFjdF9zdWIiOiIiLCJzaWQiOiJDNEE2RjdFOEI5MjVBNThBNjU4MzdCRDlGOEYxNzY5OSIsImlhdCI6MTY3Mjg4OTY1Miwic2NvcGUiOlsib3BlbmlkIiwicHJvZmlsZSIsImFwaTEiXSwiYW1yIjpbInB3ZCJdfQ.D_2Z8Wup3WtzvwLZnV1WpeqYjXJtAEZYNvPeJRmwsCvjf2ZwF_0v7w-ZL_vyK-nEgx3QWKlf9mrX34YDf9GVvxtOzSArOi5faM2zVBxOPOwL4hGr-8eyKhuAifr7OtU-aZYACMmfPkpzYD70paTk3UpFfYqOI2us5Sv-MrDRiMC8p5kRH5HKM7LOJCGc5T9cdImt6FjPuxos4WqoGBp9z8BvE_D4p-OhaPSDMjqRxxbwXhz7zMisdoDL1LTaJKD6ctJKBOTvZy-SWCKSvC9nigCn7SUaJQ4AEjobeFqQnLxk149uxFOaly-Zuxr02Cl_7u3Ry6uZeFBEjGbI4kt-og";
            _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
        }

        // qas : bfd12d81-c5f8-40ff-9b04-483d03601295
        // prd : b2477a1e-873d-42fc-b5cc-d0f267e01b45

        //[Theory]
        //[InlineData("4a80e567-5355-446a-b64a-5a791587ef22", "https://authdwt.wistron.com/PortalAPI/ClientResource/SaveClientResource")]
        //public async Task insert_client_resource_idm_dev(string clientid, string url)
        //{
        //    List<ClientResource> data = JsonHelper.Read<ClientResource>(Path.Combine("Json", "permission.json"));
        //    foreach (var item in data)
        //    {
        //        item.clientId = clientid;
        //        string stringJson = JsonConvert.SerializeObject(item);
        //        var temp = await PostHelperAsync(url, stringJson);
        //    }

        //}

        //[Theory]
        //[InlineData("5e2843c3-fd53-4cf5-912d-3e9cc53dfce1", "https://authqwt.wistron.com/PortalAPI/ClientResource/SaveClientResource")]
        //public async Task insert_client_resource_idm_qas(string clientid, string url)
        //{
        //    List<ClientResource> data = JsonHelper.Read<ClientResource>(Path.Combine("Json", "permission.json"));
        //    foreach (var item in data)
        //    {
        //        item.clientId = clientid;
        //        string stringJson = JsonConvert.SerializeObject(item);
        //        var temp = await PostHelperAsync(url, stringJson);
        //    }

        //}

        //[Theory]
        //[InlineData("06ebb004-0ee9-48f4-9391-3230067acfa7", "https://authwt.wistron.com/PortalAPI/ClientResource/SaveClientResource")]
        //public async Task insert_client_resource_idm_prd(string clientid, string url)
        //{
        //    List<ClientResource> data = JsonHelper.Read<ClientResource>(Path.Combine("Json", "permission.json"));
        //    foreach (var item in data)
        //    {
        //        item.clientId = clientid;
        //        string stringJson = JsonConvert.SerializeObject(item);
        //        var temp = await PostHelperAsync(url, stringJson);
        //    }

        //}


        private async Task<string> PostHelperAsync(string url, string json, string ContentType = "application/json")
        {
            HttpContent content = new StringContent(json);
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(ContentType);
            HttpResponseMessage response = await _httpClient.PostAsync(url, content);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            return responseBody;
        }

        public List<T> ReadJsonFile<T>(string path)
        {
            JsonSerializer serializer = new JsonSerializer();
            using (var file = new StreamReader(Path.Combine(Directory.GetCurrentDirectory(), path)))
            using (var reader = new JsonTextReader(file))
            {
                return (List<T>)serializer.Deserialize(reader, typeof(List<T>));
            }
        }


        private class ClientResource
        {
            public bool isMenuItem { get; set; }
            public string clientId { get; set; }
            public string icon { get; set; }
            public string program { get; set; }
            public string action { get; set; }
            public string actionName { get; set; }
            public string url { get; set; }
        }
    }
}
