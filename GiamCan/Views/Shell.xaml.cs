using GiamCan.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.IsolatedStorage;
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
    public sealed partial class Shell : Page
    {
        SQLite.Net.SQLiteConnection connection;
        NguoiDung nguoidung;
        MucTieu muctieu;
        ThongKeNgay thongkengay;
        public Shell()
        {
            this.InitializeComponent();
            string path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "giamcandb.sqlite");
            connection = new SQLite.Net.SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            nguoidung = e.Parameter as NguoiDung;
            // kiem tra ngay hien tai da co trong db chua, chua co thi them vao
            muctieu = connection.Table<MucTieu>().Where(r => r.TenDangNhap == nguoidung.TenDangNhap && (r.TrangThai == "Đã bắt đầu" || r.TrangThai == "Chưa bắt đầu")).FirstOrDefault();

            string today = DateTime.Today.ToString("dd/MM/yyyy");
            
            thongkengay = (muctieu != null) ? connection.Table<ThongKeNgay>().Where(r => r.IdMucTieu == muctieu.IdMucTieu && r.Ngay == today).FirstOrDefault() : null;

            tendangnhapTextBlock.Text = nguoidung.TenDangNhap;
            tuoiTextBlock.Text = (DateTime.Today.Year - DateTime.ParseExact(nguoidung.NgaySinh, "dd/MM/yyyy", new CultureInfo("vi-vn")).Year).ToString();
            gioitinhTextBlock.Text = nguoidung.GioiTinh;
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Frame rootFrame = null;
            if(rootFrame == null)
            {
                rootFrame = new Frame();
            }
            this.DataContext = rootFrame;
            if(rootFrame.Content == null)
            {
                rootFrame.Navigate(typeof(TrangChu), nguoidung);
            }

            (this.DataContext as Frame).Navigated += OnNavigated;
        }

   

        private void OnNavigated(object sender, NavigationEventArgs e)
        {
            this.SplitView.IsPaneOpen = false;

            if ((sender as Frame).Content.GetType() == typeof(TrangChu))
            {
                var tc = (sender as Frame).Content as TrangChu;
                if(e.Parameter.GetType() == typeof(NguoiDung))
                {
                    nguoidung = e.Parameter as NguoiDung;
                    muctieu = TrangChu.getMucTieuHienTai(nguoidung);

                    thongkengay = TrangChu.getThongKeNgayHienTai(muctieu);

                    tendangnhapTextBlock.Text = nguoidung.TenDangNhap;
                    tuoiTextBlock.Text = (DateTime.Today.Year - DateTime.ParseExact(nguoidung.NgaySinh, "dd/MM/yyyy", new CultureInfo("vi-vn")).Year).ToString();
                    gioitinhTextBlock.Text = nguoidung.GioiTinh;
                }
            }
                // neu tro ve trang MainPage thi loại bỏ shell
                if ((sender as Frame).Content.GetType() == typeof(MainPage))
            {
                Frame.Navigate(typeof(MainPage));
                Frame.BackStack.Clear();
            }
        }

        private void HamburgerRadioButton_Click(object sender, RoutedEventArgs e)
        {
            if (!this.SplitView.IsPaneOpen)
            {
                this.SplitView.IsPaneOpen = true;
            }
        }

        private void trangchuButton_Click(object sender, RoutedEventArgs e)
        {
            var frame = this.DataContext as Frame;
            Page page = frame?.Content as Page;
            if (page?.GetType() != typeof(TrangChu))
            {
                frame.Navigate(typeof(TrangChu), nguoidung);
            }
        }

        private void baitapButton_Click(object sender, RoutedEventArgs e)
        {
            var frame = this.DataContext as Frame;
            Page page = frame?.Content as Page;
            if (page?.GetType() != typeof(DanhSachBaiTap))
            {
                if(nguoidung != null)
                {
                    frame.Navigate(typeof(DanhSachBaiTap), nguoidung);
                }
            }
        }

        private void chedoanButton_Click(object sender, RoutedEventArgs e)
        {
            var frame = this.DataContext as Frame;
            Page page = frame?.Content as Page;
            if (page?.GetType() != typeof(MonAnPage))
            {
                if (nguoidung != null)
                {
                    frame.Navigate(typeof(MonAnPage), nguoidung);
                }
            }
        }

        private void thongkeButton_Click(object sender, RoutedEventArgs e)
        {
            var frame = this.DataContext as Frame;
            Page page = frame?.Content as Page;
            if (page?.GetType() != typeof(ThongKePage))
            {
                if (nguoidung != null)
                {
                    frame.Navigate(typeof(ThongKePage), nguoidung);
                }
            }
        }

        private void datnhacnhoButton_Click(object sender, RoutedEventArgs e)
        {
            var frame = this.DataContext as Frame;
            Page page = frame?.Content as Page;
            if (page?.GetType() != typeof(DatNhacNho))
            {
                frame.Navigate(typeof(DatNhacNho), nguoidung);
            }
        }

        private void dangxuatButton_Click(object sender, RoutedEventArgs e)
        {
            IsolatedStorageFile ISOFile = IsolatedStorageFile.GetUserStoreForApplication();
            if (ISOFile.FileExists("CurrentUser"))
            {
                ISOFile.DeleteFile("CurrentUser");
            }

            Frame.Navigate(typeof(MainPage));
            Frame.BackStack.Clear();
        }
    }
}
