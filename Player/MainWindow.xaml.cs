using GalaSoft.MvvmLight.Command;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Messages;
using ToastNotifications.Position;

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

        Notifier notifier;

        public MainWindow()
        {
            InitializeComponent();

            InitBookmarkConfig();

            InitCommand();

            InitToast();
        }

        private void InitToast()
        {
            notifier = new Notifier(cfg =>
            {
                cfg.PositionProvider = new WindowPositionProvider(
                    parentWindow: Application.Current.MainWindow,
                    corner: Corner.TopRight,
                    offsetX: 0,
                    offsetY: 40);

                cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
                    notificationLifetime: TimeSpan.FromSeconds(1.5),
                    maximumNotificationCount: MaximumNotificationCount.UnlimitedNotifications());

                cfg.Dispatcher = Application.Current.Dispatcher;
            });
        }

        private void InitCommand()
        {
            DeleteCommand = new RelayCommand<Bookmark>(DeleteBookmark);
        }

        private void DeleteBookmark(Bookmark bookmark)
        {
            if (null == bookmark) return;

            Bookmarks.Remove(bookmark);

            notifier.ShowInformation("书签已删除！");
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
            if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(url))
            {
                notifier.ShowWarning("网址不能为空！");
                return;
            }

            var find = Bookmarks.FirstOrDefault(b => b.Url == url);
            if (null != find)
            {
                notifier.ShowInformation("书签已存在！");
                return;
            }

            Bookmarks.Add(new Bookmark { Title = title, Url = url });

            notifier.ShowInformation("书签已添加!");
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
                    Browser.Address = url;
                }

                Bookmark.IsChecked = false;
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

            notifier.Dispose();
        }
    }
}
