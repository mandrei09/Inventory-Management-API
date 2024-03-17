select * from RequestBudgetForecastMaterial where RequestBudgetForecastId = 781

update RequestBudgetForecastMaterial set Quantity = 1, QuantityRem = 1 where RequestBudgetForecastId = 781

select * from RequestBudgetForecastMaterial where OfferMaterialId in (1258, 1259, 1260)

select * from RequestBudgetForecast where RequestId = 856

select * from OfferMaterial where OfferId in (774,775,776) and IsDeleted = 0 

select * from RequestBFMaterialCostCenter where OfferMaterialId in (1258, 1259, 1260)

select * from Offer where Code in ('OFFER0000270','OFFER0000271','OFFER0000272')
select * from OfferType where Id = 3


select * from OfferMaterial where Guid = 'd5ec5aeb-ed9a-46ab-bb11-79506ce4031f'

select * from Offer where Id = 775
select * from OfferMaterial where MaterialId = 931

update OfferMaterial set Guid = (select guid from Offer where id = 775),Validated = 1 where Id = 1259

select * from [Order] where Code = 'ORD0000769'

update [Order] set AppStateId = 86 where Code = 'ORD0000769'

select * from EmailOrderStatus where OrderId = 929

select * from AppState