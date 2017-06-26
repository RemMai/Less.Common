﻿//bibaoke.com

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Less.Text;
using Less.Collection;
using System;

namespace Less.Windows
{
    /// <summary>
    /// String 扩展方法
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// 复制目录到指定目录
        /// </summary>
        /// <param name="s"></param>
        /// <param name="destDirName"></param>
        /// <returns></returns>
        public static string CopyDirToDir(this string s, string destDirName)
        {
            return s.CopyDirToDir(destDirName, false);
        }

        /// <summary>
        /// 复制目录到指定目录
        /// </summary>
        /// <param name="s"></param>
        /// <param name="destDirName"></param>
        /// <param name="merge"></param>
        /// <returns></returns>
        public static string CopyDirToDir(this string s, string destDirName, bool merge)
        {
            destDirName = destDirName.CombinePath(s.GetFolderName());

            s.CopyDir(destDirName, merge);

            return s;
        }

        /// <summary>
        /// 复制目录
        /// </summary>
        /// <param name="s"></param>
        /// <param name="destDirName"></param>
        /// <returns></returns>
        public static string CopyDir(this string s, string destDirName)
        {
            return s.CopyDir(destDirName, false);
        }

        /// <summary>
        /// 复制目录
        /// </summary>
        /// <param name="s"></param>
        /// <param name="destDirName"></param>
        /// <param name="merge"></param>
        /// <returns></returns>
        public static string CopyDir(this string s, string destDirName, bool merge)
        {
            if (destDirName.IsChildDirOf(s))
                throw new InvalidOperationException("目标文件夹是源文件夹的子文件夹");

            if (s.ExistsDir())
            {
                destDirName.CreateDir();

                foreach (string i in s.GetFiles())
                    i.CopyFileToDir(destDirName, merge);

                foreach (string i in s.GetDirectories())
                    i.CopyDirToDir(destDirName);
            }
            else
            {
                throw new DirectoryNotFoundException();
            }

            return s;
        }

