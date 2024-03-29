﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Chainword
{
    // Класс меню администратора
    public partial class MenuAdmin : Form
    {
        bool open_next = false;

        // Коструктор класса, инициализирует компоненты и выводит список словарей и кроссвордов
        public MenuAdmin()
        {
            InitializeComponent();
            ShowAllFiles(Environment.CurrentDirectory, "*.dict", Dictionary_listBox);
            ShowAllFiles(Environment.CurrentDirectory, "*.cros", CrossWord_listBox);
            Edit_button.Enabled = false;
            Delete_button.Enabled = false;
        }

        // Событийный метод. Срабатывает при закрытии формы. Открывает форму меню администратора
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (!open_next)
            {
                Form ifrm = Application.OpenForms[0];
                ifrm.Show();
            }
        }

        // Отобразить список кроссвордов и словарей
        private void ShowAllFiles(string rootDirectory, string fileExtension, ListBox files)
        {
            string test = null;
            string[] f = Directory.GetFiles(rootDirectory, fileExtension); // массив путей до файлов dict/cross
            for (int i = 0; i < f.Length; i++)
            {
                //разбиваем наш путь на части
                string[] path = f[i].Split(new string[] { "\\" }, StringSplitOptions.RemoveEmptyEntries);

                for (int j = 0; j < path.Length; j++)
                {
                    test += path[j] + "\n";
                    if (path[j].Contains("dict"))// выбираем тот кусок раздробленного пути, где содержится dict
                    {
                        string[] result = { path[j].Split('.')[0] };//удаляем все после точки у файла и выводим лишь название
                        files.Items.AddRange(result);
                    }
                    if (path[j].Contains("cros"))// выбираем тот кусок раздробленного пути, где содержится cros
                    {
                        string[] result = { (path[j]).Split('.')[0] };//удаляем все после точки у файла и выводим лишь название
                        files.Items.AddRange(result);
                    }
                }
            }
        }

        // Метод создания словаря
        private void Create_button_Click_1(object sender, EventArgs e)
        {
            open_next = true;
            Form cd = new CreateDictionary();
            cd.Show();
            cd.BringToFront();
            this.Hide();
        }

        // Перейти к созданию кроссворда
        private void Createcross_button_Click_1(object sender, EventArgs e)
        {
            open_next = true;
            Form cc = new CreateCross();
            cc.Show();
            cc.BringToFront();
            this.Hide();
        }

        // Перейти к редактированию кроссворда или словаря
        private void Edit_button_Click(object sender, EventArgs e)
        {
            // Edit_button_Click(sender: object, e: EventArgs)
            open_next = true;
            Thread thread = new Thread(SampleThreadMethod);
            thread.Start();
            foreach (var item in Dictionary_listBox.SelectedItems)
            {
                string file_name = (string)item;
                string[] f = Directory.GetFiles(Environment.CurrentDirectory, file_name + ".dict");
                Form fd = new FillingDictionary(f[0]);
                fd.Show();
            }
            foreach (var item in CrossWord_listBox.SelectedItems)
            {
                string file_name = (string)item;
                string[] f = Directory.GetFiles(Environment.CurrentDirectory, file_name + ".cros");
                Form fc = new FillingCross(f[0]);
                fc.Show();
            }
            Edit_button.Enabled = false;
            Delete_button.Enabled = false;
            thread.Abort();
            thread.Join();
            this.Close();
        }

        // Прогрессбар
        static void SampleThreadMethod()
        {
            ProgressBar pb = new ProgressBar();
            pb.ShowDialog();
            pb.BringToFront();
        }

        // Удалить кроссворд или словарь
        private void Delete_button_Click(object sender, EventArgs e)
        {
            foreach (var item in Dictionary_listBox.SelectedItems)
            {
                string file_name = (string)item;
                string[] f = Directory.GetFiles(Environment.CurrentDirectory, file_name + ".dict");
                File.Delete(f[0]);
            }
            Dictionary_listBox.Items.Clear();
            ShowAllFiles(Environment.CurrentDirectory, "*.dict", Dictionary_listBox);
            foreach (var item in CrossWord_listBox.SelectedItems)
            {
                string file_name = (string)item;
                string[] f = Directory.GetFiles(Environment.CurrentDirectory, file_name + ".cros");
                File.Delete(f[0]);
            }
            CrossWord_listBox.Items.Clear();
            ShowAllFiles(Environment.CurrentDirectory, "*.cros", CrossWord_listBox);
            Edit_button.Enabled = false;
            Delete_button.Enabled = false;
        }

        #region Методы запрета открытия сразу нескольких файлов 
        private void CrossWord_listBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CrossWord_listBox.SelectedIndex != -1)
            {
                Edit_button.Enabled = true;
                Delete_button.Enabled = true;
                Dictionary_listBox.SelectedIndex = -1;
            }
        }

        private void Dictionary_listBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Dictionary_listBox.SelectedIndex != -1)
            {
                Edit_button.Enabled = true;
                Delete_button.Enabled = true;
                CrossWord_listBox.SelectedIndex = -1;
            }
        }
        #endregion
    }
}