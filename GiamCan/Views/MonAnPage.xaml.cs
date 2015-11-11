using System;
using GiamCan.Model;
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
using Windows.UI.Popups;
using System.Data;
using System.ComponentModel;
using System.Runtime.CompilerServices;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace GiamCan.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MonAnPage : Page, INotifyPropertyChanged
    {
        
        string path;
        SQLite.Net.SQLiteConnection conn;
        ThongKeNgay tkngay;
        // list chua cac mon an tinh
        private List<MonAn> monAnList = new List<MonAn>();
        public List<MonAn> MonAnList
        {
            get { return monAnList; }
            set { monAnList = value; }
        }
        // list chua cac mon an da chon
        private List<ThucDon> monanChonList = new List<ThucDon>();
        private List<ThucDon> monanDBList;



        private static double kaloSum; //tong luong kalo of cac mon an da check
        public double TongLuongKalo
        {
            get { return kaloSum; }
            set {
                if(TongLuongKalo != value)
                {
                    kaloSum = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public MonAnPage()
        {
            this.InitializeComponent();
            path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "giamcandb.sqlite");
            conn = new SQLite.Net.SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path);

        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            monAnList = conn.Table<MonAn>().ToList<MonAn>();
            tkngay = e.Parameter as ThongKeNgay;
            monanDBList = conn.Table<ThucDon>().Where(r => r.IdThongKeNgay == tkngay.IdThongKeNgay).ToList<ThucDon>();
            TongLuongKalo = 0;
        }

        

        //hien thi dialog ve nhan xet luon kalo da an
        private async void XongBtn_Click(object sender, RoutedEventArgs e)
        {
            if(kaloSum == 0)
            {
                var ms = new MessageDialog("Bạn nên chọn lượng Cals nhập vào \nđể thống kê chính xác nhất");
                await ms.ShowAsync();
                return;
            }
            var msKalo = new MessageDialog("Tổng lượng Cals bạn đã ăn là: " + kaloSum);
            await msKalo.ShowAsync();
            conn.Execute("DELETE FROM ThucDon WHERE IdThongKeNgay =?", tkngay.IdThongKeNgay);
            //add cac thuc an da chon vao database
            foreach (var item in monanChonList)
            {
                conn.Insert(item);
            }

            tkngay.LuongKaloDuaVao = conn.ExecuteScalar<double>("SELECT SUM(LuongKalo) FROM THUCDON WHERE IdThongKeNgay =?", tkngay.IdThongKeNgay);
            conn.Update(tkngay);
            // lay thong tin nguoidung de chuyen ve trang chu
            MucTieu muctieu = conn.Table<MucTieu>().Where(r => r.IdMucTieu == tkngay.IdMucTieu).FirstOrDefault();
            NguoiDung nguoidung = conn.Table<NguoiDung>().Where(r => r.TenDangNhap == muctieu.TenDangNhap).FirstOrDefault();
            Frame.Navigate(typeof(TrangChu), nguoidung);
        }

        //chuyen trang sau dialog
        //public void chuyenPage(IUICommand command)
        //{
        //    //Frame.Navigate(typeof(KaloNgoaiPage));
        //}
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            ThucDon td = new ThucDon();

            // them cac IdThongKeNgay o day
            td.IdThongKeNgay = tkngay.IdThongKeNgay;
            var checkMonAn = sender as CheckBox;
            td.IdMonAn = (int)(checkMonAn.Content as StackPanel).Tag;

            td.SoLuong = Int32.Parse(((checkMonAn.Content as StackPanel).Children.OfType<ComboBox>().FirstOrDefault().SelectedItem as ComboBoxItem).Content.ToString());
            var listBlock = AllChildren(checkMonAn.Content as StackPanel).OfType<TextBlock>();
            foreach (var item in listBlock)
            {
                if ((item as TextBlock).Name.Equals("kaloBlock"))
                {
                    td.LuongKalo = double.Parse((item as TextBlock).Text);
                    TongLuongKalo += double.Parse((item as TextBlock).Text);
                    
                }
            }
            // chua co idThongKeNgay
            monanChonList.Add(td);
        }
        private void idCheck_Unchecked(object sender, RoutedEventArgs e)
        {
            int checktag = Int32.Parse(((sender as CheckBox).Content as StackPanel).Tag.ToString());
            var listBlock = AllChildren((sender as CheckBox).Content as StackPanel).OfType<TextBlock>();
            foreach (var item in listBlock)
            {
                if ((item as TextBlock).Name.Equals("kaloBlock"))
                {
                    TongLuongKalo -= double.Parse((item as TextBlock).Text);
                    // tra lai gia tri mac dinh cho text
                    //(item as TextBlock).Text = monAnList.Find(r => r.IdMonAn == checktag).LuongKaloTrenDonVi.ToString();
                }
            }
            ThucDon td = monanChonList.Find(r => r.IdMonAn == checktag);
            monanChonList.Remove(td);
        }

        private void HuyBtn_Click(object sender, RoutedEventArgs e)
        {
            var childControls = AllChildren(viewLst);
            foreach (var item in childControls)
            {
                if (item is CheckBox)
                {
                    ((CheckBox)item).IsChecked = false;
                }
            }
        }


        // find all child of parent
        private void soLuongCom_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //init_monandachon();
            double kaloFollow;
            ComboBox combo = sender as ComboBox;

            // cho lan chay dau tien, khi tag chua duoc binding
            if (combo.Tag == null) return;
            // lay tag của nhóm
            int checkTag = (int)combo.Tag;

            // lấy số lượng chọn
            int soluong = Int32.Parse((combo.SelectedItem as ComboBoxItem).Content.ToString());
            // thay doi phan tu o list
            ThucDon td = monanChonList.Find(r => r.IdMonAn == checkTag);
            td.SoLuong = soluong;

            var lstText = AllChildren(combo.Parent).OfType<TextBlock>();
            // truong hop check box chua duoc check
            if (((combo.Parent as StackPanel).Parent as CheckBox).IsChecked == false)
            {
                foreach (var item in lstText)
                {
                    if ((item as TextBlock).Name.Equals("kaloBlock"))
                    {
                        // lay luong kalo cua mon an tu list;
                        double kaloBD = monAnList.Find(r => r.IdMonAn == checkTag).LuongKaloTrenDonVi;
                        // nhan voi so luong
                        kaloFollow = kaloBD * soluong;
                        // roi day ra man hinh
                        (item as TextBlock).Text = kaloFollow.ToString();

                    }
                }
            }
            // truong hop check box da duoc check
            else
            {
                foreach (var item in lstText)
                {
                    if ((item as TextBlock).Name.Equals("kaloBlock"))
                    {
                        // phai tru di luong ban dau
                        TongLuongKalo -= Double.Parse((item as TextBlock).Text);
                        // lay luong kalo cua mon an tu list;
                        double kaloBD = monAnList.Find(r => r.IdMonAn == checkTag).LuongKaloTrenDonVi;
                        // nhan voi so luong
                        kaloFollow = kaloBD * soluong;
                        td.LuongKalo = kaloFollow;
                        // cong lai luong thay doi
                        TongLuongKalo += kaloFollow;
                        // roi day ra man hinh
                        (item as TextBlock).Text = kaloFollow.ToString();

                    }
                }
            }
        }

        
        /// <summary>
        /// hàm này để lấy list thức ăn đã chọn từ lần trước, rồi check lại các món ăn như lần trước
        /// </summary>
        private void init_monandachon()
        {
            var checkboxList = AllChildren(viewLst).OfType<CheckBox>();
            foreach (CheckBox checkbox in checkboxList)
            {
                var contentSP = checkbox.Content as StackPanel;
                foreach (ThucDon monan in monanDBList)
                {
                    if(contentSP.Tag.ToString() == monan.IdMonAn.ToString())
                    {
                        checkbox.IsChecked = true;
                        contentSP.Children.OfType<ComboBox>().First().SelectedIndex = (int)(monan.SoLuong - 1); 
                    }
                }
            }
        }

        //tim tat ca cac phan tu con trong listView
        private List<DependencyObject> AllChildren(DependencyObject parent)
        {
            var _List = new List<DependencyObject>();
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var _Child = VisualTreeHelper.GetChild(parent, i);
                if (_Child is DependencyObject)
                {
                    _List.Add(_Child as DependencyObject);
                }
                _List.AddRange(AllChildren(_Child));
            }
            return _List;
        }

        private void searchBtn_Click(object sender, RoutedEventArgs e)
        {
            searchBox.Text = "";
            viewLst.ItemsSource = monAnList;
        }

        private void searchBox_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (searchBox.Text == "")
            {
                viewLst.ItemsSource = monAnList;

            }
            else
            {
                var monanList1 = monAnList.FindAll(r => r.TenMonAn.Contains(searchBox.Text));
                viewLst.ItemsSource = monanList1;
            }
        }



        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void ThemLuongKaloBtn_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(KaloNgoaiPage), tkngay);
        }

        private void viewLst_Loaded(object sender, RoutedEventArgs e)
        {
            init_monandachon();
        }
    }
}
