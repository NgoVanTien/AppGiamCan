using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GiamCan.Model
{
    class ThucDon
    {
        [PrimaryKey, AutoIncrement]
        public int IdThucDon { get; set; }
        public int IdThongKeNgay { get; set; }
        public int IdMonAn { get; set; }
        public double SoLuong { get; set; }
        public double LuongKalo  { get; set; }
        
    }
}
