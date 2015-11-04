using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GiamCan.Model
{
    public class MucTieu
    {
        [PrimaryKey, AutoIncrement]
        public int IdMucTieu { get; set; }
        
        // tên đăng nhập của người dùng
        // tạo foreign key??/
        [ForeignKey(typeof(NguoiDung))]
        public string TenDangNhap { get; set; }
        public string ThoiGianBatDau { get; set; }
        public int SoNgay { get; set; }
        public double ChieuCaoBanDau { get; set; }
        public double CanNangBanDau { get; set; }
        public int SoCanMuonGiam { get; set; }
        public double MucHoanThanh { get; set; }
        [ManyToOne]
        public NguoiDung NguoiDung { get; set; }
    }
}
