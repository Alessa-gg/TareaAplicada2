##Persona 4 — Integración de Datos: Persistencia Local y API Externa

- Arquitectura offline-first: la app no depende 100% de la conexión
- API = datos temporales / Almacenamiento local = datos persistentes
- Favoritos guardados en List<T> → archivo CSV
- Flujo: Consultar API → mostrar → guardar como favorito → cargar al iniciar

Casos de uso: sistemas POS, clientes de correo, apps de inventario/campo
En una app de escritorio como la construida en este ejercicio, coexisten dos "fuentes de verdad": la API externa, que ofrece datos remotos y actualizados pero requiere conexión, y el almacenamiento local (archivo, CSV o base de datos embebida), disponible siempre, incluso sin internet. El patrón que combina ambas fuentes se conoce como arquitectura offline-first (o "local-first"): la aplicación trata la conexión a internet como un recurso opcional en lugar de una dependencia obligatoria.

El flujo implementado en FavoritosService.cs sigue estos pasos:

(1) la app consulta la API REST de forma asíncrona y muestra el resultado en pantalla.

(2) si el usuario decide guardar esa consulta, se serializa a CSV y se escribe a un archivo local usando List<T> como colección genérica.

(3) al iniciar el programa, ese archivo se vuelve a cargar a memoria, de modo que las consultas favoritas estén disponibles aunque no haya conexión en ese momento.

(4) los datos de la API y los datos locales cumplen roles distintos: unos son temporales (se pierden si no se guardan) y otros son persistentes.

Un punto relevante para la defensa técnica: al tratarse de una aplicación de un solo usuario en una sola máquina, no existe el problema de conflictos de escritura simultánea entre múltiples usuarios, lo cual simplifica el caso frente a aplicaciones móviles o web multiusuario, donde sí es necesario resolver qué versión del dato "gana" cuando dos dispositivos sincronizan al mismo tiempo.
Casos de uso en la industria: sistemas de facturación/POS que siguen operando sin internet y sincronizan ventas al volver la conexión; clientes de correo de escritorio (Outlook, Thunderbird) que guardan copia local de los correos y sincronizan con el servidor; herramientas de campo (inspecciones, inventarios) usadas en zonas con mala señal; y aplicaciones de inventario en tiendas que consultan precios o stock desde una API central pero permiten seguir operando localmente si cae la conexión. Este patrón tiene nombre propio en la literatura técnica: "local-first software".
