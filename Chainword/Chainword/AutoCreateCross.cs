﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Chainword
{
    // Класс автоматической генерации кроссворда
    class AutoCreateCross
    {
        string dictionary;
        string name_cross;
        int cross_letters, length_cross, type_cross;
        Crossword cross;

        // Конструктор класса с некоторыми параметрами будущего кроссворда 
        public AutoCreateCross(string name_cross, string dictionary, int type_cross, int cross_letters, int length_cross)
        {
            try
            {
                this.dictionary = dictionary;
                this.name_cross = name_cross;
                this.type_cross = type_cross;
                this.cross_letters = cross_letters;
                this.length_cross = length_cross;
                cross = new Crossword();
            }
            catch
            {
                MessageBox.Show("Не удалось создать кроссворд!");
            }

        }

        // Метод создания кроссворда с инициализацией полей класса Crossword и сериализацией
        public void CreateCross()
        {
            Thread thread = new Thread(SampleThreadMethod);
            thread.Start();
            string[] words = GetWords();
            Crossword cross = new Crossword();
            cross.Name = name_cross;
            cross.Length = length_cross;
            cross.DisplayType = type_cross;
            cross.Dictionary = dictionary;
            cross.CrossLetters = cross_letters;
            cross.AddWords(words, words.Length);
            decimal counter = 0;
            for(int i = 0; i < words.Length; i++)
            {
                char[] temp = words[i].ToCharArray();
                for(int k = 0; k < temp.Length; k++)
                    counter++;
            }
            cross.Hint = (int)Math.Ceiling(counter / 10);

            FileStream stream = File.Create(Environment.CurrentDirectory + "\\" + name_cross + ".cros");

            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, cross);
            stream.Close();
            thread.Abort();

        }

        // Метод для открытия формы с прогрессбаром
        static void SampleThreadMethod()
        {
            ProgressBar pb = new ProgressBar();
            pb.ShowDialog();
            pb.BringToFront();
        }

        // Метод получения случайных слов, которые пойдут в будущий кроссворд
        private string[] GetWords()
        {
            Thread thread = new Thread(SampleThreadMethod);
            thread.Start();

            string[] arr_words, arr_only_words, result = null;
            string only_words = "";
            Random rand = new Random((int)DateTime.Now.Ticks);

            #region получаем в массив arr_only_words все слова, которые есть в словаре
            using (StreamReader fs = new StreamReader(dictionary))
            {
                string words = null;
                while (true)
                {
                    string tmp = fs.ReadLine();
                    if (tmp == null) break;
                    tmp += "\n";
                    words += tmp;
                }
                arr_words = words.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
                for (int j = 0; j < arr_words.Length; j++)
                {
                    only_words += arr_words[j].Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries)[0] + "\n";
                }
                arr_only_words = only_words.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            }
            #endregion


            List<string> words_without_repetitions = new List<string>();
            List<string> memory_words = new List<string>();

            #region Сам процесс получения рандомных слов из arr_only_words в массив result
            try
            {
                result = new string[length_cross];
                int i = 0;
                int max_iter = 10000;
                while (i < length_cross)
                {
                    int index = rand.Next(0, arr_only_words.Length);
                    if (i != 0)
                    {

                        bool check = true;
                        for (int z = 0; z < arr_only_words.Length; z++)
                        {
                            foreach (var item in memory_words)
                            {
                                if (arr_only_words[z] == item)
                                    check = false;
                            }
                            if(check)
                                words_without_repetitions.Add(arr_only_words[z]);
                            check = true;
                        }

                        string[] arr_wwr = new string[words_without_repetitions.Count];
                        int x = 0;
                        foreach (var item in words_without_repetitions)
                        {
                            arr_wwr[x] = item;
                            x++;
                        }
                        // Вернет список с подходящими словами
                        List<string> sw =
                                SearchWordsMask(arr_wwr, result[i - 1].Substring(result[i - 1].Length - cross_letters).ToCharArray());
                        if (!sw.Any())
                        {
                            i -= 2;
                            max_iter--;
                        }

                        else
                        {
                            string[] arr_sw = new string[sw.Count];
                            int k = 0;
                            foreach (var item in sw)
                            {
                                arr_sw[k] = item;
                                k++;
                            }
                            index = rand.Next(0, arr_sw.Length);
                            result[i] = arr_sw[index];
                            memory_words.Add(result[i]);
                        }
                    }
                    if (i == 0)
                    {
                        result[i] = arr_only_words[index];
                        memory_words.Add(result[i]);
                    }
                    i++;
                    words_without_repetitions.Clear();
                    if (max_iter < 0)
                    {
                        MessageBox.Show("Не удалось составить кроссворд из данного словаря автоматически! Попробуйте составить вручную.");
                        break;
                    }
                }
            }
            catch
            {
                MessageBox.Show("Не удалось составить кроссворд из данного словаря автоматически! Попробуйте составить вручную.");
            }
            #endregion

            thread.Abort();
            return result;
        }

        // Поиск слов по маске (последние буквы последнего добавленного слова)
        private List<string> SearchWordsMask(string[] arr_only_words, char[] letters)
        {
            List<string> suitable_words = new List<string>();
            for (int i = 0; i < arr_only_words.Length; i++)
            {
                if (cross_letters == 1)
                {
                    if (arr_only_words[i][0] == letters[0])
                    {
                        suitable_words.Add(arr_only_words[i]);
                    }
                }
                else if(cross_letters == 2)
                {
                    if (arr_only_words[i][0] == letters[0] && 
                        arr_only_words[i][1] == letters[1] &&
                        arr_only_words[i].Length > 4)
                    {
                        suitable_words.Add(arr_only_words[i]);
                    }
                }
                else
                {
                    if (arr_only_words[i][0] == letters[0] && 
                        arr_only_words[i][1] == letters[1] && 
                        arr_only_words[i][2] == letters[2] &&
                        arr_only_words[i].Length > 6)
                    {
                        suitable_words.Add(arr_only_words[i]);
                    }
                }
            }
            return suitable_words;
        }
    }
}

