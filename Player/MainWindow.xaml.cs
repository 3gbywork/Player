using GalaSoft.MvvmLight.Command;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Linq;
using System.ComponentModel;

namespace Player
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        const string CONFIG_FILE = "bookmarks.json";

        public ICommand DeleteCommand { get; set; }
        public ObservableCollection<Bookmark> Bookmarks { get; set; }

        BookmarkConfig bookmarkConfig;

        public MainWindow()
        {
            InitializeComponent();

            InitBookmarkConfig();

            InitCommand();
        }

        private void InitCommand()
        {
            DeleteCommand = new RelayCommand<Bookmark>(DeleteBookmark);
        }

        private void DeleteBookmark(Bookmark bookmark)
        {
            if (null == bookmark) return;

            Bookmarks.Remove(bookmark);
        }

        private void InitBookmarkConfig()
        {
            Bookmarks = new ObservableCollection<Bookmark>();

            if (!File.Exists(CONFIG_FILE)) return;

            var configStr = File.ReadAllText(CONFIG_FILE, Encoding.UTF8);
            bookmarkConfig = JsonConvert.DeserializeObject<BookmarkConfig>(configStr);
            if (null == bookmarkConfig || null == bookmarkConfig.Bookmarks || bookmarkConfig.Bookmarks.Length == 0) return;

            foreach (var item in bookmarkConfig.Bookmarks)
            {
                Bookmarks.Add(item);
            }
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            var title = Browser.Title;
            var url = Browser.Address;
            if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(url)) return;

            var find = Bookmarks.FirstOrDefault(b => b.Url == url);
            if (null != find) return;

            Bookmarks.Add(new Bookmark { Title = title, Url = url });
        }

        private void ListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left) return;

            if (sender is ListBox list)
            {
                var index = list.SelectedIndex;
                if (index < 0) return;

                var url = Bookmarks[index].Url;
                if (Browser.Address != url)
                {
                    Browse.Text = url;
                }
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            if (null != bookmarkConfig && Bookmarks.Count > 0)
            {
                bookmarkConfig.Bookmarks = Bookmarks.ToArray();
                var configStr = JsonConvert.SerializeObject(bookmarkConfig);
                File.WriteAllText(CONFIG_FILE, configStr);
            }
        }
    }
}
