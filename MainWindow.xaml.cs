using Digital_Library.Model;
using Digital_Library.Model.Search;
using Digital_Library.Properties;
using Digital_Library.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static System.Net.WebRequestMethods;

namespace Digital_Library
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public bool isBookDownloaded = false;
        public bool isProfileMenuOpen = false;
        public bool isProfileEditing = false;
        public bool isLoggedOn = false;
        Login LogInWindow;

        List<Tag> tags = new();

        List<Document> documents = new();
        ObservableCollection<DocumentView> documentViews { get; set; }
        ObservableCollection<DocumentView> filtredDocumentViews { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            DocumentInfo.Visibility = Visibility.Collapsed;
            ProfileGrid.Visibility = Visibility.Collapsed;
            documentViews = new ObservableCollection<DocumentView>();

            if (Settings.Default.IsLoggedOn)
            {
                LogIn();
            }

            CatalogList.DataContext = this;
            CatalogList.ItemsSource = documentViews;

            NicknameTBox.Visibility = Visibility.Collapsed;
            EmailTBox.Visibility = Visibility.Collapsed;
            NameTBox.Visibility = Visibility.Collapsed;
            NameTBox.Text = "       ";
            LastNameTBox.Visibility = Visibility.Collapsed;
            LastNameTBox.Text = "       ";
        }

        async void LogIn()
        {
            Account acc = Account.GetInstance();
            (bool, string) result = await acc.TryLogIn(Settings.Default.Login, Settings.Default.PasswordHash);
            if (result.Item1)
            {
                UpdateAccountInfo();
            }
        }

        private void AccountButton_Click(object sender, RoutedEventArgs e)
        {
            if (!isLoggedOn)
            {
                if (LogInWindow == null) { LogInWindow = new Login(); }
                LogInWindow.ShowDialog();

                if (LogInWindow.DialogResult == true)
                {
                    UpdateAccountInfo();
                    isLoggedOn = true;
                }
                LogInWindow = null;
            }
            else
            {
                if (!isProfileMenuOpen)
                {
                    UpdateAccountInfo();
                    ProfileGrid.Visibility = Visibility.Visible;
                    isProfileMenuOpen = true;
                }
                else
                {
                    ProfileGrid.Visibility = Visibility.Collapsed;
                    isProfileMenuOpen = false;
                }
            }
        }

        public void UpdateAccountInfo()
        {
            Account acc = Account.GetInstance();
            if (acc.currentUser == null) 
            {
                ProfileLogIn.Content = "Войти";
                return; 
            }
            ProfileLogIn.Content = acc.currentUser.Username;
            isLoggedOn = true;

            NicknameTBlock.Text = acc.currentUser.Username;
            NicknameTBlock.DataContext = acc.currentUser;
            EmailTBlock.Text = acc.currentUser.Email;
            EmailTBlock.DataContext = acc.currentUser;
            NameTBlock.Text = acc.currentUser.FirstName;
            NameTBlock.DataContext = acc.currentUser;
            LastNameTBlock.Text = acc.currentUser.LastName;
            LastNameTBlock.DataContext = acc.currentUser;

            OpenAP.Visibility = acc.currentUser.isAdmin ? Visibility.Visible : Visibility.Collapsed;
        }

        public async Task GetCatalog()
        {
            documents.Clear();
            tags.Clear();
            documentViews.Clear();

            DBservice db = new DBservice();

            tags = db.GetAllTags();

            documents = db.GetAllDocuments();
            Debug.Write($"Tags: {tags.Count}, Docs: {documents.Count}");

            foreach (var item in documents)
            {
                Task.Run(() => GetDocView(db, item));
            }

            Debug.Write($"Views: {documentViews.Count}");
        }

        public async Task GetDocView(DBservice db, Document item)
        {
            string fullname;
            Author a = db.GetAuthorByDocumentId(item.DocumentId);
            fullname = $"{a.FirstName} {a.LastName} {a.MiddleName}";

            List<Tag> Tags = new();

            List<DocumentTag> d = db.GetAllTagsToDocument(item.DocumentId);

            foreach (var item1 in d)
            {
                Tags.Add(tags[item1.TagId - 1]);
            }

            DocumentView DV = new DocumentView()
            {
                DocumentId = item.DocumentId,
                Title = item.Title,
                Authors = fullname,
                PublicationYear = (int)item.PublicationYear,
                Language = db.GetLanguageById(item.LanguageId).LanguageName,
                Description = item.Description,
                Tags = Tags,
                IsInCollection = false,
            };

            await Dispatcher.BeginInvoke(new Action(() =>
            {
                documentViews.Add(DV);
            }));
            string imgPath = await new DocumentDownloader().DownloadCover(item.ImagePath);
            documentViews.Where(d => d.DocumentId == item.DocumentId).FirstOrDefault().ImagePath = imgPath;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await GetCatalog();
        }

        private void BeginEditProfile()
        {
            NicknameTBlock.Visibility = Visibility.Collapsed;
            EmailTBlock.Visibility = Visibility.Collapsed;
            NameTBlock.Visibility = Visibility.Collapsed;
            LastNameTBlock.Visibility = Visibility.Collapsed;

            NicknameTBox.Visibility = Visibility.Visible;
            NicknameTBox.Text = NicknameTBlock.Text;
            EmailTBox.Visibility = Visibility.Visible;
            EmailTBox.Text = EmailTBlock.Text;
            NameTBox.Visibility = Visibility.Visible;
            NameTBox.Text = NameTBlock.Text;
            LastNameTBox.Visibility = Visibility.Visible;
            LastNameTBox.Text = LastNameTBlock.Text;
        }

        private async Task EndEditProfile()
        {
            await SaveProfileAsync();
            NicknameTBox.Visibility = Visibility.Collapsed;
            EmailTBox.Visibility = Visibility.Collapsed;
            NameTBox.Visibility = Visibility.Collapsed;
            LastNameTBox.Visibility = Visibility.Collapsed;

            NicknameTBlock.Visibility = Visibility.Visible;
            EmailTBlock.Visibility = Visibility.Visible;
            NameTBlock.Visibility = Visibility.Visible;
            LastNameTBlock.Visibility = Visibility.Visible;

        }

        private async Task SaveProfileAsync()
        {
            User user = new User()
            {
                Username = NicknameTBox.Text,
                Email = EmailTBox.Text,
                FirstName = NameTBox.Text,
                LastName = LastNameTBox.Text,
            };

            Account acc = Account.GetInstance();
            await acc.SaveProfile(user);
            UpdateAccountInfo();
        }

        private async void Image_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!isProfileEditing)
            {
                BeginEditProfile();
                isProfileEditing = true;
            }
            else
            {
                await EndEditProfile();
                isProfileEditing = false;
            }
        }

        private void SearchTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(SearchTextBox.Text))
            {
                //выключаем сортировку
                CatalogList.ItemsSource = documentViews;
            }
            else
            {
                //включаем сортировку
                List<DocumentView> documentView = new();
                documentView = documentViews.Where(d => d.Title.ToLower().Contains(SearchTextBox.Text.ToLower()) ||
                d.Description.ToLower().Contains(SearchTextBox.Text.ToLower()) ||
                d.Tags.Where(t => t.TagName.ToLower().Contains(SearchTextBox.Text.ToLower())).FirstOrDefault() != null).ToList();

                filtredDocumentViews = new(documentView);

                CatalogList.ItemsSource = filtredDocumentViews;
            }
        }

        private async void CatalogList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            DocumentView doc = CatalogList.SelectedItem as DocumentView;
            if (doc != null)
            {
                OpenDocumentInfo(doc);
            }
            CatalogList.SelectedItem = null;
        }

        Document selectedDocument;

        private void OpenDocumentInfo(DocumentView doc)
        {
            Document document = documents.FirstOrDefault(d => d.DocumentId == doc.DocumentId);
            selectedDocument = document;
            DocumentDownloader dd = new();


            if (dd.CheckForFileExists(document.FilePath))
            {
                ReadBookText.Text = "Читать";
                isBookDownloaded = true;
                DownloadProgressBar.Visibility = Visibility.Collapsed;
            }
            else
            {
                ReadBookText.Text = "Скачать";
                isBookDownloaded = false;
                DownloadProgressBar.Visibility = Visibility.Visible;
            }

            DocumentInfo.Visibility = Visibility.Visible;
            InfoImage.DataContext = doc;

            InfoTags.DataContext = doc;
            InfoTags.ItemsSource = doc.Tags;

            Author.Text = doc.Authors;

            Year.Text = doc.PublicationYear.ToString();

            DocumentTitle.Text = doc.Title;

            DocumentAlternativeTitle.Text = document.AlternativeTitle;

            if (!String.IsNullOrEmpty(document.ISBN))
            {
                ISBN.Text = document.ISBN;
            }
            else
            {
                ISBN.Text = "Отсутствует";
            }

            if (!String.IsNullOrEmpty(document.ISSN))
            {
                ISSN.Text = document.ISBN;
            }
            else
            {
                ISSN.Text = "Отсутствует";
            }

            Description.Text = document.Description;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DocumentInfo.Visibility = Visibility.Collapsed;
        }

        private async void Label_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Account acc = Account.GetInstance();
            await acc.LogOut();
            isLoggedOn = false;
            ProfileGrid.Visibility = Visibility.Collapsed;
            isProfileMenuOpen = false;
            UpdateAccountInfo();
        }

        private void ReadBook_Click(object sender, RoutedEventArgs e)
        {
            OpenBook();
        }

        private async Task OpenBook()
        {
            DocumentDownloader dd = new();

            if (isBookDownloaded)
            {
                DocumentReader window = new DocumentReader(dd.GetLocalFilePath(selectedDocument.FilePath), selectedDocument.Title);
                window.Show();
            }
            else
            {
                IProgress<double> progress = new Progress<double>(value =>
                {
                    // Обновление прогресс-бара
                    DownloadProgressBar.Value = value;
                });

                ReadBook.IsEnabled = false;
                _ = await dd.DownloadDocument(selectedDocument.FilePath, progress);
                ReadBook.IsEnabled = true;
                isBookDownloaded = true;
                ReadBookText.Text = "Читать";
                DownloadProgressBar.Visibility = Visibility.Collapsed;
                DownloadProgressBar.Value = 0;
            }
        }

        AdminWindow window;

        private void OpenAP_Click(object sender, RoutedEventArgs e)
        {
            if (window == null)
            {
                window = new AdminWindow();
            }
            window.Show();
            ProfileGrid.Visibility = Visibility.Collapsed;
            isProfileMenuOpen = false;
        }
    }
}