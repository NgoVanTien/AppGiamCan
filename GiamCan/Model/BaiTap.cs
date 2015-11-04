using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GiamCan.Model
{
    public class BaiTap
    {
        [PrimaryKey, AutoIncrement]
        public int IdBaiTap { get; set; }
        public string TenBaiTap { get; set; }
        public string DonViTinh { get; set; }
        public double LuongKaloTrenDVT { get; set; }
        public string HuongDan { get; set; }
    }
}
