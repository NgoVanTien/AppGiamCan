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
         private  BaiTap baiTapBung = new BaiTap();
        SQLite.Net.SQLiteConnection connection = TrangChu.connection;
        int idBT ;
        string tenBt;

        //public object Controls { get; private set; }

        public BaiTapBung()
        {
            this.InitializeComponent();
            
            //ngay = DateTime.Today.ToString("dd/MM/yyyy");
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // lấy thông tin từ  trang muc tap
            mucTap muctap = (mucTap)e.Parameter;
            tenBt = muctap.tenBaiTap;
            level = muctap.level;
            tenBT.Text = tenBt;
            nguoidung = TrangChu.nguoidung;
            muctieu = TrangChu.muctieu;
            
            //
            baiTapBung = connection.Table<BaiTap>().Where(r => r.TenBaiTap == tenBt).FirstOrDefault();
            idBT = baiTapBung.IdBaiTap;

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
            baiTapBung = connection.Table<BaiTap>().Where(r => r.TenBaiTap == tenBt).FirstOrDefault();
            tongCalo = baiTapBung.LuongKaloTrenDVT * x;
            return tongCalo;
        }
       

        private async void startButton_Click(object sender, RoutedEventArgs e)
        {
            // chế độ tập với người mới tập
            if ("Bắt Đầu".Equals(startButton.Content) || "Tiếp Tục".Equals(startButton.Content))
            {
                thongKeText.Visibility = Visibility.Collapsed;
                tongText.Visibility = Visibility.Collapsed;
               // startButton.IsEnabled = false;
                startButton.Content = "Tạm Dừng";
                flag = true;
                Stopwatch stopwatch = new Stopwatch();
                while (true)
                {
                    stopwatch.Start();
                    bipMedia.Play();

                    if (level == 1)
                    {
                        count += 1;
                        await System.Threading.Tasks.Task.Delay(3500);
                        
                    }
                    else if (level == 2)
                    {
                        count += 1;
                        await System.Threading.Tasks.Task.Delay(2500);
                       
                    }
                    else if (level == 3)
                    {
                        count += 1;
                        await System.Threading.Tasks.Task.Delay(1500);
                      }
                    if (flag == false) break;
                    solanText.Text = "SỐ LẦN: " + count.ToString();
                }
            }
            else if("Tạm Dừng".Equals(startButton.Content))
                {
                startButton.Content = "Tiếp Tục";
                flag = false;
                startButton.IsEnabled = true;
                bipMedia.Pause();

            }
          
           else if("Tiếp Tục".Equals(startButton.Content))
                {
                startButton.Content = "Tạm dừng";
            }

        }


        private void dungButton_Click(object sender, RoutedEventArgs e)
        {
            if ("Dừng".Equals(dungButton.Content ) )
            {
                startButton.IsEnabled = false;
                startButton.Visibility = Visibility.Collapsed;
                dungButton.Content = "Trở Về";
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
                thongKeTapBung.SoLan += count;
                thongKeTapBung.LuongKaloTieuHao += tongCalo;
                thongKeNgay.LuongKaloTieuHao += tongCalo;

                if (muctieu != null && thongKeNgay != null)
                {
                    connection.Update(thongKeTapBung);
                    connection.Update(thongKeNgay);
                }

                count = 0;
                startButton.Content = "Bắt Đầu";
            }
            else if("Trở Về".Equals(dungButton.Content))
            {
                startButton.IsEnabled = false;
                Frame.Navigate(typeof(DanhSachBaiTap),nguoidung);
            }


        }

    }
}
