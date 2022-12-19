using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Management;
using Microsoft.VisualBasic;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace TaskManager
{
    public partial class Form1 : Form
    {
        
        //лист хранит список текущих процессов
        private List<Process> processes = null;

        private ListViewItemComparer comparer = null;

        public Form1()
        {
            InitializeComponent();
        }
        
        
        //данный метод заполняет и обновляет список
        private void GetProcesses()
        {
            processes.Clear();
            
            processes = Process.GetProcesses().ToList<Process>();
        }

        
        
        //метод заполняющий список контентом
        private void RefreshProcessesList(List<Process> processes, string tag)
        {
            try
            {
                listView1.Items.Clear();

                double memSize = 0;

                //проверка и создание объекта в NP
                foreach (Process p in processes)
                {
                    if (p != null)
                    {
                        memSize = 0;
                        PerformanceCounter pc = new PerformanceCounter();
                        pc.CategoryName = "Process";
                        pc.CounterName = "Working Set - Private";
                        pc.InstanceName = p.ProcessName;

                        memSize = (double)pc.NextValue() / (1000 * 1000);

                        //массив строк для хранения колонок
                        string[] row = new string[] { p.ProcessName.ToString(), Math.Round(memSize, 1).ToString() };

                        listView1.Items.Add(new ListViewItem(row));

                        pc.Close();
                        pc.Dispose();
                    }
                }
                Text = $"Processes Started :" + processes.Count.ToString();
            }
            catch (Exception) { }
        }

        
        //метод для завершения процесса
        private void KillProcess(Process process)
        {
            process.Kill();

            process.WaitForExit();
        }

        
        
        //метод для завершения дерева процессов
        private void KillProcessAndChildren(int id)
        {
            if (id == 0)
            {
                return;
            }
            //метод нахождения всех связанных процессов
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("Select * From Win32_Process Where ParentProcessID=" + id);

            ManagementObjectCollection objectCollection = searcher.Get();
            foreach (ManagementObject obj in objectCollection)
            {
                KillProcessAndChildren(Convert.ToInt32(obj["ProcessID"]));
            }
            try
            {
                Process p = Process.GetProcessById(id);
                p.Kill();
                p.WaitForExit();
            }
            catch (ArgumentException) { }
        }
        
        
        //метод получения ID род-го процесса
        private int GetParentProcessID(Process p)
        {
            int parentID = 0;

            try
            {
                ManagementObject managementObject = new ManagementObject("win32_process.handle='" + p.Id + "'");

                managementObject.Get();

                parentID = Convert.ToInt32(managementObject["ParentProcessId"]);
            }
            catch (Exception) { }
            return parentID; 
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            processes = new List<Process>();
            GetProcesses();

            RefreshProcessesList(processes, ToString());

            comparer = new ListViewItemComparer();
            comparer.Index= 0;
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            GetProcesses();

            RefreshProcessesList(processes, ToString());
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            try
            {
                if (listView1.SelectedItems[0]!= null)
                {
                    Process processToKill = processes.Where((x) => x.ProcessName == listView1.SelectedItems[0].SubItems[0].Text).ToList()[0];

                    KillProcess(processToKill);

                    GetProcesses();

                    RefreshProcessesList(processes, ToString());

                }
            }
            catch (Exception) { }
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            try
            {
                if (listView1.SelectedItems[0] != null)
                {
                    Process processToKill = processes.Where((x) => x.ProcessName == listView1.SelectedItems[0].SubItems[0].Text).ToList()[0];

                    KillProcessAndChildren(GetParentProcessID(processToKill));

                    GetProcesses();

                    RefreshProcessesList(processes, ToString());

                }
            }
            catch (Exception) { }
        }

                private void toCompleteToolStripMenuItem_Click(object sender, EventArgs e)
                {
                    try
                    {
                        if (listView1.SelectedItems[0] != null)
                        {
                            Process processToKill = processes.Where((x) => x.ProcessName == listView1.SelectedItems[0].SubItems[0].Text).ToList()[0];

                            KillProcess(processToKill);

                            GetProcesses();

                            RefreshProcessesList(processes, ToString());

                        }
                    }
                    catch (Exception) { }
                }

                private void endTheProcessTreeToolStripMenuItem_Click(object sender, EventArgs e)
                {
                    try
                    {
                        if (listView1.SelectedItems[0] != null)
                        {
                            Process processToKill = processes.Where((x) => x.ProcessName == listView1.SelectedItems[0].SubItems[0].Text).ToList()[0];

                            KillProcessAndChildren(GetParentProcessID(processToKill));

                            GetProcesses();

                            RefreshProcessesList(processes, ToString());

                        }
                    }
                    catch (Exception) { }
                }

        private void runTaskToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string path = Interaction.InputBox("Введите имя программы", "Запуск новой задачи");

            try
            {
                Process.Start(path);
            }
            catch (Exception) { }
        }

        private void toolStripTextBox1_TextChanged(object sender, EventArgs e)
        {
            GetProcesses();

            List<Process> filteredprocesses = processes.Where((x) => x.ProcessName.ToLower().Contains(toolStripTextBox1.Text.ToLower())).ToList<Process>();

            RefreshProcessesList(filteredprocesses, toolStripTextBox1.Text);
        }

        private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            comparer.Index= e.Column;

            comparer.SortDirection = comparer.SortDirection == SortOrder.Ascending ? SortOrder.Descending :SortOrder.Ascending;

            listView1.ListViewItemSorter= comparer;

            listView1.Sort();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
