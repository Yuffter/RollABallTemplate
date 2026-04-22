using System.Collections;
using TMPro;
using UnityEngine;

/// <summary>
/// ゲーム全体の流れを管理するクラス。
/// 「ゲーム開始（カウントダウン）→ プレイ中 → ゲームクリア」の3つの状態を
/// 順番に切り替えながらゲームを進行させる。
/// </summary>
public class InGameManager : MonoBehaviour
{
    // ゲームの状態を表す列挙型（enum）。
    // GameStart = カウントダウン中, Playing = プレイ中, GameClear = クリア済み
    private enum GameState
    {
        GameStart,
        Playing,
        GameClear,
        GameOver
    }

    // 現在のゲーム状態。最初は GameStart（カウントダウン中）にしておく
    private GameState currentState = GameState.GameStart;

    // カウントダウン表示を制御するコンポーネント（インスペクターで割り当てる）
    public CountDownController CountDownController;

    // プレイヤーの移動を制御するコンポーネント（インスペクターで割り当てる）
    public PlayerController PlayerController;

    // タイマーを制御するコンポーネント（インスペクターで割り当てる）
    public TimerController TimerController;

    // ゲームクリア時に表示するテキストUI（インスペクターで割り当てる）
    public TMP_Text GameClearText;

    // ゲームクリア後の結果表示テキスト（インスペクターで割り当てる）
    public TMP_Text ResultText;

    // ゲームオーバー時に表示するテキストUI（インスペクターで割り当てる）
    public TMP_Text GameOverText;

    /// <summary>
    /// 最初に一度だけ呼ばれるメソッド。
    /// ゲームクリアテキストを非表示にしてからゲーム開始状態に移行する。
    /// </summary>
    private void Start()
    {
        // 最初はゲームクリアテキストを非表示にする
        GameClearText.gameObject.SetActive(false);
        // 最初はゲームオーバーテキストを非表示にする
        GameOverText.gameObject.SetActive(false);

        // ゲーム開始状態（カウントダウン）に切り替える
        ChangeState(GameState.GameStart);
    }

    /// <summary>
    /// ゲームの状態を切り替えるメソッド。
    /// 新しい状態に応じた処理を実行する。
    /// </summary>
    /// <param name="newState">切り替え先の状態</param>
    private void ChangeState(GameState newState)
    {
        // 現在の状態を更新する
        currentState = newState;

        // 新しい状態に合わせて処理を分岐する
        switch (currentState)
        {
            case GameState.GameStart:
                // カウントダウンコルーチンを開始する
                // StartCoroutine でコルーチンを実行すると、他の処理と並行して動かせる
                StartCoroutine(GameStart());
                break;

            case GameState.Playing:
                // ゲームプレイの処理をここに追加
                // プレイヤーが動けるようにする
                PlayerController.CanMove = true;
                // タイマーのカウントを開始する
                TimerController.StartTimer();
                break;

            case GameState.GameClear:
                // ゲームクリアの処理をここに追加
                // プレイヤーを動けないようにする
                PlayerController.CanMove = false;
                // タイマーを止める
                TimerController.StopTimer();
                // ゲームクリアテキストを表示する
                GameClearText.gameObject.SetActive(true);
                // プレイヤーを完全に固定して動かなくする
                PlayerController.FreezePlayer();
                // タイムを小数点以下2桁で表示する
                ResultText.text = $"Time: {TimerController.CurrentTime.ToString("F2")}s";
                break;
            
            case GameState.GameOver:
                // ゲームオーバーの処理をここに追加
                // プレイヤーを動けないようにする
                PlayerController.CanMove = false;
                // タイマーを止める
                TimerController.StopTimer();
                // ゲームオーバーテキストを表示する
                GameOverText.gameObject.SetActive(true);
                // プレイヤーを完全に固定して動かなくする
                PlayerController.FreezePlayer();
                break;
        }
    }

    /// <summary>
    /// 毎フレーム呼ばれるメソッド。
    /// プレイ中のみ、残りアイテム数を確認してすべて取得したらクリアにする。
    /// </summary>
    private void Update()
    {
        // プレイ中のときだけアイテム数を確認する
        if (currentState == GameState.Playing)
        {
            // シーン内で "Item" タグが付いたオブジェクトの数を調べる
            int itemCount = GameObject.FindGameObjectsWithTag("Item").Length;

            // アイテムが0個になったらゲームクリア状態に切り替える
            if (itemCount == 0)
            {
                ChangeState(GameState.GameClear);
            }

            // プレイヤーが特定の高さ以下に落ちたらゲームオーバー状態に切り替える
            if (PlayerController.transform.position.y < -10f)
            {
                ChangeState(GameState.GameOver);
            }
        }
    }

    /// <summary>
    /// ゲーム開始処理を行うコルーチン。
    /// カウントダウンが終わるまで待ってからプレイ状態に移行する。
    /// </summary>
    private IEnumerator GameStart()
    {
        // CountDownController のカウントダウンが完了するまでここで待機する
        // yield return StartCoroutine(...) で別のコルーチンの終了を待てる
        yield return StartCoroutine(CountDownController.StartCountDown());

        // カウントダウンが終わったらプレイ中状態に切り替える
        ChangeState(GameState.Playing);
    }
}
