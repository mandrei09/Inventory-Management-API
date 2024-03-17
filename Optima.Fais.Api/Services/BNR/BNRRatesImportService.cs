using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Optima.Fais.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Optima.Fais.Api.Services.BNR
{
	public class BNRRatesImportService : IBNRRatesImportService
    {
        private const string BaseUrl = @"http://www.bnr.ro/nbrfxrates.xml";
		private const string BaseUrl2005 = @"https://www.bnr.ro/files/xml/years/nbrfxrates2005.xml";
		private const string BaseUrl2006 = @"https://www.bnr.ro/files/xml/years/nbrfxrates2006.xml";
		private const string BaseUrl2007 = @"https://www.bnr.ro/files/xml/years/nbrfxrates2007.xml";
		private const string BaseUrl2008 = @"https://www.bnr.ro/files/xml/years/nbrfxrates2008.xml";
		private const string BaseUrl2009 = @"https://www.bnr.ro/files/xml/years/nbrfxrates2009.xml";
		private const string BaseUrl2010 = @"https://www.bnr.ro/files/xml/years/nbrfxrates2010.xml";
		private const string BaseUrl2011 = @"https://www.bnr.ro/files/xml/years/nbrfxrates2011.xml";
		private const string BaseUrl2012 = @"https://www.bnr.ro/files/xml/years/nbrfxrates2012.xml";
		private const string BaseUrl2013 = @"https://www.bnr.ro/files/xml/years/nbrfxrates2013.xml";
		private const string BaseUrl2014 = @"https://www.bnr.ro/files/xml/years/nbrfxrates2014.xml";
		private const string BaseUrl2015 = @"https://www.bnr.ro/files/xml/years/nbrfxrates2015.xml";
		private const string BaseUrl2016 = @"https://www.bnr.ro/files/xml/years/nbrfxrates2016.xml";
		private const string BaseUrl2017 = @"https://www.bnr.ro/files/xml/years/nbrfxrates2017.xml";
		private const string BaseUrl2018 = @"https://www.bnr.ro/files/xml/years/nbrfxrates2018.xml";
		private const string BaseUrl2019 = @"https://www.bnr.ro/files/xml/years/nbrfxrates2019.xml";
		private const string BaseUrl2020 = @"https://www.bnr.ro/files/xml/years/nbrfxrates2020.xml";
		private const string BaseUrl2021 = @"https://www.bnr.ro/files/xml/years/nbrfxrates2021.xml";
		private const string BaseUrl2022 = @"https://www.bnr.ro/files/xml/years/nbrfxrates2022.xml";
        private const string BaseUrl2023 = @"https://www.bnr.ro/files/xml/years/nbrfxrates2023.xml";
        private const string BaseUrl2024 = @"https://www.bnr.ro/files/xml/years/nbrfxrates2024.xml";
        public BNRRatesImportService(IServiceProvider services)
        {
            Services = services;
        }

        public IServiceProvider Services { get; }

        public async Task<int> BNRRatesImportAsync()
        {
			Model.AccMonth accMonth = null;
			int countChanges = 0;

            using (var scope = Services.CreateScope())
            {
                var dbContext =
                   scope.ServiceProvider
                       .GetRequiredService<ApplicationDbContext>();

				Model.SyncStatus syncStatus = await dbContext.Set<Model.SyncStatus>().Where(s => s.IsDeleted == false && s.Code == "RATE").SingleOrDefaultAsync();

				if(syncStatus == null)
				{
					syncStatus = new Model.SyncStatus()
					{
						Code = "RATE",
						Name = "Sincronizare curs BNR",
						SyncEnabled = true,
						SyncInterval = 1440
					};

					dbContext.Add(syncStatus);
					dbContext.SaveChanges();
				}

				if (syncStatus.SyncEnabled)
				{
					var lastSync = syncStatus.SyncLast;

					//if (1 == 1)
				    if (((lastSync == null) || (lastSync.Value.AddMinutes(syncStatus.SyncInterval) < DateTime.Now)))
					{
						List<Model.Rate> rateOlds = dbContext.Set<Model.Rate>().Where(com => com.IsDeleted == false && com.IsLast == true).ToList();

						for (int i = 0; i < rateOlds.Count; i++)
						{
							rateOlds[i].IsLast = false;
							dbContext.Update(rateOlds[i]);
							dbContext.SaveChanges();
						}

						syncStatus.SyncStart = DateTime.Now;
						accMonth = dbContext.Set<Model.AccMonth>().Where(a => a.Year == DateTime.Now.Year && a.IsDeleted == false).FirstOrDefault();

						for (int i = 0; i < 3; i++)
						{
							if (i == 0)
							{
								var rates = GetBNRRates();

								Model.Rate rate = null;
								Model.Rate rateRon = null;
								Model.Uom uom = null;
								Model.Uom uomRon = null;
								
								List<Model.ContractAmount> contractAmount = null;
								List<Model.ContractAmount> contractAmountRon = null;

								if (rates.Count > 0)
								{
								
									for (int c = 0; c < rates.Count; c++)
									{
										if (rates[c].Moneda != null)
										{
											uom = dbContext.Set<Model.Uom>().Where(a => a.Code == rates[c].Moneda && a.IsDeleted == false).FirstOrDefault();

											if (uom == null)
											{
												uom = new Model.Uom()
												{
													Code = rates[c].Moneda,
													Name = rates[c].Moneda,
													IsDeleted = false
												};

												dbContext.Add(uom);
											}
											else
											{
												uom.IsDeleted = false;
												dbContext.Update(uom);
											}
										}


										if (rates[c].DataCurenta != null)
										{

											if (accMonth != null)
											{

												//rate = dbContext.Set<Model.Rate>().Where(com => com.Code == rates[c].DataCurenta && com.AccMonthId == accMonth.Id && com.UomId == uom.Id && com.IsDeleted == false).FirstOrDefault();
												rate = dbContext.Set<Model.Rate>().Where(com => com.Code == rates[c].DataCurenta && com.UomId == uom.Id && com.IsDeleted == false).FirstOrDefault();

												if (rate == null)
												{
													rate = new Model.Rate()
													{
														Code = rates[c].DataCurenta,
														Name = rates[c].DataCurenta,
														AccMonth = accMonth,
														Uom = uom,
														Value = decimal.Parse(rates[c].Valoarea),
														CreatedAt = DateTime.Now,
														ModifiedAt = DateTime.Now,
														Multiplier = rates[c].Multiplier,
														IsLast = true
													};

													//rateOld.IsLast = false;
													dbContext.Add(rate);
													// dbContext.Update(rateOld);
												}
												else
												{
													rate.Value = decimal.Parse(rates[c].Valoarea);
													rate.ModifiedAt = DateTime.Now;
													rate.Multiplier = rates[c].Multiplier;
													rate.AccMonthId = accMonth.Id;
													rate.IsLast = true;
													dbContext.Update(rate);
												}


												contractAmount = dbContext.Set<Model.ContractAmount>().Include(u => u.Uom).Where(a => a.UomId == uom.Id && a.IsDeleted == false).ToList();

												for (int d = 0; d < contractAmount.Count; d++)
												{
													if (contractAmount[d].RateRonId == null)
													{
														uomRon = dbContext.Set<Model.Uom>().Where(a => a.Code == "RON").FirstOrDefault();
														rateRon = dbContext.Set<Model.Rate>().Include(u => u.Uom).Where(com => com.Code == rates[c].DataCurenta && com.AccMonthId == accMonth.Id && com.UomId == uomRon.Id).FirstOrDefault();

														contractAmount[d].RateRon = rateRon;
													}
													else
													{
														uomRon = dbContext.Set<Model.Uom>().Where(a => a.Code == "RON").FirstOrDefault();
														rateRon = dbContext.Set<Model.Rate>().Include(u => u.Uom).Where(com => com.Code == rates[c].DataCurenta && com.AccMonthId == accMonth.Id && com.UomId == uomRon.Id).FirstOrDefault();

														contractAmount[d].RateRon = rateRon;

													}

													contractAmount[d].Rate = rate;

													dbContext.Update(contractAmount[d]);
													dbContext.SaveChanges();
												}


												if (c + 1 == rates.Count)
												{
													uomRon = dbContext.Set<Model.Uom>().Where(a => a.Code == "RON").FirstOrDefault();

													rateRon = dbContext.Set<Model.Rate>().Include(u => u.Uom).Where(com => com.Code == rates[c].DataCurenta && com.AccMonthId == accMonth.Id && com.UomId == uomRon.Id).FirstOrDefault();


													if (rateRon == null)
													{
														rateRon = new Model.Rate()
														{
															Code = rates[c].DataCurenta,
															Name = rates[c].DataCurenta,
															AccMonth = accMonth,
															Uom = uomRon,
															Value = 1,
															CreatedAt = DateTime.Now,
															ModifiedAt = DateTime.Now,
															Multiplier = 1,
															IsLast = true
														};

														dbContext.Add(rateRon);
													}

													contractAmountRon = dbContext.Set<Model.ContractAmount>().Where(a => a.UomId == uomRon.Id && a.IsDeleted == false).ToList();

													for (int b = 0; b < contractAmountRon.Count; b++)
													{
														contractAmountRon[b].Rate = rateRon;
														contractAmountRon[b].RateRon = rateRon;
														contractAmountRon[b].AmountRonRem = contractAmountRon[b].AmountRem * rate.Value;
														dbContext.Update(contractAmountRon[b]);
														dbContext.SaveChanges();
													}
												}
											}
										}

										dbContext.SaveChanges();

										countChanges++;
									}
								}
							}
						}

						var count = dbContext.Set<Model.RecordCount>().FromSql("UpdateAllContractAmount").ToList();
						
						syncStatus.SyncEnd = DateTime.Now;
						syncStatus.SyncLast = DateTime.Now;
						dbContext.Update(syncStatus);
						dbContext.SaveChanges();

					}
				}
			}

			

			return countChanges;
        }

		public List<ExchangeRate> GetBNRRates()
		{
			CurrenciesDataSet dataset = null;

			List<ExchangeRate> rates = new();
			XDocument doc = XDocument.Load(BaseUrl2024);

			using (TextReader sr = new StringReader(doc.ToString(SaveOptions.DisableFormatting)))
			{
				var serializer = new XmlSerializer(typeof(CurrenciesDataSet));
				dataset = (CurrenciesDataSet)serializer.Deserialize(sr);
			}


			//List<Cube> cubes = dataset.Body.Cubes.ToList();

			//for (int i = 0; i < cubes.Count; i++)
			//{

			//	var items = cubes[i].Rates.Select(x => new ExchangeRate
			//	{
			//		DataCurenta = cubes[i].Date,
			//		Moneda = x.Currency,
			//		Valoarea = x.Value.ToString(),
			//		Multiplier = x.Multiplier > 0 ? x.Multiplier : 1

			//	}).ToList();

			//	for (int r = 0; r < items.Count; r++)
			//	{
			//		rates.Add(items[r]);
			//	}
			//}

			Cube cube = dataset.Body.Cubes.FirstOrDefault();

			if (cube != null)
			{
				rates = cube.Rates.Select(x => new ExchangeRate
				{
					DataCurenta = cube.Date,
					Moneda = x.Currency,
					Valoarea = x.Value.ToString(),
					Multiplier = x.Multiplier > 0 ? x.Multiplier : 1

				}).ToList();
			}

			return rates;

		}
	}
}

