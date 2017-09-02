using Newtonsoft.Json.Linq;
using OverwatchResponsesBot.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace OverwatchResponsesBot.Services
{
    class OverwatchGamepediaParser
    {
        //Note: cmLimit max is 500   
        static readonly int LIMIT = 500;
        readonly string API_RESPONSES_URL = $"api.php?action=query&list=categorymembers&cmlimit={LIMIT}&cmprop=title&format=json&cmtitle=";
        string m_category;

        public OverwatchGamepediaParser(string category)
        {
            m_category = category;
        }

        public List<CharacterResponse> Parse()
        {
            return GetResponses();
        }

        List<CharacterResponse> GetResponses()
        {
            List<CharacterResponse> responses = new List<CharacterResponse>();

            string heroCategoryReplaced = m_category.Replace(" ", "_");
            string pageUrl = $"{Constants.BASE_URL}/{API_RESPONSES_URL}{heroCategoryReplaced}";

            string categoriesJson = GetDataFromPage(pageUrl);
            JObject categoriesObject = GetJObjectFromJson(categoriesJson);

            //Get pages to query
            List<string> categoryNames = new List<string>();
            foreach(JToken value in categoriesObject["query"]["categorymembers"])
            {
                categoryNames.Add(value["title"].ToString());
            }

            foreach(string category in categoryNames)
            {
                string categoryReplaced = category.Replace(" ", "_");
                string pageJson = GetDataFromPage($"{Constants.BASE_URL}/{API_RESPONSES_URL}{categoryReplaced}");
                JObject heroObject = GetJObjectFromJson(pageJson);

                string charName = categoryReplaced.Split('_').First().Replace("Category:", "");
                foreach (JToken value in heroObject["query"]["categorymembers"])
                {
                    string titleValue = value["title"].ToString();
                    string withoutFile = titleValue.Replace("File:", "");
                    string withoutExtension = RemoveFileExtension(titleValue);
                    string withoutFileAndMp3 = withoutExtension = withoutExtension.Replace("File:", "");

                    string[] split = withoutFileAndMp3.Split('-');

                    //Skip file names not formatted correctly (HeroName - Response)
                    if (!withoutFileAndMp3.Contains('-') || !withoutFileAndMp3.Contains(charName))
                        continue;

                    string character = "";
                    string response = "";
                    if(split.Length == 1)
                    {
                        //If file name doesn't contains a '-'
                        var list = split.First().Split(' ').ToList();
                        list[1] = string.Join(" ", list.Skip(1).ToArray());
                        int index = 2;
                        list.RemoveRange(index, list.Count - index);
                        split = list.ToArray();
                    }

                    character =  split[0];
                    //Remove white space that was left when removing -
                    if (character.Last() == ' ')
                        character = character.Remove(character.Length - 1, 1);

                    response = string.Concat(split.Skip(1).ToArray());
                    if (response.First() == ' ')
                        response = response.Remove(0, 1);

                    string url = GetUrlFromFileName(titleValue);

                    //Skip WAVs since can't play em
                    if (Path.GetExtension(url) == ".wav")
                        continue;

                    responses.Add(new CharacterResponse(character, response, url));
                    Debug.Log($"Added New Response - {character}, {response}");
                }
            }

            return responses;
        }

        string RemoveFileExtension(string original)
        {
            string[] exts = new string[] { ".wav", ".ogg", ".mp3" };
            foreach(string ext in exts)
                original = original.Replace(ext, "");

            return original;
        }

        string GetDataFromPage(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Stream receiveStream = response.GetResponseStream();
                StreamReader readStream = null;

                if (response.CharacterSet == null)
                {
                    readStream = new StreamReader(receiveStream);
                }
                else
                {
                    readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
                }

                string data = readStream.ReadToEnd();

                response.Close();
                readStream.Close();

                return data;
            }
            return "";
        }

        JObject GetJObjectFromJson(string json)
        {
            return JObject.Parse(json);
        }

        /// <summary>
        /// Calls the Wiki again for the data related to that file. Parses the Url
        /// </summary>
        /// <param name="fullString"></param>
        /// <returns></returns>
        string GetUrlFromFileName(string title)
        {
            string url = $"{Constants.BASE_URL}/api.php?action=query&titles={title}&prop=imageinfo&iiprop=url&format=json";
            string json = GetDataFromPage(url);
            JObject jsonObject = GetJObjectFromJson(json);

            try
            {
                //Full path of json on each media page Query -> Pages -> Id of file -> ImageInfo -> Url
                foreach (var page in jsonObject["query"]["pages"])
                {
                    foreach (var child in page)
                        foreach (var image in child["imageinfo"])
                            return image["url"].ToString();
                }
            }
            catch (Exception e)
            {
                Debug.LogException($"Unable to load url for page '{title}'", e);
            }

            return "";
        }
    }
}
