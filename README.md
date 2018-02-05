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

# Important #
* Please deselect Cell Parent game object when running in editor to avoid fps drop

# Controls #
* Play with grid generation by modifying the options on Cell Parent game object under GridGenerator.cs script before starting simulation
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
* Assumed toroidal array when determining neighbors to keep the number of neighbors for each cell same.
* Optimized algorithm by using egocentric approach of the inner field regarding its neighbors.
* The camera controller is highly optimized and provides a good interface.
* Made my own rule set for 3D.
* Kept cubes in discreet planes so that 3D did not require different calculations for neighbors, you only need to know whose in front and back of you and its neighbors are your neighbors too. 
* Same data structure i.e. 3 dimensional array is used to maintain cells in 2D and 3D with the difference being that 2D has depth set to 1
* It took me roughly 2 days to get everything done.

# Shortcomings #
* Project could use a start scene with options if a standalone is to be made.
* 3D camera initial placement could be improved. 