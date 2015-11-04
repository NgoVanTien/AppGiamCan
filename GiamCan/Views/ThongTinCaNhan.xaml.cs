using GiamCan.Model;
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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace GiamCan.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ThongTinCaNhan : Page
    {
        public NguoiDung NguoiDung { get; set; }
        public ThongTinCaNhan()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// lấy dữ liệu từ trang chủ gửi vào ( Người Dùng)
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            NguoiDung = (NguoiDung)e.Parameter;
            this.DataContext = NguoiDung;
        }

        private void suathongtin_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SuaThongTin), NguoiDung);
        }

        private void trove_Click(object sender, RoutedEventArgs e)
        {
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
            }
        }
    }
}
