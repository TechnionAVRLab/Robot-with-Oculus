#include <Servo.h> 
#define SERVO_NUM 24
#define MAX_PORT 45
#define LEFT_SHOULDER_RAISE 0
#define LEFT_SHOULDER_ROTATE 1
#define LEFT_SHOULDER_TWIST 2
#define LEFT_ELBOW 3
#define RIGHT_SHOULDER_RAISE 4
#define RIGHT_SHOULDER_ROTATE 5
#define RIGHT_SHOULDER_TWIST 6
#define RIGHT_ELBOW 7
#define LEFT_HAND_WRIST 8
#define LEFT_HAND_PINKY 9
#define LEFT_HAND_RING 10
#define LEFT_HAND_MID 11
#define LEFT_HAND_INDEX 12
#define LEFT_HAND_THUMB 13
#define RIGHT_HAND_WRIST 14
#define RIGHT_HAND_PINKY 15
#define RIGHT_HAND_RING 16
#define RIGHT_HAND_MID 17
#define RIGHT_HAND_INDEX 18
#define RIGHT_HAND_THUMB 19
#define HEAD_ROLL_MINUS 20
#define HEAD_ROLL_PLUS 21
#define HEAD_YAW 22
#define HEAD_PITCH 23


Servo servos[SERVO_NUM];
int i=0;
void setup()
{
  pinMode(6, OUTPUT);
  digitalWrite(7, LOW);
  for (int j=0 ; j<SERVO_NUM ;j++)
  {
    servos[j].attach(j+(MAX_PORT-SERVO_NUM+1));
  }
  //Default pose initialization
  servos[0].write(90);
  servos[1].write(30);
  servos[2].write(90);
  servos[3].write(30);
  servos[4].write(100);
  servos[5].write(30);
  servos[6].write(90);
  servos[7].write(20);
  servos[8].write(140);
  servos[9].write(35);
  servos[10].write(35);
  servos[11].write(35);
  servos[12].write(35);
  servos[13].write(35);
  servos[14].write(35);
  servos[15].write(35);
  servos[16].write(35);
  servos[17].write(35);
  servos[18].write(35);
  servos[19].write(35);
  servos[20].write(90);
  servos[21].write(90);
  servos[22].write(100);
  servos[23].write(110);
  
  Serial.begin(115200);
}

void loop()
{

}

void serialEvent(){
  if(Serial.available()>0)
  {
    int data =  Serial.parseInt();
      switch (data){
      case 9020:
        //Serial.parseInt();
        digitalWrite(7, LOW);
        break;
      case 9021:
        //Serial.parseInt();
        digitalWrite(7, HIGH);
        break;
      case 9022:
        servos[LEFT_SHOULDER_RAISE].write(Serial.parseInt());
        break;
      case 9023:
        servos[LEFT_SHOULDER_ROTATE].write(Serial.parseInt());
        break;
      case 9024:
        servos[LEFT_SHOULDER_TWIST].write(Serial.parseInt());
        break;
      case 9025:
        servos[LEFT_ELBOW].write(Serial.parseInt());
        break;
      case 9026:
        servos[RIGHT_SHOULDER_RAISE].write(Serial.parseInt());
        break;
      case 9027:
        servos[RIGHT_SHOULDER_ROTATE].write(Serial.parseInt());
        break;
      case 9028:
        servos[RIGHT_SHOULDER_TWIST].write(Serial.parseInt());
        break;
      case 9029:
        servos[RIGHT_ELBOW].write(Serial.parseInt());
        break;
      case 9030:
        servos[LEFT_HAND_WRIST].write(Serial.parseInt());
        break;
      case 9031:
        servos[LEFT_HAND_PINKY].write(Serial.parseInt());
        break;
      case 9032:
        servos[LEFT_HAND_RING].write(Serial.parseInt());
        break;
      case 9033:
        servos[LEFT_HAND_MID].write(Serial.parseInt());
        break;
      case 9034:
        servos[LEFT_HAND_INDEX].write(Serial.parseInt());
        break;
      case 9035:
        servos[LEFT_HAND_THUMB].write(Serial.parseInt());
        break;
      case 9036:
        servos[RIGHT_HAND_WRIST].write(Serial.parseInt());
        break;
      case 9037:
        servos[RIGHT_HAND_PINKY].write(Serial.parseInt());
        break;
      case 9038:
        servos[RIGHT_HAND_RING].write(Serial.parseInt());
        break;
      case 9039:
        servos[RIGHT_HAND_MID].write(Serial.parseInt());
        break;
      case 9040:
        servos[RIGHT_HAND_INDEX].write(Serial.parseInt());
        break;
      case 9041:
        servos[RIGHT_HAND_THUMB].write(Serial.parseInt());
        break;
      case 9042:
        servos[HEAD_ROLL_MINUS].write(Serial.parseInt());
        break;
      case 9043:
        servos[HEAD_ROLL_PLUS].write(Serial.parseInt());
        break;
      case 9044:
        servos[HEAD_YAW].write(Serial.parseInt());
        break;
      case 9045:
        servos[HEAD_PITCH].write(Serial.parseInt());
        break;
      }     
  }
}
