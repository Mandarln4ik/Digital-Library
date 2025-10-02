using Digital_Library.Model;
using Digital_Library.Services;
using MySql.Data.MySqlClient;
using MySql.Data.Types;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
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
using System.Windows.Shapes;

namespace Digital_Library
{
    /// <summary>
    /// Логика взаимодействия для AddDocumentWindow.xaml
    /// </summary>
    public partial class AddDocumentWindow : Window
    {
        public ObservableCollection<Tag> appliedTags = new ObservableCollection<Tag>();
        public List<Language> langs = new();
        public List<Author> authors = new();

        public AddDocumentWindow()
        {
            InitializeComponent();
            TagsControl.DataContext = this;
            TagsControl.ItemsSource = appliedTags;

            LoadData();
        }

        public async Task LoadData()
        {
            DBservice db = new();
            DataTable languages = await db.GetResultsFromSQL("SELECT * FROM `languages`");
            DataTable authorstb = await db.GetResultsFromSQL("SELECT * FROM `authors`");

            foreach (DataRow item in languages.Rows)
            {
                Language lang = new()
                {
                    LanguageId = Convert.ToInt32(item.ItemArray[0]),
                    LanguageCode = Convert.ToString(item.ItemArray[1]),
                    LanguageName = Convert.ToString(item.ItemArray[2])
                };
                langs.Add(lang);
                LanguageComboBox.Items.Add($"{lang.LanguageName} - {lang.LanguageCode}");
            }

            foreach (DataRow item in authorstb.Rows)
            {
                Author auth = new()
                {
                    AuthorId = Convert.ToInt32(item.ItemArray[0]),
                    FirstName = Convert.ToString(item.ItemArray[1]),
                    LastName = Convert.ToString(item.ItemArray[2]),
                    MiddleName = Convert.ToString(item.ItemArray[3])
                };
                authors.Add(auth);
                AuthorComboBox.Items.Add($"{auth.FirstName} {auth.LastName} {auth.MiddleName}");
            }
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            TagsWindow w = new TagsWindow();
            w.ShowDialog();
            if (w.DialogResult == true)
            {
                foreach (Tag item in w.tags)
                {
                    if (appliedTags.FirstOrDefault(t => t.TagId == item.TagId) == null)
                    {
                        appliedTags.Add(item);
                    }
                }
            }
            w.Close();
            
        }

        private void RemoveTag(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.CommandParameter is Tag tag)
            {
                // Удаляем из коллекции
                appliedTags.Remove(tag);
            }
        }

        private void AddDocument_Click(object sender, RoutedEventArgs e)
        {
            AddDocument();
        }

        public async Task AddDocument()
        {
            try
            {
                DBservice db = new DBservice();

                string title, alternativeTitle, description, isbn, issn, udc, bbk;
                int publicationYear, totalPages, uploadedBy, languageId, authorId;

                if (String.IsNullOrEmpty(TitleBox.Text))
                {
                    MessageBox.Show("Название не может быть пустым");
                    return;
                }
                else { title = TitleBox.Text; }

                if (!int.TryParse(YearPublicationBox.Text, out publicationYear))
                {
                    MessageBox.Show("Год выпуска должен быть числом!");
                    return;
                }

                if (!int.TryParse(TotalPagesBox.Text, out totalPages))
                {
                    MessageBox.Show("Кол-во страниц должно быть числом!");
                    return;
                }

                if (AuthorComboBox.SelectedItem != null)
                {
                    authorId = authors[AuthorComboBox.SelectedIndex].AuthorId;
                }
                else { MessageBox.Show("Нужно выбрать автора!"); return; }

                if (LanguageComboBox.SelectedItem != null)
                {
                    languageId = langs[LanguageComboBox.SelectedIndex].LanguageId;
                }
                else { MessageBox.Show("Нужно выбрать язык!"); return; }

                alternativeTitle = AlternativeTitleBox.Text;
                description = DescriptionBox.Text;

                isbn = ISBNBox.Text;
                issn = ISBNBox.Text;
                udc = ISBNBox.Text;
                bbk = ISBNBox.Text;

                uploadedBy = Account.GetInstance().currentUser.Id;


                MySqlConnection conn = db.GetConnection();
                MySqlCommand command = new($"INSERT INTO `documents` (`document_id`, `title`, `alternative_title`, `publication_year`, `isbn`, `issn`, `udc`, `bbk`, `description`, `total_pages`, `file_path`, `image_path`, `date_uploaded`, `uploaded_by`, `language_id`) VALUES (NULL, '{title}', '{alternativeTitle}', '{publicationYear}', '{isbn}', '{issn}', '{udc}', '{bbk}', '{description}', '{totalPages}', ' ', NULL, current_timestamp(), '{uploadedBy}', '{languageId}')", conn);
                await command.ExecuteNonQueryAsync();
                Debug.WriteLine("doc added!");

                command = new($"SELECT LAST_INSERT_ID()", conn);
                int docId = 0;
                using (var reader = command.ExecuteReader())
                {
                    while(reader.Read())
                    {
                        docId = reader.GetInt32(0);
                    }
                }
                Debug.WriteLine("doc id recieved!");

                command = new($"INSERT INTO `document_authors` (`document_id`, `author_id`) VALUES ('{docId}', '{authorId}')", conn);
                await command.ExecuteNonQueryAsync();
                Debug.WriteLine("doc author added!");

                string cmd = "INSERT INTO `document_tags` (`document_id`, `tag_id`) VALUES";

                foreach (var item in appliedTags)
                {
                    cmd += $" ({docId}, {item.TagId}),";
                }
                cmd = cmd.Remove(cmd.Length - 1, 1);

                Debug.WriteLine(cmd);
                command = new(cmd, conn);
                await command.ExecuteNonQueryAsync();
                Debug.WriteLine("doc tags added!");

                MessageBox.Show("Документ успешно добавлен!");
                MainWindow mw = (MainWindow)Application.Current.MainWindow;
                await Dispatcher.BeginInvoke(() => mw.GetCatalog());
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
