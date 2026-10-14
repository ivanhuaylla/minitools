using System;
using System.Reflection;
using Autodesk.Revit.UI;

namespace Riga.LimpiarNulos
{
    public class App : IExternalApplication
    {
        const string TAB = "dsdsaaggr";

        public Result OnStartup(UIControlledApplication app)
        {
            try { app.CreateRibbonTab(TAB); } catch { }

            RibbonPanel panel = null;
            foreach (var p in app.GetRibbonPanels(TAB))
            {
                if (p.Name == "Parámetros") { panel = p; break; }
            }
            if (panel == null) panel = app.CreateRibbonPanel(TAB, "Parámetros");

            string dll = Assembly.GetExecutingAssembly().Location;

            var datos = new PushButtonData(
                "LimpiarNulos", "Limpiar\nnulos", dll, "Riga.LimpiarNulos.Comandos.CmdLimpiar")
            {
                ToolTip = "Limpia los parámetros booleanos de ejemplar que tengan valor nulo, asignándoles el valor 'No'.",
                LongDescription =
                    "Abre una ventana para seleccionar los parámetros booleanos. " +
                    "Convierte los valores nulos (vacíos) a 'No' en todos los elementos del proyecto."
            };
            var boton = panel.AddItem(datos) as PushButton;
            if (boton != null)
            {
                boton.AvailabilityClassName = typeof(Comun.ConDocumento).FullName;
                boton.LargeImage = Comun.Iconos.Escoba(32);
                boton.Image = Comun.Iconos.Escoba(16);
            }

            try
            {
                string dllInyector = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(dll), "InyectorDatos.dll");
                var datosInyector = new PushButtonData(
                    "InyectarDatos", "Inyectar\ndatos", dllInyector, "Riga.InyectorDatos.Comandos.DataInjectorCommand")
                {
                    ToolTip = "Inyecta datos desde un archivo CSV a parámetros de elementos en Revit.",
                    LongDescription =
                        "Abre una ventana para seleccionar un archivo CSV y mapear sus columnas a parámetros de Revit. " +
                        "Inyecta los datos de acuerdo a la clave primaria seleccionada."
                };

                var botonInyector = panel.AddItem(datosInyector) as PushButton;
                if (botonInyector != null)
                {
                    // Force load the assembly into memory for .NET 8 ALC resolution
                    System.Reflection.Assembly.LoadFrom(dllInyector);
                    botonInyector.LargeImage = new System.Windows.Media.Imaging.BitmapImage(new Uri("pack://application:,,,/InyectorDatos;component/Resources/icon_injector.jpg", UriKind.Absolute));
                }

                string dllConcatenador = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(dll), "ConcatenadorParametros.dll");
                var datosConcatenador = new PushButtonData(
                    "ConcatenadorParametros", "Concatenador\nde Parámetros", dllConcatenador, "Riga.ConcatenadorParametros.Comandos.CmdConcatenadorParametros")
                {
                    ToolTip = "Concatena múltiples parámetros en un parámetro de destino.",
                    LongDescription = "Permite seleccionar elementos o filtrar por categoría para unir los valores de varios parámetros en un único parámetro destino, usando un separador personalizado."
                };

                var botonConcatenador = panel.AddItem(datosConcatenador) as PushButton;
                if (botonConcatenador != null)
                {
                    System.Reflection.Assembly.LoadFrom(dllConcatenador);
                    botonConcatenador.AvailabilityClassName = typeof(Comun.ConDocumento).FullName;
                    botonConcatenador.LargeImage = CuadradoNegro(32);
                    botonConcatenador.Image = CuadradoNegro(16);
                }
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Debug Ribbon Error", ex.ToString());
            }

            return Result.Succeeded;
        }

        static System.Windows.Media.ImageSource CuadradoNegro(int lado)
        {
            try
            {
                var visual = new System.Windows.Media.DrawingVisual();
                using (var dc = visual.RenderOpen())
                {
                    var brush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(0x18, 0x1C, 0x20));
                    brush.Freeze();
                    dc.DrawRectangle(brush, null, new System.Windows.Rect(0, 0, lado, lado));
                }

                var bmp = new System.Windows.Media.Imaging.RenderTargetBitmap(lado, lado, 96, 96, System.Windows.Media.PixelFormats.Pbgra32);
                bmp.Render(visual);
                bmp.Freeze();
                return bmp;
            }
            catch { return null; }
        }

        public Result OnShutdown(UIControlledApplication app)
        {
            return Result.Succeeded;
        }
    }
}
