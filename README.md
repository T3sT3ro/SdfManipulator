# Usage

TODO

# Packages

- NaughtyAttributes

---

# Problems

- unified perspective and ortho rays - something wrong with the inverse projection of ray origin and ends
  - hypothesis 1: coordinates are in different coordinate systems in vertex and fragment shader - NDC, `(w, w, w)` etc.
  - hypothesis 2: perspective division is setting up `w` for GPU to do projection division. Inverse Projection division is not working, because the fragment shader W coordinate is already 1.0. Maybe pass undivided screen pos via texture coordinate (to skip perspective division and get proper coords, but then maybe `w` param may be different and invalid for projections onto near and far clip planes)
  - hypothesis 3: direction of rays is flipped, as `z` may be increasing in different direction
  - [UNITY is doing something under the hood](https://forum.unity.com/threads/does-unity_matrix_mv-unity_matrix_it_mv-identity.199032/)
  - [proper clip space to world space](https://feepingcreature.github.io/math.html)

# TODO

- basic raymarch domain
  - global option
- gizmos/handles can be drawn using SDFs:
  - Elongate using semi-transparent box with draggable sides. Render face using color of unit vector for corresponding axis `(1,0,0)`. Get pixel color of selected object 
  - Round using 2D circle
  - twist using a shifted twisting infinite cylinder
  - bend using torus
  - <kbd>CTRL</kbd> for discrete steps, <kbd>SHIFT</kbd> for finer control
- passes:  
  [gradients reference](https://www.andrewnoske.com/wiki/Code_-_heatmaps_and_color_gradients)
  - iteration count
  - depth
  - [normal debug](https://www.falloutsoftware.com/tutorials/gl/normal-map.html)
    - [example on shadertoy](https://www.shadertoy.com/view/XsfXDr)
  - concavity
  - occlusion (subsurf)
  - flat materialx
  - displacement
  - clock cycles
- [BVH](https://iquilezles.org/www/articles/sdfbounding/sdfbounding.htm)
- render modes:
  - smooth (default)
  - splats (circles/textures)
  - AO approximation
- Bi/Triplanar texture mapping (sample by normal + blend on edge)
- depth
  - [legacy unity but explained depth reymarching](https://adrianb.io/2016/10/01/raymarching.html)
  - [forum thread](https://forum.unity.com/threads/raymarcher-with-depth-buffer.877936/)
  - [japanese tutorial on uRaymarchToolkit + alpha + depth](https://tips.hecomi.com/entry/2018/12/31/211448)
- ortho+perspective generalized ray origin and direction
  - [stack question asking for the same](https://stackoverflow.com/questions/2354821/raycasting-how-to-properly-apply-a-projection-matrix)
  - [Post with graphic about coordinates at different stages of shader](https://forum.unity.com/threads/what-does-the-function-computescreenpos-in-unitycg-cginc-do.294470/)

# Notes

- [noises](https://lodev.org/cgtutor/randomnoise.html)
- [volumetric light](https://fvcaputo.github.io/2017/05/02/ray-marching.html)
- [depth vs distance and how to convert](https://forum.unity.com/threads/direction-from-camera-to-pixel-is-slightly-shifted-around-the-edges-of-the-screen.1151969/)
- [comparison to raymarching toolkit](https://kev.town/raymarching-toolkit/limitations/)
- [on unity matrices](https://forum.unity.com/threads/does-unity_matrix_mv-unity_matrix_it_mv-identity.199032/#post-1350131)


costs of shader operations:
```
scalar add,sub,mult,div	1
vec4 add/mul/div/sub	4*1
scalar fract()	        2
scalar sqrt()	        2
vec3 dot()	        3
vec3 normalize()	5	(dot + sqrt)
scalar sin()	        2+3	(fract + dot)
vec4 sin()	        4*(3+2)	(4 * sin)
mat4 * vec3	        4*3	(4 * dot)
mat4 * mat4	        4*4*3	(4 * 4 * dot)
```
