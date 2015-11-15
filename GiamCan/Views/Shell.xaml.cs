using GiamCan.Model;
using System;
using System.Collections.Generic;
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
        SQLite.Net.SQLiteConnection conn;
        NguoiDung nguoidung;
        MucTieu muctieu;
        ThongKeNgay thongkengay;
        public Shell()
        {
            this.InitializeComponent();
            string path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "giamcandb.sqlite");
            conn = new SQLite.Net.SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            nguoidung = e.Parameter as NguoiDung;
            // kiem tra ngay hien tai da co trong db chua, chua co thi them vao
            muctieu = conn.Table<MucTieu>().Where(r => r.TenDangNhap == nguoidung.TenDangNhap && (r.TrangThai == "Đã bắt đầu" || r.TrangThai == "Chưa bắt đầu")).FirstOrDefault();

            string today = DateTime.Today.ToString("dd/MM/yyyy");
            
            thongkengay = (muctieu != null) ? conn.Table<ThongKeNgay>().Where(r => r.IdMucTieu == muctieu.IdMucTieu && r.Ngay == today).FirstOrDefault() : null;
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

        private async void baitapButton_Click(object sender, RoutedEventArgs e)
        {
            var frame = this.DataContext as Frame;
            Page page = frame?.Content as Page;
            if (page?.GetType() != typeof(DanhSachBaiTap))
            {
                if(muctieu != null)
                {
                    frame.Navigate(typeof(DanhSachBaiTap), muctieu);
                }
                else
                {
                    MessageDialog msDialog = new MessageDialog("Bạn phải bắt đầu mục tiêu của mình trước");
                    await msDialog.ShowAsync();
                }
            }
        }

        private void chedoanButton_Click(object sender, RoutedEventArgs e)
        {
            var frame = this.DataContext as Frame;
            Page page = frame?.Content as Page;
            if (page?.GetType() != typeof(MonAnPage))
            {
                if (thongkengay != null)
                {
                    frame.Navigate(typeof(MonAnPage), thongkengay);
                }
            }
        }

        private async void thongkeButton_Click(object sender, RoutedEventArgs e)
        {
            var frame = this.DataContext as Frame;
            Page page = frame?.Content as Page;
            if (page?.GetType() != typeof(ThongKePage))
            {
                if (muctieu != null)
                {
                    frame.Navigate(typeof(ThongKePage), muctieu);
                }
                else
                {
                    MessageDialog msDialog = new MessageDialog("Bạn phải bắt đầu mục tiêu của mình trước");
                    await msDialog.ShowAsync();
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
