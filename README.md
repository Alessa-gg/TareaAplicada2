# Punto 3 - Diseño de la Interfaz Gráfica

## Qué incluye
- `Form1.Designer.cs`: diseño de los controles (TextBox para ciudad, botones "Consultar" y "Guardar Favorito", DataGridView para mostrar resultados).
- `Form1.cs`: lógica del formulario, incluyendo un método `async` (`ObtenerClimaDummyAsync`) que **simula** la llamada a la API mientras el compañero del Punto 2 no suba su `ClimaService.cs`.
- `Program.cs`: punto de entrada de la aplicación.

## Cómo probarlo
1. Abrir la carpeta `AppClima` en Visual Studio (o `dotnet run` si tienes el SDK de .NET 8 con soporte de Windows).
2. Ejecutar el proyecto.
3. Escribir cualquier ciudad y presionar "Consultar": después de ~1.5 segundos aparece una fila con datos simulados en la tabla, sin congelar la ventana.

## Cómo integrar la API real (cuando llegue el Punto 2)
En `Form1.cs`, dentro de `btnConsultar_Click`, reemplazar:
```csharp
var datos = await ObtenerClimaDummyAsync(ciudad);
```
por:
```csharp
var datos = await ClimaService.ObtenerClimaAsync(ciudad);
```
(ajustando el nombre del método/clase según cómo lo entregue el compañero).

## Pendiente de integración con otros puntos
- Botón "Guardar Favorito" está listo en la interfaz, pero la lógica de guardado en CSV se conecta cuando llegue `FavoritosService.cs` (Punto 4).
