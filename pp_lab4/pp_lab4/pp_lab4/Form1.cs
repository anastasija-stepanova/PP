using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace pp_lab4
{
    public partial class Form1 : Form
    {
        private Entities entities = new Entities();
        private bool isAsync = true;
        public Form1()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            isAsync = !isAsync;
        }

        private void async_Click(object sender, EventArgs e)
        {
            try
            {
                runAsync();
                
            }
            catch (Exception ex)
            {
                richTextBox1.Text = ex.Message;
            }
        }

        private void runSync()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            string filewithlistpath = textBox1.Text;

            string filecontent = entities.ReadFromFile(filewithlistpath);
            HashSet<string> needed = new HashSet<string>(filecontent.Split(' '));

            string response = entities.Request("https://www.cbr-xml-daily.ru/daily_json.js");

            List<Money> listOfMoney = new List<Money>();

            JObject obj = JObject.Parse(response);

            foreach (var valute in obj["Valute"])
            {
                var val = valute.First();
                var money = new Money();
                money.Name = val["Name"].ToString();
                money.Nominal = int.Parse(val["Nominal"].ToString());
                money.Value = double.Parse(val["Value"].ToString());
                money.CharCode = val["CharCode"].ToString();

                if (needed.Contains(money.CharCode))
                {
                    listOfMoney.Add(money);
                }
            }

            string finalRes = "Sync...\n";

            foreach (var item in listOfMoney)
            {
                finalRes += $"{item.Nominal} {item.Name} по курсу {item.Value}\n";
            }

            File.WriteAllText(textBox2.Text, finalRes);

            finalRes += "Time: " + sw.ElapsedMilliseconds.ToString();
            richTextBox1.Text = finalRes;

            sw.Stop();
        }

        private async Task runAsync()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            string filewithlistpath = textBox1.Text;

            string response = await entities.RequestAsync("https://www.cbr-xml-daily.ru/daily_json.js");

            string filecontent = await entities.ReadFromFileAsync(filewithlistpath);
            HashSet<string> needed = new HashSet<string>(filecontent.Split(' '));

            List<Money> listOfMoney = new List<Money>();

            JObject obj = JObject.Parse(response);

            foreach (var valute in obj["Valute"])
            {
                var val = valute.First();
                var money = new Money();
                money.Name = val["Name"].ToString();
                money.Nominal = int.Parse(val["Nominal"].ToString());
                money.Value = double.Parse(val["Value"].ToString());
                money.CharCode = val["CharCode"].ToString();

                if (needed.Contains(money.CharCode))
                {
                    listOfMoney.Add(money);
                }
            }

            string finalRes = "Async...\n";

            foreach (var item in listOfMoney)
            {
                finalRes += $"{item.Nominal} {item.Name} по курсу {item.Value}\n";
            }

            using (FileStream stream = new FileStream(textBox2.Text, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true))
            using (StreamWriter writer = new StreamWriter(stream))
            {
                await writer.WriteLineAsync(finalRes);
            }
            finalRes += "Time: " + sw.ElapsedMilliseconds.ToString();
            richTextBox1.Text = finalRes;

            sw.Stop();
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void sync_Click(object sender, EventArgs e)
        {
            try
            {
                runSync();
            }
            catch (Exception ex)
            {
                richTextBox1.Text = ex.Message;
            }
        }
    }
}
