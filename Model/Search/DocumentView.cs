using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Digital_Library.Model.Search
{
    internal class DocumentView : INotifyPropertyChanged
    {
        public int DocumentId { get; set; }
        public string Title { get; set; }
        public string Authors { get; set; } // Форматированная строка авторов
        public int PublicationYear { get; set; }
        public string Language { get; set; }

        private string _imagePath;
        public string ImagePath
        {
            get => _imagePath;
            set { _imagePath = value; OnPropertyChanged(); }
        }

        public string Description { get; set; }
        public List<Tag> Tags { get; set; }
        public bool IsInCollection { get; set; }


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}