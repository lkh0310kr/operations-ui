# Operations UI 개발 도구

- `Operations.UI`: 제품 분야와 무관하게 재사용하는 다크 우선 디자인 토큰, 컨트롤 스타일, 작업공간 구성 요소.
- `Operations.UiCatalog`: 색상, 버튼, 입력, 상태, 목록, 표, 작업공간, 대화상자를 독립 실행으로 확인하는 WPF 쇼케이스.
- `VisionApp.Dev.slnx`: UI 라이브러리와 카탈로그를 함께 여는 Visual Studio 솔루션.

카탈로그 실행:

```powershell
dotnet run --project .\Operations.UiCatalog\Operations.UiCatalog.csproj
```

현재 화면을 PNG로 저장:

```powershell
dotnet run --project .\Operations.UiCatalog\Operations.UiCatalog.csproj -- --snapshot=artifacts\ui-catalog\catalog.png
```

각 구성 요소 구간을 PNG로 저장:

```powershell
dotnet run --project .\Operations.UiCatalog\Operations.UiCatalog.csproj -- --snapshot-dir=artifacts\ui-catalog
```

`Themes/DesignSystem.xaml`은 사전을 조립하며 실제 토큰·스타일·작업공간 요소는 각각 `Tokens.xaml`, `Controls.xaml`, `Components.xaml`에 둔다. 공유 리소스 변경 후에는 카탈로그와 제품 앱을 모두 빌드하고 주요 상태 스냅샷을 확인한다.

공용 컨트롤은 `Operations.UI.Controls` 네임스페이스에서 제공한다.

- `WorkspaceShell`: 탐색, 헤더, 본문, 상태 표시 영역이 있는 단일 창 작업 레이아웃.
- `DialogHost`: 새 네이티브 창을 열지 않는 모달 오버레이. `IsDialogOpen`, `DialogContent`, 크기·여백 속성으로 제어한다. Esc 닫기를 지원하며 바깥 클릭은 설정으로 허용할 수 있다. 닫힘 요청은 `DismissRequested` 이벤트로 전달한다.
- `NotificationBanner`: 정보·성공·주의·오류 알림. 닫기 동작은 `DismissRequested` 이벤트로 소비 앱에 전달한다.

제품 앱은 `App.xaml`에서 `pack://application:,,,/Operations.UI;component/Themes/DesignSystem.xaml` 리소스를 병합해 테마와 기본 컨트롤 스타일을 적용한다.


## 디자인 원칙

구성 요소와 상태 계획은 [DESIGN_SYSTEM.md](DESIGN_SYSTEM.md), 화면 구성·카탈로그 검토 규칙은 [UI_GUIDELINES.md](UI_GUIDELINES.md)를 따른다. 기본 확인 해상도는 1024×700, 1280×800, 1440×900이다.
