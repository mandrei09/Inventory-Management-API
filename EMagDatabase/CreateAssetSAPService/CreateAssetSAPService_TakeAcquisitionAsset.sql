CREATE PROCEDURE [dbo].[CreateAssetSAPService_TakeAcquisitionAsset]
AS
SELECT [com].[Id], [com].[ASSET],[com].[ASVAL_DATE], 
[com].[AccMonthId], [com].[AssetId], [com].[BudgetManagerId], 
[com].[COMPANYCODE], [com].[CURRENCY], 
[com].[CreatedAt], [com].[CreatedBy], 
[com].[DOC_DATE], [com].[EXCH_RATE], 
[com].[ErrorId], [com].[GL_ACCOUNT], 
[com].[Guid], [com].[HEADER_TXT], 
[com].[ITEM_TEXT], [com].[IsDeleted], 
[com].[IsTesting], [com].[ModifiedAt], 
[com].[ModifiedBy], [com].[NET_AMOUNT], 
[com].[NotSync], [com].[PSTNG_DATE], 
[com].[REF_DOC_NO], [com].[STORNO], 
[com].[SUBNUMBER], [com].[SyncErrorCount], 
[com].[TAX_AMOUNT], [com].[TAX_CODE], 
[com].[TOTAL_AMOUNT], [com].[VENDOR_NO], 
[com].[WBS_ELEMENT]
      FROM [AcquisitionAssetSAP] AS [com]
      WHERE ((([com].[IsDeleted] = 0) AND ([com].[NotSync] = 1)) AND ([com].[IsTesting] = 0)) AND ([com].[SyncErrorCount] < 3)