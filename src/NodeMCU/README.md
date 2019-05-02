#  NODEMCU
## Arduino IDE
Download the Arduino Software (IDE)
Get the latest version from the https://www.arduino.cc/en/Main/Software. You can choose between the Installer (.exe) and the Zip packages. 
When the download finishes, proceed with the installation and please allow the driver installation process when you get a warning from the operating system.
Choose the components to be installed.

![The home page image is not found](https://github.com/cw-rashmi/AISCMM/blob/master/src/NodeMCU/pics/1.jpg)

* Choose the installation directory (we suggest to keep the default one)

![The home page image is not found](https://github.com/cw-rashmi/AISCMM/blob/master/src/NodeMCU/pics/2.jpg)

* The process will extract and install all the required files to execute properly the Arduino Software (IDE)

![The home page image is not found](https://github.com/cw-rashmi/AISCMM/blob/master/src/NodeMCU/pics/3.jpg)

Once the installation is finished just hit the finish button.

## Steps to Setup Arduino IDE for NODEMCU ESP8266
* Install Arduino IDE software from the link http://www.arduino.cc/en/main/software.
* Open File ->Preferences

![The home page image is not found](https://github.com/cw-rashmi/AISCMM/blob/master/src/NodeMCU/pics/4.jpg)


![The home page image is not found](https://github.com/cw-rashmi/AISCMM/blob/master/src/NodeMCU/pics/5.jpg)

* Adding ESP8266 Board ManagerIn the Additional Boards Manager enter below URL.
http://arduino.esp8266.com/stable/package_esp8266com_index.json

![The home page image is not found](https://github.com/cw-rashmi/AISCMM/blob/master/src/NodeMCU/pics/6.jpg)

The Boards Manager window opens, scroll the window page to bottom till you see the module with the name ESP8266. Once we get it, select that module and select version and click on the Install button. When it is installed it shows Installed in the module as shown in the figure and then close the window.

* Selecting ESP8266 Arduino Board

![The home page image is not found](https://github.com/cw-rashmi/AISCMM/blob/master/src/NodeMCU/pics/7.jpg)

* To run the esp8266 with Arduino we have to select the Board: “Arduino/Genuino Uno” and then change it to NodeMCU 1.0 (ESP-12E Module) or other esp8266 modules. This can be done by scrolling down, as shown in the figure.

* Connecting ESP8266 to the PC

![The home page image is not found](https://github.com/cw-rashmi/AISCMM/blob/master/src/NodeMCU/pics/8.jpg)

*  Let’s connect the ESP8266 module to your computer through USB cable as shown in the figure. When module is connected to the USB, COM port is detected eg: here COM5 is shown in the figure.

* Selecting COM Port

![The home page image is not found](https://github.com/cw-rashmi/AISCMM/blob/master/src/NodeMCU/pics/9.jpg)

### Include DallasTemperature and OneWire packages.

* Download the DallasTemperature-3.8.0.zip and OneWire-2.3.3.zip from the AISCMM/src/NodeMCU from github.
* In the Adruino IDE, traverse to Sketch>Include Library>Add .ZIP Library, browse to the location of the zip files and click okay.

### Upload the program to NodeMCU and check the output on the Serial Monitor

* to upload the program first click the verify button below the menu bar and the click the upload button to upload the code to NodeMCU.
* after 100% upload of the program the output can be monitored on the serial monitor.

### Connecting Temperature Sensor to the Node MCU

DS18B20 is used for measuring the soil temperature. It is one wire interface sensor, very common in PCB embedded electrical circuits. Its unique one wire protocol requires only one port pin for communication and needs no other external components to work. It is waterproof sensor hence it can be used in humid and wet environmental conditions. It can operate on 3.3v and 5.5v.
This sensor has 3 pins.
Vcc - 5V
Gnd - Gnd
Data

![The home page image is not found](https://github.com/cw-rashmi/AISCMM/blob/master/src/NodeMCU/pics/10.jpg)

### Connecting Moisture Sensor to the Node MCU
 
KG003 soil moisture sensor is used to measure the soil moisture. This is a simple water sensor that can be used to detect soil moisture. Module output is high level when the soil moisture deficit or output is low. The sensitivity can be adjusted by using the digital potentiometer. The output is available in both analog and digital format. In the project analog interfacing is used for accurate output. The soil moisture sensor consists of two probes. The two probes allow the current to pass through the soil and then it gets the resistance value to measure the moisture value. When there is more water, the soil will conduct more electricity which means that there will be less resistance. Therefore, the moisture level will be higher. Dry soil conducts electricity poorly, so when there will be less water, the soil will conduct less electricity which means that there will be more resistance. 

![The home page image is not found](https://github.com/cw-rashmi/AISCMM/blob/master/src/NodeMCU/pics/11.jpg)

