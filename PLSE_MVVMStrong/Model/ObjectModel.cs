
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using LingvoNET;
using Microsoft.Office.Interop.Word;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace PLSE_MVVMStrong.Model
{
    public enum Version
    {
        Original = 0x0001,
        New = 0x0002,
        Edited = 0x0004,
        ContentEdited = 0x008
    }
    public enum DBAction : byte
    {
        Add = 0x00,
        Edit = 0x01,
        Delete = 0x02
    }
    public enum PermissionProfile
    {
        Admin,
        Boss,
        Subboss,
        Accountant,
        Expert,
        Laboratorian,
        Clerk,
        Staffinspector,
        Provisionboss,
        Rightless
    }
    public enum PartOfSpeech
        {
            None,
            Noun,
            Pronoun,
            Verb,
            Adjective,
            Numeral,
            Adverb,
            Preposition
        }
    internal enum WordGender
        {
            None,
            Neuter,
            Male,
            Female
        }
    internal sealed class Word
    {
        public readonly string _text;
        public readonly bool _IsDeclinated;
        public readonly WordGender _WordGender;
        public readonly PartOfSpeech _PartofSpeech;
        public readonly bool _HasRunawayVowel;
        public static readonly HashSet<Word> _exeptions;
        public static Word Determine(string str, PartOfSpeech expect = PartOfSpeech.None)
        {
            var x = _exeptions.FirstOrDefault(n => n._text == str.ToLower());
            if (x != null) return x;
            if (str.LastRight(2) == "ёк")
            {
                if ("лнр".Contains(str[str.PositionRunawayVowel() - 1])) return new Word(str, true, WordGender.Male, expect, true);
                else return new Word(str, true, WordGender.Male, expect, false);
            }
            if (str.LastRight(2) == "ок" || str.LastRight(2) == "ец" || str.LastRight(2) == "ек")
            {
                if (str.EndsWith("век")) return new Word(str, true, WordGender.Male, expect, false);
                int _pos = str.Length - 3, __pos = _pos - 1;
                if (_pos <= 0)
                {
                    return new Word(str, true, WordGender.Male, expect, false);
                }
                else
                {
                    if (!"ёйуеъыаоэяиью".Contains(str[_pos]) && !"ёйуеъыаоэяиью".Contains(str[__pos]))
                    {
                        return new Word(str, true, WordGender.Male, expect, false);
                    }
                    else return new Word(str, true, WordGender.Male, expect, true);
                }
            }
            return new Word(str, true, DetermineGender(str), expect, false);
        }
        public static WordGender DetermineGender(string str)
        {
            char[] consonantLetter = { 'ц', 'к', 'н', 'г', 'ш', 'щ', 'з', 'х', 'ф', 'в', 'п', 'р', 'л', 'д', 'ж', 'ч', 'с', 'м', 'т', 'б' };
            if (consonantLetter.Contains(str.Last()) || str.Last() == 'й')
            {
                return WordGender.Male;
            }
            if (str.Last() == 'ь')
            {
                if (str.LastRight(3) == "арь" || str.LastRight(4) == "тель") return WordGender.Male;
                else return WordGender.Female;
            }
            if (str.Last() == 'а' || str.Last() == 'я') return WordGender.Female;
            if (str.Last() == 'е' || str.Last() == 'о' || str.Last() == 'ё' || str.Last() == 'э') return WordGender.Neuter;
            return WordGender.None;
        }
        public static string ReplaceRunawayVowel(string str)
        {
            var p = str.PositionRunawayVowel();
            StringBuilder sb = new StringBuilder(str);
            if (str[str.PositionRunawayVowel() - 1] == 'л' && str[p] != 'о')
            {
                return sb.Replace(str[str.PositionRunawayVowel()], 'ь', str.PositionRunawayVowel(), 1).ToString();
            }
            if ("нр".Contains(str[str.PositionRunawayVowel() - 1]) && str[p] == 'ё')
            {
                return sb.Replace('ё', 'ь', str.PositionRunawayVowel(), 1).ToString();
            }
            if ("уеаоэяию".Contains(str[str.PositionRunawayVowel() - 1]))
            {
                return sb.Remove(p, 1).Insert(p, 'й').ToString();
            }
            return sb.Remove(p, 1).ToString();
        }
        public static string AdjectiveToGenetive(string str)
        {
            if (str.LastRight(2) == "ый")
            {
                if ("жшчщц".Contains(str[str.Length - 3]))
                {
                    return str.PositionReplace("его", str.Length - 2);
                }
                else
                {
                    return str.PositionReplace("ого", str.Length - 2);
                }
            }
            if (str.LastRight(2) == "ий")
            {
                if ("хгк".Contains(str[str.Length - 3]))
                {
                    return str.PositionReplace("ого", str.Length - 2);
                }
                else
                {
                    return str.PositionReplace("его", str.Length - 2);
                }
            }
            if (str.LastRight(2) == "ой")
            {
                return str.PositionReplace("ого", str.Length - 2);
            }
            if (str.LastRight(2) == "ая")
            {
                if ("жшчщц".Contains(str[str.Length - 3]))
                {
                    return str.PositionReplace("ей", str.Length - 2);
                }
                else
                {
                    return str.PositionReplace("ой", str.Length - 2);
                }
            }
            if (str.LastRight(2) == "яя")
            {
                if ("хгк".Contains(str[str.Length - 3]))
                {
                    return str.PositionReplace("ой", str.Length - 2);
                }
                else
                {
                    return str.PositionReplace("ей", str.Length - 2);
                }
            }
            else throw new NotImplementedException("(прил.) невозможно склонить в родительном падеже");
        }
        public static string AdjectiveToDative(string str)
        {
            if (str.LastRight(2) == "ый")
            {
                if ("жшчщц".Contains(str[str.Length - 3]))
                {
                    return str.PositionReplace("ему", str.Length - 2);
                }
                else
                {
                    return str.PositionReplace("ому", str.Length - 2);
                }
            }
            if (str.LastRight(2) == "ий")
            {
                if ("хгк".Contains(str[str.Length - 3]))
                {
                    return str.PositionReplace("ому", str.Length - 2);
                }
                else
                {
                    return str.PositionReplace("ему", str.Length - 2);
                }
            }
            if (str.LastRight(2) == "ой")
            {
                return str.PositionReplace("ого", str.Length - 2);
            }
            if (str.LastRight(2) == "ая")
            {
                if ("жшчщц".Contains(str[str.Length - 3]))
                {
                    return str.PositionReplace("ей", str.Length - 2);
                }
                else
                {
                    return str.PositionReplace("ой", str.Length - 2);
                }
            }
            if (str.LastRight(2) == "яя")
            {
                if ("хгк".Contains(str[str.Length - 3]))
                {
                    return str.PositionReplace("ой", str.Length - 2);
                }
                else
                {
                    return str.PositionReplace("ей", str.Length - 2);
                }
            }//добавить - ов -ев и т.д.
            else throw new NotImplementedException("(прил.) невозможно склонить в родительном падеже");
        }
        public static string NounToGenetive(string str)
        {
            var word = Word.Determine(str);
            if (word._IsDeclinated)
            {
                if (word._HasRunawayVowel)
                {
                    str = Word.ReplaceRunawayVowel(word._text);
                }
                string L1 = str.LastRight(1);
                if (word._WordGender == WordGender.Male)
                {
                    if (str == "путь") return "пути";
                    if (L1 == "ь" || L1 == "й") return str.PositionReplace("я", str.Length - 1);
                    if (L1 == "а")
                    {
                        if ("хгкжщчщ".Contains(str[str.Length - 2])) return str.PositionReplace("и", str.Length - 1);
                        else return str.PositionReplace("ы", str.Length - 1);
                    }
                    if (L1 == "я") return str.PositionReplace("и", str.Length - 1);
                    return str + "а";
                }
                if (word._WordGender == WordGender.Female)
                {
                    if (str == "мать" || str == "дочь") return str.PositionReplace("ери", str.Length - 1);
                    if (L1 == "я" || L1 == "ь")
                    {
                        return str.PositionReplace("и", str.Length - 1);
                    }
                    if (L1 == "а")
                    {
                        if ("хгкжщчщ".Contains(str[str.Length - 2])) return str.PositionReplace("и", str.Length - 1);
                        else return str.PositionReplace("ы", str.Length - 1);
                    }
                }
                if (word._WordGender == WordGender.Neuter)
                {
                    if (str.LastRight(2) == "мя") return str.PositionReplace("ени", str.Length - 1);
                    if (str == "дитя") return "дитяти";
                    if ((L1 == "е" || L1 == "ё") && !"жщчщь".Contains(str[str.Length - 2])) return str.PositionReplace("я", str.Length - 1);
                    else return str.PositionReplace("а", str.Length - 1);
                }
                throw new NotSupportedException("Склонение сущ. в родительном падеже невозможно");
            }
            else return str;
        }
        public static string NounToDative(string str)
        {
            var word = Word.Determine(str);
            if (word._IsDeclinated)
            {
                if (word._HasRunawayVowel)
                {
                    str = Word.ReplaceRunawayVowel(word._text);
                }
                string L1 = str.LastRight(1);
                if (word._WordGender == WordGender.Female)
                {
                    if (str == "мать" || str == "дочь") return str.PositionReplace("ери", str.Length - 1);
                    if (L1 == "ь" || str.LastRight(2) == "ия") return str.PositionReplace("и", str.Length - 1);
                    else return str.PositionReplace("е", str.Length - 1);
                }
                if (word._WordGender == WordGender.Male)
                {
                    if (str == "путь") return "пути";
                    if (L1 == "ь" || L1 == "й") return str.PositionReplace("ю", str.Length - 1);
                    if (L1 == "а" || L1 == "я") return str.PositionReplace("е", str.Length - 1);
                    else return str + "у";
                }
                if (word._WordGender == WordGender.Neuter)
                {
                    if (str.LastRight(2) == "мя") return str.PositionReplace("ени", str.Length - 1);
                    if (str == "дитя") return "дитяти";
                    if ((L1 == "е" || L1 == "ё") && !"жщчщ".Contains(str[str.Length - 2])) return str.PositionReplace("ю", str.Length - 1);
                    else return str.PositionReplace("у", str.Length - 1);
                }
                throw new NotImplementedException("Склонение сущ. в дательном падеже невозможно");
            }
            else
            {
                return str;
            }
        }
        public string ToGenetive()
        {
            string str = _text;
            switch (_PartofSpeech)
            {
                case PartOfSpeech.Noun:
                    if (_IsDeclinated)
                    {
                        if (_HasRunawayVowel)
                        {
                           str = Word.ReplaceRunawayVowel(_text);
                        }
                        string L1 = str.LastRight(1);
                        if (_WordGender == WordGender.Male)
                        {
                            if (str == "путь") return "пути";
                            if (L1 == "ь" || L1 == "й") return str.PositionReplace("я", str.Length - 1);
                            if (L1 == "а")
                            {
                                if ("хгкжщчщ".Contains(str[str.Length - 2])) return str.PositionReplace("и", str.Length - 1);
                                else return str.PositionReplace("ы", str.Length - 1);
                            }
                            if (L1 == "я") return str.PositionReplace("и", str.Length - 1);
                            return str + "а";
                        }
                        if (_WordGender == WordGender.Female)
                        {
                            if (str == "мать" || str == "дочь") return str.PositionReplace("ери", str.Length - 1);
                            if (L1 == "я" || L1 == "ь")
                            {
                                return str.PositionReplace("и", str.Length - 1);
                            }
                            if (L1 == "а")
                            {
                                if ("хгкжщчщ".Contains(str[str.Length - 2])) return str.PositionReplace("и", str.Length - 1);
                                else return str.PositionReplace("ы", str.Length - 1);
                            }
                        }
                        if (_WordGender == WordGender.Neuter)
                        {
                            if (str.LastRight(2) == "мя") return str.PositionReplace("ени", str.Length - 1);
                            if (str == "дитя") return "дитяти";
                            if ((L1 == "е" || L1 == "ё") && !"жщчщь".Contains(str[str.Length - 2])) return str.PositionReplace("я", str.Length - 1);
                            else return str.PositionReplace("а", str.Length - 1);
                        }
                        throw new NotSupportedException("Склонение сущес. в родительном падеже невозможно");
                    }
                    else return str;
                case PartOfSpeech.Adjective:
                    string l2 = str.LastRight(2);
                    if (l2 == "ый")
                    {
                        if ("жшчщц".Contains(str[str.Length - 3]))
                        {
                            return str.PositionReplace("его", str.Length - 2);
                        }
                        else
                        {
                            return str.PositionReplace("ого", str.Length - 2);
                        }
                    }
                    if (l2 == "ий")
                    {
                        if ("хгк".Contains(str[str.Length - 3]))
                        {
                            return str.PositionReplace("ого", str.Length - 2);
                        }
                        else
                        {
                            return str.PositionReplace("его", str.Length - 2);
                        }
                    }
                    if (l2 == "ой")
                    {
                        return str.PositionReplace("ого", str.Length - 2);
                    }
                    if (l2 == "ая")
                    {
                        if ("жшчщц".Contains(str[str.Length - 3]))
                        {
                            return str.PositionReplace("ей", str.Length - 2);
                        }
                        else
                        {
                            return str.PositionReplace("ой", str.Length - 2);
                        }
                    }
                    if (l2 == "яя")
                    {
                        if ("хгк".Contains(str[str.Length - 3]))
                        {
                            return str.PositionReplace("ой", str.Length - 2);
                        }
                        else
                        {
                            return str.PositionReplace("ей", str.Length - 2);
                        }
                    }
                    if (l2 == "ее")
                    {
                        if ("жшчщц".Contains(str[str.Length - 3]))
                        {
                            return str.PositionReplace("его", str.Length - 2);
                        }
                        else
                        {
                            return str.PositionReplace("ого", str.Length - 2);
                        }
                    }
                    if (l2 == "ое")
                    {
                        if ("жшчщц".Contains(str[str.Length - 3]))
                        {
                            return str.PositionReplace("его", str.Length - 2);
                        }
                        else
                        {
                            return str.PositionReplace("ого", str.Length - 2);
                        }
                    }
                    else throw new NotImplementedException("Склонение прилаг. в родительном падеже невозможно");
                default:
                    throw new NotSupportedException("Склонение не поддерживается для применяемой части речи");
            }
        }
        public string ToDative()
        {
            string str = _text;
            switch (_PartofSpeech)
            {
                case PartOfSpeech.Noun:
                    if (_IsDeclinated)
                    {
                        if (_HasRunawayVowel)
                        {
                            str = Word.ReplaceRunawayVowel(_text);
                        }
                        string L1 = str.LastRight(1);
                        if (_WordGender == WordGender.Female)
                        {
                            if (str == "мать" || str == "дочь") return str.PositionReplace("ери", str.Length - 1);
                            if (L1 == "ь" || str.LastRight(2) == "ия") return str.PositionReplace("и", str.Length - 1);
                            else return str.PositionReplace("е", str.Length - 1);
                        }
                        if (_WordGender == WordGender.Male)
                        {
                            if (str == "путь") return "пути";
                            if (L1 == "ь" || L1 == "й") return str.PositionReplace("ю", str.Length - 1);
                            if (L1 == "а" || L1 == "я") return str.PositionReplace("е", str.Length - 1);
                            else return str + "у";
                        }
                        if (_WordGender == WordGender.Neuter)
                        {
                            if (str.LastRight(2) == "мя") return str.PositionReplace("ени", str.Length - 1);
                            if (str == "дитя") return "дитяти";
                            if ((L1 == "е" || L1 == "ё") && !"жщчщ".Contains(str[str.Length - 2])) return str.PositionReplace("ю", str.Length - 1);
                            else return str.PositionReplace("у", str.Length - 1);
                        }
                        throw new NotImplementedException("Склонение сущ. в дательном падеже невозможно");
                    }
                    else return str;
                case PartOfSpeech.Adjective:
                    string l2 = str.LastRight(2);
                    if (l2 == "ый")
                    {
                        if ("жшчщц".Contains(str[str.Length - 3]))
                        {
                            return str.PositionReplace("ему", str.Length - 2);
                        }
                        else
                        {
                            return str.PositionReplace("ому", str.Length - 2);
                        }
                    }
                    if (l2 == "ий")
                    {
                        if ("хгк".Contains(str[str.Length - 3]))
                        {
                            return str.PositionReplace("ому", str.Length - 2);
                        }
                        else
                        {
                            return str.PositionReplace("ему", str.Length - 2);
                        }
                    }
                    if (l2 == "ой")
                    {
                        return str.PositionReplace("ого", str.Length - 2);
                    }
                    if (l2 == "ая")
                    {
                        if ("жшчщц".Contains(str[str.Length - 3]))
                        {
                            return str.PositionReplace("ей", str.Length - 2);
                        }
                        else
                        {
                            return str.PositionReplace("ой", str.Length - 2);
                        }
                    }
                    if (l2 == "яя")
                    {
                        if ("хгк".Contains(str[str.Length - 3]))
                        {
                            return str.PositionReplace("ой", str.Length - 2);
                        }
                        else
                        {
                            return str.PositionReplace("ей", str.Length - 2);
                        }
                    }
                    if (l2 == "ее")
                    {
                        if ("жшчщц".Contains(str[str.Length - 3]))
                        {
                            return str.PositionReplace("его", str.Length - 2);
                        }
                        else
                        {
                            return str.PositionReplace("ого", str.Length - 2);
                        }
                    }
                    if (l2 == "ое")
                    {
                        if ("жшчщц".Contains(str[str.Length - 3]))
                        {
                            return str.PositionReplace("ему", str.Length - 2);
                        }
                        else
                        {
                            return str.PositionReplace("ому", str.Length - 2);
                        }
                    }//добавить - ов -ев и т.д.
                    else throw new NotImplementedException("Склонение прилаг. в дательном падеже невозможно");
                default:
                    throw new NotSupportedException("Склонение не поддерживается для применяемой части речи");
            }
        }
        public Word(string word, bool decl, WordGender kind, PartOfSpeech part, bool runaway)
        {
            _text = word;
            _IsDeclinated = decl;
            _WordGender = kind;
            _PartofSpeech = part;
            _HasRunawayVowel = runaway;
        }
        static Word()
        {
            _exeptions = new HashSet<Word>()
                {
                    //сущ. м.р. с нестандартными окончаниями
                    new Word("папа", true, WordGender.Male, PartOfSpeech.Noun, false),
                    new Word("дядя", true, WordGender.Male, PartOfSpeech.Noun, false),
                    new Word("дедушка", true, WordGender.Male, PartOfSpeech.Noun, false),
                    new Word("прадедушка", true, WordGender.Male, PartOfSpeech.Noun, false),
                    new Word("атташе", false, WordGender.Male, PartOfSpeech.Noun, false),
                    new Word("денди", false, WordGender.Male, PartOfSpeech.Noun, false),
                    new Word("импресарио", false, WordGender.Male, PartOfSpeech.Noun, false),
                    new Word("кюре", false, WordGender.Male, PartOfSpeech.Noun, false),
                    new Word("портье", false, WordGender.Male, PartOfSpeech.Noun, false),
                    new Word("крупье", false, WordGender.Male, PartOfSpeech.Noun, false),
                    new Word("маэстро", false, WordGender.Male, PartOfSpeech.Noun, false),
                    new Word("конферансье", false, WordGender.Male, PartOfSpeech.Noun, false),
                    new Word("буржуа", false, WordGender.Male, PartOfSpeech.Noun, false),
                    new Word("кофе", false, WordGender.Male, PartOfSpeech.Noun, false),
                    //сущ. м.р. оканчивающиеся на -ь, кроме -арь, -тель
                    new Word("автомобиль", true, WordGender.Male, PartOfSpeech.Noun, false),
                    new Word("апрель", true, WordGender.Male, PartOfSpeech.Noun, false),
                    new Word("артикль", true, WordGender.Male, PartOfSpeech.Noun, false),
                    new Word("аэрозоль", true, WordGender.Male, PartOfSpeech.Noun, false),
                    new Word("бемоль", true, WordGender.Male, PartOfSpeech.Noun, false),
                    new Word("бинокль", true, WordGender.Male, PartOfSpeech.Noun, false),
                    new Word("богатырь", true, WordGender.Male, PartOfSpeech.Noun, false),
                    new Word("бюллетень", true, WordGender.Male, PartOfSpeech.Noun, false),
                    new Word("вестибюль", true, WordGender.Male, PartOfSpeech.Noun, false),
                    new Word("вихрь", true, WordGender.Male, PartOfSpeech.Noun, false),
                    new Word("водевиль", true, WordGender.Male, PartOfSpeech.Noun, false),
                    new Word("вождь", true, WordGender.Male, PartOfSpeech.Noun, false),
                    new Word("гармонь", true, WordGender.Male, PartOfSpeech.Noun, false),
                    new Word("гвоздь", true, WordGender.Male, PartOfSpeech.Noun, false),
                    new Word("главарь", true, WordGender.Male, PartOfSpeech.Noun, false),
                    new Word("глухарь", true, WordGender.Male, PartOfSpeech.Noun, false),
                    new Word("голубь", true, WordGender.Male, PartOfSpeech.Noun, false),
                    new Word("госпиталь", true, WordGender.Male, PartOfSpeech.Noun, false),
                    new Word("гость", true, WordGender.Male, PartOfSpeech.Noun, false),
                    new Word("гребень", true, WordGender.Male, PartOfSpeech.Noun, true),
                    new Word("гусь", true, WordGender.Male, PartOfSpeech.Noun, false),
                    new Word("дактиль", true, WordGender.Male, PartOfSpeech.Noun, false),
                    new Word("деготь", true, WordGender.Male, PartOfSpeech.Noun, true),
                    new Word("декабрь", true, WordGender.Male, PartOfSpeech.Noun, false),
                    new Word("день", true, WordGender.Male, PartOfSpeech.Noun, true),
                    new Word("дирижабль", true, WordGender.Male, PartOfSpeech.Noun, false),
                    new Word("дождь", true, WordGender.Male, PartOfSpeech.Noun, false),
                    new Word("егерь", true, WordGender.Male, PartOfSpeech.Noun, false),
                    new Word("жёлудь", true, WordGender.Male, PartOfSpeech.Noun, false),
                    new Word("журавль", true, WordGender.Male, PartOfSpeech.Noun, false),
                    new Word("зверь", true, WordGender.Male, PartOfSpeech.Noun, false),
                    new Word("зять", true, WordGender.Male, PartOfSpeech.Noun, false),
                    new Word("игорь", true, WordGender.Male, PartOfSpeech.Noun, false),
                    new Word("июль", true, WordGender.Male, PartOfSpeech.Noun, false),
                    new Word("июнь", true, WordGender.Male, PartOfSpeech.Noun, false),
                    new Word("кабель", true, WordGender.Male, PartOfSpeech.Noun, false),
                    new Word("камень", true, WordGender.Male, PartOfSpeech.Noun, true),
                    new Word("каракуль", true, WordGender.Male, PartOfSpeech.Noun, false),
                    new Word("карась", true, WordGender.Male, PartOfSpeech.Noun, false),
                    new Word("картофель", true, WordGender.Male, PartOfSpeech.Noun, false),
                    new Word("кашель", true, WordGender.Male, PartOfSpeech.Noun, true),
                    new Word("кисель", true, WordGender.Male, PartOfSpeech.Noun, false),
                    new Word("клубень", true, WordGender.Male, PartOfSpeech.Noun, true),
                    new Word("коготь", true, WordGender.Male, PartOfSpeech.Noun, true),
                    new Word("коктейль", true, WordGender.Male, PartOfSpeech.Noun, false),
                    new Word("контроль", true, WordGender.Male, PartOfSpeech.Noun, false),
                    new Word("конь", true, WordGender.Male, PartOfSpeech.Noun, false),
                    new Word("корабль", true, WordGender.Male, PartOfSpeech.Noun, false),
                    new Word("корень", true, WordGender.Male, PartOfSpeech.Noun, true),
                    new Word("король", true, WordGender.Male, PartOfSpeech.Noun, false),
                    new Word("костыль", true, WordGender.Male, PartOfSpeech.Noun, false),
                    new Word("кремль", true, WordGender.Male, PartOfSpeech.Noun, false),
                    new Word("крендель", true, WordGender.Male, PartOfSpeech.Noun, false),
                    new Word("куль", true, WordGender.Male, PartOfSpeech.Noun, false),
                    new Word("лагерь", true, WordGender.Male, PartOfSpeech.Noun, false),
                    new Word("лапоть", true, WordGender.Male, PartOfSpeech.Noun, true),
                    new Word("лебедь", true, WordGender.Male, PartOfSpeech.Noun, false),
                    new Word("ливень", true, WordGender.Male, PartOfSpeech.Noun, true),
                    new Word("лодырь", true, WordGender.Male, PartOfSpeech.Noun, false),
                    new Word("локоть", true, WordGender.Male, PartOfSpeech.Noun, true),
                    new Word("ломоть", true, WordGender.Male, PartOfSpeech.Noun, true),
                    new Word("лосось", true, WordGender.Male, PartOfSpeech.Noun, false),
                    new Word("лось", true, WordGender.Male, PartOfSpeech.Noun, false),
                    new Word("медведь", true, WordGender.Male, PartOfSpeech.Noun, false),
                    new Word("миндаль", true, WordGender.Male, PartOfSpeech.Noun, false),
                    new Word("модуль", true, WordGender.Male, PartOfSpeech.Noun, false),
                    new Word("нашатырь", true, WordGender.Male, PartOfSpeech.Noun, false),
                    new Word("недоросль", true, WordGender.Male, PartOfSpeech.Noun, false),
                    new Word("никель", true, WordGender.Male, PartOfSpeech.Noun, false),
                    new Word("ноготь", true, WordGender.Male, PartOfSpeech.Noun, true),
                    new Word("ноль", true, WordGender.Male, PartOfSpeech.Noun, false),
                    new Word("ноябрь", true, WordGender.Male, PartOfSpeech.Noun, false),
                    new Word("огонь", true, WordGender.Male, PartOfSpeech.Noun, true),
                    new Word("октябрь", true, WordGender.Male, PartOfSpeech.Noun, false),
                    new Word("окунь", true, WordGender.Male, PartOfSpeech.Noun, false),
                    new Word("олень", true, WordGender.Male, PartOfSpeech.Noun, false),
                    new Word("отель", true, WordGender.Male, PartOfSpeech.Noun, false),
                    new Word("панцирь", true, WordGender.Male, PartOfSpeech.Noun, false),
                    new Word("пароль", true, WordGender.Male, PartOfSpeech.Noun, false),
                    new Word("паствиль", true, WordGender.Male, PartOfSpeech.Noun, false),
                    new Word("патруль", true, WordGender.Male, PartOfSpeech.Noun, false),
                    new Word("пельмень", true, WordGender.Male, PartOfSpeech.Noun, false),
                    new Word("пень", true, WordGender.Male, PartOfSpeech.Noun, true),
                    new Word("перечень", true, WordGender.Male, PartOfSpeech.Noun, true),
                    new Word("печень", true, WordGender.Male, PartOfSpeech.Noun, false),
                    new Word("пластырь", true, WordGender.Male, PartOfSpeech.Noun, false),
                    new Word("плетень", true, WordGender.Male, PartOfSpeech.Noun, true),
                    new Word("полдень", true, WordGender.Male, PartOfSpeech.Noun, true),
                    new Word("портфель", true, WordGender.Male, PartOfSpeech.Noun, false),
                    new Word("поршень", true, WordGender.Male, PartOfSpeech.Noun, true),
                    new Word("профиль", true, WordGender.Male, PartOfSpeech.Noun, false),
                    new Word("пудель", true, WordGender.Male, PartOfSpeech.Noun, false),
                    new Word("путь", true, WordGender.Male, PartOfSpeech.Noun, false),
                    new Word("рашпиль", true, WordGender.Male, PartOfSpeech.Noun, false),
                    new Word("ремень", true, WordGender.Male, PartOfSpeech.Noun, true),
                    new Word("рояль", true, WordGender.Male, PartOfSpeech.Noun, false),
                    new Word("рубль", true, WordGender.Male, PartOfSpeech.Noun, false),
                    new Word("руль", true, WordGender.Male, PartOfSpeech.Noun, false),
                    new Word("сентябрь", true, WordGender.Male, PartOfSpeech.Noun, false),
                    new Word("скальпель", true, WordGender.Male, PartOfSpeech.Noun, false),
                    new Word("соболь", true, WordGender.Male, PartOfSpeech.Noun, false),
                    new Word("спектакль", true, WordGender.Male, PartOfSpeech.Noun, false),
                    new Word("ставень", true, WordGender.Male, PartOfSpeech.Noun, true),
                    new Word("стебель", true, WordGender.Male, PartOfSpeech.Noun, true),
                    new Word("стержень", true, WordGender.Male, PartOfSpeech.Noun, true),
                    new Word("стиль", true, WordGender.Male, PartOfSpeech.Noun, false),
                    new Word("студень", true, WordGender.Male, PartOfSpeech.Noun, true),
                    new Word("табель", true, WordGender.Male, PartOfSpeech.Noun, false),
                    new Word("текстиль", true, WordGender.Male, PartOfSpeech.Noun, false),
                    new Word("толь", true, WordGender.Male, PartOfSpeech.Noun, false),
                    new Word("тополь", true, WordGender.Male, PartOfSpeech.Noun, false),
                    new Word("трутень", true, WordGender.Male, PartOfSpeech.Noun, true),
                    new Word("туннель", true, WordGender.Male, PartOfSpeech.Noun, false),
                    new Word("тюлень", true, WordGender.Male, PartOfSpeech.Noun, false),
                    new Word("тюль", true, WordGender.Male, PartOfSpeech.Noun, false),
                    new Word("уголь", true, WordGender.Male, PartOfSpeech.Noun, true),
                    new Word("уровень", true, WordGender.Male, PartOfSpeech.Noun, true),
                    new Word("февраль", true, WordGender.Male, PartOfSpeech.Noun, false),
                    new Word("фитиль", true, WordGender.Male, PartOfSpeech.Noun, false),
                    new Word("флигель", true, WordGender.Male, PartOfSpeech.Noun, false),
                    new Word("хмель", true, WordGender.Male, PartOfSpeech.Noun, false),
                    new Word("хрусталь", true, WordGender.Male, PartOfSpeech.Noun, false),
                    new Word("шампунь", true, WordGender.Male, PartOfSpeech.Noun, false),
                    new Word("шмель", true, WordGender.Male, PartOfSpeech.Noun, false),
                    new Word("штиль", true, WordGender.Male, PartOfSpeech.Noun, false),
                    new Word("штемпель", true, WordGender.Male, PartOfSpeech.Noun, false),
                    new Word("штепсель", true, WordGender.Male, PartOfSpeech.Noun, false),
                    new Word("щавель", true, WordGender.Male, PartOfSpeech.Noun, false),
                    new Word("щебень", true, WordGender.Male, PartOfSpeech.Noun, true),
                    new Word("эндшпиль", true, WordGender.Male, PartOfSpeech.Noun, false),
                    new Word("юань", true, WordGender.Male, PartOfSpeech.Noun, false),
                    new Word("якорь", true, WordGender.Male, PartOfSpeech.Noun, false),
                    new Word("ячмень", true, WordGender.Male, PartOfSpeech.Noun, false),
                    //сущ. с беглыми гласными, кроме оканчивающихся на -ец, -ок, -ек
                    new Word("лев", true, WordGender.Male, PartOfSpeech.Noun, true),
                    new Word("павел", true, WordGender.Male, PartOfSpeech.Noun, true),
                    new Word("ров", true, WordGender.Male, PartOfSpeech.Noun, true),
                    new Word("сон", true, WordGender.Male, PartOfSpeech.Noun, true),
                    new Word("угол", true, WordGender.Male, PartOfSpeech.Noun, true),
                    new Word("лоб", true, WordGender.Male, PartOfSpeech.Noun, true),
                    new Word("рот", true, WordGender.Male, PartOfSpeech.Noun, true),
                    new Word("лёд", true, WordGender.Male, PartOfSpeech.Noun, true),
                    new Word("лён", true, WordGender.Male, PartOfSpeech.Noun, true),
                    new Word("сон", true, WordGender.Male, PartOfSpeech.Noun, true),
                    new Word("чехол", true, WordGender.Male, PartOfSpeech.Noun, true)
                };
        }
        public override string ToString()
        {
            return $"{_text}({_PartofSpeech}, {_WordGender}, {_IsDeclinated}, {_HasRunawayVowel})";
        }
    }

    /// <summary>
    /// Класс с общей информацией по серверу, базе данных и лаборатории
    /// </summary>
    public static class CommonInfo
    {
        /// <summary>
        /// Строка подключения к базе данных
        /// </summary>
        public static readonly SqlConnection connection;

        /// <summary>
        /// Общая информация о ПЛСЭ
        /// </summary>
        public static readonly Laboratory PLSE;
        private static ObservableCollection<Speciality> _specialities = new ObservableCollection<Speciality>();
        private static ObservableCollection<Organization> _organizations = new ObservableCollection<Organization>();
        private static ObservableCollection<Employee> _employees = new ObservableCollection<Employee>();
        private static ObservableCollection<Expert> _experts = new ObservableCollection<Expert>();
        private static ObservableCollection<Customer> _customers = new ObservableCollection<Customer>();
        private static ObservableCollection<Settlement> _settlements = new ObservableCollection<Settlement>();
        private static ObservableCollection<Departament> departaments = new ObservableCollection<Departament>();
        private static string[] _status = { "действует", "не действует" };
        static string[] _payers = { "истца", "ответчика", "истца и ответчика", "иное" };
        static IReadOnlyList<string> _genders;
        static IReadOnlyList<string> _streettypes;
        static IReadOnlyList<string> _inneroffices;
        static IReadOnlyList<string> _settlementprefixs;
        static IReadOnlyList<string> _settlementsigns;
        static IReadOnlyList<string> _employeestatus;
        static IReadOnlyDictionary<string, string> _casetypes;
        static IReadOnlyList<string> _expertisetypes;
        static IReadOnlyList<string> _resolutiontypes;
        static IReadOnlyList<string> _expertiresult;
        static IReadOnlyList<string> _resolutionstatus;
        static IReadOnlyList<string> _ranks;
        static IReadOnlyList<string> _outeroffices;
        static IReadOnlyList<string> _typerequest;

        public static bool IsInitializated { get; private set; }
        public static IReadOnlyList<string> OuterOffices
        {
            get => _outeroffices;
            set => _outeroffices = value;
        }
        public static IReadOnlyList<string> Ranks
        {
            get => _ranks;
            set => _ranks = value;
        }
        public static IReadOnlyList<string> ResolutionStatus
        {
            get => _resolutionstatus;
            set => _resolutionstatus = value;
        }
        public static IReadOnlyList<string> ExpertiseResult
        {
            get => _expertiresult;
            set => _expertiresult = value;
        }
        public static IReadOnlyList<string> ResolutionTypes
        {
            get => _resolutiontypes;
            set => _resolutiontypes = value;
        }
        public static IReadOnlyList<string> ExpertiseTypes => _expertisetypes;
        public static IReadOnlyDictionary<string, string> CaseTypes => _casetypes;
        public static IReadOnlyList<string> EmployeeStatus => _employeestatus;
        public static IReadOnlyList<string> SettlementSignificancies => _settlementsigns;
        public static IReadOnlyList<string> SettlementPrefixs => _settlementprefixs;
        public static IReadOnlyList<string> InnerOfficies => _inneroffices;
        public static IReadOnlyList<string> StreetTypes => _streettypes;
        public static IReadOnlyList<string> Genders => _genders;
        public static IReadOnlyList<string> Status => _status;
        public static IReadOnlyList<string> RequestTypes => _typerequest;

        public static ObservableCollection<Settlement> Settlements
        {
            get => _settlements;
            set => _settlements = value;
        }
        public static ObservableCollection<Speciality> Specialities
        {
            get => _specialities;
            set => _specialities = value;
        }
        public static ObservableCollection<Employee> Employees
        {
            get => _employees;
            set => _employees = value;
        }
        public static ObservableCollection<Expert> Experts
        {
            get => _experts;
            set => _experts = value;
        }
        public static ObservableCollection<Customer> Customers
        {
            get => _customers;
            set => _customers = value;
        }
        public static ObservableCollection<Organization> Organizations
        {
            get => _organizations;
            set => _organizations = value;
        }
        public static ObservableCollection<Departament> Departaments
        {
            get => departaments;
            set => departaments = value;
        }
        
        static CommonInfo()
        {
            SqlConnectionStringBuilder sb = new SqlConnectionStringBuilder()
            {
                DataSource = Environment.UserName != "Богатов" ? @".\SIRSERVER" : @".\SQLEXPRESS",
                IntegratedSecurity = true,
                InitialCatalog = "PLSE_New",
                ConnectTimeout = 5
            };
            connection = new SqlConnection(sb.ConnectionString);
            PLSE = new Laboratory()
            {
                Name = "федеральное бюджетное учреждение Пензенская лаборатория судебной экспертизы Министерства юстиции Российской Федерации",
                ShortName = "ФБУ Пензенская ЛСЭ Минюста России",
                PostCode = "440018",
                Telephone = "68-61-09",
                Fax = "68-33-55",
                Email = "penza@forlabpnz.ru",
                WebSite = "www.forlabpnz.ru",
                IsValid = true,
                Adress = new Adress()
                {
                    Streetprefix = "ул.",
                    Street = "Бекешская",
                    Housing = "41",
                    Settlement = new Settlement()
                    {
                        Federallocation = "Пензенская обл.",
                        Significance = "областной",
                        Title = "Пенза",
                        Telephonecode = "+7 8412",
                        Settlementtype = "г."
                    }
                }
            };
            try
            {
                LoadInitialInfo(connection);
                IsInitializated = true;
            }
            catch (Exception)
            {
                throw;
            }
        }
        private static void LoadInitialInfo(SqlConnection connection)
        {
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "InnResources.prCommonInfo";
            connection.Open();
            SqlDataReader rd = cmd.ExecuteReader();
            //Genders
            if (rd.HasRows)
            {
                List<string> genders = new List<string>();
                while (rd.Read())
                {
                    genders.Add(rd.GetString(0));
                }
                _genders = genders;
            }
            //StreetType
            if (rd.NextResult())
            {
                List<string> streettype = new List<string>();
                while (rd.Read())
                {
                    streettype.Add(rd.GetString(0));
                }
                _streettypes = streettype;
            }
            //InnerOffice
            if (rd.NextResult())
            {
                List<string> lInnerOffice = new List<string>();
                while (rd.Read())
                {
                    lInnerOffice.Add(rd.GetString(0));
                }
                _inneroffices = lInnerOffice;
            }
            //EmployeeStatus
            if (rd.NextResult())
            {
                List<string> lEmployeeStatus = new List<string>();
                while (rd.Read())
                {
                    lEmployeeStatus.Add(rd.GetString(0));
                }
                _employeestatus = lEmployeeStatus;
            }
            //Departaments
            if (rd.NextResult())
            {
                int colTitle = rd.GetOrdinal("Title");
                int colStatus = rd.GetOrdinal("StatusID");
                int colAcronym = rd.GetOrdinal("Acronym");
                int colID = rd.GetOrdinal("DepartamentID");
                int colCode = rd.GetOrdinal("DigitalCode");
                while (rd.Read())
                {
                    Departament newDep = new Departament(isvalid: rd.GetBoolean(colStatus),
                                                            title: rd.GetString(colTitle),
                                                            acronym: rd.GetString(colAcronym),
                                                            id: rd.GetByte(colID),
                                                            code: rd.GetString(colCode)
                                                            );
                    Departaments.Add(newDep);
                }
            }
            //Specialities
            if (rd.NextResult())
            {
                int colID = rd.GetOrdinal("SpecialityID");
                int colCode = rd.GetOrdinal("Code");
                int colCat1 = rd.GetOrdinal("Category1");
                int colCat2 = rd.GetOrdinal("Category2");
                int colCat3 = rd.GetOrdinal("Category3");
                int colSpecies = rd.GetOrdinal("Species");
                int colAcronym = rd.GetOrdinal("Acronym");
                int colStatus = rd.GetOrdinal("StatusID");
                int colUpdateDate = rd.GetOrdinal("UpdateDate");
                while (rd.Read())
                {
                    Speciality newspec = new Speciality(updatedate: rd.GetDateTime(colUpdateDate),
                                                        id: rd.GetInt16(colID),
                                                        code: rd.GetString(colCode),
                                                        species: rd[colSpecies] != DBNull.Value ? rd.GetString(colSpecies) : null,
                                                        status: rd.GetBoolean(colStatus),
                                                        acr: rd[colAcronym] != DBNull.Value ? rd.GetString(colAcronym) : null,
                                                        cat_1: rd[colCat1] != DBNull.Value ? new Byte?(rd.GetByte(colCat1)) : null,
                                                        cat_2: rd[colCat2] != DBNull.Value ? new Byte?(rd.GetByte(colCat2)) : null,
                                                        cat_3: rd[colCat3] != DBNull.Value ? new Byte?(rd.GetByte(colCat3)) : null,
                                                        vr: Version.Original
                                                        );
                    Specialities.Add(newspec);
                }
            }
            //SettlementPrefix
            if (rd.NextResult())
            {
                List<string> lSettlementPrefixes = new List<string>();
                while (rd.Read())
                {
                    lSettlementPrefixes.Add(rd.GetString(0));
                }
                _settlementprefixs = lSettlementPrefixes;
            }
            //SettlementSign
            if (rd.NextResult())
            {
                List<string> lSettlementSignificances = new List<string>();
                while (rd.Read())
                {
                    lSettlementSignificances.Add(rd.GetString(0));
                }
                _settlementsigns = lSettlementSignificances;
            }
            //Settlements
            if (rd.NextResult())
            {
                int colSettlementID = rd.GetOrdinal("SettlementID");
                int colTitle = rd.GetOrdinal("Title");
                int colSettlementType = rd.GetOrdinal("SettlementType");
                int colSignificance = rd.GetOrdinal("Significance");
                int colFederalLocationID = rd.GetOrdinal("FederalLocation");
                int colTerritorialLocationID = rd.GetOrdinal("TerritorialLocation");
                int colTelephoneCode = rd.GetOrdinal("TelephoneCode");
                int colPostCode = rd.GetOrdinal("PostCode");
                int colStatus = rd.GetOrdinal("StatusID");
                int colUpdateDate = rd.GetOrdinal("UpdateDate");
                while (rd.Read())
                {
                    Settlement setl = new Settlement(id: rd.GetInt32(colSettlementID),
                                                        title: rd.GetString(colTitle),
                                                        type: rd.GetString(colSettlementType),
                                                        significance: rd.GetString(colSignificance),
                                                        telephonecode: rd[colTelephoneCode] == DBNull.Value ? null : rd.GetString(colTelephoneCode),
                                                        postcode: rd[colPostCode] == DBNull.Value ? null : rd.GetString(colPostCode),
                                                        federallocation: rd[colFederalLocationID] == DBNull.Value ? null : rd.GetString(colFederalLocationID),
                                                        territoriallocation: rd[colTerritorialLocationID] == DBNull.Value ? null : rd.GetString(colTerritorialLocationID),
                                                        isvalid: rd.GetBoolean(colStatus),
                                                        vr: Version.Original,
                                                        updatedate: rd.GetDateTime(colUpdateDate)
                                                        );
                    _settlements.Add(setl);
                }
            }
            //Employees
            if (rd.NextResult())
            {
                int colEmployeeID = rd.GetOrdinal("EmployeeID");
                int colFirstName = rd.GetOrdinal("FirstName");
                int colMiddleName = rd.GetOrdinal("MiddleName");
                int colSecondName = rd.GetOrdinal("SecondName");
                int colDeclinated = rd.GetOrdinal("Declinated");
                int colWorkPhone = rd.GetOrdinal("WorkPhone");
                int colBirthDate = rd.GetOrdinal("BirthDate");
                int colHireDate = rd.GetOrdinal("HireDate");
                int colEducation1 = rd.GetOrdinal("Education_1");
                int colEducation2 = rd.GetOrdinal("Education_2");
                int colEducation3 = rd.GetOrdinal("Education_3");
                int colScienceDegree = rd.GetOrdinal("ScienceDegree");
                int colCondition = rd.GetOrdinal("EmployeeStatusID");
                int colFoto = rd.GetOrdinal("Foto");
                int colDepartament = rd.GetOrdinal("DepartamentID");
                int colInnerOffice = rd.GetOrdinal("InnerOfficeID");
                int colSettlementID = rd.GetOrdinal("SettlementID");
                int colStreetPrefix = rd.GetOrdinal("StreetPrefix");
                int colStreet = rd.GetOrdinal("Street");
                int colHousing = rd.GetOrdinal("Housing");
                int colFlat = rd.GetOrdinal("Flat");
                int colCorpus = rd.GetOrdinal("Corpus");
                int colGender = rd.GetOrdinal("Gender");
                int colMobilePhone = rd.GetOrdinal("MobilePhone");
                int colEmail = rd.GetOrdinal("Email");
                int colPassword = rd.GetOrdinal("UserPassword");
                int colUpdateDate = rd.GetOrdinal("UpdateDate");
                int colStructure = rd.GetOrdinal("Structure");
                int colProfile = rd.GetOrdinal("PermissionProfile");
                int colPrevID = rd.GetOrdinal("PreviousID");
                while (rd.Read())
                {
                    Adress adr = new Adress(settlement: rd[colSettlementID] == DBNull.Value ? null : Settlements.Single(x => x.SettlementID == rd.GetInt32(colSettlementID)),
                                                streetprefix: rd[colStreetPrefix] == DBNull.Value ? null : rd.GetString(colStreetPrefix),
                                                street: rd[colStreet] == DBNull.Value ? null : rd.GetString(colStreet),
                                                housing: rd[colHousing] == DBNull.Value ? null : rd.GetString(colHousing),
                                                flat: rd[colFlat] == DBNull.Value ? null : rd.GetString(colFlat),
                                                corpus: rd[colCorpus] == DBNull.Value ? null : rd.GetString(colCorpus),
                                                structure: rd[colStructure] == DBNull.Value ? null : rd.GetString(colStructure)
                                                );
                    Employee emp = new Employee(id: rd.GetInt32(colEmployeeID),
                                                previd: rd[colPrevID] == DBNull.Value ? null : new int?(rd.GetInt32(colPrevID)),
                                                firstname: rd.GetString(colFirstName),
                                                middlename: rd.GetString(colMiddleName),
                                                secondname: rd.GetString(colSecondName),
                                                declinated: rd.GetBoolean(colDeclinated),
                                                workphone: rd[colWorkPhone] == DBNull.Value ? null : rd.GetString(colWorkPhone),
                                                birthdate: rd[colBirthDate] == DBNull.Value ? null : new DateTime?(rd.GetDateTime(colBirthDate)),
                                                hiredate: rd[colHireDate] == DBNull.Value ? null : new DateTime?(rd.GetDateTime(colHireDate)),
                                                education1: rd[colEducation1] == DBNull.Value ? null : rd.GetString(colEducation1),
                                                education2: rd[colEducation2] == DBNull.Value ? null : rd.GetString(colEducation2),
                                                education3: rd[colEducation3] == DBNull.Value ? null : rd.GetString(colEducation3),
                                                sciencedegree: rd[colScienceDegree] == DBNull.Value ? null : rd.GetString(colScienceDegree),
                                                condition: rd.GetString(colCondition),
                                                foto: rd[colFoto] == DBNull.Value ? null : (byte[])rd[colFoto],
                                                departament: Departaments.Single(x => x.DepartamentID == rd.GetByte(colDepartament)),
                                                inneroffice: rd.GetString(colInnerOffice),
                                                adress: adr,
                                                gender: rd.GetString(colGender),
                                                mobilephone: rd[colMobilePhone] == DBNull.Value ? null : rd.GetString(colMobilePhone),
                                                email: rd[colEmail] == DBNull.Value ? null : rd.GetString(colEmail),
                                                profile: (PermissionProfile)rd.GetByte(colProfile),
                                                password: rd[colPassword] == DBNull.Value ? null : rd.GetString(colPassword),
                                                updatedate: rd.GetDateTime(colUpdateDate),
                                                vr: Version.Original);
                    _employees.Add(emp);
                }
            }
            //Experts
            if (rd.NextResult())
            {
                int colExpertID = rd.GetOrdinal("ExpertID");
                int colEmployeeID = rd.GetOrdinal("EmployeeID");
                int colSpecialityID = rd.GetOrdinal("SpecialityID");
                int colExperience = rd.GetOrdinal("Experience");
                int colLastAtt = rd.GetOrdinal("LastAttestation");
                int colClosed = rd.GetOrdinal("Closed");
                int colUpdateDate = rd.GetOrdinal("UpdateDate");
                while (rd.Read())
                {
                    DateTime? lastatt = null;
                    if (!rd.IsDBNull(colLastAtt)) lastatt = rd.GetDateTime(colLastAtt);
                    Expert expert = new Expert(id: rd.GetInt32(colExpertID),
                                                employee: Employees.Single(x => x.EmployeeID == rd.GetInt32(colEmployeeID)),
                                                speciality: Specialities.Single(x => x.SpecialityID == rd.GetInt16(colSpecialityID)),
                                                receiptdate: rd.GetDateTime(colExperience),
                                                lastattestationdate: lastatt,
                                                closed: rd.GetBoolean(colClosed),
                                                updatedate: rd.GetDateTime(colUpdateDate),
                                                vr: Version.Original
                                                );
                    _experts.Add(expert);
                }
            }
            //TypeCase
            if (rd.NextResult())
            {
                Dictionary<string, string> lTypeCase = new Dictionary<string, string>();
                while (rd.Read())
                {
                    lTypeCase.Add(rd.GetString(0), rd.GetString(1));
                }
                _casetypes = lTypeCase;
            }
            //ExpertiseType
            if (rd.NextResult())
            {
                List<string> lTypeExpertise = new List<string>();
                while (rd.Read())
                {
                    lTypeExpertise.Add(rd.GetString(0));
                }
                _expertisetypes = lTypeExpertise;
            }
            //ExpertiseResult
            if (rd.NextResult())
            {
                List<string> lExpertiseResult = new List<string>();
                while (rd.Read())
                {
                    lExpertiseResult.Add(rd.GetString(0));
                }
                _expertiresult = lExpertiseResult;
            }
            //ResolutionType
            if (rd.NextResult())
            {
                List<string> ltypeResolution = new List<string>();
                while (rd.Read())
                {
                    ltypeResolution.Add(rd.GetString(0));
                }
                _resolutiontypes = ltypeResolution;
            }
            //ResolutionStatus
            if (rd.NextResult())
            {
                List<string> lResolutionStatus = new List<string>();
                while (rd.Read())
                {
                    lResolutionStatus.Add(rd.GetString(0));
                }
                _resolutionstatus = lResolutionStatus;
            }
            //Organization
            if (rd.NextResult())
            {
                int colID = rd.GetOrdinal("OrganizationID");
                int colName = rd.GetOrdinal("Name");
                int colShortName = rd.GetOrdinal("ShortName");
                int colPost = rd.GetOrdinal("Post");
                int colSettlement = rd.GetOrdinal("SettlementID");
                int colStreetPrefix = rd.GetOrdinal("StreetPrefix");
                int colStreet = rd.GetOrdinal("Street");
                int colHousing = rd.GetOrdinal("Housing");
                int colOffice = rd.GetOrdinal("Office");
                int colCorpus = rd.GetOrdinal("Corpus");
                int colStructure = rd.GetOrdinal("Structure");
                int colTel = rd.GetOrdinal("Telephone");
                int colTel2 = rd.GetOrdinal("Telephone_2");
                int colFax = rd.GetOrdinal("Fax");
                int colEmail = rd.GetOrdinal("Email");
                int colWebSite = rd.GetOrdinal("WebSite");
                int colStatus = rd.GetOrdinal("StatusID");
                int colUpdateDate = rd.GetOrdinal("UpdateDate");
                while (rd.Read())
                {
                    Adress adr = new Adress(settlement: rd[colSettlement] == DBNull.Value ? null : Settlements.Single(x => x.SettlementID == rd.GetInt32(colSettlement)),
                                                    streetprefix: rd[colStreetPrefix] == DBNull.Value ? null : rd.GetString(colStreetPrefix),
                                                    street: rd[colStreet] == DBNull.Value ? null : rd.GetString(colStreet),
                                                    housing: rd[colHousing] == DBNull.Value ? null : rd.GetString(colHousing),
                                                    flat: rd[colOffice] == DBNull.Value ? null : rd.GetString(colOffice),
                                                    corpus: rd[colCorpus] == DBNull.Value ? null : rd.GetString(colCorpus),
                                                    structure: rd[colStructure] == DBNull.Value ? null : rd.GetString(colStructure)
                                                    );
                    Organization org = new Organization(id: rd.GetInt32(colID),
                                                        name: rd.GetString(colName),
                                                        shortname: rd[colShortName] == DBNull.Value ? null : rd.GetString(colShortName),
                                                        postcode: rd.GetString(colPost),
                                                        adress: adr,
                                                        telephone: rd[colTel] == DBNull.Value ? null : rd.GetString(colTel),
                                                        telephone2: rd[colTel2] == DBNull.Value ? null : rd.GetString(colTel2),
                                                        fax: rd[colFax] == DBNull.Value ? null : rd.GetString(colFax),
                                                        email: rd[colEmail] == DBNull.Value ? null : rd.GetString(colEmail),
                                                        website: rd[colWebSite] == DBNull.Value ? null : rd.GetString(colWebSite),
                                                        status: rd.GetBoolean(colStatus),
                                                        vr: Version.Original,
                                                        updatedate: rd.GetDateTime(colUpdateDate)
                                                        );
                    _organizations.Add(org);
                }
            }
            //Customers
            if (rd.NextResult())
            {
                int colId = rd.GetOrdinal("CustomerID");
                int colFirstName = rd.GetOrdinal("FirstName");
                int colSecondName = rd.GetOrdinal("SecondName");
                int colMiddleName = rd.GetOrdinal("MiddleName");
                int colDeclinated = rd.GetOrdinal("Declinated");
                int colGender = rd.GetOrdinal("Gender");
                int colWorkPhone = rd.GetOrdinal("WorkPhone");
                int colMobilePhone = rd.GetOrdinal("MobilePhone");
                int colOrganization = rd.GetOrdinal("OrganizationID");
                int colOffice = rd.GetOrdinal("Office");
                int colRank = rd.GetOrdinal("RankID");
                int colDep = rd.GetOrdinal("Departament");
                int colEmail = rd.GetOrdinal("Email");
                int colStatus = rd.GetOrdinal("StatusID");
                int colUpdateDate = rd.GetOrdinal("UpdateDate");
                int colPrevId = rd.GetOrdinal("PreviousID");
                while (rd.Read())
                {
                    Customer cus = new Customer(id: rd.GetInt32(colId),
                                            previd: rd[colPrevId] == DBNull.Value ? null : new int?(rd.GetInt32(colPrevId)),
                                            firstname: rd.GetString(colFirstName),
                                            secondname: rd.GetString(colSecondName),
                                            middlename: rd.GetString(colMiddleName),
                                            declinated: rd.GetBoolean(colDeclinated),
                                            gender: rd.GetString(colGender),
                                            workphone: rd[colWorkPhone] == DBNull.Value ? null : rd.GetString(colWorkPhone),
                                            mobilephone: rd[colMobilePhone] == DBNull.Value ? null : rd.GetString(colMobilePhone),
                                            organization: rd[colOrganization] == DBNull.Value ? null : Organizations.First(n => n.OrganizationID == rd.GetInt32(colOrganization)),
                                            office: rd[colOffice] == DBNull.Value ? null : rd.GetString(colOffice),
                                            rank: rd[colRank] == DBNull.Value ? null : rd.GetString(colRank),
                                            departament: rd[colDep] == DBNull.Value ? null : rd.GetString(colDep),
                                            email: rd[colEmail] == DBNull.Value ? null : rd.GetString(colEmail),
                                            status: rd.GetBoolean(colStatus),
                                            vr: Version.Original,
                                            updatedate: rd.GetDateTime(colUpdateDate)
                                            );
                    _customers.Add(cus);
                }
            }
            //Ranks
            if (rd.NextResult())
            {
                var ranks = new List<string>(80);
                while (rd.Read())
                {
                    ranks.Add(rd.GetString(0));
                }
                _ranks = ranks;
            }
            //TypeRequest
            if (rd.NextResult())
            {
                var rtype = new List<string>(5);
                while (rd.Read())
                {
                    rtype.Add(rd.GetString(0));
                }
                _typerequest = rtype;
            }
            rd.Close();
            connection.Close();
        }
        
        public static List<Resolution> LoadResolution(string query)
        {
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = query;
            List<Resolution> resolutions = new List<Resolution>();
            try
            {
                connection.Open();
                var rd = cmd.ExecuteReader();
                if (rd.HasRows)
                {
                    int colExpertiseID = rd.GetOrdinal("ExpertiseID");
                    int colNumber = rd.GetOrdinal("Number");
                    int colExpertiseResult = rd.GetOrdinal("ExpertiseResult");
                    int colStartDate = rd.GetOrdinal("StartDate");
                    int colExecutionDate = rd.GetOrdinal("ExecutionDate");
                    int colExpertiseType = rd.GetOrdinal("TypeExpertise");
                    int colPreviousExpertise = rd.GetOrdinal("PreviousExpertise");
                    int colSpendHours = rd.GetOrdinal("SpendHours");
                    int colTimelimit = rd.GetOrdinal("Timelimit");
                    int colExpertID = rd.GetOrdinal("ExpertID");
                    int colBillDate = rd.GetOrdinal("BillDate");
                    int colBillID = rd.GetOrdinal("BillID");
                    int colBillNumber = rd.GetOrdinal("BillNumber");
                    int colHourprice = rd.GetOrdinal("HourPrice");
                    int colNHours = rd.GetOrdinal("NHours");
                    int colPaid = rd.GetOrdinal("Paid");
                    int colPaidDate = rd.GetOrdinal("PaidDate");
                    int colPayer = rd.GetOrdinal("PayerID");
                    int colDelayDate = rd.GetOrdinal("DelayDate");
                    int colReason = rd.GetOrdinal("Reason");
                    int colReportDate = rd.GetOrdinal("ReportDate");
                    int colReportID = rd.GetOrdinal("ReportID");
                    int colRequestComment = rd.GetOrdinal("RequestComment");
                    int colRequestDate = rd.GetOrdinal("DateRequest");
                    int colRequestID = rd.GetOrdinal("RequestID");
                    int colRequestType = rd.GetOrdinal("TypeRequest");
                    int colCustomerID = rd.GetOrdinal("CustomerID");
                    int colPrescribeType = rd.GetOrdinal("PrescribeType");
                    int colObjects = rd.GetOrdinal("ProvidedObjects");
                    int colQuestions = rd.GetOrdinal("Questions");
                    int colRegDate = rd.GetOrdinal("RegDate");
                    int colResolDate = rd.GetOrdinal("ResolDate");
                    int colResolutionID = rd.GetOrdinal("ResolutionID");
                    int colResolutionType = rd.GetOrdinal("TyperesolID");
                    int colResolutionStatus = rd.GetOrdinal("ResolutionStatusID");
                    int colAnnotate = rd.GetOrdinal("Annotate");
                    int colCaseComment = rd.GetOrdinal("Comment");
                    int colDispatchDate = rd.GetOrdinal("DispatchDate");
                    int colNumberCase = rd.GetOrdinal("NumberCase");
                    int colPlaintiff = rd.GetOrdinal("Plaintiff");
                    int colRespondent = rd.GetOrdinal("Respondent");
                    int colCaseType = rd.GetOrdinal("TypeCase");
                    Resolution _resolution = null;
                    Expertise _expertise = null;
                    while (rd.Read())
                    {
                        if (!resolutions.Any(n => n.ResolutionID == rd.GetInt32(colResolutionID)))
                        {
                            _resolution = new Resolution(
                                                id: rd.GetInt32(colResolutionID),
                                                registrationdate: rd.GetDateTime(colRegDate),
                                                resolutiondate: rd[colResolDate] == DBNull.Value ? null : new DateTime?(rd.GetDateTime(colResolDate)),
                                                resolutiontype: rd.GetString(colResolutionType),
                                                customer: Customers.Single(n => n.CustomerID == rd.GetInt32(colCustomerID)),
                                                obj: rd[colObjects] == DBNull.Value ? null : (ObjectsList)rd[colObjects],
                                                quest: (QuestionsList)rd[colQuestions],
                                                status: rd.GetString(colResolutionStatus),
                                                prescribe: rd[colPrescribeType] == DBNull.Value ? null : rd.GetString(colPrescribeType),
                                                vr: Version.Original,
                                                updatedate: DateTime.Now
                                                );
                            if (rd[colAnnotate] != DBNull.Value) _resolution.Case.Annotate = rd.GetString(colAnnotate);
                            if (rd[colDispatchDate] != DBNull.Value) _resolution.Case.DispatchDate = new DateTime?(rd.GetDateTime(colDispatchDate));
                            if (rd[colCaseComment] != DBNull.Value) _resolution.Case.Comment = rd.GetString(colCaseComment);
                            if (rd[colNumberCase] != DBNull.Value) _resolution.Case.Number = rd.GetString(colNumberCase);
                            if (rd[colPlaintiff] != DBNull.Value) _resolution.Case.Plaintiff = rd.GetString(colPlaintiff);
                            if (rd[colRespondent] != DBNull.Value) _resolution.Case.Respondent = rd.GetString(colRespondent);
                            if (rd[colCaseType] != DBNull.Value) _resolution.Case.TypeCase = CaseTypes[rd.GetString(colCaseType)];
                            resolutions.Add(_resolution);
                        }
                        if (!_resolution.Expertisies.Any(n => n.ExpertiseID == rd.GetInt32(colExpertiseID)))
                        {
                            _expertise = new Expertise(id: rd.GetInt32(colExpertiseID),
                                                        number: rd.GetString(colNumber),
                                                        expert: Experts.Single(n => n.ExpertID == rd.GetInt32(colExpertID)),
                                                        status: rd[colExpertiseResult] == DBNull.Value ? null : rd.GetString(colExpertiseResult),
                                                        start: rd.GetDateTime(colStartDate),
                                                        end: rd[colExecutionDate] == DBNull.Value ? null : new DateTime?(rd.GetDateTime(colExecutionDate)),
                                                        timelimit: rd.GetByte(colTimelimit),                                                       
                                                        type: rd.GetString(colExpertiseType),
                                                        previous: rd[colPreviousExpertise] == DBNull.Value ? null : new Int32?(rd.GetInt32(colPreviousExpertise)),
                                                        spendhours: rd[colSpendHours] == DBNull.Value ? null : new short?(rd.GetInt16(colSpendHours)),
                                                        vr: Version.Original
                                                        );
                            if (_resolution.ResolutionID == rd.GetInt32(colResolutionID))
                            {
                                _resolution.Expertisies.Add(_expertise);
                            }
                            else resolutions.First(n => n.ResolutionID == rd.GetInt32(colResolutionID)).Expertisies.Add(_expertise);
                        }
                        if (rd[colRequestID] != DBNull.Value && !_expertise.Requests.Any(n => n.RequestID == rd.GetInt32(colRequestID)))
                        {
                            var _request = new Request(id: rd.GetInt32(colRequestID),
                                                        requestdate: rd.GetDateTime(colRequestDate),
                                                        type: rd.GetString(colRequestType),
                                                        comment: rd[colRequestComment] == DBNull.Value ? null : rd.GetString(colRequestComment),
                                                        vr: Version.Original);
                            if (_expertise.ExpertiseID == rd.GetInt32(colExpertiseID))
                            {
                                _expertise.Requests.Add(_request);
                            }
                            else resolutions.First(n => n.ResolutionID == rd.GetInt32(colResolutionID)).Expertisies.First(n => n.ExpertiseID == rd.GetInt32(colExpertiseID))
                                                                                                    .Requests.Add(_request);
                        }
                        if (rd[colReportID] != DBNull.Value && !_expertise.Reports.Any(n => n.ReportID == rd.GetInt32(colReportID)))
                        {
                            var _report = new Report(id: rd.GetInt32(colReportID),
                                                        repdate: rd.GetDateTime(colReportDate),
                                                        delay: rd.GetDateTime(colDelayDate),
                                                        reason: rd[colReason] == DBNull.Value ? null : rd.GetString(colReason),
                                                        vr: Version.Original);
                            if (_expertise.ExpertiseID == rd.GetInt32(colExpertiseID))
                            {
                                _expertise.Reports.Add(_report);
                            }
                            else resolutions.First(n => n.ResolutionID == rd.GetInt32(colResolutionID)).Expertisies.First(n => n.ExpertiseID == rd.GetInt32(colExpertiseID))
                                                                                                    .Reports.Add(_report);
                        }
                        if (rd[colBillID] != DBNull.Value && !_expertise.Bills.Any(n => n.BillID == rd.GetInt32(colBillID)))
                        {
                            var _bill = new Bill(id: rd.GetInt32(colBillID),
                                                number: rd.GetString(colBillNumber),
                                                billdate: rd.GetDateTime(colBillDate),
                                                paiddate: rd[colPaidDate] == DBNull.Value ? null : new DateTime?(rd.GetDateTime(colPaidDate)),
                                                payer: rd.GetString(colPayer),
                                                hours: rd.GetByte(colNHours),
                                                hourprice: rd.GetDecimal(colHourprice),
                                                paid: rd.GetDecimal(colPaid),
                                                vr: Version.Original);
                            if (_expertise.ExpertiseID == rd.GetInt32(colExpertiseID))
                            {
                                _expertise.Bills.Add(_bill);
                            }
                            else resolutions.First(n => n.ResolutionID == rd.GetInt32(colResolutionID)).Expertisies.First(n => n.ExpertiseID == rd.GetInt32(colExpertiseID))
                                                                                                    .Bills.Add(_bill);
                        }
                    }
                    rd.Close();
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                connection.Close();
            }
            return resolutions;
        }
        //public static List<Resolution> LoadResolution(int empID)
        //{
        //    SqlCommand cmd = connection.CreateCommand();
        //    cmd.CommandType = CommandType.Text;
        //    cmd.CommandText = "Select * from Activity.fResolution3(@Empl);";
        //    cmd.Parameters.Add("@Empl", SqlDbType.Int).Value = empID;
        //    List<Resolution> resolutions = new List<Resolution>();
        //    try
        //    {
        //        connection.Open();
        //        var rd = cmd.ExecuteReader();
        //        if (rd.HasRows)
        //        {
        //            int colExpertiseID = rd.GetOrdinal("ExpertiseID");
        //            int colNumber = rd.GetOrdinal("Number");
        //            int colExpertiseStatus = rd.GetOrdinal("ExpertiseStatusID");
        //            int colStartDate = rd.GetOrdinal("StartDate");
        //            int colExecutionDate = rd.GetOrdinal("ExecutionDate");
        //            int colExpertiseType = rd.GetOrdinal("TypeExpertise");
        //            int colPreviousExpertise = rd.GetOrdinal("PreviousExpertise");
        //            int colSpendHours = rd.GetOrdinal("SpendHours");
        //            int colTimelimit = rd.GetOrdinal("Timelimit");
        //            int colExpertID = rd.GetOrdinal("ExpertID");
        //            int colBillDate = rd.GetOrdinal("BillDate");
        //            int colBillID = rd.GetOrdinal("BillID");
        //            int colBillNumber = rd.GetOrdinal("BillNumber");
        //            int colHourprice = rd.GetOrdinal("HourPrice");
        //            int colNHours = rd.GetOrdinal("NHours");
        //            int colPaid = rd.GetOrdinal("Paid");
        //            int colPaidDate = rd.GetOrdinal("PaidDate");
        //            int colPayer = rd.GetOrdinal("PayerID");
        //            int colDelayDate = rd.GetOrdinal("DelayDate");
        //            int colReason = rd.GetOrdinal("Reason");
        //            int colReportDate = rd.GetOrdinal("ReportDate");
        //            int colReportID = rd.GetOrdinal("ReportID");
        //            int colRequestComment = rd.GetOrdinal("RequestComment");
        //            int colRequestDate = rd.GetOrdinal("DateRequest");
        //            int colRequestID = rd.GetOrdinal("RequestID");
        //            int colRequestType = rd.GetOrdinal("TypeRequest");
        //            int colCustomerID = rd.GetOrdinal("CustomerID");
        //            int colPrescribeType = rd.GetOrdinal("PrescribeType");
        //            int colObjects = rd.GetOrdinal("ProvidedObjects");
        //            int colQuestions = rd.GetOrdinal("Questions");
        //            int colRegDate = rd.GetOrdinal("RegDate");
        //            int colResolDate = rd.GetOrdinal("ResolDate");
        //            int colResolutionID = rd.GetOrdinal("ResolutionID");
        //            int colResolutionType = rd.GetOrdinal("TyperesolID");
        //            int colResolutionStatus = rd.GetOrdinal("ResolutionStatusID");
        //            int colAnnotate = rd.GetOrdinal("Annotate");
        //            int colCaseComment = rd.GetOrdinal("Comment");
        //            int colDispatchDate = rd.GetOrdinal("DispatchDate");
        //            int colNumberCase = rd.GetOrdinal("NumberCase");
        //            int colPlaintiff = rd.GetOrdinal("Plaintiff");
        //            int colRespondent = rd.GetOrdinal("Respondent");
        //            int colCaseType = rd.GetOrdinal("TypeCase");
        //            Resolution _resolution;
        //            Expertise _expertise;
        //            while (rd.Read())
        //            {
        //                if (!resolutions.Any(n => n.ResolutionID == rd.GetInt32(colResolutionID)))
        //                {
        //                    _resolution = new Resolution(
        //                                        id: rd.GetInt32(colResolutionID),
        //                                        registrationdate: rd.GetDateTime(colRegDate),
        //                                        resolutiondate: rd[colResolDate] == DBNull.Value ? null : new DateTime?(rd.GetDateTime(colResolDate)),
        //                                        resolutiontype: rd.GetString(colResolutionType),
        //                                        customer: Customers.Single(n => n.CustomerID == rd.GetInt32(colCustomerID)),
        //                                        obj: rd[colObjects] == DBNull.Value ? null : (ObjectsList)rd[colObjects],
        //                                        quest: (QuestionsList)rd[colQuestions],
        //                                        status: rd.GetString(colResolutionStatus),
        //                                        prescribe: rd[colPrescribeType] == DBNull.Value ? null : rd.GetString(colPrescribeType),
        //                                        vr: Version.Original,
        //                                        updatedate: DateTime.Now
        //                                        );
        //                    if (rd[colAnnotate] != DBNull.Value) _resolution.Case.Annotate = rd.GetString(colAnnotate);
        //                    if (rd[colDispatchDate] != DBNull.Value) _resolution.Case.DispatchDate = new DateTime?(rd.GetDateTime(colDispatchDate));
        //                    if (rd[colCaseComment] != DBNull.Value) _resolution.Case.Comment = rd.GetString(colCaseComment);
        //                    if (rd[colNumberCase] != DBNull.Value) _resolution.Case.Number = rd.GetString(colNumberCase);
        //                    if (rd[colPlaintiff] != DBNull.Value) _resolution.Case.Plaintiff = rd.GetString(colPlaintiff);
        //                    if (rd[colRespondent] != DBNull.Value) _resolution.Case.Respondent = rd.GetString(colRespondent);
        //                    if (rd[colCaseType] != DBNull.Value) _resolution.Case.TypeCase = CaseTypes.Single(n => n.Value == rd.GetString(colCaseType));
        //                    resolutions.Add(_resolution);
        //                }
        //                else _resolution = resolutions.Single(n => n.ResolutionID == rd.GetInt32(colResolutionID));
        //                if (!_resolution.Expertisies.Any(n => n.ExpertiseID == rd.GetInt32(colExpertiseID)))
        //                {
        //                    _expertise = new Expertise(id: rd.GetInt32(colExpertiseID),
        //                                                number: rd.GetString(colNumber),
        //                                                expert: Experts.Single(n => n.ExpertID == rd.GetInt32(colExpertID)),
        //                                                status: rd.GetString(colExpertiseStatus),
        //                                                start: rd.GetDateTime(colStartDate),
        //                                                end: rd[colExecutionDate] == DBNull.Value ? null : new DateTime?(rd.GetDateTime(colExecutionDate)),
        //                                                timelimit: rd.GetByte(colTimelimit),
        //                                                resolid: _resolution.ResolutionID,
        //                                                type: rd.GetString(colExpertiseType),
        //                                                previous: rd[colPreviousExpertise] == DBNull.Value ? null : new Int32?(rd.GetInt32(colPreviousExpertise)),
        //                                                spendhours: rd[colSpendHours] == DBNull.Value ? null : new short?(rd.GetInt16(colSpendHours)),
        //                                                vr: Version.Original
        //                                                );
        //                    _resolution.Expertisies.Add(_expertise);
        //                }
        //                else _expertise = _resolution.Expertisies.Single(n => n.ExpertiseID == rd.GetInt32(colExpertiseID));
        //                if (rd[colRequestID] != DBNull.Value && !_expertise.Requests.Any(n => n.RequestID == rd.GetInt32(colRequestID)))
        //                {
        //                    var _request = new Request(id: rd.GetInt32(colRequestID),
        //                                                expid: _expertise.ExpertiseID,
        //                                                requestdate: rd.GetDateTime(colRequestDate),
        //                                                type: rd.GetString(colRequestType),
        //                                                comment: rd[colRequestComment] == DBNull.Value ? null : rd.GetString(colRequestComment),
        //                                                vr: Version.Original);
        //                    _expertise.Requests.Add(_request);
        //                }
        //                if (rd[colReportID] != DBNull.Value && !_expertise.Reports.Any(n => n.ReportID == rd.GetInt32(colReportID)))
        //                {
        //                    var _report = new Report(id: rd.GetInt32(colReportID),
        //                                                expid: _expertise.ExpertiseID,
        //                                                repdate: rd.GetDateTime(colReportDate),
        //                                                delay: rd.GetDateTime(colDelayDate),
        //                                                reason: rd[colReason] == DBNull.Value ? null : rd.GetString(colReason),
        //                                                vr: Version.Original);
        //                    _expertise.Reports.Add(_report);
        //                }
        //                if (rd[colBillID] != DBNull.Value && !_expertise.Bills.Any(n => n.BillID == rd.GetInt32(colBillID)))
        //                {
        //                    var _bill = new Bill(id: rd.GetInt32(colBillID),
        //                                        expertise: _expertise.ExpertiseID,
        //                                        number: rd.GetString(colBillNumber),
        //                                        billdate: rd.GetDateTime(colBillDate),
        //                                        paiddate: rd[colPaidDate] == DBNull.Value ? null : new DateTime?(rd.GetDateTime(colPaidDate)),
        //                                        payer: rd.GetString(colPayer),
        //                                        hours: rd.GetByte(colNHours),
        //                                        hourprice: rd.GetDecimal(colHourprice),
        //                                        paid: rd.GetDecimal(colPaid),
        //                                        vr: Version.Original);
        //                    _expertise.Bills.Add(_bill);
        //                }
        //            }
        //            rd.Close();
        //        }
        //    }
        //    //catch (Exception)
        //    //{
        //    //    throw;
        //    //}
        //    finally
        //    {
        //        connection.Close();
        //    }
        //    return resolutions;
        //}
    }

    public class RelayCommand : ICommand
    {
        private Action<object> _execute;
        private Func<object, bool> _canexecute;

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public RelayCommand(Action<object> exec, Func<object, bool> canexec = null)
        {
            _execute = exec; _canexecute = canexec;
        }

        public bool CanExecute(object parameter)
        {
            return _canexecute == null || _canexecute.Invoke(parameter);
        }
        public void Execute(object parameter)
        {
            _execute(parameter);
        }
    }

    public class BaseViewModel<T> : INotifyPropertyChanged
                                where T : NotifyBase
    {
        public T CurrentItem { get; set; }
        private RelayCommand _additem;
        private RelayCommand _deleteitem;
        private RelayCommand _save;

        #region Commands

        public RelayCommand AddItem => _additem ?? (_additem = new RelayCommand(o => ItemsList.Add((T)o)));

        //public RelayCommand DeleteItem => _deleteitem ?? (_deleteitem = new RelayCommand(
        //            o => { T item = (T)o; item.DeleteFromDB(CommonInfo.connection); ItemsList.Remove(item); },
        //            o => CurrentItem == null
        //            ));

        #endregion Commands

        public ObservableCollection<T> ItemsList { get; } = new ObservableCollection<T>();

        public Dictionary<string, IReadOnlyList<string>> ConstantData { get; } = new Dictionary<string, IReadOnlyList<string>>();

        public BaseViewModel(T item)
        {
            CurrentItem = item;
        }

        public BaseViewModel(IEnumerable<T> items)
        {
            if (items is ObservableCollection<T>) ItemsList = (ObservableCollection<T>)items;
            else
                foreach (var item in items)
                {
                    ItemsList.Add(item);
                }
        }

        public void AddConstData(string name, IReadOnlyList<string> data)
        {
            ConstantData.Add(name, data);
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public enum MsgType
    {
        Normal = 0x0001,
        Temporary = 0x0002,
        Warning = 0x0004,
        Error = 0x0008,
        Congratulation = 0x0010
    }

    public class Message
    {
        private string _msg;
        private MsgType _msgtype;
        private DateTime _inicialtime;
        private TimeSpan _lifetime;

        public TimeSpan LifeTime
        {
            get => _lifetime;
            set => _lifetime = value;
        }
        public DateTime InicialTime => _inicialtime;
        public MsgType Type
        {
            get => _msgtype;
            set => _msgtype = value;
        }
        public string Msg
        {
            get => _msg;
            set => _msg = value;
        }

        public override string ToString()
        {
            return _msg + " (" + _inicialtime.ToString() + ")";
        }

        //public Message()
        //{
        //    _inicialtime = DateTime.Now;
        //}
        public Message(string msg, MsgType type, TimeSpan lifetime)
        {
            _msg = msg; _msgtype = type; _inicialtime = DateTime.Now; _lifetime = lifetime;
        }
        public Message(string msg) : this(msg, MsgType.Temporary, TimeSpan.FromSeconds(5)) { }
        public Message(string msg, MsgType type) : this(msg, type, TimeSpan.FromSeconds(5)) { }
    }
    public class MessageQuery : ObservableCollection<Message>
    {
        private DispatcherTimer timer;

        public int TickInterval { get; set; } = 1;

        private void OnTimerTick(object sender, EventArgs e)
        {
            var fil = this.Where(x => DateTime.Now - x.InicialTime > x.LifeTime).ToArray();
            foreach (var item in fil)
            {
                if (item.Type == MsgType.Temporary) Remove(item);
            }
        }

        public MessageQuery()
        {
            timer = new DispatcherTimer(DispatcherPriority.Normal)
            {
                Interval = TimeSpan.FromSeconds(TickInterval),
            };
            timer.Tick += OnTimerTick;
            timer.Start();
        }
    }
    public class Passport
    {
        private string _firstName;
        private string _middleName;
        private string _secondName;
        private string _gender;
        private DateTime _issueDate;
        private DateTime _birthDate;
        private string _birthPlace;
        private string _granted;
        private string _number;
        private string _depCode;
        private string _series;
        private Adress _registration;

        public Adress Registration
        {
            get => _registration;
            set => _registration = value;
        }

        public string Series
        {
            get => _series;
            set => _series = value;
        }

        public string DepartmentCode
        {
            get => _depCode;
            set => _depCode = value;
        }

        public string Number
        {
            get => _number;
            set => _number = value;
        }

        public string Granted
        {
            get => _granted;
            set => _granted = value;
        }

        public string BirthPlace
        {
            get => _birthPlace;
            set => _birthPlace = value;
        }

        public DateTime BirthDate
        {
            get => _birthDate;
            set => _birthDate = value;
        }

        public DateTime IssueDate
        {
            get => _issueDate;
            set => _issueDate = value;
        }

        public string Gender
        {
            get => _gender;
            set => _gender = value;
        }

        public string SecondName
        {
            get => _secondName;
            set => _secondName = value;
        }

        public string MiddleName
        {
            get => _middleName;
            set => _middleName = value;
        }

        public string FirstName
        {
            get => _firstName;
            set => _firstName = value;
        }
    }
    public class DataBaseActionEventArgs : EventArgs
    {
        public DBAction Action { get; set; }
        public DataBaseActionEventArgs(DBAction action) : base()
        {
            Action = action;
        }
    }
    public abstract class NotifyBase : INotifyPropertyChanged
    {
        private Version _version;
        private DateTime _updatedate;
        
        public DateTime UpdateDate
        {
            get => _updatedate;
            set { _updatedate = value; OnPropertyChanged(); }
        }
        public Version Version
        {
            get => _version;
            set { _version = value; OnPropertyChanged("Version"); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public abstract void SaveChanges(SqlConnection con);
        protected void OnPropertyChanged([CallerMemberName]string prop = null, bool contentAdd = false)
        {
            switch (_version)
            {
                case Version.Original:
                    if (prop != "Version")
                    {
                        if (contentAdd) Version = Version.ContentEdited;
                        else Version = Version.Edited;
                    }
                    break;
                case Version.ContentEdited:
                    if (prop != "Version")
                    {
                        if (!contentAdd) Version = Version.Edited;
                    }
                    break;
                default:
                    break;
            }
            _updatedate = DateTime.Now;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
#if DEBUG
            //Debug.WriteLine($"Property changed {prop} ({Version})", "NotifyBase delegate");
#endif
           
        }
        protected object ConvertToDBNull<T>(T obj)
        {
            if (obj == null) return DBNull.Value;
            else return obj;
        }
       
        public NotifyBase() : this(Version.New, DateTime.Now) {}
        public NotifyBase(Version vr) : this(vr, DateTime.Now) {}
        public NotifyBase(Version vr, DateTime updatedate)
        {
             _version = vr; _updatedate = updatedate;
        }
    }
    public sealed class Speciality : NotifyBase, ICloneable
    {
 #region Fields
        private int _id;
        private string _code;
        private string _species;
        private Byte? _category_1;
        private Byte? _category_2;
        private Byte? _category_3;
        private string _acronym;
        private bool _status;
        #endregion
 #region Properties
        public int SpecialityID => _id;
        public string Code
        {
            get => _code;
            set
            {
                if (_code == value) return;
                if (String.IsNullOrWhiteSpace(value)) throw new ArgumentException("Поле <специальность> не может быть пустым");
                _code = value;
                OnPropertyChanged("Code");
            }
        }
        public string Species
        {
            get => _species;
            set { if (_species == value) return; _species = value; OnPropertyChanged("Species"); }
        }
        public Byte? Category_1
        {
            get => _category_1;
            set
            {
                if (_category_1 != value)
                {
                    if (value >= _category_2 || value >= _category_3) throw new ArgumentException("!");
                    _category_1 = value;
                    OnPropertyChanged("Category_1");
                }
            }
        }
        public Byte? Category_2
        {
            get => _category_2;
            set
            {
                if (_category_2 != value)
                {
                    if (value <= _category_1 || value >= _category_3) throw new ArgumentException("!!");
                    _category_2 = value;
                    OnPropertyChanged("Category_2");
                }
            }
        }
        public Byte? Category_3
        {
            get => _category_3;
            set
            {
                if (_category_3 != value)
                {
                    if (value <= _category_2 || value <= _category_1) throw new ArgumentException("!!!");
                    _category_3 = value;
                    OnPropertyChanged("Category_3");
                }
            }
        }
        public string Acronym
        {
            get => _acronym;
            set { if (_acronym == value) return; _acronym = String.IsNullOrWhiteSpace(value) ? null : value; OnPropertyChanged(); }
        }
        public string Categories
        {
            get
            {
                StringBuilder r = new StringBuilder(_category_1.HasValue ? _category_1.ToString() : "-", 14);
                r.Append("/");
                r.Append(_category_2.HasValue ? _category_2.ToString() : "-");
                r.Append("/");
                r.Append(_category_3.HasValue ? _category_3.ToString() : "-");
                return r.ToString();
            }
        }
        public string FullTitle
        {
            get
            {
                if (Acronym != null) return $"{Code} ({Acronym})";
                else return Code;
            }
        }
        public bool IsValid
        {
            get => _status;
            set { if (_status == value) return; _status = value; OnPropertyChanged("SpecialityStatus"); }
        }
        public bool IsInstanceValidState => !String.IsNullOrWhiteSpace(_code);
        #endregion

        public Speciality() : base() {}
        public Speciality(int id, string code, string species, Byte? cat_1, Byte? cat_2, Byte? cat_3, string acr, bool status, Version vr, DateTime updatedate)
            : base(vr, updatedate)
        {
            _id = id; _code = code; _species = species; _category_1 = cat_1; _category_2 = cat_2; _category_3 = cat_3;
            _acronym = acr; _status = status;
        }
        #region Metods
        private void AddToDB(SqlConnection con)
        {
            SqlCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "InnResources.prAddSpeciality";
            cmd.Parameters.Add("@Code", SqlDbType.NVarChar, 205).Value = Code;
            cmd.Parameters.Add("@Cat1", SqlDbType.TinyInt).Value = ConvertToDBNull(Category_1);
            cmd.Parameters.Add("@Cat2", SqlDbType.TinyInt).Value = ConvertToDBNull(Category_2);
            cmd.Parameters.Add("@Cat3", SqlDbType.TinyInt).Value = ConvertToDBNull(Category_3);
            cmd.Parameters.Add("@Species", SqlDbType.NVarChar, 75).Value = ConvertToDBNull(Species);
            cmd.Parameters.Add("@Acronym", SqlDbType.NVarChar, 10).Value = ConvertToDBNull(Acronym);
            var p = cmd.Parameters.Add("@InsertedID", SqlDbType.SmallInt);
            p.Direction = ParameterDirection.Output;
            try
            {
                cmd.Connection.Open();
                cmd.ExecuteNonQuery();
                _id = (Int16)cmd.Parameters["@InsertedID"].Value;
                Version = Version.Original;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                cmd.Connection.Close();
            }
        }
        private void EditToDB(SqlConnection con)
        {
            SqlCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "InnResources.prEditSpeciality";
            cmd.Parameters.Add("@Code", SqlDbType.NVarChar, 205).Value = Code;
            cmd.Parameters.Add("@Cat1", SqlDbType.TinyInt).Value = ConvertToDBNull(Category_1);
            cmd.Parameters.Add("@Cat2", SqlDbType.TinyInt).Value = ConvertToDBNull(Category_2);
            cmd.Parameters.Add("@Cat3", SqlDbType.TinyInt).Value = ConvertToDBNull(Category_3);
            cmd.Parameters.Add("@Species", SqlDbType.NVarChar, 75).Value = ConvertToDBNull(Species);
            cmd.Parameters.Add("@Acronym", SqlDbType.NVarChar, 10).Value = ConvertToDBNull(Acronym);
            cmd.Parameters.Add("@Status", SqlDbType.Bit).Value = _status;
            cmd.Parameters.Add("@SpecialityID", SqlDbType.Int).Value = SpecialityID;
            try
            {
                cmd.Connection.Open();
                cmd.ExecuteNonQuery();
                Version = Version.Original;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                cmd.Connection.Close();
            }
        }
        public void DeleteFromDB(SqlConnection con)
        {
            SqlCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "InnResources.prDeleteSpeciality";
            cmd.Parameters.Add("@SpecialityID", SqlDbType.SmallInt).Value = SpecialityID;
            try
            {
                con.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                cmd.Connection.Close();
            }
        }
        public override void SaveChanges(SqlConnection con)
        {
            switch (Version)
            {
                case Version.New:
                    AddToDB(con);
                    break;
                case Version.Edited:
                    EditToDB(con);
                    break;
                default:
                    break;
            }
        }
        public override string ToString()
        {
            return Code;
        }
        public Speciality Clone()
        {
            return new Speciality(id: SpecialityID,
                                    code: _code,
                                    species: _species,
                                    cat_1: _category_1,
                                    cat_2: _category_2,
                                    cat_3: _category_3,
                                    acr: _acronym,
                                    status: _status,
                                    vr: this.Version,
                                    updatedate: this.UpdateDate);
        }
        object ICloneable.Clone() => Clone();
        public void Copy(Speciality sp)
        {
            Code = sp._code;
            Species = sp._species;
            Category_1 = sp._category_1;
            Category_2 = sp._category_2;
            Category_3 = sp._category_3;
            Acronym = sp._acronym;
            IsValid = sp._status;
            _id = sp._id;
        }
        #endregion
    }
    public sealed class Settlement : NotifyBase, IEquatable<Settlement>, ICloneable
    {
#region Fields
        private string _title;
        private string _settlementtype;
        private string _significance;
        private string _telephonecode;
        private string _postcode;
        private string _federallocation;
        private string _territorylocation;
        private bool _isvalid;
        private int _id;

        #endregion Fields
#region Properties
        public int SettlementID => _id;
        public bool IsValid
        {
            get { return _isvalid; }
            set { 
                    if (value != _isvalid)
                    {
                        _isvalid = value;
                        OnPropertyChanged();
                    }
                 }
        }
        public string Title
        {
            get => _title;
            set
            {
                if (String.IsNullOrEmpty(value)) throw new ArgumentException("Поле <название> не может быть пустым");
                _title = value;
                OnPropertyChanged();
            }
        }
        public string Settlementtype
        {
            get => _settlementtype;
            set
            {
                if (String.IsNullOrEmpty(value)) throw new ArgumentException("Поле <тип> не может быть пустым");
                _settlementtype = value;
                OnPropertyChanged();
            }
        }
        public string Significance
        {
            get => _significance;
            set
            {
                if (String.IsNullOrEmpty(value)) throw new ArgumentException("Поле <значение> не может быть пустым");
                _significance = value;
                OnPropertyChanged();
            }
        }
        public string Telephonecode
        {
            get => _telephonecode;
            set { _telephonecode = value; OnPropertyChanged(); }
        }
        public string Postcode
        {
            get => _postcode;
            set { _postcode = value; OnPropertyChanged(); }
        }
        public string Federallocation
        {
            get => _federallocation;
            set { _federallocation = value; OnPropertyChanged(); }
        }
        public string Territorylocation
        {
            get => _territorylocation;
            set { _territorylocation = value; OnPropertyChanged(); }
        }
        public bool IsInstanceValidState => !String.IsNullOrWhiteSpace(_title) && _settlementtype != null && _significance != null; 
        #endregion Properties
        public Settlement() : base() {}
        public Settlement(int id, string title, string type, string significance, string telephonecode, string postcode, string federallocation,
                            string territoriallocation, bool isvalid, Version vr, DateTime updatedate) : base(vr, updatedate)
        {
            _id = id;
            _title = title;
            _settlementtype = type;
            _significance = significance;
            _telephonecode = telephonecode;
            _postcode = postcode;
            _federallocation = federallocation;
            _territorylocation = territoriallocation;
            _isvalid = isvalid;
        }
        #region Methods
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(200);
            if (Significance == "федеральный") sb.Append(Title);
            else
            {
                if (Significance == "нет")
                {
                    sb.Append(Settlementtype);
                    sb.Append(" ");
                    sb.Append(Title);
                    if (Territorylocation != null)
                    {
                        sb.Append(", ");
                        sb.Append(Territorylocation.ToString());
                    }
                    if (Federallocation != null)
                    {
                        sb.Append(", ");
                        sb.Append(Federallocation.ToString());
                    }
                }
                else
                {
                    if (Significance == "районный")
                    {
                        sb.Append(Settlementtype);
                        sb.Append(" ");
                        sb.Append(Title);
                        if (Federallocation != null)
                        {
                            sb.Append(", ");
                            sb.Append(Federallocation.ToString());
                        }
                    }
                    else
                    {
                        sb.Append(Settlementtype);
                        sb.Append(" ");
                        sb.Append(Title);
                    }
                }
            }
            return sb.ToString();
        }
        private void AddToDB(SqlConnection con)
        {
            SqlCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "OutResources.prAddSettlement";
            cmd.Parameters.Add("@Title", SqlDbType.NVarChar, 40).Value = Title;
            cmd.Parameters.Add("@SettlementType", SqlDbType.NVarChar, 20).Value = Settlementtype;
            cmd.Parameters.Add("@Significance", SqlDbType.NVarChar, 15).Value = Significance;
            cmd.Parameters.Add("@FederalLocation", SqlDbType.VarChar, 50).Value = ConvertToDBNull(Federallocation);
            cmd.Parameters.Add("@TerritorialLocation", SqlDbType.NVarChar, 50).Value = ConvertToDBNull(Territorylocation);
            cmd.Parameters.Add("@TelephoneCode", SqlDbType.NVarChar, 8).Value = ConvertToDBNull(Telephonecode);
            cmd.Parameters.Add("@PostCode", SqlDbType.NVarChar, 13).Value = ConvertToDBNull(Postcode);
            var par = cmd.Parameters.Add("@InsertedID", SqlDbType.Int);
            par.Direction = ParameterDirection.Output;
            try
            {
                cmd.Connection.Open();
                cmd.ExecuteNonQuery();
                _id = (int)cmd.Parameters["@InsertedID"].Value;
                Version = Version.Original;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                cmd.Connection.Close();
            }
        }
        private void EditToDB(SqlConnection con)
        {
            SqlCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "OutResources.prEditSettlement";
            cmd.Parameters.Add("@Title", SqlDbType.NVarChar, 40).Value = Title;
            cmd.Parameters.Add("@SettlementType", SqlDbType.NVarChar, 20).Value = Settlementtype;
            cmd.Parameters.Add("@Significance", SqlDbType.NVarChar, 15).Value = Significance;
            cmd.Parameters.Add("@FederalLocation", SqlDbType.VarChar, 50).Value = ConvertToDBNull(Federallocation);
            cmd.Parameters.Add("@TerritorialLocation", SqlDbType.NVarChar, 50).Value = ConvertToDBNull(Territorylocation);
            cmd.Parameters.Add("@TelephoneCode", SqlDbType.NVarChar, 8).Value = ConvertToDBNull(Telephonecode);
            cmd.Parameters.Add("@PostCode", SqlDbType.NVarChar, 13).Value = ConvertToDBNull(Postcode);
            cmd.Parameters.Add("@Status", SqlDbType.Bit).Value = IsValid;
            cmd.Parameters.Add("@SettlementID", SqlDbType.Int).Value = SettlementID;
            try
            {
                cmd.Connection.Open();
                cmd.ExecuteNonQuery();
                Version = Version.Original;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                cmd.Connection.Close();
            }
        }
        public void DeleteFromDB(SqlConnection con)
        {
            SqlCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "OutResources.prDeleteSettlement";
            cmd.Parameters.Add("@SettlementID", SqlDbType.Int).Value = SettlementID;
            try
            {
                cmd.Connection.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                cmd.Connection.Close();
            }
        }
        public override void SaveChanges(SqlConnection con)
        {
            switch (Version)
            {
                case Version.New:
                    AddToDB(con);
                    break;
                case Version.Edited:
                    EditToDB(con);
                    break;

            }
        }
        public bool Equals(Settlement other)
        {
            if (other == null) return false;
            return Title != null ? Title.Equals(other.Title, StringComparison.OrdinalIgnoreCase) : Title == other.Title &&
            Settlementtype != null ? Settlementtype.Equals(other.Settlementtype, StringComparison.OrdinalIgnoreCase) : Settlementtype == other.Settlementtype &&
            Federallocation != null ? Federallocation.Equals(other.Federallocation, StringComparison.OrdinalIgnoreCase) : Federallocation == other.Federallocation;
        }
        object ICloneable.Clone() => Clone();
        public Settlement Clone()
        {
            return new Settlement(SettlementID, _title, _settlementtype, _significance, _telephonecode, _postcode, _federallocation, _territorylocation, _isvalid, 
                                    this.Version, this.UpdateDate);

        }
        public void Copy(Settlement s)
        {
            _id = s._id;
            IsValid = s._isvalid;
            Title = s._title;
            Settlementtype = s._settlementtype;
            Significance = s._significance;
            Telephonecode = s._telephonecode;
            Postcode = s._postcode;
            Federallocation = s._federallocation;
            Territorylocation = s._territorylocation;
        }
        #endregion
    }
    internal class AdressEventArgs : EventArgs
    {
        private string _propertyName;
        public string PropertyName => _propertyName;

        public AdressEventArgs(string prop)
        {
            _propertyName = prop;
        }
    }
    public class Adress : IEquatable<Adress>, INotifyPropertyChanged, ICloneable
    {
        #region Fields

        private Settlement _settlement;
        private string _street;
        private string _streetprefix;
        private string _housing;
        private string _flat;
        private string _corpus;
        private string _structure;

        #endregion Fields
        #region Properties
        public string Structure
        {
            get => _structure;
            set { _structure = value; OnAdressPropertyChanged("Structure"); }
        }
        public string Streetprefix
        {
            get => _streetprefix;
            set { _streetprefix = value; OnAdressPropertyChanged("Streetprefix"); }
        }
        public string Street
        {
            get => _street;
            set { _street = value; OnAdressPropertyChanged("Street"); }
        }
        public string Flat
        {
            get => _flat;
            set { _flat = value; OnAdressPropertyChanged("Flat"); }
        }
        public string Corpus
        {
            get => _corpus;
            set { _corpus = value; OnAdressPropertyChanged("Corpus"); }
        }
        public string Housing
        {
            get => _housing;
            set { _housing = value; OnAdressPropertyChanged("Housing"); }
        }
        public Settlement Settlement
        {
            get => _settlement;
            set
            {
                if (_settlement == value) return;
                _settlement = value;
                OnAdressPropertyChanged("Settlement");
            }
        }
        public bool IsInstanceValidState => _settlement != null && !String.IsNullOrWhiteSpace(_streetprefix) && !String.IsNullOrWhiteSpace(_street)
                                             && !String.IsNullOrWhiteSpace(_housing);
        #endregion Properties

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnAdressPropertyChanged([CallerMemberName]string prop = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
            //Debug.WriteLine("Property changed " + prop, "Adress delegate");
        }

        public Adress() { }
        public Adress(Settlement settlement, string streetprefix, string street, string housing, string flat, string corpus, string structure)
        {
            _settlement = settlement;
            _street = street;
            _streetprefix = streetprefix;
            _housing = housing;
            _flat = flat;
            _corpus = corpus;
            _structure = structure;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(300);
            sb.Append(Streetprefix);
            sb.Append(" "); 
            sb.Append(Street);
            if (Housing != null)
            {
                sb.Append(", д. ");
                sb.Append(Housing);
            }
            if (Corpus != null)
            {
                sb.Append(", корп. ");
                sb.Append(Corpus);
            }   
            if (Structure != null)
            {
                sb.Append(", стр. ");
                sb.Append(Structure);
            }    
            if (Flat != null)
            {
                sb.Append(", кв.(оф.) ");
                sb.Append(Flat);
            }          
            if (Settlement != null)
            {
                sb.Append(", ");
                sb.Append(Settlement.ToString());
            }
            return sb.ToString();
        }
        public bool Equals(Adress other)
        {
            if (other == null) return false;
            return Settlement != null ? Settlement.Equals(other.Settlement) : Settlement == other.Settlement &&
                Street != null ? Street.Equals(other.Street, StringComparison.OrdinalIgnoreCase) : Street == other.Street &&
                Housing != null ? Housing.Equals(other.Housing, StringComparison.OrdinalIgnoreCase) : Housing == other.Housing &&
                Flat == other.Flat;
        }
        object ICloneable.Clone()
        {
            return Clone();
        }
        public Adress Clone()
        {
            return new Adress
            {
                Structure = _structure,
                Corpus = _corpus,
                Flat = _flat,
                Housing = _housing,
                Streetprefix = _streetprefix,
                Street = _street,
                Settlement = _settlement?.Clone()
            };
        }
    }
    public sealed class Departament : IEquatable<Departament>
    {
        private string _title;
        private string _digitalcode;
        private string _acronym;
        private byte _departamentID;
        private bool _isvalid;

        public bool IsValid
        {
            get { return _isvalid; }
            set { 
                    if (value != _isvalid)
                    {
                        _isvalid = value;
                    }
                }
        }
        public string DigitalCode
        {
            get => _digitalcode;
            set
            {
                if (value != _digitalcode) _digitalcode = value;
            }
        }
        public byte DepartamentID
        {
            get => _departamentID;
            private set => _departamentID = value;
        }
        public string Title
        {
            get => _title;
            set => _title = value;
        }
        public string Acronym
        {
            get => _acronym;
            set => _acronym = value.ToUpper();
        }

        public Departament(byte id, string title, string acronym, string code, bool isvalid)
        {
            _departamentID = id;
            _title = title;
            _acronym = acronym;
            _digitalcode = code;
            _isvalid = isvalid;
        }
        public Departament() { }
        public Departament(Departament dep)
        {
            _departamentID = dep.DepartamentID;
            _title = dep.Title;
            _acronym = dep.Acronym;
            _digitalcode = dep.DigitalCode;
            _isvalid = dep.IsValid;
        }
        public override string ToString()
        {
            return Acronym;
        }
        public bool Equals(Departament other)
        {
            if (other == null) return false;
            return Title.Equals(other.Title, StringComparison.CurrentCultureIgnoreCase);
        }
    }
    public class Person : NotifyBase, IFormattable, ICloneable
    {
 #region Fields
        protected string _fname;
        protected bool _declinated;
        protected string _mname;
        protected string _sname;
        protected string _mobilephone;
        protected string _workphone;
        protected string _gender;
        protected string _email;
        protected Adress _adress = new Adress();
        protected int _id;
        #endregion

#region Properties
        public int PersonID => _id;
        public string Fname
        {
            get => _fname;
            set
            {
                if (_fname == value) return;
                if (!isValidName(value)) throw new ArgumentException("Неверный формат имени");
                _fname = value.ToUpperFirstLetter();
                OnPropertyChanged();
                OnPropertyChanged("Fio");
            }
        }
        public string Mname
        {
            get => _mname;
            set
            {
                if (_mname == value) return;
                if (!isValidMiddleName(value)) throw new ArgumentException("Неверный формат отчества");
                _mname = value.ToUpperFirstLetter();
                OnPropertyChanged();
                OnPropertyChanged("Fio");
            }
        }
        public string Sname
        {
            get => _sname;
            set
            {
                if (_sname == value) return;
                if (!isValidSecondName(value)) throw new ArgumentException("Неверный формат фамилии. Допускаются буквы русского алфавита и одиночный '-'");
                _sname = value.ToLower().ToUpperFirstLetter();
                OnPropertyChanged();
                OnPropertyChanged("Fio");
            }
        }
        public string Mobilephone
        {
            get => MobilePnoneStandartNumber(_mobilephone);
            set
            {
                if (_mobilephone == value) return;
                var trim = Regex.Replace(value, "[-() ]", "");
                if (Regex.IsMatch(trim, @"^\+7|8[1-9]\d{9}$")) 
                {
                    StringBuilder sb = new StringBuilder(trim);
                    if (trim.Length == 11) sb.Replace("8", "+7", 0, 1);
                    _mobilephone = sb.ToString();
                }
                else throw new ArgumentException("Неверный формат мобильного номера");
                OnPropertyChanged();
            }
        }
        public string Gender
        {
            get => _gender;
            set
            {
                if (_gender == value) return;
                if (String.IsNullOrWhiteSpace(value)) throw new ArgumentException("Поле <пол> не может быть пустым");
                _gender = value;
                OnPropertyChanged();
            }
        }
        public string Email
        {
            get => _email;
            set
            {
                if (_email != value)
                {
                    if (!isValidEmail(value)) throw new ArgumentException("Неверный фoрмат Email");
                    _email = value;
                    OnPropertyChanged();
                }
            }
        }
        public Adress Adress
        {
            get => _adress;
            set
            {
                if (_adress == value) return;
                _adress = value;
                OnPropertyChanged();
            }
        }
        public string Workphone
        {
            get => WorkPnoneStandartNumber(_workphone);
            set
            {
                if (_workphone == value) return;
                var trim = Regex.Replace(value, "[-() ]", "");
                if (Regex.IsMatch(trim, @"^[1-9]\d{3,6}$"))
                {
                    _workphone = trim;
                }
                else throw new ArgumentException("Неверный формат номера");
                OnPropertyChanged();
            }
        }
        public bool Declinated
        {
            get => _declinated;
            set
            {
                if (_declinated == value) return;
                _declinated = value;
                OnPropertyChanged();
            }
        }
        public string Fio => Sname + " " + Fname[0] + "." + Mname[0] + ".";
        public bool IsInstanceValidState => isValidName(_fname) && isValidMiddleName(_mname) && isValidSecondName(_sname)
                                            && !String.IsNullOrWhiteSpace(_gender);
        #endregion

        public Person() : base()
        {
            _adress.PropertyChanged += AdressChanged;
        }
        public Person(int id,string firstname, string middlename, string secondname, string mobilephone, string workphone, string gender, string email, Adress adress, bool declinated, Version vr, DateTime updatedate)
            : base(vr, updatedate)
        {
            _id = id;
            _fname = firstname;
            _mname = middlename;
            _sname = secondname;
            _mobilephone = mobilephone;
            _workphone = workphone;
            _gender = gender;
            _email = email;
            _adress = adress ?? new Adress();
            _adress.PropertyChanged += AdressChanged;
            _declinated = declinated;
        }

        #region Methods
        private string MobilePnoneStandartNumber(string mobilephone)
        {
            if (mobilephone == null) return null;
            StringBuilder sb = new StringBuilder(mobilephone);
            sb.Insert(10, "-");
            sb.Insert(8, "-");
            sb.Insert(5, " ");
            sb.Insert(2, " ");
            return sb.ToString();
        }
        private string WorkPnoneStandartNumber(string workphone)
        {
            if (workphone == null) return null;
            StringBuilder sb = new StringBuilder(workphone);
            var x = workphone.Length;
            sb.Insert(x - 2, "-");
            if (x > 4) sb.Insert(x - 4, "-");
            return sb.ToString();
        }
        private void AdressChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(e.PropertyName);
        }
        public bool isValidName(string name)
        {
            Regex regex = new Regex(@"^\p{IsCyrillic}{2,25}$", RegexOptions.IgnoreCase);
            return regex.IsMatch(name);
        }
        public bool isValidMiddleName(string mname)
        {
            Regex regex = new Regex(@"^\p{IsCyrillic}{2,25}$", RegexOptions.IgnoreCase);
            return regex.IsMatch(mname);
        }
        public bool isValidSecondName(string sname)
        {
            Regex regex = new Regex(@"^\p{IsCyrillic}{2,15}(?:-\p{IsCyrillic}{2,15})?$", RegexOptions.IgnoreCase);
            return regex.IsMatch(sname);
        }
        public bool isValidEmail(string mail)
        {
            Regex regex = new Regex(@"\A[^@]+@([^@\.]+\.)+[^@\.]+\z", RegexOptions.Compiled);
            if (regex.IsMatch(mail)) return true;
            else return false;
        }
        /// <summary>
        /// Склоняет фамилию в указанном параметром падеже
        /// </summary>
        /// <param name="case">Требуемое склонение</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException">Пол не мужской или женский</exception>
        /// <exception cref="NotSupportedException">Склонение не предусмотрено</exeption>
        protected string SurnameDeclinate(LingvoNET.Case @case)
        {
            if (_declinated == false) return Sname;
            var devide = Sname.Split(separator: new char[] { '-' }, options: StringSplitOptions.RemoveEmptyEntries, count: 2);//двойная или одинарная фамилия, более двойной запрещено законом
            if (Gender == "мужской")
            {
                string[] parts = new string[devide.Length];
                string l2, l1;
                for (int i = 0; i < devide.Length; i++)
                {
                    l2 = devide[i].LastRight(2); l1 = devide[i].LastRight(1);
                    if (l2 == "ий" || l2 == "ый" || l2 == "ой")
                    {
                        var en = Nouns.FindOne(devide[i]);
                        if (en != null)
                        {
                            parts[i] = en[@case];
                        }
                        else
                        {
                            var a = Adjectives.FindSimilar(devide[i]);
                            if (a != null) parts[i] = a[@case, LingvoNET.Gender.MA];
                            else throw new NotSupportedException($"Не удалось склонить фамилию {Sname}");
                        }
                        continue;
                    }
                    if (l1 == "о" || l1 == "и" || l1 == "ю" || l1 == "у" || l1 == "е")
                    {
                        parts[i] = devide[i];
                        continue;
                    }
                    if (l2 == "ых" || l2 == "их")
                    {
                        var n = Nouns.FindOne(devide[i]);
                        if (n != null) parts[i] = n[@case];
                        else parts[i] = devide[i];
                        continue;
                    }
                    var m = Nouns.FindOne(devide[i]);
                    if (m != null) parts[i] = m[@case];
                    else
                    {
                        try
                        {
                            switch (@case)
                            {
                                case LingvoNET.Case.Nominative:
                                    parts[i] = devide[i];
                                    break;
                                case LingvoNET.Case.Genitive:
                                    parts[i] = Word.NounToGenetive(devide[i]);
                                    break;
                                case LingvoNET.Case.Dative:
                                    parts[i] = Word.NounToDative(devide[i]);
                                    break;
                                case LingvoNET.Case.Accusative:
                                case LingvoNET.Case.Instrumental:
                                case LingvoNET.Case.Locative:
                                    throw new NotSupportedException("Нереализованный тип склонения");
                                default:
                                    break;
                            }
                        }
                        catch (Exception)
                        {
                            throw;
                        }
                    }
                }
                return String.Join("-", parts);
            }
            if (Gender == "женский")
            {
                string l2, l1;
                string[] parts = new string[devide.Length];
                for (int i = 0; i < devide.Length; i++)
                {
                    l2 = devide[i].LastRight(2); l1 = devide[i].LastRight(1);
                    if (l2 == "ая" || l2 == "яя")
                    {
                        var en = Nouns.FindOne(devide[i]);
                        if (en != null)
                        {
                            parts[i] = en[@case];
                        }
                        else
                        {
                            var a = Adjectives.FindSimilar(devide[i]);
                            if (a != null) parts[i] = a[@case, LingvoNET.Gender.FA];
                            else throw new NotSupportedException($"Не удалось склонить фамилию {Sname}");
                        }
                        continue;
                    }
                    if (l1 == "о" || l1 == "и" || l1 == "ю" || l1 == "у" || l1 == "е")
                    {
                        parts[i] = devide[i];
                        continue;
                    }
                    if (l2 == "ых" || l2 == "их")
                    {
                        parts[i] = devide[i];
                        continue;
                    }
                    var w = Word.Determine(devide[i], PartOfSpeech.Noun);
                    switch (w._WordGender)
                    {
                        case WordGender.Male:
                            parts[i] = devide[i];
                            break;
                        case WordGender.Female:
                            var en = Nouns.FindOne(devide[i]);
                            if (en != null)
                            {
                                parts[i] = en[@case];
                            }
                            else
                            {
                                bool st = (devide[i].LastRight(3) == "ова" || devide[i].LastRight(3) == "ева" || devide[i].LastRight(3) == "ина"
                                                            || devide[i].LastRight(3) == "ына" || devide[i].LastRight(3) == "ёва");
                                {
                                    switch (@case)
                                    {
                                        case LingvoNET.Case.Nominative:
                                            parts[i] = devide[i];
                                            break;
                                        case LingvoNET.Case.Genitive:
                                            parts[i] = st ? devide[i].PositionReplace("ой", devide[i].Length - 1) : w.ToGenetive();
                                            break;
                                        case LingvoNET.Case.Dative:
                                            parts[i] = st ? devide[i].PositionReplace("ой", devide[i].Length - 1) : w.ToDative();
                                            break;
                                        case LingvoNET.Case.Accusative:
                                        case LingvoNET.Case.Instrumental:
                                        case LingvoNET.Case.Locative:
                                            throw new NotSupportedException("Нереализованный тип склонения");
                                        default:
                                            throw new NotSupportedException("Нереализованный тип склонения");
                                    }
                                }
                            }
                            break;
                        default:
                            throw new NotSupportedException($"Не удалось склонить фамилию {Sname}");
                    }
                }
                return String.Join("-", parts);
            }
            else throw new NotImplementedException("Пол неизвестен. Склонение фамилии невозможно");
        }
        protected string MiddleNameDeclinate(LingvoNET.Case @case)
        {
            switch (@case)
            {
                case LingvoNET.Case.Nominative:
                    return Mname;
                case LingvoNET.Case.Genitive:
                    if (Mname.LastRight(1) == "ч" && Gender == "мужской") return Mname + "а";
                    if (Mname.LastRight(1) == "а" && Gender == "женский") return Mname.PositionReplace("ы", Mname.Length - 1);
                    throw new NotSupportedException("Склонение отчества не поддерживается");
                case LingvoNET.Case.Dative:
                    if (Mname.LastRight(1) == "ч" && Gender == "мужской") return Mname + "у";
                    if (Mname.LastRight(1) == "а" && Gender == "женский") return Mname.PositionReplace("е", Mname.Length - 1);
                    throw new NotSupportedException("Склонение отчества не поддерживается");
                case LingvoNET.Case.Accusative:
                    if (Mname.LastRight(1) == "ч" && Gender == "мужской") return Mname + "а";
                    if (Mname.LastRight(1) == "а" && Gender == "женский") return Mname.PositionReplace("у", Mname.Length - 1);
                    throw new NotSupportedException("Склонение отчества не поддерживается");
                case LingvoNET.Case.Instrumental:
                    if (Mname.LastRight(1) == "ч" && Gender == "мужской") return Mname + "ем";
                    if (Mname.LastRight(1) == "а" && Gender == "женский") return Mname.PositionReplace("ой", Mname.Length - 1);
                    throw new NotSupportedException("Склонение отчества не поддерживается");
                case LingvoNET.Case.Locative:
                    if (Mname.LastRight(1) == "ч" && Gender == "мужской") return Mname + "е";
                    if (Mname.LastRight(1) == "а" && Gender == "женский") return Mname.PositionReplace("е", Mname.Length - 1);
                    throw new NotSupportedException("Склонение отчества в запрошенном падеже не поддерживается");
                default:
                    throw new NotSupportedException($"Не поддерживаемый формат склонения");
            }
        }
        protected string NameDeclinate(LingvoNET.Case @case)
        {
            if (Gender == "мужской")
            {
                    switch (@case)
                    {
                        case LingvoNET.Case.Nominative:
                            return Fname;
                        case LingvoNET.Case.Genitive:
                            return Word.NounToGenetive(Fname);
                        case LingvoNET.Case.Dative:
                            return Word.NounToDative(Fname);
                        case LingvoNET.Case.Accusative:
                        case LingvoNET.Case.Instrumental:
                        case LingvoNET.Case.Locative:
                            throw new NotSupportedException("Склонение отчества в запрошенном падеже не поддерживается");
                        default:
                            throw new NotSupportedException($"Не поддерживаемый формат склонения");
                    }
            }
            if (Gender == "женский")
            {
                if ("цкнгшщзхфвпрлджчсмтб".Contains(Fname.LastRight(1))) return Fname;
                else
                {
                    switch (@case)
                    {
                        case LingvoNET.Case.Nominative:
                            return Fname;
                        case LingvoNET.Case.Genitive:
                            return Word.NounToGenetive(Fname);
                        case LingvoNET.Case.Dative:
                            return Word.NounToDative(Fname);
                        case LingvoNET.Case.Accusative:
                        case LingvoNET.Case.Instrumental:
                        case LingvoNET.Case.Locative:
                            throw new NotSupportedException("Склонение отчества в запрошенном падеже не поддерживается");
                        default:
                            throw new NotSupportedException($"Не поддерживаемый формат склонения");
                    }
                }
            }
            else throw new NotImplementedException("Пол неизвестен.Склонение имени невозможно");
        }
        protected string NameToGenitive()
        {
            if (Gender == "мужской")
            {
                if (String.Compare(Fname, "Павел", true) == 0) return "Павла";
                if (String.Compare(Fname, "Лев", true) == 0) return "Льва";
                string rs;
                try
                {
                    rs = Word.NounToGenetive(Fname);
                }
                catch (NotImplementedException ex)
                {
                    rs = Fname;
                }
                return rs;
            }
            if (Gender == "женский")
            {
                if ("цкнгшщзхфвпрлджчсмтб".Contains(Fname.LastRight(1))) return Fname;
                else
                {
                    string rs;
                    try
                    {
                        rs = Word.NounToGenetive(Fname);
                    }
                    catch (NotImplementedException ex)
                    {
                        rs = Fname;
                    }
                    return rs;
                }
            }
            else throw new NotImplementedException("Пол неизвестен.Склонение имени невозможно");
        }
        protected string NameToDative()
        {
            if (Gender == "мужской")
            {
                if (String.Compare(Fname, "Павел", true) == 0) return "Павлу";
                if (String.Compare(Fname, "Лев", true) == 0) return "Льву";
                string rs;
                try
                {
                    rs = Word.NounToDative(Fname);
                }
                catch (NotImplementedException ex)
                {
                    rs = Fname;
                }
                return rs;
            }
            if (Gender == "женский")
            {
                if ("цкнгшщзхфвпрлджчсмтб".Contains(Fname.LastRight(1))) return Fname;
                else
                {
                    string rs;
                    try
                    {
                        rs = Word.NounToDative(Fname);
                    }
                    catch (NotImplementedException ex)
                    {
                        rs = Fname;
                    }
                    return rs;
                }
            }
            else throw new NotImplementedException("Пол неизвестен.Склонение имени невозможно");
        }
        public override string ToString()
        {
            return ToString(null, null);
        }
        public string ToString(string format)
        {
            return ToString(format, new CultureInfo("ru-RU"));
        }
        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (String.IsNullOrEmpty(format)) format = "n";
            if (formatProvider == null) formatProvider = new CultureInfo("ru-RU");
            StringBuilder sb = new StringBuilder(35);
            switch (format)
            {
                case "n":
                    sb.Append(Sname); sb.Append(" "); sb.Append(Fname[0]);
                    sb.Append(".");sb.Append(Mname[0]);sb.Append(".");
                    return sb.ToString();
                case "N":
                    sb.Append(Sname); sb.Append(" "); sb.Append(Fname);
                    sb.Append(" "); sb.Append(Mname);
                    return sb.ToString();
                case "G"://genetive case, full
                    sb.Append(SurnameDeclinate(LingvoNET.Case.Genitive)); sb.Append(" "); sb.Append(NameToGenitive());
                    sb.Append(" "); sb.Append(MiddleNameDeclinate(LingvoNET.Case.Genitive));
                    return sb.ToString();
                case "g"://genetive case, short
                    sb.Append(SurnameDeclinate(LingvoNET.Case.Genitive)); sb.Append(" "); sb.Append(Fname[0]);
                    sb.Append("."); sb.Append(Mname[0]); sb.Append(".");
                    return sb.ToString();
                case "D":// dative case
                    sb.Append(SurnameDeclinate(LingvoNET.Case.Dative)); sb.Append(" "); sb.Append(NameToDative());
                    sb.Append(" "); sb.Append(MiddleNameDeclinate(LingvoNET.Case.Dative));
                    return sb.ToString();
                case "d":
                    sb.Append(SurnameDeclinate(LingvoNET.Case.Dative)); sb.Append(" "); sb.Append(Fname[0]);
                    sb.Append("."); sb.Append(Mname[0]); sb.Append(".");
                    return sb.ToString();
                default:
                    throw new FormatException("Неизвестный формат");
            }
        }
        public Person Clone()
        {
            return new Person
            {
                Fname = _fname,
                Sname = _sname,
                Mname = _mname,
                Declinated = _declinated,
                Mobilephone = _mobilephone,
                Workphone = _workphone,
                Gender = _gender,
                Email = _email,
                Adress = _adress.Clone(),
                Version = this.Version,
                UpdateDate = this.UpdateDate
            };
        }
        object ICloneable.Clone() => Clone();
        public override void SaveChanges(SqlConnection con)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
    public class Employee : Person, ICloneable
    {
        #region Fields
        private string _education1;
        private string _education2;
        private string _education3;
        private string _sciencedegree;
        private string _inneroffice;
        private Departament _departament;
        private string _employeeStaus;
        private DateTime? _birthdate;
        private DateTime? _hiredate;
        private PermissionProfile _profile;
        private string _password;
        private byte[] _foto;
        private int? _previd;

        #endregion
        #region Properties
        public int EmployeeID => PersonID;
        public int? PreviousID => _previd;
        public string Education1
        {
            get => _education1;
            set
            {
                _education1 = value;
                OnPropertyChanged("Education1");
            }
        }
        public string Education2
        {
            get => _education2;
            set
            {
                _education2 = value;
                OnPropertyChanged("Education2");
            }
        }
        public string Education3
        {
            get => _education3;
            set
            {
                _education3 = value;
                OnPropertyChanged("Education3");
            }
        }
        public string Sciencedegree
        {
            get => _sciencedegree;
            set
            {
                _sciencedegree = value;
                OnPropertyChanged("ScienceDegree");
            }
        }
        public string Inneroffice
        {
            get => _inneroffice;
            set
            {
                if (_inneroffice == value) return;
                _inneroffice = value;
                OnPropertyChanged("InnerOffice");
            }
        }
        public Departament Departament
        {
            get => _departament;
            set
            {
                if (_departament == value) return;
                _departament = value;
                OnPropertyChanged("Departament");
            }
        }
        public string EmployeeStatus
        {
            get => _employeeStaus;
            set
            {
                if (_employeeStaus == value) return;
                _employeeStaus = value;
                OnPropertyChanged("EmployeeStatus");
            }
        }
        public DateTime? Birthdate
        {
            get => _birthdate;
            set
            {
                _birthdate = value;
                OnPropertyChanged("BirthDate");
            }
        }
        public DateTime? Hiredate
        {
            get => _hiredate;
            set
            {
                if (value != _hiredate)
                {
                    if (_birthdate != null && _hiredate != null && _birthdate.Value >= _hiredate.Value) throw new ArgumentException("Дата найма сотрудника не может быть ранее даты рождения");
                    _hiredate = value;
                    OnPropertyChanged("HireDate");
                }
            }
        }
        public PermissionProfile Profile
        {
            get => _profile;
            set
            {
                if (value != _profile)
                {
                    _profile = value;
                    OnPropertyChanged();
                }
            }
        }
        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged("PassWord");
            }
        }
        public byte[] Foto
        {
            get => _foto;
            set
            {
                _foto = value;
                OnPropertyChanged("Foto");
                OnPropertyChanged("Image");
            }
        }
        public BitmapImage Image
        {
            get
            {
                BitmapImage image = new BitmapImage();
                if (_foto != null)
                {
                    image.BeginInit();
                    image.StreamSource = new MemoryStream(_foto);
                    image.EndInit();
                }
                else
                {
                    switch (Gender)
                    {
                        case "женский":
                            image.BeginInit();
                            image.UriSource = new Uri(@"pack://application:,,,/Resources/UnknownFemale.jpg");
                            image.EndInit();
                            break;
                        case "мужской":
                            image.BeginInit();
                            image.UriSource = new Uri(@"pack://application:,,,/Resources/UnknownMale.jpg");
                            image.EndInit();
                            break;
                        default:
                            image.BeginInit();
                            image.UriSource = new Uri(@"pack://application:,,,/Resources/Unknown.jpg");
                            image.EndInit();
                            break;
                    }
                }
                return image;
            }
        }
        public int FullAge => (int)Age();
        public new bool IsInstanceValidState => base.IsInstanceValidState && _inneroffice != null && _departament != null && _employeeStaus != null;
        [Obsolete]
        public string Summary => DisplayInfo();
        #endregion
        public Employee() : base() { }
        public Employee(string firstname, string middlename, string secondname, string mobilephone, string workphone, string gender, string email, Adress adress, bool declinated, Version vr, DateTime updatedate,
                        int id, string education1, string education2, string education3, string sciencedegree, string inneroffice, Departament departament, string condition,
                        DateTime? birthdate, DateTime? hiredate, PermissionProfile profile, string password, byte[] foto, int? previd)
            : base(id, firstname, middlename, secondname, mobilephone, workphone, gender, email, adress, declinated, vr, updatedate)
        {
            _education1 = education1;
            _education2 = education2;
            _education3 = education3;
            _sciencedegree = sciencedegree;
            _inneroffice = inneroffice;
            _departament = departament;
            _employeeStaus = condition;
            _birthdate = birthdate;
            _hiredate = hiredate;
            _profile = profile;
            _password = password;
            _foto = foto;
            _previd = previd;
        }

#region Methods
        public override string ToString()
        {
            return base.ToString("n");
        }
        public double Age()
        {
            if (this.Birthdate == null) throw new InvalidOperationException("BirthDate is null");
            TimeSpan diff = DateTime.Today - Birthdate.Value.Date;
            return diff.Days / 365.25;
        }
        public string DisplayInfo()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(ToString());
            sb.AppendLine("ID:  " + EmployeeID.ToString());
            sb.AppendLine("Birthdate:  " + Birthdate);
            sb.AppendLine("Hiredate:  " + Hiredate);
            sb.AppendLine("Gender:  " + Gender);
            sb.AppendLine("Declinated:  " + Declinated);
            sb.AppendLine("WorkPhone:  " + Workphone);
            sb.AppendLine("Mobile:  " + Mobilephone); sb.AppendLine("Email:" + Email);
            sb.AppendLine("InnerOffice:  " + Inneroffice);
            sb.AppendLine("Dep:  " + Departament.Acronym);
            sb.AppendLine("Profile: " + Profile.ToString());
            sb.AppendLine("Status:  " + EmployeeStatus);
            sb.AppendLine("Adress:  " + Adress?.ToString());
            sb.AppendLine(Version + "\t" + UpdateDate);
            return sb.ToString();
        }
        public bool IsBirthDate() => DateTime.Today.Day == Birthdate?.Day && DateTime.Today.Month == Birthdate?.Month;
        public bool IsOperate()
        {
            return EmployeeStatus != "не работает" && (Inneroffice == "начальник" || Inneroffice == "заместитель начальника" ||
                Inneroffice == "государственный судебный эксперт" || Inneroffice == "старший государственный судебный эксперт"
                || Inneroffice == "ведущий государственный судебный эксперт" || Inneroffice == "начальник отдела");
        }
        private void AddToDB(SqlConnection con)
        {
            SqlCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "InnResources.prAddEmployee";
            cmd.Parameters.Add("@FN", SqlDbType.NVarChar, 25).Value = Fname;
            cmd.Parameters.Add("@SN", SqlDbType.NVarChar, 25).Value = Sname;
            cmd.Parameters.Add("@MN", SqlDbType.NVarChar, 25).Value = Mname;
            cmd.Parameters.Add("@Declinated", SqlDbType.Bit).Value = Declinated;
            cmd.Parameters.Add("@WPhone", SqlDbType.VarChar, 20).Value = ConvertToDBNull(_workphone);
            cmd.Parameters.Add("@Birth", SqlDbType.Date).Value = ConvertToDBNull(Birthdate);
            cmd.Parameters.Add("@Hire", SqlDbType.Date).Value = ConvertToDBNull(Hiredate);
            cmd.Parameters.Add("@Educ_1", SqlDbType.NVarChar, 150).Value = ConvertToDBNull(Education1);
            cmd.Parameters.Add("@Educ_2", SqlDbType.NVarChar, 150).Value = ConvertToDBNull(Education2);
            cmd.Parameters.Add("@Educ_3", SqlDbType.NVarChar, 150).Value = ConvertToDBNull(Education3);
            cmd.Parameters.Add("@Science", SqlDbType.NVarChar, 250).Value = ConvertToDBNull(Sciencedegree);
            cmd.Parameters.Add("@EmployeeStatusID", SqlDbType.NVarChar, 30).Value = EmployeeStatus;
            cmd.Parameters.Add("@foto", SqlDbType.VarBinary).Value = ConvertToDBNull(_foto); // check it
            cmd.Parameters.Add("@Departament", SqlDbType.TinyInt).Value = ConvertToDBNull(Departament?.DepartamentID);
            cmd.Parameters.Add("@InnerOffice", SqlDbType.NVarChar, 60).Value = Inneroffice;
            cmd.Parameters.Add("@SettlementID", SqlDbType.Int).Value = ConvertToDBNull(Adress.Settlement?.SettlementID);
            cmd.Parameters.Add("@StreetPrefix", SqlDbType.NVarChar, 20).Value = ConvertToDBNull(Adress.Streetprefix);
            cmd.Parameters.Add("@Street", SqlDbType.NVarChar, 40).Value = ConvertToDBNull(Adress.Street);
            cmd.Parameters.Add("@Housing", SqlDbType.NVarChar, 8).Value = ConvertToDBNull(Adress.Housing);
            cmd.Parameters.Add("@Flat", SqlDbType.NVarChar, 8).Value = ConvertToDBNull(Adress.Flat);
            cmd.Parameters.Add("@Corpus", SqlDbType.NVarChar, 12).Value = ConvertToDBNull(Adress.Corpus);
            cmd.Parameters.Add("@Structure", SqlDbType.NVarChar, 12).Value = ConvertToDBNull(Adress.Structure);
            cmd.Parameters.Add("@Gend", SqlDbType.NVarChar, 15).Value = Gender;
            cmd.Parameters.Add("@Mphone", SqlDbType.VarChar, 20).Value = ConvertToDBNull(_mobilephone);
            cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 50).Value = ConvertToDBNull(_email);
            cmd.Parameters.Add("@Profile", SqlDbType.TinyInt).Value = (byte)Profile;
            var par = cmd.Parameters.Add("@InsertedID", SqlDbType.Int);
            par.Direction = ParameterDirection.Output;
            try
            {
                cmd.Connection.Open();
                cmd.ExecuteNonQuery();
                _id = (int)cmd.Parameters["@InsertedID"].Value;
                Version = Version.Original;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                cmd.Connection.Close();
            }
        }
        private void EditToDB(SqlConnection con)
        {
            SqlCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "InnResources.prEditEmployee";
            cmd.Parameters.Add("@FN", SqlDbType.NVarChar, 25).Value = Fname;
            cmd.Parameters.Add("@SN", SqlDbType.NVarChar, 25).Value = Sname;
            cmd.Parameters.Add("@MN", SqlDbType.NVarChar, 25).Value = Mname;
            cmd.Parameters.Add("@Declinated", SqlDbType.Bit).Value = Declinated;
            cmd.Parameters.Add("@WPhone", SqlDbType.VarChar, 20).Value = ConvertToDBNull(_workphone);
            cmd.Parameters.Add("@Birth", SqlDbType.Date).Value = ConvertToDBNull(Birthdate);
            cmd.Parameters.Add("@Hire", SqlDbType.Date).Value = ConvertToDBNull(Hiredate);
            cmd.Parameters.Add("@Educ_1", SqlDbType.NVarChar, 150).Value = ConvertToDBNull(Education1);
            cmd.Parameters.Add("@Educ_2", SqlDbType.NVarChar, 150).Value = ConvertToDBNull(Education2);
            cmd.Parameters.Add("@Educ_3", SqlDbType.NVarChar, 150).Value = ConvertToDBNull(Education3);
            cmd.Parameters.Add("@Science", SqlDbType.NVarChar, 250).Value = ConvertToDBNull(Sciencedegree);
            cmd.Parameters.Add("@EmployeeStatusID", SqlDbType.NVarChar, 30).Value = EmployeeStatus;
            cmd.Parameters.Add("@foto", SqlDbType.VarBinary).Value = ConvertToDBNull(_foto);
            cmd.Parameters.Add("@Departament", SqlDbType.TinyInt).Value = ConvertToDBNull(Departament?.DepartamentID);
            cmd.Parameters.Add("@InnerOffice", SqlDbType.NVarChar, 60).Value = Inneroffice;
            cmd.Parameters.Add("@SettlementID", SqlDbType.Int).Value = ConvertToDBNull(Adress.Settlement?.SettlementID);
            cmd.Parameters.Add("@StreetPrefix", SqlDbType.NVarChar, 20).Value = ConvertToDBNull(Adress.Streetprefix);
            cmd.Parameters.Add("@Street", SqlDbType.NVarChar, 40).Value = ConvertToDBNull(Adress.Street);
            cmd.Parameters.Add("@Housing", SqlDbType.NVarChar, 8).Value = ConvertToDBNull(Adress.Housing);
            cmd.Parameters.Add("@Flat", SqlDbType.NVarChar, 8).Value = ConvertToDBNull(Adress.Flat);
            cmd.Parameters.Add("@Corpus", SqlDbType.NVarChar, 12).Value = ConvertToDBNull(Adress.Corpus);
            cmd.Parameters.Add("@Structure", SqlDbType.NVarChar, 12).Value = ConvertToDBNull(Adress.Structure);
            cmd.Parameters.Add("@Gend", SqlDbType.NVarChar, 15).Value = Gender;
            cmd.Parameters.Add("@Mphone", SqlDbType.VarChar, 20).Value = ConvertToDBNull(_mobilephone);
            cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 50).Value = ConvertToDBNull(_email);
            cmd.Parameters.Add("@Profile", SqlDbType.TinyInt).Value = (byte)Profile;
            cmd.Parameters.Add("@EmployeeID", SqlDbType.Int).Value = EmployeeID;
            var par = cmd.Parameters.Add("@NewID", SqlDbType.Int);
            par.Direction = ParameterDirection.Output;
            try
            {
                cmd.Connection.Open();
                cmd.ExecuteNonQuery();
                if (cmd.Parameters["@NewID"].Value != DBNull.Value) _id = (int)cmd.Parameters["NewID"].Value;
                Version = Version.Original;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                cmd.Connection.Close();
            }
        }
        public void DeleteFromDB(SqlConnection con)
        {
            SqlCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "InnResources.prDeleteEmployee";
            cmd.Parameters.Add("@id", SqlDbType.Int).Value = EmployeeID;
            try
            {
                cmd.Connection.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                cmd.Connection.Close();
            }
        }
        public override void SaveChanges(SqlConnection con)
        {
            Adress.Settlement?.SaveChanges(con);
            switch (Version)
            {
                case Version.New:
                    AddToDB(con);
                    break;
                case Version.Edited:
                    EditToDB(con);
                    break;
            }
        }
        public new Employee Clone()
        {
            return new Employee(_fname, _mname, _sname, _mobilephone, _workphone, Gender, _email, Adress.Clone(), _declinated, this.Version, this.UpdateDate, EmployeeID,
                                _education1, _education2, _education3, _sciencedegree, _inneroffice, new Departament(_departament), _employeeStaus, _birthdate,
                                _hiredate, _profile, _password, (byte[])Foto?.Clone(), _previd);
        }
        object ICloneable.Clone() => Clone();
        public void Copy (Employee em)
        {
            Fname = em._fname;
            Declinated = em._declinated;
            Mname = em._mname;
            Sname = em._sname;
            Mobilephone = em._mobilephone;
            Workphone = em._workphone;
            Gender = em._gender;
            Email = em._email;
            Adress = em._adress;
            Education1 = em._education1;
            Education2 = em._education2;
            Education3 = em._education3;
            Sciencedegree = em._sciencedegree;
            Inneroffice = em._inneroffice;
            Departament = em._departament;
            EmployeeStatus = em._employeeStaus;
            Birthdate = em._birthdate;
            Hiredate = em._hiredate;
            Profile = em._profile;
            Password = em._password;
            Foto = em._foto;
            _previd = em._previd;
            _id = em._id;
        }
