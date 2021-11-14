# LostSprit_Server

C# 소켓 통신 프로그램
- 멀티플레이 게임을 위한 서버
- 콘솔 프로그램 + Socket 통신
 * ![image](https://user-images.githubusercontent.com/73861946/141686016-addfc4f3-3d79-4684-9e02-8766fd4b5786.png)
 * 서버 가동
- 비동기 방식 사용하여 데이터 통신관리
  * ![image](https://user-images.githubusercontent.com/73861946/141670455-93808b18-55bc-49f3-a4a5-a40b0806044e.png)
- ServerCore라는 클래스 라이브러리에 공통부분 작성
- 세션을 통해서 사용자의 정보를 관리한다.
 * session 이라는 클래스에 통신을 위한 메소드 정의 
  + ![image](https://user-images.githubusercontent.com/73861946/141685725-ff011b4b-4867-4093-80fc-24199a8f63f2.png)
  + 연결된 소켓 저장, 네트워크 통신을 위한 메서드 정의
  + SendBuffer, RecvBuffer를 사용하여 송, 수신 데이터 전송
 * 서버는 ClientSession, 클라이언트는 ServerSession 이라는 클래스에 session 클래스를 상속받아서 각자 접속한 유저와 서버와의 통신을 관리한다.
  + 세션 안에는 객체( 유저, 서버 )의 정보를 기록한다.
  + ![image](https://user-images.githubusercontent.com/73861946/141685742-1b29ea0c-ddbb-48e0-abf4-ac5323ea5472.png) 
  + ↑ 각 유저의 정보(ClientSession)


- SessionManager : 전체 세션을 의미한다. 
 * 방안 에서 일어나는 일을 제외한 모든 정보처리를 담당한다. 
  + 서버에 처음 접속하는 객체는 세션 번호를 key로 가진 Dictionary에 저장한다.
  + ![image](https://user-images.githubusercontent.com/73861946/141686030-4c5efad8-e1ac-4d65-9c2e-2ef2a02fb251.png)
  + ![image](https://user-images.githubusercontent.com/73861946/141686058-ab92a1b9-d8d4-42ad-80ff-9938e3e7d38f.png)
  + ![image](https://user-images.githubusercontent.com/73861946/141686070-a4d51af9-0a5a-43c7-900a-9ff751e6a66c.png)
  + 데이터 손실이 일어날만한 곳은 lock을 사용하여 처리한다.
- GameRoom : 방 하나를 의미한다. 
 * 방 안에는 최대 4인 까지 접속 가능하며, 순차적으로 파일을 전송하는 JobQueue를 사용(내부의 입출력은 lock을 사용하여 구현하였다.)
- xml 파일에 패킷 구조를 정의하고 xml파일을 읽어들여 미리 정의 한 string Format형식으로 패킷을 정의하고 읽고 쓸수있는 클래스 생성
 * ![image](https://user-images.githubusercontent.com/73861946/141686101-137e1e49-47fe-4e8e-92a6-da084a2a6595.png)
 * ![image](https://user-images.githubusercontent.com/73861946/141686136-8fead71b-bdc4-4daf-a44e-276b7c9c8100.png)
 * 완성된 패킷 클래스
 * ![image](https://user-images.githubusercontent.com/73861946/141686504-c75de10a-c30b-4a75-9ff9-03209b644e83.png)
 * (전체 구조는 너무 많기 때문에 생략하였습니다 - PacketGenerator: Program.cs, Server : GenPacket.cs)
 
- PacketManager : 패킷을 관리하는 클래스
 * 전체 패킷 핸들러 함수 저장과 그것을 실행하는 Action을 Dictionary에 저장
 * ![image](https://user-images.githubusercontent.com/73861946/141686327-b3286c0b-79e2-4747-b6af-4a76191b62e8.png)
 * 핸들러 추가
 * ![image](https://user-images.githubusercontent.com/73861946/141686396-658f94bd-f529-4e9c-9dab-2d0638a6aa39.png)
 * 서버로 데이터가 Recv되었을때 호출하여 패킷을 검사하는 메서드
 * ![image](https://user-images.githubusercontent.com/73861946/141686429-1642cdc7-cbff-4900-b4e7-bdf5eb94136b.png)
 * 핸들러 함수가 실행하는 함수들
 * ![image](https://user-images.githubusercontent.com/73861946/141686647-b0c967c0-a2fc-4c00-93d6-601f10c355da.png)
 * 패킷 정의 프로토콜(테스트용 패킷 포함)
 * ![image](https://user-images.githubusercontent.com/73861946/141689264-42083539-bdfc-4d09-8951-f7f98383ee29.png)


핸들러 함수까지오면 클라이언트측에서 데이터 관리한다.

MySQL을 사용한 DB연결
- 랭킹, 로그인을 위한 DataBase연결
- ![image](https://user-images.githubusercontent.com/73861946/141684849-0193b557-c41b-459b-aeea-8171b198d960.png)
- 로그인 성공 반환을 위한 메서드
- ![image](https://user-images.githubusercontent.com/73861946/141686715-a6805822-1a7d-46de-8ff8-6abfa0e81592.png)
- 회원가입
- ![image](https://user-images.githubusercontent.com/73861946/141686739-911642bc-ebd7-47db-a85f-1a762cb0a53d.png)
- 회원가입 중복 확인을 위하 메서드
- ![image](https://user-images.githubusercontent.com/73861946/141686759-b778126d-8673-467f-96ef-21e580b741fe.png)




