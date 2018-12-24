using Microsoft.ML.Runtime.Api;

namespace TaxiFarePrediction
{
    public class TaxiTrip
    {
        [Column("0")]
        public string VendorId;
        [Column("1")]
        public float RateCode;
        [Column("2")]
        public float PassengerCount;
        [Column("3")]
        public float TripTimeInSecs;
        [Column("4")]
        public float TripDistance;
        [Column("5")]
        public string PaymentType;
        [Column("6")]
        public float FareAmount;

    }
    public class TaxiTripFarePrediction
    {
        [ColumnName("Score")]
        public float FareAmount;
    }
}