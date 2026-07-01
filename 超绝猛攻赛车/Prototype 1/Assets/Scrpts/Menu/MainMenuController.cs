using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [Header("场景设置")]
    [SerializeField] private string gameSceneName = "RaceMap";

    [Header("界面面板")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject settingsPanel;

    [Header("声音设置")]
    [SerializeField] private Slider volumeSlider;

    private const string VolumeKey = "GameVolume";

    private void Start()
    {
        // 读取上一次保存的音量。
        // 如果之前没有保存过，就默认使用 1，也就是最大音量。
        float savedVolume = PlayerPrefs.GetFloat(VolumeKey, 1f);

        // 设置游戏全局音量
        AudioListener.volume = savedVolume;

        // 设置滑条显示的数值
        if (volumeSlider != null)
        {
            volumeSlider.value = savedVolume;
        }

        // 开始时显示主菜单，隐藏设置界面
        ShowMainMenu();
    }

    // 点击开始游戏按钮时调用
    public void StartGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    // 点击设置按钮时调用
    public void ShowSettings()
    {
        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    // 点击返回按钮时调用
    public void ShowMainMenu()
    {
        mainMenuPanel.SetActive(true);
        settingsPanel.SetActive(false);
    }

    // 当声音滑条发生变化时调用
    public void SetVolume(float volume)
    {
        // 改变全局音量
        AudioListener.volume = volume;

        // 保存音量设置
        PlayerPrefs.SetFloat(VolumeKey, volume);
        PlayerPrefs.Save();
    }

    // 点击退出按钮时调用
    public void QuitGame()
    {
        Debug.Log("退出游戏");
        Application.Quit();
    }
}
