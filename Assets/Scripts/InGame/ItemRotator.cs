using UnityEngine;

/// <summary>
/// アイテムを回転させながら上下に浮遊させるアニメーションを行うクラス。
/// 収集アイテムのゲームオブジェクトにアタッチして使う。
/// </summary>
public class ItemRotator : MonoBehaviour
{
    // 1秒あたりの回転角度（度）。大きいほど速く回る
    public float RotationSpeed = 50f;

    // 上下に浮遊する振れ幅（メートル単位）。大きいほど大きく揺れる
    public float FloatAmplitude = 0.5f;

    // 上下に浮遊する速さ（周波数）。大きいほど速く揺れる
    public float FloatFrequency = 1f;

    // ゲーム開始時のアイテムの初期位置を保存する変数
    private Vector3 initialPosition;

    /// <summary>
    /// 最初に一度だけ呼ばれるメソッド。
    /// アイテムの初期位置を記録しておく（浮遊アニメーションの基準になる）。
    /// </summary>
    private void Start()
    {
        initialPosition = transform.position;
    }

    /// <summary>
    /// 毎フレーム呼ばれるメソッド。
    /// アイテムの回転と上下浮遊を更新する。
    /// </summary>
    private void Update()
    {
        // Vector3.up（Y軸）を中心に RotationSpeed × 経過時間 分だけ回転させる
        // Space.World は「ワールド座標（シーン全体の基準）」で回転することを意味する
        transform.Rotate(Vector3.up, RotationSpeed * Time.deltaTime, Space.World);

        // Mathf.Sin は -1 〜 +1 の間を波のように繰り返す関数。
        // Time.time（ゲーム開始からの総経過時間）に FloatFrequency を掛けて速さを調整し、
        // FloatAmplitude を掛けて振れ幅を調整してから初期位置に加算することで浮遊効果を出す
        transform.position = initialPosition + Vector3.up * Mathf.Sin(Time.time * FloatFrequency) * FloatAmplitude;
    }
}
