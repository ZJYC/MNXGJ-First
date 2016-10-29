using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace ZJYC_CS_DIR
{
    public class DirOperate
    {
        //当前路径
        String CurPath = Environment.CurrentDirectory;
        /// <summary>
        /// 通过时间得到文件夹路径
        /// </summary>
        /// <param name="CurTime"></param>
        /// <returns></returns>
        public String Date2String(DateTime CurTime)
        {
            String DirName = 
                CurPath + "\\" +
                CurTime.Year.ToString("D4") + "--" +
                CurTime.Month.ToString("D2") + "--" +
                CurTime.Day.ToString("D2") + "--" +
                CurTime.Hour.ToString("D2") + "--" +
                CurTime.Minute.ToString("D2") + "--" +
                CurTime.Second.ToString("D2");
            return DirName;
        }
        /// <summary>
        /// 根据当前时间创建一个文件夹
        /// </summary>
        /// <returns></returns>日期字符串
        public String CreateDirAccordingCurDate()
        {
            DateTime CurTime = DateTime.Now;
            String path = Date2String(CurTime);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return path;
        }
        /// <summary>
        /// copy file according to Source and Dest
        /// </summary>
        /// <param name="sourceDirectory"></param>
        /// <param name="destDirectory"></param>
        public void CopyDirectory(string sourceDirectory, string destDirectory)
        {
            if (sourceDirectory == "" || destDirectory == "")
            {
                MessageBox.Show("路径为空");
                return;
            }
            //判断源目录和目标目录是否存在，如果不存在，则创建一个目录
            if (!Directory.Exists(sourceDirectory))
            {
                Directory.CreateDirectory(sourceDirectory);
            }
            if (!Directory.Exists(destDirectory))
            {
                Directory.CreateDirectory(destDirectory);
            }
            //拷贝文件
            CopyFile(sourceDirectory, destDirectory);

            //拷贝子目录       
            //获取所有子目录名称
            string[] directionName = Directory.GetDirectories(sourceDirectory);

            foreach (string directionPath in directionName)
            {
                //根据每个子目录名称生成对应的目标子目录名称
                string directionPathTemp = destDirectory + "\\" + directionPath.Substring(sourceDirectory.Length + 1);

                //递归下去
                CopyDirectory(directionPath, directionPathTemp);
            }
        }
        public static void CopyFile(string sourceDirectory, string destDirectory)
        {
            //获取所有文件名称
            string[] fileName = Directory.GetFiles(sourceDirectory);

            foreach (string filePath in fileName)
            {
                //根据每个文件名称生成对应的目标文件名称
                string filePathTemp = destDirectory + "\\" + filePath.Substring(sourceDirectory.Length + 1);

                //若不存在，直接复制文件；若存在，覆盖复制
                if (File.Exists(filePathTemp))
                {
                    File.Copy(filePath, filePathTemp, true);
                }
                else
                {
                    File.Copy(filePath, filePathTemp);
                }
            }
        }
        /// <summary>
        /// convert string to Date according to the special format
        /// </summary>
        /// <param name="Dir"></param>
        /// <returns></returns>
        public DateTime StrToTime(String Dir)
        {
            int Second, Minute, Hour, Day, Month, Year;
            List<String> Result = Regex.Split(Dir, "--", RegexOptions.IgnoreCase).ToList();

            string StringTemp = Result[5] == null ? "" : Result[5];
            Second= int.Parse(StringTemp);
            StringTemp = Result[4] == null ? "" : Result[4];
            Minute = int.Parse(StringTemp);
            StringTemp = Result[3] == null ? "" : Result[3];
            Hour = int.Parse(StringTemp);
            StringTemp = Result[2] == null ? "" : Result[2];
            Day = int.Parse(StringTemp);
            StringTemp = Result[1] == null ? "" : Result[1];
            Month = int.Parse(StringTemp);
            StringTemp = Result[0] == null ? "" : Result[0];
            Year = int.Parse(StringTemp);
            DateTime Time = new DateTime(Year, Month, Day, Hour, Minute, Second);
            return Time;
        }
        /// <summary>
        /// search the lated dir
        /// </summary>
        /// <param name="TimeLimit"></param>
        /// <returns></returns>日期字符串
        public String FindLatedDir(int TimeLimit)
        {
            DateTime CurTime = DateTime.Now;
            List<string> DirInf =  Directory.GetDirectories(CurPath).ToList();
            TimeSpan Min = CurTime - CurTime.AddMonths((-1) * TimeLimit);
            String MinString = "";
            foreach (string item in DirInf)
            {
                String Temp = item.Substring(item.Length - 24, 24);
                DateTime TimeTemp_1 = StrToTime(Temp);
                if (OutOfDate(TimeTemp_1,CurTime, TimeLimit * 30) == true)
                {
                    return "";
                }
                TimeSpan TS =  CurTime - TimeTemp_1;
                if (Min > TS)
                {
                    Min = TS;
                    MinString = Temp;
                }
            }
            return MinString;
        }
        /// <summary>
        /// check if the date had out of date
        /// </summary>
        /// <param name="CurTime_1"></param>
        /// <param name="CurTime_2"></param>
        /// <param name="DayLimit"></param>
        /// <returns></returns>
        public bool OutOfDate(DateTime CurTime_1,DateTime CurTime_2,long DayLimit)
        {
            long Day_1 = CurTime_1.Year * 365 + CurTime_1.Month * 30 + CurTime_1.Day;
            long Day_2 = CurTime_2.Year * 365 + CurTime_2.Month * 30 + CurTime_2.Day;
            if(System.Math.Abs(Day_1 - Day_2) >= DayLimit)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    
}
