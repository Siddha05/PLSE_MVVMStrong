using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace PLSE_MVVMStrong.SQL
{
    public class ContentWrapper
    {
        private string _content;

        public string Content
        {
            get => _content;
            set => _content = value;
        }

        public ContentWrapper() { }
        public ContentWrapper(string q)
        {
            _content = q;
        }

        public override string ToString()
        {
            return _content;
        }
    }

    [Serializable]
    [SqlUserDefinedType(Format.UserDefined, MaxByteSize = -1, IsByteOrdered = true)]
    [StructLayout(LayoutKind.Sequential)]
    public class QuestionsList : INullable, IBinarySerialize
    {
        private List<ContentWrapper> _quest = new List<ContentWrapper>();
        private bool _null;

        public List<ContentWrapper> Questions => _quest;
        public bool IsNull => _null;

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in _quest)
            {
                sb.AppendLine(item.Content);
            }
            return sb.ToString();
        }
        public static QuestionsList Null
        {
            get
            {
                QuestionsList h = new QuestionsList();
                h._null = true;
                return h;
            }
        }
        public static QuestionsList Parse(SqlString s)
        {
            if (s.IsNull)
                return Null;
            QuestionsList u = new QuestionsList();
            var a = s.Value.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var item in a)
            {
                u._quest.Add(new ContentWrapper(item));
            }
            return u;
        }
        public void Read(BinaryReader r)
        {
            _null = r.ReadBoolean();
            //_quest = new List<ContentWrapper>();
            var a = r.ReadString().Split('*');
            foreach (var item in a)
            {
                _quest.Add(new ContentWrapper(item));
            }
        }
        public void Write(BinaryWriter w)
        {
            w.Write(_null);
            string[] sa = _quest.ConvertAll<string>(n => n.ToString()).ToArray();
            w.Write(String.Join("*", sa));
        }

        public QuestionsList()
        {
        }
        public QuestionsList(IEnumerable<ContentWrapper> r)
        {
            foreach (var item in r)
            {
                _quest.Add(item);
            }
        }
    }

    [Serializable]
    [SqlUserDefinedType(Format.UserDefined, MaxByteSize = -1, IsByteOrdered = true)]
    [StructLayout(LayoutKind.Sequential)]
    public class ObjectsList : INullable, IBinarySerialize
    {
        private bool _null;
        private List<ContentWrapper> _objects = new List<ContentWrapper>();

        public List<ContentWrapper> Objects => _objects;
        public static ObjectsList Null
        {
            get
            {
                ObjectsList h = new ObjectsList();
                h._null = true;
                return h;
            }
        }
        public bool IsNull => _null;

        public static ObjectsList Parse(SqlString s)
        {
            if (s.IsNull) return Null;
            ObjectsList u = new ObjectsList();
            var a = s.Value.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var item in a)
            {
                u._objects.Add(new ContentWrapper(item));
            }
            return u;
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in _objects)
            {
                sb.AppendLine(item.Content);
            }
            return sb.ToString();
        }
        
        public void Read(BinaryReader r)
        {
            _null = r.ReadBoolean();
            var a = r.ReadString().Split('*');
            foreach (var item in a)
            {
                _objects.Add(new ContentWrapper(item));
            }
        }
        public void Write(BinaryWriter w)
        {
            w.Write(_null);
            string[] sa = _objects.ConvertAll<string>(n => n.ToString()).ToArray();
            w.Write(String.Join("*", sa));
        }
        public ObjectsList()
        {
        }
    }
}