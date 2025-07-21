# 🎬 Servicio SORA para Generación de Videos

## 📋 Descripción

El `SoraService` es un servicio completamente integrado que permite generar videos usando la API de SORA de Azure OpenAI. Convierte el código original proporcionado en un servicio reutilizable con inyección de dependencias y configuración basada en `Preferences`.

## ⚙️ Configuración

El servicio lee la configuración desde `Preferences.Default`:

| Clave | Descripción | Valor por defecto |
|-------|-------------|-------------------|
| `SoraEndpoint` | URL del endpoint de Azure SORA | - |
| `SoraApiKey` | Clave de API de Azure SORA | - |
| `SoraDeployment` | Nombre del modelo/deployment | `"sora"` |

## 🏗️ Arquitectura

### Interfaces

```csharp
public interface ISoraService
{
    Task<bool> IsConfiguredAsync();
    Task<string> GenerateVideoAsync(string prompt, int width = 480, int height = 480, int nSeconds = 5);
    string GetCurrentStatus();
}
```

### Implementación

- **Configuración automática**: Lee credenciales desde `Preferences`
- **Directorio de salida**: `Documents/ViralVideos/`
- **Nombres de archivo**: `sora_{timestamp}_{prompt_safe}.mp4`
- **Timeout**: 10 minutos máximo de generación
- **Manejo de errores**: Excepciones específicas para diferentes tipos de errores

## 📁 Archivos Creados

```
Services/
├── ISoraService.cs          # Interfaz del servicio
└── SoraService.cs           # Implementación completa
```

## 🔌 Integración

### 1. Registro en DI Container

```csharp
// En MauiProgram.cs
builder.Services.AddSingleton<ISoraService, SoraService>();
```

### 2. Inyección en ViewModels

```csharp
public VideoPromptsViewModel(IChatService chatService, ISoraService soraService)
{
    _chatService = chatService;
    _soraService = soraService;
}
```

### 3. Uso en el código

```csharp
// Verificar configuración
if (!await _soraService.IsConfiguredAsync())
{
    // Mostrar mensaje para configurar credenciales
    return;
}

// Generar video
var videoPath = await _soraService.GenerateVideoAsync(prompt, 480, 480, 5);

// El archivo se guarda automáticamente en Documents/ViralVideos/
```

## 🎯 Funcionalidades

### ✅ Implementadas

- [x] **Configuración desde Preferences**: Lee credenciales automáticamente
- [x] **Validación de configuración**: Método `IsConfiguredAsync()`
- [x] **Creación de trabajos**: API para iniciar generación de video
- [x] **Polling de estado**: Monitoreo automático del progreso
- [x] **Descarga automática**: Guarda el video en directorio local
- [x] **Manejo de errores**: Excepciones específicas y detalladas
- [x] **Timeout management**: Evita esperas indefinidas
- [x] **Nombres seguros de archivos**: Sanea caracteres problemáticos
- [x] **Estado en tiempo real**: Método `GetCurrentStatus()`
- [x] **Integración completa**: Funciona con la UI existente

### 🔄 Flujo de Trabajo

1. **Inicialización**: El servicio se crea con HttpClient inyectado
2. **Validación**: Se verifica que las credenciales estén configuradas
3. **Creación de trabajo**: Se envía el prompt a la API de SORA
4. **Monitoreo**: Se hace polling cada 5 segundos del estado
5. **Descarga**: Una vez completado, se descarga el video
6. **Resultado**: Se retorna la ruta del archivo generado

### 🎥 Parámetros de Video

- **Ancho**: 480px (configurable)
- **Alto**: 480px (configurable)  
- **Duración**: 5 segundos (configurable)
- **Modelo**: "sora" (configurable desde Settings)

## 🚨 Manejo de Errores

El servicio maneja múltiples tipos de errores:

- `InvalidOperationException`: Servicio no configurado
- `HttpRequestException`: Errores de API/conectividad
- `TimeoutException`: Generación toma más de 10 minutos
- `ArgumentException`: Prompt vacío o inválido

## 📂 Ubicación de Videos

Los videos generados se guardan en:
```
%USERPROFILE%\Documents\ViralVideos\
```

Ejemplo de nombre de archivo:
```
sora_21Jul2025_143052_Unas_delicias_poblanas_sobre.mp4
```

## 🔧 Configuración en la App

El usuario puede configurar las credenciales en la página de Settings:

1. **SORA Endpoint**: `https://realtimeapidemos.openai.azure.com`
2. **SORA API Key**: Clave obtenida de Azure
3. **SORA Deployment**: Nombre del modelo (usualmente "sora")

## 🎬 Integración en VideoPromptsViewModel

El servicio ya está completamente integrado en el flujo de generación de videos:

```csharp
[RelayCommand]
private async Task GenerateVideo()
{
    // 1. Verificar configuración
    // 2. Combinar prompts de IA
    // 3. Generar video con SORA
    // 4. Navegar a página de reproducción
    // 5. Mostrar mensaje de éxito
}
```

## 🎉 Resultado

El servicio SORA está **100% funcional** y listo para generar videos reales usando la API de Azure OpenAI. La integración mantiene la arquitectura MVVM y proporciona una experiencia de usuario fluida con manejo completo de errores y estados de carga.
