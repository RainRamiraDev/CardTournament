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
│   ├── menu/
│   ├── usuario/
│   └── cartas/
├── hooks/                 # Custom hooks
├── services/              # Servicios para llamadas a API y lógica de negocio
├── themes/                # Temas y configuración de Material UI
├── views/                 # Vistas principales (CRUD, login, etc)
└── App.jsx                # Componente raíz
```

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
