# Commandos cone of sight in Unity3D
This project shows how to implement Commandos cone of sight fx in Unity3D.

This effect is also present in other games such as Desperados, Shadow Tactics or Brawl Stars.

It is implemented using a secondary camera depth texture instead of raycasting. This camera is attached to the soldier, which allows the effect to be accurate in terms of what the character could actually see.

The cone angle can be tweaked on the ConeOfSightRenderer component (existing in the soldier hierarchy).

The shader works like a deferred decal so this effect would work with different terrain heights if the cube (rendered by ConeOfSight) is tall enough.

![Cone of Sight demo](https://github.com/joscanper/unity_coneofsightfx/blob/master/Assets/Showcase/Demo.gif)

## Assets
ToonSolider WW2 by PolygonBlacksmith

Tank by Game-Ready Studios

Oil Tank by Rakshi Games

Industrial Building by ReinforcedCC 
