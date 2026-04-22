using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// リトライボタンの処理を管理するクラス。
/// ボタンが押されたとき、現在のシーンを再ロードしてゲームをリスタートする。
/// </summary>
public class RetryButton : MonoBehaviour
{
    /// <summary>
    /// リトライボタンが押されたときに呼ばれるメソッド。
    /// 現在のシーンを最初から読み込み直すことでゲームをリセットする。
    /// このメソッドは Unity の Button コンポーネントの OnClick に手動で登録する。
    /// </summary>
    public void OnRetryButtonClicked()
    {
        // GetActiveScene().buildIndex で現在のシーン番号を取得し、
        // そのシーンを再ロードすることでゲームをリスタートする
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
