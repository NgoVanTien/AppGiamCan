using GiamCan.Model;
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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace GiamCan.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TrangChu : Page
    {
        string path;
        SQLite.Net.SQLiteConnection conn;
        NguoiDung nguoidung;
        MucTieu muctieuhientai;
        ThongKeNgay thongkengay;
        public TrangChu()
        {
            this.InitializeComponent();
            path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "giamcandb.sqlite");
            conn = new SQLite.Net.SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path);
        }

        private void nhacnhoButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(DatNhacNho), nguoidung);
        }

        private async void muctieumoiButton_Click(object sender, RoutedEventArgs e)
        {
            MessageDialog msDialog = new MessageDialog("TẠO MỚI MỤC TIÊU\nBạn sẽ hủy mục tiêu hiện tại?");

            msDialog.Commands.Add(new UICommand("Đồng ý") { Id = 0 });
            msDialog.Commands.Add(new UICommand("Hủy") { Id = 1 });
            msDialog.DefaultCommandIndex = 1;
            var result = await msDialog.ShowAsync();
            if (result.Label == "Đồng ý")
            {
                Frame.Navigate(typeof(TaoMoiMucTieu), nguoidung);
            }
            
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // lay nguoidung tu trang dang nhap vao
            nguoidung = (NguoiDung) e.Parameter;

            // kiem tra ngay hien tai da co trong db chua, chua co thi them vao
            muctieuhientai = conn.Table<MucTieu>().Where(r => r.TenDangNhap == nguoidung.TenDangNhap).FirstOrDefault();
            string today = DateTime.Today.ToString("dd/MM/yyyy");
            thongkengay = conn.Table<ThongKeNgay>().Where(r => r.Ngay == today).FirstOrDefault();
            if(thongkengay == null)
            {
                thongkengay = new ThongKeNgay();
                thongkengay.IdMucTieu = muctieuhientai.IdMucTieu;
                thongkengay.Ngay = today;
                conn.Insert(thongkengay);
            }
        }

        private void thongtinButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ThongTinCaNhan), nguoidung);
        }

        private void baitapButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(DanhSachBaiTap));
        }

        private void thongkeButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ThongKePage), muctieuhientai);
        }

        private void chedoanButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MonAnPage), thongkengay);
        }
    }
}
