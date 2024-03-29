﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Chainword
{
    // Класс задания имени будущего словаря. Первый этап создания словаря
    public partial class CreateDictionary : Form
    {
        bool open_next = false;
        FileWorker fw;

        // Конструктор класс с инициализациями
        public CreateDictionary()
        {
            InitializeComponent();
            fw = new FileWorker();
        }

        // Кнопка открывает форму заполнения словаря
        private void CreateDictionary_Button_Click(object sender, EventArgs e)
        {
            open_next = true;
            if (!Regex.IsMatch(textBox1.Text, "^[A-Za-z0-9А-Яа-я_]+$"))
            {
                MessageBox.Show("Название словаря должно состоять из латинских букв, цифр и нижнего подчеркивания");
                open_next = false;
            }
            else
            {
                string writePath = Environment.CurrentDirectory + "\\" + textBox1.Text + ".dict";
                CreateDictionary cd = this;
                fw.CreateDictionary(writePath, cd);
                if(fw.IsNext == false)
                {
                    open_next = false;
                }
            }
        }

        // Событийный метод. Срабатывает при закрытии формы. Открывает форму авторизации
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (open_next == false)
            {
                Form ifrm = Application.OpenForms[0];
                ifrm.Show();
            }
        }
    }
}
