using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ERS
{
    public static class HttpClientExtension
    {
        public static async Task<Stream> GetFileToStreamAsync(this HttpClient httpClient, string url, MultipartFormDataContent data)
        {
            HttpResponseMessage response = await httpClient.PostAsync(url, data);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                return default(Stream);
            }
            Stream result = await response.Content.ReadAsStreamAsync();
            return result;
        }
        public static async Task<T> PostHelperAsync<T>(this HttpClient httpClient, string url, string json, string ContentType = "application/json")
        {
            HttpContent content = new StringContent(json);
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(ContentType);
            HttpResponseMessage response = await httpClient.PostAsync(url, content);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                return default(T);
            }
            string result = await response.Content.ReadAsStringAsync();
            T res = JsonConvert.DeserializeObject<T>(result);
            return res;
        }
        public static async Task<byte[]> GetFileToByteAsync(this HttpClient httpClient, string url, MultipartFormDataContent data)
        {
            HttpResponseMessage response = await httpClient.PostAsync(url, data);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                return default(byte[]);
            }
            byte[] result = await response.Content.ReadAsByteArrayAsync();
            return result;
        }
        public static async Task<T> PostHelperAsync<T>(this HttpClient httpClient, string url, MultipartFormDataContent data)
        {
            HttpResponseMessage response = await httpClient.PostAsync(url, data);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                return default(T);
            }
            string result = await response.Content.ReadAsStringAsync();
            T res = JsonConvert.DeserializeObject<T>(result);
            return res;
        }
        public static async Task<T> GetHelperAsync<T>(this HttpClient httpClient, string url, string json, string token)
        {
            Dictionary<string, string> dic = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            StringBuilder builder = new StringBuilder();
            builder.Append(url);
            if (dic != null && dic.Count > 0)
            {
                builder.Append("?");
                int i = 0;
                foreach (var item in dic)
                {
                    if (i > 0)
                        builder.Append("&");
                    builder.AppendFormat("{0}={1}", item.Key, item.Value);
                    i++;
                }
            }
            httpClient.DefaultRequestHeaders.Add("Authorization", token);
            HttpResponseMessage response = await httpClient.GetAsync(builder.ToString());
            if (response.StatusCode != HttpStatusCode.OK)
            {
                return default(T);
            }
            string result = response.Content.ReadAsStringAsync().Result;
            T res = JsonConvert.DeserializeObject<T>(result);
            return res;
        }
        public static async Task<T> PostHelperAsync<T>(this HttpClient httpClient, string url, string json, string token, string ContentType = "application/json")
        {
            HttpContent content = new StringContent(json);
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(ContentType);
            httpClient.DefaultRequestHeaders.Add("Authorization", token);
            HttpResponseMessage response = await httpClient.PostAsync(url, content);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                return default(T);
            }
            string result = await response.Content.ReadAsStringAsync();
            T res = JsonConvert.DeserializeObject<T>(result);
            return res;
        }
     

        public static async Task<T> PostHelperAsync<T>(this HttpClient httpClient, string url, Dictionary<string, string> formData)
        {
            var content = new FormUrlEncodedContent(formData);
            HttpResponseMessage response = await httpClient.PostAsync(url, content);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                return default(T);
            }
            string result = await response.Content.ReadAsStringAsync();
            T res = JsonConvert.DeserializeObject<T>(result);
            return res;
        }
        public static async Task<T> PostHelperAsync<T>(this HttpClient httpClient, string url, List<KeyValuePair<string, string>> formData)
        {
            var content = new FormUrlEncodedContent(formData);
            HttpResponseMessage response = await httpClient.PostAsync(url, content);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                return default(T);
            }
            string result = await response.Content.ReadAsStringAsync();
            T res = JsonConvert.DeserializeObject<T>(result);
            return res;
        }

        public static async Task<T> PostHelperAsyncForMcp<T>(this HttpClient httpClient, string url, string body,  string ContentType = "text/plain")
        {
            HttpContent content = new StringContent(body, Encoding.UTF8, ContentType);
            HttpResponseMessage response = await httpClient.PostAsync(url, content);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                return default(T);
            }
            string result = await response.Content.ReadAsStringAsync();
            T res = JsonConvert.DeserializeObject<T>(result);
            return res;

        }
        public static async Task<T> PostHelperNoTokenAsync<T>(this HttpClient httpClient, string url, string json, string ContentType = "application/json")
        {
            HttpContent content = new StringContent(json);
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(ContentType);
            HttpResponseMessage response = await httpClient.PostAsync(url, content);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                return default(T);
            }
            string result = await response.Content.ReadAsStringAsync();
            T res = JsonConvert.DeserializeObject<T>(result);
            return res;
        }
        public static async Task<T> PostHelperNoTokenAsync<T>(this HttpClient httpClient, string url, string messageID, FileInfo fileInfo)
        {
            MultipartFormDataContent form = new MultipartFormDataContent();
            // 添加文件字段
            using (FileStream fileStream = fileInfo.OpenRead())
            {
                byte[] fileBytes = new byte[fileStream.Length];
                await fileStream.ReadAsync(fileBytes, 0, fileBytes.Length);

                ByteArrayContent fileContent = new ByteArrayContent(fileBytes);
                form.Add(fileContent, "File", Path.GetFileName(fileInfo.FullName));
            }
            form.Add(new StringContent(messageID), "messageID");

            HttpResponseMessage response = await httpClient.PostAsync(url, form);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                return default(T);
            }

            string result = await response.Content.ReadAsStringAsync();
            T res = JsonConvert.DeserializeObject<T>(result);
            return res;
        }
        public static async Task<T> PostHelperNoTokenAsync<T>(this HttpClient httpClient, string url, string messageID, string base64String, string fileName)
        {
            MultipartFormDataContent form = new MultipartFormDataContent();

            // 将Base64字符串转换为字节数组
            byte[] fileBytes = Convert.FromBase64String(base64String);

            // 创建文件内容的 ByteArrayContent
            ByteArrayContent fileContent = new ByteArrayContent(fileBytes);

            // 将文件内容添加到表单中，并指定文件名
            form.Add(fileContent, "File", fileName);
            form.Add(new StringContent(messageID), "messageID");

            HttpResponseMessage response = await httpClient.PostAsync(url, form);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                return default(T);
            }

            string result = await response.Content.ReadAsStringAsync();
            T res = JsonConvert.DeserializeObject<T>(result);
            return res;
        }


        public static byte[] ConvertFileInfoToByteArray(FileInfo fileInfo)
        {
            byte[] data;

            using (FileStream fileStream = fileInfo.OpenRead())
            {
                data = new byte[fileStream.Length];
                fileStream.Read(data, 0, data.Length);
            }

            return data;
        }


    }
}
