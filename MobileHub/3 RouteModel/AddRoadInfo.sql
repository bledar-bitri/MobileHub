-- =============================================
ALTER PROCEDURE [dbo].[AddRoadInfo]
	-- Add the parameters for the stored procedure here
	@fromLat int
	,@fromLon int
	,@toLat int
	,@toLon int
	,@distance int
	,@timeInSeconds int
AS
BEGIN
	SET NOCOUNT ON;
	declare @tmp table 
	(
		FromLatitude int,
		FromLongitude int,
		ToLatitude int,
		ToLongitude int,
		Distance int,
		TimeInSeconds int
	)
	
	insert into @tmp values (@fromLat, @fromLon, @toLat, @toLon, @distance, @timeInSeconds);

    MERGE RoadInfo AS T
	USING @tmp AS S
	ON ((T.FromLatitude = S.FromLatitude AND T.FromLongitude = S.FromLongitude  AND T.ToLatitude = S.ToLatitude  AND T.ToLongitude = S.ToLongitude) OR
		(T.FromLatitude = S.ToLatitude AND T.FromLongitude = S.ToLongitude  AND T.ToLatitude = S.FromLatitude  AND T.ToLongitude = S.FromLongitude) )
	WHEN NOT MATCHED BY TARGET  
		THEN INSERT(FromLatitude, FromLongitude, ToLatitude, ToLongitude, Distance, TimeInSeconds, LookupDate) VALUES(S.FromLatitude, S.FromLongitude, S.ToLatitude, S.ToLongitude, S.Distance, S.TimeInSeconds, GetUtcDate())
	WHEN MATCHED 
		THEN UPDATE SET T.Distance = S.Distance, T.TimeInSeconds = S.TimeInSeconds, T.LookupDate = GetUtcDate();

END
