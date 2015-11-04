using GiamCan.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
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
    public sealed partial class TaoMoiMucTieu2 : Page
    {
        MucTieu muctieu;
        int socanmuongiam, thoigian;
        string path;
        SQLite.Net.SQLiteConnection connection;
        public TaoMoiMucTieu2()
        {
            this.InitializeComponent();
            path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "giamcandb.sqlite");
            connection = new SQLite.Net.SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            muctieu = (MucTieu)e.Parameter;

        }

        private async void themmuctieuButton_Click(object sender, RoutedEventArgs e)
        {
            
            if (await checkNumberTextBox() == true)
            {
                muctieu.SoCanMuonGiam = socanmuongiam;
                muctieu.SoNgay = thoigian;
                connection.Insert(muctieu);
                MessageDialog msDialog = new MessageDialog("Tạo mục tiêu thành công");
                await msDialog.ShowAsync();
                // lay du lieu NguoiDung de gui về lại trang chủ
                NguoiDung nguoidung = connection.Query<NguoiDung>("SELECT * FROM NguoiDung WHERE TenDangNhap=?", muctieu.TenDangNhap).FirstOrDefault();
                Frame.Navigate(typeof(TrangChu), nguoidung);
            }
        }

        private void huyButton_Click(object sender, RoutedEventArgs e)
        {
            NguoiDung nguoidung = connection.Query<NguoiDung>("SELECT * FROM NguoiDung WHERE TenDangNhap=?", muctieu.TenDangNhap).FirstOrDefault();
            Frame.Navigate(typeof(TrangChu), nguoidung);
            //// neu nhan nut huy thi xoa lich su
            //Frame.BackStack.RemoveAt(Frame.BackStackDepth - 1);
            //Frame.BackStack.RemoveAt(Frame.BackStackDepth - 1);
        }

        private async Task<bool> checkNumberTextBox()
        {
            MessageDialog msDialog;

            if (canmuongiamTextBox.Text == "" || thoigiangiamTextBox.Text == "")
            {
                msDialog = new MessageDialog("Vui lòng điền đầy đủ thông tin\nHoặc bạn có thể chọn các mục tiêu đề xuất");
                await msDialog.ShowAsync();
                return false;
            }
            if (Int32.TryParse(canmuongiamTextBox.Text, out socanmuongiam) && Int32.TryParse(thoigiangiamTextBox.Text, out thoigian))
            {
                if (socanmuongiam <= 0 || thoigian <= 0)
                {
                    msDialog = new MessageDialog("Dữ liệu nhập vào không chính xác ");
                    await msDialog.ShowAsync();
                    return false;
                }
                return true;
            }
            return false;
        }
      
    }
}
