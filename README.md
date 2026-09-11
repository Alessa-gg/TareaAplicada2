#  Investigación Aplicada 2


## Descripción

Aplicación de escritorio en Windows Forms (C#) que consume una API REST pública,
gestiona información en tiempo real y permite guardar/cargar consultas favoritas
en almacenamiento local, con manejo de excepciones y colecciones genéricas.

## Tecnologías utilizadas

- C# (.NET)
- Windows Forms
- HttpClient (consumo de API REST)
- System.Text.Json (serialización/deserialización)
- Colecciones genéricas (`List<T>`)
- Git / GitHub

## Estructura del proyecto
proyecto-clima-equipo/
├── README.md
├── src/
│ ├── Program.cs
│ ├── Form1.cs
│ ├── ClimaService.cs
│ └── FavoritosService.cs
└── docs/
└── investigacion.md


## Integrantes del equipo

| # | Integrante | Rama asignada | Parte del programa |
|---|---|---|---|
| 1 | <nombre> | `config-entorno` | Configuración del entorno + creación del repo |
| 2 | <nombre> | `api-rest` | Consumo de la API REST (ClimaService.cs) |
| 3 | <nombre> | `interfaz-grafica` | Diseño de la interfaz gráfica |
| 4 | Susana Nicole Valle Méndez | `persistencia-datos` | Persistencia de datos (FavoritosService.cs) |
| 5 | Daniela Jazmin Torres Ramos | `manejo-errores` | Manejo de errores e interacción con el usuario main
