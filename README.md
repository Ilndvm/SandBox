Procedural World Generation in Unity

This repository contains a Unity project implementing a procedural world generation system based on chunked terrain, Perlin noise, and domain warping techniques. The project was developed as part of an academic thesis and is intended for educational and research purposes.

Project Overview

The system generates a three-dimensional voxel-based world in real time using a chunk-based approach. Terrain height is controlled by configurable noise parameters, while additional techniques such as domain warping are used to reduce repetition and improve visual variety. A configuration menu allows users to adjust generation parameters before creating or regenerating the world.

Key features: Chunk-based world generation, Perlin noise terrain generation, Optional domain warping, Dynamic chunk loading and unloading, First-person player controller, Configuration UI for world generation parameters

Technologies Used
Unity Engine
C#
Unity Input System

Requirements:
Unity Hub, 
Unity Editor (editor version: 6000.0.37f1) 

Notes: 
The project is intended for learning and experimentation.
Performance depends on chunk drawing range and noise settings.
All parameters are exposed and can be modified through the UI or scripts.

License: 
This project is provided for educational use.
Feel free to explore, modify, and extend it.
