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
using Windows.Devices.Sensors;
using Windows.UI.Core;
using Windows.System.Threading;
using GiamCan.Model;
using Windows.UI.Popups;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace GiamCan.Views
{

    public sealed partial class ChayBo_Version1 : Page
    {
        SQLite.Net.SQLiteConnection connection = TrangChu.connection;
        NguoiDung nguoidung;
        MucTieu muctieu;
        ThongKeNgay thongkengay;
        ThongKeBaiTap chaybo;
        private Accelerometer _accelerometer;
        private uint _desiredReportInterval;

        float x, x_old;
        float y, y_old;
        float z, z_old;
        private bool mInitialized;

        private const float NOISE = (float)0.5;

        bool isPause = false;//kiem tra co tam dung hay k
        long timeStart = 0; //time bat dau
        long saveTime = 0;
        long timeCount = 0;

        ThreadPoolTimer timeThread;
        //Time
        DispatcherTimer mytimer = new DispatcherTimer();

        public enum MediaState
        {
            Playing,
            Paused
        }
        private MediaState state = MediaState.Playing;
        public ChayBo_Version1()
        {
            this.InitializeComponent();


            _accelerometer = Accelerometer.GetDefault();
            if (_accelerometer != null)
            {
                uint minReportInterval = _accelerometer.MinimumReportInterval;
                _desiredReportInterval = minReportInterval > 16 ? minReportInterval : 16;
            }
            else
            {
                return;
            }

        }



        private async void UpdateTime(ThreadPoolTimer timer)
        {
            //while (true)
            {
                string s1, s2, s3;
                long currentTime = DateTime.Now.Ticks;
                long toDisplay = currentTime - timeStart;
                if (isPause) return;
                DateTime time = DateTime.FromBinary(toDisplay);
                if (isPause) return;
                timeCount = time.Second + time.Minute * 60 + time.Hour * 3600 + saveTime;
                long hour = timeCount / 3600;
                long timeMin = timeCount - (hour * 3600);
                long minutes = timeCount / 60;
                long timeSe = timeCount - (minutes * 60);
                await Dispatcher.RunAsync(
                           CoreDispatcherPriority.High,
                           () =>
                           {
                               if (hour < 10)
                               {
                                   s1 = "0" + hour.ToString();
                               }
                               else
                               {
                                   s1 = hour.ToString();
                               }
                               if (minutes < 10)
                               {
                                   s2 = "0" + minutes.ToString();
                               }
                               else
                               {
                                   s2 = minutes.ToString();
                               }
                               if (timeSe < 10)
                               {
                                   s3 = "0" + timeSe.ToString();
                               }
                               else
                               {
                                   s3 = timeSe.ToString();
                               }
                               string s = s1 + ":" + s2 + ":" + s3;
                               timeblock.Text = s;
                           });

            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ScenarioEnableButton.IsEnabled = true;
            ScenarioDisableButton.IsEnabled = false;
            ScenarioPauseButton.IsEnabled = false;

            nguoidung = TrangChu.nguoidung;
            muctieu = TrangChu.muctieu;
            if (muctieu != null)
            {
                thongkengay = TrangChu.getThongKeNgayHienTai(muctieu);
                chaybo = connection.Table<ThongKeBaiTap>().Where(r => r.IdThongKeNgay == thongkengay.IdThongKeNgay && r.IdBaiTap == 2).FirstOrDefault();
                if (chaybo == null)
                {
                    chaybo = new ThongKeBaiTap()
                    {
                        IdBaiTap = 1,
                        IdThongKeNgay = thongkengay.IdThongKeNgay,
                        QuangDuong = 0,
                        SoBuoc = 0,
                        LuongKaloTieuHao = 0,
                        ThoiGianTap = 0
                    };
                    connection.Insert(chaybo);
                }
            }

            // mục tiêu == null || thống kê ngày == null -> tập nhưng không đưa vào database
            else
            {
                chaybo = new ThongKeBaiTap();
            }

        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            if (ScenarioDisableButton.IsEnabled)
            {
                Window.Current.VisibilityChanged -= new WindowVisibilityChangedEventHandler(VisibilityChanged);
                _accelerometer.ReadingChanged -= new TypedEventHandler<Accelerometer, AccelerometerReadingChangedEventArgs>(ReadingChanged);

                // Restore the default report interval to release resources while the sensor is not in use
                _accelerometer.ReportInterval = 0;
            }

            base.OnNavigatingFrom(e);
        }

        private void VisibilityChanged(object sender, VisibilityChangedEventArgs e)
        {
            if (ScenarioDisableButton.IsEnabled)
            {
                if (e.Visible)
                {
                    _accelerometer.ReadingChanged += new TypedEventHandler<Accelerometer, AccelerometerReadingChangedEventArgs>(ReadingChanged);
                }
                else
                {
                    _accelerometer.ReadingChanged -= new TypedEventHandler<Accelerometer, AccelerometerReadingChangedEventArgs>(ReadingChanged);
                }
            }
        }


        private bool hasChanged;
        private int counter;



        async private void ReadingChanged(object sender, AccelerometerReadingChangedEventArgs e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                AccelerometerReading reading = e.Reading;
                double savetime0 = 0;
                double nowtime = 0;
                x = (float)reading.AccelerationX;
                y = (float)reading.AccelerationY;
                z = (float)reading.AccelerationZ;
                if (!mInitialized)
                {
                    x_old = x;
                    y_old = y;
                    z_old = z;
                    mInitialized = true;
                    savetime0 = nowtime;
                }
                else
                {
                    double oldValue = ((x_old * x) + (y_old * y)) + (z_old * z);
                    double oldValueSqrT = Math.Abs(Math.Sqrt((double)(((x_old * x_old) + (y_old * y_old)) + (z_old * z_old))));
                    double newValue = Math.Abs(Math.Sqrt((double)(((x * x) + (y * y)) + (z * z))));
                    nowtime = DateTime.Now.Millisecond;

                    double index = Math.Abs(nowtime - savetime0);

                    oldValue /= oldValueSqrT * newValue;
                    if ((oldValue <= 0.994) && (oldValue > 0.9) && index > 400)
                    {
                        if (!hasChanged)
                        {
                            hasChanged = true;
                            counter++; //here the counter
                            Steps.Text = String.Format("{0,5:####}", counter);
                            Distance.Text = String.Format("{0:0.000}", (double)counter * 54 / 10000);

                        }
                        else
                        {
                            hasChanged = false;
                        }
                    }
                    x_old = x;
                    y_old = y;
                    z_old = z;
                    savetime0 = nowtime;
                    mInitialized = false;
                    ///Rank

                    if (counter > 200)
                    {
                        Rank.Text = String.Format("{0,5:####}", 1);
                    }
                    if (counter > 500)
                    {
                        Rank.Text = String.Format("{0,5:####}", 2);
                    }
                    if (counter > 1000)
                    {
                        Rank.Text = String.Format("{0,5:####}", 3);
                    }
                    if (counter > 3000)
                    {
                        Rank.Text = String.Format("{0,5:####}", 4);
                    }
                    if (counter > 5000)
                    {
                        Rank.Text = "Đồng";
                    }
                    if (counter > 7000)
                    {
                        Rank.Text = "Bạc";
                    }
                    if (counter > 10000)
                    {
                        Rank.Text = "Vàng";
                    }
                }

            });
        }
        private async void ScenarioEnable(object sender, RoutedEventArgs e)
        {
            if (_accelerometer != null)
            {
                // Establish the report interval
                _accelerometer.ReportInterval = _desiredReportInterval;
                isPause = false;
                Window.Current.VisibilityChanged += new WindowVisibilityChangedEventHandler(VisibilityChanged);
                _accelerometer.ReadingChanged += new TypedEventHandler<Accelerometer, AccelerometerReadingChangedEventArgs>(ReadingChanged);


                ScenarioPauseButton.IsEnabled = true;
                ScenarioDisableButton.IsEnabled = true;
                timeStart = DateTime.Now.Ticks;
                state = MediaState.Playing;
                ScenarioEnableButton.IsEnabled = false;
                timeThread = ThreadPoolTimer.CreatePeriodicTimer(UpdateTime, TimeSpan.FromMilliseconds(1000));
            }
            else
            {
                var ketQua = new MessageDialog("Điện thoại của bạn không có thiết bị để hổ trợ bài tập");
                await ketQua.ShowAsync();
                if (Frame.CanGoBack) { Frame.GoBack(); }
            }
        }
        private void ScenarioPause(object sender, RoutedEventArgs e)
        {


            if (state == MediaState.Paused)
            {
                Window.Current.VisibilityChanged += new WindowVisibilityChangedEventHandler(VisibilityChanged);
                _accelerometer.ReadingChanged += new TypedEventHandler<Accelerometer, AccelerometerReadingChangedEventArgs>(ReadingChanged);
                state = MediaState.Playing;
                ScenarioPauseButton.Content = "Pause";
                isPause = false;
                saveTime = timeCount;
                timeStart = DateTime.Now.Ticks;
            }
            else
            if (state == MediaState.Playing)
            {
                state = MediaState.Paused;
                ScenarioPauseButton.Content = "Continue";
                isPause = true;
                Window.Current.VisibilityChanged -= new WindowVisibilityChangedEventHandler(VisibilityChanged);
                _accelerometer.ReadingChanged -= new TypedEventHandler<Accelerometer, AccelerometerReadingChangedEventArgs>(ReadingChanged);
                _accelerometer.ReportInterval = 0;
            }

        }

        private async void ScenarioDisable(object sender, RoutedEventArgs e)
        {
            Window.Current.VisibilityChanged -= new WindowVisibilityChangedEventHandler(VisibilityChanged);
            _accelerometer.ReadingChanged -= new TypedEventHandler<Accelerometer, AccelerometerReadingChangedEventArgs>(ReadingChanged);

            // Restore the default report interval to release resources while the sensor is not in use
            _accelerometer.ReportInterval = 0;
            isPause = true;
            ScenarioEnableButton.IsEnabled = true;



            int buoc = counter;
            double kaloTieuHao = 0;

            kaloTieuHao = buoc * 3;
            chaybo.SoBuoc += buoc;
            chaybo.QuangDuong += (double)counter * 54 / 10000;
            chaybo.LuongKaloTieuHao += kaloTieuHao;
            chaybo.ThoiGianTap += (double)timeCount;

            // nếu chưa có mục tiêu và thống kê ngày thì không đưa vào database
            if (muctieu != null && thongkengay != null)
                connection.Update(chaybo);

            var ketQua = new MessageDialog("Số bước: " + Steps.Text + "\nQuãng đường bạn đã đi được là: " + Distance.Text +
                "Km \nLượng kalo tiêu hao: " + kaloTieuHao + "kalo");
            await ketQua.ShowAsync();
            if (Frame.CanGoBack) { Frame.GoBack(); }

        }

    }

}
