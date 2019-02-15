# [CHANGELOG] Webex Teams API Client for .NET

#### CHANGELOG in other language
* [日本語のCHANGELOGはこちら](https://github.com/thrzn41/WebexTeamsAPIClient/blob/master/CHANGELOG.ja-JP.md) ([Japanese CHANGELOG is here](https://github.com/thrzn41/WebexTeamsAPIClient/blob/master/CHANGELOG.ja-JP.md))


## Webex Teams API Client Changelog

> NOTE: Datetime format is `yyyy-MM-dd`.

---
### [2019-02-15] Version 1.6.2

#### [FIXED DEFECTS]
* `CopyFileDataToStreamAsync()` throws `OutOfMemoryException` when the file is too large.
* `DetectPersonIdType()` throws Exception in case of `personId == null`.

#### [OBSOLETES]
* `TeamsListResult.GetListResultEnumerator(TeamsRetry, TeamsListResultFunc)` is obsoleted. Please use TeamsRetryHandler or TeamsRetryOnErrorHandler.

---
### [2018-12-17] Version 1.6.1

#### [NEW FEATURES]

* TeamsRetryHandler/TeamsRetryOnErrorHandler feature to retry on HTTP 429 and optionally 500, 502, 503 and 504.
* Copy file data to stream feature.
* TransactionId/ListTransactionId for TeamsResultInfo.
* TeamsResourceOperation for TeamsResultInfo.
* RequestLine for TeamsResultInfo.

#### [OBSOLETES]
* `RetryExecutor` is obsoleted. Please use TeamsRetryHandler or TeamsRetryOnErrorHandler.

---
### [2018-10-12] Version 1.5.1

#### [NEW FEATURES]

* Guest Issuer feature.
* GroupResource/GroupResourceMembership feature.

---
### [2018-08-06] Version 1.4.1

#### [NEW FEATURES]

* Update/Reactivate Webhook feature.
* Group mention(All) feature in `MarkdownBuilder`.

#### [OBSOLETES]

* The following methods are obsoleted because of typo in method name.  
Please use the fixed versions. But, The obsoleted methods continue to work.
  * `MarkdownBuilder.AppendOrderdList()`
  * `MarkdownBuilder.AppendOrderdListFormat()`
  * `MarkdownBuilder.AppendUnorderdList()`
  * `MarkdownBuilder.AppendUnorderdListFormat()`

---
### [2018-06-13] Version 1.3.1

#### [NEW FEATURES]

* `TeamsListResultEnumerator` to facilitate the pagination.

---
### [2018-06-04] Version 1.2.2

#### [NEW FEATURES]
This is the initial release.  
The project is moved from `Thrzn41.CiscoSpark`.  
All the features are inherited from `Thrzn41.CiscoSpark`.

* Basic Webex Teams APIs(List/Get/Create Message, Space, etc.).
* Webex Teams Admin APIs(List/Get Event, License, etc.).
* Encrypt/Decrypt Webex Teams token in storage.
* Pagination for list APIs. Enumerator to facilitate the pagination.
* Retry-after value, Retry executor.
* Markdown builder.
* Error code, error description.
* Webhook secret validator, Webhook notification manager, Webhook event handler.
* OAuth2 helper.
* Simple Webhook Listener/Server.

---
