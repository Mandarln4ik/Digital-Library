using Digital_Library.Model;
using Digital_Library.Services;
using System;
using System.Collections.Generic;
using System.Data;
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
    /// Логика взаимодействия для TagsWindow.xaml
    /// </summary>
    public partial class TagsWindow : Window
    {
        public List<Tag> tags = new();
        public DataTable tagsTable;

        public TagsWindow()
        {
            InitializeComponent();
            LoadTagsData();
        }

        public async Task LoadTagsData()
        {
            DBservice db = new DBservice();
            tagsTable = await db.GetResultsFromSQL("SELECT * FROM `tags`");
            DataGrid.ItemsSource = tagsTable.DefaultView;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (DataGrid.SelectedItems.Count < 1)
            {
                MessageBox.Show("Выберите хотя бы 1 строку!");
            }
            else
            {
                foreach (var item in DataGrid.SelectedItems)
                {
                    if (item is DataRowView row)
                    {
                        
                        tags.Add(new Tag()
                        {
                            TagId = Convert.ToInt32(row[0]),
                            TagName = Convert.ToString(row[1])
                        });
                            
                    }
                }
                if (tags.Count > 0)
                {
                    DialogResult = true;
                }
            }
        }
    }
}
