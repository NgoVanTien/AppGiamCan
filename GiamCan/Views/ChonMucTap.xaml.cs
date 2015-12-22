using GiamCan.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
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
        string tenBt;
        // lấy thông tin đầu vào bài tập
 
        //NguoiDung nguoidung;

        public ChonMucTap()
        {
            this.InitializeComponent();
            // tiepButton.IsEnabled = false;

        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // muctieu = (MucTieu)e.Parameter;
           // nguoidung = (NguoiDung)e.Parameter;
            tenBt = (String)e.Parameter;
            tenBT.Text = tenBt;
     
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


        private async void tiepButton_Click(object sender, RoutedEventArgs e)
        {
            if (valueRadioButton() != 0)
            {
                valueRadio = valueRadioButton();
                mucTap muctap = new mucTap();
                muctap.level = valueRadio;
                muctap.tenBaiTap = tenBt;
                Frame.Navigate(typeof(BaiTapBung), muctap);
            }
            else
            {
                MessageDialog msDialog = new MessageDialog("Bạn chưa chọn mức tập, vui lòng chọn trước khi tập");

                msDialog.Commands.Add(new UICommand("Ok") { Id = 0 });
                msDialog.DefaultCommandIndex = 1;
                var result = await msDialog.ShowAsync();
                if(result.Label == "Ok")
                {
                    mucTap muctap = new mucTap();
                    muctap.level = valueRadio;
                    muctap.tenBaiTap = tenBt;
                    Frame.Navigate(typeof(ChonMucTap),tenBt);
                }
            }
            // }
        }

        private void huyButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(DanhSachBaiTap));
        }
   
}
    public class mucTap
    {
        public int level { get; set; }
        public string tenBaiTap { get; set; }
    }
}

