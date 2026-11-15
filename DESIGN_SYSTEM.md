# Operations UI 디자인 시스템

`Operations.UI`는 제품 화면과 카탈로그가 함께 사용하는 WPF 디자인 시스템이다. 새 화면은 먼저 의미 토큰과 공통 컨트롤을 사용하고, 카탈로그에서 구성 요소의 목적과 상태를 확인한다.

## 설계 원칙

- 각 구성 요소는 목적과 우선순위가 분명해야 한다.
- 상태는 색상에만 의존하지 않는다. 문구, 모양, 아이콘, 동작을 함께 사용한다.
- 간격과 계층 구조로 관계를 표현하고, 중복 테두리와 장식용 카드를 줄인다.
- 포커스와 키보드 동작을 마우스 동작과 동등하게 다룬다.
- 구성 요소별 기본, hover, pressed, keyboard focus, disabled, busy, error 상태를 설계한다.

## 토큰

### 색상

| 역할 | 토큰 | 사용 |
| --- | --- | --- |
| 화면 배경 | `CanvasColor` | 기본 작업 영역 |
| 표면 | `SurfaceColor`, `SurfaceRaisedColor`, `SurfaceHoverColor` | 콘텐츠 계층과 상호작용 표면 |
| 경계 | `StrokeColor` | 입력 경계와 실제 구분선 |
| 텍스트 | `TextPrimaryColor`, `TextSecondaryColor`, `TextDisabledColor` | 본문, 보조 정보, 비활성 정보 |
| 주요 동작 | `AccentColor`, `AccentHoverColor`, `AccentPressedColor` | 주요 동작 및 선택 |
| 상태 | `SuccessColor`, `WarningColor`, `DangerColor`, `InfoColor` | 의미가 있는 상태에만 사용 |
| 상태 표면 | `SuccessSurfaceColor`, `WarningSurfaceColor`, `ErrorSurfaceColor` | 상태 알림 배경 |
| 키보드 포커스 | `FocusColor` | 조작 요소의 외곽선 |

색상 토큰은 `Operations.UI/Themes/Tokens.xaml`에서 관리한다. 상태 색은 상태명과 아이콘 또는 형태를 함께 표현한다. 위험 동작을 색만으로 암시하지 말고 동작 이름을 명확히 쓴다.

### 글꼴, 간격, 크기

| 역할 | 토큰 | 값 |
| --- | --- | ---: |
| 페이지 제목 | `FontPageTitle` | 26 |
| 섹션 제목 | `FontSectionTitle` | 18 |
| 카드 제목 | `FontCardTitle` | 15 |
| 본문 | `FontBody` | 14 |
| 보조 설명 | `FontSupporting` | 13 |
| 캡션 및 라벨 | `FontCaption` | 12 |
| 지표 값 | `FontMetricValue` | 28 |
| 아이콘 동작 글리프 | `FontActionGlyph` | 17 |
| 간격 | `Space1`–`Space16` | 4px 기반 배수 |
| 컨트롤 모서리 | `RadiusControl` | 4px |
| 표면 모서리 | `RadiusSurface` | 6px |
| 상태 배지 모서리 | `RadiusPill` | 12px |
| 표준 조작 높이 | `ControlHeight`, `TouchTargetHeight` | 44px |
| 버튼 높이 | `ButtonHeightCompact/Standard/Large` | 36/44/52px |
| 포커스 외곽선 | `FocusBorderThickness` | 2px |

작은 글씨는 보조 정보로 제한한다. 숫자와 단위는 가까이 배치하고, 데이터 표에서는 본문보다 큰 대비를 사용한다. 예외적인 크기나 색을 추가하기 전에 토큰을 확장한다.

## 구성 요소

### 버튼

`OperationButton.Variant`는 `Primary`, `Standard`, `Success`, `Warning`, `Danger`를 제공한다. `Size`는 `Compact`(36), `Standard`(44), `Large`(52)이며 기본 크기는 `Standard`다. 한 화면의 주요 동작은 하나의 Primary 버튼으로 강조한다. 위험 동작은 구체적인 라벨을 사용한다.

```xml
<ui:OperationButton Content="저장" Variant="Primary" Size="Standard" />
<ui:OperationButton Content="저장 중" Variant="Primary" IsBusy="True" BusyContent="저장 중" />
```

기본, hover, pressed, keyboard focus, disabled, busy 상태를 확인한다. Busy 상태는 진행 문구와 반복 실행 차단을 제공한다. 아이콘 전용 버튼, 로딩 성공·실패 후속 패턴은 카탈로그와 라이브러리 확장 항목이다.

### 텍스트

페이지 제목, 섹션 제목, 카드 제목, 본문, 보조 설명, 라벨, 캡션을 구분한다. 공용 스타일 `PageTitle`, `SectionTitle`, `CardTitle`, `BodyText`, `SupportingText`, `FieldLabel`, `CaptionText`는 각 글꼴 토큰을 연결한다. 크기와 굵기를 임의로 만들지 말고 의미 계층을 사용한다. 링크, 코드, 선택 가능한 텍스트 사례는 다음 확장 항목이다.

