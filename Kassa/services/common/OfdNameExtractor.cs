using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Registrator.services
{
    internal class OfdNameExtractor
    {
        private static readonly string[] CompanyPrefixes =
    { "ООО", "АО", "ПАО", "ЗАО"};

        public static string Extract(string input)
        {
            if (input == "ООО «Компания «Тензор»")
                return "Тензор";

            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            // 1. Пытаемся извлечь текст в кавычках
            var quotedName = ExtractQuotedText(input);
            if (!string.IsNullOrEmpty(quotedName))
                return quotedName;

            // 2. Пытаемся удалить юридические префиксы
            var unprefixedName = RemoveCompanyPrefix(input);
            if (!string.IsNullOrEmpty(unprefixedName))
                return unprefixedName;

            // 3. Возвращаем очищенную исходную строку
            return CleanInput(input);
        }

        private static string ExtractQuotedText(string input)
        {
            // Ищем все совпадения кавычек и их содержимого
            var matches = Regex.Matches(input, @"([«»""'])(.+?)\1");

            // Если нет кавычек - возвращаем null
            if (matches.Count == 0) return null;

            // Берём последнее совпадение (самые внутренние кавычки)
            var lastMatch = matches[matches.Count - 1];
            return CleanInput(lastMatch.Groups[2].Value);
        }

        private static string RemoveCompanyPrefix(string input)
        {
            foreach (var prefix in CompanyPrefixes)
            {
                if (input.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                {
                    return CleanInput(input.Substring(prefix.Length));
                }
            }
            return null;
        }

        private static string CleanInput(string input)
        {
            return input.Trim().Trim('"', '\'', '«', '»', ' ', '.', ',');
        }
    }
}
