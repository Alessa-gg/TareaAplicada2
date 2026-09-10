# Punto 3 - Parte I: Investigación
## Programación Asíncrona en C#

### 1. ¿Qué es la programación asíncrona?

La programación asíncrona permite que una aplicación siga respondiendo (por ejemplo, la interfaz gráfica) mientras se ejecuta una tarea que tarda tiempo, como una petición a internet, leer un archivo grande o consultar una base de datos. En vez de "congelar" el programa mientras se espera el resultado, la tarea se ejecuta en segundo plano y el programa continúa haciendo otras cosas.

### 2. Síncrono vs Asíncrono

**Síncrono:**
- Las instrucciones se ejecutan una detrás de otra, en orden.
- Si una instrucción tarda (ej. una llamada a una API), todo el programa se queda esperando (bloqueado) hasta que termine.
- En una app de escritorio, esto se nota porque la ventana deja de responder ("se congela") y hasta puede salir el mensaje de "No responde".

**Asíncrono:**
- Una tarea que tarda se "lanza" y el programa puede seguir ejecutando otras instrucciones mientras esa tarea trabaja en segundo plano.
- Cuando la tarea termina, el programa retoma el resultado donde lo dejó.
- La interfaz gráfica sigue respondiendo (se puede mover la ventana, hacer clic en otros botones, etc.) mientras se espera la respuesta.

### 3. Palabras clave: `async` y `await`

- **`async`**: se coloca en la firma de un método para indicar que ese método contiene operaciones asíncronas y puede usar `await` dentro de él. Un método `async` normalmente devuelve `Task`, `Task<T>` o `void` (este último solo se recomienda para manejadores de eventos).

- **`await`**: se usa antes de una operación que devuelve una `Task`. Le dice al programa: "esperá a que esto termine, pero mientras tanto no bloquees el hilo principal; dejá que la aplicación siga funcionando".

Ejemplo básico:

```csharp
private async void btnConsultar_Click(object sender, EventArgs e)
{
    string resultado = await ObtenerDatosClimaAsync();
    lblResultado.Text = resultado;
}

private async Task<string> ObtenerDatosClimaAsync()
{
    await Task.Delay(2000); // Simula una espera, como una llamada a una API
    return "Soleado, 28°C";
}
```

Mientras se ejecuta `Task.Delay(2000)`, la ventana del formulario sigue respondiendo con normalidad.

### 4. ¿Por qué es importante en apps de escritorio (WinForms)?

WinForms funciona con un solo hilo principal (el hilo de la interfaz gráfica). Si una operación larga (como una llamada HTTP a una API del clima) se hace de forma síncrona, ese hilo se bloquea y toda la ventana deja de responder hasta que termine. Usando `async`/`await`, la llamada se hace sin congelar la interfaz, mejorando la experiencia del usuario.

### 5. Errores comunes al usar async/await

1. **Usar `async void` en métodos que no son eventos**: dificulta el manejo de excepciones, porque no se puede hacer `await` sobre un `void`. Se recomienda usar `async Task` siempre que sea posible, excepto en manejadores de eventos (`Click`, `Load`, etc.).

2. **Olvidar el `await`**: si se llama a un método asíncrono sin `await`, el programa no espera el resultado y puede seguir con datos vacíos o incompletos, generando errores difíciles de detectar.

3. **Bloquear código asíncrono con `.Result` o `.Wait()`**: esto anula el propósito de la asincronía y puede provocar un **deadlock** (el programa se queda "colgado" esperando a sí mismo), especialmente en aplicaciones con interfaz gráfica.

4. **No manejar excepciones**: las excepciones dentro de métodos `async` deben capturarse con `try/catch` alrededor del `await`, porque si no, la excepción puede perderse o hacer que la aplicación se cierre inesperadamente.

5. **Actualizar controles de la interfaz desde otro hilo sin cuidado**: aunque `async/await` en WinForms regresa automáticamente al hilo de la interfaz después del `await`, si se mezclan tareas manuales (`Task.Run` con hilos propios) hay que tener cuidado de no modificar controles desde un hilo que no sea el principal.

### 6. Conclusión

`async` y `await` son herramientas clave para que una aplicación de escritorio se mantenga fluida y responsiva mientras realiza tareas que toman tiempo, como consumir una API REST. Usarlos correctamente evita que la interfaz se congele y mejora la experiencia del usuario final.
