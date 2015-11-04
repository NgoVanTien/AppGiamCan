using System;
using GiamCan.Model;
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
using Windows.UI.Popups;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace GiamCan.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class KaloNgoaiPage : Page
    {
        String path;
        SQLite.Net.SQLiteConnection conn;
        List<ThucDon> thucDonLst = new List<ThucDon>();
        public static double kaloRieng;
        ThongKeNgay tkNgay;
        public KaloNgoaiPage()
        {
            this.InitializeComponent();
            path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "giamcandb.sqlite");
            conn = new SQLite.Net.SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            tkNgay = e.Parameter as ThongKeNgay;
            kaloBox.Text = tkNgay.LuongKaloNgoaiDuKien.ToString();
        }
        private void XongBtn_Click(object sender, RoutedEventArgs e)
        {

            kaloRieng = double.Parse(kaloBox.Text);
            //dua cac mon an da check vao thuc don
            //ThucDon thucDonChon = new ThucDon();
            //thucDonChon.IdMonAn = MonAnPage.idChecked;
            //thucDonChon.SoLuong = MonAnPage.slChon;
            //thucDonChon.LuongKalo = MonAnPage.kaloSum;
            //thucDonChon.LuongKaloNgoaiDuKien = kaloRieng;
            //thucDonLst.Add(thucDonChon);
            tkNgay.LuongKaloNgoaiDuKien = kaloRieng;
            conn.Update(tkNgay);
            MessageDialog msDialog = new MessageDialog("Thành công");
            // lay thong tin nguoidung de chuyen ve trang chu
            MucTieu muctieu = conn.Table<MucTieu>().Where(r => r.IdMucTieu == tkNgay.IdMucTieu).FirstOrDefault();
            NguoiDung nguoidung = conn.Table<NguoiDung>().Where(r => r.TenDangNhap == muctieu.TenDangNhap).FirstOrDefault();
            Frame.Navigate(typeof(TrangChu), nguoidung);
        }

        private void HuyBtn_Click(object sender, RoutedEventArgs e)
        {
            if (Frame.CanGoBack)
                Frame.GoBack();
        }
    }
}
