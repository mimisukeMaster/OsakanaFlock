# OsakanaFlock
<br><p align="center">
<img src="https://user-images.githubusercontent.com/81568941/188664081-43ba3a7b-7be9-4795-883f-4e51d3c2c28a.png" width="450px"></p>
<br><p align="center">
    <img src="https://img.shields.io/badge/build-passing-test.svg?&logo=github">
    [<img src="https://img.shields.io/github/stars/mimisukeMaster/OsakanaFlock?color=yellow&logo=github">](https://github.com/mimisukeMaster/OsakanaFlock/stargazers)
    [<img src="https://img.shields.io/badge/PRs-welcome-orange">](https://github.com/mimisukeMaster/OsakanaFlock/pulls)
    [<img  src="https://img.shields.io/hexpm/l/plug?color=red&logo=apache">](https://www.apache.org/licenses/)
    <img src="https://img.shields.io/badge/Unity%202020.x-made with-blue.svg?&logo=unity"><br>
    <img src="https://img.shields.io/badge/Windows-supported-succeess.svg?&logo=windows"> 
    <img src="https://img.shields.io/badge/WebGL-supported-succeess.svg?&logo=webgl">
    <img src="https://img.shields.io/github/repo-size/mimisukeMaster/OsakanaFlock?color=ff69b4">
    [<img src="https://img.shields.io/static/v1?logo=visualstudiocode&label=&message=Open%20in%20Visual%20Studio%20Code&labelColor=2c2c32&color=007acc&logoColor=007acc">](https://open.vscode.dev/mimisukeMaster/OsakanaFlock)
    </p>


## Description
Unity Youth Creator Cup 2022 のエントリー作品<br> 
PCカメラを使ってジェスチャーで魚群を動かすゲームです。
~~詳しいゲーム説明や操作等はエントリーページをご覧ください。~~
<br><p align="center">
**大会ページごと削除されてしまっていることが確認されたため、リンクが正常に機能しておりません。**
<br><br>
    [<img src="https://img.shields.io/badge/Unity Youth Creator Cup 2022-Osakana Flock-blue?style=for-the-badge&logo=unity">](https://uycc.unity3d.jp/entry/2022/154)
</p><br>

## Game feature
<p>
    <b>作品名</b>: おさかなフロック
</p>
<p>
    <b>作品紹介</b>: 仮想空間にたくさんの魚が群れで泳いでいます。
    PCのカメラで手を認識させて、ジェスチャで魚たちを美しく泳がせよう。<br>
    【PCカメラの使用を許可してください】<br>
    美しさのポイントは、群れ行動、元気度合い、スピードです。美しさに応じてスコアが上昇します。<br>
    ジェスチャで出せる指示は二つ：<br>
    1. 天敵を召喚する　→　魚たちがびっくりして群れる。<br>
    2. 餌をあげる　→　魚たちが元気をだす。<br>
    魚たちにはHPがあるので餌を食べずにいると死にます。<br>
    スコアを稼いでノルマ達成を目指そう。
</p>
<p>
    <b>ゲームのクリア条件</b>: 一定時間に規定以上のスコアを稼ぐとゲームクリア。
</p>
<p>
    <b>アピールポイント</b>: アイディア, 操作性
</p>
<p>
    <b>最大プレイ人数</b>: 1人
</p>
<p>
    <b>対応プラットフォーム</b>: PC（Windows）
</p>
<p>
    <b>推奨プレイ環境</b>: マウス, その他
</p>

## How to play
#### ■マウス
右ドラッグ：視点変更<br>
左長押し：ジェスチャ待機モード<br>

#### ■手のジェスチャ
左から右へ振り払う：天敵を召喚<br>
豆まきするように投げる動作：餌をあげる<br>

#### ■おまけ機能<br>
「おさかなビュー」ボタン：泳いでいる魚の目線で楽しめる<br>

## Environments
- Unity version<br>
Unity2020.3.6f1(2020.3.x系列)
- Modules <br>
[packages-lock.json](Packages/packages-lock.json)をご覧ください。<br>
(このファイルで必要なPackageが指定され、起動時に自動で取り込まれます。)
- GPU<br>
NVIDIAのGPUを搭載したPCで開発しています。<br>
NVIDIA GeForce GTX 1050

## derivation(派生) - 単純化プログラム紹介
このゲームの作成過程で切り離し、別リポジトリとして[FlockingBoidManipulator](https://github.com/mimisukeMaster/FlockingBoidManipulator)があります。こちらはジェスチャー機能をすべて除き、Boidアルゴリズムとその操作機能のみが実装されています。よろしければご活用ください。

## License 
*"Osakana Flock" is under [Apache License Version 2.0](https://www.apache.org/licenses/)*<br>
ライセンス概要 ([引用](https://coliss.com/articles/build-websites/operation/work/choose-a-license-by-github.html)より一部抜粋)<br>
> *Apache License 2.0<br>
  Required（必須）<br>
  著作権の表示、変更箇所の明示、ソースの明示<br>
  Permitted（許可）<br>
  商用利用、修正、配布、サブライセンス、個人利用<br>
  Forbidden（禁止）<br>
  責任免除、トレードマークの使用*<br>

## Author
みみすけ名人 mimisukeMaster <br>
[<img src="https://img.shields.io/badge/-Twitter-%231DA1F2.svg?&style=flat-square&logo=twitter&logoColor=white">](https://twitter.com/mimisukeMaster)
[<img src="https://img.shields.io/badge/-ArtStation-artstation.svg?&style=flat-square&logo=artstation&logoColor=blue&color=gray">](https://www.artstation.com/mimisukemaster)
[<img src="https://img.shields.io/badge/-Youtube-youtube.svg?&style=flat-square&logo=youtube&logoColor=white&color=red">](https://www.youtube.com/channel/UCWnmp8t4GJzcjBxhtgo9rKQ)
## References
- [Boid -wikipedia](https://ja.m.wikipedia.org/wiki/%E3%83%9C%E3%82%A4%E3%83%89_(%E4%BA%BA%E5%B7%A5%E7%94%9F%E5%91%BD))<br>
- [UnityでBoidsシミュレーションを作成してEntity Component System(ECS)を学んでみた](https://www.google.com/amp/s/tips.hecomi.com/entry/2018/12/23/200817%3famp=1)<br>
- [Unity Technologies製推論エンジン Barracudaがスゴイという話](https://qiita.com/highno_RQ/items/478e1145f0eb868c0f2e)<br>
- [Unity Barracuda やーる](https://qiita.com/SatoshiGachiFujimoto/items/739f5986f65c0d7465f0)

# Credits
From Asset Store<br>
- [Fantasy Skybox FREE](https://assetstore.unity.com/packages/2d/textures-materials/sky/fantasy-skybox-free-18353)
- [PowerUp particles](https://assetstore.unity.com/packages/vfx/particles/powerup-particles-16458)
- [Sherbb's Particle Collection](https://assetstore.unity.com/packages/vfx/particles/sherbb-s-particle-collection-170798)
- [Simple Button Set 01](https://assetstore.unity.com/packages/2d/gui/icons/simple-button-set-01-153979)
- [DOTween (HOTween v2)](https://assetstore.unity.com/packages/tools/animation/dotween-hotween-v2-27676)

A tool used from (for hand pose recognition from camera images)<br>
アセットストア以外のアセットデータ: カメラ画像からの手のポーズ認識
- [HandPoseBarracuda – Keijiro Takahashi](https://github.com/keijiro/HandPoseBarracuda)

Gesture recognition model generation<br>
- [hand-gesture-recognition-using-mediapipe – Kazuhito00](https://github.com/mimisukeMaster/hand-gesture-recognition-using-mediapipe)

Flocking-boid algorithm<br>
- [FlockingBoidManipulator](https://github.com/mimisukeMaster/FlockingBoidManipulator)

3D model of catfish<br>
- [Blue cat fish test](https://sketchfab.com/3d-models/blue-cat-fish-test-9908dcd44f41477f8504d0590de726af)

3D model of a carp<br>
- [Koi Fish](https://sketchfab.com/3d-models/koi-fish-8ffded4f28514e439ea0a26d28c1852a)

SE & BGM<br>
- [Otologic](https://otologic.jp/)
- [効果音ラボ](https://soundeffect-lab.info/)
- [DOVA-SYNDROME](https://dova-s.jp/)

Images<br>
- [いらすと・ごー](https://illustgo.com/)
- [いらすとや](https://www.irasutoya.com/)

Font<br>
- [コーポレート・ロゴ](https://logotype.jp/corporate-logo-font-dl.html)
- [国鉄っぽいフォント](http://tabi-mo.travel.coocan.jp/font.htm)
