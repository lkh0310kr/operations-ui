# Operations UI

`Operations.UI`는 WPF 앱에서 일관된 화면과 상태를 만들기 위한 범용 디자인 시스템 라이브러리다. 의미 토큰, 재사용 컨트롤, 상세 구성 요소 카탈로그를 함께 제공한다.

- `Operations.UI`: 테마, 토큰, 공용 입력·상태·레이아웃 컨트롤.
- `Operations.UiCatalog`: 구성 요소를 목적별 탭과 상태별 사례로 살펴보는 독립 실행 카탈로그.
- `VisionApp.Dev.slnx`: 라이브러리와 카탈로그를 함께 여는 Visual Studio 솔루션.

## 시작

```powershell
dotnet run --project .\Operations.UiCatalog\Operations.UiCatalog.csproj
```

상단의 테마 버튼으로 다크·라이트 테마를 바꿔 비교한다. 다른 앱에서는 `App.xaml`에서 `Themes/DesignSystem.xaml` 또는 `Themes/DesignSystem.Light.xaml` 중 하나를 병합한다. 런타임 전환과 토큰 규칙은 [DESIGN_SYSTEM.md](DESIGN_SYSTEM.md)를 참고한다.

## 카탈로그 캡처

현재 선택한 화면을 PNG로 저장한다.

```powershell
dotnet run --project .\Operations.UiCatalog\Operations.UiCatalog.csproj -- --size=1280x800 --snapshot=catalog.png
```

다크 테마의 모든 구성 요소 상태를 저장한다.

```powershell
dotnet run --project .\Operations.UiCatalog\Operations.UiCatalog.csproj -- --size=1280x800 --snapshot-dir=artifacts\ui-catalog\dark
```

라이트 테마는 `--theme=light`를 추가한다. 지원 비교 크기는 1024×700, 1280×800, 1440×900이다. 카탈로그 캡처는 시각 검토 자료이며 실제 입력·보조기술·장비 확인을 대체하지 않는다.

## 공용 컨트롤

`Operations.UI.Controls` 네임스페이스에서 제공한다.

- `OperationButton`, `OperationSwitch`: 동작 우선순위·크기·처리 중 상태, 선택형 on/off.
- `FormField`: 입력 라벨·도움말·필수·오류 상태.
- `NotificationBanner`, `StatusBadge`: 의미가 있는 알림과 상태 표시.
- `MetricTile`, `DataStatePanel`: 지표 값과 데이터 영역의 로딩·빈 결과·오류 상태.
- `WorkspaceShell`, `DialogHost`: 재사용 작업공간과 모달 대화상자 패턴.

토큰과 구성 요소 상태 표는 [DESIGN_SYSTEM.md](DESIGN_SYSTEM.md), 화면 구성과 검토 원칙은 [UI_GUIDELINES.md](UI_GUIDELINES.md)를 따른다.
