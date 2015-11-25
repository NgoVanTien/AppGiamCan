using GiamCan.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Serialization;
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
    public sealed partial class SuaThongTin : Page
    {
        string path;
        SQLite.Net.SQLiteConnection connection;
        public NguoiDung NguoiDung { get; set; }
        public SuaThongTin()
        {
            this.InitializeComponent();
            path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "giamcandb.sqlite");
            connection = new SQLite.Net.SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            NguoiDung = (NguoiDung)e.Parameter;
            this.DataContext = NguoiDung;
            ngaysinhDatePicker.Date = DateTime.ParseExact(NguoiDung.NgaySinh,"dd/MM/yyyy", new CultureInfo("vi-vn"));
            if (NguoiDung.GioiTinh == "Nam")
            {
                gioitinhPanel.Children.OfType<RadioButton>().FirstOrDefault(r => r.Content.ToString() == "Nam").IsChecked = true;
            }
            else
            {
                gioitinhPanel.Children.OfType<RadioButton>().FirstOrDefault(r => r.Content.ToString() == "Nữ").IsChecked = true;
            }
        }
        private void xongButton_Click(object sender, RoutedEventArgs e)
        {
            NguoiDung.NgaySinh = ngaysinhDatePicker.Date.ToString("dd/MM/yyyy");
            RadioButton selectedRadio = gioitinhPanel.Children.OfType<RadioButton>().FirstOrDefault(r => r.IsChecked == true);
            NguoiDung.GioiTinh = selectedRadio.Content.ToString();
            connection.Update(NguoiDung);

            // cap nhat lai file nguoi dung hien tai
            IsolatedStorageFile ISOFile = IsolatedStorageFile.GetUserStoreForApplication();
            if (ISOFile.FileExists("CurrentUser"))
            {
                ISOFile.DeleteFile("CurrentUser");
                using (IsolatedStorageFileStream fileStream = ISOFile.OpenFile("CurrentUser", FileMode.Create))
                {
                    DataContractSerializer serializer = new DataContractSerializer(typeof(NguoiDung));

                    serializer.WriteObject(fileStream, NguoiDung);

                }
            }

            Frame.Navigate(typeof(ThongTinCaNhan), NguoiDung);
            Frame.BackStack.RemoveAt(Frame.BackStackDepth - 1);
            Frame.BackStack.RemoveAt(Frame.BackStackDepth - 1);
        }

        private void huyButton_Click(object sender, RoutedEventArgs e)
        {
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
            }
        }

    }
}
