using GiamCan.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
        DateTime calendarDate;
        public List<ThongKeNgay> ThongKeNgayList { get; set; }
        private List<ThongKeBaiTap> thongkeBaiTapList;

        public event PropertyChangedEventHandler PropertyChanged;
        SolidColorBrush myBrush;

        public List<ThongKeBaiTap> BaiTapList
        {
            get { return thongkeBaiTapList; }
            set {
                if (value != BaiTapList)
                {
                    thongkeBaiTapList = value;
                    NotifyPropertyChanged();
                }
            }
        }

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
            MucTieu muctieu = e.Parameter as MucTieu;
            // tu muc tieu lay danh sach thongkengay
            ThongKeNgayList = connection.Query<ThongKeNgay>("SELECT * FROM ThongKeNgay WHERE IdMucTieu=?", muctieu.IdMucTieu);
            Initialize_Calendar(calendarDate);
        }

        void Initialize_Calendar(DateTime date)
        {
            CalendarHeader.Text = date.ToString("MMMM yyyy");
            DateTime date1 = new DateTime(date.Year, date.Month, 1);
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
                        o3.Text = (i - dayOfWeek + 1).ToString();
                        // border ngay hien tai
                        myBrush = new SolidColorBrush(Colors.Black);
                        if (i == date.Day && DateTime.Today.Month == date.Month)
                        {
                            (o2 as Grid).BorderBrush = myBrush;   
                            (o2 as Grid).BorderThickness = new Thickness(1);
                        }
                        else
                        {
                            (o2 as Grid).BorderThickness = new Thickness();
                        }
                        foreach (var tkNgay in ThongKeNgayList)
                        {
                            DateTime date2 = DateTime.Parse(tkNgay.Ngay);
                            if(i == date2.Day && date2.Month == date.Month)
                            {
                                (o2 as Grid).BorderBrush = new SolidColorBrush(Colors.OrangeRed);
                                (o2 as Grid).BorderThickness = new Thickness(0, 0, 0, 1);
                            }
                            
                        }
                    }
                    else
                    {
                        o3.Text = "";
                        (o2 as Grid).BorderThickness = new Thickness();
                        (o2 as Grid).Background = new SolidColorBrush();
                    }
                    i++;
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

        

        private void Grid_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            myBrush = new SolidColorBrush(Colors.CadetBlue);
            (sender as Grid).Background = myBrush;
        }

        private void Grid_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            myBrush = new SolidColorBrush(Colors.White);
            (sender as Grid).Background = myBrush;
        }
    }
}