        /// <summary>
        /// 是否指定目录的子目录
        /// </summary>
        /// <param name="s"></param>
        /// <param name="dirName"></param>
        /// <returns></returns>
        public static bool IsChildDirOf(this string s, string dirName)
        {
            DirectoryInfo parent = new DirectoryInfo(s).Parent;

            if (parent.IsNotNull())
            {
                DirectoryInfo directory = new DirectoryInfo(dirName);

                while (parent.FullName != directory.FullName)
                {
                    if (parent.Parent.IsNotNull())
                        parent = parent.Parent;
                    else
                        return false;
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 复制文件到指定目录
        /// </summary>
        /// <param name="s"></param>
        /// <param name="destDirName"></param>
        /// <returns></returns>
        public static string CopyFileToDir(this string s, string destDirName)
        {
            return s.CopyFileToDir(destDirName, false);
        }

        /// <summary>
        /// 复制文件到指定目录
        /// </summary>
        /// <param name="s"></param>
        /// <param name="destDirName"></param>
        /// <param name="overwrite"></param>
        /// <returns></returns>
        public static string CopyFileToDir(this string s, string destDirName, bool overwrite)
        {
            s.CopyFile(destDirName.CombinePath(s.GetFileName()), overwrite);

            return s;
        }

        /// <summary>
        /// 复制文件
        /// </summary>
        /// <param name="s"></param>
        /// <param name="destFileName"></param>
        /// <param name="overwrite"></param>
        /// <returns></returns>
        public static string CopyFile(this string s, string destFileName, bool overwrite)
        {
            File.Copy(s, destFileName, overwrite);

            return s;
        }

        /// <summary>
        /// 复制文件
        /// </summary>
        /// <param name="s"></param>
        /// <param name="destFileName"></param>
        /// <returns></returns>
        public static string CopyFile(this string s, string destFileName)
        {
            File.Copy(s, destFileName);

            return s;
        }

        /// <summary>
        /// 读取文件
        /// </summary>
        /// <param name="s"></param>
        /// <param name="e">编码</param>
        /// <returns></returns>
        public static string ReadString(this string s, Encoding e)
        {
            return e.GetString(s.ReadBytes());
        }

        /// <summary>
        /// 读取文件
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static byte[] ReadBytes(this string s)
        {
            return File.ReadAllBytes(s);
        }

        /// <summary>
        /// 读取文件
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static FileStream ReadStream(this string s)
        {
            return File.OpenRead(s);
        }

        /// <summary>
        /// 写入数据流 目录不存在则创建 文件存在则覆盖
        /// </summary>
        /// <param name="s"></param>
        /// <param name="readStream"></param>
        /// <returns></returns>
        public static string Write(this string s, Stream readStream)
        {
            s.GetDirectoryName().CreateDir();

            using (Stream writeStream = File.Open(s, FileMode.Create, FileAccess.Write, FileShare.Read))
            {
                //分配 8KB 缓存
                Buffer buffer = new Buffer(1024 * 8);

                //缓存后写入
                buffer.Buff(readStream, writeStream);
            }

            return s;
        }

        /// <summary>
        /// 写入数据 目录不存在则创建 文件存在则覆盖
        /// </summary>
        /// <param name="s"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string Write(this string s, byte[] data)
        {
            s.GetDirectoryName().CreateDir();

            File.WriteAllBytes(s, data);

            return s;
        }

        /// <summary>
        /// 向文件写入一行 文件不存在则创建
        /// </summary>
        /// <param name="s"></param>
        /// <param name="content">内容</param>
        /// <param name="e">编码</param>
        /// <returns></returns>
        public static string AppendLine(this string s, string content, Encoding e)
        {
            File.AppendAllText(s, content.Combine(Symbol.NewLine), e);

            return s;
        }

        /// <summary>
        /// 删除文件 文件不存在不抛出异常
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string DeleteFile(this string s)
        {
            if (s.ExistsFile())
                File.Delete(s);

            return s;
        }

        /// <summary>
        /// 目录是否存在
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool ExistsDir(this string s)
        {
            return Directory.Exists(s);
        }

        /// <summary>
        /// 文件是否存在
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool ExistsFile(this string s)
        {
            return File.Exists(s);
        }

        /// <summary>
        /// 修改文件名 保留扩展名
        /// </summary>
        /// <param name="s"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string ChangeFileName(this string s, string fileName)
        {
            return s.Combine(fileName.Combine(s.GetExtension()));
        }

        /// <summary>
        /// 获取扩展名
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string GetExtension(this string s)
        {
            return Path.GetExtension(s);
        }

        /// <summary>
        /// 获取不带扩展名的文件名
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string GetFileNameWithoutExtension(this string s)
        {
            return Path.GetFileNameWithoutExtension(s);
        }

        /// <summary>
        /// 获取文件名
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string GetFileName(this string s)
        {
            return Path.GetFileName(s);
        }

        /// <summary>
        /// 是否包括扩展名
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool HasExtension(this string s)
        {
            return Path.HasExtension(s);
        }

        /// <summary>
        /// 查找以一定前缀开始的文件
        /// 按文件名的升序返回文件路径
        /// </summary>
        /// <param name="s">查找目录</param>
        /// <param name="startWith">文件前缀</param>
        /// <returns>返回找的文件的路径</returns>
        public static IEnumerable<string> SearchFiles(string s, string startWith)
        {
            return s.SearchFiles(startWith, true);
        }

        /// <summary>
        /// 查找以一定前缀开始的文件
        /// </summary>
        /// <param name="s">查找目录</param>
        /// <param name="startWith">文件前缀</param>
        /// <param name="ascending">是否按升序返回文件</param>
        /// <returns>返回找的文件的路径</returns>
        public static IEnumerable<string> SearchFiles(this string s, string startWith, bool ascending)
        {
            string[] files = Directory.GetFiles(s, startWith.Combine("*"));

            return ascending ? files.Sort() : files.SortDescending();
        }

        /// <summary>
        /// 获取指定路径下的文件
        /// </summary>
        /// <param name="s"></param>
        /// <param name="searchPattern"></param>
        /// <returns></returns>
        public static string[] GetFiles(this string s, string searchPattern)
        {
            return Directory.GetFiles(s, searchPattern);
        }

        /// <summary>
        /// 获取指定路径下的所有文件
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string[] GetFiles(this string s)
        {
            return Directory.GetFiles(s);
        }

        /// <summary>
        /// 清理指定路径下的空目录
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string ClearEmptyDir(this string s)
        {
            IEnumerable<string> children = s.GetDirectories();

            foreach (string i in children)
            {
                if (i.IsEmptyDir())
                    i.DeleteDir();
                else
                    i.ClearEmptyDir();
            }

            return s;
        }

        /// <summary>
        /// 删除目录、文件及子目录
        /// </summary>
        /// <param name="s"></param>
        /// <param name="deleteAll"></param>
        /// <returns></returns>
        public static string DeleteDir(this string s, bool deleteAll)
        {
            if (deleteAll)
            {
                foreach (string i in s.GetFiles())
                    i.DeleteFile();

                foreach (string i in s.GetDirectories())
                {
                    if (i.IsEmptyDir())
                        i.DeleteDir();
                    else
                        i.DeleteDir(true);
                }
            }

            s.DeleteDir();

            return s;
        }

        /// <summary>
        /// 删除空目录
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string DeleteDir(this string s)
        {
            Directory.Delete(s);

            return s;
        }

        /// <summary>
        /// 是否空目录
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsEmptyDir(this string s)
        {
            return s.GetFiles().IsEmpty() && s.GetDirectories().IsEmpty();
        }

        /// <summary>
        /// 创建目录
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string CreateDir(this string s)
        {
            Directory.CreateDirectory(s);

            return s;
        }

        /// <summary>
        /// 获取文件夹名
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string GetFolderName(this string s)
        {
            return new DirectoryInfo(s).Name;
        }

        /// <summary>
        /// 获取目录
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string GetDirectoryName(this string s)
        {
            return Path.GetDirectoryName(s);
        }

        /// <summary>
        /// 获取指定路径下的所有目录
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetDirectories(this string s)
        {
            return Directory.GetDirectories(s);
        }

        /// <summary>
        /// 更改扩展名
        /// </summary>
        /// <param name="s"></param>
        /// <param name="extension"></param>
        /// <returns></returns>
        public static string ChangeExtension(this string s, string extension)
        {
            return Path.ChangeExtension(s, extension);
        }

        /// <summary>
        /// 连接文件路径
        /// </summary>
        /// <param name="s"></param>
        /// <param name="paths"></param>
        /// <returns></returns>
        public static string CombinePath(this string s, params object[] paths)
        {
            return s.CombinePath(paths.Select(i => i.ToString()).ToArray());
        }

        /// <summary>
        /// 连接文件路径
        /// </summary>
        /// <param name="s"></param>
        /// <param name="paths"></param>
        /// <returns></returns>
        public static string CombinePath(this string s, params string[] paths)
        {
            foreach (string i in paths)
                s = Path.Combine(s, i);

            return s;
        }
    }
}