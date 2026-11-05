# SDF manipulator

A tool for generating raymarching shaders with **S**igned **D**istance **F**ields — a technique common in a demoscene
community for achieving advanced effects with math. Often dubbed as "painting with math". Common operations that are
otherwise difficult in regular rendering include:

- almost infinite resolution of the shapes, without the need for expensive tessellation and meshes
- complex shapes, such as fractals, that are impossible or impractical to render with traditional methods
- boolean operations between shapes: union, subtract, intersect with a bunch of modifiers, like smooth blending
- infinite repetition of the scene geometry with virtually no cost penalty (of course raymarching shaders incur a little
  bit of penalty of their own)
- cheap space-transforming operation such as bending, twisting, elongating, rotating, adding noise, inflating,
  deflating, revolving, mirroring, hollowing out and others.

## Goals of this package:

- Provide easy, intuitive control over generated shaders, without the need for manual and painstaking
  editing and tinkering with the source code. If you only started learning about raymarching and SDFs, this tool is for
  you.
- Add easy to control gizmos and a set of operators that can be used to edit the scene in a WYSWIG manner.
- A bunch of functionality that is more flexible than ShaderGraph API for generating custom shaders **correctly**,
  without the need to learn new templating language or perform a bunch of string manipulations. This tool uses
  Abstract (and Concrete) Syntax Trees for generating, formatting and processing code. A big inspiration for this kind
  of design was the Roslyn's Syntax API.
- Generate stand-alone shaders, but also shader partials, that can be used in other packages (e.g. compatible with
  ShaderGraph) and other software (HLSL partial files ) 
