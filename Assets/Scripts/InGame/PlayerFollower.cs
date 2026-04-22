using UnityEngine;

/// <summary>
/// このスクリプトをカメラに付けると、カメラがプレイヤーを追い続けるようになるクラス。
/// 開始時のカメラとプレイヤーの位置の差（オフセット）を保ちながら追従する。
/// </summary>
public class PlayerFollower : MonoBehaviour
{
    // 追従するプレイヤーの Transform（インスペクターでプレイヤーを割り当てる）
    // Transform とは、ゲームオブジェクトの位置・回転・拡縮を管理するコンポーネント
    public Transform PlayerTransform;

    // カメラとプレイヤーの初期位置の差（オフセット）を保存する変数
    private Vector3 offset;

    /// <summary>
    /// 最初に一度だけ呼ばれるメソッド。
    /// カメラとプレイヤーの初期位置の差を記録しておく。
    /// </summary>
    private void Start()
    {
        // カメラの位置 - プレイヤーの位置 = 差分（オフセット）を計算して保存する
        offset = transform.position - PlayerTransform.position;
    }

    /// <summary>
    /// 毎フレーム、他のすべての Update が終わった後に呼ばれるメソッド。
    /// LateUpdate を使うことで、プレイヤーが移動し終えてからカメラを追従できる。
    /// </summary>
    private void LateUpdate()
    {
        // プレイヤーの現在位置 + 初期オフセット = カメラが移動すべき位置
        transform.position = PlayerTransform.position + offset;
    }
}
