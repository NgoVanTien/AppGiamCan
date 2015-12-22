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
    public sealed partial class NhayTanTabata : Page, INotifyPropertyChanged
    {
        private string timeStr;
        private string noiDung = "Bạn nên tập 20s, nghỉ 10s rồi lại tiếp tục" + "/nMỗi 1 phút tiêu hao 14,3kals";
        MucTieu muctieu;
        ThongKeNgay thongkengay;
        ThongKeBaiTap tabata;
        NguoiDung nguoidung;
        SQLite.Net.SQLiteConnection connection = TrangChu.connection;
        public string NoiDung
        {
            get { return noiDung; }
            set { noiDung = value; RaiseProperty(nameof(NoiDung)); }
        }
        public NhayTanTabata()
        {
            this.InitializeComponent();
            this.DataContext = this;
            //noiDung = "Bạn nên tập 20s, nghỉ 10s rồi lại tiếp tục";
            maxtime = 5 * 60;
            solantick = 0; // so lan tick
            TimeStr = "00:00";
        }
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
            //Uri uri1 = new System.Uri("ms-appx:///Assets/tabata1.png");
            //Uri uri2 = new System.Uri("ms-appx:///Assets/tabata2.png");
            //Uri uri3 = new System.Uri("ms-appx:///Assets/tabata3.png");
            //Uri uri4 = new System.Uri("ms-appx:///Assets/tabata4.png");
            //Uri uri5 = new System.Uri("ms-appx:///Assets/tabata5.png");
            //List<Uri> uriLst = new List<Uri>();
            //uriLst.Add(uri1);
            //uriLst.Add(uri2);
            //uriLst.Add(uri3);
            //uriLst.Add(uri4);
            //uriLst.Add(uri5);
            List<Uri> uriLst = new List<Uri>();
            for(int i=1; i<=5; i++)
            {
                uriLst.Add(new Uri("ms-appx:///Assets/tabata" + i + ".png"));
            }
            //List<ImageSource> imglstSou = new List<ImageSource>();
            //for(int i = 0; i < 5; i++)
            //{
            //    imglstSou.Add(new BitmapImage(uriLst[i]));
            //}
            for(int i = 0; i < 5; i++)
            {
                imgLst.Add(new BitmapImage(uriLst[i]));
            }
            threadImg = ThreadPoolTimer.CreatePeriodicTimer(UpdateSprite, TimeSpan.FromMilliseconds(400));

            //kiem tra bai tap nay da co trong database chua
            nguoidung = TrangChu.nguoidung;

            // lấy mục tiêu hiện tại (có thể null)
            muctieu = TrangChu.muctieu;
            if (muctieu != null)
            {
                // lấy thống kê ngày hiện tại (có thể null)
                thongkengay = TrangChu.getThongKeNgayHienTai(muctieu);
                //check da tap lan nao trong ngay chua

                tabata = connection.Table<ThongKeBaiTap>().Where(r => r.IdThongKeNgay == thongkengay.IdThongKeNgay && r.IdBaiTap == 7).FirstOrDefault();
                if (tabata == null)
                {
                    tabata = new ThongKeBaiTap()
                    {
                        IdBaiTap = 7,
                        IdThongKeNgay = thongkengay.IdThongKeNgay,
                        QuangDuong = 0,
                        LuongKaloTieuHao = 0,
                        ThoiGianTap = 0
                    };
                    connection.Insert(tabata);
                }
            }

            // neu khong thi cho tap nhung khong dua vao database
            else
            {
                tabata = new ThongKeBaiTap();
            }
        }
        private async void UpdateSprite(ThreadPoolTimer timer)
        {

            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                tabataImg.Source = imgLst.ElementAt(currentImage);
            });
            currentImage = currentImage + 1;
            if (currentImage == 5)
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
                if (min > 0)
                {
                    if (second < 10)
                    {
                        TimeStr = "0" + min + ":" + "0" + second;
                    }
                    else TimeStr = "0" + min + ":" + second;
                }
                else
                {
                    if (second < 10) TimeStr = "00:0" + Time.ToString();
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

        private async void stopBtn_Click(object sender, RoutedEventArgs e)
        {
            threadImg.Cancel();
            double kaloTieuHao = 0;
            if (Percent == 100)
            {
                kaloTieuHao = 4.47;
            }
            else
            {
                timer.Stop();
                saveTime = Time;
                saveTime++;
                timer.Tick -= OnTimerTick;
                kaloTieuHao = (13.4 /  60) * saveTime;
            }

            tabata.ThoiGianTap += saveTime;
            tabata.LuongKaloTieuHao += kaloTieuHao;

            // nếu mục tiêu và thống kê ngày != null mới đưa vào database
            if (muctieu != null && thongkengay != null)
                connection.Update(tabata);

            var ketQua = new MessageDialog("Thời gian bạn đã tập được là: " + saveTime + "\n Lượng kalo đã tiêu hao là: " + kaloTieuHao);
            ketQua.Commands.Add(new UICommand("Tiếp tục", new UICommandInvokedHandler(chuyenPage1)));
            ketQua.Commands.Add(new UICommand("Ok", new UICommandInvokedHandler(chuyenPage2)));
            await ketQua.ShowAsync();
        }
        public void chuyenPage2(IUICommand command)
        {
            Frame.Navigate(typeof(DanhSachBaiTap));
        }

        public void chuyenPage1(IUICommand command)
        {
            Frame.Navigate(typeof(NhayTanTabata));
        }
    }


}
