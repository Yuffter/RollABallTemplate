using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// タイトル画面を管理するクラス。
/// スタートボタンが押されたらゲームシーン（InGame）に切り替える。
/// </summary>
public class TitleManager : MonoBehaviour
{
    // タイトル画面のスタートボタン（インスペクターで割り当てる）
    public Button StartButton;

    /// <summary>
    /// 最初に一度だけ呼ばれるメソッド。
    /// スタートボタンにクリック時の処理（リスナー）を登録する。
    /// </summary>
    private void Start()
    {
        // AddListener でボタンが押されたときに呼ぶメソッドを登録する
        // ここでは OnStartButtonClicked を登録している
        StartButton.onClick.AddListener(OnStartButtonClicked);
    }

    /// <summary>
    /// スタートボタンが押されたときに呼ばれるメソッド。
    /// "InGame" という名前のシーンに切り替える。
    /// </summary>
    private void OnStartButtonClicked()
    {
        // SceneManager.LoadScene でシーン名を指定してシーンを切り替える
        // （Build Settings にシーンが登録されている必要がある）
        SceneManager.LoadScene("InGame");
    }

    /// <summary>
    /// このゲームオブジェクトが破棄されるときに呼ばれるメソッド。
    /// 登録したリスナーを解除してメモリリーク（不要な参照が残り続ける問題）を防ぐ。
    /// </summary>
    private void OnDestroy()
    {
        // AddListener で追加したものは RemoveListener で必ず解除する
        StartButton.onClick.RemoveListener(OnStartButtonClicked);
    }
}
