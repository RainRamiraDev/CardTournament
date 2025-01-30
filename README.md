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



