using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GiamCan.Model
{
    public class ThongKeNgay
    {
        [PrimaryKey, AutoIncrement]
        public int IdThongKeNgay { get; set; }
        public int IdMucTieu { get; set; }
        public string Ngay { get; set; }
        public double LuongKaloTieuHao { get; set; }
        public double LuongKaloDuaVao { get; set; }

        public double LuongKaloNgoaiDuKien { get; set; }

    }
}
