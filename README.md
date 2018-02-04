# CGOL
Conway's Game of Life In Unity 

# Rules #
### 2D ###
1. Any live cell with fewer than two live neighbors dies, as if caused by underpopulation.
2. Any live cell with two or three live neighbors lives on to the next generation.
3. Any live cell with more than three live neighbors dies, as if by overpopulation.
4. Any dead cell with exactly three live neighbors becomes a live cell, as if by reproduction.
### 3D ###
1. Any live cell with fewer than seven live neighbors dies, as if caused by underpopulation.
2. Any live cell with twelve or thirteen live neighbors lives on to the next generation.
3. Any live cell with more than thirteen live neighbors dies, as if by overpopulation.
4. Any dead cell with eight to twelve live neighbors becomes a live cell, as if by reproduction.

# Controls #
* Play with grid generation by modifying the option on Cell Parent game object under GridGenerator.cs script before starting simulation
* LMB on the cube to make it alive/dead only when simulation not started/paused
* Enter/Spacebar to start/pause simulation
* R to reset Camera back to original position
* Escape to exit in standalone build
### Only in 2D ###
* LMB hold and drag to pan
* Scroll to zoom in/out
### Only in 3D ###
* WASD to move camera 
* LMB hold and move mouse to rotate camera


# Highlights #
* The camera controller is highly optimized and provides a good interface.
* It took me roughly 2 days to get everything done.

# Shortcomings #
* Project could use a start scene with options if a standalone is to be made.