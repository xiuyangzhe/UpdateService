﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Web;

namespace Update
{
    public sealed class FileInfomation
    {

        private string _fieldMd5 = string.Empty;
        private string _fileName = string.Empty;
        private string _filePath = string.Empty;
        private byte[] _filebody;
        private string _version = string.Empty;

        /// <summary>
        /// 文件MD5
        /// </summary>
        public string FieldMd5 { get => _fieldMd5; set => _fieldMd5 = value; }

        /// <summary>
        /// 文件名称
        /// </summary>
        public string FileName { get => _fileName; set => _fileName = value; }

        /// <summary>
        /// 文件内容
        /// </summary>
        public byte[] Filebody { get => _filebody; set => _filebody = value; }

        /// <summary>
        /// 文件相对路径
        /// </summary>
        public string FilePath { get => _filePath; set => _filePath = value; }

        /// <summary>
        /// 更新文件版本号
        /// </summary>
        public string Version { get => _version; set => _version = value; }

        /// <summary>
        /// 更新类型 File,Folder
        /// </summary>
        public string Type { get; set; } = String.Empty;

        /// <summary>
        /// 获取文件MD5值
        /// </summary>
        /// <param name="fileName">文件绝对路径</param>
        /// <returns>MD5值</returns>
        public static string GetMD5HashFromFile(string fileName)
        {
            try
            {
                using (var file = new FileStream(fileName, FileMode.Open))
                {
                    System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                    byte[] retVal = md5.ComputeHash(file);
                    file.Close();

                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < retVal.Length; i++)
                    {
                        sb.Append(retVal[i].ToString("x2"));
                    }

                    return sb.ToString();
                }

            }
            catch (Exception ex)
            {
                Log.Error("GetMD5HashFromFile() fail,error:" + ex.Message, ex);
            }

            return "";
        }

        private static object lockobj = new object();

        /// <summary>
        /// 获取指定文件信息，包括相对路径，md5，文件名,文件整体
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static List<FileInfomation> GetAllFiles(string path)
        {
            var files = new List<FileInfomation>();
            //lock (lockobj)
            //{
            var dirinfo = new DirectoryInfo(path);
            var fileinfos = new List<FileInfo>();
            GetFiles(dirinfo, ref fileinfos);
            foreach (var item in fileinfos)
            {
                var fileinfo = new FileInfomation();
                //fileinfo.FieldMd5 = GetMD5HashFromFile(item.FullName);
                //if (fileinfo.FieldMd5 == string.Empty)
                //    continue;
                fileinfo.FilePath = item.DirectoryName.Replace(path, "").Trim('\\');
                fileinfo.FileName = item.Name;
                //fileinfo.Filebody = File.ReadAllBytes(item.FullName);

                files.Add(fileinfo);
            }
            //}

            return files;
        }


        /// <summary>
        /// 获取指定文件信息，包括相对路径，md5，文件名,文件整体
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static List<FileInfomation> GetAllFilesWithMd5(string path)
        {
            var files = new List<FileInfomation>();
            //lock (lockobj)
            //{
            var dirinfo = new DirectoryInfo(path);
            var fileinfos = new List<FileInfo>();
            GetFiles(dirinfo, ref fileinfos);
            foreach (var item in fileinfos)
            {
                var fileinfo = new FileInfomation();
                fileinfo.FieldMd5 = GetMD5HashFromFile(item.FullName);
                if (fileinfo.FieldMd5 == string.Empty)
                    continue;
                fileinfo.FilePath = item.DirectoryName.Replace(path, "").Trim('\\');
                fileinfo.FileName = item.Name;
                //fileinfo.Filebody = File.ReadAllBytes(item.FullName);

                files.Add(fileinfo);
            }
            //}

            return files;
        }

        /// <summary>
        /// 获取文件信息，遍历子文件夹
        /// </summary>
        /// <param name="dirinfo"></param>
        /// <param name="files"></param>
        /// <returns></returns>
        public static List<FileInfo> GetFiles(DirectoryInfo dirinfo, ref List<FileInfo> files)
        {
            var _files = dirinfo.GetFiles();
            var _dirs = dirinfo.GetDirectories();
            foreach (var item in _files)
            {
                files.Add(item);
            }

            foreach (var item in _dirs)
            {
                GetFiles(item, ref files);
            }

            return files;

        }
    }
}