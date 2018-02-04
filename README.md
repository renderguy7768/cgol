# CGOL
Conway's Game of Life In Unity 

# Rules
1. Any live cell with fewer than two live neighbours dies, as if caused by underpopulation.
2. Any live cell with two or three live neighbours lives on to the next generation.
3. Any live cell with more than three live neighbours dies, as if by overpopulation.
4. Any dead cell with exactly three live neighbours becomes a live cell, as if by reproduction.

# Controls
LMB on the cube to make it alive/dead only when simulation not started/paused
Enter/Spacebar to start/pause simulation
R to reset Camera back to original position
LMB hold and drag to pan
Scroll to zoom in/out
Esc to exit in standalone build
Press Go to go to simulation scene if on start scene

# Highlights
The camera controller of orthographic camera is highly optimized and provided a good interface.

# Shortcomings
The start scene is pretty rudimentary and not optimized at all. It is just provided to toggle certain settings if a build is made.
For all editor testing please use the main scene directly you can find all the options given on start screen on the cell parent game object under grid generator script. 
