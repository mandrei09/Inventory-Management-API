DECLARE @currentInvNo VARCHAR(20); -- Specify the maximum length for your VARCHAR
DECLARE @currentAssetId INT;
DECLARE @currentSubNo INT;
DECLARE CInvNo CURSOR FOR
	SELECT DISTINCT InvNo FROM asset
	WHERE InvNo IN (
	select a.InvNo from Asset a  where a.IsDeleted = 0 and SubNo=0 and Invno not like('WFH%') and (select count(*) from Asset where subno = a.SubNo and InvNo = a.InvNo and IsDeleted = 0) > 1 
)

-- Open the CInvNo cursor
OPEN CInvNo;
FETCH NEXT FROM CInvNo INTO @currentInvNo;

-- Outer cursor loop
WHILE @@FETCH_STATUS = 0
BEGIN
	SET @currentSubNo = 0;

	DECLARE CAssetId CURSOR FOR
		SELECT AssetId FROM asset WHERE InvNo = @currentInvNo;

	-- Open the CAssetId cursor
	OPEN CAssetId;
	FETCH NEXT FROM CAssetId INTO @currentAssetId;

	-- Inner cursor loop
	WHILE @@FETCH_STATUS = 0
	BEGIN
		-- Update the SubNo for the current asset
		UPDATE asset 
		SET SubNo = @currentSubNo
		WHERE AssetId = @currentAssetId;

		SET @currentSubNo += 1;

		FETCH NEXT FROM CAssetId INTO @currentAssetId;
	END;

	-- Close and deallocate the CAssetId cursor
	CLOSE CAssetId;
	DEALLOCATE CAssetId;

	FETCH NEXT FROM CInvNo INTO @currentInvNo;
END;

-- Close and deallocate the CInvNo cursor
CLOSE CInvNo;
DEALLOCATE CInvNo;
