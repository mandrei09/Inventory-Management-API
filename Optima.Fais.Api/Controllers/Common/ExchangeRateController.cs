//using Microsoft.AspNetCore.Mvc;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Threading.Tasks;
//using System.Xml.Linq;
//using System.Xml.Serialization;

//namespace Optima.Fais.Api.Controllers.Common
//{
//    public class ExchangeRateController : Controller
//    {
//        public ActionResult ExchangeRate(List<ExchangeRate> data)
//        {
//            {
//				_ = new List<ExchangeRate>();

//                data = ExchangeRates();

//                return View(data);
//            }
//        }
//        public List<ExchangeRate> ExchangeRates()
//        {

//            CurrenciesDataSet dataset = null;

//            List<ExchangeRate> rates = new List<ExchangeRate>();
//            XDocument doc = XDocument.Load(@"http://www.bnr.ro/nbrfxrates.xml");

//            using (TextReader sr = new StringReader(doc.ToString(SaveOptions.DisableFormatting)))
//            {
//                var serializer = new XmlSerializer(typeof(CurrenciesDataSet));
//                dataset = (CurrenciesDataSet)serializer.Deserialize(sr);
//            }


//            Cube cube = dataset.Body.Cubes.FirstOrDefault();

//            if (cube != null)
//            {
//                rates = cube.Rates.Select(x => new ExchangeRate
//                {
//                    DataCurenta = cube.Date,
//                    Moneda = x.Currency,
//                    Valoarea = x.Multiplier.ToString(),
//                }).ToList();
//            }

//            return rates;
//        }
//    }
//}

////[Serializable]
////[XmlRoot("DataSet", Namespace = "http://www.bnr.ro/xsd", IsNullable = false)]
////public class CurrenciesDataSet
////{
////    public Header Header { get; set; }
////    public Body Body { get; set; }
////}

////[Serializable]
////public class Header
////{
////    public string Publisher { get; set; }

////    [XmlElement(DataType = "date")]
////    public DateTime PublishingDate { get; set; }

////    public string MessageType { get; set; }
////}


////public class Cube
////{
////    [XmlElement("Rate")]
////    public List<Rate> Rates { get; set; }


////    [XmlAttribute("date")]
////    public string Date { get; set; }
////}

////[Serializable]
////public class Rate
////{
////    [XmlAttribute("currency")]
////    public string Currency { get; set; }

////    [XmlAttribute("multiplier")]
////    public int Multiplier { get; set; }

////    [XmlText]
////    public decimal Value { get; set; }
////}


////[Serializable]
////public class Body
////{
////    public string Subject { get; set; }

////    public string Description { get; set; }

////    public string OrigCurrency { get; set; }

////    [XmlElement("Cube")]
////    public List<Cube> Cubes { get; set; }
////}

////[Serializable]
////public class ExchangeRate
////{
////    public string DataCurenta { get; set; }


////    public string Moneda { get; set; }


////    public string Valoarea { get; set; }
////}
