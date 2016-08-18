using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Configuration;
using Microsoft.Office.Interop.Outlook;
using System.Diagnostics;

namespace SSISLogSimplifier
{
    public partial class Simplifier : Form
    {
        public List<LogEntity> warninList,errorList;
        public   List <string> FileStatus;
        public string logPath;

        public Simplifier()
        {
            InitializeComponent();
        }
        string filePath = string.Empty;
        private void button1_Click(object sender, EventArgs e)
        {
            using  (var dialog = new FolderBrowserDialog())
{
    //setup here

    if (dialog.ShowDialog() == DialogResult.OK)  //check for OK...they might press cancel, so don't do anything if they did.
    {
         var path = dialog.SelectedPath;    
         textBox1.Text= path;
         //do something with path
    }
}         
        }

        private void Simplifier_Load(object sender, EventArgs e)
        {
      //  filePath=    System.Configuration.ConfigurationManager["LogPath"]
            logPath = ConfigurationManager.AppSettings["LogFilePath"].ToString();
            textBox1.Text = logPath;
            //System.IO.FileInfo file = new System.IO.FileInfo(logPath+"ParsedLogFiles");
            //file.Directory.Create();
            pictureBox1.Visible = false;
            pictureBox2.Visible = false;
            if (!Directory.Exists(logPath + "ParsedLogFiles"))
            {
                DirectoryInfo di = Directory.CreateDirectory(logPath + "ParsedLogFiles");
            }
            
        }

        private void Simplify_Click(object sender, EventArgs e)
        {
            GetFileDetails();
            if (!Directory.Exists(textBox1.Text + "ParsedLogFiles"))
            {
                DirectoryInfo di = Directory.CreateDirectory(textBox1.Text + "ParsedLogFiles");
            }
            if (listBox1.SelectedItem != null)
            {
                Export2HTML.Enabled = true;
                button2.Enabled = true;
                button3.Enabled = true;
            }
         //   SimplifyLog();
           // FileOps fo = new FileOps();
           //List<LogEntity> leList=  fo.readFromLog("C:\\SSISLog\\dt.log");
           //var warnList = from a in leList
           //               where a.EventType == LogEnum.Warning
           //               select a;
           //warnGrdView.DataSource = warnList.ToList();
           //warnGrdView.Refresh();
           //var errList = from b in leList
           //              where b.EventType == LogEnum.Error
           //              select b;
           //errGrdView.DataSource = errList.ToList();
           //errGrdView.Refresh();

        }

        public void ParseAllBegin(object sender, EventArgs e)
        {
       //     GetFileDetails();
            if (listBox1.SelectedItem.ToString().Contains(".log")||listBox1.SelectedItem.ToString().Contains(".txt"))
            {
                SimplifyLog(listBox1.SelectedItem.ToString());
                GetLastLines(listBox1.SelectedItem.ToString());
                //   CreateHtmlFile();
                if (FileStatus.Count > 0)
                {
                    label1.Text = FileStatus[3].ToString() + "\n " + FileStatus[2].ToString() + "\n " + FileStatus[1].ToString() + "\n " + FileStatus[0].ToString();
                    if (label1.Text.Contains("FAILURE"))
                    {
                        label1.ForeColor = Color.Red;
                        pictureBox2.Visible = true;
                        pictureBox1.Visible = false;
                        label2.Visible = true;
                        label2.Text = errorList.Count.ToString()+ " Errors";
                    }
                    else if (label1.Text.Contains("SUCCESS"))
                    {
                        label1.ForeColor = Color.ForestGreen;
                        pictureBox1.Visible = true;
                        pictureBox2.Visible = false;
                        label2.Visible = false;
                    }
                }
          
      
            }
           
        }

        private void folderBrowserDialog1_HelpRequest(object sender, EventArgs e)
        {

        }
        public void GetFileDetails()
        {
            DirectoryInfo di = new DirectoryInfo(textBox1.Text);
            FileSystemInfo[] files = di.GetFileSystemInfos();
            var orderedFiles = files.OrderByDescending(f => f.CreationTime);
            listBox1.DataSource = orderedFiles.ToList();
            //listBox1.da
        }

        public void SimplifyLog(string fileName)
        {
            FileOps fo = new FileOps();
            List<LogEntity> leList = fo.readFromLog(textBox1.Text +'\\'+ fileName);
            var warnList = from a in leList
                           where a.EventType == LogEnum.Warning
                           select a;
            warnGrdView.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCells;
            warnGrdView.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            warnGrdView.DataSource = warnList.ToList();
            warnGrdView.Refresh();
            warninList = warnList.ToList();
            var errList = from b in leList
                          where b.EventType == LogEnum.Error
                          select b;
            errGrdView.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCells;
       //     errGrdView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
            errGrdView.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
         //   errGrdView.Dock = DockStyle.Fill;
            errGrdView.DataSource = errList.ToList();
            errGrdView.Refresh();
            errorList = errList.ToList();
            
        }

