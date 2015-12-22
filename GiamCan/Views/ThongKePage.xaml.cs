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
using Windows.UI.Popups;
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
        public class PieData
        {
            public string Title { get; set; }
            public double Value { get; set; }
            public PieData(string title, double value)
            {
                Title = title;
                Value = value;
            }
            public PieData() { }
        }

        SQLite.Net.SQLiteConnection connection = TrangChu.connection;
        public event PropertyChangedEventHandler PropertyChanged;
        SolidColorBrush myBrush;

        DateTime calendarDate;

        public List<ThongKeNgay> ThongKeNgayList { get; set; }
        public List<ThongKeNgay> ThongKeNgayKhacList { get; set; }

        MucTieu muctieu;
        NguoiDung nguoidung;

        public double LuongKaloTieuHao { get; set; }
        public double LuongKaloDuaVao { get; set; }

        public ThongKePage()
        {
            this.InitializeComponent();
            calendarDate = DateTime.Today;

        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // lấy người dùng hiện tại
            nguoidung = TrangChu.nguoidung;
            // lay muctieu hien tai
            muctieu = TrangChu.muctieu;
            
            // neu muctieu == null -> thu lay muctieu da hoan thanh truoc do
            if (muctieu == null) muctieu = connection.Table<MucTieu>().Where(r=>r.TenDangNhap == nguoidung.TenDangNhap && r.TrangThai=="Hoàn thành").OrderBy(r=>r.IdMucTieu).FirstOrDefault();

            if (muctieu != null)
            {
                // lay danh sách các ngày từ mục tiêu hiện tại
                ThongKeNgayList = connection.Table<ThongKeNgay>().Where(r => r.IdMucTieu == muctieu.IdMucTieu).ToList<ThongKeNgay>();
                // lấy danh sách các mục tiêu khác của người dùng hiện tại (để hiển thị)
                List<MucTieu> muctieukhacList = connection.Table<MucTieu>().Where(r => r.TenDangNhap == nguoidung.TenDangNhap && r.IdMucTieu != muctieu.IdMucTieu).ToList();
                ThongKeNgayKhacList = new List<ThongKeNgay>();
                // lấy danh sách các ngày từ các mục tiêu trước
                foreach (var muctieukhac in muctieukhacList)
                {
                    var list = connection.Table<ThongKeNgay>().Where(r => r.IdMucTieu == muctieukhac.IdMucTieu).ToList();
                    list.ForEach(r => ThongKeNgayKhacList.Add(r));
                }
            }
            else
            {
                ThongKeNgayList = new List<ThongKeNgay>();
                List<MucTieu> muctieukhacList = connection.Table<MucTieu>().Where(r => r.TenDangNhap == nguoidung.TenDangNhap).ToList();

                ThongKeNgayKhacList = new List<ThongKeNgay>();
                // lấy danh sách các ngày từ các mục tiêu trước
                foreach (var muctieukhac in muctieukhacList)
                {
                    var list = connection.Table<ThongKeNgay>().Where(r => r.IdMucTieu == muctieukhac.IdMucTieu).ToList();
                    list.ForEach(r => ThongKeNgayKhacList.Add(r));
                }
            }

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
            if ((date1.DayOfWeek == DayOfWeek.Friday && daysInMoth == 31) || (date1.DayOfWeek == DayOfWeek.Saturday && daysInMoth > 30))
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
                    if (Int32.TryParse(o3.Text, out i) == false) break;
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
                        DateTime date2 = DateTime.ParseExact(tkn.Ngay, "dd/MM/yyyy", new CultureInfo("vi-vn"));
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
            ThongKeNgay tkn = connection.Table<ThongKeNgay>().Where(r => r.IdMucTieu == muctieu.IdMucTieu && r.Ngay == date).FirstOrDefault();

            if (tkn != null)
            {
                thongkeFlipView.Visibility = Visibility.Visible;
                muctieuTextBlock.Text = tkn.IdMucTieu.ToString();
                kaloduavaoTextBlock.Text = tkn.LuongKaloDuaVao.ToString();
                kalotieuhaoTextBlock.Text = tkn.LuongKaloTieuHao.ToString();

                int soluongbaitap = connection.ExecuteScalar<int>("SELECT COUNT(*) FROM ThongKeBaiTap WHERE IdThongKeNgay =?", tkn.IdThongKeNgay);
                soluongbtTextBlock.Text = soluongbaitap.ToString();

                int tongthoigiantap = connection.ExecuteScalar<int>("SELECT SUM(ThoiGianTap) FROM THONGKEBAITAP WHERE IdThongKeNgay =?", tkn.IdThongKeNgay);
                thoigiantapTextBlock.Text = tongthoigiantap.ToString();

                var q = connection.Table<ThongKeBaiTap>().Where(r => r.IdThongKeNgay == tkn.IdThongKeNgay);
                List<BaiTapDaTap> baidatapList = new List<BaiTapDaTap>();
                foreach (var item in q)
                {
                    var tenbaitap = connection.Table<BaiTap>().Where(r => r.IdBaiTap == item.IdBaiTap).FirstOrDefault().TenBaiTap;
                    baidatapList.Add(new BaiTapDaTap()
                    {
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
                thongkengayFlipView.Visibility = Visibility.Collapsed;
                //kaloduavaoTextBlock.Text = "0";
                //kalotieuhaoTextBlock.Text = "0";
                //soluongbtTextBlock.Text = "0";
                //thoigiantapTextBlock.Text = "0";
            }

        }

        public class BaiTapDaTap : ThongKeBaiTap
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

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if(muctieu != null)
                LoadChartContents();
        }

        private void LoadChartContents()
        {
            // tổng lượng kcal tiêu hao cho bài tập -> cần sửa lại
            double kcalbaitap = connection.ExecuteScalar<double>("Select sum(LuongKaloTieuHao) from ThongKeNgay where IdMucTieu = ?", muctieu.IdMucTieu);
            // tổng lượng kcal đưa vào của mục tiêu hiện tại
            double kcalvao = connection.ExecuteScalar<double>("Select sum(LuongKaloDuaVao) from ThongKeNgay where IdMucTieu = ?", muctieu.IdMucTieu) + connection.ExecuteScalar<double>("Select sum(LuongKaloNgoaiDuKien) from ThongKeNgay where IdMucTieu=?", muctieu.IdMucTieu);
            // tổng lượng kalo cho hoạt động bình thường của mục tiêu hiện tại
            double kcalbinhthuong = muctieu.ChiSoBMR * ThongKeNgayList.Count;
            // lượng kcal phải giảm để giảm đúng số cân mong muốn
            double kcalphaigiam = muctieu.SoCanMuonGiam * 7700;
            // lượng kcal đã giảm = tổng lượng kcal tiêu hao - tổng lượng kcal đưa vào
            double kcaldagiam = kcalbinhthuong + kcalbaitap - kcalvao;
            // lượng kcal còn lại = kcal phải giảm - kcal đã giảm
            double kcalconlai = kcalphaigiam - kcaldagiam;
            List<PieData> pieList = new List<PieData>()
            {
                new PieData() { Title="kcal đã giảm", Value=kcaldagiam },
                new PieData() { Title="kcal còn lại", Value=kcalconlai }
            };
            (PieChart1.Series[0] as PieSeries).ItemsSource = pieList;

            //List<ThongKeBaiTap> thongKeBaiTap = connection.Table<ThongKeBaiTap>().ToList<ThongKeBaiTap>(); ;
            //double tongKaloTieuHao = 0;
            //double kaloBieuDo = 0;
            //double kaloBaiTap = 0;
            //List<double> kaloTapList = new List<double>();
            //for (int i = 0; i < thongKeBaiTap.Count; i++)
            //{
            //    kaloBaiTap += thongKeBaiTap[i].LuongKaloTieuHao;
            //}
            //double kaloMuonGiam = 0;
            //kaloMuonGiam = muctieu.SoCanMuonGiam * 7700;

            ////lay gia tri tu bang thong ke ngay
            //List<ThongKeNgay> thongKeLst  = connection.Table<ThongKeNgay>().ToList<ThongKeNgay>();
            //for (int i = 0; i < thongKeLst.Count; i++)
            //{
            //    tongKaloTieuHao += kaloBaiTap + thongKeLst[i].LuongKaloTieuHao - thongKeLst[i].LuongKaloDuaVao - thongKeLst[i].LuongKaloNgoaiDuKien;
            //}

            //if (tongKaloTieuHao < 0)
            //{
            //    mesagBlock.Text = "Lượng Kalo bạn đưa vào quá nhiều, bạn có thể bị tăng cân";
            //}
            //else
            //{
            //    kaloBieuDo = kaloMuonGiam - tongKaloTieuHao; //kalocon lai can phai giam
            //    mesagBlock.Text = "Bạn đã giảm được " + string.Format("{0:0.00}", tongKaloTieuHao / 7700) + " kg";
            //    List<PieData> PieList = new List<PieData>()
            //    {
            //        new PieData("Kalo Đã giảm",tongKaloTieuHao),
            //        new PieData("Kalo phải giảm", kaloBieuDo),
            //     };
            //    (PieChart1.Series[0] as PieSeries).ItemsSource = PieList;
            //}



            //BIEU DO COT CHO TUNG NGAY
            (lineChart.Series[0] as LineSeries).ItemsSource = ThongKeNgayList;
            ////thong ke theo tung ngay
            //double kaloGiamHangNgay = muctieu.SoCanMuonGiam * 7700 / muctieu.SoNgay;
            //List<ThongKeBaiTap> thongKeBaiTapMotNgay = new List<ThongKeBaiTap>();
            //double kaloTieuHaoNgay = 0;
            //// ObservableCollection<PieData> ColunmList = new ObservableCollection<PieData>();
            //List<PieData> lineList = new List<PieData>();
            //double kaloTapTieuHao = 0;
            //double tongKaloDuaVao = 0;
            //int idNgay;
            //double kaloTap = 0;
            ////ColunmList.Add(new PieData("", 0));
            //for (int i = 0; i < thongKeLst.Count; i++)
            //{
            //    idNgay = thongKeLst[i].IdThongKeNgay;
            //    kaloTap = connection.ExecuteScalar<double>("Select sum(LuongKaloTieuHao) from ThongKeBaiTap where IdThongKeNgay =?", idNgay);
            //    kaloTapTieuHao = kaloTap + thongKeLst[i].LuongKaloTieuHao;
            //    tongKaloDuaVao = thongKeLst[i].LuongKaloDuaVao + thongKeLst[i].LuongKaloNgoaiDuKien;
            //    kaloTieuHaoNgay = kaloTapTieuHao - tongKaloDuaVao;
            //    //ColunmList.Add(new PieData("ngay " + (i + 1), kaloTieuHaoNgay));
            //    lineList.Add(new PieData("ngày " + idNgay, kaloTieuHaoNgay));
            //    kaloTap = 0;
            //}
            ////ColunmList.Add(new PieData("1", 1330));
            ////ColunmList.Add(new PieData("2", 800));
            ////ColunmList.Add(new PieData("3", 1200));
            ////ColunmList.Add(new PieData("4", 1000));
            ////(ColumnChart.Series[0] as ColumnSeries).ItemsSource = ColunmList;
            ////(lineChart.Series[0] as ColumnSeries)
            //(lineChart.Series[0] as LineSeries).ItemsSource = lineList;
        }

        public async void share_Clicked(object sender, RoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("https://www.facebook.com/sharer/sharer.php?app_id=113869198637480&sdk=joey&u=https%3A%2F%2Fwww.youtube.com%2Fwatch%3Fv%3DhP8lqgRQC4o&display=popup&ref=plugin&src=share_button"));
        }


    }
}
