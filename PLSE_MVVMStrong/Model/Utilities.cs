﻿using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;

namespace PLSE_MVVMStrong.Model
{
    public static class StringUtil
    {
        /// <summary>
        /// Переворачивает строку
        /// </summary>
        /// <param name="str">Исходная строка</param>
        /// <returns>Новая перевернутая строка</returns>
        static public string StrReverse(this string str)
        {
            return new string(str?.Reverse().ToArray());
        }

        /// <summary>
        /// Ищет первую букву в исходной строке и переводит ее в верхний регистр
        /// </summary>
        /// <param name="str">Исходная строка</param>
        /// <returns>String</returns>
        static public string ToUpperFirstLetter(this string str)
        {
            if (String.IsNullOrWhiteSpace(str)) return str;//throw new ArgumentException("Аргумент является пустой строкой или состоящей только из символов разделителей");
            int i;
            for (i = 0; i < str.Length; i++)
            {
                if (Char.IsLetter(str[i])) return str.Remove(i, 1).Insert(i, str.Substring(i, 1).ToUpper()); ;
            }
            throw new ArgumentException("Строка не содержит букв");
        }

        static public string LastRight(this string str, int cnt)
        {
            if (str.Length < cnt) cnt = str.Length;
            if (cnt <= 0) throw new ArgumentOutOfRangeException();
            return str.Substring(str.Length - cnt);
        }//возвращает cnt последних символов

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
        /// <param name="s">Исходная строка</param>
        /// <returns>String</returns>
        public static string OnlyDigits(this string s)
        {
            return new string(s.Where(n => Char.IsDigit(n)).ToArray());
        }

        /// <summary>
        /// Убирает все пробелы из исходной строки
        /// </summary>
        /// <param name="s">Исходная строка</param>
        /// <returns></returns>
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
}