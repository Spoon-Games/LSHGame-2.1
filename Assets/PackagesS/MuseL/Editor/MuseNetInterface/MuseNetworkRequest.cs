using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace MuseL
{
    public static class MuseNetworkRequest
    {
        static readonly HttpClient client;

        static MuseNetworkRequest()
        {
            client = new HttpClient(new HttpClientHandler() { Proxy = GetProxy() })
            {
                Timeout = new TimeSpan(0, 5, 0)
            };
        }

        private static IWebProxy GetProxy()
        {
            HttpWebRequest myWebRequest = (HttpWebRequest)WebRequest.Create("http://www.microsoft.com");

            // Obtain the 'Proxy' of the  Default browser.
            IWebProxy proxy = myWebRequest.Proxy;
            // Print the Proxy Url to the console.
            return proxy;
        }

        public static async Task<string[]> MakeMuseRequest(string[] encoding, string genre, Instruments instruments, int temperature,int trunication,CancellationToken cancellation)
        {
            MuseInput museInput = new MuseInput(genre, encoding, instruments, temperature, trunication);
            string request = JsonUtility.ToJson(museInput, true);
            //string request = "{\"genre\":\"video\",\"instrument\":{ \"piano\":true,\"strings\":true,\"winds\":true,\"drums\":true,\"harp\":false,\"guitar\":true,\"bass\":true},\"encoding\":\"\",\"temperature\":1,\"truncation\":27,\"generationLength\":225,\"audioFormat\":\"\"}";
            string response = await MakeHTTPRequest(request,cancellation);

            if (string.IsNullOrEmpty(response))
                return encoding;

            MuseOutput output = JsonUtility.FromJson<MuseOutput>(response);
            return output.completions[0].encoding.Split(' ');
        }

        private static async Task<string> MakeHTTPRequest(string jsonContent, CancellationToken token)
        {
            // Call asynchronous network methods in a try/catch block to handle exceptions.
            try
            {

                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                using (HttpResponseMessage response = await client.PostAsync("https://musenet.openai.com/sample", content,token)) {;
                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();
                    // Above three lines can be replaced with new helper method below
                    // string responseBody = await client.GetStringAsync(uri);
                    return responseBody;
                }
            }
            catch (Exception)
            {
                return "";
            }
        }

        public static void Test()
        {
            
        }

        private class MuseOutput
        {
            public Completion[] completions;    
        }

        [System.Serializable]
        private class Completion
        {
            public string encoding;
        }

        [System.Serializable]
        private class MuseInput
        {
            public string genre;
            public InstrumentStruct instrument;
            public string encoding;
            public int temperature;
            public int truncation;

            public int generationLength;
            public string audioFormat;

            public MuseInput(string genre, string[] encoding, Instruments instruments, int temperature, int trunication)
            {
                this.genre = genre;
                StringBuilder builder = new StringBuilder();
                foreach (var e in encoding)
                {
                    builder.Append(e);
                    builder.Append(' ');
                }
                this.encoding = builder.ToString();

                this.instrument = new InstrumentStruct(instruments);
                this.temperature = temperature;
                this.truncation = trunication;
                this.generationLength = 225;
                this.audioFormat = "";
            }
        }

        [System.Serializable]
        public class InstrumentStruct
        {
            public bool piano;
            public bool strings;
            public bool winds;
            public bool drums;
            public bool harp;
            public bool guitar;
            public bool bass;

            public InstrumentStruct(Instruments instruments)
            {
                this.piano = instruments.HasFlag(Instruments.piano);
                this.strings = instruments.HasFlag(Instruments.strings);
                this.winds = instruments.HasFlag(Instruments.winds);
                this.drums = instruments.HasFlag(Instruments.drums);
                this.harp = instruments.HasFlag(Instruments.harp);
                this.guitar = instruments.HasFlag(Instruments.guitar);
                this.bass = instruments.HasFlag(Instruments.bass);
            }
        }
    }
}