//title, owner, contractId, contractStatus, supplier, effectiveDate, expirationDate, creationDate, contractAmount, version, templateId, 
//    businessSystemId, amendmentType, amendmentReason, origin, hierarchicalType, parentAgreement, expirationTermType, commodity, region


[Serializable]
[XmlRoot("DataSet", Namespace = "http://www.bnr.ro/xsd", IsNullable = false)]
public class CurrenciesDataSet
{
    public Header Header { get; set; }
    public Body Body { get; set; }
}

[Serializable]
public class Header
{
    public string Publisher { get; set; }

    [XmlElement(DataType = "date")]
    public DateTime PublishingDate { get; set; }

    public string MessageType { get; set; }
}


public class Cube
{
    [XmlElement("Rate")]
    public List<Rate> Rates { get; set; }


    [XmlAttribute("date")]
    public string Date { get; set; }
}

[Serializable]
public class Rate
{
    [XmlAttribute("currency")]
    public string Currency { get; set; }

    [XmlAttribute("multiplier")]
    public int Multiplier { get; set; }

    [XmlText]
    public decimal Value { get; set; }
}


[Serializable]
public class Body
{
    public string Subject { get; set; }

    public string Description { get; set; }

    public string OrigCurrency { get; set; }

    [XmlElement("Cube")]
    public List<Cube> Cubes { get; set; }
}

[Serializable]
public class ExchangeRate
{
    public string DataCurenta { get; set; }

    public string Moneda { get; set; }

    public string Valoarea { get; set; }

	public int Multiplier { get; set; }
}


