using TMPro;
using UnityEngine;

/// <summary>
/// ゲーム中の経過時間を計測してUIに表示するクラス。
/// InGameManager から StartTimer / StopTimer を呼び出して制御する。
/// </summary>
public class TimerController : MonoBehaviour
{
    // 現在の経過時間（秒）。public なので他のスクリプトから読み取れる
    public float CurrentTime = 0f;

    // タイマーが動いているかどうかを表すフラグ（true = 計測中）
    private bool isRunning = false;

    // 経過時間を表示するテキストUI（インスペクターで割り当てる）
    public TMP_Text TimerText;

    /// <summary>
    /// タイマーを 0 からスタートさせるメソッド。
    /// ゲームプレイ開始時に InGameManager から呼ばれる。
    /// </summary>
    public void StartTimer()
    {
        // 経過時間を0にリセットして計測を開始する
        CurrentTime = 0f;
        isRunning = true;
    }

    /// <summary>
    /// タイマーを止めるメソッド。
    /// ゲームクリア時に InGameManager から呼ばれる。
    /// </summary>
    public void StopTimer()
    {
        // フラグを false にするだけで Update での加算が止まる
        isRunning = false;
    }

    /// <summary>
    /// 毎フレーム呼ばれるメソッド。
    /// タイマーが動いているとき（isRunning = true）だけ時間を加算してUIを更新する。
    /// </summary>
    private void Update()
    {
        if (isRunning)
        {
            // Time.deltaTime は「前のフレームからの経過時間（秒）」。
            // 毎フレーム加算することで現実の時間と同じ速さで増える
            CurrentTime += Time.deltaTime;

            // $"..." は文字列補間。{} の中に変数を入れると文字列に変換して埋め込める
            // "F2" は小数点以下2桁で表示するフォーマット指定子
            TimerText.text = $"Time: {CurrentTime.ToString("F2")}s";
        }
    }
}
