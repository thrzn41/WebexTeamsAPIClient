# [CHANGELOG] Webex Teams API Client for .NET

#### ほかの言語のCHANGELOG
* [English CHANGELOG is here](https://github.com/thrzn41/WebexTeamsAPIClient/blob/master/CHANGELOG.md) ([英語のCHANGELOGはこちら](https://github.com/thrzn41/WebexTeamsAPIClient/blob/master/CHANGELOG.md))


## Webex Teams API Client Changelog

> NOTE: 日付形式は、`yyyy-MM-dd`です。

---
### [2018-08-06] Version 1.4.1

#### [NEW FEATURES]

* Webhookの更新と再Activate機能。
* `MarkdownBuilder`での、Group Mention(All)機能。

#### [OBSOLETES]

* メソッド名に誤記があるため、以下のメソッドを非推奨にしました。  
修正済みバージョンをご利用ください。非推奨のメソッドも継続して動作します。
  * `MarkdownBuilder.AppendOrderdList()`
  * `MarkdownBuilder.AppendOrderdListFormat()`
  * `MarkdownBuilder.AppendUnorderdList()`
  * `MarkdownBuilder.AppendUnorderdListFormat()`

---
### [2018-06-13] Version 1.3.1

#### [NEW FEATURES]

* Pagination機能を簡単にするための、`TeamsListResultEnumerator`。

---
### [2018-06-04] Version 1.2.2

#### [NEW FEATURES]
最初のリリースです。
このプロジェクトは、`Thrzn41.CiscoSpark`から移動されました。  
`Thrzn41.CiscoSpark`のすべての機能が継承されています。

* Webex Teamsの基本的なAPI(List/Get/Create Message, Spaceなど)。
* Webex TeamsのAdmin API(List/Get Event, Licenseなど)。
* ストレージに保存するTokenの暗号化と復号。
* List API用のPagination機能。Paginationを簡単にするためのEnumerator。
* Retry-after値の処理とRetry executor。
* Markdown builder。
* エラーコードや詳細の取得。
* Webhook secretの検証とWebhook notification manager、Webhook event handler。
* OAuth2 helper
* 簡易Webhookサーバ機能。

---
