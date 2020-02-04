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

  int pAx = -1;
  int pAy = -1;
  int pBx = -1;
  int pBy = -1;

  if (myDFRobotIRPosition.available()) {
    for (int i=0; i<4; i++) {

      positionX[i]=myDFRobotIRPosition.readX(i);
      positionY[i]=myDFRobotIRPosition.readY(i);

      if(positionX[i] < 1023)
      {
        if(pAx == -1)
        {
          pAx = positionX[i];
          pAy = positionY[i];
        } else {
          pBx = positionX[i];
          pBy = positionY[i];
        }
      } 
    }

    if(pAx != -1 && pBx != -1)
    {

      if(pBx < pAx)
      {
        int tmpX = pAx;
        int tmpY = pAy;

        pAx = pBx;
        pAy = pBy;
        pBx = tmpX;
        pBy = tmpY;
       
      }
      
      Serial.print(pAx);
      Serial.print(",");
      Serial.print(pAy);
      Serial.print(",");
      Serial.print(pBx);
      Serial.print(",");
      Serial.print(pBy);
      Serial.print(",");

      int deltaY = (pAy - pBy);
      int deltaX = (pAx - pBx);
      float resultRad = atan2(deltaY, deltaX);
      float resultDeg = resultRad * 180/PI;

      //resultRad = (resultRad < 0) ? (2*PI+resultRad) : resultRad;
      //resultDeg = (resultDeg < 0) ? (360+resultDeg) : resultDeg;

      int centerX = ((pBx + pAx)/2) - (1024/2);
      int centerY = ((pBy + pAy)/2) - (768/2);

      //Serial.print(" (");
      //Serial.print(centerX);
      //Serial.print(",");
      //Serial.print(centerY);
      //Serial.println(")");
      
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
