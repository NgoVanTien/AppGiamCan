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
    public sealed partial class DanhSachBaiTap : Page
    {
        NguoiDung nguoidung;
        MucTieu muctieu;
        SQLite.Net.SQLiteConnection connection = TrangChu.connection;
        public DanhSachBaiTap()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            nguoidung = e.Parameter as NguoiDung;
            // get muctieu hien tai cua nguoid ung hien tai

            muctieu = TrangChu.getMucTieuHienTai(nguoidung);
            if(muctieu != null)
            {
                if (muctieu.ThoiGianBatDau == null || muctieu.TrangThai == "Chưa bắt đầu")
                {
                    muctieu.TrangThai = "Đã bắt đầu";
                    muctieu.ThoiGianBatDau = DateTime.Today.ToString("dd/MM/yyyy");
                    connection.Update(muctieu);
                }
            }


        }

        private void chayboButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ChayBo_Version1), nguoidung);
        }

        private void dapxeButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(DapXe), nguoidung);
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
            }
        }

        private void gapbungButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ChonMucTap), nguoidung);
        }
    }
}
