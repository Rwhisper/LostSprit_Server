# LostSprit_Server

C# 소켓 통신 프로그램
- 멀티플레이 게임을 위한 서버
- 비동기 방식 사용하여 데이터 통신관리
  * 
  * ![image](https://user-images.githubusercontent.com/73861946/141670455-93808b18-55bc-49f3-a4a5-a40b0806044e.png)

- 세션관리를 통해서 사용자의 정보를 관리한다.
- 
* session 이라는 클래스에 통신을 위한 메소드 정의 
* 
* 서버는 ClientSession, 클라이언트는 ServerSession 이라는 클래스에 session 클래스를 상속받아서 각자 접속한 유저와 서버와의 통신을 관리한다.
* 

- xml 파일에 패킷 구조를 정의하고 xml파일을 읽어들여 미리 정의 한 string Format형식으로 패킷을 정의하고 읽고 쓸수있는 클래스 생성
- 



MySQL을 사용한 DB연결
- 랭킹, 로그인을 위한 DataBase연결

