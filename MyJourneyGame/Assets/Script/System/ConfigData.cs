[System.Serializable]
public class ConfigData
{
    public float bgmVolume = 1.0f;     // BGM音量（0.0〜1.0）
    public float seVolume = 1.0f;      // SE音量（0.0〜1.0）
    public bool isFullScreen = true;  // フルスクリーン設定
    public int resolutionIndex = 0;       // 解像度選択インデックス（追加）
}
