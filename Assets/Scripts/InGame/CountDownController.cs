using System.Collections;
using TMPro;
using UnityEngine;

/// <summary>
/// ゲーム開始前に「3, 2, 1, Go!」とカウントダウンを表示するクラス。
/// InGameManager から呼び出され、カウントダウンが終わるとゲームが始まる。
/// </summary>
public class CountDownController : MonoBehaviour
{
    // インスペクターで設定できるカウントダウンの開始秒数（デフォルトは3秒）
    public int CountDownTime = 3;

    // 現在のカウントダウンの残り秒数（このクラスの内部でだけ使う変数）
    private int currentCountDownTime = 0;

    // カウントダウンの数字を表示するテキストUI（インスペクターで割り当てる）
    public TMP_Text CountDownText;

    /// <summary>
    /// ゲームオブジェクトが有効になったとき最初に一度だけ呼ばれるメソッド。
    /// ここでは特に何もしていない（初期化は StartCountDown で行う）。
    /// </summary>
    private void Start()
    {

    }

    /// <summary>
    /// カウントダウンを開始するコルーチン（IEnumerator）。
    /// コルーチンとは「途中で一時停止できる処理」のこと。
    /// yield return を使うことで、他の処理を止めずに指定時間だけ待機できる。
    /// </summary>
    public IEnumerator StartCountDown()
    {
        // カウントダウンをインスペクターで設定した値にリセットする
        currentCountDownTime = CountDownTime;

        // カウントダウンが 0 より大きい間、繰り返す
        while (currentCountDownTime > 0)
        {
            // テキストUIに現在の残り秒数を表示する（int → string に変換）
            CountDownText.text = currentCountDownTime.ToString();

            // 1秒間待機する（コルーチンなので他の処理はブロックしない）
            yield return new WaitForSeconds(1f);

            // 1秒経ったので残り時間を1減らす
            currentCountDownTime--;
        }

        // カウントが0になったら「Go!」と表示してゲーム開始を知らせる
        CountDownText.text = "Go!";

        // 1秒待ってからカウントダウンテキスト全体を非表示にする
        yield return new WaitForSeconds(1f);
        CountDownText.gameObject.SetActive(false);
    }
}
