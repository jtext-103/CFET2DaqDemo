using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jtext103.CFET2.Things.ShotDirOperate
{
    public static class ShotDirOperator
    {
        /// <summary>
        /// 找到最新炮（不一定采集完成）
        /// 读取上层目录中所有文件夹名，找到最大文件名数字
        /// 当没有子文件夹或者均不为数字时，返回0
        /// </summary>
        /// <param name="parentDirectory"></param>
        /// <returns></returns>
        public static int FindMaxShotNo(string parentDirectory)
        {
            List<int> childNames = childDirectoryNames(parentDirectory);
            if (childNames.Count <= 0)
            {
                return 0;
            }
            else
            {
                return childNames.Last();
            }
        }

        /// <summary>
        /// 找到已经采集完成的最新炮
        /// 读取上层目录中所有文件夹名，找到最大文件名数字
        /// 当没有子文件夹或者均不为数字时，返回0
        /// </summary>
        /// <param name="parentDirectory"></param>
        /// <returns></returns>
        public static int FindMaxCompletedShotNo(string parentDirectory)
        {
            List<int> childNames = childDirectoryNames(parentDirectory);
            int childDirectoryCount = childNames.Count;
            if (childDirectoryCount <= 0)
            {
                return 0;
            }
            else
            {
                for (int i = 0; i < childDirectoryCount; i++)
                {
                    //（从大到小顺序）当前检查到的文件夹
                    int nowCount = childDirectoryCount - 1 - i;
                    return childNames[nowCount];
                }
                return 0;
            }
        }

        /// <summary>
        /// 读取上层目录中所有文件夹名，按顺序排序子以数字命名的子文件夹名，忽略非数字子文件夹
        /// </summary>
        /// <param name="parentDirectory"></param>
        /// <returns></returns>
        private static List<int> childDirectoryNames(string parentDirectory)
        {
            //目录完整路径
            string[] directoryPath = Directory.GetDirectories(parentDirectory);
            //该目录下所有数据子文件夹名
            List<int> childDataDirectoryNames = new List<int>();
            foreach (string childDirectory in directoryPath)
            {
                //子文件夹名
                int name;
                if (int.TryParse(Path.GetFileName(childDirectory), out name))
                {
                    childDataDirectoryNames.Add(name);
                }
            }

            //顺序排序
            childDataDirectoryNames.Sort();
            return childDataDirectoryNames;
        }

        /// <summary>
        /// 读取上层目录中所有文件夹名，并按从小到大数字规则新建下一炮的文件夹
        /// </summary>
        /// <param name="parentDirectory">上层目录路径</param>
        /// <returns>新建的文件夹名称</returns>
        public static string CreateNextDirectory(string parentDirectory)
        {
            //如果还没有该目录，则创建
            if (!Directory.Exists(parentDirectory))
            {
                Directory.CreateDirectory(parentDirectory);
            }
            //新建的目录完整路径
            string nextDirectoryPath;
            //新建的目录名
            string nextDirectoryName = (FindMaxCompletedShotNo(parentDirectory) + 1).ToString();

            nextDirectoryPath = parentDirectory + "\\" + nextDirectoryName;
            Directory.CreateDirectory(nextDirectoryPath);
            return nextDirectoryPath;
        }

        /// <summary>
        /// 删除parentDirectory路径下多余的数据文件夹，及所有其他类型文件夹
        /// 数据文件夹是以纯数字命名，否则就是其他类型文件夹
        /// 至少会保留最新reservedCount个数的数据文件夹
        /// <param name="parentDirectory">目录路径</param>
        /// <param name="deleteCount">删除数据文件夹数，0代表尽量删除所有数据，至少也会保留最新reservedCount个数的数据文件夹;如果总数据文件夹数小于deleteCount，则不删除</param>
        /// <param name="reservedCount">保留数据文件夹数</param>
        /// <returns>真正删除的文件夹数（只统计数据文件夹）</returns>
        public static int DeleteRedundantDataDirectories(string parentDirectory, uint deleteCount, uint reservedCount = 2)
        {
            //如果还没有该目录，则创建
            if (!Directory.Exists(parentDirectory))
            {
                Directory.CreateDirectory(parentDirectory);
            }

            //目录完整路径
            string[] directoryPath = Directory.GetDirectories(parentDirectory);
            //该目录下所有数据子文件夹名
            List<int> childDataDirectoryNames = new List<int>();
            foreach (string childDirectory in directoryPath)
            {
                //子文件夹名
                int name;
                if (int.TryParse(Path.GetFileName(childDirectory), out name))
                {
                    childDataDirectoryNames.Add(name);
                }
                else
                {
                    //不是数据文件夹，直接删除
                    Directory.Delete(childDirectory, true);
                }
            }
            //所有数据文件夹个数
            int allCount = childDataDirectoryNames.Count();
            //应删除个数
            int shouldDelete;
            

            if (deleteCount == 0)
            {
                //全删
                shouldDelete = Convert.ToInt32(allCount - reservedCount);
            }
            else
            {
                if((allCount - reservedCount) < deleteCount)
                {
                    //如果总数据文件夹数小于deleteCount，则不删除
                    return 0;
                }
                else
                {
                    shouldDelete = Convert.ToInt32(deleteCount);
                }                
            }
            //文件夹名按顺序排序，依次删除
            childDataDirectoryNames.Sort();
            //真正删除个数
            int actualDelete = 0;
            //删除actualDelete个数据文件夹
            for (int i = 0; i < shouldDelete; i++)
            {
                Directory.Delete(parentDirectory + "\\" + childDataDirectoryNames[i], true);
                actualDelete++;
            }
            return actualDelete;
        }
    }
}