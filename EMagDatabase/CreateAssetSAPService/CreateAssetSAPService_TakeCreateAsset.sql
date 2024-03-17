CREATE PROCEDURE [dbo].[CreateAssetSAPService_TakeCreateAsset]
	AS
	SELECT caSAP.*
from [CreateAssetSAP] caSAP
Where caSAP.IsDeleted = 0 AND caSAP.NotSync = 1 AND caSAP.SyncErrorCount < 3 AND caSAP.IsTesting = 0
