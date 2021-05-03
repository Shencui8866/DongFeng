using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Josing.Net;

namespace Josing.IO
{
    public class JIni
    {
        public string Path { get; set; }
        public List<CfgItem> CfgPools { get; } = new List<CfgItem>();
        public string this[string val] { get { return GetString(val); } }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path">路径</param>
        public JIni(string path) { ReadCfg(path); }
        /// <summary>
        /// 读取配置,读取前清除已存在配置
        /// </summary>
        /// <param name="path"></param>
        public void ReadCfg(string path)
        {
            ClearCfg();
            Path = path;
            if (!IOUtils.HasFile(path)) return;
            CfgItem item = new CfgItem();
            IOUtils.Read(path, (reader) =>
            {
                if (string.IsNullOrEmpty(reader)) return;
                if (reader.Contains("##"))
                    item.AddNote(reader);
                else
                {
                    item.AddCfg(reader);
                    CfgPools.Add(item);
                    item = new CfgItem();
                }
            }, EncodingType.UTF8);
            if (!CfgPools.Contains(item) && item.KeyValid) CfgPools.Add(item);
        }
        /// <summary>
        /// 清除配置
        /// </summary>
        public void ClearCfg() { CfgPools.Clear(); }
        /// <summary>
        /// 添加配置
        /// </summary>
        /// <param name="key">配置名称</param>
        /// <param name="val">值</param>
        /// <returns></returns>
        public CfgItem AddCfg(string key, string val)
        {
            CfgItem cfg = GetCfgItem(key);
            if (cfg != null)
            {
                Debug.LogError("key已存在 " + key);
                return null;
            }
            cfg = new CfgItem(key, val);
            CfgPools.Add(cfg);
            return cfg;
        }
        /// <summary>
        /// 修改配置值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public CfgItem EditCfg(string key, string val)
        {
            CfgItem cfg  = GetCfgItem(key);
            if (cfg != null)
                cfg.SetVal(val);
            return cfg;
        }
        /// <summary>
        /// 删除配置
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public CfgItem DeleteCfg(string key)
        {
            CfgItem cfg = GetCfgItem(key);
            if (cfg != null)
                CfgPools.Remove(cfg);
            return cfg;
        }
        /// <summary>
        /// 通过key获取该配置
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public CfgItem GetCfgItem(string key)
        {
            CfgItem cfg = CfgPools.Find((x) =>
            {
                if (x.Key.Equals(key)) return true;
                return false;
            });
            return cfg;
        }
        /// <summary>
        /// 是否存在key的配置
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool HasCfg(string key) { return GetCfgItem(key) != null; }
        /// <summary>
        /// 保存
        /// </summary>
        /// <returns></returns>
        public bool Save()
        {
            try
            {
                List<string> pools = new List<string>();
                for (int i = 0; i < CfgPools.Count; i++)
                    pools.AddRange(CfgPools[i].GetCfg());
                if (!IOUtils.HasFile(Path)) IOUtils.CreateFile(Path);
                IOUtils.WriteCover(Path, pools);
                return true;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// 获取float值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <returns>是否成功</returns>
        public bool GetFloat(string key, ref float val)
        {
            CfgItem cfg = GetCfgItem(key);
            if(cfg != null)
            {
                try
                {
                    val = float.Parse(cfg.Val);
                    return true;
                }
                catch { return false; }
            }
            return false;
        }
        /// <summary>
        /// 获取int值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <returns>是否成功</returns>
        public bool GetInt(string key, ref int val)
        {
            CfgItem cfg = GetCfgItem(key);
            if (cfg != null)
            {
                try
                {
                    val = int.Parse(cfg.Val);
                    return true;
                }
                catch { return false; }
            }
            return false;
        }
        /// <summary>
        /// 获取bool值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <returns>是否成功</returns>
        public bool GetBool(string key, ref bool val)
        {
            CfgItem cfg = GetCfgItem(key);
            if (cfg != null)
            {
                try
                {
                    val = bool.Parse(cfg.Val);
                    return true;
                }
                catch { return false; }
            }
            return false;
        }
        /// <summary>
        /// 获取字符串
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetString(string key)
        {
            CfgItem cfg = GetCfgItem(key);
            if (cfg != null)
                return cfg.Val;
            return "";
        }
    }

    public class CfgItem
    {
        List<string> notes = new List<string>();

        public string Key { get; private set; }
        public string Val { get; private set; }
        public bool KeyValid { get { return !string.IsNullOrEmpty(Key); } }

        public CfgItem() { }
        public CfgItem(string note)
        {
            notes.Add(note);
        }
        public CfgItem(string key, string val)
        {
            Key = key;
            Val = val;
        }
        internal void AddNote(string note)
        {
            if(note.Contains("##"))
                notes.Insert(0, note);
            else
            {
                string nt = note.Insert(0, "##");
                notes.Insert(0, nt);
            }
        }
        public void AddCfg(string cfg)
        {
            string[] c = cfg.Split('=');
            Key = c[0];
            Val = c[1];
        }
        public void SetKey(string key) { Key = key; }
        public void SetVal(string val) { Val = val; }
        public List<string> GetCfg()
        {
            List<string> l = new List<string>();
            l.AddRange(notes);
            l.Add(Key + "=" + Val);
            return l;
        }

        public override string ToString()
        {
            return string.Format("key : {0}, val : {1}", Key, Val);
        }
    }
}
