using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GiamCan.Model
{
    public class MonAn
    {
        [PrimaryKey, AutoIncrement]
        public int IdMonAn { get; set; }
        public string TenMonAn { get; set; }
        public double LuongKaloTrenDonVi { get; set; }
        public string DonViTinh { get; set; }
    }
}
