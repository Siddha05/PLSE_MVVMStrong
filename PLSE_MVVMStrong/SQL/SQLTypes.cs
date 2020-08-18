using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace PLSE_MVVMStrong.SQL
{
    public class ContentWrapper : INotifyPropertyChanged
    {
        string _num;
        string _cont;
        public string Content
        {
            get => _cont;
            set
            {
                _cont = value;
                OnPropertyChanged();
            }
        }
        public string Number
        {
            get => _num;
            set
            {
                _num = value;
                OnPropertyChanged();
            }
        }
        public ContentWrapper(string q)
        {
            Content = q;
        }
        public ContentWrapper()
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public override string ToString()
        {
            return Content;
        }
        private void OnPropertyChanged([CallerMemberName] string prop = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }

    [Serializable]
    [SqlUserDefinedType(Format.UserDefined, MaxByteSize = -1, IsByteOrdered = true)]
    [StructLayout(LayoutKind.Sequential)]
    public class QuestionsList : INullable, IBinarySerialize
    {
        private ObservableCollection<ContentWrapper> _quest = new ObservableCollection<ContentWrapper>();
        private bool _null;

        public ObservableCollection<ContentWrapper> Questions => _quest;
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
            var a = r.ReadString().Split('*');
            foreach (var item in a)
            {
                _quest.Add(new ContentWrapper(item));
            }
        }
        public void Write(BinaryWriter w)
        {
            w.Write(_null);
            string[] sa = _quest.Select(n => n.ToString()).ToArray();
            w.Write(String.Join("*", sa));
        }

        public QuestionsList()
        {
            _quest.CollectionChanged += _quest_CollectionChanged;
        }

        private void _quest_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                case System.Collections.Specialized.NotifyCollectionChangedAction.Move:
                    int i = 1;
                    foreach (var item in _quest)
                    {
                        item.Number = i.ToString();
                        i++;
                    }
                    break;
                default:
                    break;
            }
        }
    }

    [Serializable]
    [SqlUserDefinedType(Format.UserDefined, MaxByteSize = -1, IsByteOrdered = true)]
    [StructLayout(LayoutKind.Sequential)]
    public class ObjectsList : INullable, IBinarySerialize
    {
        private bool _null;
        private ObservableCollection<ContentWrapper> _objects = new ObservableCollection<ContentWrapper>();

        public ObservableCollection<ContentWrapper> Objects => _objects;
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
            string[] sa = _objects.Select(n => n.ToString()).ToArray();
            w.Write(String.Join("*", sa));
        }
        public ObjectsList()
        {
            this._objects.CollectionChanged += _objects_CollectionChanged;
        }

        private void _objects_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                case System.Collections.Specialized.NotifyCollectionChangedAction.Move:
                    int i = 1;
                    foreach (var item in _objects)
                    {
                        item.Number = i.ToString();
                        i++;
                    }
                    break;
                default:
                    break;
            }
        }
    }
}