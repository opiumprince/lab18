using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab18
{
    public partial class Form1 : Form
    {
        private ContextMenuStrip contextMenuProcess;
        public Form1()
        {
            InitializeComponent();

            // вид елемента ListView на Details
            listViewProcesses.View = View.Details;

            // два стовпці до елемента ListView
            listViewProcesses.Columns.Add("ID", 100);
            listViewProcesses.Columns.Add("Назва", 200);

        // Оновити список процесів
        RefreshProcessList();
        }

        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            RefreshProcessList();
        }

        private void RefreshProcessList()
        {
            // очистка списоку процесів
            listViewProcesses.Items.Clear();

            // всі запущені процеси
            Process[] processes = Process.GetProcesses();

            // + кожен процес до ListView
            foreach (Process process in processes)
            {
                ListViewItem item = new ListViewItem(process.Id.ToString());
                item.SubItems.Add(process.ProcessName);
                listViewProcesses.Items.Add(item);
            }
        }

        private void buttonExport_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "Текстові файли (*.txt)|*.txt";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    using (StreamWriter writer = new StreamWriter(saveFileDialog.FileName))
                    {
                        foreach (ListViewItem item in listViewProcesses.Items)
                        {
                            writer.WriteLine($"{item.Text}\t{item.SubItems[1].Text}");
                        }
                    }
                }
            }
        }

        private void killMenuItem_Click(object sender, EventArgs e)
        {
            // Перевірити, чи вибрано процес для зупинки
            if (listViewProcesses.SelectedItems.Count > 0)
            {
                // Отримати вибраний елемент
                ListViewItem selectedProcess = listViewProcesses.SelectedItems[0];

                // Отримати ідентифікатор процесу
                int processId = Convert.ToInt32(selectedProcess.SubItems[0].Text);

                // Отримати об'єкт процесу за його ідентифікатором
                Process process = Process.GetProcessById(processId);

                // Зупинити процес
                process.Kill();

                // Оновити список процесів
                RefreshProcessList();
            }
        }
        private void infoProcess_Click(object sender, EventArgs e)
        {
            // Перевірити, чи вибрано процес для відображення інформації
            if (listViewProcesses.SelectedItems.Count > 0)
            {
                // Отримати вибраний елемент
                ListViewItem selectedProcess = listViewProcesses.SelectedItems[0];

                // Отримати ідентифікатор процесу
                int processId = Convert.ToInt32(selectedProcess.SubItems[0].Text);

                // Отримати об'єкт процесу за його ідентифікатором
                Process process = Process.GetProcessById(processId);

                // Відобразити інформацію про процес
                string processInfo = $"ID: {process.Id}\nНазва: {process.ProcessName}\n";
                processInfo += $"Загальний час роботи: {process.TotalProcessorTime}\n";
                processInfo += $"Фізична пам'ять: {process.WorkingSet64} bytes\n";
                MessageBox.Show(processInfo);
            }
        }

        // Функція для виведення інформації про потоки процесу
        private void DisplayProcessThreads(Process process)
        {
            string threadInfo = "Потоки:\n";
            foreach (ProcessThread thread in process.Threads)
            {
                threadInfo += $"ID: {thread.Id}, Статус: {thread.ThreadState}\n";
            }
            MessageBox.Show(threadInfo);
        }

        // Функція для виведення інформації про модулі процесу
        private void DisplayProcessModules(Process process)
        {
            string moduleInfo = "Модулі:\n";
            foreach (ProcessModule module in process.Modules)
            {
                moduleInfo += $"Назва: {module.ModuleName}, Версія: {module.FileVersionInfo.FileVersion}\n";
            }
            MessageBox.Show(moduleInfo);
        }

        private void infoThreadsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listViewProcesses.SelectedItems.Count > 0)
            {
                ListViewItem selectedProcess = listViewProcesses.SelectedItems[0];
                int processId = Convert.ToInt32(selectedProcess.SubItems[0].Text);
                Process process = Process.GetProcessById(processId);
                DisplayProcessThreads(process);
            }
        }
        private void infoModulesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listViewProcesses.SelectedItems.Count > 0)
            {
                ListViewItem selectedProcess = listViewProcesses.SelectedItems[0];
                int processId = Convert.ToInt32(selectedProcess.SubItems[0].Text);
                Process process = Process.GetProcessById(processId);
                DisplayProcessModules(process);
            }
        }

        private void listViewProcesses_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && listViewProcesses.FocusedItem.Bounds.Contains(e.Location))
            {
                contextMenuProcess.Show(Cursor.Position);
            }
        }
    }

}
