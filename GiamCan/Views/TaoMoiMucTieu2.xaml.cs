﻿using GiamCan.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
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
        ChiSo chiso;
        NguoiDung nguoidung;
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
            nguoidung = connection.Table<NguoiDung>().Where(r => r.TenDangNhap == muctieu.TenDangNhap).FirstOrDefault();
            int tuoi = DateTime.Today.Year - DateTime.ParseExact(nguoidung.NgaySinh, "dd/MM/yyyy", new CultureInfo("vi-vn")).Year;
            chiso = new ChiSo(muctieu.CanNangBanDau, muctieu.ChieuCaoBanDau, nguoidung.GioiTinh, tuoi);
            // so ngay giam de nghi cho mac dinh = 30;
            int socangiamdenghi = chiso.tinhSoCanGiamDeNghi(); // cannang - cannanglytuong

            // muc 1
            DeNghi muc1 = new DeNghi();
            muc1.SoCanGiamDeNghi = socangiamdenghi;
            muc1.SoNgayGiamDeNghi = Convert.ToInt32(Math.Round(muc1.SoCanGiamDeNghi / 0.05));
            muc1Button.Content = string.Format("Giảm {0} kg trong vòng {1} ngày", muc1.SoCanGiamDeNghi, muc1.SoNgayGiamDeNghi);
            muc1Button.Tag = muc1;

            // muc 2 
            DeNghi muc2 = new DeNghi();
            muc2.SoCanGiamDeNghi = socangiamdenghi;
            muc2.SoNgayGiamDeNghi = Convert.ToInt32(Math.Round(muc2.SoCanGiamDeNghi / 0.1));
            muc2Button.Content = string.Format("Giảm {0} kg trong vòng {1} ngày", muc2.SoCanGiamDeNghi, muc2.SoNgayGiamDeNghi);
            muc2Button.Tag = muc2;

            // muc3
            DeNghi muc3 = new DeNghi();
            muc3.SoCanGiamDeNghi = socangiamdenghi;
            muc3.SoNgayGiamDeNghi = Convert.ToInt32(Math.Round(muc3.SoCanGiamDeNghi / 0.15));
             muc3Button.Content = string.Format("Giảm {0} kg trong vòng {1} ngày", muc3.SoCanGiamDeNghi, muc3.SoNgayGiamDeNghi);
            muc3Button.Tag = muc3;

            
        }

        private async void themmuctieuButton_Click(object sender, RoutedEventArgs e)
        {
            
            if (await checkNumberTextBox() == true)
            {
                // xóa mục tiêu hiện tại
                var mtht = connection.Table<MucTieu>().Where(r => r.TenDangNhap == nguoidung.TenDangNhap && (r.TrangThai == "Đã bắt đầu" || r.TrangThai == "Chưa bắt đầu")).FirstOrDefault();
                if (mtht != null)
                {
                    mtht.TrangThai = "Hủy";
                    connection.Update(mtht);
                }

                // thêm mục tiêu mới vào db
                muctieu.SoCanMuonGiam = socanmuongiam;
                muctieu.SoNgay = thoigian;
                muctieu.TrangThai = "Chưa bắt đầu";
                // 1kg = 7700;
                muctieu.LuongKaloCanTieuHaoMoiNgay = Math.Round(socanmuongiam * 7700.0 / thoigian,2);
                connection.Insert(muctieu);

                MessageDialog msDialog = new MessageDialog("Tạo mục tiêu thành công");
                await msDialog.ShowAsync();
                
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

        private void mucButton_Click(object sender, RoutedEventArgs e)
        {
            DeNghi denghi = (sender as Button).Tag as DeNghi;
            canmuongiamTextBox.Text = denghi.SoCanGiamDeNghi.ToString();
            thoigiangiamTextBox.Text = denghi.SoNgayGiamDeNghi.ToString();
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
    class DeNghi
    {
        public int SoNgayGiamDeNghi { get; set; }
        public int SoCanGiamDeNghi { get; set; }
    }
}
