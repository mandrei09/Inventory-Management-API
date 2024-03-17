select * from TransferInStockSAP
where IsDeleted = 0 and IsTesting = 0 
order by id desc

select * from createassetsap
where IsDeleted = 0 and IsTesting = 0 and FromStock = 1 
order by id desc

select * from AcquisitionAssetSAP
where IsDeleted = 0 and IsTesting = 0 and assetid = 417813 --and FromStock = 1  
order by id desc

select * from assetchangesap
where IsDeleted = 0 and IsTesting = 0 --and assetid = 417803
order by id desc

select assetid,projectid,orderid,stockid,* from asset 
where (projectid is not null and orderid is null) or (projectid is null and orderid is not null)
	and IsDeleted = 0 
order by CreatedAt desc

select * from AspNetUsers where id = 'f12c8c12-20b0-4acc-bb48-57832bbb9319'

select * from Error order by id desc

select top 10 *
from Error
where Code = 'RESULT-ACQUISITION-CREATE'
	and Name like '"{\"meta\":{\"code\":400}%'

	select top 10 *
from Error
where Code = 'RESULT-ACQUISITION-CREATE'
	and Name like '"{\"meta\":{\"code\":200}%'
order by Id desc

select distinct Code
from Error
where Name like '"{\"meta\":{\"code\":400}%'

select * from project where code in ('10IM_03_40_41_05', '10IM_03_40_40_05')

select * from Partner where FiscalCode = '0001006307'

select d.* from asset a
inner join document d on d.documentid = a.DocumentId
where a.invno = '212100000629'

select * from partner where partnerid = 2925

