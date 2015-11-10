using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GiamCan.Model
{
    public class ThongKeBaiTap
    {
        [PrimaryKey, AutoIncrement]
        public int IdThongKeBaiTap { get; set; }
        public int IdThongKeNgay { get; set; }
        public int IdBaiTap { get; set; }
        public double QuangDuong { get; set; }
        public double ThoiGianTap { get; set; }
        public double LuongKaloTieuHao { get; set; }
    }
}
