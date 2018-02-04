using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class ControlPanelManger : MonoBehaviour
    {
        [Serializable]
        public struct SliderInfo
        {
            public Slider Slider;
            public Text Text;
        }
        public SliderInfo WidthSlider;
        public SliderInfo HeightSlider;
        public SliderInfo GenerationGapSlider;
        public Toggle Toggle3D;
        public Button GoButton;

        private void Awake()
        {
            Manager.GetSavedValues();
            InitializeControls();
        }

        private void OnEnable()
        {
            WidthSlider.Slider.onValueChanged.AddListener(OnWidthSliderChanged);
            HeightSlider.Slider.onValueChanged.AddListener(OnHeightSliderChanged);
            GenerationGapSlider.Slider.onValueChanged.AddListener(OnGenerationGapChanged);
            Toggle3D.onValueChanged.AddListener(OnToggle3DChanged);
            GoButton.onClick.AddListener(GoButtonClicked);
        }

        private void OnDisable()
        {
            WidthSlider.Slider.onValueChanged.RemoveListener(OnWidthSliderChanged);
            HeightSlider.Slider.onValueChanged.RemoveListener(OnHeightSliderChanged);
            GenerationGapSlider.Slider.onValueChanged.RemoveListener(OnGenerationGapChanged);
            Toggle3D.onValueChanged.RemoveListener(OnToggle3DChanged);
        }

        private void InitializeControls()
        {
            WidthSlider.Slider.value = Manager.Width;
            WidthSlider.Text.text = Manager.Width.ToString();

            HeightSlider.Slider.value = Manager.Height;
            HeightSlider.Text.text = Manager.Height.ToString();

            GenerationGapSlider.Slider.value = Manager.GenerationGap;
            GenerationGapSlider.Text.text = Manager.GenerationGap.ToString("F2");

            Toggle3D.isOn = Manager.IsGame3D;
        }

        private static void GoButtonClicked()
        {
            PlayerPrefs.Save();
            SceneManager.LoadScene((int) Scenes.Main, LoadSceneMode.Single);
        }

        private void OnWidthSliderChanged(float val)
        {
            Manager.Width = (int)val;
            WidthSlider.Text.text = ((int)val).ToString();
            PlayerPrefs.SetInt("cgol_width", Manager.Width);
        }
        private void OnHeightSliderChanged(float val)
        {
            Manager.Height = (int)val;
            HeightSlider.Text.text = ((int)val).ToString();
            PlayerPrefs.SetInt("cgol_height", Manager.Height);
        }
        private void OnGenerationGapChanged(float val)
        {
            Manager.GenerationGap = val;
            GenerationGapSlider.Text.text = val.ToString("F2");
            PlayerPrefs.SetFloat("cgol_generationgap", Manager.GenerationGap);
        }

        private static void OnToggle3DChanged(bool val)
        {
            Manager.IsGame3D = val;
            PlayerPrefs.SetInt("cgol_isgame3d", val ? 1 : 0);
        }
    }
}
