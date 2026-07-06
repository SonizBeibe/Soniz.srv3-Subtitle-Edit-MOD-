# SonizSRV3

El editor de subtítulos pero mejor :) 
Un fork especializado y de alto rendimiento diseñado específicamente para typesetting avanzado en ASS y animación tipográfica.

> **⚠️ Agradecimientos y Créditos:**
> SonizSRV3 es una modificación personalizada construida sobre la increíble base de [Subtitle Edit](https://github.com/SubtitleEdit/subtitleedit), creado y mantenido por Nikolaj Olsson (niksedk) y colaboradores. Todo el procesamiento central de subtítulos, soporte de formatos y elementos base de la interfaz Avalonia UI son acreditados con orgullo al equipo original de Subtitle Edit.

---

## ✨ Características Exclusivas de SonizSRV3

A diferencia de los editores de subtítulos estándar, SonizSRV3 está diseñado a la medida para editores visuales y typesetters:
- **Interfaz de Typesetting Reimaginada:** Un área de edición principal completamente refactorizada que cuenta con selectores de color de 4 capas de acceso rápido (`\1c` a `\4c`), gestores de estilos y botones de formato.
- **Libertad Temporal Total:** Se han eliminado los límites internos de la línea de tiempo, permitiendo una superposición infinita y animaciones de texto simultáneas precisas sin desplazamientos automáticos.
- **Posicionamiento Visual:** Haz clic directamente sobre el reproductor de video para generar etiquetas `\pos` con precisión basándose en la resolución nativa del video.
- **Integración del Motor de Karaoke:** Análisis nativo avanzado para etiquetas `\k`, `\kf` y `\ko` con conversión de centisegundos a milisegundos.
- **Puente YTT / SRV3:** Integración perfecta en el backend que ejecuta automáticamente `ytsubconverter.exe` al guardar tu archivo `.ass` para generar subtítulos estilizados listos para YouTube al instante.
- **Formatos Optimizados:** Despojado de software heredado innecesario, restringiendo los formatos estrictamente a lo que necesitan los editores de alto nivel (ASS, SRT, TXT y Adobe After Effects).

---

## 🌐 Documentación y Preguntas Frecuentes
Para el uso general del software base, consulta la documentación original:
http://subtitleedit.github.io/subtitleedit/

---

## 🚀 Compilaciones Automatizadas (Builds)
Puedes encontrar las últimas versiones multiplataforma del **Mod SonizSRV3** aquí:  
👉 [Lanzamientos (Releases)](https://github.com/SonizBeibe/Soniz.srv3-Subtitle-Edit-MOD-/releases)

---

## 💻 Requisitos del Sistema

### Requisitos Específicos de SonizSRV3
- **Puente ytsubconverter:** Para utilizar la generación automática de YTT, `ytsubconverter.exe` debe colocarse en el mismo directorio que el ejecutable de SonizSRV3.

### Windows
- Mínimo: Windows 10 versión 22H2 (compilación 19045) o más reciente, completamente actualizado. Las compilaciones anteriores de Windows 10 (2004/20H2/21H1/21H2) han llegado al final de su vida útil y pueden fallar al iniciar con un error de ejecución de .NET (`0x80131506`).

### macOS

- **Versión mínima de macOS**: 12 (Monterey) o más reciente
- El archivo `.dmg` es independiente: `libmpv` y `ffmpeg` vienen incluidos dentro de `Subtitle Edit.app`, por lo que no se requiere instalar MacPorts o Homebrew.

#### Instalación en macOS (Aplicación no firmada)

Debido a que este fork personalizado no está firmado con un certificado de desarrollador de Apple, macOS lo bloqueará por defecto. Aún puedes instalarlo y ejecutarlo siguiendo estos pasos:

1. **Descarga** y haz **doble clic** en el archivo `.dmg` para montarlo.
2. En la ventana que aparece, **arrastra `Subtitle Edit.app` a tu carpeta de Aplicaciones**.
3. Abre la aplicación **Terminal** (puedes encontrarla a través de Spotlight o en `/Applications/Utilities/`).
4. En la Terminal, ejecuta los siguientes comandos para eliminar la marca de cuarentena de seguridad de macOS y agregar una firma de código adhoc:
   ```bash
   sudo xattr -rd com.apple.quarantine "/Applications/Subtitle Edit.app"
