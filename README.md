# Conway's Game of life in 2D- and 3D-modes

## Project navigation

### Code

> /game life code/Assets/Scripts

### Builded game

> /GameLife

## Program description

### Basic info

The game of life is a cellular automata invented by mathematician John Conway in 1970. It is a model of cell population growth on a 2D-field.
Each cell can be alive or dead. Depending on the state of cells nearby, the cell may change its state in the next step.

The classic version of this game has been updated with the ability to switch between two-dimensional and three-dimensional modes.
In three-dimensional mode, the player can observe the development of a population of cells in three-dimensional space.

### Cell types

The rules have been changed so that each cell can be in one of five states: an empty place, a regular cell, a parasite, a fungus or a stem cell.
As in the classic version depending on the state of cells nearby, the cell may change its state in the next step.

The above types of cells can born and survive according to the rules set by the player. If you give all cell types numbers:
_empty_ — 0, _regular_ — 1, _parasite_ — 2, _fungus_ — 3, _stem cell_ — 4, — then the neighbors for the conditions check are considered as follows:

- for a regular cell: (1)-(2)+(4)
- for a parasite: (1)+(3)+(4)
- for a fungus: (0)+(3)
- for a stem cell: (1)+(2)+(3)+(4)

Cell types (2), (3), (4) have their own traits distinguishing them from others. For example (2) subtracts one from the number of neighbors of (1) next to it
because it is a parasite that takes away nutrients from regular cells. (2) is the only type that can't appear from nothing but mutates from a cell (1).
(3) borns and stays alive depending on empty space. This means that it can live if there are enough space and fungi nearby. The most strange cell is (4)
because if this cell can't survive it turns into the cell of another type that can live in current environment.

### Player's possibilities

The player can:

- influence the development of a population by changing the initial location of cells and the rules by which cells are born and survive and by observing how they interact with each other.
- change the field size, simulation speed, and field limitations in the settings.
  > If the field is limited, then the simulation takes place in a closed field that has form of a rectangle or a parallelepiped. At the same time, the cells
  > located near the wall have the maximum number of neighbors more than other cells. Else the cell located next to the wall counts some cells next to the
  > opposite wall as the neighbours.
- change simulation speed.
- save field and rools (and color palette in 2D) and load this data.
- use zoom on 2D-field, move along it and change color palette
- draw on 2D-field.
- create a slice of a 3D-field at a certain coordinate or insert a 2D-field as a slice inside a 3D one.

### General technical data

The project is based on the Unity and C#.

The gameplay works like this: the array that stores information about the state of the field has a copy. To avoid errors related to the sequence of
processing cells on the field, each cell searches for neighbors in the old array, and writes its new state to the new array. Counting the number of neighbors
depends on the type of cell. The principle of counting neighbors for each type of cell has already been described above.

### Save and load

Saving the state of the field and settings occurs as follows: first, serializable classes are created, one of this classes contains information about the
field, and the other contains information about the settings, including 4 more classes that contain data about cell types settings. Next, the serializable
classes are placed in a _JSON_-file. So, this file contains all the necessary info to restore the state of the field and game settings. Next, the picture
for save slot is rewrited. It is the photo of the field.

For each game mode (2D and 3D) there are four save slots, that can be called from main menu without possibility to save or from game.

When the saved game is loading, the _JSON_-file is serialized and unpacked to objects that contain data about game. Next, all the settings are applied
and the field is rewrited to correspond the _JSON_-file.

> 2D save/load menu
> ![Alt-Текст](https://github.com/PaberuDesu/gamelifesaved/blob/main/Screenshots/2D_save.jpg)

> 3D save/load menu
> ![Alt-Текст](https://github.com/PaberuDesu/gamelifesaved/blob/main/Screenshots/3D_save.jpg)

### 2D and 3D interactions

The transition from 3D- to 2D-mode is moved into separated game mode where player can transport 2D-field into 3D one or make a slice of 3D-field. You can also change game mode without changing the game field. You can rotate the 3D-field here with the right mouse button drag.

> 3D-2D transition mode
> ![Alt-Текст](https://github.com/PaberuDesu/gamelifesaved/blob/main/Screenshots/3D_2D_transition.jpg)

### Features of the 2D field

If 2D mode is selected, then the cells are not objects as in 3D, but pixels on a texture. Zooming and moving along the field actually changes the size and
position of the entire canvas, which is limited by a mask for a stable position on the screen. This way of working with a 2D-field makes it easier to save
the game, since the picture attached to the saved game is the field itself. This isn't true for a 3D-field, where this picture is rendered through a separate
camera that made for this purpose. These images are needed in order to distinguish the saved fields from each other without downloading games.

Selecting a cell type in 2D-mode is a selection of one of the colors of the palette, each of which corresponds to one of the cell types. Colors of the
palette are changable. Player saves and loads the palette with all the game data.

The creation of new cells is performed by tracking the point of the texture on which the cursor is hovered. If the left mouse button is pressed, then the pixel at this point changes color to the one selected by the user, and the value of the cell ID in the array changes to the ID corresponding to this color. You can also pick the fill instrument and turn the bunch of one-type cells into the cells of the other type.
![Alt-Текст](https://github.com/PaberuDesu/gamelifesaved/blob/main/Screenshots/2D_mode.jpg)

### Features of the 3D field

In 3D mode, in addition to layer-by-layer drawing, there are two modes for creating new cells. When creating a cell using one of these methods, a
corresponding note appears in the history of cell changes in the field.

The first method is long but precise: the user must manually set the coordinates of the cell he wants to change and the new cell type.

The second type is more like main way of interacting with a 2D-field. The ray from the camera passes through the cursor position. A cell is created if the
ray collides with a non-empty cell of the field. User chooses cell type to create. Then he presses right mouse button. If the type of cell into
which the ray crashed doesn't match the type selected by the user, then this cell is replaced with the selected one. Else program tracks which of the six
faces the ray has crashed into, and then a new cell is created so that it touches this face with its face.

If you press the "E" key while editing the 3D field, a mode in which most of the interface is missing will be activated. It increases the view, and it will
be possible to move and rotate the camera. This mode will be active until the same button is pressed again.
![Alt-Текст](https://github.com/PaberuDesu/gamelifesaved/blob/main/Screenshots/3D_mode.jpg)
