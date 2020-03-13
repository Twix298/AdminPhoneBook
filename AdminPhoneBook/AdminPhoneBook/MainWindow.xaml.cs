using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.Data.Entity;
using Microsoft.Win32;
using System.IO;

namespace AdminPhoneBook
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
            image1.PreviewMouseLeftButtonUp += Image1_MouseLeftButtonUp;
            listBox1.SelectionChanged += ListBox1_SelectionChanged;
            textName.PreviewMouseLeftButtonUp += TextName_PreviewMouseLeftButtonUp;
        }

        private void TextName_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            textName.Background = Brushes.White;
            textName.Foreground = Brushes.Black;
            textName.SelectAll();
        }

        private void Button1_Click_1(object sender, RoutedEventArgs e)
        {
            image1.Source = new BitmapImage(new Uri("noImage.png", UriKind.Relative));
            if(listBox1.SelectedItems.Count > 0)
            {
                listBox1.Items.Remove(listBox1.SelectedItems[0]);
            }
            listBox1.Items.Clear();
            textName.Text = "ФИО";
            textDepartament.Text = "НИЦ";
            textUnit.Text = "Отдел";
            textPosition.Text = "Должность";
            textNumber.Text = "Телефон";
            textBuilding.Text = null;
            textCabinet.Text = null;
            using (PhoneBookContext phoneBook = new PhoneBookContext())
            {
                if (textBox1.Text != "")
                {
                    string text = textBox1.Text;
                    var people = phoneBook.phonebooks;
                    foreach (Phonebook person in people)
                    {
                        if (person.Name.IndexOf(text, StringComparison.OrdinalIgnoreCase) != -1)
                        {
                            listBox1.Items.Add(person.Name);

                            Console.WriteLine("ФИО: {0}     Кабинет:{1}     Телефон:{2}", person.Name, person.Cabinet, person.Number);
                        }

                    }
                }
            }
        }

        private void Image1_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "image files (*.jpg) | *.jpg | All files (*.*) | *.*";
            Nullable<bool> result = openFileDialog.ShowDialog();
            if (result == true)
            {
                string fileName = openFileDialog.FileName;
                image1.Source = new BitmapImage(new Uri(fileName));

            }
            
        }

        private void ListBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedName = "";
            if(listBox1.SelectedItem != null)
                selectedName = listBox1.SelectedItem.ToString();
            GetParameters(selectedName);
            selectedName = "";
        }

        private void GetParameters(string selectedName)
        {
            using( PhoneBookContext phoneBook = new PhoneBookContext())
            {
                var people = phoneBook.phonebooks;
                foreach(var person in people)
                {
                    if(selectedName == person.Name)
                    {
                        textName.Text = person.Name;
                        textDepartament.Text = person.Departament;
                        textUnit.Text = person.Unit;
                        textPosition.Text = person.Position;
                        textNumber.Text = person.Number;
                        textBuilding.Text = person.Building;
                        textCabinet.Text = person.Cabinet;
                        BitmapImage image = new BitmapImage();
                        if(person.ImageData != null)
                        {
                            using (MemoryStream ms = new MemoryStream(person.ImageData))
                            {
                                image.BeginInit();
                                image.CacheOption = BitmapCacheOption.OnLoad;
                                image.StreamSource = ms;
                                image.EndInit();
                            }
                            image1.Source = image;
                        }
                    }
                }
            }
        }

        private void Button_save_Click(object sender, RoutedEventArgs e)
        {
            PhoneBookContext phoneBook = new PhoneBookContext();
            var people = phoneBook.phonebooks.Where(c => c.Name == textName.Text).FirstOrDefault() ;
            people.Name = textName.Text;
            people.Departament = textDepartament.Text;
            people.Unit = textUnit.Text;
            people.Position = textPosition.Text;
            people.Number = textNumber.Text;
            people.Building = textBuilding.Text;
            people.Cabinet = textCabinet.Text;

            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage = image1.Source as BitmapImage;
            byte[] imageData = null;
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmapImage));
            using(MemoryStream ms = new MemoryStream())
            {
                encoder.Save(ms);
                imageData = ms.ToArray();
            }
            people.ImageData = imageData;

            phoneBook.SaveChanges();

        }
    }

    public class PhoneBookContext : DbContext
    {
        public PhoneBookContext(): base("PhoneBook")
        {

        }
        public DbSet<Phonebook> phonebooks { get; set; }
    }

    
}
