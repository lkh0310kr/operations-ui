# WPF UI 개발 도구

- `Operations.UI`: 제품 분야와 무관하게 재사용하는 디자인 리소스와 공통 WPF 요소.
- `Operations.UiCatalog`: 리소스와 컨트롤 상태를 독립 실행으로 확인하는 개발용 쇼케이스.
- `VisionApp.Dev.slnx`: 제품 앱, UI 라이브러리, 카탈로그를 함께 여는 Visual Studio 솔루션.

카탈로그 실행:

```powershell
dotnet run --project .\vision-app-ui\Operations.UiCatalog\Operations.UiCatalog.csproj
```

현재 화면을 PNG로 저장:

```powershell
dotnet run --project .\vision-app-ui\Operations.UiCatalog\Operations.UiCatalog.csproj -- --snapshot=artifacts\ui-catalog\catalog.png
```

공유 리소스 변경 후에는 카탈로그와 제품 앱을 모두 빌드하고 PNG 상태를 확인한다.
