using GiamCan.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Runtime.CompilerServices;
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
using WinRTXamlToolkit.Controls.DataVisualization.Charting;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace GiamCan.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TrangChu : Page, INotifyPropertyChanged
    {

        public static SQLite.Net.SQLiteConnection connection;
        public static NguoiDung nguoidung;
        public static MucTieu muctieu;
        public static ThongKeNgay thongkengay;
        public TrangChu()
        {
            this.InitializeComponent();
            string path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "giamcandb.sqlite");
            connection = new SQLite.Net.SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path);
        }
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            try
            {
                // lay nguoidung tu trang dang nhap vao
                nguoidung = (NguoiDung)e.Parameter;

                // lay muctieu hien tai cua nguoidung
                muctieu = getMucTieuHienTai(nguoidung);
            }
            catch (Exception)
            {
                MessageDialog msDialog = new MessageDialog("Một số lỗi đã xảy ra");
                await msDialog.ShowAsync();
                Frame.Navigate(typeof(MainPage));
            }
        }

        /// <summary>
        /// lấy mục tiêu hiện tại của người dùng
        /// </summary>
        /// <param name="nd">người dùng</param>
        /// <returns>mục tiêu hiện tại</returns>
        public static MucTieu getMucTieuHienTai(NguoiDung nd)
        {
            MucTieu muctieu = connection.Table<MucTieu>().Where(r => r.TenDangNhap == nd.TenDangNhap && (r.TrangThai == "Đã bắt đầu" || r.TrangThai == "Chưa bắt đầu")).FirstOrDefault();
            return muctieu;
        }

        /// <summary>
        /// lấy thống kê ngày hiện tại 
        /// </summary>
        /// <param name="mtht">mục tiêu hiện tại</param>
        /// <returns>thống kê ngày</returns>
        public static ThongKeNgay getThongKeNgayHienTai(MucTieu mtht)
        {
            string today = DateTime.Today.ToString("dd/MM/yyyy");

            ThongKeNgay thongkengay = mtht != null ? connection.Table<ThongKeNgay>().Where(r => r.IdMucTieu == mtht.IdMucTieu && r.Ngay == today).FirstOrDefault() : null;
            return thongkengay;
        }
        private void nhacnhoButton_Click(object sender, RoutedEventArgs e)
        {
            if (nguoidung != null)
                Frame.Navigate(typeof(DatNhacNho), nguoidung);
        }

        private void muctieumoiButton_Click(object sender, RoutedEventArgs e)
        {
            denTaoMucTieuMoiPage();
        }

        private void thongtinButton_Click(object sender, RoutedEventArgs e)
        {
            if (nguoidung != null)
                Frame.Navigate(typeof(ThongTinCaNhan), nguoidung);
        }

        private async void baitapButton_Click(object sender, RoutedEventArgs e)
        {
            if (nguoidung != null)
            {
                MessageDialog msDialog = new MessageDialog("Bạn vẫn chưa có mục tiêu nào!");
                msDialog.Commands.Add(new UICommand("Tạo mục tiêu", r => Frame.Navigate(typeof(TaoMoiMucTieu), nguoidung)));
                msDialog.Commands.Add(new UICommand("Để sau", r => Frame.Navigate(typeof(DanhSachBaiTap), nguoidung)));
                await msDialog.ShowAsync();
                
            }
                
        }

        private void thongkeButton_Click(object sender, RoutedEventArgs e)
        {
            if (nguoidung != null)
                Frame.Navigate(typeof(ThongKePage), nguoidung);
        }

        private void chedoanButton_Click(object sender, RoutedEventArgs e)
        {
            if (nguoidung != null)
                Frame.Navigate(typeof(MonAnPage), nguoidung);
        }

        private async void denTaoMucTieuMoiPage()
        {
            // kiem tra muctieu da hoan thanh chua, neu chua thi se hien len thong bao
            if (muctieu != null && muctieu.TrangThai != "Hoàn thành")
            {
                MessageDialog msDialog = new MessageDialog("TẠO MỚI MỤC TIÊU\nBạn sẽ hủy mục tiêu hiện tại?");

                msDialog.Commands.Add(new UICommand("Đồng ý") { Id = 0 });
                msDialog.Commands.Add(new UICommand("Trở về") { Id = 1 });
                msDialog.DefaultCommandIndex = 1;
                var result = await msDialog.ShowAsync();
                if (result.Label == "Đồng ý")
                {
                    Frame.Navigate(typeof(TaoMoiMucTieu), nguoidung);
                }
                else
                {
                    return;
                }
            }
            Frame.Navigate(typeof(TaoMoiMucTieu), nguoidung);
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

        private async void muctieunull()
        {
            MessageDialog msDialog = new MessageDialog("Bạn chưa bắt đầu một mục tiêu nào!");
            msDialog.Commands.Add(new UICommand("Bắt đầu ngay"));
            msDialog.Commands.Add(new UICommand("Để sau"));
            var result = await msDialog.ShowAsync();
            if (result.Label != "Để sau")
            {
                denTaoMucTieuMoiPage();
            }
        }
        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {

            // neu muctieu == null ---> tuc nguoidung chua co muc tieu nao hien tai
            if (muctieu == null)
            {
                // an header dau tien
                calsGiamGrid.Visibility = Visibility.Collapsed;

            }
            else
            {
                string today = DateTime.Today.ToString("dd/MM/yyyy");
                // neu nguoi dung chua bat dau muc tieu hien tai
                if (muctieu.ThoiGianBatDau == null || muctieu.TrangThai == "Chưa bắt đầu")
                {
                    calsGiamGrid.Visibility = Visibility.Collapsed;
                    MessageDialog msDialog = new MessageDialog("Bạn vẫn chưa bắt đầu tập luyện!\n");
                    msDialog.Commands.Add(new UICommand("Ok, đến trang tập luyện"));
                    msDialog.Commands.Add(new UICommand("Để sau"));
                    var result = await msDialog.ShowAsync();
                    if (result.Label.Equals("Để sau"))
                    {
                        return;
                    }
                    else
                    {
                        // dat thoi gian bat dau la ngay hom nay
                        muctieu.ThoiGianBatDau = today;
                        muctieu.TrangThai = "Đã bắt đầu";
                        connection.Update(muctieu);
                        Frame.Navigate(typeof(DanhSachBaiTap), nguoidung);
                    }

                }

                // kiem tra thoigianketthuc cua muc tieu

                //DateTime ngayketthuc = DateTime.Parse(muctieu.ThoiGianBatDau).AddDays((int)muctieu.SoNgay); <--- nho dung ParseExact de theo chuan dd/MM/yyyy
                DateTime ngayketthuc = DateTime.ParseExact(muctieu.ThoiGianBatDau, "dd/MM/yyyy", new CultureInfo("vi-vn")).AddDays(muctieu.SoNgay);
                // neu da vuot qua so ngay
                if (DateTime.Today > ngayketthuc)
                {
                    muctieu.TrangThai = "Hoàn thành";
                    connection.Update(muctieu);
                    MessageDialog msDialog = new MessageDialog("Chúc mừng, bạn đã tập hết số ngày của mục tiêu đề ra\nHãy xem lại quá trình luyện tập của bạn!");
                    msDialog.Commands.Add(new UICommand("Xem thống kê"));
                    msDialog.Commands.Add(new UICommand("Mục tiêu mới"));
                    var result = await msDialog.ShowAsync();
                    if (result.Label.Equals("Mục tiêu mới"))
                    {
                        // chuyển tới trang mục tiêu mới
                        denTaoMucTieuMoiPage();
                    }
                    else
                    {
                        // chuyển đến trang thống kê
                        Frame.Navigate(typeof(ThongKePage), nguoidung);
                    }
                }
                // neu van con trong thoi han cua muc tieu
                else
                {
                    thongkengay = connection.Table<ThongKeNgay>().Where(r => r.IdMucTieu == muctieu.IdMucTieu && r.Ngay == today).FirstOrDefault();
                    if (thongkengay == null)
                    {
                        thongkengay = new ThongKeNgay();
                        thongkengay.IdMucTieu = muctieu.IdMucTieu;
                        thongkengay.Ngay = today;
                        // mặc định = chỉ số bmr
                        thongkengay.LuongKaloDuaVao = muctieu.ChiSoBMR;
                        connection.Insert(thongkengay);
                    }
                }
                // LuongKaloCanGiamHomNay = KaloCanTieuHaoMoiNgay (để giảm cân) - LuongKaloTieuHao (chohoatdongbaitap) - chisobmr (luongkalo chohoatdonghangngay) + tongluongkaloduavao
                if(muctieu != null && thongkengay != null)
                {
                    double calsCanGiam = muctieu.LuongKaloCanTieuHaoMoiNgay;
                    calsCanGiamTextBlock.Text = calsCanGiam.ToString();

                    // luong cals giam tu bai tap
                    double calsGiamBaiTap = thongkengay.LuongKaloTieuHao;
                    calsBaiTapTextBlock.Text = calsGiamBaiTap.ToString();

                    // luong Cals giam tu thuc don
                    double calsGiamThucDon = muctieu.ChiSoBMR - thongkengay.LuongKaloDuaVao - thongkengay.LuongKaloNgoaiDuKien;
                    calsThucDonTextBlock.Text = calsGiamThucDon.ToString();

                    double calsConThieu = calsCanGiam - calsGiamBaiTap - calsGiamThucDon;
                    calsConThieuTextBlock.Text = calsConThieu.ToString();
                }
                

            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
