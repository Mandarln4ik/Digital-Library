using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// Логика взаимодействия для DocumentReader.xaml
    /// </summary>
    public partial class DocumentReader : Window
    {
        string Path;
        public DocumentReader(string documentPath, string title)
        {
            InitializeComponent();
            Title = title;
            if (documentPath == null)
            {
                Close();
            }
            Path = documentPath;
            InitializeAsync();
        }

        private async void InitializeAsync()
        {
            var environment = await CoreWebView2Environment.CreateAsync();
            await WebView.EnsureCoreWebView2Async(environment);

            // Разрешаем доступ к локальным файлам
            WebView.CoreWebView2.Settings.IsWebMessageEnabled = true;
            WebView.CoreWebView2.Settings.AreDefaultScriptDialogsEnabled = true;

            LoadPdfFile();
        }

        public void LoadPdfFile()
        {
            if (File.Exists(Path))
            {
                WebView.Source = new Uri(Path);
            }
        }
    }
}
