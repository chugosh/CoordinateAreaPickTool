using System;
using System.Collections.Generic;
using System.Text;

namespace CoordManagerTool
{
    /// <summary>
    /// 用于经纬度纠偏的静态类
    /// </summary>
    public static class CoordManager
    {
        private static double x_PI = 3.14159265358979324 * 3000.0 / 180.0;
        private static double PI = 3.1415926535897932384626;
        private static double a = 6378245.0;
        private static double ee = 0.00669342162296594323;

        /// <summary>
        /// 百度转国测局
        /// </summary>
        /// <param name="bd_lon"></param>
        /// <param name="bd_lat"></param>
        /// <returns></returns>
        public static double[] BD09ToGCJ02(double bd_lon, double bd_lat)
        {
            double x = bd_lon - 0.0065;
            double y = bd_lat - 0.006;
            double z = Math.Sqrt(x * x + y * y) - 0.00002 * Math.Sin(y * x_PI);
            double theta = Math.Atan2(y, x) - 0.000003 * Math.Cos(x * x_PI);
            double[] arr = new double[2];
            arr[0] = z * Math.Cos(theta);
            arr[1] = z * Math.Sin(theta);
            return arr;
        }
        /// <summary>
        /// 国测局转WGS84
        /// </summary>
        /// <param name="gcj_lon">经度</param>
        /// <param name="gcj_lat">纬度</param>
        /// <returns></returns>
        public static double[] GCJ02ToWGS84(double gcj_lon, double gcj_lat)
        {
            double[] d = new double[2];
            if (outOfChina(gcj_lon, gcj_lat))
            {
                return new double[] { gcj_lon, gcj_lat };
            }
            double dlat = transformlat(gcj_lon - 105.0, gcj_lat - 35.0);
            double dlng = transformlng(gcj_lon - 105.0, gcj_lat - 35.0);
            double radlat = gcj_lat / 180.0 * PI;
            double magic = Math.Sin(radlat);
            magic = 1 - ee * magic * magic;
            double sqrtmagic = Math.Sqrt(magic);
            dlat = (dlat * 180.0) / ((a * (1 - ee)) / (magic * sqrtmagic) * PI);
            dlng = (dlng * 180.0) / (a / sqrtmagic * Math.Cos(radlat) * PI);
            double mglat = gcj_lat + dlat;
            double mglng = gcj_lon + dlng;
            d[0] = gcj_lon * 2 - mglng;
            d[1] = gcj_lat * 2 - mglat;
            return d;
        }
        /// <summary>
        /// 国测局to百度
        /// </summary>
        /// <param name="lng"></param>
        /// <param name="lat"></param>
        /// <returns></returns>
        public static double[] GCJ02toBD09(double lng, double lat)
        {
            var z = Math.Sqrt(lng * lng + lat * lat) + 0.00002 * Math.Sin(lat * x_PI);
            var theta = Math.Atan2(lat, lng) + 0.000003 * Math.Cos(lng * x_PI);
            var bd_lng = z * Math.Cos(theta) + 0.0065;
            var bd_lat = z * Math.Sin(theta) + 0.006;
            double[] arr = new double[2];
            arr[0] = bd_lng;
            arr[1] = bd_lat;
            return arr;
        }
        /// <summary>
        /// WGS84转国测局
        /// </summary>
        /// <param name="lng"></param>
        /// <param name="lat"></param>
        /// <returns></returns>
        public static double[] WGS84toGCJ02(double lng, double lat)
        {
            double[] ar = new double[2];
            if (outOfChina(lng, lat))
            {

                ar[0] = lng;
                ar[1] = lat;
                return ar;
            }
            else
            {
                var dlat = transformlat(lng - 105.0, lat - 35.0);
                var dlng = transformlng(lng - 105.0, lat - 35.0);
                var radlat = lat / 180.0 * PI;
                var magic = Math.Sin(radlat);
                magic = 1 - ee * magic * magic;
                var sqrtmagic = Math.Sqrt(magic);
                dlat = (dlat * 180.0) / ((a * (1 - ee)) / (magic * sqrtmagic) * PI);
                dlng = (dlng * 180.0) / (a / sqrtmagic * Math.Cos(radlat) * PI);
                var mglat = lat + dlat;
                var mglng = lng + dlng;
                ar[0] = mglng;
                ar[1] = mglat;
                return ar;
            }
        }

        public static double transformlat(double lng, double lat)
        {
            double ret = -100.0 + 2.0 * lng + 3.0 * lat + 0.2 * lat * lat + 0.1 * lng * lat + 0.2 * Math.Sqrt(Math.Abs(lng));
            ret += (20.0 * Math.Sin(6.0 * lng * PI) + 20.0 * Math.Sin(2.0 * lng * PI)) * 2.0 / 3.0;
            ret += (20.0 * Math.Sin(lat * PI) + 40.0 * Math.Sin(lat / 3.0 * PI)) * 2.0 / 3.0;
            ret += (160.0 * Math.Sin(lat / 12.0 * PI) + 320 * Math.Sin(lat * PI / 30.0)) * 2.0 / 3.0;
            return ret;
        }
        public static double transformlng(double lng, double lat)
        {
            double ret = 300.0 + lng + 2.0 * lat + 0.1 * lng * lng + 0.1 * lng * lat + 0.1 * Math.Sqrt(Math.Abs(lng));
            ret += (20.0 * Math.Sin(6.0 * lng * PI) + 20.0 * Math.Sin(2.0 * lng * PI)) * 2.0 / 3.0;
            ret += (20.0 * Math.Sin(lng * PI) + 40.0 * Math.Sin(lng / 3.0 * PI)) * 2.0 / 3.0;
            ret += (150.0 * Math.Sin(lng / 12.0 * PI) + 300.0 * Math.Sin(lng / 30.0 * PI)) * 2.0 / 3.0;
            return ret;
        }
        //判断坐标是否在中国
        public static bool outOfChina(double lng, double lat)
        {
            return (lng < 72.004 || lng > 137.8347) || ((lat < 0.8293 || lat > 55.8271) || false);
        }
    }
}
