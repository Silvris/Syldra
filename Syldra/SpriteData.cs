using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Syldra
{
    public class SpriteData
    {
        public bool hasRect = false;
        public bool hasPivot = false;
        public bool hasBorder = false;
        public bool hasPPU = false;
        public bool hasWrap = false;
        public bool hasFilter = false;
        public bool hasMeshType = false;
        public bool hasTO = false;
        public string name = "";
        public Rect rect;
        public Vector2 pivot;
        public Vector4 border;
        public Single pixelsPerUnit;
        public TextureWrapMode wrapMode;
        public FilterMode filterMode;
        public SpriteMeshType meshType;
        public string textureOverride = "";
        public Dictionary<string, string> customData;

        public static bool ExportFromSprite(Sprite spr, string fullPath, bool useTextureRect = false, string textureOverride = "")
        {
            try
            {
                string sprData = "";
                if (textureOverride != "")
                {
                    sprData += $"TextureOverride = {textureOverride}\n";
                }
                if (spr.packed || useTextureRect)
                {
                    Rect textureRect = spr.GetTextureRect();
                    sprData += $"Rect = [{textureRect.x},{textureRect.y},{textureRect.width},{textureRect.height}]\n";
                }
                else sprData += $"Rect = [{spr.rect.x},{spr.rect.y},{spr.rect.width},{spr.rect.height}]\n";
                sprData += $"Pivot = [{spr.pivot.x / spr.rect.width},{spr.pivot.y / spr.rect.height}]\n";
                sprData += $"PixelsPerUnit = {spr.pixelsPerUnit}\n";
                sprData += $"Border = [{spr.border.x},{spr.border.y},{spr.border.z},{spr.border.w}]\n";
                sprData += $"WrapMode = {Enum.GetName(typeof(TextureWrapMode), spr.texture.wrapMode)}\n";
                sprData += $"FilterMode = {Enum.GetName(typeof(FilterMode), spr.texture.filterMode)}\n";
                File.WriteAllText(fullPath, sprData);
                return true;
            }
            catch (Exception ex)
            {
                ModComponent.Log.LogInfo((object)$"Error occured while exporting sprite {spr.name}: {ex}");
                return false;
            }

        }

        public SpriteData(string[] strings, string Name)
        {
            name = Name;
            rect = new Rect();
            pivot = new Vector2();
            border = new Vector4();
            pixelsPerUnit = 1f;
            wrapMode = TextureWrapMode.Clamp;
            filterMode = FilterMode.Point;
            meshType = SpriteMeshType.Tight;
            textureOverride = "";
            customData = new Dictionary<string, string>();

            foreach (string datatype in strings)
            {

                List<string> kvp = new List<string>(datatype.Split('='));
                foreach (string v in kvp)
                {
                    v.Trim();
                }
                if (kvp.Count != 2)
                {
                    ModComponent.Log.LogWarning((object)$"SpriteData [{name}]: Invalid entry (unable to distinguish key)");
                    return;
                }
                //ModComponent.Log.LogInfo(kvp[0].ToLower());
                switch (kvp[0].ToLower())
                {
                    case "rect":
                        SetRect(kvp[1]);
                        break;
                    case "pivot":
                        SetPivot(kvp[1]);
                        break;
                    case "border":
                        SetBorder(kvp[1]);
                        break;
                    case "pixelsperunit":
                        SetPPU(kvp[1]);
                        break;
                    case "wrapmode":
                        SetWrap(kvp[1]);
                        break;
                    case "filtermode":
                        SetFilterMode(kvp[1]);
                        break;
                    case "meshtype":
                        SetMeshType(kvp[1]);
                        break;
                    case "textureoverride":
                        SetTextureOverride(kvp[1]);
                        break;
                    default:
                        customData.Add(kvp[0], kvp[1]);
                        break;

                }
            }
        }

        public void SetRect(string input)
        {
            string[] vals = input.Replace("[", "").Replace("]", "").Split(',');
            if (vals.Length != 4)
            {
                ModComponent.Log.LogWarning((object)$"SpriteData [{name}]: Invalid rect length. Expected 4, got {vals.Length}.");
                return;
            }
            rect.x = Convert.ToSingle(vals[0]);
            rect.y = Convert.ToSingle(vals[1]);
            rect.width = Convert.ToSingle(vals[2]);
            rect.height = Convert.ToSingle(vals[3]);
            //ModComponent.Log.LogInfo($"{rect.x} {rect.y} {rect.width} {rect.height}");
            hasRect = true;
        }
        public void SetPivot(string input)
        {
            string[] vals = input.Replace("[", "").Replace("]", "").Split(',');
            if (vals.Length != 2)
            {
                ModComponent.Log.LogInfo((object)$"SpriteData [{name}]: Invalid pivot length. Expected 2, got {vals.Length}.");
                return;
            }
            pivot.x = Convert.ToSingle(vals[0]);
            pivot.y = Convert.ToSingle(vals[1]);
            //ModComponent.Log.LogInfo($"{pivot.x} {pivot.y}");
            hasPivot = true;
        }
        public void SetBorder(string input)
        {
            string[] vals = input.Replace("[", "").Replace("]", "").Split(',');
            if (vals.Length != 4)
            {
                ModComponent.Log.LogInfo((object)$"SpriteData [{name}]: Invalid border length. Expected 4, got {vals.Length}.");
                return;
            }
            border.x = Convert.ToSingle(vals[0]);
            border.y = Convert.ToSingle(vals[1]);
            border.z = Convert.ToSingle(vals[2]);
            border.w = Convert.ToSingle(vals[3]);
            //ModComponent.Log.LogInfo($"{border.x} {border.y} {border.z} {border.w}");
            hasBorder = true;
        }
        public void SetPPU(string input)
        {
            pixelsPerUnit = Convert.ToSingle(input);
            hasPPU = true;
        }
        public void SetTextureOverride(string path)
        {
            textureOverride = path;
            hasTO = true;
        }
        public void SetWrap(string input)
        {
            switch (input.ToLower())//so we don't have to check for capitalization
            {
                case "clamp":
                    wrapMode = TextureWrapMode.Clamp;
                    hasWrap = true;
                    break;
                case "repeat":
                    wrapMode = TextureWrapMode.Repeat;
                    hasWrap = true;
                    break;
                case "mirror":
                    wrapMode = TextureWrapMode.Mirror;
                    hasWrap = true;
                    break;
                case "mirroronce":
                    wrapMode = TextureWrapMode.MirrorOnce;
                    hasWrap = true;
                    break;
                default:
                    ModComponent.Log.LogInfo((object)$"SpriteData [{name}]: Invalid wrap mode: {input}.");
                    break;
            }
            //ModComponent.Log.LogInfo(type);
        }
        public void SetFilterMode(string input)
        {
            switch (input.ToLower())
            {
                case "point":
                    filterMode = FilterMode.Point;
                    hasFilter = true;
                    break;
                case "bilinear":
                    filterMode = FilterMode.Bilinear;
                    hasFilter = true;
                    break;
                case "trilinear":
                    filterMode = FilterMode.Trilinear;
                    hasFilter = true;
                    break;
                default:
                    ModComponent.Log.LogInfo((object)$"SpriteData[{name}]: Invalid filter mode: {input}.");
                    break;
            }
        }
        public void SetMeshType(string input)
        {
            switch (input.ToLower())
            {
                case "Tight":
                    meshType = SpriteMeshType.Tight;
                    hasMeshType = true;
                    break;
                case "FullRect":
                    meshType = SpriteMeshType.FullRect;
                    hasMeshType = true;
                    break;
                default:
                    ModComponent.Log.LogInfo((object)$"SpriteData[{name}]: Invalid mesh type: {input}.");
                    break;
            }
        }

        public bool HasCustomData(string key)
        {
            return customData.ContainsKey(key);
        }

        public string GetCustomData(string key)
        {
            return customData.TryGetValue(key, out var data) ? data.ToString() : string.Empty;  
        }

        public Sprite CreateSpriteFromData(Texture2D tex = null,string basePath = "")
        {
            if(tex == null)
            {
                //we're likely using TO
                if (hasTO)
                {
                    //this assumes they remember to pass basePath
                    tex = Functions.ReadTextureFromFile(Path.Combine(basePath, textureOverride + ".png"),Path.GetFileName(textureOverride));
                }
                else
                {
                    //likely unintended behavior, but we can use this to just create a blank texture at this width/height for whatever reason
                    tex = new Texture2D((int)rect.width, (int)rect.height) {
                        wrapMode = this.wrapMode,
                        filterMode = this.filterMode
                        
                    };
                }
            }
            //now that tex is guaranteed not null, we can actually create a sprite
            Sprite spr = Sprite.Create(
                tex,
                hasRect ? rect : new Rect(0, 0, tex.width, tex.height),
                hasPivot ? pivot : new Vector2(0.5f, 0.5f),
                hasPPU ? pixelsPerUnit : 1f,
                0,
                SpriteMeshType.Tight,
                hasBorder ? border : new Vector4(0, 0, 0, 0)
            );
            tex.wrapMode = hasWrap ? wrapMode : TextureWrapMode.Clamp;
            spr.name = name;
            spr.hideFlags = HideFlags.HideAndDontSave;
            return spr;
        }
    }
}
