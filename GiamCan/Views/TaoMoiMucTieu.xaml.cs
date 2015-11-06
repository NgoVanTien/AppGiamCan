using GiamCan.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
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
    public sealed partial class TaoMoiMucTieu : Page
    {
        double weight, height, bmi = 0, idealweight = 0, bmr = 0;
        MucTieu muctieu;
        NguoiDung nguoidung;
        ChiSo chiso;
        public TaoMoiMucTieu()
        {
            this.InitializeComponent();
        }

        private async void themMucTieuButton_Click(object sender, RoutedEventArgs e)
        {
            muctieu = new MucTieu();
            if(await checkNumberTextBox() == true)
            {
                // nếu cân cặng thấp hơn cân nặng lý tưởng ==> không thuộc đối tượng giảm cân => không cho tạo mục tiêu
                if(idealweight >= weight)
                {
                    MessageDialog msDialog = new MessageDialog("Bạn không nên giảm cân!");
                    await msDialog.ShowAsync();
                    return;
                }
                muctieu.TenDangNhap = nguoidung.TenDangNhap;
                muctieu.CanNangBanDau = weight;
                muctieu.ChieuCaoBanDau = height;
                Frame.Navigate(typeof(TaoMoiMucTieu2), muctieu);
            }
           
        }

        private void huyButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }

        private void chieucaobdTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            tinhChiSo();
        }

        private void cannangbdTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            tinhChiSo();
        }

        private void muchoatdongComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            tinhChiSo();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            nguoidung = (NguoiDung)e.Parameter;
            // if(Frame.BackStack.)

            // gan ngay sinh gioi tinh vao cho chiso
            chiso = new ChiSo();
            chiso.GioiTinh = nguoidung.GioiTinh;
            DateTime birthday = Convert.ToDateTime(nguoidung.NgaySinh);
            chiso.Tuoi = DateTime.Today.Year - birthday.Year;
        }

        // nguoi dung xem xem chi so bmi la gi
        private void TextBlock_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            
        }


        private void tinhChiSo()
        {
            if (Double.TryParse(chieucaobdTextBox.Text.Replace(".", ","), out height) && Double.TryParse(cannangbdTextBox.Text.Replace(".", ","), out weight))
            {
                chiso.Weight = weight;
                chiso.Height = height;
                bmi = chiso.tinhBMI();
                idealweight = chiso.tinhCanNangLyTuong();
                bmr = chiso.tinhBMR();
            }
            cannanglytuongTextBlock.Text = (idealweight > 0) ? string.Format("{0:0.00} kg", idealweight) : "";
            if (bmi!=0)
            {
                chisobmiTextBlock.Text = string.Format("{0:0.00}", bmi);
                if (bmi < 18.5) { loikhuyenTextBlock.Text = "Bạn đang THIẾU CÂN!"; }
                else if (bmi < 25) { loikhuyenTextBlock.Text = "Cơ thể CHUẨN"; }
                else if (bmi < 30) { loikhuyenTextBlock.Text = "Bạn đang THỪA CÂN, nên giảm cân ngay!"; }
                else { loikhuyenTextBlock.Text = "Bạn đang quá THỪA CÂN, giảm cân ngay lập tức!"; }

            }
            if(bmr != 0)
            {
                double heso;
                ComboBoxItem selected = (ComboBoxItem)muchoatdongComboBox.SelectedItem;
                if(selected!=null && Double.TryParse(selected.Tag.ToString().Replace(".",","), out heso)){
                    bmr = bmr * heso;
                    bmrTextBlock.Text = string.Format("{0:0.00}", bmr);
                }
            }
        }

        /// <summary>
        /// kiem tra du lieu nhap vao
        /// </summary>
        /// <returns></returns>
        private async Task<bool> checkNumberTextBox()
        {
            MessageDialog msDialog;
            
            if(chieucaobdTextBox.Text == "" || cannangbdTextBox.Text == "")
            {
                msDialog = new MessageDialog("Vui lòng điền đầy đủ thông tin");
                await msDialog.ShowAsync();
                return false;
            }
            if(Double.TryParse(chieucaobdTextBox.Text.Replace(".",","), out height) && Double.TryParse(cannangbdTextBox.Text.Replace(".", ","), out weight))
            {
                if(weight <= 0 || height <= 0)
                {
                    msDialog = new MessageDialog("Chiều cao hoặc Cân nặng bị sai");
                    await msDialog.ShowAsync();
                    return false;
                }
            }
            return true;
        }
    }

    class ChiSo
    {
        /// <summary>
        /// Weight tính bằng cm, Height tính bằng kg
        /// </summary>
        public double Weight { get; set; }
        public double Height { get; set; }
        public string GioiTinh { get; set; }
        public int Tuoi { get; set; }

        public ChiSo() { }
        public ChiSo(double weight, double height, string gioitinh, int tuoi)
        {
            this.Weight = weight;
            this.Height = height;
            this.GioiTinh = gioitinh;
            this.Tuoi = tuoi;
        }
        public double tinhBMI()
        {
            if(Weight> 0 && Height > 0)
            {
                double bmi = Weight / ((Height / 100.0) * (Height / 100.0));
                return bmi;
            }
            return 0;
        }
        public double tinhCanNangLyTuong()
        {
            if (GioiTinh == "Nam")
            {
                return 22 * (Height/100.0) * (Height/100.0);
            }
            else
            {
                return 20 * (Height/100.0) * (Height/100.0);
            }
        }
        public double tinhBMR()
        {
            double bmr;
            if (GioiTinh == "Nam")
            {
                bmr = 13.397 * Weight + 4.799 * Height - 5.677 * Tuoi + 88.362;
            }
            else
            {
                bmr = 9.247 * Weight + 3.098 * Height - 4.330 * Tuoi + 447.593;
            }
            return bmr;
        }

        public double tinhSoCanGiamDeNghi()
        {
            return Math.Round(Weight - tinhCanNangLyTuong());
        }
        
        public int tinhSoNgayGiamDeNghi()
        {
            int songay = Convert.ToInt32(Math.Round(tinhSoCanGiamDeNghi() / 0.1));
            return songay;

        }
    }
}
