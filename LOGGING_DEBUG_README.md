# 🔍 Logging y Debugging para SORA Service

## 📝 Cambios Implementados

### 1. **Logging Detallado en SoraService**

Se agregó logging completo con `Debug.WriteLine` en todas las fases del proceso:

#### ✅ **Fase de Inicialización**
```csharp
Debug.WriteLine($"[SORA] Starting video generation at {start:yyyy-MM-dd HH:mm:ss}");
Debug.WriteLine($"[SORA] Configuration loaded:");
Debug.WriteLine($"[SORA]   Endpoint: {soraEndpoint}");
Debug.WriteLine($"[SORA]   Model: {soraModel}");
Debug.WriteLine($"[SORA]   API Key: {soraApiKey[..8]}...");
```

#### ✅ **Fase de Creación de Trabajo**
```csharp
Debug.WriteLine($"[SORA] Creating job at URL: {createUrl}");
Debug.WriteLine($"[SORA] Request body: {bodyJson}");
Debug.WriteLine($"[SORA] Response status: {response.StatusCode}");
Debug.WriteLine($"[SORA] Response body: {responseBody}");
```

#### ✅ **Fase de Polling**
```csharp
Debug.WriteLine($"[SORA] Poll #{pollCount}/{maxPolls} - Checking job status...");
Debug.WriteLine($"[SORA] Poll response status: {statusResp.StatusCode}");
Debug.WriteLine($"[SORA] Poll response body: {statusJson}");
Debug.WriteLine($"[SORA] Job status: {status}");
```

#### ✅ **Fase de Descarga**
```csharp
Debug.WriteLine($"[SORA] Generation ID: {generationId}");
Debug.WriteLine($"[SORA] Downloading video from URL: {videoUrl}");
Debug.WriteLine($"[SORA] Download response status: {videoResponse.StatusCode}");
Debug.WriteLine($"[SORA] Content length: {videoResponse.Content.Headers.ContentLength} bytes");
Debug.WriteLine($"[SORA] File size on disk: {new FileInfo(outputFilename).Length} bytes");
```

### 2. **Fix VideoDisplayViewModel**

El problema principal era que `VideoDisplayViewModel` **no estaba recibiendo** el parámetro `VideoSource` del video generado:

#### ❌ **Problema Anterior**
```csharp
// Ignoraba el VideoSource real y usaba video de muestra
VideoSource = "https://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4";
```

#### ✅ **Solución Implementada**
```csharp
if (query.ContainsKey("VideoSource"))
{
    var videoPath = query["VideoSource"].ToString() ?? "";
    Debug.WriteLine($"[VideoDisplay] Received VideoSource: {videoPath}");
    
    // Validate that the file exists
    if (File.Exists(videoPath))
    {
        VideoSource = videoPath;
        Debug.WriteLine($"[VideoDisplay] ✅ Video file exists, set VideoSource: {VideoSource}");
    }
    else
    {
        Debug.WriteLine($"[VideoDisplay] ❌ Video file not found: {videoPath}");
        // Fallback to sample video
        VideoSource = "https://...sample...";
    }
}
```

### 3. **Logging en VideoPromptsViewModel**

Se agregó logging detallado para rastrear el flujo de generación:

```csharp
Debug.WriteLine("[VideoPrompts] GenerateVideo started");
Debug.WriteLine($"[VideoPrompts] SORA configured: {isConfigured}");
Debug.WriteLine($"[VideoPrompts] Combined prompt: {combinedPrompt}");
Debug.WriteLine($"[VideoPrompts] ✅ Video generated successfully: {videoFilePath}");
Debug.WriteLine($"[VideoPrompts] File exists: {File.Exists(videoFilePath)}");
Debug.WriteLine("[VideoPrompts] Navigation parameters:");
foreach (var kvp in navigationParams)
{
    Debug.WriteLine($"[VideoPrompts]   {kvp.Key}: {kvp.Value}");
}
```

## 🎯 **Cómo Usar el Logging**

