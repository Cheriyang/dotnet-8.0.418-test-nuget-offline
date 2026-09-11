# .NET SDK 8.0.418 / net8.0 离线测试与 Blazor 前后端分离依赖

本仓库提供可直接用于离线 NuGet 源的 **58 个原始 `.nupkg` 文件**，并附带独立 Blazor WebAssembly 前端、ASP.NET Core API 后端和验证测试。

独立包位于 [`packages/`](packages/)；完整 ZIP 及其 SHA-256 文件位于 [最新 Release](https://github.com/Cheriyang/dotnet-8.0.418-test-nuget-offline/releases/latest)。目标框架为 **net8.0**，验证 SDK 固定为 **8.0.418**，ASP.NET Core / 运行时版本固定为该 SDK 自带的 **8.0.24**。

## 包名与版本

| 用户需求 | 本合集实际采用 | 说明 |
| --- | --- | --- |
| microsoft.net.test.sdk.17.12+ | Microsoft.NET.Test.Sdk **17.14.1** | 采用 17.x 稳定版，已实际运行测试 |
| xunit.2.9+ | xunit **2.9.3** | 采用 xUnit v2；不混用 xunit.v3 |
| 测试适配器 | xunit.runner.visualstudio **3.1.5** | 支持运行上述 xUnit v2 测试 |
| Microsoft.AspNetCore.Components.WebAssembly | Microsoft.AspNetCore.Components.WebAssembly **8.0.24** | 独立浏览器前端 |
| WebAssembly.DerServer | Microsoft.AspNetCore.Components.WebAssembly.DevServer **8.0.24** | 正确拼写为 DevServer，开发服务器 |
| WebAssembly.Server | Microsoft.AspNetCore.Components.WebAssembly.Server **8.0.24** | ASP.NET Core 可选 WASM 静态资源托管支持 |
| Microsoft.AspNetCore.SingleR | Microsoft.AspNetCore.SignalR.Client **8.0.24** | 正确拼写为 SignalR；服务端由 Microsoft.AspNetCore.App 共享框架提供 |
| Microsoft.NetCore.App.Runtime | Microsoft.NETCore.App.Runtime.Mono.browser-wasm **8.0.24** | 浏览器所需 Mono/WASM 运行时包 |
| Microsoft.NetCore.App.Runtime | Microsoft.NETCore.App.Runtime.win-x64 **8.0.24** | Windows x64 后端自包含发布所需运行时包 |

`Microsoft.NETCore.App.Runtime` 需要具体平台后缀，不能作为一个通用运行时包来安装。运行时包属于 SDK 解析的 runtime pack，不应随意作为普通业务 `PackageReference` 添加。

合集还包含 `Microsoft.NET.Sdk.WebAssembly.Pack`、`Microsoft.NET.ILLink.Tasks`、各项目的传递依赖，以及实际还原时由 SDK 请求的 ASP.NET Core / Windows Desktop Windows x64 运行时包。后者不表示前后端项目依赖桌面 UI。

## 升级后空间对比

| 范围 | 包数 | 原始 nupkg 合计 |
| --- | ---: | ---: |
| v1 旧测试合集 | 88 | 48,729,653 bytes |
| v2 新测试依赖闭包 | 15 | 21,819,215 bytes |
| v2 测试 + Blazor + SignalR + 运行时完整合集 | 58 | 155,160,461 bytes |

新版**测试依赖本身减少约 55.2%**，`NETStandard.Library` 及旧平台依赖链已从当前合集移除。完整合集增加了浏览器、Windows 等运行时，因此不能将其总大小与旧版仅测试包的大小直接比较。

## 离线还原、测试和发布

完整解压 Release ZIP，在根目录执行。机器需已安装 **SDK 8.0.418**；`global.json` 禁止自动滚动到其他 SDK。

```powershell
dotnet --version
dotnet restore sample/Tests/Tests.csproj --configfile NuGet.Config --locked-mode -p:NuGetAudit=false
dotnet restore sample/Client/Client.csproj --configfile NuGet.Config --locked-mode -p:NuGetAudit=false
dotnet restore sample/Server/Server.csproj --configfile NuGet.Config --locked-mode -p:NuGetAudit=false
dotnet restore sample/IntegrationTests/IntegrationTests.csproj --configfile NuGet.Config --locked-mode -p:NuGetAudit=false

dotnet test sample/Tests/Tests.csproj --no-restore
dotnet test sample/IntegrationTests/IntegrationTests.csproj --no-restore
dotnet publish sample/Client/Client.csproj --no-restore -c Release -o artifacts/client
dotnet publish sample/Server/Server.csproj --no-restore -c Release -r win-x64 --self-contained true -o artifacts/server
```

`NuGet.Config` 清除在线源并使用本地 `packages/`。`NuGetAudit=false` 用于离线操作，避免查询在线漏洞数据库，不代表进行了漏洞审计。验证使用空的独立 NuGet 缓存。

本合集支持普通 Blazor WebAssembly 构建/发布；未包含可选 `wasm-tools` AOT/原生优化工作负载。发布提示建议安装该工作负载时，普通非 AOT 发布仍可成功完成。其他目标框架、Linux/macOS 自包含发布或其他包版本不属于本次离线验证范围。

## 前后端分开运行

还原后，在两个终端分别运行：

```powershell
# 后端：API 和 SignalR
dotnet run --project sample/Server/Server.csproj --no-restore --urls http://127.0.0.1:5181
```

```powershell
# 前端：独立 WASM 开发服务器
dotnet run --project sample/Client/Client.csproj --no-restore --urls http://127.0.0.1:5180
```

浏览器打开 `http://127.0.0.1:5180`。前端通过 HTTP API 和 SignalR 与 `http://127.0.0.1:5181` 通信。后端明确允许该开发源的 CORS；生产部署时应修改前端后端地址、CORS 来源及 HTTPS 配置。

`Client` 和 `Server` 互不引用，可独立构建和部署；`IntegrationTests` 通过真实本地 HTTP/WebSocket 连接验证后端 API、跨域预检和 SignalR Echo 调用。浏览器交互按钮提供手工验证入口，本次自动验证不包含浏览器 UI 点击。

## 清单、校验和来源

- `packages-manifest.json`：各包版本、项目用途、下载来源、大小、SHA-256/SHA-512；也标注普通依赖图外的 SDK/runtime 下载包。
- `SHA256SUMS.txt`：所有 `.nupkg` 的 SHA-256。
- `sample/*/packages.lock.json`：精确版本依赖锁；SDK/runtime 下载包还需结合清单查看。
- `VALIDATION.txt`：实际离线还原、测试与发布验证结果。
- NuGet 官方源：<https://api.nuget.org/v3/index.json>
- [Microsoft：Blazor 使用 SignalR](https://learn.microsoft.com/aspnet/core/blazor/tutorials/signalr-blazor?view=aspnetcore-8.0)

各包保持原始内容，版权和许可证属于原发布者，请查看包内 `.nuspec` 和许可证文件。
