using GiamCan.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Notifications;
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
    public sealed partial class DatNhacNho : Page
    {
        NguoiDung nguoidung;
        public DatNhacNho()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            nguoidung = (NguoiDung)e.Parameter;
            // dat mac dinh ngay la hom nay
            ngayDatePicker.Date = DateTime.Today;
        }
        private async void themNhacNhoButton_Click(object sender, RoutedEventArgs e)
        {
            // kiem tra ngay gio bao
            if (ngayDatePicker.Date == null || ngayDatePicker.Date < DateTime.Today || (ngayDatePicker.Date == DateTime.Today && gioTimePicker.Time < DateTime.Now.TimeOfDay))
            {
                MessageDialog msDialog1 = new MessageDialog("Ngày báo phải ở tương lai");
                await msDialog1.ShowAsync();
                return;
            }
            // lay thoi gian 
            int day = ngayDatePicker.Date.Value.Day;
            int month = ngayDatePicker.Date.Value.Month;
            int year = ngayDatePicker.Date.Value.Year;
            int hour = gioTimePicker.Time.Hours;
            int minute = gioTimePicker.Time.Minutes;
            DateTime thoigianbao = new DateTime(year, month, day, hour, minute, 0);
            string content = noidungTextBox.Text;
            //int snooze = Convert.ToInt32(((ComboBoxItem)baolaiComboBox.SelectedItem).Tag) * 60;
            //Debug.WriteLine(snooze);
            var xmlString = @"
<toast launch='args' scenario='alarm'>
    <visual>
        <binding template='ToastGeneric'>
            <text>Giảm cân 360</text>
            <text>" + content + @"</text>
        </binding>
    </visual>
    <actions>
        <input id='snoozeTime' type='selection' defaultInput='5'>
            <selection id = '1' content = '1 minutes' />
            <selection id = '5' content = '5 minutes' />
            <selection id = '10' content = '10 minutes' />
            <selection id = '20' content = '20 minutes' />
            <selection id = '30' content = '30 minutes' />
        </input >
        <action activationType='system' arguments = 'snooze' hint-inputId='snoozeTime'
                content = 'Báo lại' />
        <action activationType='system' arguments = 'dismiss'
                content = 'Bỏ qua' />
    </actions>
    <audio loop='true' src='ms-winsoundevent:Notification.Looping.Alarm2' />
</toast>";
            var doc = new Windows.Data.Xml.Dom.XmlDocument();
            doc.LoadXml(xmlString);
            //var toast = new ToastNotification(doc);
            ToastNotifier toastNotifier = ToastNotificationManager.CreateToastNotifier();
            //thoi gian bao lai
            //TimeSpan snoozeInterval = TimeSpan.FromSeconds(snooze);
            // bao vao thoi gian 
            //var scheduledToast = new ScheduledToastNotification(doc, thoigianbao, snoozeInterval, 0);
            var scheduledToast = new ScheduledToastNotification(doc, thoigianbao);
            toastNotifier.AddToSchedule(scheduledToast);

            MessageDialog msDialog = new MessageDialog("Thêm nhắc nhở thành công");
            await msDialog.ShowAsync();
            Frame.Navigate(typeof(TrangChu), nguoidung);
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
