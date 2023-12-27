using System;
using System.Globalization;
using System.Linq;
using System.Text;
using Microsoft.International.Converters.PinYinConverter;

namespace EmbeddedChineseCharacter
{
    public static class EmbeddedChineseCharacter
    {
        public static bool IsChineseCharacter(char character)
        {
            // 判断字符是否为字母，并且属于中文文化
            return
                char.IsLetter(character)
                && char.GetUnicodeCategory(character) == UnicodeCategory.OtherLetter
                && (
                    string.Compare(
                        character.ToString(CultureInfo.InvariantCulture),
                        "\u4E00",
                        StringComparison.Ordinal
                    ) >= 0
                )
                && (
                    string.Compare(
                        character.ToString(CultureInfo.InvariantCulture),
                        "\u9FA5",
                        StringComparison.Ordinal
                    ) <= 0
                );
        }

        public static bool IsDigit(char character)
        {
            // 使用char.IsDigit方法来判断字符是否为数字
            return char.IsDigit(character);
        }

        public static bool IsLetter(char character)
        {
            return char.IsLetter(character);
        }

        public static string RemoveNonAlphabetic(string input)
        {
            var result = new StringBuilder();

            foreach (var c in input.Where(char.IsLetter))
            {
                result.Append(c);
            }

            return result.ToString();
        }

        public static string GetPinyin(char chinese)
        {
            var chineseChar = new ChineseChar(chinese);
            var pinyin = chineseChar.Pinyins[0];
            return RemoveNonAlphabetic(pinyin).ToUpper().Trim();
        }

        public static string GetUniqueIdentification(string fileName)
        {
            var resultStringBuilder = new StringBuilder();

            var lastType = 0;

            foreach (var c in fileName)
            {
                var currentString = "";

                var currentType = 0;

                {
                    if (IsChineseCharacter(c))
                    {
                        currentString = GetPinyin(c);
                        currentType = 1;
                    }
                    else if (IsDigit(c))
                    {
                        currentString = c.ToString();
                        currentType = 2;
                    }
                    else if (IsLetter(c))
                    {
                        currentString = c.ToString();
                        currentType = 3;
                    }
                    else if (c == ' ' || c == '.')
                    {
                        currentString = "_";
                        currentType = 4;
                    }

                    if (currentType == 0)
                    {
                        continue;
                    }
                }

                if (
                    (
                        lastType != currentType ||
                        (lastType == currentType && lastType == 1)
                    ) && lastType != 0
                )
                {
                    resultStringBuilder.Append("_");
                }

                lastType = currentType;

                Console.WriteLine($"{c}:{currentString}");
                resultStringBuilder.Append(currentString);
            }

            var finalString = resultStringBuilder.ToString().ToUpper();

            // if (IsDigit(finalString[0]))
            // {
            //     finalString = "_" + finalString;
            // }

            while (finalString.Contains("__"))
            {
                finalString = finalString.Replace("__", "_");
            }

            return finalString.Trim();
        }

        public static string GetFileNameByPath(string path)
        {
            // Get FileName With Ext
            return FixPathSlash(path).Split('\\').Last();
        }

        public static string FixPathSlash(string path)
        {
            var returnString = path;

            while (returnString.Contains("/"))
            {
                returnString = returnString.Replace("/", "\\");
            }

            while (returnString.Contains(@"\\"))
            {
                returnString = returnString.Replace(@"\\", "\\");
            }

            return returnString;
        }
    }
}