### **En Visual Studio**
1. Abre la ventana **Output**
2. Selecciona **Debug** en el dropdown
3. Ejecuta la app en modo Debug
4. Los logs aparecerán con formato `[SORA]`, `[VideoPrompts]`, `[VideoDisplay]`

### **En VS Code**
1. Ejecuta con el debugger
2. Los logs aparecen en el **Debug Console**

### **Ejemplo de Output Esperado**
```
[VideoPrompts] GenerateVideo started
[VideoPrompts] SORA configured: True
[VideoPrompts] Combined prompt: A beautiful sunset over mountains. Create a viral video that captures the essence of this scene.
[SORA] Starting video generation at 2025-07-21 14:30:52
[SORA] Configuration loaded:
[SORA]   Endpoint: https://realtimeapidemos.openai.azure.com
[SORA]   Model: sora
[SORA]   API Key: sk-proj12...
[SORA] Creating job at URL: https://realtimeapidemos.openai.azure.com/openai/v1/video/generations/jobs?api-version=preview
[SORA] Request body: {"prompt":"A beautiful sunset...","width":480,"height":480,"n_seconds":5,"model":"sora"}
[SORA] Response status: OK
[SORA] Job created successfully with ID: job_abc123
[SORA] Poll #1/120 - Checking job status...
[SORA] Job status: processing
[SORA] ✅ Job completed successfully after 15 polls
[SORA] Generation ID: gen_xyz789
[SORA] ✅ Video downloaded successfully
[SORA] File size on disk: 1245632 bytes
[VideoPrompts] ✅ Video generated successfully: C:\Users\hprez\Documents\ViralVideos\sora_21Jul2025_143052_A_beautiful_sunset_over.mp4
[VideoPrompts] File exists: True
[VideoDisplay] ApplyQueryAttributes called
[VideoDisplay] Received VideoSource: C:\Users\hprez\Documents\ViralVideos\sora_21Jul2025_143052_A_beautiful_sunset_over.mp4
[VideoDisplay] ✅ Video file exists, set VideoSource: C:\Users\hprez\Documents\ViralVideos\sora_21Jul2025_143052_A_beautiful_sunset_over.mp4
```

## 🐛 **Debugging de Problemas Comunes**

### **1. Video no aparece en VideoDisplayPage**
- **Check**: `[VideoDisplay] Received VideoSource: [path]`
- **Check**: `[VideoDisplay] ✅ Video file exists`
- **Fix**: Si dice "Video file not found", el archivo no se guardó correctamente

### **2. SORA genera error de API**
- **Check**: `[SORA] Response status: [status]`
- **Check**: `[SORA] Response body: [error details]`
- **Fix**: Verificar credenciales y endpoint en Settings

### **3. Generación se cuelga**
- **Check**: `[SORA] Poll #X/120 - Checking job status...`
- **Check**: `[SORA] Job status: [status]`
- **Fix**: Si se queda en "processing" por mucho tiempo, hay un problema con el trabajo

### **4. Navegación falla**
- **Check**: `[VideoPrompts] Navigation parameters:`
- **Check**: Los parámetros incluyen `VideoSource` con ruta válida
- **Fix**: Verificar que el Shell routing esté configurado

## 📊 **Monitoreo de Performance**

El logging incluye métricas de tiempo y tamaño:
- ⏱️ **Tiempo total**: Desde inicio hasta completación
- 📊 **Número de polls**: Cuántas veces se verificó el estado
- 📁 **Tamaño de archivo**: MB del video generado
- 🔄 **Estados de trabajo**: Progreso del job de SORA

## ✅ **Estado Actual**

- ✅ **Logging completo** implementado en todas las fases
- ✅ **VideoDisplayViewModel** corregido para usar videos reales
- ✅ **Validación de archivos** antes de mostrar el video
- ✅ **Fallback automático** a video de muestra si hay problemas
- ✅ **Error handling** mejorado con contexto detallado

¡Ahora puedes rastrear exactamente qué está pasando en cada paso de la generación de videos! 🎬📊
