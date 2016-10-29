using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace ZJYC_CS_1
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            //byte[] byDll = Properties.Resources.IrisSkin2;//获取嵌入dll文件的字节数组
            //string strPath = Application.StartupPath + @"\IrisSkin2.dll";//设置释放路径
            //创建dll文件（覆盖模式）
            //using (FileStream fs = new FileStream(strPath, FileMode.Create))
            //{
            //    fs.Write(byDll, 0, byDll.Length);
            //}

            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