#endregion
    }
    public class Expert : NotifyBase, ICloneable
    {
        #region Fields
        private Employee _employee;
        private Speciality _speciality;
        private DateTime _receiptdate;
        private DateTime? _lastattestationdate;
        private bool _closed;
        private int _id;
        #endregion
        #region Properties
        public Employee Employee
        {
            get => _employee;
            set
            {
                if (_employee == value) return;
                _employee = value;
                OnPropertyChanged("Employee");
            }
        }
        public int ExpertID => _id;
        public Speciality Speciality
        {
            get => _speciality;
            set
            {
                if (_speciality == value) return;
                _speciality = value;
                OnPropertyChanged("Speciality");
            }
        }
        public DateTime ReceiptDate
        {
            get => _receiptdate;
            set
            {
                if (_receiptdate == value) return;
                _receiptdate = value;
                OnPropertyChanged("ReceiptDate");
            }
        }
        public DateTime? LastAttestationDate
        {
            get => _lastattestationdate;
            set
            {
                if (_lastattestationdate == value) return;
                _lastattestationdate = value;
                OnPropertyChanged("LastAttestation");
            }
        }
        public bool Closed
        {
            get { return _closed; }
            set 
            { 
                if (value != _closed)
                {
                    _closed = value;
                    OnPropertyChanged();
                }
            }
        }
        public bool IsInstanceValidState => _employee != null && _speciality != null;
        public int Experience => DateTime.Now.Year - ReceiptDate.Year;
        public bool ValidAttestation => (DateTime.Now - (LastAttestationDate ?? ReceiptDate)).Days /365.25 <= 5.0;
        #endregion
        public Expert() : base() { }
        public Expert(int id, Employee employee, Speciality speciality, DateTime receiptdate, DateTime? lastattestationdate, Version vr, DateTime updatedate, bool closed = false)
            : base(vr, updatedate)
        {
            _id = id;
            _employee = employee;
            _closed = closed;
            _speciality = speciality;
            _receiptdate = receiptdate;
            _lastattestationdate = lastattestationdate;
        }

        public string SpecialityExperience() => ReceiptDate.Year.ToString();
        public string Requisite()
        {
            return $"квалификацию судебного эксперта по специальности {Speciality.Code}, стаж экспертной работы по которой с {ReceiptDate.Year} года";
        }
        private void AddToDB(SqlConnection con)
        {
            SqlCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "InnResources.prAddExpert";
            cmd.Parameters.Add("@EmployeeID", SqlDbType.Int).Value = Employee.EmployeeID;
            cmd.Parameters.Add("@SpecialityID", SqlDbType.SmallInt).Value = Speciality.SpecialityID;
            cmd.Parameters.Add("@Experience", SqlDbType.Date).Value = ReceiptDate;
            cmd.Parameters.Add("@LastAtt", SqlDbType.Date).Value = ConvertToDBNull(LastAttestationDate);
            cmd.Parameters.Add("@Closed", SqlDbType.Bit).Value = Closed;
            var par = cmd.Parameters.Add("@InsertedID", SqlDbType.Int);
            par.Direction = ParameterDirection.Output;
            try
            {
                cmd.Connection.Open();
                cmd.ExecuteNonQuery();
                _id = (int)cmd.Parameters["@InsertedID"].Value;
                Version = Version.Original;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                cmd.Connection.Close();
            }
        }
        private void EditToDB(SqlConnection con)
        {
            SqlCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "InnResources.prEditExpert";
            cmd.Parameters.Add("@EmployeeID", SqlDbType.Int).Value = Employee.EmployeeID;
            cmd.Parameters.Add("@SpecialityID", SqlDbType.SmallInt).Value = Speciality.SpecialityID;
            cmd.Parameters.Add("@Experience", SqlDbType.Date).Value = ReceiptDate;
            cmd.Parameters.Add("@LastAtt", SqlDbType.Date).Value = ConvertToDBNull(LastAttestationDate);
            cmd.Parameters.Add("@Closed", SqlDbType.Bit).Value = Closed;
            cmd.Parameters.Add("@ExpertID", SqlDbType.Int).Value = ExpertID;
            try
            {
                cmd.Connection.Open();
                cmd.ExecuteNonQuery();
                Version = Version.Original;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                cmd.Connection.Close();
            }
        }
        public void DeleteFromDB(SqlConnection con)
        {
            SqlCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "InnResources.prDeleteExpert";
            cmd.Parameters.Add("@id", SqlDbType.Int).Value = ExpertID;
            try
            {
                cmd.Connection.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                cmd.Connection.Close();
            }
        }
        /// <summary>
        /// Сохраняет изменения в базу данных
        /// </summary>
        /// <param name="con">SqlConnection. Строка подключения к базе данных</param>
        /// <exception cref="System.NullReferenceException">Поле <c>Employee</c> или <c>Speciality</c> равны null</exception>
        /// <exception cref="System.Data.SqlClient.SqlException"></exception>
        public override void SaveChanges(SqlConnection con)
        {
            Employee.SaveChanges(con);
            Speciality.SaveChanges(con);
            switch (Version)
            {
                case Version.New:
                    AddToDB(con);
                    break;
                case Version.Edited:
                    EditToDB(con);
                    break;
            }
        }
        public override string ToString()
        {
            return Employee.ToString() + Environment.NewLine + Speciality.Code;
        }
        public Expert Clone()
        {
            return new Expert(id: ExpertID,
                                employee: Employee.Clone(),
                                speciality: Speciality.Clone(),
                                receiptdate: ReceiptDate,
                                lastattestationdate: LastAttestationDate,
                                vr: this.Version,
                                updatedate: this.UpdateDate,
                                closed: Closed);
        }
        object ICloneable.Clone() => Clone();
    }
    public class Organization : NotifyBase, IFormattable, ICloneable
    {
        private class OrganizationEventArg
        {
            private string _propertyName;

            public string PropertyName
            {
                get => _propertyName;
                set => _propertyName = value;
            }

            public OrganizationEventArg(string prop)
            {
                _propertyName = prop;
            }
        }
        #region Fields
        private string _name;
        private string _shortname;
        private string _postcode;
        private Adress _adress = new Adress { Streetprefix = "ул." };
        private string _telephone;
        private string _telephone2;
        private string _fax;
        private string _email;
        private string _website;
        private bool _status;
        private int _id;
        #endregion
        #region Properties
        public bool IsValid
        {
            get => _status;
            set
            {
                if (value == _status) return;
                _status = value;
                OnPropertyChanged();
            }
        }
        public string WebSite
        {
            get => _website;
            set
            {
                if (value == _website) return;
                _website = value;
                OnPropertyChanged();
            }
        }
        public string Email
        {
            get => _email;
            set
            {
                if (value == _email) return;
                _email = value;
                OnPropertyChanged();
            }
        }
        public string Fax
        {
            get => _fax;
            set
            {
                if (value == _fax) return;
                _fax = value;
                OnPropertyChanged();
            }
        }
        public string Telephone2
        {
            get
            {
                StringBuilder sb = new StringBuilder(_telephone2);
                if (sb.Length > 4) sb.Insert(sb.Length - 2, "-").Insert(sb.Length - 5, "-");
                else
                {
                    if (sb.Length > 2) sb.Insert(sb.Length - 2, "-");
                }
                if (Adress?.Settlement?.Telephonecode != null && sb.Length > 0)
                {
                    return sb.Insert(0, "(").Insert(1, Adress?.Settlement?.Telephonecode + ") ").ToString();
                }
                return sb.ToString();
            }
            set
            {
                if (value == _telephone2) return;
                _telephone2 = StringUtil.OnlyDigits(value);
                OnPropertyChanged();
            }
        }
        public string Telephone
        {
            get
            {
                StringBuilder sb = new StringBuilder(_telephone);
                if (sb.Length > 4) sb.Insert(sb.Length - 2, "-").Insert(sb.Length - 4, "-");
                else
                {
                    if (sb.Length > 2) sb.Insert(sb.Length - 2, "-");
                }
                if (Adress?.Settlement?.Telephonecode != null && sb.Length > 0)
                {
                    return sb.Insert(0, "(").Insert(1, Adress?.Settlement?.Telephonecode + ") ").ToString();
                }
                return sb.ToString();
            }
            set
            {
                if (value == _telephone) return;
                _telephone = StringUtil.OnlyDigits(value);
                OnPropertyChanged();
            }
        }
        public Adress Adress
        {
            get => _adress;
            set
            {
                if (_adress == value) return;
                _adress = value;
                OnPropertyChanged();
            }
        }
        public string PostCode
        {
            get => _postcode;
            set
            {
                if (value == _postcode) return;
                if (String.IsNullOrWhiteSpace(value)) throw new ArgumentException("Поле <почтовый индекс> не может быть пустым");
                _postcode = value;
                OnPropertyChanged();
            }
        }
        public string ShortName
        {
            get => _shortname;
            set
            {
                if (value == _shortname) return;
                _shortname = value;
                OnPropertyChanged();
            }
        }
        public string Name
        {
            get => _name;
            set
            {
                if (value == _name) return;
                if (String.IsNullOrWhiteSpace(value)) throw new ArgumentException("Поле <Название> не может быть пустым");
                _name = value;
                OnPropertyChanged();
            }
        }
        public int OrganizationID => _id;
        public string Requisite => (ShortName ?? Name) + Environment.NewLine + Adress.ToString();
        public bool IsInstanceValidState => !String.IsNullOrWhiteSpace(_name) && !String.IsNullOrWhiteSpace(_postcode) && _adress.IsInstanceValidState;

        #endregion Properties

        public Organization() : base()
        {
           _adress.PropertyChanged += AdressChanged;
        }
        public Organization(int id, string name, string shortname, string postcode, Adress adress, string telephone, string telephone2, string fax,
                        string email, string website, bool status, Version vr, DateTime updatedate) : base(vr, updatedate)
        {
            _id = id;
            _name = name;
            _shortname = shortname;
            _postcode = postcode;
            _adress = adress ?? new Adress();
            _adress.PropertyChanged += AdressChanged;
            _telephone = telephone;
            _telephone2 = telephone2;
            _fax = fax;
            _email = email;
            _website = website;
            _status = status;
        }

        public override string ToString()
        {
            return Name;
        }
        private void AdressChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(e.PropertyName);
        }
        private void AddToDB(SqlConnection con)
        {
            SqlCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "OutResources.prAddOrganization";
            cmd.Parameters.Add("@Name", SqlDbType.NVarChar, 200).Value = Name;
            cmd.Parameters.Add("@ShortName", SqlDbType.NVarChar, 150).Value = ConvertToDBNull(ShortName);
            cmd.Parameters.Add("@Post", SqlDbType.Char, 6).Value = PostCode;
            cmd.Parameters.Add("@SettlementID", SqlDbType.Int).Value = Adress.Settlement.SettlementID;
            cmd.Parameters.Add("@StreetPrefix", SqlDbType.NVarChar, 10).Value = Adress.Streetprefix;
            cmd.Parameters.Add("@Street", SqlDbType.NVarChar, 40).Value = Adress.Street;
            cmd.Parameters.Add("@Housing", SqlDbType.NVarChar, 8).Value = Adress.Housing;
            cmd.Parameters.Add("@Office", SqlDbType.NVarChar, 12).Value = ConvertToDBNull(Adress.Flat);
            cmd.Parameters.Add("@Corpus", SqlDbType.NVarChar, 12).Value = ConvertToDBNull(Adress.Corpus);
            cmd.Parameters.Add("@Structure", SqlDbType.NVarChar, 12).Value = ConvertToDBNull(Adress.Structure);
            cmd.Parameters.Add("@Telephone", SqlDbType.VarChar, 20).Value = ConvertToDBNull(Telephone);
            cmd.Parameters.Add("@Telephone_2", SqlDbType.VarChar, 20).Value = ConvertToDBNull(Telephone2);
            cmd.Parameters.Add("@Fax", SqlDbType.VarChar, 20).Value = ConvertToDBNull(Fax);
            cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 50).Value = ConvertToDBNull(Email);
            cmd.Parameters.Add("@WebSite", SqlDbType.NVarChar, 50).Value = ConvertToDBNull(WebSite);
            var par = cmd.Parameters.Add("@InsertedID", SqlDbType.Int);
            par.Direction = ParameterDirection.Output;
            try
            {
                cmd.Connection.Open();
                cmd.ExecuteNonQuery();
                _id = (int)cmd.Parameters["@InsertedID"].Value;
                Version = Version.Original;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                cmd.Connection.Close();
            }
        }
        private void EditToDB(SqlConnection con)
        {
            SqlCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "OutResources.prEditOrganization";
            cmd.Parameters.Add("@Name", SqlDbType.NVarChar, 200).Value = Name;
            cmd.Parameters.Add("@ShortName", SqlDbType.NVarChar, 150).Value = ConvertToDBNull(ShortName);
            cmd.Parameters.Add("@Post", SqlDbType.Char, 6).Value = PostCode;
            cmd.Parameters.Add("@SettlementID", SqlDbType.Int).Value = Adress.Settlement.SettlementID;
            cmd.Parameters.Add("@StreetPrefix", SqlDbType.NVarChar, 10).Value = Adress.Streetprefix;
            cmd.Parameters.Add("@Street", SqlDbType.NVarChar, 40).Value = Adress.Street;
            cmd.Parameters.Add("@Housing", SqlDbType.NVarChar, 8).Value = Adress.Housing;
            cmd.Parameters.Add("@Office", SqlDbType.NVarChar, 12).Value = ConvertToDBNull(Adress.Flat);
            cmd.Parameters.Add("@Corpus", SqlDbType.NVarChar, 12).Value = ConvertToDBNull(Adress.Corpus);
            cmd.Parameters.Add("@Structure", SqlDbType.NVarChar, 12).Value = ConvertToDBNull(Adress.Structure);
            cmd.Parameters.Add("@Telephone", SqlDbType.VarChar, 20).Value = ConvertToDBNull(Telephone);
            cmd.Parameters.Add("@Telephone_2", SqlDbType.VarChar, 20).Value = ConvertToDBNull(Telephone2);
            cmd.Parameters.Add("@Fax", SqlDbType.VarChar, 20).Value = ConvertToDBNull(Fax);
            cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 50).Value = ConvertToDBNull(Email);
            cmd.Parameters.Add("@WebSite", SqlDbType.NVarChar, 50).Value = ConvertToDBNull(WebSite);
            cmd.Parameters.Add("@StatusID", SqlDbType.NVarChar, 30).Value = _status;
            cmd.Parameters.Add("@OrganizationID", SqlDbType.Int).Value = OrganizationID;
            try
            {
                cmd.Connection.Open();
                cmd.ExecuteNonQuery();
                Version = Version.Original;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                cmd.Connection.Close();
            }
        }
        public void DeleteFromDB(SqlConnection con)
        {
            SqlCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "OutResources.prDeleteOrganization";
            cmd.Parameters.Add("@id", SqlDbType.Int).Value = OrganizationID;
            try
            {
                cmd.Connection.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                cmd.Connection.Close();
            }
        }
        public override void SaveChanges(SqlConnection con)
        {
            Adress.Settlement?.SaveChanges(con);
            switch (Version)
            {
                case Version.New:
                    AddToDB(con);
                    break;
                case Version.Edited:
                    EditToDB(con);
                    break;
            }
        }
        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (String.IsNullOrEmpty(format)) format = "n";
            if (formatProvider == null) formatProvider = CultureInfo.CurrentCulture;
            switch (format)
            {
                case "N":
                case "n":
                    return Name;

                case "S":
                case "s"://ShortName only
                    return ShortName;

                case "ns":
                    return Name + " (" + ShortName + ")";

                case "nsa": //nominative case
                    return null;

                case "g":
                    return null;

                default:
                    throw new FormatException("Неизвестный формат");
            }
        }
        public Organization Clone()
        {
            return new Organization(OrganizationID, _name, _shortname, _postcode, _adress, _telephone, _telephone2, _fax, _email, _website, _status,
                                        this.Version, this.UpdateDate);
        }
        object ICloneable.Clone() => Clone();
        public void Copy(Organization o)
        {
            Name = o._name;
            ShortName = o._shortname;
            PostCode = o._postcode;
            Adress = o._adress;
            Telephone = o._telephone;
            Telephone2 = o._telephone2;
            Fax = o._fax;
            Email = o._email;
            WebSite = o._website;
            IsValid = o._status;
            _id = o._id;
        }
    }
    public class Laboratory : Organization
    {

    }
    public class Customer : Person, ICloneable
    {
        private string _rank;
        private string _office;
        private Organization _organization;
        private bool _status;
        private string _departament;
        private int? _previd;

        public int? PreviousID => _previd;
        public string Departament
        {
            get => _departament;
            set
            {
                if (value == _departament) return;
                _departament = value;
                OnPropertyChanged();
            }
        }
        public bool IsValid
        {
            get => _status;
            set
            {
                if (value == _status) return;
                _status = value;
                OnPropertyChanged();
            }
        }
        public Organization Organization
        {
            get => _organization;
            set
            {
                _organization = value;
                OnPropertyChanged();
            }
        }
        public string Office
        {
            get => _office;
            set
            {
                if (value == _office) return;
                _office = value;
                OnPropertyChanged();
            }
        }
        public string Rank
        {
            get => _rank;
            set
            {
                if (value == _rank) return;
                _rank = value;
                OnPropertyChanged();
            }
        }
        public int CustomerID => _id;
        public string Requisite => ToString();
        public new bool IsInstanceValidState => !String.IsNullOrEmpty(Office); // check base valid state

        public Customer() : base() {}
        public Customer(string firstname, string middlename, string secondname, string mobilephone, string workphone, string gender, string email, bool declinated, Version vr, DateTime updatedate,
                        int id, int? previd, string rank, string office, Organization organization, string departament, bool status)
            : base(id, firstname, middlename, secondname, mobilephone, workphone, gender, email, null, declinated, vr, updatedate)
        {
            _rank = rank;
            _office = office;
            _organization = organization;
            _departament = departament;
            _status = status;
            _previd = previd;
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (Office != null)
            {
                stringBuilder.Append(Office);
            }
            if (Rank != null)
            {
                if (stringBuilder.Length > 0) stringBuilder.Append(", ");
                stringBuilder.Append(Rank);
            }
            stringBuilder.Append(" ");
            stringBuilder.Append(base.ToString("n"));
            if (Organization != null)
            {
                stringBuilder.AppendLine();
                stringBuilder.Append(Organization.ToString());
            }       
            return stringBuilder.ToString();
        }
        private void AddToDB(SqlConnection con)
        {
            SqlCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "OutResources.prAddCustomer";
            cmd.Parameters.Add("@FN", SqlDbType.NVarChar, 25).Value = Fname;
            cmd.Parameters.Add("@SN", SqlDbType.NVarChar, 25).Value = Sname;
            cmd.Parameters.Add("@MN", SqlDbType.NVarChar, 25).Value = Mname;
            cmd.Parameters.Add("@Declinated", SqlDbType.Bit).Value = Declinated;
            cmd.Parameters.Add("@Gend", SqlDbType.NVarChar, 15).Value = Gender;
            cmd.Parameters.Add("@WorkPhone", SqlDbType.VarChar, 20).Value = ConvertToDBNull(Workphone);
            cmd.Parameters.Add("@MobilePhone", SqlDbType.VarChar, 20).Value = ConvertToDBNull(Mobilephone);
            cmd.Parameters.Add("@OrgID", SqlDbType.Int).Value = ConvertToDBNull(Organization?.OrganizationID);
            cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 50).Value = ConvertToDBNull(Email);
            cmd.Parameters.Add("@Rank", SqlDbType.NVarChar, 100).Value = ConvertToDBNull(Rank);
            cmd.Parameters.Add("@Departament", SqlDbType.NVarChar, 100).Value = ConvertToDBNull(Departament);
            cmd.Parameters.Add("@Office", SqlDbType.NVarChar, 150).Value = Office; 
            var par = cmd.Parameters.Add("@InsertedID", SqlDbType.Int);
            par.Direction = ParameterDirection.Output;
            try
            {
                cmd.Connection.Open();
                cmd.ExecuteNonQuery();
                _id = (int)cmd.Parameters["@InsertedID"].Value;
                Version = Version.Original;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                cmd.Connection.Close();
            }
        }
        private void EditToDB(SqlConnection con)
        {
            SqlCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "OutResources.prEditCustomer";
            cmd.Parameters.Add("@FN", SqlDbType.NVarChar, 25).Value = Fname;
            cmd.Parameters.Add("@SN", SqlDbType.NVarChar, 25).Value = Sname;
            cmd.Parameters.Add("@MN", SqlDbType.NVarChar, 25).Value = Mname;
            cmd.Parameters.Add("@Declinated", SqlDbType.Bit).Value = Declinated;
            cmd.Parameters.Add("@Gend", SqlDbType.NVarChar, 15).Value = Gender;
            cmd.Parameters.Add("@WorkPhone", SqlDbType.VarChar, 20).Value = ConvertToDBNull(Workphone);
            cmd.Parameters.Add("@MobilePhone", SqlDbType.VarChar, 20).Value = ConvertToDBNull(Mobilephone);
            cmd.Parameters.Add("@OrgID", SqlDbType.Int).Value = ConvertToDBNull(Organization?.OrganizationID);
            cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 50).Value = ConvertToDBNull(Email);
            cmd.Parameters.Add("@Rank", SqlDbType.NVarChar, 100).Value = ConvertToDBNull(Rank);
            cmd.Parameters.Add("@Departament", SqlDbType.NVarChar, 100).Value = ConvertToDBNull(Departament);
            cmd.Parameters.Add("@Office", SqlDbType.NVarChar, 100).Value = Office;
            cmd.Parameters.Add("@Status", SqlDbType.Bit).Value = _status;
            cmd.Parameters.Add("@CusIden", SqlDbType.Int).Value = CustomerID;
            var par = cmd.Parameters.Add("@NewID", SqlDbType.Int);
            par.Direction = ParameterDirection.Output;
            try
            {
                cmd.Connection.Open();
                cmd.ExecuteNonQuery();
                if (cmd.Parameters["@NewID"].Value != DBNull.Value) _id = (int)cmd.Parameters["NewID"].Value;
                Version = Version.Original;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                cmd.Connection.Close();
            }
        }
        public void DeleteFromDB(SqlConnection con)
        {
            SqlCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "OurResources.prDeleteCustomer";
            cmd.Parameters.Add("@id", SqlDbType.Int).Value = CustomerID;
            try
            {
                cmd.Connection.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                cmd.Connection.Close();
            }
        }
        public override void SaveChanges(SqlConnection con)
        {
            Organization?.SaveChanges(con);
            switch (Version)
            {
                case Version.New:
                    AddToDB(con);
                    break;
                case Version.Edited:
                    EditToDB(con);
                    break;
            }
        }
        public new Customer Clone()
        {
            return new Customer(firstname: Fname,
                                  middlename: Mname,
                                  secondname: Sname,
                                  mobilephone: Mobilephone,
                                  workphone: Workphone,
                                  gender: Gender,
                                  email: Email,
                                  declinated: Declinated,
                                  vr: Version,
                                  updatedate: UpdateDate,
                                  id: CustomerID,
                                  rank: _rank,
                                  office: _office,
                                  organization: _organization.Clone(),
                                  departament: _departament,
                                  status: _status,
                                  previd: _previd);
        }
        object ICloneable.Clone() => Clone();
    }
    public sealed class Case : INotifyPropertyChanged
    {
        private string _number;
        private string _respondent;
        private string _plaintiff;
        private string _typecase;
        private string _annotate;
        private string _comment;
        private DateTime? _dispatchdate;

        public DateTime? DispatchDate
        {
            get => _dispatchdate;
            set
            {
                if (value != _dispatchdate)
                {
                    _dispatchdate = value;
                    OnPropertyChanged();
                }
            }
        }
        public string Comment
        {
            get => _comment;
            set
            {
                if (value != _comment)
                {
                    _comment = value;
                    OnPropertyChanged();
                }
            }
        }
        public string Annotate
        {
            get => _annotate;
            set
            {
                if (value != _annotate)
                {
                    _annotate = value;
                    OnPropertyChanged();
                }
            }
        }
        public string TypeCase
        {
            get => _typecase;
            set
            {
                if (value != _typecase)
                {
                    _typecase = value;
                    OnPropertyChanged();
                }
            }
        }
        public string Plaintiff
        {
            get
            {
                if (TypeCase == "проверка КУCП" && TypeCase == "уголовное" && TypeCase == "административное правонарушение") return null;
                else return _plaintiff;
            }
            set
            {
                if (value != _plaintiff)
                {
                    _plaintiff = value;
                    OnPropertyChanged();
                }
            }
        }
        public string Respondent
        {
            get
            {
                if (TypeCase == "проверка КУCП" && TypeCase == "уголовное" && TypeCase == "административное правонарушение") return null;
                else return _respondent;
            }
            set
            {
                if (value != _respondent)
                {
                    _respondent = value;
                    OnPropertyChanged();
                }
            }
        }
        public string Number
        {
            get => _number;
            set
            {
                if (value != _number)
                {
                    _number = value;
                    OnPropertyChanged();
                }
            }
        }
        public string Essense => AnnotateBuilder();

        public bool IsInstanceValidState => !String.IsNullOrWhiteSpace(_typecase);

        public Case(){}
        public Case(string number, string type, string respondent, string plaintiff, string annotate, string comment = null, DateTime? dispatchdate = null)
                    : base()
        {
            _number = number;
            _typecase = type;
            _respondent = respondent;
            _plaintiff = plaintiff;
            _annotate = annotate;
            _comment = comment;
            _dispatchdate = dispatchdate;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName]string prop = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Number: ");
            sb.AppendLine(Number);
            sb.Append("Type: ");
            sb.AppendLine(TypeCase);
            sb.Append("DispatchDate: ");
            sb.AppendLine(this.DispatchDate.ToString());
            sb.Append("Annotate: ");
            sb.AppendLine(Annotate);
            sb.Append("Comment: ");
            sb.AppendLine(Comment);
            sb.Append("Respondent: ");
            sb.AppendLine(Respondent);
            sb.Append("Plaintiff: ");
            sb.AppendLine(Plaintiff);
            return sb.ToString();
        }
        private string CaseTypeDeclination()
        {
            switch (_typecase)
            {
                case "уголовное":
                    return "уголовного";
                case "гражданское":
                    return "гражданского";
                case "арбитражное":
                    return "арбитражного";
                case "административное правонарушение":
                    return "административного правонарушения";
                case "проверка КУСП":
                    return "проверки КУСП";
                case "исследование":
                    return "исследования";
                case "административное судопроизводство":
                    return "административного судопроизводства";
                default:
                    return _typecase;
            }
        }
        public string AnnotateBuilder()
        {
            switch (_typecase)
            {
                case "уголовное":
                case "гражданское":
                case "арбитражное":
                    return $"по материалам {CaseTypeDeclination()} дела № {Number} {Annotate}";
                case "административное правонарушение":
                    return $"по материалам {CaseTypeDeclination()} {Annotate}";
                case "проверка КУСП":
                    return $"по материалам {CaseTypeDeclination()} № {Number} {Annotate}";
                case "исследование":
                    return $"{CaseTypeDeclination()} {Annotate}";
                case "административное судопроизводство":
                    return $"по материалам {CaseTypeDeclination()} № {Number} {Annotate}";
                default:
                    return null;
            }
        }
        public string Codex()
        {
            switch (_typecase)
            {
                case "уголовное":
                    return "57 УПК РФ";
                case "гражданское":
                    return "85 ГПК РФ";
                case "арбитражное":
                    return "55 АПК РФ";
                case "административное правонарушение":
                    return "25.9 КоАП РФ";
                case "проверка КУСП":
                    return "57 УПК РФ";
                case "административное судопроизводство":
                    return "49 КАС РФ";
                default:
                    return null;
            }
        }
        public string SubcribeArticle()
        {
            switch (_typecase)
            {
                case "уголовное":  
                case "гражданское":
                case "арбитражное":
                case "проверка КУСП":
                case "административное судопроизводство":
                    return "307 УК РФ";
                case "административное правонарушение":
                    return "17.9 КоАП РФ";
                default:
                    return null;
            }
        }
    }

    /// <summary>
    /// The main <c>Resolution</c> class. Contains all another linked entities, like expertise, customers, ect.
    /// </summary>
    public sealed class Resolution : NotifyBase
    {
#region Fields
        private DateTime _regdate;
        private DateTime? _resdate;
        private string _restype;
        private Customer _customer;
        private Case _case = new Case();
        private ObjectsList _objects = new ObjectsList();
        private string _prescribetype;
        private QuestionsList _quest = new QuestionsList();
        private string _status;
        private readonly ObservableCollection<Expertise> _expertisies = new ObservableCollection<Expertise>();
        private int _id;
        #endregion
       
#region Property
        public string ResolutionStatus
        {
            get => _status;
            set
            {
                if (value != _status)
                {
                    _status = value;
                    OnPropertyChanged();
                }
            }
        }
        public QuestionsList Questions
        {
            get => _quest;
            set
            {
                if (!Object.ReferenceEquals(value, _quest))
                {
                    _quest = value;
                    OnPropertyChanged();
                }
            }
        }
        /// <summary>
        /// Список предоставленных объектов
        /// </summary>
        public ObjectsList Objects
        {
            get => _objects;
            set
            {
                if (!Object.ReferenceEquals(value, _objects))
                {
                    _objects = value;
                    OnPropertyChanged();
                }
            }
        }
        public string PrescribeType
        {
            get => _prescribetype;
            set
            {
                if (value != _prescribetype)
                {
                    _prescribetype = value;
                    OnPropertyChanged();
                }
            }
        }
        public Case Case => _case;
        public Customer Customer
        {
            get => _customer;
            set
            {
                if (value != _customer)
                {
                    if (value == null) throw new ArgumentException("Поле <заказчик> не может быть пустым");
                    _customer = value;
                    OnPropertyChanged();
                }
            }
        }
        public string ResolutionType
        {
            get => _restype;
            set
            {
                if (value != _restype)
                {
                    if (String.IsNullOrWhiteSpace(value)) throw new ArgumentException("Поле не может быть пустым");
                    _restype = value;
                    if (value == "договор") this.Case.TypeCase = "исследование";
                    OnPropertyChanged();
                }
            }
        }
        public DateTime? ResolutionDate
        {
            get => _resdate;
            set
            {
                if (value != _resdate)
                {
                    _resdate = value;
                    OnPropertyChanged();
                }
            }
        }
        /// <summary>
        /// Дата регистрации постановления
        /// </summary>
        public DateTime RegistrationDate
        {
            get => _regdate;
            set
            {
                if (value != _regdate)
                {
                    if (value < _resdate) throw new ArgumentException("Неверная дата регистрации");
                    _regdate = value;
                    OnPropertyChanged();
                }
            }
        }
        public int ResolutionID => _id;
        public ObservableCollection<Expertise> Expertisies => _expertisies;
        public bool IsInstanceValidState => (Customer?.IsInstanceValidState ?? false) && !String.IsNullOrWhiteSpace(ResolutionType)
                                            && !String.IsNullOrWhiteSpace(ResolutionStatus) && Case.IsInstanceValidState; 
        public string QeustionsString
        {
            get
            {
                StringBuilder sb = new StringBuilder(200);
                foreach (var item in Questions.Questions)
                {
                    sb.AppendLine(item.Content);
                }
                return sb.ToString();
            }
        }
        public string OverallNumber
        {     
            get
            {
                StringBuilder sb = new StringBuilder(21);
                foreach (var item in _expertisies)
                {
                    sb.Append($", {item.Number}");
                }
                return sb.Length > 2 ? sb.Remove(0, 2).ToString() : null;
            }
        }
        public string FullOverallNumber
        {
            get
            {
                StringBuilder sb = new StringBuilder(40);
                foreach (var item in _expertisies)
                {
                    sb.Append($", {item.FullNumber}");
                }
                return sb.Length > 2 ? sb.Remove(0, 2).ToString() : null;
            }
        }
#endregion
        
        public Resolution() : base()
        {
            _expertisies.CollectionChanged += ExpertiseListChanged;
            ((INotifyPropertyChanged)_expertisies).PropertyChanged += ExpertiseStatusChanged;
            _case.PropertyChanged += (o, e) => OnPropertyChanged(e.PropertyName);
        }
        public Resolution(int id, DateTime registrationdate, DateTime? resolutiondate, string resolutiontype, Customer customer, ObjectsList obj, string prescribe, QuestionsList quest,
                            string status, Version vr, DateTime updatedate) : base(vr, updatedate)
        {
            _id = id;
            _regdate = registrationdate;
            _resdate = resolutiondate;
            _restype = resolutiontype;
            _customer = customer;
            _objects = obj;
            _prescribetype = prescribe;
            _quest = quest;
            _status = status;
            _expertisies.CollectionChanged += ExpertiseListChanged;
            ((INotifyPropertyChanged)_expertisies).PropertyChanged += ExpertiseStatusChanged;
            _case.PropertyChanged += (o, e) => OnPropertyChanged(e.PropertyName);
        }

        private void ExpertiseStatusChanged(object o, PropertyChangedEventArgs e)//CHECK!!!!
        {
            OnPropertyChanged("Expertisies", true);
            if (e.PropertyName == "ExpertiseStatus")
            {
                foreach (var item in _expertisies)
                {
                    if (item.EndDate == null) ResolutionStatus = "в работе";
                    return;
                }
                ResolutionStatus = "выполнено";
            }
        }
        private void ExpertiseListChanged(object o, NotifyCollectionChangedEventArgs e)  
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (Expertise item in e.NewItems)
                    {
                        item.FromResolution = this;
                        if (item.EndDate == null) ResolutionStatus = "в работе";
                    }
                    OnPropertyChanged("Expertisies", true);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    try
                    {
                        foreach (Expertise item in e.OldItems)
                        {
                            item.DeleteFromDB(CommonInfo.connection);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    if (_expertisies.Count > 0)
                    {
                        foreach (var item in _expertisies)
                        {
                            if (item.EndDate == null) ResolutionStatus = "в работе";
                            return;
                        }
                    }
                    else ResolutionStatus = "рассмотрение";
                    break;
                case NotifyCollectionChangedAction.Reset:   
                    try
                    {
                        foreach (Expertise item in e.OldItems)
                        {
                            item.DeleteFromDB(CommonInfo.connection);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    ResolutionStatus = "рассмотрение";
                    break;
            }
            
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("<Resolution> ID is ");
            sb.AppendLine(ResolutionID.ToString());
            sb.AppendLine($"Status: {ResolutionStatus}");
            sb.Append("Type: ");
            sb.AppendLine(ResolutionType);
            sb.Append("PrescribeType: ");
            sb.AppendLine(PrescribeType);
            sb.Append("RegDate: ");
            sb.AppendLine(RegistrationDate.ToString("d"));
            sb.Append("ResDate: ");
            sb.AppendLine(ResolutionDate?.ToString("d"));
            sb.Append("Customer: ");
            sb.AppendLine(Customer?.ToString());
            sb.AppendLine("----------------------------");
            sb.AppendLine("Questions: ");
            foreach (var item in Questions?.Questions)
            {
                sb.Append("\t");
                sb.AppendLine(item.Content);
            }
            sb.AppendLine("---------------------------");
            sb.AppendLine("Case: ");
            sb.AppendLine(Case?.ToString());
            sb.AppendLine("Expertisies: ");
            sb.Append(Expertisies.Count);
            return sb.ToString();
        }
        /// <summary>
        /// Сохраняет новое основание в базу данных
        /// </summary>
        /// <param name="con">SqlConnection. Строка подключения к базе данных</param>
        /// <returns>Int32. Новое значение ключа идентификации в базе данных</returns>
        /// <exception cref="System.NullReferenceException">Поле <c>Customer</c> или аргумент <c>con</c> равны null</exception>
        /// <exception cref="System.Data.SqlClient.SqlException"></exception>
        private void AddToDB(SqlConnection con)
        {
            SqlCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "Activity.prAddResolution";
            cmd.Parameters.Add("@RegDate", SqlDbType.Date).Value = RegistrationDate;
            cmd.Parameters.Add("@ResolDate", SqlDbType.Date).Value = ConvertToDBNull(ResolutionDate);
            cmd.Parameters.Add("@TypeResol", SqlDbType.NVarChar, 30).Value = ResolutionType;
            cmd.Parameters.Add("@Status", SqlDbType.NVarChar, 30).Value = _status;
            cmd.Parameters.Add("@CustID", SqlDbType.Int).Value = Customer.CustomerID;
            cmd.Parameters.Add("@TypeCase", SqlDbType.Char, 1).Value = CommonInfo.CaseTypes[Case.TypeCase];
            cmd.Parameters.Add("@Annotate", SqlDbType.NVarChar, 500).Value = ConvertToDBNull(Case.Annotate);
            cmd.Parameters.Add("@Comment", SqlDbType.NVarChar, 500).Value = ConvertToDBNull(Case.Comment);
            cmd.Parameters.Add("@NumberCase", SqlDbType.NVarChar, 50).Value = ConvertToDBNull(Case.Number);
            cmd.Parameters.Add("@Respondent", SqlDbType.NVarChar, 150).Value = ConvertToDBNull(Case.Respondent);
            cmd.Parameters.Add("@Plaintiff", SqlDbType.NVarChar, 150).Value = ConvertToDBNull(Case.Plaintiff);
            cmd.Parameters.Add("@DispatchDate", SqlDbType.Date).Value = ConvertToDBNull(Case.DispatchDate);
            cmd.Parameters.Add("@PrescribeType", SqlDbType.NVarChar, 200).Value = ConvertToDBNull(PrescribeType);
            var par = cmd.Parameters.Add("@Questions", SqlDbType.Udt);
            par.UdtTypeName = "PLSE_New.dbo.QuestionsList";
            par.Value = Questions.Questions.Count == 0 ? DBNull.Value : ConvertToDBNull(Questions);
            par = cmd.Parameters.Add("@Objects", SqlDbType.Udt);
            par.UdtTypeName = "PLSE_New.dbo.ResObjects";
            par.Value = Objects.Objects.Count == 0 ? DBNull.Value : ConvertToDBNull(Objects);
            par = cmd.Parameters.Add("@InsertedID", SqlDbType.Int);
            par.Direction = ParameterDirection.Output;
            cmd.Connection.Open();
            try
            {
                cmd.ExecuteNonQuery();
                cmd.Connection.Close();
                _id = (int)cmd.Parameters["@InsertedID"].Value;
                Version = Version.Original;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {           
                cmd.Connection.Close();
            }
        }
        private void EditToDB(SqlConnection con)
        {
            SqlCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "Activity.prEditResolution";
            cmd.Parameters.Add("@RegDate", SqlDbType.Date).Value = RegistrationDate;
            cmd.Parameters.Add("@ResolDate", SqlDbType.Date).Value = ConvertToDBNull(ResolutionDate);
            cmd.Parameters.Add("@TypeResol", SqlDbType.NVarChar, 30).Value = ResolutionType;
            cmd.Parameters.Add("@Status", SqlDbType.NVarChar, 30).Value = _status;
            cmd.Parameters.Add("@CustID", SqlDbType.Int).Value = Customer.CustomerID;
            cmd.Parameters.Add("@NumberCase", SqlDbType.NVarChar, 50).Value = ConvertToDBNull(Case.Number);
            cmd.Parameters.Add("@Annotate", SqlDbType.NVarChar, 500).Value = ConvertToDBNull(Case.Annotate);
            cmd.Parameters.Add("@Respondent", SqlDbType.NVarChar, 150).Value = ConvertToDBNull(Case.Respondent);
            cmd.Parameters.Add("@Plaintiff", SqlDbType.NVarChar, 150).Value = ConvertToDBNull(Case.Plaintiff);
            cmd.Parameters.Add("@DispatchDate", SqlDbType.Date).Value = ConvertToDBNull(Case.DispatchDate);
            cmd.Parameters.Add("@TypeCase", SqlDbType.Char, 1).Value = CommonInfo.CaseTypes[Case.TypeCase];
            cmd.Parameters.Add("@Comment", SqlDbType.NVarChar, 500).Value = ConvertToDBNull(Case.Comment);
            cmd.Parameters.Add("@PrescribeType", SqlDbType.NVarChar, 200).Value = ConvertToDBNull(PrescribeType);
            cmd.Parameters.Add("@ResolIden", SqlDbType.Int).Value = ResolutionID;
            var par = cmd.Parameters.Add("@Questions", SqlDbType.Udt);
            par.UdtTypeName = "PLSE_New.dbo.QuestionsList";
            par.Value = ConvertToDBNull(Questions);
            par = cmd.Parameters.Add("@Objects", SqlDbType.Udt);
            par.UdtTypeName = "PLSE_New.dbo.ResObjects";
            par.Value = ConvertToDBNull(Objects);
            try
            {
                cmd.Connection.Open();
                cmd.ExecuteNonQuery();
                Version = Version.Original;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                cmd.Connection.Close();
            }
        }
        public void DeleteFromDB(SqlConnection con)
        {
            if (Version == Version.New) return;
            SqlCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "Activity.prDeleteResolution";
            cmd.Parameters.Add("@id", SqlDbType.Int).Value = ResolutionID;
            try
            {
                cmd.Connection.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                cmd.Connection.Close();
            }
        }
        private void ContentToDB(SqlConnection con)
        {
            foreach (var item in _expertisies)
            {
                item.SaveChanges(con);
            }
            Version = Version.Original;
        }
        public override void SaveChanges(SqlConnection con)
        {
            Customer.SaveChanges(con);
            switch (Version)
            {
                case Version.New:
                    AddToDB(con);
                    ContentToDB(con);
                    break;
                case Version.Edited:
                    EditToDB(con);
                    ContentToDB(con);
                    break;
                case Version.ContentEdited:
                    ContentToDB(con);
                    break;
            }
        }
        
    }

    public class Equipment : NotifyBase // todo load from db
    {
        private string _eqname;
        private string _descr;
        private DateTime _commisiondate;
        private bool _status;
        private int _id;
        private DateTime? _checkdate;

        public int EquipmentID => _id;
        public bool IsValid
        {
            get => _status;
            set
            {
                if (value != _status)
                {
                    _status = value;
                    OnPropertyChanged();
                }
            }
        }
        public DateTime CommisionDate
        {
            get => _commisiondate;
            set
            {
                if (value != _commisiondate)
                {
                    _commisiondate = value;
                    OnPropertyChanged();
                }
            }
        }
        public DateTime? LastCheckDate
        {
            get => _checkdate;
            set
            {
                if (value != _checkdate)
                {
                    _checkdate = value;
                    OnPropertyChanged();
                }
            }
        }
        public string Description
        {
            get => _descr;
            set
            {
                if (value != _descr)
                {
                    _descr = value;
                    OnPropertyChanged();
                }
            }
        }
        public string EquipmentName => _eqname;

        public Equipment() : base() { }
        public Equipment(int id, string name, string description, DateTime commisiondate, DateTime check, bool status, Version vr, DateTime updatedate)
            : base(vr, updatedate)
        {
            _id = id;
            _eqname = name;
            _descr = description;
            _commisiondate = commisiondate;
            _status = status;
            _checkdate = check;
        }

        public override string ToString()
        {
            return _eqname;
        }
        private void AddToDB(SqlConnection con)
        {
            SqlCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "InnResources.prAddEquipment";
            cmd.Parameters.Add("@Name", SqlDbType.NVarChar, 100).Value = EquipmentName;
            cmd.Parameters.Add("@Descr", SqlDbType.NVarChar, 500).Value = ConvertToDBNull(_descr);
            cmd.Parameters.Add("@CommDate", SqlDbType.Date).Value = ConvertToDBNull(_commisiondate);
            cmd.Parameters.Add("@CheckDate", SqlDbType.Date).Value = ConvertToDBNull(_checkdate);
            var par = cmd.Parameters.Add("@InsertedID", SqlDbType.Int);
            par.Direction = ParameterDirection.Output;
            try
            {
                cmd.Connection.Open();
                cmd.ExecuteNonQuery();
                _id = (int)cmd.Parameters["@InsertedID"].Value;
                Version = Version.Original;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                cmd.Connection.Close();
            }
        }
        private void EditToDB(SqlConnection con)
        {
            SqlCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "InnResources.prEditEquipment";
            cmd.Parameters.Add("@Name", SqlDbType.NVarChar, 100).Value = EquipmentName;
            cmd.Parameters.Add("@Descr", SqlDbType.NVarChar, 500).Value = ConvertToDBNull(Description);
            cmd.Parameters.Add("@CommDate", SqlDbType.Date).Value = ConvertToDBNull(_commisiondate);
            cmd.Parameters.Add("@CheckDate", SqlDbType.Date).Value = ConvertToDBNull(_checkdate);
            cmd.Parameters.Add("@Status", SqlDbType.NVarChar, 30).Value = _status;
            cmd.Parameters.Add("@EquipmantID", SqlDbType.SmallInt).Value = EquipmentID;
            try
            {
                cmd.Connection.Open();
                cmd.ExecuteNonQuery();
                Version = Version.Original;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                cmd.Connection.Close();
            }
        }
        public void DeleteFromDB(SqlConnection con)
        {
            SqlCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "InnResources.prDeleteEquipment";
            cmd.Parameters.Add("@id", SqlDbType.SmallInt).Value = EquipmentID;
            try
            {
                cmd.Connection.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                cmd.Connection.Close();
            }
        }
        public override void SaveChanges(SqlConnection con)
        {
            switch (Version)
            {
                case Version.New:
                    AddToDB(con);
                    break;
                case Version.Edited:
                    EditToDB(con);
                    break;
            }
        }
    }
    // NOT COMPLEATED
    public class EquipmentUsage : NotifyBase
    {
        #region Fields
        private Expertise _expertise;
        #endregion
        #region Properties
        public Expertise FromExpertise
        {
            get => _expertise;
            set { _expertise = value; }
        }

        public override void SaveChanges(SqlConnection con)
        {
            throw new NotImplementedException();
        }
        #endregion
        private void AddToDB(SqlConnection con)
        {
            throw new NotImplementedException();
        }
        public void DeleteFromDB(SqlConnection con)
        {
            throw new NotImplementedException();
        }
        private void EditToDB(SqlConnection con)
        {
            throw new NotImplementedException();
        }
    }
    public class ExpertiseDetail : NotifyBase
    {
        private short? _nobj;
        private short? _ncat;
        private short? _nver;
        private short? _nalt;
        private short? _nnmet;
        private short? _nnmat;
        private short? _nncom;
        private short? _nnother;
        private string _comment;
        private short? _eval;
        private int _id;

        public int ExpertiseID => _id;
        public short? ObjectsCount
        {
            get => _nobj;
            set
            {
                if (value != _nobj)
                {
                    if (value < 1) throw new ArgumentException("Количество должно быть больше 0");
                    _nobj = value;
                    OnPropertyChanged();
                }
            }
        }
        public short? CategoricalAnswers
        {
            get => _ncat;
            set
            {
                if (value != _ncat)
                {
                    if (value < 1) throw new ArgumentException("Количество должно быть больше 0");
                    _ncat = value;
                    OnPropertyChanged();
                }
            }
        }
        public short? ProbabilityAnswers
        {
            get => _nver;
            set
            {
                if (value != _nver)
                {
                    if (value < 1) throw new ArgumentException("Количество должно быть больше 0");
                    _nver = value;
                    OnPropertyChanged();
                }
            }
        }
        public short? AlternativeAnswers
        {
            get => _nalt;
            set
            {
                if (value != _nalt)
                {
                    if (value < 1) throw new ArgumentException("Количество должно быть больше 0");
                    _nalt = value;
                    OnPropertyChanged();
                }
            }
        }
        public short? NPV_MetodAnswers
        {
            get => _nnmet;
            set
            {
                if (value != _nnmet)
                {
                    if (value < 1) throw new ArgumentException("Количество должно быть больше 0");
                    _nnmet = value;
                    OnPropertyChanged();
                }
            }
        }
        public short? NPV_MaterialAnswers
        {
            get => _nnmat;
            set
            {
                if (value != _nnmat)
                {
                    if (value < 1) throw new ArgumentException("Количество должно быть больше 0");
                    _nnmat = value;
                    OnPropertyChanged();
                }
            }
        }
        public short? NPV_CompAnswers
        {
            get => _nncom;
            set
            {
                if (value != _nncom)
                {
                    if (value < 1) throw new ArgumentException("Количество должно быть больше 0");
                    _nncom = value;
                    OnPropertyChanged();
                }
            }
        }
        public short? NPV_OtherAnswers
        {
            get => _nnother;
            set
            {
                if (value != _nnother)
                {
                    if (value < 1) throw new ArgumentException("Количество должно быть больше 0");
                    _nnother = value;
                    OnPropertyChanged();
                }
            }
        }
        public string Comment
        {
            get => _comment;
            set
            {
                if (value != _comment)
                {
                    _comment = value;
                    OnPropertyChanged();
                }
            }
        }
        public short? Evaluation
        {
            get => _eval;
            set
            {
                if (value != _eval)
                {
                    if (value < 1 || value > 10) throw new ArgumentException("Оценка должна быть от 1 до 10");
                    _eval = value;
                    OnPropertyChanged();
                }
            }
        }

        public ExpertiseDetail(int id, short? nobj, short? ncat, short? nver, short? nalt, short? nnmet, short? nnmat, short? nncom, short? nnother, string comment, short? eval, Version vr)
            : base(vr)
        {
            _id = id; _nobj = nobj; _ncat = ncat; _nver = nver; _nalt = nalt;
            _nnmet = nnmet; _nnmat = nnmat; _nncom = nncom; _nnother = nnother;
            _comment = comment; _eval = eval;
        }

        private void EditToDB(SqlConnection con)
        {
            SqlCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "Activity.prEditExpDetail";
            cmd.Parameters.Add("@NObject", SqlDbType.SmallInt).Value = ConvertToDBNull(_nobj);
            cmd.Parameters.Add("@NCat", SqlDbType.TinyInt).Value = ConvertToDBNull(_ncat);
            cmd.Parameters.Add("@NVer", SqlDbType.TinyInt).Value = ConvertToDBNull(_nver);
            cmd.Parameters.Add("@NAlt", SqlDbType.TinyInt).Value = ConvertToDBNull(_nalt);
            cmd.Parameters.Add("@NNPV_Metod", SqlDbType.TinyInt).Value = ConvertToDBNull(_nnmet);
            cmd.Parameters.Add("@NNPV_Mater", SqlDbType.TinyInt).Value = ConvertToDBNull(_nnmat);
            cmd.Parameters.Add("@NNPV_Comp", SqlDbType.TinyInt).Value = ConvertToDBNull(_nncom);
            cmd.Parameters.Add("@NNPV_Other", SqlDbType.TinyInt).Value = ConvertToDBNull(_nnother);
            cmd.Parameters.Add("@Comment", SqlDbType.NVarChar, 500).Value = ConvertToDBNull(_comment);
            cmd.Parameters.Add("@Evaluation", SqlDbType.TinyInt).Value = ConvertToDBNull(_eval);
            cmd.Parameters.Add("@ExpertiseID", SqlDbType.Int).Value = ExpertiseID;
            try
            {
                cmd.Connection.Open();
                cmd.ExecuteNonQuery();
                Version = Version.Original;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                cmd.Connection.Close();
            }
        }
        public override void SaveChanges(SqlConnection con)
        {
            EditToDB(con);
        }
    }

    public sealed class Expertise : NotifyBase
    {
 #region Fields
        private string _number;
        private Expert _expert;
        private Resolution _resolution;
        private string _result;
        private DateTime _startdate;
        private DateTime? _enddate;
        private byte _timelimit;
        private string _type;
        private int? _prevexp;
        private short? _spendhours;
        ExpertiseDetail _detail;
        private int _id;
        private ObservableCollection<Request> _requests = new ObservableCollection<Request>();
        private ObservableCollection<Report> _raports = new ObservableCollection<Report>();
        private ObservableCollection<Bill> _bills = new ObservableCollection<Bill>();
        private ObservableCollection<EquipmentUsage> _equipmentusage = new ObservableCollection<EquipmentUsage>();
        #endregion

 #region Properties
        public short? SpendHours
        {
            get => _spendhours;
            set
            {
                if (value != _spendhours)
                {
                    if (value < 1) throw new ArgumentException("Количество часов должно быть больше 0");
                    _spendhours = value;
                    OnPropertyChanged();
                }
            }
        }
        public int? PreviousExpertise
        {
            get => _prevexp;
            set
            {
                if (value != _prevexp)
                {
                    _prevexp = value;
                    OnPropertyChanged();
                }
            }
        }
        public string ExpertiseType
        {
            get => _type;
            set
            {
                if (value != _type)
                {
                    if (String.IsNullOrWhiteSpace(value)) throw new ArgumentException("Тип не может быть пустым");
                    _type = value;
                    OnPropertyChanged();
                }
            }
        }
        public Resolution FromResolution
        {
            get => _resolution;
            set { _resolution = value; }
        }
        public byte TimeLimit
        {
            get => _timelimit;
            set
            {
                if (value != _timelimit)
                {
                    if (value == 0) throw new ArgumentException("Срок не может быть 0");
                    _timelimit = value;
                    OnPropertyChanged();
                }
            }
        }
        public DateTime? EndDate
        {
            get => _enddate;
            set
            {
                if (value != _enddate)
                {
                    if (value < _startdate) throw new ArgumentException("Неверная дата окончания");
                    _enddate = value;
                    OnPropertyChanged();
                }
            }
        }
        public DateTime StartDate
        {
            get => _startdate;
            set
            {
                if (value != _startdate)
                {
                    _startdate = value;
                    OnPropertyChanged();
                }
            }
        }
        public string ExpertiseResult
        {
            get => _result;
            set
            {
                if (value != _result)
                {
                    _result = value;
                    OnPropertyChanged();
                }
            }
        }
        public Expert Expert
        {
            get => _expert;
            set
            {
                if (value != _expert)
                {
                    _expert = value;
                    OnPropertyChanged();
                }
            }
        }
        public string Number
        {
            get => _number;
            set
            {
                if (value != _number)
                {
                    if (!IsValidNumber(value)) throw new ArgumentException("Неверный формат номера");
                    _number = value;
                    OnPropertyChanged();
                }
            }
        }
        public ExpertiseDetail ExpertiseDetail
        {
            get => _detail;
            set
            {
                _detail = value;
                OnPropertyChanged();
            }
        }
        public string FullNumber
        {
            get
            {
                string s;
                CommonInfo.CaseTypes.TryGetValue(_resolution.Case.TypeCase, out s);
                return $"{_number}/{Expert.Employee?.Departament.DigitalCode}-{s}";
            }
        }
        public int ExpertiseID => _id;
        public string Remain
        {
            get
            {
                if (_result == "в работе")
                {
                    if (_raports.Count > 0)
                    {
                        var md = _raports.Max(n => n.DelayDate);
                        return (md - DateTime.Now).Days.ToString();
                    }
                    if (_requests.Count > 0)
                    {
                        var zapr = _requests.Where(n => n.RequestType == "запрос").OrderBy(n => n.RequestDate).ToList();
                        if (zapr.Count == 0) return (TimeLimit - (DateTime.Now - _startdate).Days).ToString();
                        int cnt = 0;
                        for (int i = 0; i < zapr.Count(); i++)
                        {
                            DateTime min = zapr[i].RequestDate; DateTime max = min.AddYears(100);
                            if (i == zapr.Count - 1)
                            {
                                var x = _requests.Where(n => (n.RequestType == "ответ" || n.RequestType == "осмотр") && n.RequestDate >= min);
                                if (x.Count() > 0)
                                {
                                    DateTime submax = x.Max(n => n.RequestDate);
                                    cnt += (DateTime.Now - submax).Days;
                                }
                            }
                            else
                            {
                                max = zapr[i + 1].RequestDate;
                                var x = _requests.Where(n => (n.RequestType == "ответ" || n.RequestType == "осмотр") && n.RequestDate < max);
                                if (x.Count() > 0)
                                {
                                    DateTime submax = x.Max(n => n.RequestDate);
                                    cnt += (zapr[i + 1].RequestDate - submax).Days;
                                }
                            }
                        }
                        return (TimeLimit - 1 - (zapr.First().RequestDate - _startdate).Days - cnt).ToString();
                    }
                    return (TimeLimit - (DateTime.Now - _startdate).Days).ToString();
                }
                else return null;
            }
        }
        public int? Remain2
        {
            get
            {
                if (_result == null)
                {
                    if (_raports.Count > 0)
                    {
                        var md = _raports.Max(n => n.DelayDate);
                        return (md - DateTime.Now).Days;
                    }
                    if (_requests.Count > 0)
                    {
                        var zapr = _requests.Where(n => n.RequestType == "запрос").OrderBy(n => n.RequestDate).ToList();
                        if (zapr.Count == 0) return (TimeLimit - (DateTime.Now - _startdate).Days);
                        int cnt = 0;
                        for (int i = 0; i < zapr.Count(); i++)
                        {
                            DateTime min = zapr[i].RequestDate; DateTime max = min.AddYears(100);
                            if (i == zapr.Count - 1)
                            {
                                var x = _requests.Where(n => (n.RequestType == "ответ" || n.RequestType == "осмотр") && n.RequestDate >= min);
                                if (x.Count() > 0)
                                {
                                    DateTime submax = x.Max(n => n.RequestDate);
                                    cnt += (DateTime.Now - submax).Days;
                                }
                            }
                            else
                            {
                                max = zapr[i + 1].RequestDate;
                                var x = _requests.Where(n => (n.RequestType == "ответ" || n.RequestType == "осмотр") && n.RequestDate < max);
                                if (x.Count() > 0)
                                {
                                    DateTime submax = x.Max(n => n.RequestDate);
                                    cnt += (zapr[i + 1].RequestDate - submax).Days;
                                }
                            }
                        }
                        return (TimeLimit - 1 - (zapr.First().RequestDate - _startdate).Days - cnt);
                    }
                    return (TimeLimit - (DateTime.Now - _startdate).Days);
                }
                else return null;
            }
        }
        public int Inwork => EndDate == null ? (DateTime.Now - StartDate).Days : (EndDate.Value - StartDate).Days;
        public int LinkedExpertiseCount => (_resolution?.Expertisies.Count - 1) ?? 0;
        public ObservableCollection<Request> Requests => _requests;
        public ObservableCollection<Report> Reports => _raports;
        public ObservableCollection<Bill> Bills => _bills;
        public ObservableCollection<EquipmentUsage> EquipmentUsage => _equipmentusage;
        #endregion

        public static Expertise New => new Expertise()
        {
            Version = Version.New,
            _startdate = DateTime.Now,
            _timelimit = 30
        };

        [Browsable(false)]
        public string BillSummary
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                foreach (var item in _bills)
                {
                    sb.Append("№ "); sb.Append(item.Number); sb.Append("\tЗадолжность: "); sb.AppendLine(item.Balance.ToString());
                }
                return sb.ToString();
            }
        }
        [Browsable(false)]
        public string RequestSummary
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(StartDate.ToString("d")); sb.AppendLine(" начало производства");
                var rqs = _requests.Select(n => new { D = n.RequestDate, T = n.RequestType });
                var rp = _raports.Select(n => new { D = n.ReportDate, T = $"продлена до {n.DelayDate.ToString("d")}" });
                foreach (var item in rqs.Concat(rp).OrderBy(n => n.D))
                {
                    sb.Append(item.D.ToString("d")); sb.Append(" "); sb.AppendLine(item.T);
                }
                if (EndDate != null)
                {
                    sb.Append(EndDate.Value.ToString("d")); sb.AppendLine(" сдана");
                }
                return sb.ToString();
            }
        }

        private void OnBillListChanged(object o, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (Bill item in e.NewItems)
                    {
                        item.FromExpertise = this;
                    }
                    OnPropertyChanged("Bills", true);
                    break;
            }
        }
        private void OnRequestListChanged(object o, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (Request item in e.NewItems)
                    {
                        item.FromExpertise = this;
                    }
                    OnPropertyChanged("Requests", true);
                    break;
            }
        }
        private void OnReportListChanged(object o, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (Report item in e.NewItems)
                    {
                        item.FromExpertise = this;
                    }
                    OnPropertyChanged("Reports", true);
                    break;
            }
        }
        private void OnEquipmenUsageListChanged(object o, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (EquipmentUsage item in e.NewItems)
                    {
                        item.FromExpertise = this;
                    }
                    OnPropertyChanged("Equipments", true);
                    break;
            }
        }

        public Expertise() : base()
        {
            _bills.CollectionChanged +=OnBillListChanged;
            ((INotifyPropertyChanged)_bills).PropertyChanged += (n, e) => OnPropertyChanged(nameof(Bills), true);
            _requests.CollectionChanged += OnRequestListChanged;
            ((INotifyPropertyChanged)_requests).PropertyChanged += (n, e) => OnPropertyChanged(nameof(Requests), true);
            _raports.CollectionChanged += OnReportListChanged;
            ((INotifyPropertyChanged)_raports).PropertyChanged += (n, e) => OnPropertyChanged(nameof(Reports), true);
            _equipmentusage.CollectionChanged += OnEquipmenUsageListChanged;
            ((INotifyPropertyChanged)_equipmentusage).PropertyChanged += (n, e) => OnPropertyChanged(nameof(EquipmentUsage), true);
        }
        public Expertise(int id, string number, Expert expert, string status, DateTime start, DateTime? end, byte timelimit, string type, int? previous,
                        short? spendhours, Version vr)
            : base(vr)
        {
            _id = id;
            _number = number;
            _expert = expert;
            _result = status;
            _startdate = start;
            _enddate = end;
            _timelimit = timelimit;
            _type = type;
            _prevexp = previous;
            _spendhours = spendhours;
            _bills.CollectionChanged += OnBillListChanged;
            ((INotifyPropertyChanged)_bills).PropertyChanged += (n, e) => OnPropertyChanged(nameof(Bills), true);
            _requests.CollectionChanged += OnRequestListChanged;
            ((INotifyPropertyChanged)_requests).PropertyChanged += (n, e) => OnPropertyChanged(nameof(Requests), true);
            _raports.CollectionChanged += OnReportListChanged;
            ((INotifyPropertyChanged)_raports).PropertyChanged += (n, e) => OnPropertyChanged(nameof(Reports), true);
            _equipmentusage.CollectionChanged += OnEquipmenUsageListChanged;
            ((INotifyPropertyChanged)_equipmentusage).PropertyChanged += (n, e) => OnPropertyChanged(nameof(EquipmentUsage), true);
        }

        public override string ToString()
        {
            return Number + " " + Expert.Employee.ToString() + " (" + Expert.Speciality.ToString() + ")";
        }
        /// <summary>
        /// Сохраняет новую экспертизу в базу данных
        /// </summary>
        /// <param name="con">SqlConnection. Строка соединения</param>
        /// <returns>Int32. Новое значение ключа идентификации в базе данных</returns>
        /// <exception cref="System.NullReferenceException">Поля <c>Resolution</c>, <c>Expert</c> или аргумент <c>con</c> равны null</exception>
        /// <exception cref="System.Data.SqlClient.SqlException"></exception>
        private void AddToDB(SqlConnection con)
        {
            SqlCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "Activity.prAddExpertise";
            cmd.Parameters.Add("@Num", SqlDbType.VarChar, 5).Value = _number;
            cmd.Parameters.Add("@Expert", SqlDbType.Int).Value = Expert.ExpertID;
            cmd.Parameters.Add("@Result", SqlDbType.NVarChar, 50).Value = _result;
            cmd.Parameters.Add("@StartDate", SqlDbType.Date).Value = _startdate;
            cmd.Parameters.Add("@ExDate", SqlDbType.Date).Value = ConvertToDBNull(_enddate);
            cmd.Parameters.Add("@Limit", SqlDbType.TinyInt).Value = _timelimit;
            cmd.Parameters.Add("@Resol", SqlDbType.Int).Value = _resolution.ResolutionID;
            cmd.Parameters.Add("@Type", SqlDbType.NVarChar, 50).Value = _type;
            cmd.Parameters.Add("@PreviousExpertise", SqlDbType.Int).Value = ConvertToDBNull(_prevexp);
            var par = cmd.Parameters.Add("@InsertedID", SqlDbType.Int);
            par.Direction = ParameterDirection.Output;
            try
            {
                cmd.Connection.Open();
                cmd.ExecuteNonQuery();
                _id = (int)cmd.Parameters["@InsertedID"].Value;
                Version = Version.Original;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                cmd.Connection.Close();
            }
        }
        private void EditToDB(SqlConnection con)
        {
            SqlCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "Activity.prEditExpertise";
            cmd.Parameters.Add("@Num", SqlDbType.Char, 5).Value = _number;
            cmd.Parameters.Add("@Expert", SqlDbType.Int).Value = Expert.ExpertID;
            cmd.Parameters.Add("@Result", SqlDbType.NVarChar, 50).Value = _result;
            cmd.Parameters.Add("@StartDate", SqlDbType.Date).Value = _startdate;
            cmd.Parameters.Add("@ExDate", SqlDbType.Date).Value = ConvertToDBNull(_enddate);
            cmd.Parameters.Add("@Limit", SqlDbType.TinyInt).Value = _timelimit;
            cmd.Parameters.Add("@Resol", SqlDbType.Int).Value = _resolution.ResolutionID;
            cmd.Parameters.Add("@Type", SqlDbType.NVarChar, 50).Value = _type;
            cmd.Parameters.Add("@PreviousExpertise", SqlDbType.Int).Value = ConvertToDBNull(_prevexp);
            cmd.Parameters.Add("@ExpIden", SqlDbType.Int).Value = ExpertiseID;
            try
            {
                cmd.Connection.Open();
                cmd.ExecuteNonQuery();
                Version = Version.Original;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                cmd.Connection.Close();
            }
        }
        public void DeleteFromDB(SqlConnection con)
        {
            if (Version == Version.New) return;
            SqlCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "Activity.prDeleteExpertise";
            cmd.Parameters.Add("@id", SqlDbType.Int).Value = ExpertiseID;
            try
            {
                cmd.Connection.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                cmd.Connection.Close();
            }
        }
        private void ContentToDB(SqlConnection con)
        {
            foreach (var item in _bills)
            {
                item.SaveChanges(con);
            }
            foreach (var item in _requests)
            {
                item.SaveChanges(con);
            }
            foreach (var item in _raports)
            {
                item.SaveChanges(con);
            }
            foreach (var item in _equipmentusage)
            {
                item.SaveChanges(con);
            }
            Version = Version.Original;
        }
        private bool IsValidNumber(string num)
        {
            Regex regex = new Regex(@"^[1-9]\d{0,3}$");
            return num != null && regex.IsMatch(num);
        }
        public bool InstanceValidState() => _expert != null && IsValidNumber(_number) && !String.IsNullOrWhiteSpace(_type);
        public override void SaveChanges(SqlConnection con)
        {
            Expert.SaveChanges(con);
            switch (Version)
            {
                case Version.New:
                    AddToDB(con);
                    ContentToDB(con);
                    break;
                case Version.Edited:
                    EditToDB(con);
                    ContentToDB(con);
                    break;
                case Version.ContentEdited:
                    ContentToDB(con);
                    break;
                default:
                    break;
            }
        }
    }

    /// <summary>
    /// Класс инкапсулирующий счет
    /// </summary>
    /// <remarks>
    /// <para>Номер счета используется нативный.</para>
    /// <para>Позже возможно изменение реализации.</para>
    /// </remarks>
    public sealed class Bill : NotifyBase
    {
        #region Fields
        private Expertise _expertise;
        private string _number;
        private DateTime _billdate;
        private DateTime? _paiddate;
        private string _payer;
        private byte _hours;
        private decimal _hourprice;
        private decimal _paid;
        private int _id;
        #endregion
        #region Properties
        public int BillID => _id;
        public Expertise FromExpertise
        {
            get => _expertise;
            set { _expertise = value; }
        }
        /// <summary>
        /// Номер счета
        /// </summary>
        public string Number
        {
            get => _number;
            set
            {
                if (value != _number)
                {
                    if (String.IsNullOrWhiteSpace(value)) throw new ArgumentException("Поле не может быть пустым");
                    _number = value;
                    OnPropertyChanged();
                }
            }
        }
        /// <summary>
        /// Дата счета
        /// </summary>
        public DateTime BillDate
        {
            get => _billdate;
            set
            {
                if (value != _billdate)
                {
                    if (_paiddate.HasValue)
                    {
                        if (value > _paiddate) throw new ArgumentException("Дата счета позднее даты оплаты");
                    }
                    _billdate = value;
                    OnPropertyChanged();
                }
            }
        }
        /// <summary>
        /// Дата оплаты счета. Null если оплаты не было
        /// </summary>
        public DateTime? PaidDate
        {
            get => _paiddate;
            set
            {
                if (value != _paiddate)
                {
                    if (value.HasValue)
                    {
                        if (value.Value < _billdate) throw new ArgumentException("Дата оплаты ранее даты счета");
                    }
                    _paiddate = value;
                    OnPropertyChanged();
                }
            }
        }
        public string Payer
        {
            get => _payer;
            set
            {
                if (value != _payer)
                {
                    if (String.IsNullOrWhiteSpace(value)) throw new ArgumentException("Поле не может быть пустым");
                    _payer = value;
                    OnPropertyChanged();
                }
            }
        }
        public byte Hours
        {
            get => _hours;
            set
            {
                if (value != _hours)
                {
                    if (value == 0) throw new ArgumentException("Количество часов должно быть больше 0");
                    _hours = value;
                    OnPropertyChanged();
                }
            }
        }
        public decimal HourPrice
        {
            get => _hourprice;
            set
            {
                if (value != _hourprice)
                {
                    if (value <= 0) throw new ArgumentException("Цена должна быть больше 0");
                    _hourprice = value;
                    OnPropertyChanged();
                }
            }
        }
        public decimal Paid
        {
            get => _paid;
            set
            {
                if (value != _paid)
                {
                    _paid = value;
                    OnPropertyChanged();
                }
            }
        }
        public decimal Balance => _hours * _hourprice - _paid;
        #endregion
        
        public Bill() : base() { }
        public Bill(int id, string number, DateTime billdate, DateTime? paiddate, string payer, byte hours, decimal hourprice, decimal paid, Version vr)
                        : base (vr)
        {
           _number = number; _id = id;
            _billdate = billdate; _paiddate = paiddate; _payer = payer;
            _hours = hours; _hourprice = hourprice; _paid = paid;
            Version = vr;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("Счет #", 200);
            sb.Append(_number); sb.Append(" от "); sb.AppendLine(_billdate.ToString("d"));
            sb.Append("На сумму: "); sb.Append(_hours * _hourprice); sb.Append("/t"); sb.Append("Оплачено: "); sb.Append(_paid);
            return sb.ToString();
        }
        /// <summary>
        /// Сохраняет новый счет в базу данных
        /// </summary>
        /// <param name="con">SqlConnection. Строка подключения</param>
        /// <returns></returns>
        /// <exception cref="System.NullReferenceException">Полe <c>Expertise</c> или аргумент <c>con</c> равны null</exception>
        /// <exception cref="System.Data.SqlClient.SqlException"></exception>
        private void AddToDB(SqlConnection con)
        {
            SqlCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "Activity.prAddBill";
            cmd.Parameters.Add("@Num", SqlDbType.VarChar, 5).Value = _number;
            cmd.Parameters.Add("@ExpertiseID", SqlDbType.Int).Value = _expertise.ExpertiseID;
            cmd.Parameters.Add("@BillDate", SqlDbType.Date).Value = _billdate;
            cmd.Parameters.Add("@PayerID", SqlDbType.NVarChar, 18).Value = _payer;
            cmd.Parameters.Add("@Nhours", SqlDbType.TinyInt).Value = _hours;
            cmd.Parameters.Add("@HourPrice", SqlDbType.Money).Value = _hourprice;
            var par = cmd.Parameters.Add("@InsertedID", SqlDbType.Int);
            par.Direction = ParameterDirection.Output;
            try
            {
                cmd.Connection.Open();
                cmd.ExecuteNonQuery();
                _id = (int)cmd.Parameters["@InsertedID"].Value;
                Version = Version.Original;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                cmd.Connection.Close();
            }
        }
        private void EditToDB(SqlConnection con)
        {
            SqlCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "Activity.prAddBill";
            cmd.Parameters.Add("@Num", SqlDbType.VarChar, 5).Value = _number;
            cmd.Parameters.Add("@ExpertiseID", SqlDbType.Int).Value = _expertise.ExpertiseID;
            cmd.Parameters.Add("@BillDate", SqlDbType.Date).Value = _billdate;
            cmd.Parameters.Add("@PayerID", SqlDbType.NVarChar, 18).Value = _payer;
            cmd.Parameters.Add("@Nhours", SqlDbType.TinyInt).Value = _hours;
            cmd.Parameters.Add("@HourPrice", SqlDbType.Money).Value = _hourprice;
            cmd.Parameters.Add("@Paid", SqlDbType.Money).Value = _paid;
            cmd.Parameters.Add("@PaidDate", SqlDbType.Date).Value = ConvertToDBNull(_paiddate);
            cmd.Parameters.Add("@BillId", SqlDbType.Int).Value = _id;
            try
            {
                cmd.Connection.Open();
                cmd.ExecuteNonQuery();
                Version = Version.Original;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                cmd.Connection.Close();
            }
        }
        public void DeleteFromDB(SqlConnection con)
        {
            if (Version == Version.New) return;
            SqlCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "Activity.prDeleteBill";
            cmd.Parameters.Add("@BillID", SqlDbType.Int).Value = _id;
            try
            {
                cmd.Connection.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                cmd.Connection.Close();
            }
        }
        public override void SaveChanges(SqlConnection con)
        {
            switch (Version)
            {
                case Version.New:
                    AddToDB(con);
                    break;
                case Version.Edited:
                    EditToDB(con);
                    break;
            }
        }
    }
    public sealed class Request : NotifyBase
    {
        #region Fields
        private Expertise _expertise;
        private DateTime _date;
        private string _type;
        private string _comment;
        private int _id;
        #endregion
        

        public int RequestID => _id;
        public Expertise FromExpertise
        {
            get => _expertise;
            set { _expertise = value; }
        }
        public DateTime RequestDate
        {
            get => _date;
            set
            {
                if (value != _date)
                {
                    _date = value; OnPropertyChanged();
                }
            }
        }
        public string RequestType
        {
            get => _type;
            set
            {
                if (value != _type)
                {
                    _type = value; OnPropertyChanged();
                }
            }
        }
        public string Comment
        {
            get => _comment;
            set
            {
                if (value != _comment)
                {
                    _comment = value; OnPropertyChanged();
                }
            }
        }

        public Request() : base() { }
        public Request(int id, DateTime requestdate, string type, string comment, Version vr)
            : base (vr)
        {
            _id = id; _date = requestdate; _type = type; _comment = comment; Version = vr;
        }

        public override string ToString()
        {
            return _type + " (" + _date.ToString("d") + ")";
        }
        private void AddToDB(SqlConnection con)
        {
            SqlCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "Activity.prAddRequest";
            cmd.Parameters.Add("@ExpID", SqlDbType.Int).Value = _expertise.ExpertiseID;
            cmd.Parameters.Add("@Date", SqlDbType.Date).Value = _date;
            cmd.Parameters.Add("@Type", SqlDbType.NVarChar, 30).Value = _type;
            cmd.Parameters.Add("@Comment", SqlDbType.NVarChar, 500).Value = ConvertToDBNull(_comment);
            var par = cmd.Parameters.Add("@InsertedID", SqlDbType.Int);
            par.Direction = ParameterDirection.Output;
            try
            {
                cmd.Connection.Open();
                cmd.ExecuteNonQuery();
                _id = (int)cmd.Parameters["@InsertedID"].Value;
                Version = Version.Original;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                cmd.Connection.Close();
            }
        }
        private void EditToDB(SqlConnection con)
        {
            SqlCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "Activity.prEditRequest";
            cmd.Parameters.Add("@ExpID", SqlDbType.Int).Value = _expertise.ExpertiseID;
            cmd.Parameters.Add("@Date", SqlDbType.Date).Value = _date;
            cmd.Parameters.Add("@Type", SqlDbType.NVarChar, 30).Value = _type;
            cmd.Parameters.Add("@Comment", SqlDbType.NVarChar, 500).Value = ConvertToDBNull(_comment);
            cmd.Parameters.Add("@RequestID", SqlDbType.Int).Value = _id;
            try
            {
                cmd.Connection.Open();
                cmd.ExecuteNonQuery();
                Version = Version.Original;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                cmd.Connection.Close();
            }
        }
        public void DeleteFromDB(SqlConnection con)
        {
            if (Version == Version.New) return;
            SqlCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "Activity.prDeleteRequest";
            cmd.Parameters.Add("@id", SqlDbType.Int).Value = _id;
            try
            {
                cmd.Connection.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                cmd.Connection.Close();
            }
        }
        public override void SaveChanges(SqlConnection con)
        {
            switch (Version)
            {
                case Version.New:
                    AddToDB(con);
                    break;
                case Version.Edited:
                    EditToDB(con);
                    break;
            }
        }
    }
    public sealed class Report : NotifyBase
    {
        #region Fields
        private Expertise _expertise;
        private DateTime _repdate;
        private DateTime _delay;
        private string _reason;
        private int _id;
        #endregion

        #region Properties
        public int ReportID => _id;
        public Expertise FromExpertise
        {
            get => _expertise;
            set { _expertise = value; }
        }
        public DateTime ReportDate
        {
            get => _repdate;
            set
            {
                if (value != _repdate)
                {
                    _repdate = value; OnPropertyChanged();
                }
            }
        }
        public DateTime DelayDate
        {
            get => _delay;
            set
            {
                if (value != _delay)
                {
                    _delay = value; OnPropertyChanged();
                }
            }
        }
        public string Reason
        {
            get => _reason;
            set
            {
                if (value != _reason)
                {
                    _reason = value; OnPropertyChanged();
                }
            }
        }
        #endregion
        

        public Report() : base() { }
        public Report(int id, DateTime repdate, DateTime delay, string reason, Version vr)
            : base (vr)
        {
            _id = id; _repdate = repdate; _delay = delay; _reason = reason; Version = vr;
        }

        public override string ToString()
        {
            return _reason;
        }
        private void AddToDB(SqlConnection con)
        {
            SqlCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "Activity.prAddReport";
            cmd.Parameters.Add("@ExpID", SqlDbType.Int).Value = _expertise.ExpertiseID;
            cmd.Parameters.Add("@RepDate", SqlDbType.Date).Value = _repdate;
            cmd.Parameters.Add("@DelayDate", SqlDbType.Date).Value = _delay;
            cmd.Parameters.Add("@Reason", SqlDbType.NVarChar, 500).Value = ConvertToDBNull(_reason);
            var par = cmd.Parameters.Add("@InsertedID", SqlDbType.Int);
            par.Direction = ParameterDirection.Output;
            try
            {
                cmd.Connection.Open();
                cmd.ExecuteNonQuery();
                _id = (int)cmd.Parameters["InsertedID"].Value;
                Version = Version.Original;
            }
            catch
            {
                throw;
            }
            finally
            {
                cmd.Connection.Close();
            }
        }
        private void EditToDB(SqlConnection con)
        {
            SqlCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "Activity.prEditReport";
            cmd.Parameters.Add("@ExpID", SqlDbType.Int).Value = _expertise.ExpertiseID;
            cmd.Parameters.Add("@RepDate", SqlDbType.Date).Value = _repdate;
            cmd.Parameters.Add("@DelayDate", SqlDbType.Date).Value = _delay;
            cmd.Parameters.Add("@Reason", SqlDbType.NVarChar, 500).Value = ConvertToDBNull(_reason);
            cmd.Parameters.Add("@ReportID", SqlDbType.Int).Value = _id;
            try
            {
                cmd.Connection.Open();
                cmd.ExecuteNonQuery();
                Version = Version.Original;
            }
            catch
            {
                throw;
            }
            finally
            {
                cmd.Connection.Close();
            }
        }
        public void DeleteFromDB(SqlConnection con)
        {
            if (Version == Version.New) return;
            SqlCommand cmd = con.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "Activity.prDeleteReport";
            cmd.Parameters.Add("@id", SqlDbType.Int).Value = _id;
            try
            {
                cmd.Connection.Open();
                cmd.ExecuteNonQuery();
            }
            catch
            {
                throw;
            }
            finally
            {
                cmd.Connection.Close();
            }
        }
        public override void SaveChanges(SqlConnection con)
        {
            switch (Version)
            {
                case Version.New:
                    AddToDB(con);
                    break;
                case Version.Edited:
                    EditToDB(con);
                    break;
            }
        }
    }
    public class DocsCreater
    {
#region Fields
        private readonly string _inicailpath;
        private readonly string _subscribetemp;
        private readonly string _notifytemp;
        private readonly string _accompantemp;
        private readonly string _conclusiontemp;
        private readonly string _acttemp;
        private readonly string _calculationtemp;
        private readonly string _claimtemp;
        private readonly string _reporttemp;
#endregion
#region Properties
        public string InitialSavePath => _inicailpath;
        public Resolution Resolution { get; }
#endregion
        public DocsCreater(Resolution resolution)
        {
            Resolution = resolution;
            _inicailpath = @"\\ASASSIN-ПК\SIRSERVER\DocFiles\DOCS";
            _subscribetemp = @"c:\Users\Asassin\Documents\Настраиваемые шаблоны Office\подписка эксперта2.dotx";
            _notifytemp = @"c:\Users\Asassin\Documents\Настраиваемые шаблоны Office\Уведомление следователю.dotx";
        }
        private string DestinationPath()
        {
            string path = Path.Combine(_inicailpath, Resolution.RegistrationDate.Year.ToString());
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            path = Path.Combine(path, Resolution.OverallNumber);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return path;
        }
        public void CreateSubscribe(Microsoft.Office.Interop.Word.Application word, IGrouping<int,Expertise> group)
        {
            Document doc = null;
            try
            {
                doc = word.Documents.Add(_subscribetemp);
            }
            catch (Exception)
            {
                throw;
            }
            
            doc.Activate(); //needed?
            
            var bmarks = doc.Bookmarks;
            bmarks["number"].Range.Text = group.Select(n => n.FullNumber).Aggregate((c, n) => c + ", " + n);
            bmarks["annotate"].Range.Text = Resolution.Case.AnnotateBuilder();
            StringBuilder sb = new StringBuilder(400);
            Employee e = group.First().Expert.Employee;
            sb.Append(StringUtil.Decline(e.Inneroffice, LingvoNET.Case.Dative));
            sb.Append(", ");
            sb.Append(e.ToString("D"));
            if (e.Gender == "женский")
            {
                sb.Append(", имеющей ");
                bmarks["gender"].Range.Text = "предупреждена";
            }                
            else
            {
                sb.Append(", имеющему ");
                bmarks["gender"].Range.Text = "предупрежден";
            }
            sb.Append(e.Education1);
            if(e.Education2 != null)
            {
                sb.Append(", ");
                sb.Append(e.Education2);
            }
            if (e.Education3 != null)
            {
                sb.Append(", ");
                sb.Append(e.Education3);
            }
            if (e.Sciencedegree != null)
            {
                sb.Append(", ");
                sb.Append(e.Sciencedegree);
            }
            sb.Append(", ");
            foreach (var item in group)
            {
                sb.Append(item.Expert.Requisite());
                sb.Append(", ");
            }
            bmarks["expert"].Range.Text = sb.ToString();
            bmarks["fio"].Range.Text = e.ToString();
            bmarks["date"].Range.Text = group.First().StartDate.ToString("dd MMMM yyyy");
            bmarks["codex"].Range.Text = Resolution.Case.Codex();
            bmarks["respon"].Range.Text = Resolution.Case.SubcribeArticle();
            string to = Path.Combine(DestinationPath(), e.ToString() + " подписка.docx");
            if (File.Exists(to))
            {
                File.Delete(to);
            }
            try
            {
                doc.SaveAs2(to);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                doc.Close();
            }
        }
        public void CreateNotify(Microsoft.Office.Interop.Word.Application word, IGrouping<DateTime, Expertise> group)
        {
            var doc = word.Documents.Add(_notifytemp);
            doc.Activate();
            var bmarks = doc.Bookmarks;
            if (Resolution.ResolutionDate != null)
            {
                bmarks["casedate"].Range.Text = bmarks["casedate2"].Range.Text = Resolution.ResolutionDate.Value.ToString("dd.MM.yyyy");
            }
            else
            {
                Range r = bmarks["casedate"].Range;
                r.Text = "[не указано]";
                r.Font.Color = WdColor.wdColorRed;
                r = bmarks["casedate2"].Range;
                r.Text = "[не указано]";
                r.Font.Color = WdColor.wdColorRed;
            }
            bmarks["casenumber"].Range.Text = bmarks["casenumber2"].Range.Text = Resolution.Case.Number;
            bmarks["date"].Range.Text = bmarks["date2"].Range.Text = bmarks["startdate"].Range.Text = bmarks["startdate2"].Range.Text = group.Key.ToString("dd.MM.yyyy");
            bmarks["departament"].Range.Text = bmarks["departament2"].Range.Text = group.Select(n => n.Expert.Employee.Departament.Acronym)
                                                                                        .Distinct()
                                                                                        .Aggregate((c, n) => c + ", " + n);
            bmarks["fio"].Range.Text = bmarks["fio2"].Range.Text = group.Select(n => n.Expert.Employee.ToString("d"))
                                                                        .Distinct()
                                                                        .Aggregate((c, n) => c + ", " + n);
            bmarks["number"].Range.Text = bmarks["number2"].Range.Text = Resolution.FullOverallNumber;
            var pr = group.Select(n => n.Expert.Employee.ToString())
                          .Distinct()
                          .Count() > 1 ? "экспертам" : "эксперту";
            bmarks["plurality"].Range.Text =  bmarks["plurality3"].Range.Text = pr;
            bmarks["plurality2"].Range.Text = bmarks["plurality4"].Range.Text = pr.ToUpperFirstLetter();
            StringBuilder sb = new StringBuilder(300);
            sb.Append(StringUtil.Decline(Resolution.Customer.Office, LingvoNET.Case.Dative).ToUpperFirstLetter());
            sb.Append(" ");
            sb.Append(Resolution.Customer?.Organization.Name.DeclineBeforeNoun(LingvoNET.Case.Genitive));
            sb.AppendLine();
            if (Resolution.Customer.Rank != null)
            {
                sb.Append(StringUtil.Decline(Resolution.Customer.Rank, LingvoNET.Case.Dative));
                sb.AppendLine();
            }
            sb.Append(Resolution.Customer.ToString("d"));
            bmarks["recipient"].Range.Text = bmarks["recipient2"].Range.Text = sb.ToString();
            string spec = null; 
            var sp = group.Select(n => n.Expert.Speciality).Distinct(new SpecialityComperer());
            foreach (var item in sp)
            {
                spec += Declination.DeclineSpeciality(item, LingvoNET.Case.Genitive);
            }
            bmarks["species"].Range.Text = bmarks["species2"].Range.Text = spec != null ? spec : "экспертизы";
            string to = Path.Combine(DestinationPath(), $"уведомление следователю от {group.Key:d}.docx");
            if (File.Exists(to))
            {
                File.Delete(to);
            }
            try
            {
                doc.SaveAs2(to);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                doc.Close();
            }
        }
        private void StartDoc()
        {
            Microsoft.Office.Interop.Word.Application word = null;
            try
            {
                word = new Microsoft.Office.Interop.Word.Application();
                foreach (var item in Resolution.Expertisies.GroupBy(n => n.Expert.Employee.EmployeeID))
                {
                    CreateSubscribe(word, item);
                }
                if (Resolution.Case.TypeCase == "уголовное")
                {
                    foreach (var item in Resolution.Expertisies.GroupBy(n => n.StartDate))
                    {
                        CreateNotify(word, item);
                    }
                }  
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                word.Quit();
            }
        }
        public async System.Threading.Tasks.Task Crtr()
        {
            System.Threading.Tasks.Task t = System.Threading.Tasks.Task.Run(() => StartDoc());
            try
            {
                await t;
            }
            catch (Exception)
            {

                MessageBox.Show("Error occur during creating word documents");
            }
            
            MessageBox.Show("Documents created");
        }
    }
    public class SpecialityComperer : IEqualityComparer<Speciality>
    {
        public bool Equals(Speciality x, Speciality y)
        {
            return x.SpecialityID == y.SpecialityID;
        }

        public int GetHashCode(Speciality obj)
        {
            return obj.SpecialityID.GetHashCode();
        }
    }
}