Create PROCEDURE [dbo].[CreateAssetSAPService_TakeChangeAsset]
AS
SELECT com.*
      FROM [AssetChangeSAP] AS [com]
      WHERE ((([com].[IsDeleted] = 0) AND ([com].[NotSync] = 1)) AND ([com].[SyncErrorCount] < 3)) AND ([com].[IsTesting] = 0)