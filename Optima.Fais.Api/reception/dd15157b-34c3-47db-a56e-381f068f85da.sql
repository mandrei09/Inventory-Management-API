select a.assetid, ia.IsTemp,a.IsTemp,ia.IsPlus,a.IsPlus from InventoryAsset IA
inner join Inventory I on I.InventoryId = ia.InventoryId
inner join Asset a on a.AssetId = ia.AssetId 
where i.Active = 1 and a.InvNo = '821331000241' and a.IsDeleted = 0 and a.Validated = 1 and a.InInventory = 1 and a.OutDate is null

update inventoryasset set IsPlus = 1 where Assetid = 417556 and InventoryId = 16

select * from Inventory

exec UpdateAllBudgetBase

select * from BudgetForecast where IsLast = 1 

select ReqBFMCostCenterId from Asset where InvNo in ('RO10OPT00000000004963',
'RO10OPT00000000004920')

select * from RequestBFMaterialCostCenter where Id in (1247,
1258)

select * from RequestBudgetForecastMaterial where id in (854,
877)

select * from RequestBudgetForecast where id in (747,
602)

update RequestBudgetForecast set IsDeleted = 1 where id in (747,
602)

select * from BudgetForecast where id in (33569)

select * from RequestBudgetForecast where BudgetForecastid = 33569 and IsDeleted = 0

select * from RequestBudgetForecastMaterial where RequestBudgetForecastId in (select id from RequestBudgetForecast where BudgetForecastid = 33569 and IsDeleted = 0)

select * from [Order] where Id = 888

update RequestBudgetForecast set IsDeleted = 1 where id = 684
         



