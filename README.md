# mvg-project
Contains the unity project for the first AR-prototypes


## Setup

### Runnig through Unity Editor:
* Install the [Holographic Remoting Player](https://apps.microsoft.com/store/detail/holographic-remoting-player/9NBLGGH4SV40?hl=en-us&gl=us)
app on the HoloLens
* You can connect the Hololens either via
  * WiFi: Connect to the same network as your PC 
  * USB: Disconnect the Hololens from any WiFi and connect it via the USB cable
* Start the Holographic Remoting App on the Hololens and note the shown IP
* In Unity, on the top bar, under ```Mixed Reality```>```Remoting``` insert this IP and hit the play button


### Deploying via USB:
* Install the UWP Build Pipeline for Unity
* Install Desktop UWP Development for Visual Studio
* Connect HoloLens via USB-cable
* Build Unity project
* Go into the resulting build folder
* Open the visual Studio Solution there
* Set Mode to Release and Target to arm64 & Device
* Build > Build MRTK Tutorial
* Build > Deploy MRTK Tutorial
* Unplug and Replug HoloLens if not working?
