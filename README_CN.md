# Testing platform

`Testing platform` 是一个轻量级的持续测试平台. 你可以使用它来进行压力测试和接口测试，其他测试正在开发中。你可以使用 `Callback` 功能把自动化测试集成到你的 CICD 工具中（如 jenkins）。你只需要 docker 就能使用上述功能。

## 语言

[English Doc](README.md)

## Features

- 压力测试

  - [x] wrk
  - [ ] apache benchmark
  - [ ] JMeter

- 接口测试

  - [x] Postman

- 界面测试

  - [ ] selenium
  
- 代理终端

  - [x] 服务器监控

- 定时任务

  - [x] 测试计划定时任务

- 回调

  - [x] 测试回调执行
