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

    public sealed partial class ChonMucTap : Page
    {
        SQLite.Net.SQLiteConnection connection = TrangChu.connection;
        public static int valueRadio;
        
        NguoiDung nguoidung;

        public ChonMucTap()
        {
            this.InitializeComponent();
        }
        
        public int valueRadioButton()
        {
            if (radioLevel1.IsChecked == true)
            {
                return  1;
            }
            else if (radioLevel2.IsChecked == true)
            {
                return 2;
            }
            else if (radioLevel3.IsChecked == true)
            {
               return  3;
            };

            return 0;
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
           // muctieu = (MucTieu)e.Parameter;
            nguoidung = (NguoiDung)e.Parameter;

        }

        private void tiepButton_Click(object sender, RoutedEventArgs e)
        {
            valueRadio = valueRadioButton();
            Frame.Navigate(typeof(BaiTapBung), valueRadio);
            
        }


    }
}
