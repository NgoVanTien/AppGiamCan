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
  
        SQLite.Net.SQLiteConnection connection = TrangChu.connection;
        List<ThucDon> thucDonLst = new List<ThucDon>();
        public static double kaloRieng;
        ThongKeNgay tkNgay;
        NguoiDung nguoidung;
        MucTieu muctieu;
        public KaloNgoaiPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            nguoidung = e.Parameter as NguoiDung;
            muctieu = TrangChu.getMucTieuHienTai(nguoidung);
            if(muctieu != null)
            {
                tkNgay = TrangChu.getThongKeNgayHienTai(muctieu);
                kaloBox.Text = tkNgay.LuongKaloNgoaiDuKien.ToString();
            }
            else
            {
                tkNgay = new ThongKeNgay();
                kaloBox.Text = "0";
            }
        }
        private void XongBtn_Click(object sender, RoutedEventArgs e)
        {

            kaloRieng = double.Parse(kaloBox.Text);

            tkNgay.LuongKaloNgoaiDuKien = kaloRieng;
            if(muctieu != null && tkNgay != null)
            {
                connection.Update(tkNgay);
                MessageDialog msDialog = new MessageDialog("Thành công");
            }
            Frame.Navigate(typeof(TrangChu), nguoidung);
        }

        private void HuyBtn_Click(object sender, RoutedEventArgs e)
        {
            if (Frame.CanGoBack)
                Frame.GoBack();
        }
    }
}
