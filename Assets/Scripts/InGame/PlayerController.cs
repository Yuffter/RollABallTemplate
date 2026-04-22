using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

/// <summary>
/// プレイヤー（ボール）の移動操作とアイテム収集を管理するクラス。
/// Input System を使ってキーボード/コントローラーの入力を受け取り、
/// Rigidbody（物理演算コンポーネント）に力を加えて移動させる。
/// </summary>
public class PlayerController : MonoBehaviour
{
    // プレイヤーの移動速度。大きいほど速く動く
    public float Speed = 5f;

    // Rigidbody は物理演算を担当するコンポーネント（重力・衝突など）
    private Rigidbody rb;

    // 「Move」アクションを参照する変数（Input System で定義したアクション）
    private InputAction moveAction;

    // プレイヤーが動けるかどうかのフラグ。カウントダウン中は false にしておく
    public bool CanMove = false;

    /// <summary>
    /// 最初に一度だけ呼ばれるメソッド。
    /// 必要なコンポーネントの取得と入力アクションの検索を行う。
    /// </summary>
    private void Start()
    {
        // このゲームオブジェクトにアタッチされている Rigidbody を取得する
        rb = GetComponent<Rigidbody>();

        // Input System のアクションマップから "Move" アクションを探して取得する
        moveAction = InputSystem.actions.FindAction("Move");
    }

    /// <summary>
    /// 物理演算のタイミングで呼ばれるメソッド（Update より安定した一定間隔）。
    /// プレイヤーへの入力を読み取り、Rigidbody に力を加えて移動させる。
    /// </summary>
    private void FixedUpdate()
    {
        // CanMove が false のときは何もせずに処理を抜ける（return = 即終了）
        if (!CanMove) return;

        // 入力の値を Vector2（X軸・Y軸）で読み取る
        // キーボードなら WASD や矢印キー、コントローラーなら左スティックに対応する
        Vector2 input = moveAction.ReadValue<Vector2>();

        // 2D入力（画面の横・縦）を3D空間の移動ベクトルに変換する
        // Y軸（上下）は動かさないので 0 にする
        Vector3 movement = new Vector3(input.x, 0, input.y) * Speed;

        // Rigidbody に力（Force）を加えてボールを動かす
        // AddForce はリアルな物理的な動きになる（直接位置を変えるより自然）
        rb.AddForce(movement);
    }

    /// <summary>
    /// 他のコライダー（衝突判定）に触れたとき呼ばれるメソッド。
    /// ただし「トリガー」に設定されたコライダーに触れたときのみ発火する。
    /// </summary>
    /// <param name="other">触れた相手のコライダー</param>
    private void OnTriggerEnter(Collider other)
    {
        // 触れた相手のタグが "Item" かどうかを確認する
        if (other.CompareTag("Item"))
        {
            // アイテムのゲームオブジェクトをシーンから削除する（収集）
            Destroy(other.gameObject);
        }
    }

    /// <summary>
    /// プレイヤーを完全に停止・固定するメソッド。
    /// ゲームクリア時に InGameManager から呼ばれる。
    /// </summary>
    public void FreezePlayer()
    {
        // RigidbodyConstraints.FreezeAll で位置と回転をすべて固定する
        rb.constraints = RigidbodyConstraints.FreezeAll;
    }
}
