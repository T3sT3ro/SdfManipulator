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

# Troubleshoot

- when trying to debug the roslyn source generators, in the solution configuration (edit build configuration next to hammer icon in rider), unmark the linked unity project and leave only SourceGenerator project

# TODO

- basic raymarch domain
  - ~~global option~~ instead of a domain, simply use a parent transform as a scene root. Should a root be positioned inside the "portal" (current mesh domain) or should the portal be positioned inside the SDF controller? If so, then a "Portal" component could be added to narrow shader display to a mesh only.
- gizmos/handles can be drawn using SDFs:
  - Elongate using semi-transparent box with draggable sides. Render face using color of unit vector for corresponding axis `(1,0,0)`. Get pixel color of selected object 
  - Round using 2D circle
  - twist using a shifted twisting infinite cylinder
  - bend using torus
  - <kbd>CTRL</kbd> for discrete steps, <kbd>SHIFT</kbd> for finer control
  - [Handles reference](https://docs.unity3d.com/ScriptReference/Handles.html)
- passes:  
  [gradients reference](https://www.andrewnoske.com/wiki/Code_-_heatmaps_and_color_gradients)
  - iteration count
  - depth
  - [normal debug](https://www.falloutsoftware.com/tutorials/gl/normal-map.html)
    - [example on shadertoy](https://www.shadertoy.com/view/XsfXDr)
  - concavity
  - occlusion (subsurf)
  - flat material
  - displacement
  - clock cycles
- [BVH](https://iquilezles.org/www/articles/sdfbounding/sdfbounding.htm)
- render modes:
  - smooth (default)
  - splats (circles/textures)
  - AO approximation
- SDF nodes:
  - [ ] all primitives
  - [ ] blend operators
  - [ ] domain operators
  - [ ] dye operators (final dye pass that takes an already created SDF and combines it with final dye SDFs (that only evaluate once at the end), without marching them)
- [x] Tri-planar texture mapping (sample by normal + blend on edge)
  - [lengthy tutorial](https://catlikecoding.com/unity/tutorials/advanced-rendering/triplanar-mapping/)
  - [ ] maybe bi-planar projection as well, based on IQ's notes
- [x] depth to max ray distance
  - [legacy unity but explained depth raymarching](https://adrianb.io/2016/10/01/raymarching.html)
  - [forum thread](https://forum.unity.com/threads/raymarcher-with-depth-buffer.877936/)
  - [japanese tutorial on uRayMarchToolkit + alpha + depth](https://tips.hecomi.com/entry/2018/12/31/211448)
- [x] ortho+perspective generalized ray origin and direction
  - [stack question asking for the same](https://stackoverflow.com/questions/2354821/raycasting-how-to-properly-apply-a-projection-matrix)
  - [Post with graphic about coordinates at different stages of shader](https://forum.unity.com/threads/what-does-the-function-computescreenpos-in-unitycg-cginc-do.294470/)
  - [x] write depth of pixel -- to properly mix many domains
    - [x] fix depth calculation in global origin mode
- [ ] display 3D texture holding SDF or voxel data
- [X] local scale not affecting rays
  - [ ] fix normals
- [X] correct ZTest when inside of a domain
- [X] tri-planar texture swizzle and mapping
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
- [ ] think about `required` keyword for records, the `init` properties, primary constructors `record class` vs `record struct` (C#10) and `readonly record`
  - `required`, albeit interesting for constructing syntactically correct trees, prevent from a wide range of lazy-initialized nodes constructed iteratively.
  - and C#11 only...
- [ ] add `SyntaxList` record that implements `IReadOnlyList` and is used instead of IReadOnlyList anywhere in syntax, to allow simpler construction of children (without `Concat`, `Append` etc.)
- [ ] support `with` syntax for node creation and creating other nodes from already existing nodes
- [ ] add replace method to graph that handles replacing nodes and reconnecting links (traverses to parents and rebuilds them)
- [ ] apply serialization
- [ ] depth prepass for ray origin and ray
  - maybe do it so that ray starts on face when outside of mesh (isFrontFacing is true) but start on camera otherwise  
- [ ] in the graph, pass along a vector space metadata, and warn when two different vector spaces are mixed
- [ ] [create AssetImporter](https://docs.unity3d.com/Manual/ScriptedImporters.html)
- [ ] fixup init accessors for the syntax packages
- [ ] sample/ray jittering to avoid banding and similar artifacts
  - [some info here](https://www.scratchapixel.com/lessons/3d-basic-rendering/volume-rendering-for-developers/ray-marching-get-it-right.html)
- [ ] generate red factories for syntax etc, based on [SourceWriter.cs](https://github.com/dotnet/roslyn/blob/34268d1bb9370c7b01c742303a895a99daf10d6a/src/Tools/Source/CompilerGeneratorTools/Source/CSharpSyntaxGenerator/SourceWriter.cs#L1423)
  - consider containing private fields in the syntax class and generate properties from them which add get and init accessors, where init assigns th parent
- [x] add `<SyntaxElement>.MapWith(Visitor v)` which uses dynamic dispatch to update fields recursively
  - [ ] test it
- [ ] add common language syntaxes like "InjectedLanguage", `Expression`, `Statement`, `Literal`
- [ ] remove abstract MapWith from syntax and move the logic solely to abstract visitors, utilize double dispatch
- [ ] unit and vector tagging in shaders - so that a world-space vector isn't confused with model-space vector
- [ ] use a simple tree model instead of a graph model to generate a scene. This way there is no need for any graph editor and whatnot, only a game object hierarchy
- [ ] display focused SDF (even if it's in subtract mode)
- [ ] calculate estimated cost of a shader as a heuristic of basic operations * their occurrences. Parser could be needed for that
- [ ] thread group debug view (for debugging number of steps per thread group)
- [ ] fullscreen scene view rendering instead of a domain. Use domain if the SdfScene has a mesh renderer only. Learn from:
  - [This video](https://www.youtube.com/watch?v=2T2FqvtXqLw)
  - [and his code repo where he uses blit shader for camera here](https://github.com/TheAllenChou/unity-ray-marching/blob/master/unity-ray-marching/Assets/Script/PostProcessingBase.cs)
- [ ] implement conemarching:
  - [video with demonstration of fractals rendered with it](https://www.youtube.com/watch?v=qUBA8Xotc4o)
  - [an article mentioning conemarching and how it's made](https://medium.com/@bananaft/my-journey-into-fractals-d25ebc6c4dc2)
  - [a presentation of cone marching](http://www.fulcrum-demo.org/wp-content/uploads/2012/04/Cone_Marching_Mandelbox_by_Seven_Fulcrum_LongVersion.pdf)
- [ ] fix an unsafe visitor cast inside generated Accept methods of concrete syntax nodes
- [ ] use zipper instead of anchors and traversing logic
  - If zipper were to be used, some kind of child labels would be needed to avoid linear children traversal 
- [ ] use edge labels/indices for children
  - It would allow for easier checking which branch is a syntax node in instead of linearly searching children
  - traversing forward and backward logic would be easier even without a zipper

# Important to remember while documenting

- be wary of combination of ZTest, Cull, ZWrite, Origin on face vs near plane, as that can render confusing geometry when combined in weird ways. E.g.: 
  - Cull Off, Origin Face -- camera inside domain won't render the object "in the center of domain", as the origin would lie on a back face.
  - No ZWrite, Depth read, Cull Off, Origin Face - backface marched objects would layer on top of front marched objects
- use `RoslynAnalyzer` and `SourceGenerator` tags for dlls and do not not referenced directly. They will apply in the assembly
- mapping may be problematic if strongly typed nodes are used, for example when a certain `Type.Struct` will be replaced with `Type.Primitive`, a parent syntax may expect `Type.Struct`, not a `Type`, but incompatible type is returned resulting in runtime error (but regular roslyn also throws runtime errors for syntax trees if kinds don't meet expectations etc.). The method definition would also have to look like `Type.Primitive Map(Type.Struct type)` where param and return types are different. This would be non-achievable in a statically generated Visitor pattern, because all syntax would have to return `Syntax<>` or itself so this is a potential advantage of dynamic dispatch pattern. But a question may be asked if:
  1. it is worth it to implement dynamic dispatch pattern for this reason
  2. does it make sense to have strongly typed nodes in the first place, if they are not used for type checking, but only for code generation
  3. are strongly typed nodes useful at all with how limiting type system is in C# (unlike TS for example)
  4. should rewriting of such nodes be possible, or should user be expected to rewrite them in their parent, where the context is actually relevant
- some syntax nodes have nullable children, but their nullability actually represents validity of the node instead of the tru nullability. If record primary constructors were possible, it would be more correct (currently nullable syntax parts are marked as warnings). This is a C# syntax trying to emulate representing valid and invalid syntax trees on a type system level, albeit quite poorly. 
- syntax generators generate:
  - syntax partials with ChildrenOrToken getters for each child
  - acceptors of visitors in syntax
  - visitors for each language
  - mapper for a language which is like a rewriter
- anchors are used instead of red-green tree for simplicity and because they are easier to implement. Also for explicitness of the traversal and to avoid public+internal syntax nodes.

# Design notes, decisions and insights

- separating node logic from node builders was an idea that in theory should allow for creating a graph model without relying on a specific target of code generation (i.e. a Node as a "Shape" holder defining input/output contracts and Builders as processors of such nodes that collect the code for a specific target language)
  - it had the advantage of writing a single model for nodes shared across, for example, build targets, output languages (GLSL/HLSL), or even different parts of shader (for example a multiply node that is common for both HLSL shader code and for Shaderlab code)
  - this, however, seems like a needles repetition of an abstraction and distributing one idea over multiple files. For example changing a model of some SdfNode would also require all the the builders to be changed accordingly (if they somehow succeed in generation without errors, then those are shadow, possibly logically wrong but syntactically correct cases)
  - It is unlikely that anyone will want to write for other target than Shaderlab+HLSL so this abstraction may not be needed in the first place
  - It is harder to keep consistency between data passed through the node links and the output generated by builders for a port.
  - there won't, however, be an easy way to provide an alternative implementation for a node without recreating it or grossly duplicating the code
  - It was supposed to separate the traversal logic from the logical connection logic.
  - With it, non-DAG graphs could be handled, but it adds a lot of unneeded complexity
- Nodes could take other nodes on the construction state. 
  - This way there would be no way to create invalid graphs on the language level. Evey graph would be a DAG (where output nodes would be constructed using input nodes, so that layers are made "first to last"). 
  - A potential problem would be binding output ports to input ports, because they must reference each other somehow, yet disallow reconnecting an already connected port.
  - with this decision, nodes would be immutable, and creating new connection would essentially mean rebuilding a node. 
- A clear distinction must be made between a node instance and a node template (which in this case could be a class). 
  - A node class contains default values, a function for generating code (possibly factory-like, because it should return new instances of syntax nodes).
  - A node instance contains values that are specific to a given node in a graph. It should be able to generate a syntax node that represents it in a graph. A node instance should contain it's unique UUID that would be used for generating properties.
- syntax parts should have public getters and public init, no setters. The creation of syntax nodes as well as copying is accomplished without factory methods but using record designated constructors. This way selective and default builders are implicitly defined in classes and creating new nodes from old nodes becomes trivial:
   ```C#
   var x = new VariableDeclaration { type = ..., id = "x", initializer = ... };
   var another = x with {id = "another" };
   ```
- syntax sugar is added using implicit conversion operators, for example assigning `string` to `InitializerName` creates new initializer token
- Unfortunately C# doesn't have support for both sum and product types, so creating closed, compile-time, safe syntax is not possible without compromises. Typescript could do that (for example narrow the types of accepted syntax to produce valid syntax tree). The compile-type safety would be nice for constructing syntactically correct trees, however it could lead to problems with representing incorrect trees. This work doesn't focus on parsing nor handling incorrect syntax trees though, so it's good enough.  
- I came up with another approach, by noticing that tokens and (lists of) trivia form an alternating stream, so maybe it would be useful to represent trivia between two tokens by a single list and just point tokens to left and right trivia, while pointing (red) trivia to left and right tokens. This way, I can essentially create not a tree, but a top-down DAG (it connects on the last trivia layer, where each leaf token points to a previous and next trivia list. The red nodes (dynamic, on demand with parent references) then create special type for trivia list that references previous and next token (or null if it's the first/last token in the tree). With this I don't have to include special EOF token, and what's more, I gain an easy way of iterating token and trivia stream and going back and forward. But it is a bit of a pain to implement, especially when the current syntax tree is not the intended final implementation, so we will see later.
- possibly a better design would be a loosely typed syntax tree that only has Syntax, Token and Trivia nodes (and similar) with only creational methods that enforce correct syntax types and structure, as well as a facade for a tree that provide typed access. Nodes in such a facade would be bound by Syntax<> and similar as well as marker interfaces for type matching.  
- Visitors/walkers and similar:
  - visitor pattern - problematic with simple pattern matching because matching StructuredTrivia<T> is hard without specifying type in Accept<..>(Trivia<Lang>), which is not possible without reflection. Adding types makes them recurse and there is no simple way to descend structured trivia without knowing the type.
  - A Visitor interface hierarchy and accept methods sound to be the simplest. base IVisitor and language extensions possibly.
    - There is a problem with this approach - anchors. They have to be somehow passed to Accept. There is also no clear way to do an accept in anchor
    - also how to override accept methods with visitor subtypes e.g. accept hlsl visitor???
    - I don't remember why, but I have made visitor methods accept nullable parameters
      - They might have been for `a.Node.Accept(this, a.Parent)` calls when `a.Parent` is null
- maybe research a Rope or Zipper data structure for navigating syntax trees?
  - [some intro and images for zipper](https://m00nlight.github.io/functional%20programming/2017/12/17/the-functional-zipper-structure)
  - [some zipper implementation and description](https://blog.mattbierner.com/neith-zippers-for-javascript/) 
  - I've made a concept idea of a "weave tree" in other notes, that is basically a tree holding a token stream and trivia stream, where there is a trivia list between each pair of tokens. This tree would avoid the complexity of leading/trailing trivia and their attachments, would provide easy way of navigating the token and trivia stream and could give easy access to navigate between tokens and neighboring trivia. It could solve some problems with existing red/green tree and the need for attaching trivia to tokens (e.g. currently some trivia are attached sensibly, like whitespace or comments, but other, like preprocessor trivia, are attached arbitrarily and have nothing to do with an attached token). It could possibly allow for easier syntax rewrites by operating on a set of tokens and syntax nodes as "brackets" over them (or spans).  
  - There is one poor thing associated with zippers, namely that we would still need either a type variable, a differently named methods or a set of interfaces extending zipper that override/reintroduce property with their specified data type to make overwriting logic in other interfaces/mappers/rewriters easier. For example how to do something like `... Visit(Anchor<BinaryExpression> aBin)` without it?
- Maybe preprocessor syntax as a trivia is not a good solution. Maybe a layered architecture would be better, where first stage tree represents syntax visible by preprocessor, second stage by shaderlab, third stage by hlsl etc.
- Generally while developing this I noticed, that the Roslyn's model of typed tree nodes with static properties is hard to work with on a DX level. For example representing things like "Replace certain tree patterns such as leftmost token of a statement with a new token" is hard to do without advanced code-fu. Properties being static also doesn't really help with tree rewrites and updates. Traversal is problematic — having to explicitly keep track of the path and parent references is doing double work that is implicitly done by program under the hood with descending down the call stack.
- We could use generic syntax node with dynamic runtime Kind property. If the Kind were an enum, we could also create pseudo-algebraic type bounds by creating overlapping enum ranges for syntax nodes. Then we could have for example `enum SyntaxKind { UNARY, BINARY, PRINT }`, `enum ExprKind { UNARY, BINARY }`, `enum UnaryKind {}` and `Syntax<Lang, ExprKind>`
  - on the other hand there would also be problems with type casts e.g. `Syntax<Lang, ExprKind>` is not a valid tree for `Syntax<Lang, UnaryKind>`.
  - and problems with making sure that, say, `Syntax<Lang, AddExpr>` is a binary expression AND it's token is plus.
- Rewrite tree node to support generic get operation for indexed child and replacement for index with new child
  - use type casting and depend on cast exceptions when children are incompatible
  - rewrite visitor and acceptor pattern with a simple pattern matching over node kind in a hierarchy of more and more specialized methods
  - don't use typed anchor, use differently named methods and untyped zipper
  - zipper should be just a wrapper around a child with the path to parent and knowledge of the child index in the parent
  - alternatively a zipper could be just a parent + child label, and the `Node` property would be a derived value of the parent with child
    - this could make multiple calls to get child at index though, possibly 1 for each pattern match.
  - by making Node inherit from `IReadOnlyList` we gain access making reverse iterator fast, but at the same time we have to think what exactly should it enumerate? Branches? if so, is `null` valid? Imo yes. Any post-processing should be done manually by appending index and filtering nulls
- Sometimes ReferenceEquals checks could be improper when we would want to share common syntax nodes. For example an indentation whitespace node could be shared to save space, but at the same time some other mechanism (maybe tagging?) should be used to determine which child is the node (and is it really the same node or not)  

# Threads and forum posts on problems with raymarching:
- [Automatic perspective divide?](https://forum.unity.com/threads/automatic-perspective-divide.530236/)
- [CONFIRMED NOT DIRECTLY SOLVABLE - How to get exit vertex position in given ray direction (ShaderLab)](https://forum.unity.com/threads/confirmed-not-directly-solvable-how-to-get-exit-vertex-position-in-given-ray-direction-shaderlab.1029241/)
- [Pixel ray direction in frag shader](https://forum.unity.com/threads/pixel-ray-direction-in-frag-shader.808386/)
- [Extract frustum points](https://donw.io/post/frustum-point-extraction/)
- [Ray origin from MVP matrix](https://encreative.blogspot.com/2019/05/computing-ray-origin-and-direction-from.html)
- [Amplify shaders + distance fields](https://www.reddit.com/r/Unity3D/comments/iyhax6/basic_implementation_of_ray_marched_signed/)
- [ray generation w component question](https://forum.unity.com/threads/urgent-a-strange-problem-about-using-unity_camerainvprojection.1254780/)
- [unity_CameraInvProjection](https://discussions.unity.com/t/what-does-unity_camerainvprojection-actually-is-how-to-transform-point-from-ndc-space-to-view-space/226646)
- [Homogenous, clip, NDC space](https://carmencincotti.com/2022-05-02/homogeneous-coordinates-clip-space-ndc/)
- [ndc space confusion](https://forum.unity.com/threads/confused-on-ndc-space.1024414/)
- [perspective divide](https://stackoverflow.com/questions/31686613/passing-data-into-a-vertex-shader-for-perspective-divide)
- [clip space to world space in vertex shader](https://forum.unity.com/threads/solved-clip-space-to-world-space-in-a-vertex-shader.531492/)
- [normal interpolation, why normalization matters](https://github.com/gfxfundamentals/webgl-fundamentals/issues/80)
- [normalization issues](https://www.lighthouse3d.com/tutorials/glsl-12-tutorial/normalization-issues/)
- [Perspective correct interpolation](https://stackoverflow.com/questions/24441631/how-exactly-does-opengl-do-perspectively-correct-linear-interpolation)
- [Data transfer between passes](https://forum.unity.com/threads/transferring-array-data-between-passes.995629/)
- [Some depth-buffer related talk](https://discussions.unity.com/t/problems-with-depth-values-in-_cameradepthtexture/37786/3)

# Unity docs

- [Builtin shader functions](https://docs.unity3d.com/Manual/SL-BuiltinFunctions.html)
- [Builtin macros](https://docs.unity3d.com/Manual/SL-BuiltinMacros.html)

# Notes

- [noises](https://lodev.org/cgtutor/randomnoise.html)
- [volumetric light](https://fvcaputo.github.io/2017/05/02/ray-marching.html)
- [depth vs distance and how to convert](https://forum.unity.com/threads/direction-from-camera-to-pixel-is-slightly-shifted-around-the-edges-of-the-screen.1151969/)
- [rendering sphere on a quad (earth ray-marched, with shadows in raymarching)](https://bgolus.medium.com/rendering-a-sphere-on-a-quad-13c92025570c#c582)
- [screen space reflection](https://lettier.github.io/3d-game-shaders-for-beginners/screen-space-reflection.html)
- [comparison to raymarching toolkit](https://kev.town/raymarching-toolkit/limitations/)
- [on unity matrices](https://forum.unity.com/threads/does-unity_matrix_mv-unity_matrix_it_mv-identity.199032/#post-1350131)
- [Algorithms for generating 2D SDF textures from bitmaps](https://prideout.net/blog/distance_fields/)
- [graphical representation of depth buffer (in GL!)](https://twitter.com/warrenm/status/511672050625163264)
- [depth buffer for water effect](https://www.edraflame.com/blog/custom-shader-depth-texture-sampling/)
- [depth raymarching](https://www.youtube.com/watch?v=iZ6ARyKnD-k)
- [depth buffer effects](https://www.youtube.com/watch?v=yUVrtPCsCb0)
- [project grid onto world with depth](https://github.com/keijiro/DepthInverseProjection)
- [world pos from depth](https://forum.unity.com/threads/world-position-from-depth.151466/)
- [depth buffer line effect](https://www.ronja-tutorials.com/post/017-postprocessing-depth/)
- [perspective correct interpolation](https://paroj.github.io/gltut/Texturing/Tut14%20Interpolation%20Redux.html)
- [japanese blog uRaymarching](https://tips.hecomi.com/entry/2019/01/27/233137)
  - [about deferred and depth reads+writes](https://tips.hecomi.com/entry/2018/12/31/211448)
  - [Output-merger stage in HLSL](https://learn.microsoft.com/en-us/windows/win32/direct3d11/d3d10-graphics-programming-guide-output-merger-stage)
- [Order Independent Transparency](http://casual-effects.blogspot.com/2014/03/weighted-blended-order-independent.html)
- [Some raymarching effects in unity5](https://github.com/i-saint/Unity5Effects)
- [Some SDF tool in WebGL](https://www.gsn-lib.org/docs/gallery.php?projectName=RaymarchingSDF&graphName=SignedDistanceField3D)
- [Access internal Shadergraph methods](https://forum.unity.com/threads/how-to-modify-unity-packages-using-custom-code-and-files-and-also-export-custom-package.799170/)
- [Tracing a sphere](https://bgolus.medium.com/rendering-a-sphere-on-a-quad-13c92025570c) - a tutorial of raytracing sphere in a cube domain with UVs, seams and Z-Write - EPIC in terms of knowledge and useful stuff!
- [Mud bunny](http://longbunnylabs.com/mudbun/) - a tool for SDF generation that works using similar principles
- [2D SDF operations](https://www.ronja-tutorials.com/post/035-2d-sdf-combination/)
- [Scala SDF engine](https://github.com/sungiant/sdf)
- [Fast quaternion multiplication](https://blog.molecular-matters.com/2013/05/24/a-faster-quaternion-vector-multiplication/)
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
- [Touc hdesigner - shader writer, manual, node based, but windows and Mac only](https://www.youtube.com/watch?v=ZEQt6_mYplI)
- [RayTK graph editor for generating SDF shaders](https://derivative.ca/community-post/asset/raytk-raymarching-masses/63620)
- [Ray-marcher - asset in asset store, meh](https://assetstore.unity.com/packages/vfx/shaders/fullscreen-camera-effects/raymarcher-168069)
- [GDC - GPU based clay simulation and ray-tracing tech in Claybook](https://www.youtube.com/watch?v=Xpf7Ua3UqOA) - about cone tracing, SDF baking, practical aspects
- [Accelerated sphere tracing](https://diglib.eg.org/bitstream/handle/10.2312/egs20181037/029-032.pdf) - ways to enhance sphere tracing
- [Perspective correct texture mapping](https://webglfundamentals.org/webgl/lessons/webgl-3d-perspective-correct-texturemapping.html) - relates to why my view direction in vertex shader differed from those calculated in fragment shader.
- [Clouds shader (tunnel) with good scattering in the middle of article](https://webglfundamentals.org/webgl/lessons/webgl-shadertoy.html)
- [Stackoverflow - perspective correct interpolation of values](https://computergraphics.stackexchange.com/questions/4079/perspective-correct-texture-mapping?rq=1) - look into that when trying to move to ray generation in vertex shader
- [Another one on perspective correct interpolation](https://www.scratchapixel.com/lessons/3d-basic-rendering/rasterization-practical-implementation/perspective-correct-interpolation-vertex-attributes.html)
  - Additionally: [Depth interpolation](https://www.scratchapixel.com/lessons/3d-basic-rendering/rasterization-practical-implementation/visibility-problem-depth-buffer-depth-interpolation.html)
- [depth interpolation or something in NDC](http://simonstechblog.blogspot.com/2012/04/)
- [depth buffer, spanish article](https://iguagofernando.wordpress.com/2018/03/04/entendiendo-el-buffer-de-profundidad/)
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
- [Roslyn inspired syntax data structure in rust-analyzer](https://github.com/rust-lang/rust-analyzer/blob/master/docs/dev/syntax.md)
- [Roslyn code generators](https://github.com/dotnet/roslyn/blob/34268d1bb9370c7b01c742303a895a99daf10d6a/src/Tools/Source/CompilerGeneratorTools/Source/CSharpSyntaxGenerator/SourceWriter.cs#L1423)
- [UAST - unified ast and walking concept, ANLTR and Roslyn](https://github.com/PositiveTechnologies/PT.Doc/blob/master/Articles/Tree-structures-processing-and-unified-AST/English.md#visitor-and-listener)
- [UAST - comparison of roslyn and antlr source code parsing, good top view of how roslyn works and how it's useful](https://github.com/PositiveTechnologies/PT.Doc/blob/master/Articles/Theory-and-Practice-of-source-code-parsing-with-ANTLR-and-Roslyn/English.md)
- [HLSL parser - a part of HLSL Tools for Visual Studio, structure similar to Roslyn, a bit dated though, inspecting code yields many code smells](https://github.com/roy-t/HlslParser)
- [Writing shaders in HLSL](https://learn.microsoft.com/en-us/windows/win32/direct3dhlsl/dx-graphics-hlsl-writing-shaders-9#declaring-shader-variables)
- [Vertex post-processing, what happens, khronos.org](https://www.khronos.org/opengl/wiki/Vertex_Post-Processing)
- [Creating Node Graph in Unity](https://blog.devgenius.io/using-unitys-graph-view-e9fb8e78980e) 
- [HLSL grammar in Chromium](https://chromium.googlesource.com/external/github.com/google/glslang/+/58d6905ea01f7c44652eb082d26b662ca811df25/hlsl/hlslGrammar.cpp)
- [Writing shaders - shadows, cutouts, alpha testing etc, good, explanatory source](https://nedmakesgames.medium.com/transparent-and-crystal-clear-writing-unity-urp-shaders-with-code-part-3-f6ccd6686507)
- [DirectX compiler tests, allowed syntax can be referenced from it](https://github.com/microsoft/DirectXShaderCompiler/blob/main/tools/clang/test/HLSL/struct-assignments.hlsl)
- [Material property drawers custom attributes - a library of them, in fact. Except for that some useful debug shaders and explanations of commands etc.](https://github.com/supyrb/ConfigurableShaders/wiki/PropertyDrawers)
- [Ray-Marching-Based Volume Rendering of
  Computed Tomography Data in a Game
  Engine](https://cgvr.cs.uni-bremen.de/theses/finishedtheses/volume_rendering_ue4/thesis.pdf) there are elements with semi-transparency, octrees etc.
- [Thick glass textures using front and backface depths to determine mesh thickness](https://prideout.net/blog/old/blog/index.html@p=51.html)
- [RW texture 2D to declare global data and pass it between stages](https://blog.deferredreality.com/write-to-custom-data-buffers-from-shaders/)
- [Full Screen Render Feature in URP to inject fullscreen render pass](https://docs.unity3d.com/Packages/com.unity.render-pipelines.universal@16.0/manual/renderer-features/renderer-feature-full-screen-pass.html)
- [Using command buffers for rendering multipass shaders and reading depth from previous pass](https://forum.unity.com/threads/multi-pass-shader-that-uses-texture-between-passes.561025/)
- [Raymarching a cloud in shadergraph](https://www.youtube.com/watch?v=hXYOlXVRRL8)
- [How to structure the unity package](https://docs.unity3d.com/Manual/CustomPackages.html#EmbedMe)
- [Tree view GUI element in Unity (official API)](https://docs.unity3d.com/ScriptReference/IMGUI.Controls.TreeView.html)
- [Decals rendering, but there is a bit about world-position depth in shadergraph](https://samdriver.xyz/article/decal-render-intro)
- [Reversed depth buffer](https://www.danielecarbone.com/reverse-depth-buffer-in-opengl/)
- [GeoGebra model of direction vectors interpolated correctly and incorrectly](https://www.geogebra.org/calculator/fppufpcf)
  - The conclusion is that I should use plain hitpos-camera direction, interpolate linearly, normalize only in the fragment AFTER the interpolation
- [Dude answered me and explained many tings about calculating ray directions and ray origins in vertex shader](https://computergraphics.stackexchange.com/questions/13666/how-to-calculate-ray-origin-and-ray-direction-in-vertex-shader-working-universal/13667#13667)
  - [This is a better way (with sketches) of generating camera ray from the uv coordinates](https://9bitscience.blogspot.com/2013/07/raymarching-distance-fields_14.html)
- [Shaderbits - a blog about writing HLSL and UE4 shaders, samples with raymarching volumes](https://shaderbits.com/blog/distance-field-ray-tracing-part1)
- [A blog about rendering and many things from some Nvidia developer](https://www.reedbeta.com/)
- [Roslyn source generators in unity](https://medium.com/@EnescanBektas/using-source-generators-in-the-unity-game-engine-140ff0cd0dc)
- [Math utilities, also includes raymarching SDF textures somehow](https://github.com/zalo/MathUtilities)
- [Cables.gl, visual graph programming tool blending logic, visualization and shaders, cool looking](https://cables.gl/)
- [Setting up syntax generators in unity and referencing unity project, writing some tests, example with Entitas](https://github.com/sschmid/Entitas/issues/957)
- [How rowan (rust analyzer's project for syntax trees and parsing) represents syntax](https://github.com/rust-lang/rust-analyzer/blob/master/docs/dev/syntax.md)
- [Adding nuget packages and dlls to (new) unity](https://www.ankursheel.com/blog/installing-nuget-packages-unity-2021)
- [Non linear sphere tracing](https://par.nsf.gov/servlets/purl/10172295)

## Unity internals:

- `UnityEngine.Rendering.BlendOp` : blend operation enum
- `UnityEngine.Rendering.BlendMode` : blend mode enum
- `UnityEngine.Rendering.StencilOp` : stencil operation enum
- `UnityEngine.Rendering.CompareFunction` : compare function enum
- `UnityEngine.Rendering.CullMode` : cull mode enum
- `UnityEngine.Rendering.ColorWriteMask` : color write mask enum

`ComputeScreenPos` takes vector in clip space and produces vector that has to be perspective-divided by it's(?) `w` to properly represent screen position (0->1)

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

```
REEGEX for replacing syntax to generate partial classes with parent binders:
from: (^\s+public partial record (\w+)((?:\s|.)*)\n([^\S]*))public ((?:\S|, )+)\s+(\w+)\s+\{ get; init; \}(.*) 
to:   $1readonly $5 _$6; /*parent_binder*/ public partial record $2 { public $5 $6 { get => _$6; set => _$6 = value with { Parent = this } } public $2() { _$6$7 } }
requires multiple steps
but using grep the names of classes can be extracted later
```


## thesis:

- theory of raymarching
- theory of SDF
  - SDF definitions
  - AO, lightingand shadows
  - texture mapping
  - domain operations
  - CSG
  - object IDs and material blending
- coordinate system (domain world)
- ray generation (for perspective and ortho)
- program usage guide
- comparisons to other technologies
- future work
- HLSL and Shaderlab syntax API
- graph API
- controller API
- limitations
  - transparency
  - non-trivial blending with other geometry
  - edit-view marching is problematic
- HLSL and Shaderlab syntax API
- graph API
- controller API
- limitations
  - transparency
  - non-trivial blending with other geometry
  - edit-view marching is problematic
