# Plantilla Base — UDI 2: Infraestructura para aplicaciones backend

Este repositorio contiene la **API REST base en .NET** que utilizaremos a lo largo de las clases prácticas de la materia para aprender a empaquetar, configurar y desplegar servicios web en entornos de producción.

---

## 📋 Requisitos Previos

Antes de ejecutar este proyecto, asegúrate de tener instalado en tu computadora:

**1. .NET 10 SDK** (o superior en versión LTS): [Descargar .NET](https://dotnet.microsoft.com/download)  
**2. Visual Studio Code**  
**3. Extensión recomendada para VS Code:** `C# Dev Kit` (de Microsoft).

---

## 🚀 Cómo clonar y ejecutar localmente

1. **Clonar el repositorio:**
   ```bash
   git clone <URL_DE_ESTE_REPOSITO>
   cd Plantilla_practica

2. Restaurar paquetes y dependencias:

    ```bash
    dotnet restore

3. Ejecutar la aplicación:

    ```bash
    dotnet run
(O se puede usar `dotnet watch` para recompilar automáticamente ante cambios en el código).

## 🔗 Endpoints disponibles
Una vez que la aplicación esté corriendo (usualmente en http://localhost:5271 o similar según indique tu consola):

- ```GET /api/status``` (Unidad 1): Devuelve información del entorno actual (Development/Production), el nombre de la app y variables de entorno del sistema. Prueba ingresando desde el navegador a: http://localhost:5271/api/status

- ```GET /api/items``` (Unidad 2 y 3): Lista los elementos almacenados en la base de datos PostgreSQL (requiere conexión a PostgreSQL activa para responder).

- ```POST /api/items``` (Unidad 3 y 4): Permite guardar un nuevo ítem en la base de datos para probar peticiones POST y políticas CORS.

## 📂 Archivos clave del proyecto
- *Program.cs*: Configuración de la Minimal API y declaración de endpoints.

- *AppDbContext.cs*: Modelo de datos (Item) y contexto de Entity Framework Core.

- *appsettings.json*: Archivo base de configuración de la aplicación.

- *Plantilla_practica.csproj*: Definición del proyecto .NET y paquetes NuGet requeridos.

## ⚠️ Nota sobre la Base de Datos
En la Unidad 1, no se necesita tener PostgreSQL corriendo para trabajar. Si se ejecuta la app, el endpoint /api/status funcionará correctamente. La integración con PostgreSQL/Supabase se activará progresivamente durante las Unidades 2 y 3 mediante contenedores Docker y variables de entorno.