﻿using LingvoNET;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Media.Imaging;

namespace PLSE_MVVMStrong.Model
{
    public static class StringUtil
    {
        /// <summary>
        /// Согласные буквы русского алфавита.
        /// </summary>
        public static string ConsonantLetters = "бвгджзклмнпрстфхцчшщй";
        /// <summary>
        /// Гласные буквы русского алфавита.
        /// </summary>
        public static string VowelLetters = "аеёиоуэюяы";
        /// <summary>
        /// Щипящие буквы русского алфавита.
        /// </summary>
        public static string HissingLetters = "гкхжшщч";
        /// <summary>
        /// Переворачивает строку
        /// </summary>
        /// <returns>Новая перевернутая строка</returns>
        static public string StrReverse(this string str)
        {
            return new string(str?.Reverse().ToArray());
        }
        /// <summary>
        /// Ищет первую букву в исходной строке и переводит ее в верхний регистр
        /// </summary>
        /// <param name="str">Исходная строка</param>
        /// <returns>Если буква не обнаружена или исходная строка пустая, возвращается исходная строка</returns>
        static public string ToUpperFirstLetter(this string str)
        {
            if (String.IsNullOrWhiteSpace(str)) return str;
            int i;
            for (i = 0; i < str.Length; i++)
            {
                if (Char.IsLetter(str[i])) return str.Remove(i, 1).Insert(i, str.Substring(i, 1).ToUpper());
            }
            return str;
        }
        /// <summary>
        /// Возвращает <paramref name="cnt"/> последних символов строки.
        /// </summary>
        /// <param name="cnt">Целое, указывающее количесво символов для возврата.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="cnt"/> меньше или равен 0</exception>
        static public string LastRight(this string str, int cnt = 1)
        {
            if (String.IsNullOrEmpty(str)) return str;
            if (str.Length < cnt) cnt = str.Length;
            if (cnt <= 0) throw new ArgumentOutOfRangeException();
            return str.Substring(str.Length - cnt);
        }
        static public string PositionReplace(this string sourse, string str, int pos)
        {
            if (pos < 0 || pos >= sourse.Length) throw new ArgumentOutOfRangeException();
            if (str == null) throw new ArgumentNullException();
            return sourse.Substring(0, pos) + str;
        }//заменяет sourse с позиции pos строкой str
        static public bool ContainWithComparison(this string source, string str, StringComparison comparison)
        {
            if (str == null) throw new ArgumentException("Искомая строка не может быть null");
            if (!Enum.IsDefined(typeof(StringComparison), comparison)) throw new ArgumentException("Неопределенный метод ставнения");
            if (source == null) return false;
            return source.IndexOf(str, comparison) >= 0;
        }
        static public BitmapImage Bitmap2BitmapImage(Bitmap bitmap)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                bitmap.Save(ms, ImageFormat.Jpeg);
                ms.Position = 0;
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.StreamSource = ms;
                bi.EndInit();
                return bi;
            }
        }
        /// <summary>
        /// Возвращает только цифровые символы из исходной строки
        /// </summary>
        /// <returns>Строка, содержащая только цифры или пустая строка</returns>
        public static string OnlyDigits(this string s)
        {
            if (string.IsNullOrEmpty(s)) return string.Empty;
            return new string(s.Where(n => Char.IsDigit(n)).ToArray());
        }
        /// <summary>
        /// Убирает все пробелы из исходной строки
        /// </summary>
        /// <returns>Строка без пробелов</returns>
        public static string SpaceFree(this string s)
        {
            return s == null ? null : s.Replace(" ", "");
        }
        public static string BeforeFirstDot(this string s)
        {
            int posdot = s.IndexOf('.');
            if (posdot < 0) return s;
            else return s.Substring(0, posdot + 2);
        }
        /// <summary>
        /// Равно ли количество гласных букв в слове 1?
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsOneVowelLetter(this string str)
        {
            return str.Where(n => VowelLetters.Contains(n)).Count() == 1;
        }
        public static string Joining(string str, LingvoNET.Case @case)
        {
            StringBuilder sb = new StringBuilder();
            var mch = Regex.Matches(str, "[\"0-9.а-я№]+", RegexOptions.IgnoreCase);
            var col = mch.Cast<Match>();
            foreach (var item in col.Take(1))
            {
                var f = Adjectives.FindOne(item.ToString());
                if (f != null)
                {
                    sb.Append(f[@case, Gender.M]);
                    sb.Append(" ");
                    continue;
                }
                var s = Nouns.FindOne(item.ToString());
                if (s != null)
                {
                    sb.Append(s[@case]);
                    sb.Append(" ");
                    continue;
                }
                var ss = Nouns.FindSimilar(sourceForm: item.ToString(), animacy: Animacy.Animate);
                if (ss != null)
                {
                    sb.Append(ss[@case]);
                    sb.Append(" ");
                    continue;
                }
                sb.Append(item.ToString());
                sb.Append(" ");
            }
                foreach (var item in col.Skip(1))
            {
                var f = Adjectives.FindOne(item.ToString());
                if (f != null)
                {
                    sb.Append(f[@case, Gender.M]);
                    sb.Append(" ");
                    continue;
                }
                var s = Nouns.FindOne(item.ToString());
                if (s != null)
                {
                    sb.Append(s[@case]);
                    sb.Append(" ");
                    continue;
                }
                sb.Append(item.ToString());
                sb.Append(" ");
            }
            return sb.ToString();
        }
        public static string DeclineAsNoun(this string str, LingvoNET.Case @case)
        {
            if (str == null) return null;
            //return Regex.Replace(str,
            //                @"«.+?»|[а-я]+", declinated, RegexOptions.IgnoreCase);

        }
       
    }

    public static class DateUtil
    {
        public static bool IsAnnualDate(DateTime? date)
        {
            if (date.HasValue)
            {
                return date.Value.Day == DateTime.Today.Day && date.Value.Month == DateTime.Now.Month;
            }
            else return false;
        }

        public static bool IsJubileeDate(DateTime? date, params int[] jubdate)
        {
            if (IsAnnualDate(date))
            {
                foreach (var item in jubdate)
                {
                    if ((date.Value.Year - DateTime.Now.Year) % item == 0) return true;
                }
            }
            return false;
        }
    }
    public static class Declination
    {
        public static string DeclineSpeciality(this Speciality sp, LingvoNET.Case @case)
        {
            int pos1 = sp.Species?.IndexOf("экспертиза") ?? -1;
            if (pos1 < 0)
            {
                return null;
            }
            int pos2 = pos1 + 10;
            string part1, part2, part3;
            part1 = sp.Species.Substring(0, pos1);
            part2 = "экспертиза";
            part3 = sp.Species.Substring(pos2);
            var noun = Nouns.FindOne(part2);
            return part1.Decline(@case) + noun[@case] + part3;
        }
        public static Tuple<string, string, string> DevideByWord(this string str, string wrd)
        {
            int pos1 = str?.IndexOf(wrd) ?? -1;
            if (pos1 < 0)
            {
                return new Tuple<string, string, string>(null, null, null);
            }
            int pos2 = pos1 + wrd.Length;
            return new Tuple<string, string, string>(str.Substring(0, pos1), wrd, str.Substring(pos2));
        }
        //public static string DeclineAsAdjective(string[] wrd, LingvoNET.Case @case)
        //{
        //    foreach (var item in collection)
        //    {

        //    }
        //}
        public static string DeclineBeforeNoun(this string str, LingvoNET.Case @case)
        {
            if (str == null) return null;
            LingvoNET.Noun n = null;
            //var words = Regex.Split(str, @"[,.:; ]", RegexOptions.IgnoreCase);
            var words = str.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            var pos = 0;
            foreach (var item in words)
            {
                n = Nouns.FindOne(item);
                if (n != null) break;
                pos++;
            }
            if (n != null)
            {
                for (; pos > 0; pos--)
                {
                    try
                    {
                        switch (@case)
                        {
                            case LingvoNET.Case.Nominative:
                                break;
                            case LingvoNET.Case.Genitive:
                                words[pos-1] = Adjective.AdjectiveToGenetive(words[pos-1]);
                                break;
                            case LingvoNET.Case.Dative:
                                words[pos -1] = Adjective.AdjectiveToDative(words[pos-1]);
                                break;
                            case LingvoNET.Case.Accusative:
                            case LingvoNET.Case.Instrumental:
                            case LingvoNET.Case.Locative:
                            case LingvoNET.Case.Short:
                            case LingvoNET.Case.Undefined:
                                throw new NotImplementedException("Запрашиваемый падеж для склонения не реализован");
                        }
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
                return String.Join(" ", words);
            }
            else throw new NotSupportedException("Не удалось выявить существительное в предложении");
        }
    }
}