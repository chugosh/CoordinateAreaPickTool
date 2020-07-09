using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace CoordManagerTool
{
    /// <summary>
    /// xml是GCJ02格式
    /// </summary>
    public class Program
    {
        private static CoordinateModel _coordInate = new CoordinateModel();
        private static Country _country = new Country();
        //private static List<Point> _points = new List<Point>();
        public static void Main(string[] args)
        {

            //读json文件 获取所有区域
            //var fileStr = File.ReadAllText()
            //LoadJsonData();
            LoadXmlData();
            while (true)
            {
                var coordinate = Console.ReadLine();
                var coordinates = coordinate.Split(",");
                var point84 = CoordManager.WGS84toGCJ02(double.Parse(coordinates[0]), double.Parse(coordinates[1]));
                var point = new Point()
                {
                    x = point84[0],
                    y = point84[1]
                };
                //json文件TMD少很多区划
                //foreach (var l in _pointsList)
                //{
                //    var pointArray = l.ToArray();
                //    var result = IsPointInPloy(point, pointArray, out var name);
                //    if (result && !name.Equals("100000-中华人民共和国"))
                //        Console.WriteLine($"点{point.x}, {point.y} 在{name}中");
                //}
                foreach (var l in _xmlsList)
                {
                    var pointArray = l.ToArray();
                    var result = IsPointInPloy(point, pointArray, out var name);
                    if (result)
                        Console.WriteLine($"点{point.x}, {point.y} 在{name}中");
                }
            }


            //Console.WriteLine($"ok");
            //Console.ReadLine();
        }

        private static void Getdatas(XMLModel province, out List<Point> pl, out string[] tempCoordinates)
        {
            tempCoordinates = province.rings.Split(",");
            pl = new List<Point>();
            for (var i = 0; i < tempCoordinates.Length; i++)
            {
                var coords = tempCoordinates[i].Split(" ");
                if (coords.Length < 2) continue;
                pl.Add(new Point()
                {
                    x = double.Parse(coords[0]),
                    y = double.Parse(coords[1]),
                    adcode = province.code.ToString(),
                    name = province.name,
                });
            }
        }

        private static List<List<Point>> _xmlsList = new List<List<Point>>();
        private static void LoadXmlData()
        {
            var serializer = new XmlSerializer(typeof(Country));
            using (StreamReader reader = new StreamReader("chinaBoudler.xml"))
            {
                _country = (Country)serializer.Deserialize(reader);
                foreach (var province in _country.province)
                {
                    foreach (var city in province.City)
                    {
                        foreach (var piecearea in city.Piecearea)
                        {
                            var coordinates = piecearea.rings.Split(",");
                            var pl = new List<Point>();
                            for (var i = 0; i < coordinates.Length; i++)
                            {
                                var coords = coordinates[i].Split(" ");
                                if (coords.Length < 2) continue;
                                pl.Add(new Point()
                                {
                                    x = double.Parse(coords[0]),
                                    y = double.Parse(coords[1]),
                                    adcode = province.code.ToString() + "-" + city.code.ToString() + "-" + piecearea.code.ToString(),
                                    name = province.name + "-" + city.name + "-" + piecearea.name,
                                });
                            }
                            _xmlsList.Add(pl);
                        }
                    }
                }
            }
        }


        /// <summary>
        /// 根据省份确定市 根据市确定区县 
        /// 这样比较次数会少于计算出全国所有区县再比较的次数
        /// </summary>
        /// <param name="point"></param>
        private static void LoadXmlDataTest(Point point)
        {
            var serializer = new XmlSerializer(typeof(Country));
            using (StreamReader reader = new StreamReader("chinaBoudler.xml"))
            {
                //xml的所有数据的对象
                _country = (Country)serializer.Deserialize(reader);
                Test(point, _country.province);
                //各个省份的范围
                foreach (var t in _country.province)
                {
                    var p = GetPoints(t.rings);
                    if(IsPointInPloy(point, p, out var name))
                    {

                    }
                }
            }
        }


        private static void Test(Point point, object o)
        {
            if(o is CountryProvinceCityPiecearea piecearea)
            {
                return;
            }

            foreach (var t in o)
            {
                var p = GetPoints(t.rings);
                if (IsPointInPloy(point, p, out var name))
                {
                    Test(point, o);
                }
            }
        }

        private static Point[] GetPoints(string rings)
        {
            var points = new List<Point>();
            var coordinates = rings.Split(",");
            for (var i = 0; i < coordinates.Length; i++)
            {
                var coords = coordinates[i].Split(" ");
                if (coords.Length < 2) continue;
                points.Add(new Point()
                {
                    x = double.Parse(coords[0]),
                    y = double.Parse(coords[1]),
                });
            }
            return points.ToArray();
        }

        private static List<List<Point>> _pointsList = new List<List<Point>>();
        private static void LoadJsonData()
        {
            using (StreamReader r = new StreamReader("District.json"))
            {
                string jsonStr = r.ReadToEnd();
                _coordInate = JsonConvert.DeserializeObject<CoordinateModel>(jsonStr);
                var result = _coordInate.RECORDS.ToList();
                foreach (var c in result)
                {
                    var coordinates = c.polyline.Split(";");
                    var pl = new List<Point>();
                    for (var i = 0; i < coordinates.Length; i++)
                    {
                        var coords = coordinates[i].Split(",");
                        if (coords.Length < 2) continue;
                        pl.Add(new Point() {
                            x = double.Parse(coords[0]),
                            y = double.Parse(coords[1]),
                            name = c.name,
                        });
                    }
                    _pointsList.Add(pl);
                }
            }

            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="point">输入点</param>
        /// <param name="Points">区域</param>
        /// <param name="name">区域名称</param>
        /// <returns></returns>
        private static bool IsPointInPloy(Point point, Point[] Points, out string name)
        {
            name = "";
            var ALon = point.x;
            var ALat = point.y;
            int iSum, iCount, iIndex;
            double dLon1 = 0, dLon2 = 0, dLat1 = 0, dLat2 = 0, dLon;
            if (Points.Length < 3)
            {
                return false;
            }
            iSum = 0;
            iCount = Points.Length;
            for (iIndex = 0; iIndex < iCount; iIndex++)
            {
                name = Points[iIndex].adcode + "-" + Points[iIndex].name;
                if (ALon == Points[iIndex].x && ALat == Points[iIndex].y)  //A点在多边形上 
                {
                    return true;
                }

                if (iIndex == iCount - 1)
                {
                    dLon1 = Points[iIndex].x;
                    dLat1 = Points[iIndex].y;
                    dLon2 = Points[0].x;
                    dLat2 = Points[0].y;
                }
                else
                {
                    dLon1 = Points[iIndex].x;
                    dLat1 = Points[iIndex].y;
                    dLon2 = Points[iIndex + 1].x;
                    dLat2 = Points[iIndex + 1].y;
                }

                //以下语句判断A点是否在边的两端点的纬度之间，在则可能有交点
                if (((ALat > dLat1) && (ALat < dLat2)) || ((ALat > dLat2) && (ALat < dLat1)))
                {
                    if (Math.Abs(dLat1 - dLat2) > 0)
                    {
                        //获取A点向左射线与边的交点的x坐标：
                        dLon = dLon1 - ((dLon1 - dLon2) * (dLat1 - ALat)) / (dLat1 - dLat2);
                        //如果交点在A点左侧，则射线与边的全部交点数加一：
                        if (dLon < ALon)
                        {
                            iSum++;
                        }
                        //如果相等，则说明A点在边上
                        if (dLon == ALon)
                            return true;
                    }
                }
            }
            if ((iSum % 2) != 0)
            {
                return true;
            }
            return false;
        }
    }

    public class Point
    {
        public double x { get; set; }
        public double y { get; set; }

        public string adcode { get; set; }     
        
        public string name { get; set; }
    }

}
