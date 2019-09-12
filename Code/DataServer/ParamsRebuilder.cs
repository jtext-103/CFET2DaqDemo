using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataServer
{
    public class ParamsRebuilder
    {
        /// <summary>
        /// </summary>
        /// <param name="dataFilePath">文件完整路径</param>
        /// <param name="dataFileDirectory">文件所在目录</param>
        /// <param name="dataFileName">无后缀文件名</param>
        /// <param name="nChannel">通道号</param>
        /// <returns></returns>
        public int SetParams(string dataFilePath, ref string dataFileDirectory, ref string dataFileName, out int nChannel)
        {
            dataFilePath = TranslataHttpSeparator(dataFilePath);
            int nFoundBackslastIndex = -1;
            string directory;
            string path;
            string fileName;
            nChannel = -1;
            while ((nFoundBackslastIndex = dataFilePath.IndexOf(System.IO.Path.DirectorySeparatorChar, nFoundBackslastIndex + 1)) != -1)
            {
                path = dataFilePath.Substring(0, nFoundBackslastIndex);
                if (path == "") continue;
                if ((directory = Path.GetDirectoryName(path)) != null && (directory = Path.GetDirectoryName(path)) != " ")
                {
                    fileName = Path.GetFileNameWithoutExtension(path);
                    DirectoryInfo root = new DirectoryInfo(directory);
                    foreach (FileInfo fileinfo in root.GetFiles())
                    {
                        if (fileName == Path.GetFileNameWithoutExtension(fileinfo.Name))
                        {
                            dataFileDirectory = directory + System.IO.Path.DirectorySeparatorChar;
                            dataFileName = fileName;
                            nChannel = int.Parse(dataFilePath.Substring(nFoundBackslastIndex + 1));
                            return 0;
                        }
                    }
                    if (Directory.Exists(path) == false) return -1;
                }
            }
            return -1;
        }

        private string TranslataHttpSeparator(string originPath, char httpSeparator = '.')
        {
            return originPath.Replace(httpSeparator, System.IO.Path.DirectorySeparatorChar);
        }
    }
}