### 입력과 선택

`FormField`는 입력 요소와 라벨, 도움말, 필수 표시, 오류 안내를 한 단위로 묶는다.

```xml
<ui:FormField Label="표시 이름" HelpText="화면에 표시할 이름입니다." IsRequired="True">
    <TextBox />
</ui:FormField>
```

주요 속성은 `Label`, `HelpText`, `ErrorMessage`, `IsRequired`, `IsInvalid`다. 오류 상태에서는 도움말 대신 오류 문구를 표시하며, 오류 문구를 접근성 도움말로 전달한다. 입력 요소의 자동화 라벨은 필드 라벨을 참조한다. 필수 여부는 텍스트로도 나타내고, 오류는 입력 경계와 문구로 함께 표현한다.

`IsInvalid`는 내부의 `TextBox`와 `ComboBox` 경계에도 오류 스타일을 적용한다. 전용 `ErrorTextBox` 같은 별도 스타일 없이 필드 상태 하나로 오류 표시를 맞춘다.

TextBox, ComboBox, CheckBox, RadioButton은 각각의 포커스, 비활성, 선택 상태를 확인한다. 날짜·시간, 검색, 숫자 범위 등 도메인 입력은 실제 사용 사례를 확인한 뒤 별도 구성 요소로 추가한다.

### 알림과 상태

정보, 성공, 주의, 오류 알림은 제목과 짧은 설명을 사용한다. 오류에는 원인과 가능한 복구 방법을 제공한다. `NotificationBanner`는 polite live region으로 동작하고 오류는 assertive로 알리며, 로드 후 제목·메시지 변경 이벤트를 보낸다. `Title`, `Message`, `Severity`, `IsDismissible` 속성을 사용하고 `DismissRequested` 이벤트를 구독한다. Toast는 짧고 비차단인 결과에만 사용하고, 사용자의 결정이 필요한 경우 대화상자를 사용한다. `StatusBadge`는 `EquipmentState`(`Offline`, `Ready`, `Running`, `Warning`, `Fault`)와 라벨로 상태를 전달한다. 실제 보조기술의 음성 안내는 제품 환경에서 별도로 검증한다.

### 지표, 목록, 표

`MetricTile`은 `Label`, `Value`, `Unit`, `Context`, `State`, `StateMessage`를 받는다. `State`는 `Value`, `Loading`, `Empty`, `Error` 중 하나다. 값의 반올림과 단위 변환은 데이터 계층에서 끝내고 타일에는 표시 문자열을 전달한다. 지표는 값, 단위, 기준 또는 변화 방향을 함께 보여준다. 표와 목록은 선택, 정렬, 빈 결과, 로딩, 오류 상태를 분리하고 숫자 정렬 및 키보드 탐색을 확인한다.

### 레이아웃과 대화상자

`WorkspaceShell`은 `NavigationContent`, `HeaderContent`, `Content`, `StatusContent` 영역과 `ShowNavigation`, `ShowStatusBar`, `NavigationWidth` 설정을 제공한다. 작업 영역은 제목, 본문, 상태, 동작의 순서를 유지한다. `DialogHost`는 `DialogContent`, `IsDialogOpen`, `CanDismissWithEscape`, `CanDismissOnBackdrop` 속성을 제공하고 `DismissRequested` 이벤트를 발생시킨다. 대화상자는 제목, 결과 설명, 취소 및 확인 동작을 구분한다. 열릴 때 초기 포커스를 설정하고 닫을 때 호출 위치로 돌려보낸다. Escape 동작과 포커스 범위는 실제 입력 검증 대상이다.

## 카탈로그 및 검증

카탈로그는 기초, 텍스트, 버튼, 입력, 알림, 상태와 지표, 목록과 표, 레이아웃, 대화상자 탭으로 구성한다. 각 탭은 사용 목적과 대표 상태를 비교할 수 있게 유지한다.

카탈로그 스냅샷은 1024×700, 1280×800, 1440×900에서 비교한다. 캡처는 정렬, 간격, 잘림, 대비를 확인하는 자료이며 마우스·키보드 조작, 화면 낭독기, 실제 DPI, 장비 및 사용자 검토를 대체하지 않는다. 완료된 검증과 미확인 항목을 구분해 기록한다.

## 다음 확장 우선순위

1. 입력의 개별 상태와 필드 오류 연결 동작을 더 넓은 사례로 검증한다.
2. 링크, 검색, 숫자 및 날짜 입력 패턴을 추가한다.
3. Toast와 대화상자의 포커스 복귀, Escape, 키보드 동작을 실제 실행 환경에서 확인한다.
4. 표의 선택·정렬·빈 결과 및 로딩·오류 상태를 구체적인 샘플로 확장한다.
5. 고대비, 실제 DPI 배율, 제품별 밝은 테마 필요성을 검토한다.
