# Punto 5 - Parte I: Investigación Teórica
## Comparación con Otras Tecnologías de Desarrollo de Escritorio
### (Windows Forms vs .NET MAUI vs Electron vs JavaFX)

### 1. Introducción
El desarrollo de aplicaciones de escritorio ha experimentado una evolución fundamental durante las últimas décadas. En este proyecto (Tarea Aplicada 2), nuestro equipo desarrolló una aplicación de escritorio en **C# con Windows Forms** conectada a la API de WeatherAPI y almacenamiento local en archivos CSV.

Como parte de la **Persona 5**, este documento investiga y contrasta a **Windows Forms** frente a las tres alternativas contemporáneas más relevantes en el mercado: **.NET MAUI**, **Electron** y **JavaFX**, analizando sus diferencias arquitectónicas, métricas de rendimiento y criterios técnicos de decisión en la industria.

---

### 2. Análisis de Tecnologías

#### 2.1. Windows Forms (WinForms)
* **Arquitectura:** Wrapper de código administrado sobre la API Win32 y GDI+.
* **Ventajas:** Despliegue ultraligero, mínimo consumo de memoria RAM (30 – 80 MB), tiempo de arranque instantáneo (< 0.5 s) y curva de aprendizaje muy baja en C#.
* **Limitaciones:** Exclusivo para Windows, interfaz visual tradicional difícil de personalizar con diseño moderno y fuertemente acoplado al hilo de UI.

#### 2.2. .NET MAUI
* **Arquitectura:** Evolución de Xamarin.Forms para .NET 6/7/8/9. Compila nativo para Windows (WinUI 3), macOS, iOS y Android con XAML y patrón MVVM.
* **Ventajas:** Multiplataforma nativa dentro de .NET con reutilización del 80%-90% de código y soporte táctil moderno.
* **Limitaciones:** Mayor curva de aprendizaje, tiempos de compilación mayores y sin soporte oficial prioritario para Linux.

#### 2.3. Electron
* **Arquitectura:** Combina el motor Chromium con Node.js. Usa HTML5, CSS3, JavaScript/TypeScript (React, Vue). Usado en VS Code, Slack y Discord.
* **Ventajas:** Multiplataforma total (Windows, Mac, Linux), diseño flexible y aprovecha el ecosistema npm.
* **Limitaciones:** Alto consumo de memoria RAM (200 – 700+ MB) y peso de instalador elevado (> 80 MB).

#### 2.4. JavaFX
* **Arquitectura:** Sucesor de Swing, con Scene Graph acelerado por hardware (DirectX/OpenGL), FXML y CSS.
* **Ventajas:** Multiplataforma nativa en Windows, Mac y Linux sin requerir navegador embebido, con soporte para propiedades reactivas.
* **Limitaciones:** Ya no viene incluido en el JDK de Oracle (requiere OpenJFX con Maven/Gradle) y menor adopción comercial reciente.

---

### 3. Matriz Comparativa Técnica

| Métrica | Windows Forms | .NET MAUI | Electron | JavaFX |
| :--- | :--- | :--- | :--- | :--- |
| **Lenguaje** | C#, VB.NET | C#, XAML | JavaScript, TypeScript | Java, Kotlin, FXML |
| **Plataformas** | Windows | Windows, macOS, iOS, Android | Windows, macOS, Linux | Windows, macOS, Linux |
| **Consumo RAM** | Muy Bajo (30-80 MB) | Medio (80-160 MB) | Alto (200-700+ MB) | Moderado (90-220 MB) |
| **Arranque** | Ultrarrápido (< 0.5s) | Rápido (1-2s) | Lento (2-4s) | Moderado (1.5-3s) |
| **Curva de Aprendizaje** | Muy Baja | Media-Alta | Baja-Media | Media |
| **Instalador** | Muy Pequeño (10-25 MB)| Mediano (40-70 MB) | Grande (80-150 MB) | Mediano (40-80 MB) |

---

### 4. Criterios de Elección en la Industria
* **WinForms:** Ideal para herramientas empresariales internas exclusivas de Windows por su velocidad de desarrollo y bajo consumo de recursos.
* **.NET MAUI:** Ideal para organizaciones con infraestructura Microsoft que requieren presencia móvil y de escritorio con el mismo código.
* **Electron:** La opción estándar de la industria comercial cuando se requiere máxima fidelidad visual multiplataforma idéntica a la web.
* **JavaFX:** Recomendado para empresas con backend consolidado en Java/Spring Boot que deseen compartir modelos y lógica en escritorio.

### 5. Conclusión
Para los requerimientos de la Tarea Aplicada 2, **Windows Forms** fue una elección idónea: permitió una interfaz ágil, mínimo consumo de memoria y un control seguro de excepciones asíncronas con `try-catch-finally`, protegiendo la estabilidad del programa.
