using System;
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
using Windows.Media;
using System.Diagnostics;
using System.Threading;
using GiamCan.Model;
using Windows.UI.Popups;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace GiamCan.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>

    public sealed partial class BaiTapBung : Page
    {
        NguoiDung nguoidung;
        MucTieu muctieu;
        bool flag = false;
        int count = 0;
        double tongCalo;
        int level;
        ThongKeBaiTap thongKeTapBung = new ThongKeBaiTap();
        ThongKeNgay thongKeNgay = new ThongKeNgay();
        BaiTap baiTapBung;
        SQLite.Net.SQLiteConnection connection = TrangChu.connection;
        int idBT = 3;

        public object Controls { get; private set; }

        public BaiTapBung()
        {
            this.InitializeComponent();
            baiTapBung = connection.Table<BaiTap>().Where(r => r.TenBaiTap == "Gập bụng").FirstOrDefault();
            //idBT = baiTapBung.IdBaiTap;
            //ngay = DateTime.Today.ToString("dd/MM/yyyy");
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            level = (int)e.Parameter;
            nguoidung = TrangChu.nguoidung;
            muctieu = TrangChu.muctieu;
            if (muctieu != null)
            {
                thongKeNgay = TrangChu.getThongKeNgayHienTai(muctieu);
                thongKeTapBung = connection.Table<ThongKeBaiTap>().Where(r => r.IdThongKeNgay == thongKeNgay.IdThongKeNgay && r.IdBaiTap == idBT).FirstOrDefault();
                if (thongKeTapBung == null)
                {
                    thongKeTapBung = new ThongKeBaiTap()
                    {
                        IdThongKeNgay = thongKeNgay.IdThongKeNgay,
                        IdBaiTap = idBT,
                        LuongKaloTieuHao = 0,
                        SoLan = 0
                    };
                    connection.Insert(thongKeTapBung);
                }
            }
            else
            {
                thongKeTapBung = new ThongKeBaiTap();
            }
        }

        // tính tổng calo tiêu thụ
        public double tinhTongCalo(int x)
        {
            baiTapBung = connection.Table<BaiTap>().Where(r=>r.TenBaiTap == "Gập bụng").FirstOrDefault();
            tongCalo = baiTapBung.LuongKaloTrenDVT * x;
            return tongCalo;
        }
        //public void updateThongKeNgay(String ngay, double tongCalo)
        //{
        //    // update thong ke ngay
        //    var thongKeNgayCollect = connection.Table<ThongKeNgay>().Where(r => r.Ngay == ngay).FirstOrDefault();
        //    muctieu = connection.Query<MucTieu>("SELECT * FROM MUCTIEU WHERE TrangThai = ?", "Đã bắt đầu").FirstOrDefault();

        //    if (thongKeNgayCollect != null)
        //    {
        //        thongKeNgay.IdMucTieu = muctieu.IdMucTieu;
        //        thongKeNgay.Ngay = ngay;
        //        thongKeNgay.LuongKaloTieuHao = tongCalo + thongKeNgayCollect.LuongKaloTieuHao;
        //        connection.Update(thongKeNgay);

        //    }
        //    else
        //    {
        //        thongKeNgay.IdMucTieu = muctieu.IdMucTieu;
        //        thongKeNgay.Ngay = ngay;
        //        thongKeNgay.LuongKaloTieuHao = tongCalo;
        //        connection.Insert(thongKeNgay);

        //    }
        //}

        // update thống kê baitap



        private async void startButton_Click(object sender, RoutedEventArgs e)
        {
            // chế độ tập với người mới tập

            startButton.IsEnabled = false;
            startButton.Content = "Tiếp tục";
            flag = true;
            Stopwatch stopwatch = new Stopwatch();
            while (true)
            {
                stopwatch.Start();
                bipMedia.Play();

                if (level == 1)
                {
                    count += 1;
                    await System.Threading.Tasks.Task.Delay(3000);
                }
                else if (level == 2)
                {
                    count += 1;
                    await System.Threading.Tasks.Task.Delay(2000);
                }
                else if (level == 3)
                {
                    count += 1;
                    await System.Threading.Tasks.Task.Delay(1000);
                }
                if (flag == false) break;
                solanText.Text = count.ToString();
            }

        }

        private void tamdungButton_Click(object sender, RoutedEventArgs e)
        {
            flag = false;
            startButton.IsEnabled = true;
            bipMedia.Pause();
        }

        private void dungButton_Click(object sender, RoutedEventArgs e)
        {
            flag = false;
            bipMedia.Stop();
            tongCalo = tinhTongCalo(count);
            // thống kê
            thongKeText.Visibility = Visibility;

            // update so lần
            tongText.Text = "Số lần tập: " + count.ToString();
            tongText.Visibility = Visibility;
            // update tong calo
            caloText.Text = "Tổng lượng calo: " + tongCalo.ToString();
            caloText.Visibility = Visibility;
            troVeDSButton.Visibility = Visibility;
            // set count bằng 0 sau khi dừng tập. 
            //updateThongKeNgay(ngay, tongCalo);

            thongKeTapBung.SoLan += count;
            thongKeTapBung.LuongKaloTieuHao += tongCalo;

            if (muctieu != null && thongKeNgay != null)
            { 
                connection.Update(thongKeTapBung);
            }

            count = 0;
            startButton.Content = "Bắt Đầu";
            startButton.IsEnabled = true;


        }

        private void troVeDSButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(DanhSachBaiTap));
        }
    }
}
