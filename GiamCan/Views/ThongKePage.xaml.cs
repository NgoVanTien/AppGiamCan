using GiamCan.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using WinRTXamlToolkit.Controls.DataVisualization.Charting;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace GiamCan.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ThongKePage : Page, INotifyPropertyChanged
    {
        string path;
        SQLite.Net.SQLiteConnection connection;
        public event PropertyChangedEventHandler PropertyChanged;
        SolidColorBrush myBrush;

        DateTime calendarDate;

        public List<ThongKeNgay> ThongKeNgayList { get; set; }
        public List<ThongKeNgay> ThongKeNgayKhacList { get; set; }

        MucTieu muctieu;

        

        public double LuongKaloTieuHao { get; set; }
        public double LuongKaloDuaVao { get; set; }

        public ThongKePage()
        {
            this.InitializeComponent();
            path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "giamcandb.sqlite");
            connection = new SQLite.Net.SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path);
            calendarDate = DateTime.Today;
            
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

            // lay muctieu hien tai
            muctieu = e.Parameter as MucTieu;
            // tu muc tieu lay danh sach thongkengay
            ThongKeNgayList = connection.Table<ThongKeNgay>().Where(r => r.IdMucTieu == muctieu.IdMucTieu).ToList<ThongKeNgay>();
            // lay danh sach thongkengay tu cac muc tieu KHAC -> de nguoidung xem lai
            ThongKeNgayKhacList = connection.Table<ThongKeNgay>().Where(r => r.IdMucTieu != muctieu.IdMucTieu).ToList<ThongKeNgay>();
            Initialize_Calendar(calendarDate);

        }

        void Initialize_Calendar(DateTime date)
        {
            CalendarHeader.Text = date.ToString("MMMM yyyy");
            // gan thang hien tai vao CalanderHeader.Tag de sau nay truy xuat thang hien tai
            CalendarHeader.Tag = date.ToString("MM/yyyy");
            DateTime date1 = new DateTime(date.Year, date.Month, 1);

            int daysInMoth = DateTime.DaysInMonth(date1.Year, date1.Month);
            // neu thang chua 6 tuan, thi hien hang thu 6 
            if((date1.DayOfWeek == DayOfWeek.Friday && daysInMoth == 31) || (date1.DayOfWeek == DayOfWeek.Saturday && daysInMoth > 30))
            {
                (Calendar.Children[5] as StackPanel).Visibility = Visibility.Visible;
            }
            else
            {
                (Calendar.Children[5] as StackPanel).Visibility = Visibility.Collapsed;
            }
            int dayOfWeek = (int)date1.DayOfWeek + 1;
            int daysOfMonth = DateTime.DaysInMonth(date1.Year, date1.Month);
            int i = 1;
            
            foreach (var o1 in Calendar.Children)
            {
                foreach (var o2 in (o1 as StackPanel).Children)
                {
                    var o3 = (o2 as Grid).Children[0] as TextBlock;
                    if (i >= dayOfWeek && i < (daysOfMonth + dayOfWeek))
                    {
                        //if (i < 7 && dayOfWeek > 6) ((o2 as Grid).Children[5] as StackPanel).Visibility = Visibility.Visible;
                        o3.Text = (i - dayOfWeek + 1).ToString();
                    }
                    else
                    {
                        o3.Text = "";
                        
                    }
                    (o2 as Grid).BorderThickness = new Thickness();
                    (o2 as Grid).Background = new SolidColorBrush();
                    i++;
                }
            }
            myBrush = new SolidColorBrush(Colors.Black);
            foreach (var o1 in Calendar.Children)
            {
                foreach (var o2 in (o1 as StackPanel).Children)
                {
                    var o3 = (o2 as Grid).Children[0] as TextBlock;
                    if (Int32.TryParse(o3.Text, out i) == false) return;
                    // hight light ngay hien tai
                    if (i == date.Day && DateTime.Today.Month == date.Month)
                    {
                        (o2 as Grid).Background = new SolidColorBrush(Colors.LightGray);
                    }
                    foreach (var tkn in ThongKeNgayKhacList)
                    {
                        DateTime date2 = DateTime.ParseExact(tkn.Ngay, "dd/MM/yyyy", new CultureInfo("vi-vn"));
                        if (i == date2.Day && date2.Month == date.Month)
                        {
                            (o2 as Grid).BorderBrush = new SolidColorBrush(Colors.Yellow);
                            (o2 as Grid).BorderThickness = new Thickness(0, 0, 0, 2);
                            break;
                        }
                    }
                    foreach (var tkn in ThongKeNgayList)
                    {
                        DateTime date2 = DateTime.ParseExact(tkn.Ngay,"dd/MM/yyyy", new CultureInfo("vi-vn"));
                        if (i == date2.Day && date2.Month == date.Month)
                        {
                            (o2 as Grid).BorderBrush = new SolidColorBrush(Colors.OrangeRed);
                            (o2 as Grid).BorderThickness = new Thickness(0, 0, 0, 2);
                            break;
                        }
                    }
                }
            }
        }
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void previousMonth(object sender, RoutedEventArgs e)
        {
            calendarDate = calendarDate.AddMonths(-1);
            Initialize_Calendar(calendarDate);
        }

        private void nextMonth(object sender, RoutedEventArgs e)
        {
            calendarDate = calendarDate.AddMonths(1);
            Initialize_Calendar(calendarDate);
        }

        /// <summary>
        /// Khi người dùng tap vào một ngày, sẽ hiện ra thống kê cho ngày hôm đó
        /// </summary>
        private void Grid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            thongkengayFlipView.Visibility = Visibility.Visible;
            btdetailStackPanel.Visibility = Visibility.Collapsed;
            Initialize_Calendar(calendarDate);

            // change background of the grid that is tapped
            (sender as Grid).Background = new SolidColorBrush(Colors.LightSeaGreen);
           
            TextBlock textblock = (sender as Grid).Children[0] as TextBlock;
            
            // nếu ô trống thì bỏ qua
            if (textblock.Text == null || textblock.Text == "") return;

            string date = Int32.Parse(textblock.Text).ToString("00") + "/" + CalendarHeader.Tag.ToString(); /* dd/MM/yyyy */
            ngayTextBlock.Text = date;
            //DateTime date = DateTime.ParseExact(str, "dd/MM/yyyy", new CultureInfo("vi-vn"));
            ThongKeNgay tkn = connection.Table<ThongKeNgay>().Where(r => r.Ngay == date).FirstOrDefault();

            if (tkn != null)
            {
                kaloduavaoTextBlock.Text = tkn.LuongKaloDuaVao.ToString();
                kalotieuhaoTextBlock.Text = tkn.LuongKaloTieuHao.ToString();
                int soluongbaitap = connection.ExecuteScalar<int>("SELECT COUNT(*) FROM ThongKeBaiTap WHERE IdThongKeNgay =?", tkn.IdThongKeNgay);
                soluongbtTextBlock.Text = soluongbaitap.ToString();
                int tongthoigiantap = connection.ExecuteScalar<int>("SELECT SUM(ThoiGianTap) FROM THONGKEBAITAP WHERE IdThongKeNgay =?", tkn.IdThongKeNgay);
                thoigiantapTextBlock.Text = tongthoigiantap.ToString();
                var q = connection.Table<ThongKeBaiTap>().Where(r=> r.IdThongKeNgay == tkn.IdThongKeNgay);
                List<BaiTapDaTap> baidatapList = new List<BaiTapDaTap>();
                foreach (var item in q)
                {
                    var tenbaitap = connection.Table<BaiTap>().Where(r => r.IdBaiTap == item.IdBaiTap).FirstOrDefault().TenBaiTap;
                    baidatapList.Add(new BaiTapDaTap() {
                                                        IdThongKeBaiTap = item.IdThongKeNgay,
                                                        IdThongKeNgay = item.IdThongKeNgay,
                                                        IdBaiTap = item.IdBaiTap,
                                                        TenBaiTap = tenbaitap,
                                                        LuongKaloTieuHao = item.LuongKaloTieuHao,
                                                        QuangDuong = item.QuangDuong,
                                                        ThoiGianTap = item.ThoiGianTap
                                                    });
                }
                (PieChart.Series[0] as PieSeries).ItemsSource = baidatapList;
               
            }
            else
            {
                kaloduavaoTextBlock.Text = "0";
                kalotieuhaoTextBlock.Text = "0";
                soluongbtTextBlock.Text = "0";
                thoigiantapTextBlock.Text = "0";
            }
            
        }

        public class BaiTapDaTap: ThongKeBaiTap
        {
            public string TenBaiTap { get; set; }
        }

        private void PieSeries_Tapped(object sender, TappedRoutedEventArgs e)
        {
            BaiTapDaTap bt = (sender as PieSeries).SelectedItem as BaiTapDaTap;
            btdetailStackPanel.Visibility = Visibility.Visible;
            tenbaitapTextBlock.Text = bt.TenBaiTap;
            thoigiantap1TextBlock.Text = bt.ThoiGianTap.ToString();
            luongkalotieuhaoTextBlock.Text = bt.LuongKaloTieuHao.ToString();
            if (bt.QuangDuong > 0.0)
            {
                quangduongTextBlock.Text = bt.QuangDuong.ToString();
            }
            else
                quangduongGrid.Visibility = Visibility.Collapsed;

            
        }
    }
}
