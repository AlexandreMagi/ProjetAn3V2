#define SERIAL_USB

#include "Arduino.h"
#include "Wire.h"
#include "DFRobotIRPosition.h"

DFRobotIRPosition myDFRobotIRPosition;

int positionX[4];     ///< Store the X position
int positionY[4];     ///< Store the Y position

void setup()
{
  Serial.begin(250000);

  pinMode(2,INPUT_PULLUP);
  pinMode(4,INPUT_PULLUP);
  pinMode(7,INPUT_PULLUP);

  delay(100);
  
  while (!Serial);
  /*!
   *  @brief initailize the module.
   */
  myDFRobotIRPosition.begin();

  
}

void loop()
{

  InputButon();
  Serial.print("|");
  IrPosition();
  delay(5);
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

    if(pAx1 != -1 && pBx1 != -1)
    {

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
      Serial.println(finalY);
      
    } else {
      Serial.println("Out of range");
    }
  }
  else{
    Serial.println("Device not available!");
  }

}
