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
        MucTieu muctieuhientai;
        public DanhSachBaiTap()
        {
            this.InitializeComponent();
            path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "giamcandb.sqlite");
            connection = new SQLite.Net.SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            muctieuhientai = e.Parameter as MucTieu;
            if(muctieuhientai.ThoiGianBatDau==null || muctieuhientai.TrangThai=="Chưa bắt đầu")
            {
                muctieuhientai.TrangThai = "Đã bắt đầu";
                muctieuhientai.ThoiGianBatDau = DateTime.Today.ToString("dd/MM/yyyy");
                connection.Update(muctieuhientai);
            }
           
        }

        private void chayboButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ChayBo_Version1));
        }

        private void dapxeButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(DapXe));
        }
    }
}
