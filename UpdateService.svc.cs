using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.IO;
using System.Xml;

namespace Update
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class UpdateService : IUpdateService
    { 

        private List<FileInfomation> ReadLoadUpdateConfig()
        {
            var files = new List<FileInfomation>();
            // 获取程序的基目录。
            var basepath = System.AppDomain.CurrentDomain.BaseDirectory;
            var xmldoc = new XmlDocument();
            xmldoc.Load(updateFolderPath + "UpdateFiles.xml");
            var nodes = xmldoc.SelectNodes("root/UpdateFiles/File");
            foreach (XmlElement node in nodes)
            {
                var fileinfo = new FileInfomation();
                fileinfo.FilePath = node.GetAttribute("Path");
                fileinfo.Version = node.GetAttribute("Version");
                fileinfo.Type = node.GetAttribute("Type");
                fileinfo.FileName = node.InnerText;

                files.Add(fileinfo);
            }

            return files;
        }

        private string updateFolderPath = ConfigurationManager.AppSettings["UpdateFilesPath"]; //后台服务地址

        public CompositeType GetDataUsingDataContract(CompositeType composite)
        {
            if (composite == null)
            {
                throw new ArgumentNullException("composite");
            }
            if (composite.BoolValue)
            {
                composite.StringValue += "Suffix";
            }
            return composite;
        }



        public bool Update(ref List<FileInfomation> clientfiles)
        {
            try
            {
                var serverconfigs = ReadLoadUpdateConfig();
                var updatefiles = new List<FileInfomation>();

                //获取需要更新的不同版本文件
                foreach (var file in serverconfigs)
                {

                    if (file.Type == "Folder")
                    {
                        if (clientfiles.Exists(m => m.FilePath == file.FilePath && m.FileName == file.FileName))
                        {
                            var fileInfomation = clientfiles.First(m =>
                                m.FilePath == file.FilePath && m.FileName == file.FileName);
                            if (fileInfomation.Version != file.Version)
                            {
                                var files = FileInfomation.GetAllFiles(Path.Combine(updateFolderPath, file.FileName));
                                foreach (var updatefile in files)
                                {
                                    updatefile.FilePath = Path.Combine(fileInfomation.FileName, updatefile.FilePath);
                                    updatefiles.Add(updatefile);
                                }
                            }
                        }
                        else
                        {
                            var folder = Path.Combine(updateFolderPath, file.FileName);
                            var files = FileInfomation.GetAllFiles(folder);
                            foreach (var updatefile in files)
                            {
                                updatefile.FilePath = Path.Combine(file.FileName, updatefile.FilePath);
                                updatefiles.Add(updatefile);
                            }
                        }

                    }

                    else
                    {
                        if (clientfiles.Exists(m => m.FilePath == file.FilePath && m.FileName == file.FileName))
                        {
                            var fileInfomation = clientfiles.First(m =>
                                m.FilePath == file.FilePath && m.FileName == file.FileName);
                            if (fileInfomation.Version != file.Version)
                            {
                                updatefiles.Add(file);
                            }
                        }
                        else
                        {
                            updatefiles.Add(file);
                        }
                    }

                }


                //判断是否更新updateFile.xml
                if (updatefiles.Count > 0)
                {
                    var configfile = new FileInfomation();
                    configfile.FileName = "UpdateFiles.xml";
                    updatefiles.Add(configfile);
                }

                //获取文件内容
                foreach (var file in updatefiles)
                {
                    Log.Info(file.FilePath);
                    Log.Info(file.FileName);

                    var filepath = Path.Combine(updateFolderPath, file.FilePath, file.FileName);

                    using (FileStream fs = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        var filebytes = new byte[fs.Length];
                        fs.Read(filebytes, 0, filebytes.Length);
                        file.Filebody = filebytes;
                    }
                }


                clientfiles.Clear();
                clientfiles = updatefiles;
            }
            catch (Exception ex)
            {
                Log.Error("获取更新文件错误", ex);
                return false;
            }
            return true;
        }

        public bool UpdateByMd5(ref List<FileInfomation> clientfiles)
        {
            try
            {
                var updatefiles = FileInfomation.GetAllFilesWithMd5(updateFolderPath);
                if (updatefiles.Count != 0)
                {

                    for (var i = 0; i < updatefiles.Count; i++)
                    {
                        if (updatefiles[i].FieldMd5 != string.Empty)
                        {
                            var clientfile = clientfiles.Find(m => m.FieldMd5 == updatefiles[i].FieldMd5);
                            if (clientfile != null)
                            {
                                updatefiles.RemoveAt(i);
                                i--;
                            }
                        }
                    }

                    //判断是否更新updateFile.xml
                    if (updatefiles.Count > 0)
                    {
                        var configfile = new FileInfomation();
                        configfile.FileName = "UpdateFiles.xml";
                        updatefiles.Add(configfile);
                    }

                    //获取文件内容
                    foreach (var file in updatefiles)
                    {
                        Log.Info(file.FilePath);
                        Log.Info(file.FileName);

                        var filepath = Path.Combine(updateFolderPath, file.FilePath, file.FileName);

                        using (FileStream fs = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.Read))
                        {
                            var filebytes = new byte[fs.Length];
                            fs.Read(filebytes, 0, filebytes.Length);
                            file.Filebody = filebytes;
                        }
                    }


                    clientfiles.Clear();
                    clientfiles = updatefiles;
                }
            }
            catch (Exception ex)
            {
                Log.Error("获取更新文件错误", ex);
                return false;
            }
            return true;
        }
    }
}
