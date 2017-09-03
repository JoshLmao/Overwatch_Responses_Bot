using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OverwatchResponsesBot.Models
{
    public class CharacterResponse
    {
        /// <summary>
        /// The character that says response
        /// </summary>
        public string Character { get; set; }
        /// <summary>
        /// The response, formatted.
        /// </summary>
        public string Response { get; set; }
        /// <summary>
        /// The url to listen to the response
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// The amount of times the response has been used
        /// </summary>
        public uint UseCount { get; set; }
        /// <summary>
        /// The string to compare to converted comments. Can be null
        /// </summary>
        public string ConvertedResponse { get; set; }

        public CharacterResponse(string character, string response, string url)
        {
            Character = character;
            Response = response;
            Url = url;
        }
    }
}
