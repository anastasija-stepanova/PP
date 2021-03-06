﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace pp_lab4
{
    public class Entities
    {

        public Entities() { }

        public string ReadFromFile(string filePath)
        {
            using (var reader = File.OpenText(filePath))
            {
                return reader.ReadToEnd();
            }
        }

        public async Task<string> ReadFromFileAsync(string filePath)
        {
            using (var reader = File.OpenText(filePath))
            {
                return await reader.ReadToEndAsync();
            }
        }

        public string Request(string path)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(path);
            request.Method = "GET";
            request.Accept = "application/json";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            return HandleResponse(response);
        }

        public string HandleResponse(WebResponse response)
        {
            using (Stream s = response.GetResponseStream())
            using (StreamReader sR = new StreamReader(s))
            {
                string content = sR.ReadToEnd();
                return content;
            }
        }

        public Task<string> RequestAsync(string path)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(path);
            request.Method = "GET";
            request.Accept = "application/json";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            Task<WebResponse> task = Task.Factory.FromAsync(request.BeginGetResponse, res => request.EndGetResponse(res), (object)null);

            return task.ContinueWith(t => HandleResponse(t.Result));
        }
    }
}
