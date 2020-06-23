using System;
using System.Collections.Generic;
using System.Text;

namespace CoordManagerTool
{

    // 注意: 生成的代码可能至少需要 .NET Framework 4.5 或 .NET Core/Standard 2.0。
    /// <remarks/>
    
    ///国家
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategory("code")]
    [System.Xml.Serialization.XmlType(AnonymousType = true)]
    [System.Xml.Serialization.XmlRoot(Namespace = "", IsNullable = false)]
    public partial class Country : XMLModel
    {

        /// <remarks/>
        [System.Xml.Serialization.XmlElement("province")]
        public List<CountryProvince> province { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttribute()]
        public byte ID { get; set; }
    }

    /// <remarks/>
    /// 省份
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategory("code")]
    [System.Xml.Serialization.XmlType(AnonymousType = true)]
    public partial class CountryProvince : XMLModel
    {

        /// <remarks/>
        [System.Xml.Serialization.XmlElement("City")]
        public List<CountryProvinceCity> City { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttribute()]
        public byte ID { get; set; }
    }

    /// <remarks/>

    /// 城市
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategory("code")]
    [System.Xml.Serialization.XmlType(AnonymousType = true)]
    public partial class CountryProvinceCity: XMLModel
    {

        /// <remarks/>
        [System.Xml.Serialization.XmlElement("Piecearea")]
        public List<CountryProvinceCityPiecearea> Piecearea { get; set; }
    }

    /// <remarks/>
    /// 区县?
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategory("code")]
    [System.Xml.Serialization.XmlType(AnonymousType = true)]
    public partial class CountryProvinceCityPiecearea: XMLModel
    {
    }

    public abstract class XMLModel
    {
        [System.Xml.Serialization.XmlAttribute()]
        public uint code { get; set; }
        [System.Xml.Serialization.XmlAttribute()]
        public string name { get; set; }
        [System.Xml.Serialization.XmlAttribute()]
        public string rings { get; set; }
    }
}
