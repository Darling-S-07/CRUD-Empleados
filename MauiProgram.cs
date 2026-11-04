using CRUD_Empleados.Data;
using Microsoft.Extensions.Logging;

namespace CRUD_Empleados
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // Base de datos en la carpeta de datos privada de la app.
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "empleados.db3");
            builder.Services.AddSingleton(new SQLiteDatabase(dbPath));

            // La página recibe SQLiteDatabase por constructor (inyección de dependencias).
            builder.Services.AddTransient<MainPage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}