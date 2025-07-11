ERS 專案總覽
簡介
本專案為一套 前後端分離架構系統，採用 Angular 9.x 為前端框架，後端則以 .NET 6 搭配 ABP Framework 建構，並實作 DDD (Domain-Driven Design) 模組分層設計，具備模組化、可擴充、可測試性高的特性，並支援 Docker 容器化部署。
技術架構說明

前端請讀取參考（ui.zip）檔案
框架版本：Angular 9.x
UI 套件：ng-zorro-antd
建構工具：Angular CLI

版面與模組架構：
app-routing.module.ts：路由配置
layout/：畫面主框架（Header/Sider/Content）
core/：核心服務與攔截器
shared/：共用元件與管道
common/：共用服務與資料模型
pages/、ERS-Pages/：主要功能模組頁面
ng-zorro-antd.module.ts：UI 元件整合模組
部署支援：提供 Dockerfile、nginx-custom.conf 可供部署

後端請讀取參考（api.zip）檔案
框架：.NET 6 + ABP Framework
設計模式：DDD（Domain-Driven Design）
資料存取：Entity Framework Core
身份驗證：支援 Azure AD
模組架構：
ERS.Application：應用邏輯層，服務實作
ERS.Application.Contracts：DTO 與介面定義
ERS.Domain：核心領域實體與規則
ERS.Domain.Shared：共用列舉與常數
ERS.EntityFrameworkCore：資料庫操作與遷移
ERS.HttpApi：API 控制器定義（Angular 呼叫端）
ERS.HttpApi.Client：提供 API 呼叫型別給第三方使用
ERS.Host：系統入口與主機設定（含 Program.cs）
ERS.DbMigrator：資料庫初始化與遷移工具
ERS.Job：排程作業模組

程式大致流程
前端啟動 Angular App，透過 Angular Router 進入對應頁面元件。
使用者操作介面，觸發 REST API 呼叫。
呼叫 API：前端透過 HttpClient 呼叫後端 ERS.HttpApi 控制器端點。
應用邏輯處理：後端 ERS.Application 處理業務邏輯。
資料查詢或修改：透過 ERS.EntityFrameworkCore 存取資料庫。
結果回傳前端，前端呈現畫面或提示訊息。

補充
angular.json、tsconfig.*：Angular 專案設定
appsettings.*.json：後端環境設定
Dockerfile：前端與後端皆支援容器部署
