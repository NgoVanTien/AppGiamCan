using System;
using GiamCan.Views;
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
using System.ComponentModel;
using System.Threading;
using Windows.System.Threading;
using Windows.UI.Core;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Popups;
using GiamCan.Model;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace GiamCan.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LacVong : Page, INotifyPropertyChanged
    {
        MucTieu muctieu;
        private string timeStr;
        ThongKeNgay thongkengay;
        ThongKeBaiTap lacVong;
        NguoiDung nguoidung;
        SQLite.Net.SQLiteConnection connection = TrangChu.connection;
        public LacVong()
        {
            this.InitializeComponent();
            this.DataContext = this;
            maxtime = 5*60;
            solantick = 0; // so lan tick
            timeStr = "00:00";
        }
        //Image mainImage;
        ThreadPoolTimer threadImg;
        int saveTime = 0;
        int currentImage = 0;
        List<ImageSource> imgLst = new List<ImageSource>();
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaiseProperty(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        private double percent;
        private int time;
        private int maxtime;
        private int solantick;

        public string TimeStr
        {
            get { return timeStr; }
            set { timeStr = value; RaiseProperty(nameof(TimeStr)); }
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

            List<Uri> uriLst = new List<Uri>();
            for (int i = 1; i <= 13; i++)
            {
                uriLst.Add(new Uri("ms-appx:///Assets/lacvong" + i + ".png"));
            }
            for (int i = 0; i < 13; i++)
            {
                imgLst.Add(new BitmapImage(uriLst[i]));
            }
           
            threadImg = ThreadPoolTimer.CreatePeriodicTimer(UpdateSprite, TimeSpan.FromMilliseconds(100));

            //kiem tra bai tap nay da co trong database chua
            nguoidung = TrangChu.nguoidung;

            // lấy mục tiêu hiện tại (có thể null)
            muctieu = TrangChu.muctieu;
            if (muctieu != null)
            {
                // lấy thống kê ngày hiện tại (có thể null)
                thongkengay = TrangChu.getThongKeNgayHienTai(muctieu);
                //check da tap lan nao trong ngay chua

                lacVong = connection.Table<ThongKeBaiTap>().Where(r => r.IdThongKeNgay == thongkengay.IdThongKeNgay && r.IdBaiTap == 6).FirstOrDefault();
                if (lacVong == null)
                {
                    lacVong = new ThongKeBaiTap()
                    {
                        IdBaiTap = 6,
                        IdThongKeNgay = thongkengay.IdThongKeNgay,
                        QuangDuong = 0,
                        LuongKaloTieuHao = 0,
                        ThoiGianTap = 0
                    };
                    connection.Insert(lacVong);
                }

            }

            // neu khong thi cho tap nhung khong dua vao database
            else
            {
                lacVong = new ThongKeBaiTap();
            }
        }
        private async void UpdateSprite(ThreadPoolTimer timer)
        {

            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                lacVongImg.Source = imgLst.ElementAt(currentImage);
            });
            currentImage = currentImage + 1;
            if (currentImage == 13)
            {
                currentImage = 0;
            }

        }
        public int Time
        {
            get { return time; }
            set { time = value; RaiseProperty(nameof(Time)); }
        }
        public double Percent
        {
            get { return percent; }
            set { percent = value; RaiseProperty(nameof(Percent)); }
        }

        private DispatcherTimer timer = new DispatcherTimer() { Interval = TimeSpan.FromSeconds(0.1) };
        private void OnTimerTick(object sender, object e)
        {
            solantick += 1;
            Percent = 0.1 * 100 / maxtime * solantick;
            if (solantick % 10 == 0)
            {
                Time++;
                int min = Time / 60;
                int second = Time % 60;
                if(min>0)
                {
                    if(second < 10 )
                    {
                        TimeStr = "0" + min + ":" + "0"+second;
                    }
                    else TimeStr = "0" + min + ":" + second;
                }
                else
                {
                    if(second < 10) TimeStr = "00:0" +  Time.ToString();
                    else TimeStr = "00:" + Time.ToString();
                }
                    
            }
            if (Percent == 100)
            {
                timer.Stop();
            }
        }

        private void startBtn_Click(object sender, RoutedEventArgs e)
        {
            string contentBtn = (string)startBtn.Content;
            switch (contentBtn)
            {
                case "Bắt đầu":
                    {
                        startBtn.Content = "Tạm dừng";
                        timer.Tick += OnTimerTick;
                        timer.Start();
                        break;
                    }
                case "Tạm dừng":
                    {
                        startBtn.Content = "Tiếp tục";
                        saveTime = Time;
                        saveTime++;
                        timer.Tick -= OnTimerTick;
                        break;
                    }
                case "Tiếp tục":
                    {
                        startBtn.Content = "Tạm dừng";
                        Time = saveTime;
                        timer.Tick += OnTimerTick;
                        break;
                    }
                default:
                    break;
            }
        }

        private void stopBtn_Click(object sender, RoutedEventArgs e)
        {
            
            if("Trở về".Equals(stopBtn.Content))
            {
                Frame.Navigate(typeof(DanhSachBaiTap));
                return;
            }
            threadImg.Cancel();
            double kaloTieuHao;
            if (Percent == 100)
            {
                kaloTieuHao = 33.7;
            }
            else
            {
                timer.Stop();
                saveTime = Time;
                saveTime++;
                timer.Tick -= OnTimerTick;
                kaloTieuHao = Math.Round((200.0 / (30 * 60)) * saveTime,2);
            }

            lacVong.ThoiGianTap += saveTime;
            lacVong.LuongKaloTieuHao += kaloTieuHao;

            // nếu mục tiêu và thống kê ngày != null mới đưa vào database
            if (muctieu != null && thongkengay != null)
                connection.Update(lacVong);

            stopBtn.Content = "Trở về";
            huongdanStackPanel.Visibility = Visibility.Collapsed;
            lacVongImg.Visibility = Visibility.Collapsed;

            ketquaStackPanel.Visibility = Visibility.Visible;
            thoigiantapTextBlock.Text = "Thời gian tập: " + TimeStr;
            luongkcalTextBlock.Text = "Lượng Kcal tiêu hao: " + kaloTieuHao;
            
        }
        public void chuyenPage2(IUICommand command)
        {
            Frame.Navigate(typeof(DanhSachBaiTap));
        }

        public void chuyenPage1(IUICommand command)
        {
            Frame.Navigate(typeof(LacVong));
        }
    }


}
