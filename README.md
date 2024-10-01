# AR 기반 위치 정보 및 미디어 플레이어 개발 계획

## 1. 프로젝트 개요
- **프로젝트 이름**: AR 기반 위치 정보 및 미디어 플레이어
- **목표**: 사용자가 Firebase 데이터베이스를 통해 로그인을 하고, 자신의 위치 정보를 기반으로 URL을 저장 및 불러오며, 저장된 URL에 대한 콘텐츠(웹 페이지 또는 YouTube 영상)를 AR로 재생할 수 있는 시스템을 개발.
- **주요 기능**:
  1. Firebase를 사용한 사용자 회원가입 및 로그인
  2. GPS 기반 위치 정보 수집 및 Firebase에 저장
  3. 사용자의 위치에 따른 URL 정보 리스트 불러오기
  4. 선택한 URL의 콘텐츠를 AR로 재생 (YouTube 영상의 경우 AR로 플레이)

---

## 2. 사용 기술 및 툴
- **Unity**: 게임 엔진 및 UI, AR 개발
- **Firebase**: 인증, 실시간 데이터베이스, 스토리지
- **AR Foundation**: Unity에서 AR 기능 구현
- **GPS/Location API**: 위치 기반 서비스 구현
- **YouTube API**: 영상 콘텐츠 가져오기 및 재생
- **C#**: Unity 내 스크립트 작성
- **Firebase SDK for Unity**: Firebase 기능과의 연동

---

## 3. 주요 기능별 개발 단계

### 3.1 Firebase 인증 및 회원 관리
1. **Firebase 프로젝트 생성**:
   - Firebase 콘솔에서 프로젝트 생성.
   - Authentication, Real-time Database 또는 Firestore, 그리고 Storage 활성화.

2. **Firebase Unity SDK 설치**:
   - Firebase Unity SDK 설치 및 프로젝트에 연동.
   - Firebase Authentication 기능 사용을 위한 설정.

3. **회원가입 및 로그인 UI 구현**:
   - Unity에서 사용자 입력 UI (회원가입 및 로그인)를 설계.
   - Firebase Auth API를 사용해 회원가입 및 로그인 기능 구현.
   - 로그인 성공 시, 사용자 UID를 통해 데이터베이스와 연결.

### 3.2 GPS 기반 위치 정보 수집 및 저장
1. **GPS 기능 활성화**:
   - Unity에서 `Input.location`을 사용하여 GPS 정보를 수집.
   - 사용자의 현재 위도, 경도 정보를 받아옴.
   
2. **위치 정보 저장 UI**:
   - 사용자가 URL을 입력할 수 있는 UI 구현.
   - 입력된 URL과 사용자 위치(GPS 정보)를 Firebase에 저장.
   
3. **Firebase에 데이터 저장**:
   - 사용자의 UID를 기반으로 Firebase Database 또는 Firestore에 URL과 위치 정보(위도, 경도)를 저장.
   - 데이터 구조 예시:
     ```json
     {
       "userID": {
         "locationID": {
           "latitude": 37.7749,
           "longitude": -122.4194,
           "url": "https://www.example.com"
         }
       }
     }
     ```

### 3.3 저장된 위치 정보 불러오기 및 리스트로 표시
1. **위치 오차범위 설정**:
   - 사용자의 현재 GPS 위치와 Firebase에 저장된 위치 정보를 비교.
   - 오차 범위 설정 (예: 반경 500m 이내).

2. **위치 기반 URL 리스트 불러오기**:
   - Firebase에서 사용자의 위치 오차 범위 내에 있는 URL 데이터를 필터링하여 가져옴.
   - 가져온 URL 데이터를 리스트로 UI에 표시.

3. **URL 리스트 UI 구현**:
   - Unity의 ScrollView 등을 활용하여 사용자가 URL 리스트를 볼 수 있도록 UI 설계.

### 3.4 URL 클릭 및 AR 콘텐츠 재생
1. **URL 클릭 이벤트 처리**:
   - 사용자가 리스트에서 URL을 클릭하면 해당 URL에 대한 처리를 시작.
   
2. **URL 타입 처리**:
   - 웹 페이지인 경우: 기본 웹 브라우저로 URL을 염.
   - YouTube 영상인 경우: YouTube API를 사용해 영상 URL을 받아와 처리.
   
3. **AR 콘텐츠 재생**:
   - AR Foundation을 사용하여 3D 객체 또는 Plane을 생성.
   - YouTube 영상을 해당 Plane에 재생시키는 방식으로 AR 내에서 플레이어 구현.

---

## 4. 상세 개발 일정

| 단계 | 주요 작업 | 기간 | 완료 기준 |
| ---- | -------- | ---- | --------- |
| 1 | Firebase 프로젝트 생성 및 연동 | 1주 | Firebase에서 로그인 가능 |
| 2 | 회원가입 및 로그인 기능 구현 | 1주 | Unity에서 로그인 성공 시 홈 화면 이동 |
| 3 | GPS 기능 구현 및 테스트 | 1주 | GPS 정보 출력 및 Firebase에 저장 가능 |
| 4 | URL 저장 UI 및 Firebase 데이터 저장 | 2주 | URL 및 위치 정보가 Firebase에 저장됨 |
| 5 | 위치 기반 URL 리스트 필터링 및 UI 구현 | 1주 | 위치에 맞는 URL 리스트 출력 |
| 6 | URL 클릭 이벤트 및 AR 콘텐츠 재생 | 2주 | YouTube 영상 AR로 재생 완료 |
| 7 | 테스트 및 디버깅 | 2주 | 모든 기능이 안정적으로 작동 |
| 8 | 최종 배포 및 문서화 | 1주 | 프로젝트 완료 및 문서화 |

---

## 5. 예상 문제점 및 해결 방안
- **GPS 오차 문제**: GPS의 정확도 문제로 인한 오차 범위가 클 수 있음. 이를 해결하기 위해 위치 기반 데이터 필터링에서 적절한 반경 설정 필요.
- **Firebase 데이터 처리 성능**: 많은 데이터 요청 시 Firebase 처리 성능이 저하될 수 있으므로 실시간 데이터베이스 대신 Firestore를 사용하거나 데이터 구조 최적화 필요.
- **AR 콘텐츠 재생 성능 문제**: YouTube 영상 재생 시 AR 성능 저하 가능성. 성능 최적화를 위한 객체 수 줄이기 및 비동기 처리 고려.

