using System;
using GiamCan.Model;
using GiamCan.Views;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.Services.Maps;
using Windows.Devices.Geolocation;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls.Maps;
using System.Threading;
using Windows.System.Threading;
using Windows.UI;
using Windows.UI.Core;
using System.Collections.Generic;
using Windows.UI.Popups;
using System.IO;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace GiamCan.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DapXe : Page
    {
        NguoiDung nguoidung;
        MucTieu muctieu;
        ThongKeNgay thongkengay;
        //Geolocator location;
        Geoposition position;
        Geolocator locator;
        Geopoint point;
        Geopoint currentPoint;
        MapIcon icon;
        double Distance = 0;
        bool isPause;//kiem tra co tam dung hay k
        long timeStart = 0; //time bat dau
        long saveTime = 0;
        long timeCount = 0;
        bool tracking = false;


        ThreadPoolTimer timeThread;

        SQLite.Net.SQLiteConnection connection = TrangChu.connection;
        ThongKeBaiTap dapXe = new ThongKeBaiTap();
        public DapXe()
        {
            this.InitializeComponent();

            pauseBtn.Visibility = Visibility.Collapsed;
            contiBtn.Visibility = Visibility.Collapsed;
            stopBtn.Visibility = Visibility.Collapsed;
            locator = new Geolocator();
            locator.DesiredAccuracyInMeters = 50;
        }

        private async void UpdateTime(ThreadPoolTimer timer)
        {
            //  while(true)

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
                           string s = hour.ToString() + ":" + minutes.ToString() + ":" + timeSe.ToString();
                           timeBlock.Text = s;
                       });


        }

        //get current location
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            nguoidung = TrangChu.nguoidung;


            // lấy mục tiêu hiện tại (có thể null)
            muctieu = TrangChu.getMucTieuHienTai(nguoidung);
            if (muctieu != null)
            {
                // lấy thống kê ngày hiện tại (có thể null)
                thongkengay = TrangChu.getThongKeNgayHienTai(muctieu);
                //check da tap lan nao trong ngay chua

                dapXe = connection.Table<ThongKeBaiTap>().Where(r => r.IdThongKeNgay == thongkengay.IdThongKeNgay && r.IdBaiTap == 2).FirstOrDefault();
                if (dapXe == null)
                {
                    dapXe = new ThongKeBaiTap()
                    {
                        IdBaiTap = 2,
                        IdThongKeNgay = thongkengay.IdThongKeNgay,
                        QuangDuong = 0,
                        LuongKaloTieuHao = 0,
                        ThoiGianTap = 0
                    };
                    connection.Insert(dapXe);
                }

            }

            // neu khong thi cho tap nhung khong dua vao database
            else
            {
                dapXe = new ThongKeBaiTap();
            }


            var accessStatus = await Geolocator.RequestAccessAsync();
            switch (accessStatus)
            {
                case GeolocationAccessStatus.Allowed:

                    // Get the current location
                    position = await locator.GetGeopositionAsync();
                    point = position.Coordinate.Point;
                    myMap.ZoomLevel = 15;

                    // Set map location
                    await myMap.TrySetViewAsync(point);
                    locator.MovementThreshold = 1;

                    //create a icon in current location
                    icon = new MapIcon();
                    icon.Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/pin.png"));
                    icon.NormalizedAnchorPoint = new Point(0.5, 1);
                    icon.Location = position.Coordinate.Point;
                    icon.Title = "You are here";
                    myMap.MapElements.Add(icon);
                    //  locator = new Geolocator { ReportInterval = 500 };

                    // Subscribe to PositionChanged event to get location updates
                    locator.PositionChanged += OnPositionChanged;
                    break;

                case GeolocationAccessStatus.Denied:
                    // Handle when access to location is denied
                    break;

                case GeolocationAccessStatus.Unspecified:
                    // Handle when an unspecified error occurs
                    break;
            }


        }

        async private void OnPositionChanged(Geolocator sender, PositionChangedEventArgs e)
        {
            if (!tracking)
            {
                point = e.Position.Coordinate.Point;
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    icon.Location = point;
                });
                await myMap.TrySetViewAsync(point);
            }

            else
            {
                currentPoint = e.Position.Coordinate.Point;
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    icon.Location = currentPoint;
                });

                MapRouteFinderResult routeResult = await MapRouteFinder.GetWalkingRouteAsync(point, currentPoint);
                if (routeResult.Route == null) return;
                if (routeResult.Status == MapRouteFinderStatus.Success)
                {
                    await Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
                    {

                        //directionChanged
                        MapRouteView viewOfRoute = new MapRouteView(routeResult.Route);
                        viewOfRoute.RouteColor = Colors.Blue;
                        viewOfRoute.OutlineColor = Colors.Blue;
                        myMap.Routes.Add(viewOfRoute);
                    });
                    Distance += routeResult.Route.LengthInMeters;
                    point = currentPoint;
                    await Dispatcher.RunAsync(
                          CoreDispatcherPriority.High,
                          () =>
                          {
                              distanceBlock.Text = Distance.ToString() + "m";
                          });
                    //distanceBlock.Text = Distance.ToString() + "m";
                }
                await myMap.TrySetViewAsync(currentPoint);
            }

        }

        private void startBtn_Click(object sender, RoutedEventArgs e)
        {
            startBtn.Visibility = Visibility.Collapsed;
            pauseBtn.Visibility = Visibility.Visible;
            stopBtn.Visibility = Visibility.Visible;
            timeStart = DateTime.Now.Ticks;
            timeThread = ThreadPoolTimer.CreatePeriodicTimer(UpdateTime, TimeSpan.FromMilliseconds(1000));
            tracking = true;

        }
        private void pauseBtn_Click(object sender, RoutedEventArgs e)
        {
            isPause = true;
            pauseBtn.Visibility = Visibility.Collapsed;
            contiBtn.Visibility = Visibility.Visible;
            tracking = false;
        }

        private void contiBtn_Click(object sender, RoutedEventArgs e)
        {
            tracking = true;
            isPause = false;
            saveTime = timeCount;
            timeStart = DateTime.Now.Ticks;
            contiBtn.Visibility = Visibility.Collapsed;
            pauseBtn.Visibility = Visibility.Visible;
        }

        private async void stopBtn_Click(object sender, RoutedEventArgs e)
        {
            tracking = false;
            timeThread.Cancel();
            double tocDo;

            dapXe.ThoiGianTap += timeCount;
            dapXe.QuangDuong += Distance;
            double kaloTieuHao = 0;
            try
            {
                tocDo = dapXe.QuangDuong / dapXe.ThoiGianTap;
            }
            catch (DivideByZeroException)
            {
                tocDo = 0;
            }
            if (tocDo == 0) kaloTieuHao = 0;
            if (tocDo <= 5 && tocDo > 0) kaloTieuHao = (115 / 30) * (timeCount / 60.0);
            if (tocDo > 5 && tocDo < 10) kaloTieuHao = (330 / 30) * (timeCount / 60.0);
            if (tocDo >= 10) kaloTieuHao = (400 / 30) * (timeCount / 60.0);
            dapXe.LuongKaloTieuHao += kaloTieuHao;

            // nếu mục tiêu và thống kê ngày != null mới đưa vào database
            if (muctieu != null && thongkengay != null)
                connection.Update(dapXe);

            var ketQua = new MessageDialog("Thời gian: " + timeBlock.Text + "\nQuãng đường: " + distanceBlock.Text +
                "\nLượng kalo tiêu hao: " + kaloTieuHao + "kalo");
            await ketQua.ShowAsync();

            if (Frame.CanGoBack) Frame.GoBack();
        }
    }
}
