using GiamCan.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace GiamCan.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DanhSachBaiTap : Page
    {

        string path;
        SQLite.Net.SQLiteConnection connection;
        public DanhSachBaiTap()
        {
            this.InitializeComponent();
            path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "giamcandb.sqlite");
            connection = new SQLite.Net.SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            IEnumerable<BaiTap> baitapCollection = connection.Table<BaiTap>();
            this.DataContext = baitapCollection;
        }

        private void baitapGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            // xử lý cái này Item Click là gì?
            //Frame.Navigate(typeof(BTChayBoPage));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(BTChayBoPage));
        }

        //private void uncheckallButton_Click(object sender, RoutedEventArgs e)
        //{
        //    var checkboxes = AllChildren(baitapGridView).OfType<CheckBox>();
        //    foreach (var checkbox in checkboxes)
        //    {
        //        checkbox.IsChecked = false;   
        //    }
        //}
        //private List<Control> AllChildren(DependencyObject parent)
        //{
        //    var _List = new List<Control>();
        //    for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
        //    {
        //        var _Child = VisualTreeHelper.GetChild(parent, i);
        //        if (_Child is Control)
        //        {
        //            _List.Add(_Child as Control);
        //        }
        //        _List.AddRange(AllChildren(_Child));
        //    }
        //    return _List;
        //}
    }
}
