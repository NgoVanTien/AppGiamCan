using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GiamCan.Model
{
    public class NguoiDung
    {
        [PrimaryKey]
        public string TenDangNhap { get; set; }
        public string MatKhau { get; set; }
        public string NgaySinh { get; set; }
        public string GioiTinh { get; set; }

        //One to many relationship 
        [OneToMany]
        public List<MucTieu> MucTieuList { get; set; }
        public NguoiDung()
        {

        }
        public NguoiDung(string tenDangNhap, string matKhau, string ngaySinh, string gioiTinh)
        {
            TenDangNhap = tenDangNhap;
            MatKhau = matKhau;
            NgaySinh = ngaySinh;
            GioiTinh = gioiTinh;
        }
    }
}
