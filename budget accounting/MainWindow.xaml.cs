
using budget_accounting.Class;
using budget_accounting.json;
using budget_accounting.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Runtime;
using System.Windows;
using System.Windows.Controls;

namespace budget_accounting
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        double money = 0;
        static List<Note> notes = new List<Note>();
        
        public static List<string> type = new List<string>();
        List<Note> NoteFromDataGrid = new List<Note>();
        public MainWindow()
        {
            InitializeComponent();
            Update_TypeBox();
            DatePick.SelectedDate = DateTime.Now;
            notes = FileManager.ReadFromFile<Note>("Notes.json");
            if (notes != null) { ShowNotes(); } 
        }

        private void ShowNotes()
        {
            NoteFromDataGrid = new List<Note>();
            DataGridMenu.ItemsSource = null; // это обязательная часть, почему то если убрать этот null то таблица не будет обнавляться автоматически. 
            foreach (Note note in notes)
            {
                if (DatePick.Text == note.date)
                {
                    NoteFromDataGrid.Add(note);
                }
            }
            DataGridMenu.ItemsSource = NoteFromDataGrid;
            ShowBalance();
        }

        private void ShowBalance()
        { 

            money = 0;
            foreach (Note note in notes)
            {
               money = money + note.money; 
            }
            Balance_status.Text = "Сумма: " + Convert.ToString(money);
        }



        private void Create_new_type(object sender, RoutedEventArgs e)
        {
            CreateTypeWindow createTypeWindow = new CreateTypeWindow();
            createTypeWindow.ShowDialog();
            Update_TypeBox();
        }

        public void Update_TypeBox()
        {
            type = FileManager.ReadFromFile<string>("Type.json");
            if (type == null)
            {
                type = new List<string>();
            }
            Notes_type.Items.Clear();
            for (int i = 0; i < type.Count; i++)
            {
                Notes_type.Items.Add(type[i]);
            }
        }

        private void Add_new_note_Click(object sender, RoutedEventArgs e)
        {
            double summ;
            if (double.TryParse(summ_money.Text, out summ))
            {
                if (notes == null) { notes = new List<Note>(); }
                DateTime date = Convert.ToDateTime(DatePick.Text);
                Note note = new Note(Name_Note.Text, Notes_type.Text, summ, date.ToShortDateString());
                Name_Note.Text = "";
                summ_money.Text = "";
                notes.Add(note); 
                FileManager.MySerialize("Notes.json", notes);
                ShowNotes();

            }
            else
            {
                MessageBox.Show("Ошибка!");
            }



        }
        private void DataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)  // созданно для удаления(скрытия) ненужного столбца. 
        {
            if (e.PropertyName == "date")
            {
                e.Column = null;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ShowNotes();
        }

        private void Delete_Click_1(object sender, RoutedEventArgs e)
        {
            notes.Remove((Note)(DataGridMenu.SelectedItem));
            ShowNotes();
            FileManager.MySerialize("Notes.json", notes);
        }

        private void Change_click(object sender, RoutedEventArgs e)
        {
            int index;
            double summ;
            index = NoteFromDataGrid.IndexOf((Note)DataGridMenu.SelectedItem);
            foreach (Note note in notes)
            {
                if (note == NoteFromDataGrid[index])
                {
                    index = notes.IndexOf(note);

                }
            }
            if (double.TryParse(summ_money.Text, out summ))
            {
                DateTime date = Convert.ToDateTime(DatePick.Text);
                notes[index] = new Note(Name_Note.Text, Notes_type.Text, summ, date.ToShortDateString());
                Name_Note.Text = "";
                summ_money.Text = "";
                FileManager.MySerialize("Notes.json", notes);
                ShowNotes();

            }
            else
            {
                MessageBox.Show("Ошибка!");
            }
        }
        private void DataGrid_DataChange(object sender, SelectionChangedEventArgs e) {
            if (DataGridMenu.SelectedItem != null)
            {
                Name_Note.Text = ((Note)DataGridMenu.SelectedItem).nameNote;
                Notes_type.Text = ((Note)DataGridMenu.SelectedItem).type;
                double cash = ((Note)DataGridMenu.SelectedItem).money;
                summ_money.Text = Convert.ToString(cash);
            }
        }
    }
}

