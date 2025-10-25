# CREC Web Viewerの仕組み解説（初心者向け）

このドキュメントでは、CREC Web Viewerがどのように動作しているかを、Webアプリ初心者の方にも分かりやすく図解付きで説明します。

## 目次
1. [全体の仕組み](#1-全体の仕組み)
2. [ファイル構成](#2-ファイル構成)
3. [データの流れ](#3-データの流れ)
4. [各部分の役割](#4-各部分の役割)
5. [実際の動作例](#5-実際の動作例)

---

## 1. 全体の仕組み

### 基本的な構造

```
┌─────────────────────────────────────────────────────────────┐
│                     CREC Web Viewer                          │
│                                                               │
│  ┌──────────────┐         ┌──────────────┐                 │
│  │   フロント   │ ←─HTTP─→│  バックエンド │                 │
│  │   エンド     │         │              │                 │
│  │  (HTML/JS)   │         │ (ASP.NET Core)│                 │
│  └──────────────┘         └──────┬───────┘                 │
│       ↑                           │                          │
│       │                           ↓                          │
│  ┌────┴──────┐            ┌──────────────┐                 │
│  │ ブラウザ   │            │ CRECデータ    │                 │
│  │ (Chrome等) │            │ (index.txt等) │                 │
│  └───────────┘            └──────────────┘                 │
└─────────────────────────────────────────────────────────────┘
```

### 簡単に言うと...

1. **ブラウザ（あなたのChrome等）**: データを見る画面を表示
2. **フロントエンド（HTML/JavaScript）**: 画面のデザインと動き
3. **バックエンド（ASP.NET Core）**: データを読み込んで送る係
4. **CRECデータ**: あなたが管理している在庫データ

---

## 2. ファイル構成

プロジェクトは以下のように整理されています：

```
CREC_WebViewer/
│
├── wwwroot/                    ← ブラウザで見る部分
│   ├── index.html             （画面のレイアウト）
│   └── app.js                 （画面の動き）
│
├── Controllers/               ← データを送る係
│   └── ApiController.cs       （APIの処理）
│
├── Services/                  ← データを読む係
│   └── CrecDataService.cs     （ファイル読み込み）
│
├── Models/                    ← データの形を定義
│   └── CollectionData.cs      （コレクションデータの構造）
│
└── Program.cs                 ← アプリの起動設定
```

### 各ファイルの役割

| ファイル名 | 役割 | 例え |
|-----------|------|------|
| `index.html` | 画面のレイアウト | レストランのメニュー表 |
| `app.js` | 画面の動き | 注文を取るウェイター |
| `ApiController.cs` | データを渡す | 厨房への注文伝達 |
| `CrecDataService.cs` | データを読む | 倉庫から材料を取る |
| `CollectionData.cs` | データの形 | 料理のレシピカード |

---

## 3. データの流れ

### 3.1 起動時の流れ

```
     [1]                [2]                [3]                [4]
ユーザーが実行 → CREC_WebViewerが → データフォルダを → ブラウザを開いて
                起動               読み込み準備      画面を表示

┌──────────┐    ┌──────────┐    ┌──────────┐    ┌──────────┐
│ ダブル   │    │ サーバー │    │ フォルダ │    │ http://  │
│ クリック │───>│ 起動     │───>│ スキャン │───>│localhost │
│          │    │(5000番ポート)│ │(index.txt)│   │:5000     │
└──────────┘    └──────────┘    └──────────┘    └──────────┘
```

### 3.2 検索時の流れ（詳細）

```
  ブラウザ                フロントエンド           バックエンド              データ
     │                         │                      │                    │
     │ [1]ユーザーが「電子部品」と入力                │                    │
     │─────────────────>│                      │                    │
     │                         │                      │                    │
     │                         │ [2]検索リクエスト     │                    │
     │                         │ (HTTP GET)           │                    │
     │                         │─────────────────>│                    │
     │                         │                      │                    │
     │                         │                      │ [3]ファイルを読む  │
     │                         │                      │─────────────>│
     │                         │                      │                    │
     │                         │                      │ [4]データを返す    │
     │                         │                      │<─────────────│
     │                         │                      │                    │
     │                         │ [5]JSON形式で返答    │                    │
     │                         │<─────────────────│                    │
     │                         │                      │                    │
     │ [6]画面に結果を表示      │                      │                    │
     │<─────────────────│                      │                    │
     │                         │                      │                    │
```

### ステップ説明

1. **ユーザー入力**: ブラウザの検索ボックスに「電子部品」と入力
2. **HTTPリクエスト**: JavaScriptが`/api/collections/search?searchText=電子部品`にリクエスト送信
3. **データ読み込み**: バックエンドが`index.txt`ファイルを読む
4. **データ抽出**: 「電子部品」を含むデータを見つける
5. **JSON返答**: 見つかったデータをJSON形式で返す
6. **画面更新**: JavaScriptが結果をカード形式で表示

---

## 4. 各部分の役割

### 4.1 フロントエンド（ブラウザで動く部分）

#### index.html - 画面の骨組み
```html
<!-- 検索ボックスの例 -->
<input type="text" id="searchText" placeholder="名前、ID、タグなど">
<button onclick="searchCollections()">検索</button>

<!-- 結果表示エリア -->
<div id="collectionsGrid">
  <!-- ここにカードが表示される -->
</div>
```

#### app.js - 画面の動き
```javascript
// 検索ボタンを押したときの処理
async function searchCollections() {
    // 1. 入力された検索文字を取得
    const searchText = document.getElementById('searchText').value;
    
    // 2. バックエンドにリクエスト送信
    const response = await fetch(`/api/collections/search?searchText=${searchText}`);
    
    // 3. 返ってきたデータを取得
    const result = await response.json();
    
    // 4. 画面に表示
    displaySearchResults(result);
}
```

**ポイント**: 
- `fetch()`でバックエンドと通信
- `async/await`で非同期処理（待ち時間を効率化）
- JSONデータをHTMLに変換して表示

### 4.2 バックエンド（サーバーで動く部分）

#### ApiController.cs - APIエンドポイント
```csharp
[HttpGet("search")]
public async Task<ActionResult<SearchResult>> Search([FromQuery] SearchCriteria criteria)
{
    // 1. データサービスを呼び出し
    var result = await _crecDataService.SearchCollectionsAsync(criteria);
    
    // 2. 結果を返す
    return Ok(result);
}
```

**ポイント**:
- `[HttpGet("search")]`: このメソッドは`/api/collections/search`でアクセス可能
- `[FromQuery]`: URLのパラメータ（`?searchText=...`）を受け取る
- `Ok(result)`: データをJSON形式で返す

#### CrecDataService.cs - データ読み込み
```csharp
public async Task<List<CollectionData>> GetAllCollectionsAsync()
{
    // 1. フォルダ内のサブフォルダを取得
    var directories = Directory.GetDirectories(_dataFolderPath);
    
    // 2. 各フォルダからindex.txtを読む
    foreach (var dir in directories)
    {
        var indexFile = Path.Combine(dir, "index.txt");
        var lines = await File.ReadAllLinesAsync(indexFile);
        
        // 3. データを解析
        // CollectionName,電子部品サンプル
        // CollectionMC,ELEC-001
        // ...
    }
    
    return collections;
}
```

**ポイント**:
- ファイルシステムから直接読み込み
- `index.txt`の形式を解析
- データをオブジェクトに変換

---

## 5. 実際の動作例

### 例1: 画面を開く

```
ステップ1: ユーザーがCREC_WebViewerを起動
    ↓
ステップ2: Program.csがサーバーを起動（ポート5000番で待機）
    ↓
ステップ3: ブラウザが自動的に http://localhost:5000 を開く
    ↓
ステップ4: index.htmlが読み込まれる
    ↓
ステップ5: app.jsが実行され、初期データを取得
    ↓
    JavaScript: fetch('/api/collections/categories')
    ↓
    バックエンド: カテゴリ一覧を読んで返す
    ↓
    JavaScript: ドロップダウンに選択肢を表示
    ↓
ステップ6: 画面表示完了！
```

### 例2: 「電子部品」を検索する

**ユーザーの操作:**
```
1. 検索ボックスに「電子部品」と入力
2. 検索ボタンをクリック
```

**内部の処理:**
```
[フロントエンド: app.js]
┌──────────────────────────────────────┐
│ 1. 入力値を取得: "電子部品"           │
│ 2. URLを作成:                         │
│    /api/collections/search?           │
│    searchText=電子部品&page=1&        │
│    pageSize=20                        │
│ 3. HTTPリクエスト送信                 │
└────────────┬─────────────────────────┘
             │
             ↓ HTTP GET
             │
[バックエンド: ApiController.cs]
┌────────────┴─────────────────────────┐
│ 1. リクエスト受信                     │
│ 2. パラメータ解析:                    │
│    - searchText = "電子部品"          │
│    - page = 1                         │
│    - pageSize = 20                    │
│ 3. CrecDataServiceを呼び出し          │
└────────────┬─────────────────────────┘
             │
             ↓
             │
[サービス: CrecDataService.cs]
┌────────────┴─────────────────────────┐
│ 1. キャッシュをチェック               │
│ 2. なければファイルを読み込み:        │
│    - SAMPLE001/index.txt              │
│    - SAMPLE002/index.txt              │
│    - SAMPLE003/index.txt              │
│ 3. 「電子部品」を含むデータを検索     │
│    → SAMPLE001が該当！               │
│ 4. 結果をまとめる                     │
└────────────┬─────────────────────────┘
             │
             ↓ JSON
             │
[バックエンド: ApiController.cs]
┌────────────┴─────────────────────────┐
│ JSON形式でレスポンス:                 │
│ {                                     │
│   "collections": [                    │
│     {                                 │
│       "collectionID": "SAMPLE001",    │
│       "collectionName": "電子部品...", │
│       "collectionCategory": "電子部品"│
│     }                                 │
│   ],                                  │
│   "totalCount": 1                     │
│ }                                     │
└────────────┬─────────────────────────┘
             │
             ↓ HTTP Response
             │
[フロントエンド: app.js]
┌────────────┴─────────────────────────┐
│ 1. JSONデータを受信                   │
│ 2. HTMLカードを生成:                  │
│    <div class="card">                 │
│      <h6>電子部品サンプル</h6>        │
│      <p>ID: SAMPLE001</p>             │
│      ...                              │
│    </div>                             │
│ 3. 画面に表示                         │
└──────────────────────────────────────┘
             │
             ↓
        [画面更新！]
```

---

## 6. 通信の仕組み（HTTP通信）

### HTTPリクエストとは？

レストランでの注文に例えると：

```
あなた（ブラウザ）: 「電子部品のデータをください」
　　　　　　　　　　　↓ [HTTPリクエスト]
ウェイター（Web Server）: 「承知しました」
　　　　　　　　　　　↓ [処理]
料理人（バックエンド）: 「データを準備します」
　　　　　　　　　　　↓ [ファイル読み込み]
ウェイター（Web Server）: 「お待たせしました」
　　　　　　　　　　　↓ [HTTPレスポンス]
あなた（ブラウザ）: 「受け取りました」→ 画面に表示
```

### HTTPリクエストの内容

```http
GET /api/collections/search?searchText=電子部品 HTTP/1.1
Host: localhost:5000
Accept: application/json
```

**説明:**
- `GET`: データを取得するリクエスト
- `/api/collections/search`: アクセス先のパス
- `?searchText=電子部品`: 検索条件（クエリパラメータ）
- `Host: localhost:5000`: サーバーのアドレス

### HTTPレスポンスの内容

```http
HTTP/1.1 200 OK
Content-Type: application/json

{
  "collections": [
    {
      "collectionID": "SAMPLE001",
      "collectionName": "電子部品サンプル",
      ...
    }
  ],
  "totalCount": 1
}
```

**説明:**
- `200 OK`: 成功を示すステータスコード
- `Content-Type: application/json`: データ形式はJSON
- `{ ... }`: 実際のデータ（JSON形式）

---

## 7. JSON形式とは？

JSONはデータを書き表す形式です。

### 例: コレクションデータ

```json
{
  "collectionID": "SAMPLE001",
  "collectionName": "電子部品サンプル",
  "collectionCategory": "電子部品",
  "collectionTag1": "タグ１",
  "collectionTag2": "タグ２",
  "collectionCurrentInventory": 25,
  "collectionInventoryStatus": 2
}
```

**特徴:**
- `"キー": "値"` のペア
- `{ }` でオブジェクトを囲む
- `[ ]` で配列（リスト）を表す
- 人間にも機械にも読みやすい

### 配列の例

```json
{
  "collections": [
    { "collectionID": "SAMPLE001", "collectionName": "電子部品" },
    { "collectionID": "SAMPLE002", "collectionName": "書籍" },
    { "collectionID": "SAMPLE003", "collectionName": "工具" }
  ]
}
```

---

## 8. セキュリティの仕組み

### ファイルアクセスの制限

```
安全な例:
/api/files/SAMPLE001/image.jpg
    ↓
OK! SAMPLE001フォルダ内のファイルにアクセス

危険な例:
/api/files/../../../etc/passwd
    ↓
NG! 「..」を検出してブロック
```

**実装:**
```csharp
// ファイル名に危険な文字が含まれていないかチェック
if (fileName.Contains("..") || fileName.Contains("/") || fileName.Contains("\\"))
{
    return BadRequest("Invalid file name");
}
```

---

## 9. キャッシュの仕組み

### なぜキャッシュが必要？

毎回ファイルを読むと遅いので、一度読んだデータを5分間メモリに保存しておきます。

```
初回アクセス（遅い）:
ブラウザ → バックエンド → ファイル読み込み（1秒） → 返答
                             ↓
                        [キャッシュに保存]

2回目以降（速い）:
ブラウザ → バックエンド → キャッシュから取得（0.01秒） → 返答
```

**コード例:**
```csharp
// キャッシュが有効かチェック
if (_collectionsCache.Any() && DateTime.Now - _lastCacheUpdate < _cacheExpiry)
{
    return _collectionsCache; // キャッシュから返す
}

// キャッシュが古い場合は再読み込み
_collectionsCache = LoadDataFromFiles();
_lastCacheUpdate = DateTime.Now;
```

---

## 10. よくある質問

### Q1: なぜブラウザで動くの？

**A:** HTML/CSS/JavaScriptはブラウザが理解できる言語だからです。
- HTML: 構造（骨組み）
- CSS: デザイン（見た目）
- JavaScript: 動き（インタラクション）

### Q2: なぜASP.NET Coreを使うの？

**A:** 
- C#で書ける（CRECと同じ言語）
- 高速で安定している
- Windows/Linux/macOSで動く
- 長期サポートがある

### Q3: データベースは使わないの？

**A:** このアプリではファイルシステムを直接使っています。
- CRECのデータ形式（index.txt）をそのまま読める
- シンプルで分かりやすい
- 追加のインストールが不要

### Q4: ポート5000番って何？

**A:** コンピュータの「窓口」の番号です。
```
http://localhost:5000
           ↑      ↑
       コンピュータ 窓口番号
```
- 5000番: このアプリ専用の窓口
- 他のアプリは別の番号を使う（例: 80番はHTTP標準）

### Q5: どうやってデータが更新されるの？

**A:** 
1. CRECでデータを変更
2. index.txtファイルが更新される
3. 5分後にキャッシュが切れる
4. 次のアクセス時に新しいデータが読み込まれる

---

## 11. まとめ

### CREC Web Viewerの全体像

```
┌─────────────────────────────────────────────────────────────┐
│                                                               │
│  【あなたの操作】                                             │
│   ブラウザで検索                                              │
│         ↓                                                     │
│  【フロントエンド】                                           │
│   HTML/JavaScriptが動作                                       │
│   ↓ HTTPリクエスト                                           │
│  【通信】                                                     │
│   ネットワーク経由でデータ送受信                              │
│   ↓                                                          │
│  【バックエンド】                                             │
│   ASP.NET Coreが処理                                          │
│   ├─ コントローラー: リクエスト受付                          │
│   └─ サービス: データ読み込み                                │
│       ↓                                                      │
│  【データ】                                                   │
│   index.txtファイルを読む                                     │
│       ↓                                                      │
│  【返答】                                                     │
│   JSON形式でデータを返す                                      │
│       ↓                                                      │
│  【表示】                                                     │
│   ブラウザに結果を表示                                        │
│                                                               │
└─────────────────────────────────────────────────────────────┘
```

### 重要なポイント

1. **フロントエンド（HTML/JS）** = 画面の見た目と動き
2. **バックエンド（ASP.NET Core）** = データ処理
3. **HTTP通信** = フロントとバックのやり取り
4. **JSON** = データの形式
5. **ファイル読み込み** = CRECデータへのアクセス

---

## 12. さらに学ぶために

### おすすめの学習順序

1. **HTML/CSS** → 画面の作り方を学ぶ
2. **JavaScript** → 動きの付け方を学ぶ
3. **HTTP通信** → データのやり取りを学ぶ
4. **ASP.NET Core** → サーバーの作り方を学ぶ

### 参考リソース

- [MDN Web Docs（日本語）](https://developer.mozilla.org/ja/)：HTML/CSS/JavaScript
- [Microsoft Learn（日本語）](https://learn.microsoft.com/ja-jp/)：ASP.NET Core
- [HTTP通信の基礎](https://developer.mozilla.org/ja/docs/Web/HTTP/Overview)

---

このドキュメントがCREC Web Viewerの理解に役立てば幸いです！

質問があれば、GitHubのIssueやPRコメントでお気軽にお尋ねください。
