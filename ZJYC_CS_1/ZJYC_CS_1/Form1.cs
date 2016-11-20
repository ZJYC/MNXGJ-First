using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZJYC_CS_DIR;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace ZJYC_CS_1
{
    [Serializable]
    struct ConfigInf//配置信息
    {
        public String CWD;//工程文件路径
        public String LastWork;//上一次工作路径
        public bool OpenProject;//打开工程
        public bool OpenSourcefile;//打开源文件
        public List<String> FileName;//源文件目录
    }
    public struct ResultBinary
    {
        public bool Result;
        public String Inf;
    }
    //[Serializable]
    //struct SourceFileArray
    //{
    //    public List<String> FileName;
    //}
    public partial class Form1 : Form
    {
        String Str_CreateNewProject = "已经创建新工程：";
        String Str_Config = "Config.Txt";
        String Str_LogFile = "Log.txt";
        string Lated_Dir = "";
        String CurPath = Environment.CurrentDirectory;
        ConfigInf Config = new ConfigInf();
        ResultBinary resultBinary = new ResultBinary();
        ZJYC_CS_DIR.DirOperate DirOperate_Temp = new ZJYC_CS_DIR.DirOperate();
        public Form1()
        {
            InitializeComponent();
            toolStripStatusLabel1.Text = DateTime.Now.ToLongDateString() + "  "+ DateTime.Now.ToLongTimeString();
            textBox3.Text = CurPath;
            MessageBox.Show("ZJYC：本程序目录下不允许存在非‘XXXX--XX--XX--XX--XX--XX’格式的文件夹，否则出错，切记");
            Initial();
        }
        public void Initial()
        {
            if (File.Exists(CurPath + "\\" + Str_Config))
            {
                Config = (ConfigInf)ReadBinary(Str_Config, ref resultBinary);
            }
            else
            {
                resultBinary.Result = false;
            }
            if (resultBinary.Result == false || Config.CWD == "")
            {
                MessageBox.Show("配置文件异常，请稍后创建");
            }
            checkBox1.Checked = Config.OpenProject;
            checkBox2.Checked = Config.OpenSourcefile;
            Lated_Dir = DirOperate_Temp.FindLatedDir(int.Parse(textBox1.Text));
            if (Lated_Dir != "")
            {
                Config.LastWork = Lated_Dir;
                toolStripStatusLabel2.Text = Lated_Dir;
            }
        }
        public void WriteLog(string text)
        {
            String TimeInf = "\n\n" + DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLongTimeString() 
            + "\n\n";
            FileStream fs = new FileStream(CurPath + "\\" + Config.LastWork+ "\\" + Str_LogFile, FileMode.Append);
            StreamWriter sw = new StreamWriter(fs, Encoding.Default);
            TimeInf += text + "#";
            sw.Write(TimeInf);
            sw.Close();
            fs.Close();
        }
        public void DisplayHelp(String Inf)
        {
            toolStripStatusLabel2.Text = Inf;
        }
        public void WriteBinary(object obj, string file,ref ResultBinary resultBinary)
        {
            try
            {
                String Path = CurPath + "\\" + file;
                using (Stream input = File.OpenWrite(Path))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(input, obj);
                    resultBinary.Result= true;
                    resultBinary.Inf = "写入成功";
                }
            }
            catch
            {
                resultBinary.Inf = "写入失败";
                resultBinary.Result = false;
            }
        }
        public object ReadBinary(string file, ref ResultBinary resultBinary)
        {
            String Path = CurPath + "\\" + file;
            try
            {
                using (Stream outPut = File.OpenRead(Path))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    object user = bf.Deserialize(outPut);
                    if (user != null)
                    {
                        resultBinary.Inf = "读取成功";
                        resultBinary.Result = true;
                        return user;
                    }
                }
            }
            catch
            {
                resultBinary.Inf = "读取失败";
                resultBinary.Result = false;
            }
            return null;
        }
        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void button_New_Project_Click(object sender, EventArgs e)
        {
            Config.LastWork = DirOperate_Temp.CreateDirAccordingCurDate();
            Config.LastWork = Config.LastWork.Substring(CurPath.Length + 1, 24);
            WriteBinary(Config, Str_Config, ref resultBinary);
            MessageBox.Show(Str_CreateNewProject + Config.LastWork);
        }

        private void statusStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = DateTime.Now.ToLongDateString() + "  " + DateTime.Now.ToLongTimeString();
        }

        private void button_NewDay_Click(object sender, EventArgs e)
        {
            string Path_S = CurPath + "\\" + Config.LastWork;
            if (Config.LastWork == "")
            { 
                MessageBox.Show("日期超过限制--" + textBox1.Text + "月" + "或者本目录下文件夹为空");
                return;
            }
            String Path_D = DirOperate_Temp.CreateDirAccordingCurDate();
            DirOperate_Temp.CopyDirectory(Path_S,Path_D);
            Config.LastWork = Path_D.Substring(CurPath.Length+1,24);
            WriteBinary(Config, Str_Config, ref resultBinary);
            String Date_S = Path_S.Substring(Path_S.Length - 24, 24);
            String Date_D = Path_D.Substring(Path_D.Length - 24, 24);
            MessageBox.Show("Copy Dir From " + Date_S + " to " + Date_D);
            if(checkBox1.Checked == true)
            {
                Config = (ConfigInf)ReadBinary(Str_Config, ref resultBinary);
                String Path = CurPath + "\\" + Config.LastWork + "\\" + Config.CWD;
                RunFile(Path);
                Config.OpenProject = true;
            }
            if(checkBox2.Checked == true)
            {
                OpenSource();
                Config.OpenSourcefile = true;
            }
            WriteBinary(Config, Str_Config, ref resultBinary);
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button_NewConfig_Click(object sender, EventArgs e)
        {

        }

        private void button_NewConfig_Click_1(object sender, EventArgs e)
        {
            openFileDialog1.InitialDirectory = CurPath;
            if(openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string FileName = this.openFileDialog1.FileName;
                int Length = FileName.Length - CurPath.Length - 26;
                int Start = CurPath.Length + 26;
                Config.CWD = FileName.Substring(Start, Length);
                WriteBinary(Config, Str_Config, ref resultBinary);
            }
        }
        public void CreateConfigFile()
        {
            LOOP_CTEATE:
            openFileDialog1.InitialDirectory = CurPath;
            if(openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string FileName = this.openFileDialog1.FileName;
                FileName = FileName.Substring(FileName.Length - (FileName.Length - CurPath.Length - 26), (FileName.Length - CurPath.Length - 26));
                Config.CWD = FileName;
                WriteBinary(Config, Str_Config,ref resultBinary);
                if (resultBinary.Result == false) goto LOOP_CTEATE;
            }
        }
        private void button_Countinue_Click(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                RunFile(CurPath + "\\" + Config.LastWork + "\\" + Config.CWD);
            }
            if (checkBox2.Checked == true)
            {
                OpenSource();
            }
            WriteBinary(Config, Str_Config, ref resultBinary);
        }

        private void button_ReadConfig_Click(object sender, EventArgs e)
        {
            Config = (ConfigInf)ReadBinary(Str_Config,ref resultBinary);
            textBox2.Text = Config.CWD;
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            String[] FileNames = new String[] { "" };
            openFileDialog1.Multiselect = true;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                FileNames = openFileDialog1.FileNames;
                for (int i = 0; i < FileNames.Length; i++)
                {
                    int Length = FileNames[i].Length - CurPath.Length - 26;
                    int Start = CurPath.Length + 26;
                    FileNames[i] = FileNames[i].Substring(Start, Length);
                    Config.FileName.Add(FileNames[i]);
                }
                WriteBinary(Config, Str_Config, ref resultBinary);
                MessageBox.Show("添加源文件成功");
                
            }

        }
        public void OpenSource()
        {
            Config = (ConfigInf)ReadBinary(Str_Config, ref resultBinary);
            foreach (String File in Config.FileName)
            {
                RunFile(CurPath + "\\" + Config.LastWork + "\\" + File);
            }
        }

        public void RunFile(String Path)
        {
            try
            {
                System.Diagnostics.Process.Start(Path);
            }
            catch
            {
                MessageBox.Show("没有发现要运行的文件");
            }
        }
        private void button2_Click_1(object sender, EventArgs e)
        {
            OpenSource();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            MessageBox.Show("一个叫赵京阳春的家伙编的");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            String[] FileNames = new String[] { "" };
            openFileDialog1.Multiselect = true;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                //Config.FileName.Clear();
                FileNames = openFileDialog1.FileNames;
                int Count = 0;
                foreach (string Temp_Str in FileNames)
                {
                    int Length = Temp_Str.Length - CurPath.Length - 26;
                    int Start = CurPath.Length + 26;
                    FileNames[Count++] = Temp_Str.Substring(Start, Length);
                }
                Config.FileName = FileNames.ToList();
                WriteBinary(Config, Str_Config,ref resultBinary);
                MessageBox.Show("源文件写入成功");
            }

        }

        private void button_New_Project_MouseEnter(object sender, EventArgs e)
        {
            DisplayHelp("新建一个空的日期文件夹");
        }

        private void button_NewDay_MouseEnter(object sender, EventArgs e)
        {
            DisplayHelp("将之前的工程复制到新生成的文件夹里面");
        }

        private void button_Countinue_MouseEnter(object sender, EventArgs e)
        {
            DisplayHelp("打开最近的工程");
        }

        private void button3_MouseEnter(object sender, EventArgs e)
        {
            DisplayHelp("作者信息");
        }

        private void button4_MouseEnter(object sender, EventArgs e)
        {
            DisplayHelp("创建一个源文件目录的记录");
        }

        private void button1_MouseEnter(object sender, EventArgs e)
        {
            DisplayHelp("向记录添加源文件");
        }

        private void button2_MouseEnter(object sender, EventArgs e)
        {
            DisplayHelp("依据记录打开所有源文件");
        }

        private void button_NewConfig_MouseEnter(object sender, EventArgs e)
        {
            DisplayHelp("创建工程文件的记录");
        }

        private void button_ReadConfig_MouseEnter(object sender, EventArgs e)
        {
            DisplayHelp("依据记录打开工程");
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            WriteLog(textBox4.Text);//Str_LogFile
            MessageBox.Show("日志写入成功");
        }

        private void button6_Click(object sender, EventArgs e)
        {
            RunFile(CurPath + "\\" +Config.LastWork + "\\" + Str_LogFile);
        }

        private void tabControl1_DrawItem(object sender, DrawItemEventArgs e)
        {
            SolidBrush blue = new SolidBrush(Color.Blue);
            SolidBrush red = new SolidBrush(Color.Red);
            SolidBrush green = new SolidBrush(Color.Green);
            SolidBrush black = new SolidBrush(Color.Black);
            SolidBrush OrangeRed = new SolidBrush(Color.OrangeRed);
            SolidBrush Yellow = new SolidBrush(Color.Yellow);
            StringFormat stringformat = new StringFormat();
            stringformat.Alignment = StringAlignment.Center;
            Rectangle Rec1 = tabControl1.GetTabRect(0);
            e.Graphics.FillRectangle(red, Rec1);
            Rectangle Rec2 = tabControl1.GetTabRect(1);
            e.Graphics.FillRectangle(blue, Rec2);
            Rectangle Rec3 = tabControl1.GetTabRect(2);
            e.Graphics.FillRectangle(green, Rec3);
            Rectangle Rec4 = tabControl1.GetTabRect(3);
            e.Graphics.FillRectangle(OrangeRed, Rec4);
            Rectangle Rec5 = tabControl1.GetTabRect(4);
            e.Graphics.FillRectangle(Yellow, Rec5);

            for(int i = 0;i < tabControl1.TabPages.Count;i++)
            {
                Rectangle Rec = tabControl1.GetTabRect(i);
                e.Graphics.DrawString(tabControl1.TabPages[i].Text, new Font("楷体", 14, FontStyle.Bold), black, Rec, stringformat);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void button7_Click(object sender, EventArgs e)
        {
            DateTime CurTime = DateTime.Now;
            String TimeString = "";
            //长字符串，包括分秒
            if (checkBox3.Checked == true)
            {
                TimeString =
                CurTime.Year.ToString("D4") + "--" +
                CurTime.Month.ToString("D2") + "--" +
                CurTime.Day.ToString("D2") + "--" +
                CurTime.Hour.ToString("D2") + "--" +
                CurTime.Minute.ToString("D2") + "--" +
                CurTime.Second.ToString("D2");
            }
            else//不包括分秒
            {
                TimeString =
                CurTime.Year.ToString("D4") + "--" +
                CurTime.Month.ToString("D2") + "--" +
                CurTime.Day.ToString("D2") + "--" +
                CurTime.Hour.ToString("D2");
            }
            Clipboard.SetDataObject(TimeString);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            String FuncDiscribe = "";
            if (checkBox4.Checked == true)
            {
                FuncDiscribe =          "/*\n" +
                                        "****************************************************\n" +
                                        "*  函数名         : \n" +
                                        "*  函数描述       : \n" +
                                        "*  参数           : \n" +
                                        "*  返回值         : \n" +
                                        "*  作者           : -5A4A5943-\n" +
                                        "*  历史版本       : \n" +
                                        "*****************************************************\n" +
                                        "*/";
            }
            else
            {
                FuncDiscribe =          "/*\n" +
                                        "****************************************************\n" +
                                        "*  Function       : \n" +
                                        "*  Description    : \n" +
                                        "*  Params         : \n" +
                                        "*  Return         : \n" +
                                        "*  Author         : -5A4A5943-\n" +
                                        "*  History        : \n" +
                                        "*****************************************************\n" +
                                        "*/";
            }
            Clipboard.SetDataObject(FuncDiscribe);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            String SrcFile = "";
            if (checkBox5.Checked == true)
            {
                SrcFile =           "/*\n" +
                                    "****************************************************\n" +
                                    "*  文件名             : \n" +
                                    "*  作者               : -5A4A5943-\n" +
                                    "*  版本               : \n" +
                                    "*  编写日期           : \n" +
                                    "*  简介               : \n" +
                                    "*  函数列表           : \n" +
                                    "*  历史版本           : \n" +
                                    "*****************************************************\n" +
                                    "*/";
            } 
            else
            {
                SrcFile =           "/*\n" +
                                    "****************************************************\n" +
                                    "*  File name          : \n" +
                                    "*  Author             : -5A4A5943-\n" +
                                    "*  Version            : \n" +
                                    "*  Date               : \n" +
                                    "*  Description        : \n" +
                                    "*  Function List      : \n" +
                                    "*  History            : \n" +
                                    "*****************************************************\n" +
                                    "*/";
            }
            Clipboard.SetDataObject(SrcFile);
        }

        private void button10_Click(object sender, EventArgs e)
        {

        }

        private void button10_Click_1(object sender, EventArgs e)
        {
            //String Random
        }

        private void button10_Click_2(object sender, EventArgs e)
        {
            decimal Start =  numericUpDown1.Value;
            decimal End = numericUpDown2.Value;
            decimal Num = numericUpDown3.Value;
            
            String String = "";

            if (Num == 0)
            {
                MessageBox.Show("您输入的数值为0");
                return;
            }
            if (Num > 10000)
            {
                MessageBox.Show("您输入的数值大于10000");
                return;
            }

            if(Start > End)
            {
                MessageBox.Show("前面的数字应不大于后面的数字");
                return;
            }

            Random RandomTemp = new Random();

            for(UInt16 cnt = 0;cnt < Num;cnt ++)
            {
                String += RandomTemp.Next((int)Start, (int)End + 1).ToString();
                String += ",";
            }

            Clipboard.SetDataObject(String);

        }

        private void button10_MouseEnter(object sender, EventArgs e)
        {
            DisplayHelp("最大数值为10000000000000");
        }

        private void button11_Click(object sender, EventArgs e)
        {
            String SrcFile = "";
            if (checkBox6.Checked == true)
            {
                SrcFile = "/*\n" +
                          "****************************************************\n" +
                          "*  文件名             : \n" +
                          "*  作者               : -5A4A5943-\n" +
                          "*  版本               : \n" +
                          "*  编写日期           : \n" +
                          "*  简介               : \n" +
                          "*  函数列表           : \n" +
                          "*  历史版本           : \n" +
                          "*****************************************************\n" +
                          "*/" +
                          "\n\n\n/*头文件  */\n\n\n" +
                          "\n\n\n/*宏定义  */\n\n\n" +
                          "\n\n\n/*变量定义*/\n\n\n" +
                          "\n\n\n/*变量声明*/\n\n\n" +
                          "\n\n\n/*函数声明*/\n\n\n";
            }
            else
            {
                SrcFile = "/*\n" +
                          "****************************************************\n" +
                          "*  File name          : \n" +
                          "*  Author             : -5A4A5943-\n" +
                          "*  Version            : \n" +
                          "*  Date               : \n" +
                          "*  Description        : \n" +
                          "*  Function List      : \n" +
                          "*  History            : \n" +
                          "*****************************************************\n" +
                          "*/" +
                          "\n\n\n/*Header file  */\n\n\n" +
                          "\n\n\n/*MACRO        */\n\n\n" +
                          "\n\n\n/*Typedef      */\n\n\n" +
                          "\n\n\n/*Variable     */\n\n\n" +
                          "\n\n\n/*Function     */\n\n\n";
            }
            Clipboard.SetDataObject(SrcFile);
        }

        private void button7_MouseEnter(object sender, EventArgs e)
        {
            DisplayHelp("复制时间字符串到剪切板");
        }

        private void button8_DragEnter(object sender, DragEventArgs e)
        {
            DisplayHelp("复制函数注释到剪切板");
        }

        private void button9_MouseEnter(object sender, EventArgs e)
        {
            DisplayHelp("复制源文件信息到剪切板");
        }

        private void button11_MouseEnter(object sender, EventArgs e)
        {
            DisplayHelp("复制源文件模板到剪切板");
        }

        private void button12_Click(object sender, EventArgs e)
        {
            DateTime CurTime = DateTime.Now;
            String str = "/*" +
                CurTime.Year.ToString("D4") + "--" +
                CurTime.Month.ToString("D2") + "--" +
                CurTime.Day.ToString("D2") + "--" +
                CurTime.Hour.ToString("D2") + "--" +
                CurTime.Minute.ToString("D2") + "--" +
                CurTime.Second.ToString("D2") + "(ZJYC):    */ ";
            Clipboard.SetDataObject(str);
        }
    }
}
