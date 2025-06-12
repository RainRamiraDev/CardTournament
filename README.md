# Trabajo Final Backend - Curso Extrados 2024

## Introducción

El trabajo final consiste en la programación de un backend para un sistema de administración de torneos de eliminaciones online de juegos de cartas coleccionables.

## Entidades Principales

### Cartas
Las cartas se publican en "series". Una carta puede pertenecer a una o más series (por ejemplo, la carta X pertenece a la serie 1, la carta Y pertenece tanto a la serie 1 como a la serie 2).

### Mazos
Un mazo de cartas consta de 15 cartas únicas, no se permiten cartas repetidas.

### Juegos/Partidas
Un juego debe durar un máximo de 30 minutos, consta de un jugador vs un jugador, y siempre hay un ganador (no se permite empates).

### Torneo
Un torneo es organizado por un **Organizador**, quien asigna **Jueces** para oficializar los resultados. El sistema debe planificar, según la cantidad de jugadores y el tiempo disponible, los días y horarios de los juegos del torneo. Además, el torneo puede limitar las series de cartas con las que se puede jugar.

El torneo tiene 3 fases básicas:

1. **Fase 1 - Registro**: Los jugadores se registran con su mazo de cartas (el cual debe estar compuesto por cartas pertenecientes a las series aceptadas por el torneo). El organizador debe finalizar el registro manualmente.
2. **Fase 2 - Torneo**: Durante esta fase, no se pueden registrar nuevos jugadores y se deben planificar los juegos. La fase incluye múltiples rondas donde los jugadores eliminados en cada ronda son reagrupados con los ganadores para nuevas rondas. Una vez que todos los juegos de una ronda finalicen, el sistema debe pasar automáticamente a la siguiente ronda.
3. **Fase 3 - Finalización**: Cuando el juego final se haya oficializado, el torneo se marca como "finalizado".

## Roles

El sistema contará con los siguientes roles para los usuarios:

- **Administrador**: 
  - Crear, editar y eliminar otros administradores, organizadores, jueces y jugadores.
  - Ver y cancelar torneos.
  - Debe existir al menos un administrador en la base de datos.
- **Organizador**: 
  - Crear, editar y cancelar torneos, así como crear jueces.
  - Avanzar el torneo a la siguiente fase.
  - Modificar los juegos del torneo.
  - Debe ser creado por un administrador.
- **Juez**:
  - Oficializar los resultados de un juego y descalificar jugadores si es necesario.
  - Crear por un administrador o organizador.
- **Jugador**:
  - Registrar las cartas de su colección y registrarse en un torneo.
  - Crear un mazo de cartas con las cartas disponibles para un torneo.
  - Los jugadores pueden registrarse en el sistema por su cuenta.


# CardTournament Frontend

Frontend de la aplicación **CardTournament**, desarrollada en React con Material UI, para la gestión de usuarios y cartas en un torneo de cartas coleccionables.

## Tabla de Contenidos

- [Características](#características)
- [Instalación](#instalación)
- [Estructura del Proyecto](#estructura-del-proyecto)
- [Temas y Estilos](#temas-y-estilos)
- [Componentes Principales](#componentes-principales)
- [Vistas](#vistas)
- [Manejo de Errores](#manejo-de-errores)
- [Scripts Disponibles](#scripts-disponibles)
- [Créditos](#créditos)

---

## Características

- Gestión de usuarios (CRUD)
- Asignación y visualización de cartas por usuario
- Navegación protegida y rutas amigables
- Interfaz moderna y responsiva con Material UI
- Tematización avanzada y personalizable
- Manejo de errores y notificaciones visuales

## Instalación

1. Clona el repositorio:
   ```bash
   git clone https://github.com/tuusuario/CardTournament-frontend.git
   cd CardTournament-frontend
   ```

2. Instala las dependencias:
   ```bash
   npm install
   # o
   yarn install
   ```

3. Inicia la aplicación:
   ```bash
   npm run dev
   # o
   yarn dev
   ```

4. Accede a [http://localhost:5173](http://localhost:5173) en tu navegador.

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
├── services/              # Servicios para llamadas a API
├── themes/                # Temas y configuración de Material UI
├── views/                 # Vistas principales (CRUD, login, etc)
└── App.jsx                # Componente raíz
```

## Temas y Estilos

- El proyecto utiliza un **ThemeProvider** personalizado con Material UI.
- Soporta modo oscuro, gradientes, sombras y estilos responsivos.
- El fondo puede ser una imagen local o remota, configurable en `src/themes/ThemeProvider.js`.

## Componentes Principales

- **Menu**: Barra superior y drawer lateral para navegación.
- **CrudUsuarioForm**: Formulario para gestión de usuarios.
- **PlayersCards**: Visualización de cartas por usuario.
- **Error404**: Página de error personalizada y animada.
- **Snackbar/Alert**: Notificaciones visuales para feedback del usuario.

## Vistas

- **CrudUsuarioView**: Vista principal para gestión de usuarios.
- **CartasUsuarioView**: Vista para asignar y ver cartas de usuarios.
- **Error404**: Página para rutas no encontradas.

## Manejo de Errores

- Las rutas no válidas muestran una página 404 animada y centrada.
- Los errores de red y validación se notifican mediante Snackbar/Alert.

## Scripts Disponibles

- `npm run dev` — Inicia el servidor de desarrollo.
- `npm run build` — Genera la build de producción.
- `npm run preview` — Previsualiza la build de producción.

## Créditos

- [React](https://react.dev/)
- [Material UI](https://mui.com/)
- [Vite](https://vitejs.dev/)

---

**Desarrollado por [Tu Nombre o Equipo]**



