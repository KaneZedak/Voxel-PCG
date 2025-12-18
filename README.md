# PCG Voxel Cave Systems Using Astronomical Distances As Input
 This project uses the Unity Engine to procedurally generate a voxel (3d pixel) cave system environment. It also fetches real time astronomical distances with AstronomyAPI, and uses them as inputs to influence various aspects of the      procedural generation algorithm. An outline of the complete generation process is as follows:
 1. Randomly selects a planet within the Solar System, other than Earth.
 2. Fetches the current distance of that planet to Earth.
 3. Uses linear interpolation to find where, between the planet's minimum and maximum possible distances from Earth, the current distance lies. This is called the current distance proportion.
 4. Randomly chooses the number of cavities to generate, within a range. The range is different depending on which planet was selected.
 5. Determines which cavities will be connected by tunnels. All cavities will be connected to at least one other so that the entire system is traversable. The total number of tunnels increases the higher the planet's current distance proportion is.
 6. Assigns a position for the center of each cavity. Connected cavities can only be between certain minimum and maximum distances apart, bounding the length of an individual tunnel. These min/max values are also scaled by the current distance proportion.
 7. Creates a matrix of initial coordinates for voxels which make up the shells of cavities. The pre-generation cavity plans begin as simple shapes (sphere, cube) of a default size.
 8. Modifies the coordinate matrix by an erosion algorithm, displacing individual voxels to create more organic cave-like spaces. The strength of the erosion is scaled by the current distance proportion, meaning that cavities generated for a planet farther from Earth (relative to its average distance) will have more irregular geometry.
 9. Adds new coordinates to the matrix representing the voxel shells for all tunnels. Tunnels travel between the centers of connected cavities through a series of midpoints. The positions of these midpoints are randomized to create winding or curved tunnels.
 10. Finally, places actual voxels at the coordinate positions. The color scheme and animated effects of the voxel prefab used depends on the planet selected.

Video Showcase

[![Planetary Voxel PCG Showcase](https://img.youtube.com/vi/M2-_TgWQokE/0.jpg)](https://www.youtube.com/watch?v=M2-_TgWQokE)
