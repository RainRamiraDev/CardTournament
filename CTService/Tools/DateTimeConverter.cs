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
            // Verifica si la fecha es nula o está fuera del rango
            if (utcDateTime == null || utcDateTime == DateTime.MinValue)
                return DateTime.MinValue; // O un valor predeterminado que prefieras

            try
            {
                var zone = DateTimeZoneProviders.Tzdb[timeZoneId];
                var instant = Instant.FromDateTimeUtc(DateTime.SpecifyKind(utcDateTime.Value, DateTimeKind.Utc));
                var zonedDateTime = instant.InZone(zone);
                return zonedDateTime.ToDateTimeUnspecified();
            }
            catch (Exception ex)
            {
                // Loguea el error o maneja el caso según sea necesario
                Console.WriteLine($"Error al convertir la zona horaria {timeZoneId}: {ex.Message}");
                return DateTime.MinValue; // O un valor predeterminado en caso de error
            }
        }


        public static DateTime ConvertToUtc(DateTime? localDateTime, string timeZoneId)
        {
            if (localDateTime == null)
            {
                Console.WriteLine("Fecha local proporcionada es null.");
                return DateTime.MinValue; // Valor estándar cuando es null
            }

            try
            {
                // Imprimir la fecha local recibida
                Console.WriteLine($"Fecha local recibida: {localDateTime.Value}");

                var zone = DateTimeZoneProviders.Tzdb[timeZoneId];
                var localDate = LocalDateTime.FromDateTime(localDateTime.Value);
                Console.WriteLine($"LocalDateTime para zona {timeZoneId}: {localDate}");

                var zonedDateTime = localDate.InZoneStrictly(zone);
                Console.WriteLine($"Fecha convertida a UTC: {zonedDateTime.ToInstant()}");

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
