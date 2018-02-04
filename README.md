# CGOL
Conway's Game of Life In Unity 

# Rules
1. Any live cell with fewer than two live neighbours dies, as if caused by underpopulation.
2. Any live cell with two or three live neighbours lives on to the next generation.
3. Any live cell with more than three live neighbours dies, as if by overpopulation.
4. Any dead cell with exactly three live neighbours becomes a live cell, as if by reproduction.

# Controls
* LMB on the cube to make it alive/dead only when simulation not started/paused
* Enter/Spacebar to start/pause simulation
* R to reset Camera back to original position
* LMB hold and drag to pan
* Scroll to zoom in/out
* Esc to exit in standalone build
* Press Go to go to simulation scene if on start scene

# Highlights
The camera controller of orthographic camera is highly optimized and provides a good interface.
The other branch has only 2d with a basic start scene, I have decided to remove it from the master.