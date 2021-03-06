﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OverwatchResponsesBot.Utility
{
    class MessageModifier
    {
        /// <summary>
        /// Remove specific markdown characters to make a successful comparison and returns it
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static string RemoveMarkdownCharacters(string message)
        {
            if (string.IsNullOrEmpty(message))
                return message;

            string[] formatting = new string[] { "*", "#", "^", ">", "&gt;" };
            for (int i = 0; i < formatting.Length; i++)
                message = message.Replace(formatting[i], "");
            return message;
        }

        /// <summary>
        /// Removes all unicode characters like emojis and returns it
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static string RemoveUnicodeCharacters(string message)
        {
            if (string.IsNullOrEmpty(message))
                return message;

            return Regex.Replace(message, @"[^\u0000-\u007F]+", string.Empty);
        }

        /// <summary>
        /// Checks if string has a white space at start or end. If so, removes them and returns it
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static string RemoveWhiteSpaceAtStartAndEnd(string message)
        {
            if (string.IsNullOrEmpty(message))
                return message;

            while (!string.IsNullOrEmpty(message) && message.First() == ' ')
            {
                message = message.Remove(0, 1);
            }
            while (!string.IsNullOrEmpty(message) && message.Last() == ' ')
            {
                message = message.Remove(message.Length - 1, 1);
            }

            return message;
        }

        public static bool IsLastCharPunctuation(string message)
        {
            if (string.IsNullOrEmpty(message))
                return false;

            return char.IsPunctuation(message[message.Length - 1]);
        }
    }
}
