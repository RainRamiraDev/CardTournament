using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTService.Tools
{
    public class DateTimeConverter
    {
        public static DateTime ConvertToTimeZone(DateTime? utcDateTime, string timeZoneId)
        {
            if (utcDateTime == null || utcDateTime == DateTime.MinValue)
                return DateTime.MinValue; 

            try
            {
                var zone = DateTimeZoneProviders.Tzdb[timeZoneId];
                var instant = Instant.FromDateTimeUtc(DateTime.SpecifyKind(utcDateTime.Value, DateTimeKind.Utc));
                var zonedDateTime = instant.InZone(zone);
                return zonedDateTime.ToDateTimeUnspecified();
            }
            catch (Exception ex)
            {
                return DateTime.MinValue; 
            }
        }


        public static DateTime ConvertToUtc(DateTime? localDateTime, string timeZoneId)
        {
            if (localDateTime == null)
            {
                Console.WriteLine("Fecha local proporcionada es null.");
                return DateTime.MinValue; 
            }

            try
            {

                var zone = DateTimeZoneProviders.Tzdb[timeZoneId];
                var localDate = LocalDateTime.FromDateTime(localDateTime.Value);

                var zonedDateTime = localDate.InZoneStrictly(zone);

                return zonedDateTime.ToInstant().ToDateTimeUtc();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al convertir la fecha a UTC desde la zona {timeZoneId}: {ex.Message}");
                throw new InvalidOperationException($"Error al convertir la fecha a UTC desde la zona {timeZoneId}. Detalles: {ex.Message}");
            }
        }
    }

}
