How to setup the Fotobox:

- Build project with "dotnet publish -c Release -r win10-x64" in main folder
  -> // this is not needed anymore i guess ("dotnet publish -c Release"(required?))
- build https developer certificate for https: "dotnet dev-certs https" in main folder
- copy folder "Fotobox\bin\Release\netcoreapp2.2\win10-x64\publish" to NUC (pc)
- maybe generate https certificate again on NUC (see above)
- start "bin\Release\netcoreapp2.2\win10-x64\publish\Fotobox.exe" in copied folder

Server should be running on "http://localhost:51327/" (this is the website).
setup wlan with SSID "NETGEAR-Test" with default fotibox PW

The raspi is configured to call the api on "http:192.168.0.101:5000". Changed raspi config 01-apr-2021, ETP
The config can be changed with the application.properties file next to the jar in pi@<ip>: ~/Buzzer.
The ssh login for rasperrypi is pi@192.168.0.XX with default fotibox PW

Setup the static IP for the NUC which is configured in the raspi (192.168.0.101)

Disable Firewall on NUC resp. create incoming rule or add rule automatically to firewall when starting the exe the 1st time


Setup Router

- Rename Wlan to "NETGEAR-Test" (Whatsapp)
- Set password of Wlan (Whatsapp)
- DHCP -> address reservation: Bind Mac-address of NUC to 192.168.0.101 (see above, the raspi calls this ip (http request on NUC))
       -> address reservation for raspi is not needed (he only needs to know the ip of the NUC not vice versa)

When the NUC is set to factory settings (neu aufsetzen)
    -> right click desktop and start grafics application from intel
    select screen and on the left side choose "tv neu" or simmilar