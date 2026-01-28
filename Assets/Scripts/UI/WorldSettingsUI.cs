using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WorldSettingsUI : MonoBehaviour
{
    [Header("References")]
    public World world;
    public GameManager gameManager;
    public BiomeGenerator biomeGenerator;
    public WaterLayerHandler waterLayerHandler;
    public NoiseSettings biomeNoiseSettings; 
    public WorldGenerationSettings defaultSettings; // ScriptableObject with default values

    [Header("UI Controls (UGUI)")]
    public GameObject uiPanel; // panel to show/hide
    public Button generateButton;

    public Slider chunkDrawingRangeSlider;
    public TMP_InputField  chunkDrawingRangeInput;

    public TMP_InputField  mapSeedOffsetXInput;
    public TMP_InputField  mapSeedOffsetYInput;

    public Slider waterLevelSlider;
    public TMP_InputField  waterLevelInput;

    public Toggle useDomainWarpingToggle;

    public Slider noiseZoomSlider;
    public TMP_InputField  noiseZoomInput;

    public Slider noiseOctavesSlider;
    public TMP_InputField  noiseOctavesInput;

    public Slider persistanceSlider;
    public TMP_InputField  persistanceInput;

    public Slider redistributionSlider;
    public TMP_InputField  redistributionInput;

    public Slider exponentSlider;
    public TMP_InputField  exponentInput;

    private WorldGenerationSettings runtimeSettings;

    private bool isWorldGenerated = false;

    private void Awake()
    {
        // create an editable runtime clone (so we don't modify the asset directly)
        runtimeSettings = ScriptableObject.CreateInstance<WorldGenerationSettings>();
        CopySettings(defaultSettings, runtimeSettings);

        // set up UI callbacks
        generateButton.onClick.AddListener(OnGenerateClicked);

        // sync UI with runtime settings
        PopulateUIFromSettings(runtimeSettings);
        RegisterUIEvents();

        SyncInputsWithSliders();
    }

    private void Update()
    {
        // toggle UI with Escape
        if (Input.GetKeyDown(KeyCode.Escape) && isWorldGenerated)
        {
            bool show = !uiPanel.activeSelf;
            uiPanel.SetActive(show);

            if (show)
                GamePauseManager.Pause();
            else
                GamePauseManager.Resume();
        }
    }

    private void SyncInputsWithSliders()
    {
        if (chunkDrawingRangeSlider != null && chunkDrawingRangeInput != null)
            chunkDrawingRangeInput.text = ((int)chunkDrawingRangeSlider.value).ToString();

        if (waterLevelSlider != null && waterLevelInput != null)
            waterLevelInput.text = ((int)waterLevelSlider.value).ToString();

        if (noiseZoomSlider != null && noiseZoomInput != null)
            noiseZoomInput.text = noiseZoomSlider.value.ToString("F3", CultureInfo.InvariantCulture);

        if (noiseOctavesSlider != null && noiseOctavesInput != null)
            noiseOctavesInput.text = ((int)noiseOctavesSlider.value).ToString();

        if (persistanceSlider != null && persistanceInput != null)
            persistanceInput.text = persistanceSlider.value.ToString("F2", CultureInfo.InvariantCulture);

        if (redistributionSlider != null && redistributionInput != null)
            redistributionInput.text = redistributionSlider.value.ToString("F2", CultureInfo.InvariantCulture);

        if (exponentSlider != null && exponentInput != null)
            exponentInput.text = ((int)exponentSlider.value).ToString();
    }

    public void OnGenerateClicked()
    {
        ReadSettingsFromUI(runtimeSettings); // read user choices into runtimeSettings

        // apply settings to components
        ApplySettingsToWorld(runtimeSettings);

        // Reset old world 
        if (world != null) world.ResetWorld();

        // Generate world and spawn player
        if (world != null) world.GenerateWorld();
        if (gameManager != null) gameManager.SpawnPlayer();

        // hide the UI
        GamePauseManager.Resume();
        HideUI();

        isWorldGenerated = true;
    }

    private void HideUI()
    {
        uiPanel.SetActive(false);
        GamePauseManager.Resume();
    }

    private void PopulateUIFromSettings(WorldGenerationSettings s)
    {
        if (s == null) return;

        if (chunkDrawingRangeSlider != null) chunkDrawingRangeSlider.value = s.chunkDrawingRange;
        if (waterLevelSlider != null) waterLevelSlider.value = s.waterLevel;
        if (useDomainWarpingToggle != null) useDomainWarpingToggle.isOn = s.useDomainWarping;

        if (noiseZoomSlider != null) noiseZoomSlider.value = s.noiseZoom;
        if (noiseOctavesSlider != null) noiseOctavesSlider.value = s.noiseOctaves;
        if (persistanceSlider != null) persistanceSlider.value = s.persistance;
        if (redistributionSlider != null) redistributionSlider.value = s.redistributionModifier;
        if (exponentSlider != null) exponentSlider.value = s.exponent;

        if (mapSeedOffsetXInput != null) mapSeedOffsetXInput.text = s.mapSeedOffset.x.ToString();
        if (mapSeedOffsetYInput != null) mapSeedOffsetYInput.text = s.mapSeedOffset.y.ToString();
    }

    private void ReadSettingsFromUI(WorldGenerationSettings s)
    {
        if (s == null) return;

        s.chunkDrawingRange = ReadIntFromSliderOrInput(chunkDrawingRangeSlider, chunkDrawingRangeInput, s.chunkDrawingRange);

        int seedX = ParseInt(mapSeedOffsetXInput?.text, s.mapSeedOffset.x);
        int seedY = ParseInt(mapSeedOffsetYInput?.text, s.mapSeedOffset.y);
        s.mapSeedOffset = new Vector2Int(seedX, seedY);

        s.waterLevel = ReadIntFromSliderOrInput(waterLevelSlider, waterLevelInput, s.waterLevel);
        s.useDomainWarping = useDomainWarpingToggle != null && useDomainWarpingToggle.isOn;

        s.noiseZoom = ReadFloatFromSliderOrInput(noiseZoomSlider, noiseZoomInput, s.noiseZoom);
        s.noiseOctaves = ReadIntFromSliderOrInput(noiseOctavesSlider, noiseOctavesInput, s.noiseOctaves);
        s.persistance = ReadFloatFromSliderOrInput(persistanceSlider, persistanceInput, s.persistance);
        s.redistributionModifier = ReadFloatFromSliderOrInput(redistributionSlider, redistributionInput, s.redistributionModifier);
        s.exponent = ReadIntFromSliderOrInput(exponentSlider, exponentInput, s.exponent);
    }

    private int ReadIntFromSliderOrInput(Slider slider, TMP_InputField  input, int fallback)
    {
        if (input != null && !string.IsNullOrEmpty(input.text))
            return ParseInt(input.text, fallback);
        if (slider != null)
            return (int)slider.value;
        return fallback;
    }

    private float ReadFloatFromSliderOrInput(Slider slider, TMP_InputField  input, float fallback)
    {
        if (input != null && !string.IsNullOrEmpty(input.text))
            return ParseFloat(input.text, fallback);
        if (slider != null)
            return slider.value;
        return fallback;
    }

    private int ParseInt(string text, int fallback)
    {
        if (int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out int v))
            return v;
        return fallback;
    }

    private float ParseFloat(string text, float fallback)
    {
        if (float.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out float v))
            return v;
        return fallback;
    }

    private void ApplySettingsToWorld(WorldGenerationSettings s)
    {
        if (s == null) return;

        if (world != null)
        {
            world.chunkDrawingRange = s.chunkDrawingRange;
            world.mapSeedOffset = s.mapSeedOffset;
        }

        if (biomeGenerator != null)
        {
            biomeGenerator.useDomainWarp = s.useDomainWarping;
        }

        if (waterLayerHandler != null)
        {
            waterLayerHandler.waterLevel = s.waterLevel;
        }

        if (biomeNoiseSettings != null)
        {
            biomeNoiseSettings.noiseZoom = s.noiseZoom;
            biomeNoiseSettings.octaves = s.noiseOctaves;
            biomeNoiseSettings.persistance = s.persistance;
            biomeNoiseSettings.redistributionModifier = s.redistributionModifier;
            biomeNoiseSettings.exponent = s.exponent;
        }
    }

    private void CopySettings(WorldGenerationSettings from, WorldGenerationSettings to)
    {
        to.chunkDrawingRange = from.chunkDrawingRange;
        to.mapSeedOffset = from.mapSeedOffset;
        to.waterLevel = from.waterLevel;
        to.useDomainWarping = from.useDomainWarping;
        to.noiseZoom = from.noiseZoom;
        to.noiseOctaves = from.noiseOctaves;
        to.persistance = from.persistance;
        to.redistributionModifier = from.redistributionModifier;
        to.exponent = from.exponent;
    }

    private void RegisterUIEvents()
    {
        chunkDrawingRangeSlider.onValueChanged.AddListener(v =>
            chunkDrawingRangeInput.text = ((int)v).ToString());

        waterLevelSlider.onValueChanged.AddListener(v =>
            waterLevelInput.text = ((int)v).ToString());

        noiseZoomSlider.onValueChanged.AddListener(v =>
            noiseZoomInput.text = v.ToString("F3", CultureInfo.InvariantCulture));

        noiseOctavesSlider.onValueChanged.AddListener(v =>
            noiseOctavesInput.text = ((int)v).ToString());

        persistanceSlider.onValueChanged.AddListener(v =>
            persistanceInput.text = v.ToString("F2", CultureInfo.InvariantCulture));

        redistributionSlider.onValueChanged.AddListener(v =>
            redistributionInput.text = v.ToString("F2", CultureInfo.InvariantCulture));

        exponentSlider.onValueChanged.AddListener(v =>
            exponentInput.text = ((int)v).ToString());

        chunkDrawingRangeInput.onEndEdit.AddListener(v =>
            chunkDrawingRangeSlider.value = ParseInt(v, (int)chunkDrawingRangeSlider.value));

        waterLevelInput.onEndEdit.AddListener(v =>
            waterLevelSlider.value = ParseInt(v, (int)waterLevelSlider.value));

        noiseZoomInput.onEndEdit.AddListener(v =>
            noiseZoomSlider.value = ParseFloat(v, noiseZoomSlider.value));

        noiseOctavesInput.onEndEdit.AddListener(v =>
            noiseOctavesSlider.value = ParseInt(v, (int)noiseOctavesSlider.value));

        persistanceInput.onEndEdit.AddListener(v =>
            persistanceSlider.value = ParseFloat(v, persistanceSlider.value));

        redistributionInput.onEndEdit.AddListener(v =>
            redistributionSlider.value = ParseFloat(v, redistributionSlider.value));

        exponentInput.onEndEdit.AddListener(v =>
            exponentSlider.value = ParseInt(v, (int)exponentSlider.value));
    }
    public void ResetToDefaultSettings()
    {
        if (defaultSettings == null || runtimeSettings == null)
            return;

        CopySettings(defaultSettings, runtimeSettings);

        PopulateUIFromSettings(runtimeSettings);

        SyncInputsWithSliders();
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}