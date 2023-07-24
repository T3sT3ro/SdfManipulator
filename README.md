# Usage

TODO

# Packages

- NaughtyAttributes

---

# Problems

- [x] unified perspective and ortho rays - something wrong with the inverse projection of ray origin and ends
  - hypothesis 1 [x]: coordinates are in different coordinate systems in vertex and fragment shader - NDC, `(w, w, w)` etc. (indeed, NDC was wrong)
  - hypothesis 2: perspective division is setting up `w` for GPU to do projection division. Inverse Projection division is not working, because the fragment shader W coordinate is already 1.0. Maybe pass undivided screen pos via texture coordinate (to skip perspective division and get proper coords, but then maybe `w` param may be different and invalid for projections onto near and far clip planes)
  - hypothesis 3: direction of rays is flipped, as `z` may be increasing in different direction
  - [UNITY is doing something under the hood](https://forum.unity.com/threads/does-unity_matrix_mv-unity_matrix_it_mv-identity.199032/)
  - [proper clip space to world space](https://feepingcreature.github.io/math.html)
  - [use this working perspective+ortho projection as reference](https://www.shadertoy.com/view/WtfGW2)

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
- [x] Triplanar texture mapping (sample by normal + blend on edge)
  - [lengthy tutorial](https://catlikecoding.com/unity/tutorials/advanced-rendering/triplanar-mapping/)
  - [ ] maybe biplanar projection as well, based on IQ's notes
- [x] depth to max ray distance
  - [legacy unity but explained depth reymarching](https://adrianb.io/2016/10/01/raymarching.html)
  - [forum thread](https://forum.unity.com/threads/raymarcher-with-depth-buffer.877936/)
  - [japanese tutorial on uRaymarchToolkit + alpha + depth](https://tips.hecomi.com/entry/2018/12/31/211448)
- [x] ortho+perspective generalized ray origin and direction
  - [stack question asking for the same](https://stackoverflow.com/questions/2354821/raycasting-how-to-properly-apply-a-projection-matrix)
  - [Post with graphic about coordinates at different stages of shader](https://forum.unity.com/threads/what-does-the-function-computescreenpos-in-unitycg-cginc-do.294470/)
  - [x] write depth of pixel -- to properly mix many domains
    - [x] fix depth calculation in global origin mode
- [ ] display 3D texture holding SDF or voxel data
- [X] local scale not affecting rays
  - [ ] fix normals
- [X] correct ZTest when inside of a domain
- [X] triplanar texture swizzle and mapping
- [X] limit ray by doing depth prepass over backfaces
  - prepass has unexpected (but logical) behavior while multiple domains overlap
- [ ] Runtime attribute that would transform inputs in editor to toggles/properties etc, but make them constant after compilation
- [ ] Pixelize operator - something like return clamped distance 
- [ ] Check out `noperspective` for generating ray directions out of a vertices in vertex shader (instead of fragment shader). [Check here for reference](https://stackoverflow.com/questions/59786805/wobble-in-volumetric-fixed-step-raymarching#59786805)
- [ ] Fiz Z-test keyword based on [this response on unity discord](https://discord.com/channels/489222168727519232/497874081329184799/1007638427136700507) - basically [use this](https://docs.unity3d.com/Manual/SL-Stencil.html) i.e. `UnityEngine.Rendering.CompareFunction`
- [ ] support dynamic node visitors 
- [ ] fallback evaluators for nodes that are not implemented
- [ ] allow users to define procedural textures
- [ ] debug and release variants of source code generation (for in debug include number of steps made by the shader, while strip it from release)
- [ ] in-editor camera effect shader (without domain) based on [this tutorial](https://www.youtube.com/watch?v=iZ6ARyKnD-k&t=354s)
- [ ] graph -> dependency tree (with topological sort) -> AST -> code generation 

# Important to remember while documenting

- be wary of combination of ZTest, Cull, ZWrite, Origin on face vs near plane, as that can render confusing geometry when combined in weird ways. E.g.: 
  - Cull Off, Origin Face -- camera inside domain won't render the object "in the center of domain", as the origin would lie on a back face.
  - No ZWrite, Depth read, Cull Off, Origin Face - backface marched objects would layer on top of front marched objects

# Threads and forum posts on problems with raymarching:
- [Automatic perspective divide?](https://forum.unity.com/threads/automatic-perspective-divide.530236/)
- [CONFIRMED NOT DIRECTLY SOLVABLE - How to get exit vertex position in given ray direction (ShaderLab)](https://forum.unity.com/threads/confirmed-not-directly-solvable-how-to-get-exit-vertex-position-in-given-ray-direction-shaderlab.1029241/)
- [Pixel ray direction in frag shader](https://forum.unity.com/threads/pixel-ray-direction-in-frag-shader.808386/)
- [Extract frustum points](https://donw.io/post/frustum-point-extraction/)
- [Ray origin from MVP matrix](https://encreative.blogspot.com/2019/05/computing-ray-origin-and-direction-from.html)
- [Aplify shaders + distance fields](https://www.reddit.com/r/Unity3D/comments/iyhax6/basic_implementation_of_ray_marched_signed/)
- [ray generation w component question](https://forum.unity.com/threads/urgent-a-strange-problem-about-using-unity_camerainvprojection.1254780/)
- [unity_CameraInvProjection](https://discussions.unity.com/t/what-does-unity_camerainvprojection-actually-is-how-to-transform-point-from-ndc-space-to-view-space/226646)
- [Homogenous, clip, NDC space](https://carmencincotti.com/2022-05-02/homogeneous-coordinates-clip-space-ndc/)
- [ndc space confusion](https://forum.unity.com/threads/confused-on-ndc-space.1024414/)
- [perspective divide](https://stackoverflow.com/questions/31686613/passing-data-into-a-vertex-shader-for-perspective-divide)
- [clip space to world space in vertex shader](https://forum.unity.com/threads/solved-clip-space-to-world-space-in-a-vertex-shader.531492/)
- [normal interpolation, why normalization matters](https://github.com/gfxfundamentals/webgl-fundamentals/issues/80)
- [normalization issues](https://www.lighthouse3d.com/tutorials/glsl-12-tutorial/normalization-issues/)
- [Perspective correct interpolation](https://stackoverflow.com/questions/24441631/how-exactly-does-opengl-do-perspectively-correct-linear-interpolation)
- 

# Unity docs

- [Builtin shader functions](https://docs.unity3d.com/Manual/SL-BuiltinFunctions.html)
- [Builtin macros](https://docs.unity3d.com/Manual/SL-BuiltinMacros.html)

# Notes

- [noises](https://lodev.org/cgtutor/randomnoise.html)
- [volumetric light](https://fvcaputo.github.io/2017/05/02/ray-marching.html)
- [depth vs distance and how to convert](https://forum.unity.com/threads/direction-from-camera-to-pixel-is-slightly-shifted-around-the-edges-of-the-screen.1151969/)
- [rendering sphere on a quad (earth raymarched, with shadows in raymarching)](https://bgolus.medium.com/rendering-a-sphere-on-a-quad-13c92025570c#c582)
- [screen space reflection](https://lettier.github.io/3d-game-shaders-for-beginners/screen-space-reflection.html)
- [comparison to raymarching toolkit](https://kev.town/raymarching-toolkit/limitations/)
- [on unity matrices](https://forum.unity.com/threads/does-unity_matrix_mv-unity_matrix_it_mv-identity.199032/#post-1350131)
- [graphical representation of depth buffer (in GL!)](https://twitter.com/warrenm/status/511672050625163264)
- [depth buffer for water effect](https://www.edraflame.com/blog/custom-shader-depth-texture-sampling/)
- [depth raymarching](https://www.youtube.com/watch?v=iZ6ARyKnD-k)
- [depth buffer effects](https://www.youtube.com/watch?v=yUVrtPCsCb0)
- [project grid onto world with depth](https://github.com/keijiro/DepthInverseProjection)
- [world pos from depth](https://forum.unity.com/threads/world-position-from-depth.151466/)
- [depth buffer line effect](https://www.ronja-tutorials.com/post/017-postprocessing-depth/)
- [perspective correct interpolation](https://paroj.github.io/gltut/Texturing/Tut14%20Interpolation%20Redux.html)
- [japanese blog uRaymarching](https://tips.hecomi.com/entry/2019/01/27/233137)
  - [about deffered and depth reads+writes](https://tips.hecomi.com/entry/2018/12/31/211448)
- [Order Independent Transparency](http://casual-effects.blogspot.com/2014/03/weighted-blended-order-independent.html)
- [Some raymarching effects in unity5](https://github.com/i-saint/Unity5Effects)
- [Some SDF tool in WebGL](https://www.gsn-lib.org/docs/gallery.php?projectName=RaymarchingSDF&graphName=SignedDistanceField3D)
- [Access internal shadergraph methods](https://forum.unity.com/threads/how-to-modify-unity-packages-using-custom-code-and-files-and-also-export-custom-package.799170/)
- [Tracing a sphere](https://bgolus.medium.com/rendering-a-sphere-on-a-quad-13c92025570c) - a tutorial of raytracing sphere in a cube domain with UVs, seams and Z-Write - EPIC in terms of knowledge and useful stuff!
- [Mud bunny](http://longbunnylabs.com/mudbun/) - a tool for SDF generation that works using similar principles
- [2D SDF operations](https://www.ronja-tutorials.com/post/035-2d-sdf-combination/)
- [Scala SDF engine](https://github.com/sungiant/sdf)
- [Fast quaternion mutliplication](https://blog.molecular-matters.com/2013/05/24/a-faster-quaternion-vector-multiplication/)
- [SDF cloud marching](https://www.shadertoy.com/view/ssc3z4)
- [Raymarch transparent bubbles](https://blog.csdn.net/weixin_39967405/article/details/111625326)
- Cloud volumetric raymarching:
  - https://www.shadertoy.com/view/Xsc3R4
  - https://www.shadertoy.com/view/XslGRr
  - BAD https://www.shadertoy.com/view/4sVfz1
  - AA https://www.shadertoy.com/view/3lBfDm
- [Screen Space Reflections](https://lettier.github.io/3d-game-shaders-for-beginners/screen-space-reflection.html)
- [Refraction and glass](https://www.shadertoy.com/view/flcSW2)
- [WOMP online SDF editor in browser](https://alpha.womp3d.com)
- [RayTK graph editor for generaating SDF shaders](https://derivative.ca/community-post/asset/raytk-raymarching-masses/63620)
- [Raymarcher - asset in asset store, meh](https://assetstore.unity.com/packages/vfx/shaders/fullscreen-camera-effects/raymarcher-168069)
- [GDC - GPU based clay simulation and ray-tracing tech in Claybook](https://www.youtube.com/watch?v=Xpf7Ua3UqOA) - about cone tracing, SDF baking, practical aspects
- [Accelerated sphere tracing](https://diglib.eg.org/bitstream/handle/10.2312/egs20181037/029-032.pdf) - ways to enhance sphere tracing
- [Perspective correct texture mapping](https://webglfundamentals.org/webgl/lessons/webgl-3d-perspective-correct-texturemapping.html) - relates to why my view direction in vertex shader differed from those calculated in fragment shader.
- [Clouds shader (tunnel) with good scattering in the middle of article](https://webglfundamentals.org/webgl/lessons/webgl-shadertoy.html)
- [Stackoverflow - perspective correct interpolation of values](https://computergraphics.stackexchange.com/questions/4079/perspective-correct-texture-mapping?rq=1) - look into that when trying to move to ray generation in vertex shader
- [Another one on perspective correct interpolation](https://www.scratchapixel.com/lessons/3d-basic-rendering/rasterization-practical-implementation/perspective-correct-interpolation-vertex-attributes.html)
  - Additionally: [Depth interpolation](https://www.scratchapixel.com/lessons/3d-basic-rendering/rasterization-practical-implementation/visibility-problem-depth-buffer-depth-interpolation.html)
- [depth interpolation or something in NDC](http://simonstechblog.blogspot.com/2012/04/)
- [perspective correct z buffer](https://computergraphics.stackexchange.com/questions/10004/perspective-correct-interpolation-z-buffer)
- [Perspective correct texture interpolation](https://stackoverflow.com/questions/12006132/how-to-correct-for-perspective-when-plotting-a-3d-triangle-with-texture)
- [Z Test attributes in unity](https://discord.com/channels/489222168727519232/497874081329184799/1007638427136700507)
- [Nodes - visual graph editor, some references on how it works](https://nodes.io/story/)
- [Selective visitor pattern](http://www.educery.com/papers/patterns/visitors/selective.visitor.html)
- [SDF toolkit - Asset](https://assetstore.unity.com/packages/vfx/shaders/sdf-toolkit-34821#content) - adds one node to shadergraph that implements (?) raymarching with an SDF that is stored in a texture
- [Deep neural reconstruction of SDF from models](https://mobile.twitter.com/_AlecJacobson/status/1308546141760430080) - here is also [a link to the paper](https://arxiv.org/abs/2009.09808)
- [Generalized winding number for determining inside/outside of meshes](https://www.cs.utah.edu/~ladislav/jacobson13robust/jacobson13robust.html)
- [YT - SDF and Raymarching in blender](https://www.youtube.com/watch?v=TYDhNfyPzdA)
- [Litecanvas - JS library for node graphs on canvas](https://github.com/jagenjo/litegraph.js)
- [NodeGraphProcessor](https://github.com/alelievr/NodeGraphProcessor) - an implementation of a unity node graph editor, can be SUPER useful later on!!!
- [Blender nodes in unity](https://forum.unity.com/threads/beta-blender-nodes-in-unity.1249171/) - a package that implements basic shader node graph editor
- [Roslyn SyntaxNode vs SyntaxToken](https://joshvarty.com/2014/07/11/learn-roslyn-now-part-3-syntax-nodes-and-syntax-tokens/)
- [Roslyn docs - Tree, Syntax Nodes, Tokens and Trivia](https://learn.microsoft.com/en-us/dotnet/csharp/roslyn-sdk/work-with-syntax)
- [UAST - unified ast and walking concept, ANLTR and Roslyn](https://github.com/PositiveTechnologies/PT.Doc/blob/master/Articles/Tree-structures-processing-and-unified-AST/English.md#visitor-and-listener)
- [UAST - comparison of roslyn and antlr source code parsing, good top view of how roslyn works and how it's useful](https://github.com/PositiveTechnologies/PT.Doc/blob/master/Articles/Theory-and-Practice-of-source-code-parsing-with-ANTLR-and-Roslyn/English.md)
- [HLSL parser - a part of HLSL Tools for Visual Studio, structure similar to Roslyn, a bit dated though, inspecting code yields many code smells](https://github.com/roy-t/HlslParser)
- [Writing shaders in HLSL](https://learn.microsoft.com/en-us/windows/win32/direct3dhlsl/dx-graphics-hlsl-writing-shaders-9#declaring-shader-variables)
- [Vertex post-processing, what happens, khronos.org](https://www.khronos.org/opengl/wiki/Vertex_Post-Processing)
- 

## Discord:

> Cyan — 12.08.2022 15:47
ZTest is the same thing as Early-Z, kinda. It's a stage in the rendering that occurs after the vertex shader. The depth of the fragment (basically a pixel) is tested against the depth buffer, base on the ZTest compare function.
If it passes, the value would also be written to the depth buffer (assuming ZWrite On)
If it doesn't pass, that fragment/pixel isn't drawn.
>
> Except, if you use clip(), discard; or alter SV_Depth in the fragment shader, that early-Z can't occur as the depth of the fragment isn't really known. So the depth test then occurs after the fragment stage instead.
>
> "Depth or Z Prepass" is also something different, where you render opaque geometry first to the depth buffer only. Then render again normally. I'm not too familiar with it. I guess it saves time rendering pixels from overlapping objects. In URP there's a "Depth Priming" mode on the Universal Renderer asset which does this.
> 
> 


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

[transform matrix layout inside HLSL while passed via uniform](https://learn.microsoft.com/en-us/windows/win32/direct3dhlsl/dx-graphics-hlsl-per-component-math#the-matrix-type):
```
_m00, _m01, _m02, _m03
_m10, _m11, _m12, _m13
_m20, _m21, _m22, _m23
_m30, _m31, _m32, _m33
                    ^ 
                    transform

// so extracting transform is
o.color.xyz = _BoxFrame1_Transform._m03_m13_m23;
```


## praca:

- teoria raymarchingu
- teoria SDF
- obsługa programu
- porównania 