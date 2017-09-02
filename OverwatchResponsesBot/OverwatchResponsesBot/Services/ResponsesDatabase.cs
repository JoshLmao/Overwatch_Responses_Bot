using OverwatchResponsesBot.Models;
using System.Collections.Generic;

namespace OverwatchResponsesBot.Services
{
    public class ResponsesDatabase
    {
        /// <summary>
        /// Dictionary of formatted voice lines & their URL
        /// </summary>
        public static List<CharacterResponse> Responses = null;

        public static void SetDatabase(List<CharacterResponse> database)
        {
            Debug.LogImportant("Database has been updated with the latest");
            Responses = database;
        }
    }
}
