# mvg-project
Contains the unity project for the first AR-prototypes


### Setup-Steps:
* Install the UWP Build Pipeline for Unity

Runnig through Unity Editor:
* Install the [Holographic Remoting Player](https://apps.microsoft.com/store/detail/holographic-remoting-player/9NBLGGH4SV40?hl=en-us&gl=us)
app on the HoloLens
* Use Connect to the same network as your PC
* Note the remoting IP and port
* In Unity, under ```Mixed Reality/Remoting``` put the IP and port in


Deploying via USB:
* Connect HoloLens via USB-cable
* Build Unity project
* Go into the resulting build folder
* Open the visual Studio Solution there
* Set Mode to Release and Target to arm64 & Device
* Build > Build MRTK Tutorial
* Build > Deploy MRTK Tutorial
* Unplug and Replug HoloLens if not working?
