#define SERIAL_USB

#include "Arduino.h"
#include "Wire.h"
#include "DFRobotIRPosition.h"

DFRobotIRPosition myDFRobotIRPosition;

int positionX[4];     ///< Store the X position
int positionY[4];     ///< Store the Y position

unsigned long targetFrequency = 5; ///< Interval de pooling souhaité
unsigned long currentTime; ///< Temps ecoulé en ms
//unsigned long lastTime; ///< Dernier enregistrement de temps


float fTimeVibrationMax = 0.15;
float fTimeDeconteVibration = 0;
float lastTime = 0;
float dt = 0;
bool bDown = false;




void setup()
{
  


  
  Serial.begin(250000);
  Serial.setTimeout(5);
  
  pinMode(2,INPUT_PULLUP);
  pinMode(4,INPUT_PULLUP);
  pinMode(7,INPUT_PULLUP);

  pinMode(8,OUTPUT);
  
  delay(100);

  // Attends que le serial soit pret
  while (!Serial);
  /*!
   *  @brief initailize the module.
   */
  myDFRobotIRPosition.begin();

  // Initialise la memoire lastTime
  lastTime = millis();

}

void loop()
{

  //ReadUnity();
  
  InputButon();
  Serial.print("|");
  IrPosition();

  funcVibration();
  
  // Attent le vidage du buffer, pour éviter une eventuelle saturation
  Serial.flush();

  /*
  // Récupère le temps écoulé
  currentTime = millis();
  // Calcul du temps écoulé depuis la dernière boucle
  unsigned long elapsedTime = currentTime - lastTime;
  // Détermine le temps a attendre

  unsigned long timeToWait;
  
  if(targetFrequency < elapsedTime ){
    
    timeToWait = 0;
    
  }else{
    
    timeToWait = targetFrequency - elapsedTime;
    
  }
  // Mémorise la derniere mesure de temps
  lastTime = currentTime;

  funcVibration();
  // Attente
  delay(timeToWait);*/
  
  delay(5);


  //Serial.print("| Actual delay :");
  //Serial.println(timeToWait);


  

}

void ReadUnity()
{
  //char DataLu = Serial.read();
  
  String DataLu = Serial.readString();
  //Serial.println(DataLu);

  
  
  if(DataLu == ""){

    
  }else{

    delay(1000);
  }
  

  
}
void InputButon()
{
  
   funcPinDigital(2);
   Serial.print(",");
   funcPinDigital(4);
   Serial.print(",");
   funcPinDigital(7); 
  
  
}

void funcPinDigital(int iNumeroPin){

    int sensorValue = digitalRead(iNumeroPin);
    Serial.print(sensorValue, DEC);
}

void IrPosition()
{

  /*!
   *  @brief request the position
   */
  myDFRobotIRPosition.requestPosition();
  
  /*!
   *  @brief If there is data available, print it. Otherwise show the error message.
   */

  int pAx1 = -1;
  int pAy1 = -1;
  int pBx1 = -1;
  int pBy1 = -1;

  if (myDFRobotIRPosition.available()) {
    for (int i=0; i<4; i++) {

      positionX[i]=myDFRobotIRPosition.readX(i);
      positionY[i]=myDFRobotIRPosition.readY(i);

      if(positionX[i] < 1023)
      {
        if(pAx1 == -1)
        {
          pAx1 = positionX[i];
          pAy1 = positionY[i];
        } else {
          pBx1 = positionX[i];
          pBy1 = positionY[i];
        }
      } 
    }

    if(pAx1 == -1 || pBx1 == -1){

      Serial.print(-1);
  
    }else{

      CalculeDistanceEcran(pAx1,pAy1,pBx1,pBy1);
        
    }
      
    Serial.print("|");
    
    if(pAx1 != -1 && pBx1 != -1)
    {

      //delay(5000);
      if(pBx1 < pAx1)
      {
        int tmpX = pAx1;
        int tmpY = pAy1;

        pAx1 = pBx1;
        pAy1 = pBy1;
        pBx1 = tmpX;
        pBy1 = tmpY;
       
      }
      
      
      
      int deltaY = (pAy1 - pBy1);
      int deltaX = (pAx1 - pBx1);
      float resultRad = atan2(deltaY, deltaX);
      float resultDeg = resultRad * 180/PI;

      int centerX = ((pBx1 + pAx1)/2) - (1024/2);
      int centerY = ((pBy1 + pAy1)/2) - (768/2);
      
      int finalX = centerX * cos(resultRad) + centerY * sin(resultRad);
      int finalY = -centerX * sin(resultRad) + centerY * cos(resultRad);


      
      Serial.print(finalX);
      Serial.print(",");
      Serial.println(finalY); //j'ai enlever le ln
      
    } else {
      Serial.println("Out of range");
    }
  }
  else{
    Serial.println("Device not available!");
  }

}

void CalculeDistanceEcran(int P1x, int P1y,int P2x, int P2y){

  int pointX = P1x - P2x;
  int pointY = P1y - P2y;

  float delta = sqrt(sq(abs(pointX))+sq(abs(pointY)));

  if(delta > 10){ //trop proche

    Serial.print(2);
    
  }else if(delta < 10 && delta > 5){ // entre les deux GOOD!

    Serial.print(1);
    
  }else if( delta < 5){// trop loin 

    Serial.print(0);
    
  }else{

    Serial.print(-1);
    
  }

}


void funcVibration(){


  
  if(digitalRead(2) == 1 && bDown == true){
  
    fTimeDeconteVibration = fTimeVibrationMax;
    bDown = false;
  }

  if(digitalRead(2) == 0 ){

    bDown = true;
    
  }
  
  dt = (millis() - lastTime)/1000;
  lastTime = millis();
  
  if(fTimeDeconteVibration > 0){

    fTimeDeconteVibration = fTimeDeconteVibration - dt;
    analogWrite(A0, 255);
    
  }else{

    analogWrite(A0, 0);
    
  }
}
