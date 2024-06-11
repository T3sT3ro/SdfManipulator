using System;
using Unity.Collections;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using ArgumentException = System.ArgumentException;

namespace Editor {
    /**
     * An utility to take a screenshot of a render texture and save it to disk, used to screenshot images for the thesis
     * Source: https://forum.unity.com/threads/save-rendertexture-or-texture2d-as-image-file-utility.1325130/
     * authors: atomicjoe, arkano22, YoxerG
     */
    public class SaveTextureToFileWindow : EditorWindow {
        ObjectField     texture;
        TextField       filePath;
        Vector2IntField size;
        EnumField       format;

        string uniqueFilePath;



        public enum SaveTextureFileFormat {
            EXR, JPG, PNG, TGA,
        };



        [MenuItem("Tools/Save Texture To File Window")]
        public static void ShowWindow() {
            var wnd = GetWindow<SaveTextureToFileWindow>();
            wnd.minSize = new Vector2(300, 105);
            wnd.titleContent = new GUIContent("Save Texture To File");
        }

        public void CreateGUI() {
            var root = rootVisualElement;
            texture = new ObjectField("Texture") { objectType = typeof(Texture) };
            root.Add(texture);
            filePath = new TextField("File Path") { value = "Assets/texture.png" };
            root.Add(filePath);
            size = new Vector2IntField("Size")
                { value = new Vector2Int(-1, -1), tooltip = "Negative values mean original width and height." };
            root.Add(size);
            format = new EnumField("Format", SaveTextureFileFormat.PNG);
            root.Add(format);
            root.Add(new Button(Save) { text = "Save" });
            root.Add(new Button(RenderCamera) { text = "Render main camera" });
        }

        void Save() {
            uniqueFilePath = AssetDatabase.GenerateUniqueAssetPath(filePath.value);
            SaveTextureToFile(
                (Texture)texture.value,
                uniqueFilePath,
                size.value.x,
                size.value.y,
                (SaveTextureFileFormat)format.value,
                done: DebugResult
            );
        }

        void RenderCamera() {
            uniqueFilePath = AssetDatabase.GenerateUniqueAssetPath(filePath.value);
            var previousRT = Camera.main.targetTexture;

            Camera.main.targetTexture = RenderTexture.GetTemporary(4096, 2160);
            Camera.main.Render();
            var sceneCam = SceneView.GetAllSceneCameras()[0]; // scene camera?
            sceneCam.targetTexture = RenderTexture.GetTemporary(4096, 2160);
            sceneCam.Render();

            Graphics.Blit(sceneCam.targetTexture, Camera.main.targetTexture);
            RenderTexture.ReleaseTemporary(sceneCam.targetTexture);
            RenderTexture.ReleaseTemporary(Camera.main.targetTexture);

            SaveTextureToFile(
                Camera.main.targetTexture,
                uniqueFilePath,
                size.value.x,
                size.value.y,
                (SaveTextureFileFormat)format.value,
                done: DebugResult
            );

            Camera.main.targetTexture = previousRT;
        }

        void DebugResult(bool success) {
            if (success) {
                AssetDatabase.Refresh();
                var file = AssetDatabase.LoadAssetAtPath(uniqueFilePath, typeof(Texture2D));
                Debug.Log($"Texture saved to [{uniqueFilePath}]", file);
            } else
                Debug.LogError($"Failed to save texture.");
        }

        static void SaveTextureToFile(
            Texture source,
            string filePath,
            int width,
            int height,
            SaveTextureFileFormat fileFormat = SaveTextureFileFormat.PNG,
            int jpgQuality = 95,
            bool asynchronous = true,
            Action<bool> done = null
        ) {
            if (source is null)
                throw new ArgumentException("No texture selected!");
            // check that the input we're getting is something we can handle:
            if (source is not (Texture2D or RenderTexture))
                throw new ArgumentException("Unsupported texture type: " + source.GetType().Name);

            // use the original texture size in case the input is negative:
            if (width < 0 || height < 0) {
                width = source.width;
                height = source.height;
            }

            // resize the original image:
            var resizeRT = RenderTexture.GetTemporary(width, height, 0);
            Graphics.Blit(source, resizeRT);

            // create a native array to receive data from the GPU:
            var narray = new NativeArray<byte>(width * height * 4, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);

            // request the texture data back from the GPU:
            var request = AsyncGPUReadback.RequestIntoNativeArray(
                ref narray,
                resizeRT,
                0,
                request => {
                    // if the readback was successful, encode and write the results to disk
                    if (!request.hasError) {
                        var encoded = fileFormat switch
                        {
                            SaveTextureFileFormat.EXR => ImageConversion.EncodeNativeArrayToEXR(
                                narray,
                                resizeRT.graphicsFormat,
                                (uint)width,
                                (uint)height
                            ),
                            SaveTextureFileFormat.JPG => ImageConversion.EncodeNativeArrayToJPG(
                                narray,
                                resizeRT.graphicsFormat,
                                (uint)width,
                                (uint)height,
                                0,
                                jpgQuality
                            ),
                            SaveTextureFileFormat.TGA => ImageConversion.EncodeNativeArrayToTGA(
                                narray,
                                resizeRT.graphicsFormat,
                                (uint)width,
                                (uint)height
                            ),
                            _ => ImageConversion.EncodeNativeArrayToPNG(narray, resizeRT.graphicsFormat, (uint)width, (uint)height),
                        };

                        System.IO.File.WriteAllBytes(filePath, encoded.ToArray());
                        encoded.Dispose();
                    }

                    narray.Dispose();

                    // notify the user that the operation is done, and its outcome.
                    done?.Invoke(!request.hasError);
                }
            );

            if (!asynchronous)
                request.WaitForCompletion();
        }
    }
}
