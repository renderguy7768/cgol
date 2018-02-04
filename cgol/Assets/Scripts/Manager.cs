using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

namespace Assets.Scripts
{
    public enum Scenes { Start, Main }
    public enum GameStateEnum : byte { Invalid, Wait, AcceptInput, Run }
    public static class Manager
    {
        
        public static GameStateEnum GameState;

        public static GameObject CellPrefab;
        public static Material[] CellMaterials;

        public static bool IsGame3D;
        public static int Width;
        public static int Height;
        public static float GenerationGap;

        public static void GetSavedValues()
        {
            IsGame3D = PlayerPrefs.HasKey("cgol_isgame3d") && PlayerPrefs.GetInt("cgol_isgame3d") != 0;
            Width = PlayerPrefs.HasKey("cgol_width") ? PlayerPrefs.GetInt("cgol_width") : 3;
            Height = PlayerPrefs.HasKey("cgol_height") ? PlayerPrefs.GetInt("cgol_height") : 3;
            GenerationGap = PlayerPrefs.HasKey("cgol_generationgap")
                ? PlayerPrefs.GetFloat("cgol_generationgap")
                : 0.1f;

            SceneManager.sceneLoaded += delegate (Scene scene, LoadSceneMode mode)
            {
                if (scene.buildIndex == (int)Scenes.Main)
                {
                    var gridGenerator = Object.FindObjectOfType<GridGenerator>();
                    gridGenerator.Width = Width;
                    gridGenerator.Height = Height;
                    gridGenerator.GenerationGap = GenerationGap;
                }
            };
        }

        public static bool Initialize()
        {
            var result = true;

            CellPrefab = Resources.Load<GameObject>("Prefabs/Cell");
            Assert.IsNotNull(CellPrefab, "Cell prefab not found");
            if (CellPrefab == null)
            {
                GameState = GameStateEnum.Invalid;
                Debug.LogError("Cell prefab not found");
                result = false;
            }

            CellMaterials = new[]
            {
                Resources.Load<Material>("Materials/Dead"),
                Resources.Load<Material>("Materials/Alive")
            };

            for (var i = 0; i < CellMaterials.Length; i++)
            {
                if (CellMaterials[i] == null)
                {
                    GameState = GameStateEnum.Invalid;
                    Debug.LogErrorFormat("Cell Material {0} not found", i);
                    result = false;
                    break;
                }
            }

            return result;
        }
    }
}
