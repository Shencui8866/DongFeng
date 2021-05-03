using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Text;
using Josing.Net;

namespace Josing.IO
{
    /// <summary>
    /// IO工具
    /// </summary>
    public static class IOUtils
    {


        /// <summary>
        /// 是否 存在路径
        /// </summary>
        public static bool HasDirectory(string path)
        {
            return Directory.Exists(path);
        }
        /// <summary>
        /// 是否存在文件
        /// </summary>
        public static bool HasFile(string path)
        {
            return File.Exists(path);
        }
        /// <summary>
        /// 删除路径
        /// </summary>
        public static void DeDirectory(string path)
        {
            Directory.Delete(path);
        }
        /// <summary>
        /// 生成路径
        /// </summary>
        public static void CreateDirectory(string path)
        {
            Directory.CreateDirectory(path);
        }
        /// <summary>
        /// 删除文件
        /// </summary>
        public static void DeFile(string path)
        {
            File.Delete(path);
        }
        /// <summary>
        /// 生成文件
        /// </summary>
        public static void CreateFile(string path)
        {
            using (File.CreateText(path)) { }
        }
        /// <summary>
        /// 读取文件
        /// </summary>
        public static void Read(string path, ref List<string> reader, EncodingType encoding = EncodingType.Defualt)
        {
            using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.None))
            {
                using (StreamReader streamWriter = new StreamReader(fileStream, GetEncoding(encoding)))
                {
                    while (streamWriter.Peek() > 0)
                        reader.Add(streamWriter.ReadLine());
                }
            }
        }
        /// <summary>
        /// 读取文件
        /// </summary>
        public static void Read(string path, char splt, ref Dictionary<string, string> reader, EncodingType encoding = EncodingType.Defualt)
        {
            using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.None))
            {
                using (StreamReader streamWriter = new StreamReader(fileStream, GetEncoding(encoding)))
                {
                    string rd = "";
                    while (streamWriter.Peek() > 0)
                    {
                        rd = streamWriter.ReadLine();
                        if (!rd.Contains("##"))
                        {
                            string[] temp = rd.Split(splt);
                            reader.Add(temp[0], temp[1]);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 读取文件
        /// </summary>
        public static void Read(string path, Action<string> reader, EncodingType encoding = EncodingType.Defualt)
        {
            using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.None))
            {
                using (StreamReader streamWriter = new StreamReader(fileStream, GetEncoding(encoding)))
                {
                    while (streamWriter.Peek() > 0)
                        reader(streamWriter.ReadLine());
                }
            }
        }
        /// <summary>
        /// 追加写入
        /// </summary>
        public static void WriteAppend(string path, string[] content, EncodingType encoding = EncodingType.Defualt)
        {
            using (FileStream fileStream = new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.None))
            {
                using (StreamWriter streamWriter = new StreamWriter(fileStream, GetEncoding(encoding)))
                {
                    for (int i = 0; i < content.Length; i++)
                        streamWriter.WriteLine(content[i]);
                }
            }
        }
        /// <summary>
        /// 追加写入
        /// </summary>
        public static void WriteAppend(string path, char splt, Dictionary<string, string> content, EncodingType encoding = EncodingType.Defualt)
        {
            using (FileStream fileStream = new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.None))
            {
                using (StreamWriter streamWriter = new StreamWriter(fileStream, GetEncoding(encoding)))
                {
                    foreach (KeyValuePair<string, string> s in content)
                        streamWriter.WriteLine(s.Key + splt + s.Value);
                }
            }
        }
        /// <summary>
        /// 追加写入
        /// </summary>
        public static void WriteAppend(string path, IEnumerable<string> content, EncodingType encoding = EncodingType.Defualt)
        {
            using (FileStream fileStream = new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.None))
            {
                using (StreamWriter streamWriter = new StreamWriter(fileStream, GetEncoding(encoding)))
                {
                    foreach (string s in content)
                        streamWriter.WriteLine(s);
                }
            }
        }
        /// <summary>
        /// 覆盖写入
        /// </summary>
        public static void WriteCover(string path, string[] content, EncodingType encoding = EncodingType.Defualt)
        {
            using (FileStream fileStream = new FileStream(path, FileMode.Truncate, FileAccess.Write, FileShare.None))
            {
                using (StreamWriter streamWriter = new StreamWriter(fileStream, GetEncoding(encoding)))
                {
                    for (int i = 0; i < content.Length; i++)
                        streamWriter.WriteLine(content[i]);
                }
            }
        }
        /// <summary>
        /// 覆盖写入
        /// </summary>
        public static void WriteCover(string path, char splt, Dictionary<string, string> content, EncodingType encoding = EncodingType.Defualt)
        {
            using (FileStream fileStream = new FileStream(path, FileMode.Truncate, FileAccess.Write, FileShare.None))
            {
                using (StreamWriter streamWriter = new StreamWriter(fileStream, GetEncoding(encoding)))
                {
                    foreach (KeyValuePair<string, string> s in content)
                        streamWriter.WriteLine(s.Key + splt + s.Value);
                }
            }
        }
        /// <summary>
        /// 覆盖写入
        /// </summary>
        public static void WriteCover(string path, IEnumerable<string> content, EncodingType encoding = EncodingType.Defualt)
        {
            using (FileStream fileStream = new FileStream(path, FileMode.Truncate, FileAccess.Write, FileShare.None))
            {
                using (StreamWriter streamWriter = new StreamWriter(fileStream, GetEncoding(encoding)))
                {
                    foreach (string s in content)
                        streamWriter.WriteLine(s);
                }
            }
        }
        /// <summary>
        /// 写入文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static void WriteFile(string path, byte[] data)
        {
            using (FileStream fileStream = new FileStream(path, FileMode.CreateNew, FileAccess.Write, FileShare.None))
            {
                fileStream.Write(data, 0, data.Length);
            }
        }

        /// <summary>
        /// 写入文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static void WriteFile(string path, string data)
        {
            using (FileStream fileStream = new FileStream(path, FileMode.CreateNew, FileAccess.Write, FileShare.None))
            {
                fileStream.Write(Encoding.Default.GetBytes(data), 0, data.Length);
            }
        }

        static Encoding GetEncoding(EncodingType encoding)
        {
            switch(encoding)
            {
                case EncodingType.Defualt: return Encoding.Default;
                case EncodingType.ASCII: return Encoding.ASCII;
                default: return Encoding.UTF8;
            }
        }
    }
}

