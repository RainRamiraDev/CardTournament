# CardTournament Frontend

Frontend de la aplicación **CardTournament**, desarrollada en React + Material UI, para la gestión de usuarios y cartas en un torneo de cartas coleccionables.

---

## Cumplimiento de la Consigna

A continuación se detalla cómo y dónde se cumple cada punto de la consigna en este proyecto:

### 1. **Pantalla Login con datos hardcodeados**
- **Ubicación:** `src/views/LoginView.jsx` y componentes relacionados.
- **Descripción:**  
  El login solicita usuario y contraseña, los cuales están hardcodeados en el frontend para simular la autenticación.  
  Si los datos son correctos, se permite el acceso al sistema.

### 2. **CRUD de usuario con roles (Admin, Organizador, Juez, Jugador)**
- **Ubicación:**  
  - Formulario y tabla: `src/components/usuario/CrudUsuarioForm.jsx`  
  - Vista principal: `src/views/CrudUsuarioView.jsx`
- **Descripción:**  
  Permite crear, leer, actualizar y eliminar usuarios.  
  Cada usuario puede tener uno de los siguientes roles:  
  - **Admin**
  - **Organizador**
  - **Juez**
  - **Jugador**  
  Los roles se visualizan y gestionan desde el formulario y la tabla de usuarios.

### 3. **Asignación de cartas con validaciones**
- **Ubicación:**  
  - Asignación: `src/components/cartas/AsignarCartaForm.jsx`  
  - Lógica de validación: `src/services/cardService.js` y componentes de cartas.
- **Validaciones implementadas:**  
  - **No se puede asignar una carta repetida a un usuario.**
  - **Cada usuario puede tener un máximo de 15 cartas y un mínimo de 8.**
  - Los mensajes de error y éxito se muestran mediante Snackbar/Alert.

### 4. **Visualización de cartas del jugador**
- **Ubicación:**  
  - Componente: `src/components/cartas/PlayersCards.jsx`
  - Vista: `src/views/CartasUsuarioView.jsx`
- **Descripción:**  
  Permite seleccionar un usuario y ver todas sus cartas asignadas, mostrando detalles y atributos de cada carta.

### 5. **Sistema web responsivo (web y celular)**
- **Ubicación:**  
  - Estilos globales: `src/themes/ThemeProvider.js`
  - Uso de breakpoints y Grid en todos los componentes principales.
- **Descripción:**  
  El sistema utiliza Material UI y breakpoints personalizados para asegurar que todas las vistas y componentes se adapten correctamente a pantallas de escritorio y dispositivos móviles.

### 6. **Uso de `.env` para variables de entorno**
- **Ubicación:**  
  - Archivo: `.env`
  - Uso en código: `import.meta.env` o `process.env` según configuración de Vite/React.
- **Descripción:**  
  Las URLs de la API y otras configuraciones sensibles se gestionan a través de variables de entorno, permitiendo fácil configuración para distintos entornos (desarrollo, producción, etc).

---

## Estructura del Proyecto

```
src/
├── assets/                # Imágenes y recursos estáticos
├── components/            # Componentes reutilizables (UI, menú, usuario, cartas, error)
│   ├── ui/
│   │   ├── Error404.jsx
│   │   ├── PerButton.jsx
│   │   └── ...otros componentes UI
│   ├── menu/
│   │   ├── AppbarMenu.jsx
│   │   ├── Menu.jsx
│   │   └── SideDrawer.jsx
│   ├── usuario/
│   │   ├── CrudUsuarioForm.jsx
│   │   └── ...otros relacionados a usuario
│   └── cartas/
│       ├── AsignarCartaForm.jsx
│       ├── PlayersCards.jsx
│       └── ...otros relacionados a cartas
├── hooks/                 # Custom hooks (por ejemplo, useDrawer.js)
├── services/              # Servicios para llamadas a API y lógica de negocio
│   ├── userService.js
│   ├── cardService.js
│   └── ...otros servicios
├── themes/                # Temas y configuración de Material UI
│   └── ThemeProvider.js
├── views/                 # Vistas principales (CRUD, login, etc)
│   ├── CrudUsuarioView.jsx
│   ├── CartasUsuarioView.jsx
│   ├── LoginView.jsx
│   └── ...otras vistas
├── App.jsx                # Componente raíz
└── main.jsx               # Entry point de la app
```

---

## Detalle de los Elementos del Proyecto

### **Carpeta `assets/`**
- Imágenes de cartas, fondos y recursos gráficos utilizados en la app.

### **Carpeta `components/ui/`**
- **Error404.jsx:** Página de error personalizada y animada para rutas no encontradas.
- **PerButton.jsx:** Botón personalizado reutilizable en toda la app.
- Otros componentes UI reutilizables.

### **Carpeta `components/menu/`**
- **AppbarMenu.jsx:** Barra superior fija, con gradiente y responsiva.
- **Menu.jsx:** Componente que integra AppBar y Drawer para la navegación.
- **SideDrawer.jsx:** Drawer lateral con navegación y estilos avanzados.

### **Carpeta `components/usuario/`**
- **CrudUsuarioForm.jsx:** Formulario y tabla para gestión CRUD de usuarios y roles.
- Otros componentes relacionados a usuario.

### **Carpeta `components/cartas/`**
- **AsignarCartaForm.jsx:** Formulario para asignar cartas a usuarios con validaciones.
- **PlayersCards.jsx:** Visualización de cartas asignadas a cada usuario.
- Otros componentes relacionados a cartas.

### **Carpeta `hooks/`**
- **useDrawer.js:** Custom hook para manejar el estado del Drawer lateral.
- Otros hooks personalizados según necesidades.

### **Carpeta `services/`**
- **userService.js:** Funciones para interactuar con la API de usuarios.
- **cardService.js:** Funciones para interactuar con la API de cartas y lógica de validación.
- Otros servicios para la lógica de negocio.

### **Carpeta `themes/`**
- **ThemeProvider.js:** Configuración avanzada de Material UI, breakpoints, paleta de colores, gradientes, fondos y estilos globales.

### **Carpeta `views/`**
- **CrudUsuarioView.jsx:** Vista principal para gestión de usuarios.
- **CartasUsuarioView.jsx:** Vista para asignar y ver cartas de usuarios.
- **LoginView.jsx:** Vista de login con datos hardcodeados.
- Otras vistas según la navegación de la app.

### **Archivos principales**
- **App.jsx:** Componente raíz que define las rutas y estructura general.
- **main.jsx:** Punto de entrada de la aplicación.

### **Archivo `.env`**
- Variables de entorno como la URL de la API y otras configuraciones sensibles.

---

## Instalación y Uso

1. Clona el repositorio:
   ```bash
   git clone https://github.com/tuusuario/CardTournament-frontend.git
   cd CardTournament-frontend
   ```

2. Crea un archivo `.env` en la raíz y define tus variables de entorno, por ejemplo:
   ```
   VITE_API_URL=http://localhost:3000/api
   ```

3. Instala las dependencias:
   ```bash
   npm install
   # o
   yarn install
   ```

4. Inicia la aplicación:
   ```bash
   npm run dev
   # o
   yarn dev
   ```

5. Accede a [http://localhost:5173](http://localhost:5173) en tu navegador.

---
