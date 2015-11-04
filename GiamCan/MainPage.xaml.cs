using GiamCan.Model;
using GiamCan.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace GiamCan
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        string path;
        SQLite.Net.SQLiteConnection connection;
        public MainPage()
        {
            this.InitializeComponent();
            path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "giamcandb.sqlite");
            connection = new SQLite.Net.SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path);
            // tao bang NguoiDung
            connection.CreateTable<NguoiDung>();
            // tao bang MucTieu
            connection.CreateTable<MucTieu>();
            // tao bang MonAn
            connection.CreateTable<MonAn>();
            connection.CreateTable<ThongKeNgay>();
            connection.CreateTable<ThongKeBaiTap>();
            connection.CreateTable<ThucDon>();
            connection.CreateTable<BaiTap>();

        }

        private async void dangnhapButton_Click(object sender, RoutedEventArgs e)
        {
            //kiem tra xem co nguoidung khong
            NguoiDung nguoidung = connection.Query<NguoiDung>("SELECT * FROM NGUOIDUNG WHERE tenDangNhap=?", tendangnhapTextBox.Text).FirstOrDefault();
            if (nguoidung != null && nguoidung.TenDangNhap == tendangnhapTextBox.Text && nguoidung.MatKhau == matkhauTextBox.Text)
            {
                // kiem tra da co muc tieu chua, neu chua co thi chuyen den trang taomuctieu moi
                MucTieu muctieu = connection.Query<MucTieu>("SELECT * FROM MUCTIEU WHERE TenDangNhap =? and MucHoanThanh=0", nguoidung.TenDangNhap).FirstOrDefault();
                if (muctieu == null)
                {
                    MessageDialog msDialog = new MessageDialog("Bạn chưa chọn mục tiêu nào!");
                    await msDialog.ShowAsync();
                    Frame.Navigate(typeof(TaoMoiMucTieu), nguoidung);
                }
                else
                {
                    Frame.Navigate(typeof(TrangChu), nguoidung);
                }
            }
            else
            {
                MessageDialog msDialog = new MessageDialog("Tên đăng nhập hoặc Mật khẩu không đúng!");
                await msDialog.ShowAsync();
            }

            

        }

        private void dangkyButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(DangKy));
        }
    }
}
