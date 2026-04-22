# RollABallTemplate

初心者向けのチーム開発用 Unity テンプレートです。  
「ボールを転がしてアイテムを全部集めるゲーム」を題材に、**チームで分担しやすいプログラムの書き方**と**ゲームの組み立て方**を学べます。

---

## ゲームの概要

| 項目 | 内容 |
|------|------|
| Unity バージョン | 6.0.3 |
| レンダリング | Universal Render Pipeline (URP) |
| ジャンル | 3D コレクション（ボールころがし） |

**遊び方**: WASD または矢印キーでボールを動かし、フィールド上のアイテムを全部集めるとクリア。クリアタイムが表示される。

---

## ゲームの流れ

```
[ Title シーン ]
  スタートボタンを押す
        ↓
[ InGame シーン ]
  ┌─────────────────────────────────────────┐
  │ GameStart  3 → 2 → 1 → Go!（プレイヤー動けない）│
  │      ↓                                  │
  │ Playing    ボールを動かしてアイテムを収集       │
  │      ↓ アイテムが 0 個になったら              │
  │ GameClear  タイム表示・プレイヤー固定          │
  └─────────────────────────────────────────┘
```

---

## プロジェクト構造

```
Assets/
├── Scripts/
│   ├── Title/
│   │   └── TitleManager.cs          # タイトル画面：シーン遷移
│   └── InGame/
│       ├── InGameManager.cs         # ゲーム全体の状態管理（中心的な役割）
│       ├── PlayerController.cs      # プレイヤーの移動・アイテム収集
│       ├── PlayerFollower.cs        # カメラのプレイヤー追従
│       ├── CountDownController.cs   # 開始カウントダウン
│       ├── TimerController.cs       # タイマーの計測と表示
│       └── ItemRotator.cs           # アイテムの回転・浮遊アニメーション
├── Scenes/
│   ├── Title.unity                  # タイトル画面シーン
│   └── InGame.unity                 # ゲームプレイシーン
├── Prefabs/
│   └── Item.prefab                  # 収集アイテム（再利用可能なテンプレート）
└── Materials/
    ├── Floor.mat / Item.mat / Player.mat
```

---

## スクリプト解説

### TitleManager.cs — タイトル画面

**役割**: スタートボタンが押されたら InGame シーンに切り替える。

```csharp
// ボタンが押されたときのメソッドを登録する
StartButton.onClick.AddListener(OnStartButtonClicked);

// シーンを切り替える
SceneManager.LoadScene("InGame");

// オブジェクト破棄時にリスナーを解除する（メモリリーク対策）
StartButton.onClick.RemoveListener(OnStartButtonClicked);
```

> **ポイント**: `AddListener` で登録したメソッドは `OnDestroy` で `RemoveListener` を使って必ず解除する。解除しないと、オブジェクトが消えた後も参照が残り続ける「メモリリーク」が起きる。

---

### InGameManager.cs — ゲーム状態管理

**役割**: ゲーム全体の進行を「状態（State）」で管理する。各スクリプトを調整する司令塔。

```csharp
private enum GameState { GameStart, Playing, GameClear }
```

`ChangeState()` を呼ぶと状態が切り替わり、それぞれの状態で必要な処理が動く。

| 状態 | 処理内容 |
|------|----------|
| `GameStart` | カウントダウン開始。プレイヤーは動けない |
| `Playing` | プレイヤー移動を許可・タイマー開始 |
| `GameClear` | プレイヤー固定・タイマー停止・クリア表示 |

```csharp
// Update() で毎フレーム "Item" タグのオブジェクトを数え、0になったらクリア
int itemCount = GameObject.FindGameObjectsWithTag("Item").Length;
if (itemCount == 0) ChangeState(GameState.GameClear);
```

> **ポイント（ステートマシン）**: 状態を enum で管理することで、「今は何をすべきか」がコードを見ただけで分かる。複雑な条件分岐のかわりに `switch` で状態ごとの処理をまとめるのがコツ。

---

### PlayerController.cs — プレイヤー移動・アイテム収集

**役割**: キーボード/コントローラーの入力を受け取り、ボールを物理的に動かす。アイテムに触れたら収集する。

```csharp
// FixedUpdate で物理演算（AddForce）を使って移動させる
Vector2 input = moveAction.ReadValue<Vector2>();
Vector3 movement = new Vector3(input.x, 0, input.y) * Speed;
rb.AddForce(movement);

// アイテムに触れたら削除（収集）
void OnTriggerEnter(Collider other) {
    if (other.CompareTag("Item")) Destroy(other.gameObject);
}
```

> **ポイント**: 物理演算は `FixedUpdate()` で行う。`Update()` はフレームレートに依存するが、`FixedUpdate()` は一定間隔で呼ばれるため、移動速度が環境によってバラつかない。

---

### PlayerFollower.cs — カメラ追従

**役割**: カメラをプレイヤーに追従させる。

```csharp
// Start() で最初の距離（オフセット）を記録する
offset = transform.position - PlayerTransform.position;

// LateUpdate() でプレイヤー位置 + オフセット = カメラの位置を毎フレーム更新
transform.position = PlayerTransform.position + offset;
```

