using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Jtext103.CFET2.Things.DAQDataUploadThing
{
    public class FileOperator
    {
        /// <summary>
        /// 将目标文件下此文件名的文件加上时间戳重命名
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="targetDirectory"></param>
        public static void RenameExistedFile(string fileName, string targetDirectory)
        {
            //先将和源文件夹中同名的文件全部重命名
            if (File.Exists(targetDirectory + fileName))
            {
                var index = fileName.LastIndexOf('.');
                var newName = fileName.Substring(0, index) + "-" 
                                + DateTime.Now.ToLocalTime().ToString("yyyyMMddHHmmss") 
                                + fileName.Substring(index, fileName.Length - index);
                File.Move(targetDirectory + fileName, targetDirectory + newName);
            }
        }
    }
}