        public void GetLastLines(string fileName)
        {
            FileStatus = File.ReadLines(textBox1.Text+'\\'+fileName).Reverse().Take(4).ToList();
        }
        private void Export2HTML_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem!=null)
            {
                CreateHtmlFile(listBox1.SelectedItem.ToString());
            }
        }

        public void CreateHtmlFile(string fileName)
        {
            using (StreamWriter writer =
            new StreamWriter(textBox1.Text + "\\ParsedLogFiles\\" + fileName+".html"))
            {
             //   writer.Write("Word ");
                writer.WriteLine("<Html>");
                writer.WriteLine("<Head>");
                writer.WriteLine("</Head>");
                writer.WriteLine("<Body>");
                writer.WriteLine("<p align=\"center\" ><b>"+listBox1.SelectedItem+"</b></p>");
                writer.WriteLine("");
                writer.WriteLine("<p align=\"center\"><b><font color=\"blue\" size=\"3\"><u> " + FileStatus[3].ToString() + "</u></font></b></p>");
                writer.WriteLine("<p align=\"center\"><b><font color=\"blue\"> " + FileStatus[2].ToString() + "</font></b></p>");
                writer.WriteLine("<p align=\"center\"><b><font color=\"blue\"> " + FileStatus[1].ToString() + "</font></b></p>");
                writer.WriteLine("<p align=\"center\"><b><font color=\"blue\"> " + FileStatus[0].ToString() + "</font></b></p>");
                writer.WriteLine("");
                writer.WriteLine("<p align=\"center\"><b><font color=\"red\">ERRORS</font></b></p>");
                writer.WriteLine("<table align=\"center\" border=\"1\" bgcolor=\"white\">   <tr>    <th>Event Time</th>    <th>Source</th>      <th>Description</th>    <th>Code</th></tr>");
                foreach (var item in errorList)
                {
                    writer.WriteLine("<tr><td>" + item.EventTime + "</td>");
                    writer.WriteLine("<td>" + item.Source + "</td>");
                    writer.WriteLine("<td>" + item.Description + "</td>");
                    writer.WriteLine("<td>" + item.Code + "</td></tr>");
                }
                writer.WriteLine("</tr></table>");
                writer.WriteLine("");
                writer.WriteLine("");
                writer.WriteLine("<p align=\"center\" ><b><font color=\"yellow\">WARNINGS</font></b></p>");
                writer.WriteLine("<table align=\"center\" border=\"1\" bgcolor=\"yellow\">   <tr>    <th>Event Time</th>    <th>Source</th>      <th>Description</th>    <th>Code</th></tr>");
                foreach (var item in warninList)
                {
                    writer.WriteLine("<tr><td>" + item.EventTime + "</td>");
                    writer.WriteLine("<td>" + item.Source + "</td>");
                    writer.WriteLine("<td>" + item.Description + "</td>");
                    writer.WriteLine("<td>" + item.Code + "</td></tr>");
                }
                writer.WriteLine("</tr></table>");
                writer.WriteLine("<mark>Test this</mark>");
                writer.WriteLine("</Body>");
                writer.WriteLine("</Html>");
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
          // Microsoft.Office.Interop.Outlook.Application.Visible = true;

        //    book.SaveCopyAs(@”” + ConfigurationSettings.AppSettings[“Reports”] + “Course Schedules for “ + lblProviderName.Text + “.xls”);

            CreateHtmlFile(listBox1.SelectedItem.ToString());

            Microsoft.Office.Interop.Outlook.Application outlookApp = new Microsoft.Office.Interop.Outlook.Application();
            Microsoft.Office.Interop.Outlook.MailItem message = (Microsoft.Office.Interop.Outlook.MailItem)outlookApp.CreateItem(Microsoft.Office.Interop.Outlook.OlItemType.olMailItem);
            message.Subject = "Emailing Parsed Log File "+listBox1.SelectedItem.ToString();
            message.Recipients.Add(ConfigurationManager.AppSettings["EmailId"].ToString());
            message.Body = "FYI";
         //   int attachmentLocation = 1;

            message.Attachments.Add(textBox1.Text + "\\ParsedLogFiles\\" + listBox1.SelectedItem + ".html", Microsoft.Office.Interop.Outlook.OlAttachmentType.olByValue, Type.Missing, Type.Missing);
            message.Attachments.Add(textBox1.Text + "\\" + listBox1.SelectedItem , Microsoft.Office.Interop.Outlook.OlAttachmentType.olByValue, Type.Missing, Type.Missing);


message.Display(false);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            CreateHtmlFile(listBox1.SelectedItem.ToString());
            Process p = new Process();
            p.StartInfo.UseShellExecute = true;
            p.StartInfo.FileName = textBox1.Text+ "\\ParsedLogFiles\\" +listBox1.SelectedItem+".html";
            p.StartInfo.Verb = "Open";
            p.Start();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Process p = new Process();
            p.StartInfo.UseShellExecute = true;
            p.StartInfo.FileName = textBox1.Text + "\\" + listBox1.SelectedItem;
            p.StartInfo.Verb = "Open";
            p.Start();
        }
    }
}