> **ポイント**: `LateUpdate()` はその フレームの全 `Update()` が終わった後に呼ばれる。プレイヤーが動き終えてからカメラを追従させるため、カメラが「ぶれる」現象を防げる。

---

### CountDownController.cs — カウントダウン

**役割**: ゲーム開始前に「3, 2, 1, Go!」と表示する。コルーチンで非同期処理のパターンを学べる。

```csharp
public IEnumerator StartCountDown() {
    while (currentCountDownTime > 0) {
        CountDownText.text = currentCountDownTime.ToString();
        yield return new WaitForSeconds(1f); // 1秒待機（他の処理はブロックしない）
        currentCountDownTime--;
    }
    CountDownText.text = "Go!";
    yield return new WaitForSeconds(1f);
    CountDownText.gameObject.SetActive(false);
}
```

> **ポイント（コルーチン）**: `yield return new WaitForSeconds(1f)` は「1秒待ってから続きを実行」という意味。`Thread.Sleep` と違い、ゲーム全体を止めずに待機できる。`IEnumerator` を返すメソッドがコルーチン。

---

### TimerController.cs — タイマー

**役割**: 経過時間を計測して UI に表示する。`StartTimer()` / `StopTimer()` で外部から制御できる。

```csharp
// Update() 内でフラグを見て時間を加算する
if (isRunning) {
    CurrentTime += Time.deltaTime;  // 前フレームからの経過時間を加算
    TimerText.text = $"Time: {CurrentTime.ToString("F2")}s";
}
```

> **ポイント**: `Time.deltaTime` を毎フレーム加算することで、フレームレートに関係なく現実と同じ速さで時間が進む。

---

### ItemRotator.cs — アイテムアニメーション

**役割**: アイテムをY軸に回転させながら上下に浮遊させる。三角関数 `Mathf.Sin` を使ったアニメーションのパターン。

```csharp
// Y軸回転
transform.Rotate(Vector3.up, RotationSpeed * Time.deltaTime, Space.World);

// Mathf.Sin で -1 〜 +1 の波形を作り、上下浮遊を表現する
transform.position = initialPosition + Vector3.up * Mathf.Sin(Time.time * FloatFrequency) * FloatAmplitude;
```

> **ポイント**: `RotationSpeed` / `FloatAmplitude` / `FloatFrequency` はすべて `public` 変数なので、インスペクターから値を変えてプログラムを触らずに見た目を調整できる。

---

## チーム開発のポイント

### 1. 単一責任の原則（1スクリプト = 1役割）

| スクリプト | 担当する「1つの役割」 |
|------------|----------------------|
| `PlayerController` | プレイヤーの入力と移動 |
| `TimerController` | タイマーの計測と表示 |
| `CountDownController` | カウントダウンの演出 |
| `InGameManager` | 上記を束ねるゲーム進行 |

担当を分けることで、複数人が同じファイルを同時に編集する衝突（コンフリクト）を減らせる。

### 2. ステートマシンでゲーム進行を管理する

ゲームの「今の状況」を `enum` で表すことで、バグの原因になる「条件が複雑に絡み合う if 文」を避けられる。新しい状態を追加するときは `enum` に値を足して `ChangeState` に `case` を追加するだけ。

### 3. コルーチンで時間のかかる処理を書く

演出や待機処理を `IEnumerator` コルーチンで書くと、ゲームを止めずに「○秒待ってから次へ」という処理が書ける。`InGameManager` が `yield return StartCoroutine(CountDownController.StartCountDown())` でカウントダウン終了を待っているのがその例。

### 4. イベントリスナーは必ず解除する

`AddListener` で登録したコールバックは `OnDestroy` で `RemoveListener` する。解除しないとシーン切り替え後もオブジェクトへの参照が残り、メモリリークや意図しない動作の原因になる。

### 5. インスペクターで値を調整できるようにする

`public` 変数はインスペクターから変更できる。スピードや時間などの数値を直接コードに書かず（マジックナンバー）、`public float Speed = 5f;` のように変数にしておくと、プログラマー以外のチームメンバーもパラメータ調整しやすくなる。

---

## セットアップ

1. [Unity Hub](https://unity.com/ja/download) をインストールし、**Unity 6.0.3** を追加する
2. このリポジトリをクローン / ZIPダウンロードする
3. Unity Hub から「プロジェクトを開く」でフォルダを選択する
4. `Assets/Scenes/Title.unity` を開いて再生ボタンを押す

---

## 拡張ヒント

### アイテムを増やす
`Assets/Prefabs/Item.prefab` を InGame シーンにドラッグ＆ドロップするだけ。タグが `Item` に設定済みなので、自動で収集カウントに含まれる。

### ステートを追加する（例：ゲームオーバー）
1. `InGameManager.cs` の `GameState` enum に `GameOver` を追加する
2. `ChangeState` の `switch` に `case GameState.GameOver:` を追加して処理を書く
3. ゲームオーバー条件（例：落下検知）から `ChangeState(GameState.GameOver)` を呼ぶ

### 新しいシーンを追加する（例：リザルト画面）
1. `File > New Scene` で新しいシーンを作成して保存する
2. `File > Build Settings` でシーンを追加登録する
3. 遷移元のスクリプトで `SceneManager.LoadScene("シーン名")` を呼ぶ
