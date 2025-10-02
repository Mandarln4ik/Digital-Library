using Digital_Library.Services;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.Relational;
using System;
using System.Collections.Generic;
using System.Data;
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
    /// Логика взаимодействия для AdminWindow.xaml
    /// </summary>
    public partial class AdminWindow : Window
    {
        private Dictionary<string, DataTable> tables = new();
        private Dictionary<string, MySqlDataAdapter> adapters = new Dictionary<string, MySqlDataAdapter>();


        public AdminWindow()
        {
            InitializeComponent();
        }

        private async Task LoadDataBaseAsync()
        {
            DBservice db = new DBservice();
            List<string> keys = new List<string>();

            DataTable keysTables = await db.GetResultsFromSQL("SHOW TABLES;");

            foreach (DataRow row in keysTables.Rows)
            {
                keys.Add(row.ItemArray[0].ToString());
                Debug.WriteLine(row.ItemArray[0].ToString());
            }

            for (int i = 0; i < keys.Count; i++)
            {
                string s = keys[i].ToString();
                Task.Run(() => CreateTable(s));
            }
        }

        private async Task CreateTable(string tableName)
        {
            MySqlDataAdapter adapter = await CreateDataAdapter(tableName);
            DataTable table = new DataTable();

            await Task.Run(() => adapter.Fill(table));

            adapters.Add(tableName, adapter);
            tables.Add(tableName, table);

            await Dispatcher.BeginInvoke(new Action(() =>
            {
                TabItem tabItem = new TabItem();
                tabItem.Header = tableName;

                StackPanel stackPanel = new StackPanel();

                ScrollViewer scrollViewer = new ScrollViewer();
                scrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
                scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
                scrollViewer.Height = MaxHeight - 40;

                DataGrid dataGrid = new DataGrid();
                dataGrid.ItemsSource = table.DefaultView;
                dataGrid.AutoGenerateColumns = true;
                dataGrid.CanUserAddRows = true;
                dataGrid.CanUserDeleteRows = true;
                dataGrid.IsReadOnly = false;

                scrollViewer.Content = dataGrid;

                tabItem.Content = stackPanel;
                TabControl.Items.Add(tabItem);

                StackPanel buttonPanel = new StackPanel() { Orientation = Orientation.Horizontal };

                Button saveButton = new Button() { Content = "Сохранить", Margin = new Thickness(5) };
                saveButton.Click += async (s, e) => await SaveWithAdapter(tableName, table);

                Button reloadButton = new Button() { Content = "Обновить", Margin = new Thickness(5) };
                reloadButton.Click += async (s, e) => await ReloadTable(tableName, table);

                stackPanel.Children.Add(buttonPanel);
                stackPanel.Children.Add(scrollViewer);

                buttonPanel.Children.Add(saveButton);
                buttonPanel.Children.Add(reloadButton);
            }));
        }

        private async Task<MySqlDataAdapter> CreateDataAdapter(string tableName)
        {
            DBservice db = new DBservice();
            var connection = db.GetConnection(); 

            MySqlDataAdapter adapter = new MySqlDataAdapter($"SELECT * FROM `{tableName}`", connection);

            MySqlCommandBuilder commandBuilder = new MySqlCommandBuilder(adapter);

            adapter.InsertCommand = commandBuilder.GetInsertCommand();
            adapter.UpdateCommand = commandBuilder.GetUpdateCommand();
            adapter.DeleteCommand = commandBuilder.GetDeleteCommand();

            return adapter;
        }

        private async Task SaveWithAdapter(string tableName, DataTable table)
        {
            try
            {
                var adapter = adapters[tableName];
                int rowsAffected = await Task.Run(() => adapter.Update(table));
                MessageBox.Show($"Сохранено измененных строк: {rowsAffected}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}");
            }
        }

        private async Task ReloadTable(string tableName, DataTable table)
        {
            try
            {
                var adapter = adapters[tableName];
                table.Clear();
                adapter.Fill(table);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Task.Run(LoadDataBaseAsync);
        }

        AddDocumentWindow addDocWindow;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (addDocWindow == null)
            {
                addDocWindow = new();
            }
            addDocWindow.Show();
        }
    }
}
