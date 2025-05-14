namespace ECommerce.Helper
{
    public class DeliveryCostCalculator
    {
        public static double CalculateDistanceInKm(decimal lat1, decimal lon1, decimal lat2, decimal lon2)
        {
            const double R = 6371; // Earth radius in km
            var dLat = DegreesToRadians((double)(lat2 - lat1));
            var dLon = DegreesToRadians((double)(lon2 - lon1));
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(DegreesToRadians((double)lat1)) * Math.Cos(DegreesToRadians((double)lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }

        private static double DegreesToRadians(double degrees)
        {
            return degrees * Math.PI / 180;
        }

        public static decimal CalculateCost(double distanceInKm)
        {
            if (distanceInKm <= 5)
                return 25m;
            else if (distanceInKm <= 10)
                return 37.5m;
            else if (distanceInKm <= 20)
                return 55m;
            else if (distanceInKm <= 30)
                return 77.5m;
            else if (distanceInKm <= 50)
                return 115m;
            else
                return 140m + (decimal)((distanceInKm - 50) * 3);
        }
    }
}
