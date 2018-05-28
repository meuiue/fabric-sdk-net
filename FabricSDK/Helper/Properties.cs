﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Hyperledger.Fabric.SDK.Helper
{
    //From StackOverflow
    //https://stackoverflow.com/a/7696370
    //User: Nick Rimmer & Community
    //Heavly modified to suit
    public class Properties : IEnumerable<string>
    {
        private string filename;
        private Dictionary<string, string> list=new Dictionary<string, string>();

        public Properties()
        {
        }
        public string this[string field]
        {
            get => Get(field);
            set => Set(field, value);
        }

        public string Get(string field, string defValue)
        {
            return Get(field) == null ? defValue : Get(field);
        }

        public string Remove(string field)
        {
            if (list.ContainsKey(field))
            {
                string value = list[field];
                list.Remove(field);
                return value;
            }

            return null;
        }

        public string Get(string field)
        {
            return list.ContainsKey(field) ? list[field] : null;
        }

        public bool Contains(string field)
        {
            return list.ContainsKey(field);
        }

        public void Set(string field, object value)
        {
            string val = null;
            if (value != null)
                val = Convert.ToString(value, CultureInfo.InvariantCulture);
            if (!list.ContainsKey(field))
                list.Add(field, val);
            else
                list[field] = val;
        }

        public void Save()
        {
            Save(filename);
        }

        public void Save(string filename)
        {
            this.filename = filename;

            StreamWriter file = new StreamWriter(filename);

            foreach (string prop in list.Keys.ToArray())
                if (!string.IsNullOrWhiteSpace(list[prop]))
                    file.WriteLine(prop + "=" + list[prop]);

            file.Close();
        }

        public void Reload()
        {
            if (!string.IsNullOrEmpty(filename))
                Load(filename);
        }

        public void Load(string filename)
        {
            this.filename = filename;
            list = new Dictionary<string, string>();

            if (File.Exists(filename))
                LoadFromFile(filename);
        }

        private void LoadFromFile(string file)
        {
            foreach (string line in File.ReadAllLines(file))
            {
                if (!string.IsNullOrEmpty(line) && !line.StartsWith(";") && !line.StartsWith("#") && !line.StartsWith("'") && line.Contains('='))
                {
                    int index = line.IndexOf('=');
                    string key = line.Substring(0, index).Trim();
                    string value = line.Substring(index + 1).Trim();

                    if (value.StartsWith("\"") && value.EndsWith("\"") || value.StartsWith("'") && value.EndsWith("'"))
                    {
                        value = value.Substring(1, value.Length - 2);
                    }

                    list[key] = value;
                }
            }
        }

        public IEnumerator<string> GetEnumerator()
        {
            foreach (string str in list.Keys)
                yield return str;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public Properties Clone()
        {
            Properties p=new Properties();
            p.filename = filename;
            p.list = list.ToDictionary(a => a.Key, a => a.Value);
            return p;
        }

        public int Count => list.Count;
    }
}