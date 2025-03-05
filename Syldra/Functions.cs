using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Syldra
{

    public static class Functions
    {
        public static List<GameObject> GetAllChildren(GameObject obj)
        {
            List<GameObject> children = new List<GameObject>();

            if (obj != null)
            {
                for (int i = 0; i < obj.transform.childCount; i++)
                {
                    Transform child = obj.transform.GetChild(i);
                    if (child != null)
                    {
                        if (child.gameObject != null)
                        {
                            children.Add(child.gameObject);
                            if (child.childCount != 0)
                            {
                                children.AddRange(GetAllChildren(child.gameObject));
                            }
                        }
                    }


                }
            }
            else
            {
                ModComponent.Log.LogWarning("Root object is null!");
            }

            return children;
        }
        public static GameObject? GetDirectChild(GameObject obj, string childName)
        {

            if (obj != null)
            {
                for (int i = 0; i < obj.transform.childCount; i++)
                {
                    Transform child = obj.transform.GetChild(i);
                    if (child != null)
                    {
                        if (child.gameObject != null)
                        {
                            ModComponent.Log.LogInfo(child.name);
                            if (child.name == childName)
                            {
                                return child.gameObject;
                            }
                        }
                    }


                }
            }
            else
            {
                ModComponent.Log.LogWarning("Root object is null!");
            }

            return null;
        }

        public static int IsSceneCurrentlyLoaded(string sceneName_no_extention)
        {
            for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; ++i)
            {
                Scene scene = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);
                if (scene.name == sceneName_no_extention)
                {
                    //the scene is already loaded
                    return i;
                }
            }

            return -1;//scene not currently loaded in the hierarchy
        }

        //Textures
        public static Texture2D ReadTextureFromFile(String fullPath, String Name)
        {
            try
            {
                Byte[] bytes = File.ReadAllBytes(fullPath);
                Texture2D texture = new Texture2D(1, 1, TextureFormat.ARGB32, false) { name = Name };
                texture.filterMode = FilterMode.Point;
                if (!ImageConversion.LoadImage(texture, bytes))
                    throw new NotSupportedException($"Failed to load texture from file [{fullPath}]");
                texture.hideFlags = HideFlags.HideAndDontSave;
                return texture;
            }
            catch (Exception)
            {
                throw;
            }

        }
        private static Texture2D? CopyAsReadable(Texture texture)
        {
            if (texture == null)
                return null;

            RenderTexture oldTarget = Camera.main.targetTexture;
            RenderTexture oldActive = RenderTexture.active;

            Texture2D result = new Texture2D(texture.width, texture.height, TextureFormat.ARGB32, false);

            RenderTexture rt = RenderTexture.GetTemporary(texture.width, texture.height, 0, RenderTextureFormat.ARGB32);
            try
            {
                Camera.main.targetTexture = rt;
                //Camera.main.Render();
                Graphics.Blit(texture, rt);

                RenderTexture.active = rt;
                result.ReadPixels(new Rect(0, 0, texture.width, texture.height), 0, 0);
            }
            finally
            {
                RenderTexture.active = oldActive;
                Camera.main.targetTexture = oldTarget;
                RenderTexture.ReleaseTemporary(rt);
            }

            return result;
        }

        public static void WriteTextureToFile(Texture2D texture, String outputPath)
        {
            Byte[] data;
            String extension = Path.GetExtension(outputPath);
            switch (extension)
            {
                case ".png":
                    data = ImageConversion.EncodeToPNG(texture);
                    break;
                case ".jpg":
                    data = ImageConversion.EncodeToJPG(texture);
                    break;
                case ".tga":
                    data = ImageConversion.EncodeToTGA(texture);
                    break;
                default:
                    throw new NotSupportedException($"Not supported type [{extension}] of texture [{texture.name}]. Path: [{outputPath}]");
            }

            File.WriteAllBytes(outputPath, data);
        }

        public static void ExportTexture(Texture2D asset, String fullPath)
        {
            if (asset.isReadable)
            {
                WriteTextureToFile(asset, fullPath);
            }
            else
            {
                Texture2D? readable = CopyAsReadable(asset);
                if (readable != null)
                {
                    WriteTextureToFile(readable, fullPath);
                    UnityEngine.Object.Destroy(readable);
                }
            }
        }
    }
}
