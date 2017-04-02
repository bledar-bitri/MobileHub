using RouteModel;

namespace Contracts
{
    public class RoadInfoContract
    {
        public long FromLatitude { get; set; }
        public long FromLongitude { get; set; }
        public long ToLatitude { get; set; }
        public long ToLongitude { get; set; }
        public double Distance { get; set; }
        public long TimeInSeconds { get; set; }
        public string FromAddress { get; set; }
        public string ToAddress { get; set; }

        public int FromAddressId { get; set; }
        public int ToAddressId { get; set; }

        public RoadInfoContract() { }

        public RoadInfoContract(int fromAddressId, int toAddressId, RoadInfo roadInfo)
        {
            FromAddressId = fromAddressId;
            ToAddressId = toAddressId;
            Assign(roadInfo);

        }

        public void Assign(RoadInfo roadInfo)
        {
            FromLatitude = roadInfo.FromLatitude;
            FromLongitude = roadInfo.FromLongitude;
            ToLatitude = roadInfo.ToLatitude;
            ToLongitude = roadInfo.ToLongitude;
            Distance = roadInfo.Distance;
            TimeInSeconds = roadInfo.TimeInSeconds;
            FromAddress = roadInfo.FromAddress;
            ToAddress = roadInfo.ToAddress;
        }
    }
}
