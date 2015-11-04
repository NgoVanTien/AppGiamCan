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
    public sealed partial class DangKy : Page
    {
        string path;
        SQLite.Net.SQLiteConnection connection;
        public DangKy()
        {
            this.InitializeComponent();
            path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "giamcandb.sqlite");
            connection = new SQLite.Net.SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // đặt ngày giờ tối đa là hôm nay
            // chỉ mơi đặt được năm, không biết đặt ngày với tháng 
            ngaysinhDatePicker.MaxYear = DateTime.Today;
        }
        private async void dangkyButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if(tendangnhapTextBox.Text.Trim() == "" || matkhauTextBox.Text.Trim() == "")
                {
                    // neu khong nhap tendangnhap, matkhau thi se hien len thong bao
                    var msDialog = new MessageDialog("TÊN ĐĂNG NHẬP và MẬT KHẨU không được để trống");
                    await msDialog.ShowAsync();
                    return;
                }
                // kiem tra ngay sinh (ngay sinh khong duoc la ngay o tuong lai)
                if (ngaysinhDatePicker.Date > DateTime.Today)
                {
                    var msDialog = new MessageDialog("NGÀY SINH không chính xác");
                    await msDialog.ShowAsync();
                    return;
                }

                // kiem tra tendangnhap da co nguoidung su dung chua
                var nguoidungCollection = connection.Table<NguoiDung>();
                foreach (var nd in nguoidungCollection)
                {
                    if(nd.TenDangNhap == tendangnhapTextBox.Text)
                    {
                        var msDialog = new MessageDialog("TÊN ĐĂNG NHẬP đã tồn tại.\nVui lòng chọn tên đăng nhập khác.");
                        await msDialog.ShowAsync();
                        return;
                    }
                }

                //dua nguoidung vao database
                //lay ngaysinh, dua ve dang dd-MM-yyyy
                string ngaysinh = ngaysinhDatePicker.Date.ToString("dd/MM/yyyy");
                //lay gioitinh
                RadioButton selectedRadio = gioitinhPanel.Children.OfType<RadioButton>().FirstOrDefault(r => r.IsChecked == true);
                // tao moi nguoidung
                NguoiDung nguoidung = new NguoiDung(tendangnhapTextBox.Text, matkhauTextBox.Text, ngaysinh, selectedRadio.Content.ToString());
                //insert nguoidung 
                connection.Insert(nguoidung);

                //hien thi thong bao thanh cong
                var msDialog1 = new MessageDialog("Đăng ký thành công!");
                await msDialog1.ShowAsync();

                //tro ve trang MainPage de dang nhap
                Frame.Navigate(typeof(MainPage));


            }
            catch (Exception)
            {

                throw;
            }
        }

        private void troveButton_Click(object sender, RoutedEventArgs e)
        {
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
            }
        }

    }
}
